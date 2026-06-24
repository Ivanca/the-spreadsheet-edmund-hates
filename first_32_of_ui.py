#!/usr/bin/env python3
import sys

if len(sys.argv) != 2:
    print(f"Usage: {sys.argv[0]} <file>")
    sys.exit(1)

filename = sys.argv[1]

with open(filename, "rb") as f:
    f.seek(0, 2)  # seek to end
    size = f.tell()

    # Middle of file, rounded up if not exact
    middle = (size + 1) // 4

    f.seek(middle)
    data = f.read(32)

print(data.hex())