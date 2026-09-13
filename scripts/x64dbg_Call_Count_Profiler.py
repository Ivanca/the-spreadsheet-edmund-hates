"""
x64dbg-automate Phase 1 Call Counter

Counts how many times each RVA is reached during a measurement interval.

Usage:

    python profile_calls.py functions.txt

    python profile_calls.py functions.txt --seconds 10

    python profile_calls.py functions.txt --seconds 30 --output results.csv

functions.txt format:

    # Comments are allowed
    0x204320
    0x5A3D0
    0xEF570
    0xE9AC0
    0xEB170

    # Decimal is also accepted
    204320

Important:
    RVAs are resolved against the currently loaded main module.

This script assumes x64dbg is already running and attached to the game.
It connects to the existing x64dbg Automate session rather than launching
a new debugging session.
"""

import argparse
import csv
import sys
import time
from datetime import datetime

from x64dbg_automate import X64DbgClient
from x64dbg_automate.models import BreakpointType


# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

DEFAULT_SECONDS = 10.0


# ---------------------------------------------------------------------------
# RVA parsing
# ---------------------------------------------------------------------------

def load_rvas(filename):
    """
    Load RVAs from a text file.

    Accepted formats:

        0x204320
        204320

    Blank lines and lines beginning with '#' are ignored.
    """

    rvas = []

    with open(filename, "r", encoding="utf-8") as f:
        for line_number, line in enumerate(f, 1):
            line = line.strip()

            if not line:
                continue

            if line.startswith("#"):
                continue

            # Allow inline comments:
            #
            # 0x204320    # UpdatePanelLayout
            #
            if "#" in line:
                line = line.split("#", 1)[0].strip()

            try:
                # int(..., 0) accepts both 0x123 and decimal.
                rva = int(line, 0)
            except ValueError:
                print(
                    f"[!] Invalid RVA on line {line_number}: {line!r}",
                    file=sys.stderr,
                )
                continue

            if rva < 0:
                print(
                    f"[!] Negative RVA on line {line_number}: {line!r}",
                    file=sys.stderr,
                )
                continue

            if rva not in rvas:
                rvas.append(rva)

    return rvas


# ---------------------------------------------------------------------------
# Module base
# ---------------------------------------------------------------------------

def get_main_module_base(client):
    """
    Get the base address of the main module.

    x64dbg's expression evaluator understands mod.main().
    """

    base, _ = client.eval_sync("mod.main()")

    if not base:
        raise RuntimeError("Could not determine main module base.")

    return base


# ---------------------------------------------------------------------------
# Breakpoint helpers
# ---------------------------------------------------------------------------

def breakpoint_name(rva):
    return f"Profiler_{rva:X}"


def set_profiler_breakpoint(client, address, rva):
    """
    Create a named software breakpoint.

    We use a name so that cleanup only removes our breakpoints.
    """

    name = breakpoint_name(rva)

    ok = client.set_breakpoint(
        address,
        name=name,
        singleshoot=False,
    )

    if not ok:
        raise RuntimeError(
            f"Failed to set breakpoint at 0x{address:X}"
        )

    return name


def get_breakpoint_by_name(client, name):
    """
    Find one of our breakpoints.
    """

    breakpoints = client.get_breakpoints(BreakpointType.BpNormal)

    for bp in breakpoints:
        if bp.name == name:
            return bp

    return None


def remove_profiler_breakpoints(client, rvas):
    """
    Remove only the breakpoints created by this script.
    """

    for rva in rvas:
        name = breakpoint_name(rva)

        try:
            client.clear_breakpoint(name)
        except Exception as e:
            print(
                f"[!] Could not clear breakpoint {name}: {e}",
                file=sys.stderr,
            )


# ---------------------------------------------------------------------------
# Measurement
# ---------------------------------------------------------------------------

def measure(client, rvas, seconds):
    """
    Measure breakpoint hit counts over the requested interval.

    The debugger is resumed normally during the measurement.

    Returns:

        {
            rva: {
                "address": ...,
                "calls": ...,
            }
        }
    """

    print()
    print(f"[+] Measuring for {seconds:.2f} seconds...")
    print("[+] Game is running normally.")
    print()

    # Clear any stale debugger events before starting.
    try:
        client.clear_debug_events()
    except Exception:
        pass

    # Make sure the process is running.
    client.go()

    start_time = time.perf_counter()

    # Sleep in small increments so Ctrl+C can be handled reasonably.
    try:
        while True:
            elapsed = time.perf_counter() - start_time

            if elapsed >= seconds:
                break

            time.sleep(min(0.1, seconds - elapsed))

    except KeyboardInterrupt:
        print()
        print("[!] Measurement interrupted.")

    # Stop the game.
    client.pause()

    # Give x64dbg a moment to process the pause.
    time.sleep(0.1)

    actual_seconds = time.perf_counter() - start_time

    results = {}

    for rva in rvas:
        name = breakpoint_name(rva)
        bp = get_breakpoint_by_name(client, name)

        if bp is None:
            print(
                f"[!] Breakpoint disappeared: {name}",
                file=sys.stderr,
            )

            results[rva] = {
                "address": None,
                "calls": 0,
                "calls_per_second": 0.0,
            }

            continue

        calls = bp.hitCount

        results[rva] = {
            "address": bp.addr,
            "calls": calls,
            "calls_per_second": (
                calls / actual_seconds
                if actual_seconds > 0
                else 0.0
            ),
        }

    return results, actual_seconds


