import sys
import struct

from x64dbg_automate import X64DbgClient
from x64dbg_automate.events import EventType


def parse_addresses(filename):
    """
    Extract addresses only from lines in the form:

        Address=00000277F107BF80
        Disassembly=...

    Returns a list of integer addresses.
    """
    addresses = []

    with open(filename, "r", encoding="utf-8") as f:
        for line_number, line in enumerate(f, 1):
            line = line.strip()

            if not line.startswith("Address="):
                continue

            value = line[len("Address="):].strip()

            try:
                addresses.append(int(value, 16))
            except ValueError:
                print(
                    f"[!] Invalid address on line {line_number}: {value}",
                    file=sys.stderr
                )

    return addresses


def parse_int(value, name):
    """
    Parse an integer in decimal or hexadecimal form.

    Examples:
        123
        0x123
        123ABC
    """
    value = value.strip()

    try:
        return int(value, 0)
    except ValueError:
        try:
            return int(value, 16)
        except ValueError:
            raise ValueError(f"Invalid {name}: {value}")


def main():
    if len(sys.argv) != 4:
        print(
            "Usage:\n"
            "  python bulk_patcher.py "
            "<address_file> <qword_value> <breakpoint_rva>\n\n"
            "Example:\n"
            "  python bulk_patcher.py addresses.txt "
            "0x123456789ABCDEF0 0x123456"
        )
        return 1

    address_file = sys.argv[1]

    try:
        qword_value = parse_int(sys.argv[2], "QWORD value")
        breakpoint_rva = parse_int(
            sys.argv[3],
            "breakpoint RVA"
        )
    except ValueError as e:
        print(f"[!] {e}", file=sys.stderr)
        return 1

    if not 0 <= qword_value <= 0xFFFFFFFFFFFFFFFF:
        print(
            "[!] QWORD value must be between "
            "0x0000000000000000 and 0xFFFFFFFFFFFFFFFF"
        )
        return 1

    if breakpoint_rva < 0:
        print("[!] Breakpoint RVA cannot be negative.")
        return 1

    try:
        addresses = parse_addresses(address_file)
    except OSError as e:
        print(f"[!] Could not read address file: {e}", file=sys.stderr)
        return 1

    if not addresses:
        print("[!] No Address= entries were found.")
        return 1

    print(f"[+] Loaded {len(addresses)} addresses.")
    print(f"[+] QWORD value:    0x{qword_value:016X}")
    print(f"[+] Breakpoint RVA: 0x{breakpoint_rva:X}")

    # QWORD is written as 8 bytes, little-endian.
    qword_bytes = struct.pack("<Q", qword_value)

    print()
    print("[+] Connecting to x64dbg...")

    client = X64DbgClient.connect_remote(
        host="127.0.0.1",
        req_rep_port=27066,
        pub_sub_port=27067,
    )

    breakpoint_address = None

    try:
        # -------------------------------------------------------------
        # Resolve the breakpoint RVA against the currently open module.
        # -------------------------------------------------------------

        module_base, success = client.eval_sync("mod.main()")

        if module_base is None:
            print("[!] Could not determine the current module base.")
            return 1

        breakpoint_address = module_base + breakpoint_rva

        print(
            f"[+] Module base:    0x{module_base:016X}"
        )
        print(
            f"[+] Breakpoint addr: 0x{breakpoint_address:016X}"
        )

        # Remove stale breakpoint events.
        client.clear_debug_events(
            EventType.EVENT_BREAKPOINT
        )

        # Create the breakpoint.
        if not client.set_breakpoint(breakpoint_address):
            print(
                f"[!] Failed to set breakpoint at "
                f"0x{breakpoint_address:016X}"
            )
            return 1

        # Initially disable it.
        if not client.toggle_breakpoint(
            breakpoint_address,
            on=False
        ):
            print("[!] Failed to disable breakpoint.")
            return 1

        print("[+] Breakpoint created.")

        # -------------------------------------------------------------
        # Enable breakpoint and resume the program.
        # -------------------------------------------------------------

        if not client.toggle_breakpoint(
            breakpoint_address,
            on=True
        ):
            print("[!] Failed to enable breakpoint.")
            return 1

        print(
            f"[+] Breakpoint enabled at "
            f"0x{breakpoint_address:016X}"
        )

        # if not client.go():
        #     print("[!] Failed to resume the debuggee.")
        #     return 1

        print("[*] Waiting for breakpoint...")

        # -------------------------------------------------------------
        # Wait until the breakpoint is hit.
        # -------------------------------------------------------------

        event = client.wait_for_debug_event(
            EventType.EVENT_BREAKPOINT,
            timeout=1
        )

        while event is None:

            if not client.is_debugging():
                print("[!] Debugging session ended.")
                return 0

            event = client.wait_for_debug_event(
                EventType.EVENT_BREAKPOINT,
                timeout=1
            )

        print(
            f"[+] Breakpoint hit at "
            f"0x{event.event_data.addr:016X}"
        )

        # Make sure the debugger is stopped before writing memory.
        if not client.wait_until_stopped(timeout=5):
            print("[!] Debugger did not reach stopped state.")
            return 1

        # -------------------------------------------------------------
        # Write the QWORD to EVERY address.
        # -------------------------------------------------------------

        print()
        print(
            f"[*] Writing 0x{qword_value:016X} "
            f"to {len(addresses)} addresses..."
        )

        successful = 0
        failed = 0

        for address in addresses:
            if client.write_memory(
                address,
                qword_bytes
            ):
                print(
                    f"    [+] 0x{address:016X} "
                    f"<- 0x{qword_value:016X}"
                )
                successful += 1
            else:
                print(
                    f"    [!] Failed: 0x{address:016X}"
                )
                failed += 1

        print()
        print(
            f"[+] Completed: {successful} successful, "
            f"{failed} failed."
        )

        # -------------------------------------------------------------
        # Disable breakpoint and resume execution.
        # -------------------------------------------------------------

        if not client.toggle_breakpoint(
            breakpoint_address,
            on=False
        ):
            print("[!] Failed to disable breakpoint.")
            return 1

        print("[+] Breakpoint disabled.")

        if not client.go():
            print("[!] Failed to resume the debuggee.")
            return 1

        print("[+] Program resumed.")

    except KeyboardInterrupt:
        print("\n[+] Interrupted by user.")

    except Exception as e:
        print(f"\n[!] Error: {e}")
        raise

    finally:
        # Ensure the breakpoint isn't left enabled if the script
        # exits unexpectedly.
        if breakpoint_address is not None:
            try:
                if client.is_debugging():
                    client.toggle_breakpoint(
                        breakpoint_address,
                        on=False
                    )
            except Exception:
                pass

    return 0


if __name__ == "__main__":
    sys.exit(main())
