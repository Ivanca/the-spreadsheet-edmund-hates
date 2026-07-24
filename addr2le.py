#!/usr/bin/env python3

import sys

def main():
    if len(sys.argv) != 2:
        print(f"Usage: {sys.argv[0]} <64-bit address>")
        print(f"Example: {sys.argv[0]} 00000231F4D71898")
        sys.exit(1)

    addr_str = sys.argv[1].lower().replace("0x", "")

    try:
        addr = int(addr_str, 16)
    except ValueError:
        print("Error: Invalid hexadecimal address.")
        sys.exit(1)

    if not (0 <= addr <= 0xFFFFFFFFFFFFFFFF):
        print("Error: Address must fit in 64 bits.")
        sys.exit(1)

    little_endian = addr.to_bytes(8, byteorder="little").hex().upper()

    print(little_endian)

if __name__ == "__main__":
    main()