# ---------------------------------------------------------------------------
# Output
# ---------------------------------------------------------------------------

def print_results(results, actual_seconds):
    print()
    print("=" * 78)
    print("CALL COUNT RESULTS")
    print("=" * 78)

    print(
        f"{'RVA':>12}  "
        f"{'Address':>18}  "
        f"{'Calls':>12}  "
        f"{'Calls/sec':>14}"
    )

    print("-" * 78)

    # Sort by total calls, highest first.
    sorted_results = sorted(
        results.items(),
        key=lambda item: item[1]["calls"],
        reverse=True,
    )

    for rva, data in sorted_results:
        address = data["address"]

        address_text = (
            f"0x{address:X}"
            if address is not None
            else "N/A"
        )

        print(
            f"0x{rva:08X}  "
            f"{address_text:>18}  "
            f"{data['calls']:>12,}  "
            f"{data['calls_per_second']:>14,.2f}"
        )

    print("-" * 78)
    print(f"Measurement time: {actual_seconds:.3f} seconds")
    print("=" * 78)


def write_csv(filename, results, actual_seconds):
    timestamp = datetime.now().isoformat(timespec="seconds")

    with open(filename, "w", newline="", encoding="utf-8") as f:
        writer = csv.writer(f)

        writer.writerow([
            "timestamp",
            "measurement_seconds",
            "rva",
            "address",
            "calls",
            "calls_per_second",
        ])

        for rva, data in sorted(
            results.items(),
            key=lambda item: item[1]["calls"],
            reverse=True,
        ):
            writer.writerow([
                timestamp,
                f"{actual_seconds:.6f}",
                f"0x{rva:X}",
                (
                    f"0x{data['address']:X}"
                    if data["address"] is not None
                    else ""
                ),
                data["calls"],
                f"{data['calls_per_second']:.6f}",
            ])

    print(f"[+] Results written to: {filename}")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser(
        description="Count calls to RVA-based functions using x64dbg."
    )

    parser.add_argument(
        "rva_file",
        help="Text file containing function RVAs.",
    )

    parser.add_argument(
        "--seconds",
        type=float,
        default=DEFAULT_SECONDS,
        help=f"Measurement duration (default: {DEFAULT_SECONDS}).",
    )

    parser.add_argument(
        "--output",
        "-o",
        help="Optional CSV output filename.",
    )

    args = parser.parse_args()

    if args.seconds <= 0:
        parser.error("--seconds must be greater than zero.")

    # ---------------------------------------------------------------
    # Load RVAs
    # ---------------------------------------------------------------

    rvas = load_rvas(args.rva_file)

    if not rvas:
        print("[!] No valid RVAs found.")
        return 1

    print(f"[+] Loaded {len(rvas)} RVAs.")

    # ---------------------------------------------------------------
    # Connect to existing x64dbg session
    # ---------------------------------------------------------------

    print("[+] Connecting to x64dbg...")

    try:
        client = X64DbgClient.connect_remote(
            host="127.0.0.1",
            req_rep_port=27066,
            pub_sub_port=27067,
        )

    except Exception as e:
        print(f"[!] Could not connect to x64dbg: {e}")
        return 1

    # ---------------------------------------------------------------
    # Resolve module base
    # ---------------------------------------------------------------

    try:
        module_base = get_main_module_base(client)

    except Exception as e:
        print(f"[!] Could not resolve main module: {e}")
        return 1

    print(f"[+] Main module base: 0x{module_base:X}")

    # ---------------------------------------------------------------
    # Resolve RVAs
    # ---------------------------------------------------------------

    addresses = {}

    for rva in rvas:
        address = module_base + rva
        addresses[rva] = address

        print(
            f"    RVA 0x{rva:X} -> 0x{address:X}"
        )

    # ---------------------------------------------------------------
    # Install breakpoints
    # ---------------------------------------------------------------

    print()
    print("[+] Installing profiler breakpoints...")

    installed = []

    try:
        for rva in rvas:
            address = addresses[rva]

            # Check whether this address already has a breakpoint.
            #
            # We deliberately don't blindly overwrite an existing
            # breakpoint belonging to the user.
            existing = None

            for bp in client.get_breakpoints(BreakpointType.BpNormal):
                if bp.addr == address:
                    existing = bp
                    break

            if existing is not None:
                print(
                    f"[!] Breakpoint already exists at "
                    f"0x{address:X} ({existing.name}); skipping."
                )
                continue

            set_profiler_breakpoint(
                client,
                address,
                rva,
            )

            installed.append(rva)

            print(
                f"    + 0x{rva:X} "
                f"(0x{address:X})"
            )

    except Exception as e:
        print(f"[!] Failed installing breakpoints: {e}")

        remove_profiler_breakpoints(client, installed)
        return 1

    if not installed:
        print("[!] No profiler breakpoints were installed.")
        return 1

    # ---------------------------------------------------------------
    # Measurement
    # ---------------------------------------------------------------

    try:
        results, actual_seconds = measure(
            client,
            installed,
            args.seconds,
        )

        print_results(
            results,
            actual_seconds,
        )

        if args.output:
            write_csv(
                args.output,
                results,
                actual_seconds,
            )

    finally:
        # -----------------------------------------------------------
        # Cleanup
        # -----------------------------------------------------------

        print()
        print("[+] Removing profiler breakpoints...")

        remove_profiler_breakpoints(
            client,
            installed,
        )

        print("[+] Done.")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
