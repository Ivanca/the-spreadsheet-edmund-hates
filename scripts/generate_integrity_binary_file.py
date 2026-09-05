import re
import struct
from pathlib import Path


# ============================================================
# Configuration
# ============================================================

SOURCE_FILES = [
    Path("../CatstableMod.cs"),
    Path("../Autoinjector.cs"),
]

BINARY_PATH = Path(r"E:\SteamLibrary\steamapps\common\Mewgenics\Mewgenics.exe")
OUTPUT_PATH = Path("../expectedBinaryData.cs")

BYTES_PER_RVA = 64

# ============================================================
# PE parsing
# ============================================================

class PESection:
    def __init__(
        self,
        name: str,
        virtual_size: int,
        virtual_address: int,
        raw_size: int,
        raw_pointer: int,
    ):
        self.name = name
        self.virtual_size = virtual_size
        self.virtual_address = virtual_address
        self.raw_size = raw_size
        self.raw_pointer = raw_pointer

    def __repr__(self) -> str:
        return (
            f"PESection("
            f"name={self.name!r}, "
            f"VA=0x{self.virtual_address:X}, "
            f"VS=0x{self.virtual_size:X}, "
            f"raw=0x{self.raw_pointer:X}, "
            f"raw_size=0x{self.raw_size:X}"
            f")"
        )


class PEFile:
    def __init__(self, data: bytes):
        self.data = data
        self.sections: list[PESection] = []

        self.image_base = 0
        self.is_64_bit = False

        self._parse()

    def _parse(self) -> None:
        data = self.data

        # ----------------------------------------------------
        # DOS header
        # ----------------------------------------------------

        if len(data) < 0x40:
            raise ValueError("File is too small to be a PE file.")

        if data[0:2] != b"MZ":
            raise ValueError("File does not have a valid MZ header.")

        # e_lfanew is at offset 0x3C.
        pe_offset = struct.unpack_from("<I", data, 0x3C)[0]

        if pe_offset + 4 > len(data):
            raise ValueError("Invalid PE header offset.")

        # ----------------------------------------------------
        # PE signature
        # ----------------------------------------------------

        if data[pe_offset:pe_offset + 4] != b"PE\0\0":
            raise ValueError("Invalid PE signature.")

        file_header_offset = pe_offset + 4

        # IMAGE_FILE_HEADER
        #
        # Offset 0:
        #   Machine          WORD
        #   NumberOfSections WORD
        #   TimeDateStamp    DWORD
        #   PointerToSymbolTable DWORD
        #   NumberOfSymbols  DWORD
        #   SizeOfOptionalHeader WORD
        #   Characteristics WORD

        (
            machine,
            number_of_sections,
            _timestamp,
            _symbol_table,
            _symbols,
            optional_header_size,
            _characteristics,
        ) = struct.unpack_from(
            "<HHIIIHH",
            data,
            file_header_offset,
        )

        optional_header_offset = file_header_offset + 20

        # ----------------------------------------------------
        # Optional header
        # ----------------------------------------------------

        magic = struct.unpack_from(
            "<H",
            data,
            optional_header_offset,
        )[0]

        if magic == 0x20B:
            # PE32+
            self.is_64_bit = True

            # ImageBase is at +24 for PE32+.
            self.image_base = struct.unpack_from(
                "<Q",
                data,
                optional_header_offset + 24,
            )[0]

        elif magic == 0x10B:
            # PE32
            self.is_64_bit = False

            # ImageBase is at +28 for PE32.
            self.image_base = struct.unpack_from(
                "<I",
                data,
                optional_header_offset + 28,
            )[0]

        else:
            raise ValueError(
                f"Unknown PE optional-header magic: 0x{magic:X}"
            )

        # ----------------------------------------------------
        # Section table
        # ----------------------------------------------------

        section_table_offset = (
            optional_header_offset + optional_header_size
        )

        section_size = 40

        for i in range(number_of_sections):
            offset = section_table_offset + i * section_size

            if offset + section_size > len(data):
                raise ValueError(
                    "Section table extends beyond the end of the file."
                )

            raw_name = data[offset:offset + 8]

            name = raw_name.split(b"\0", 1)[0].decode(
                "ascii",
                errors="replace",
            )

            # IMAGE_SECTION_HEADER:
            #
            # +00 Name
            # +08 VirtualSize
            # +0C VirtualAddress
            # +10 SizeOfRawData
            # +14 PointerToRawData

            virtual_size = struct.unpack_from(
                "<I",
                data,
                offset + 8,
            )[0]

            virtual_address = struct.unpack_from(
                "<I",
                data,
                offset + 12,
            )[0]

            raw_size = struct.unpack_from(
                "<I",
                data,
                offset + 16,
            )[0]

            raw_pointer = struct.unpack_from(
                "<I",
                data,
                offset + 20,
            )[0]

            section = PESection(
                name=name,
                virtual_size=virtual_size,
                virtual_address=virtual_address,
                raw_size=raw_size,
                raw_pointer=raw_pointer,
            )

            self.sections.append(section)

    def rva_to_file_offset(self, rva: int) -> int | None:
        """
        Convert a PE RVA to its corresponding raw file offset.

        Returns None if the RVA does not belong to a section.
        """

        for section in self.sections:
            section_start = section.virtual_address

            # For mapping an RVA, the section's virtual extent is
            # normally based on VirtualSize. Using the larger of
            # VirtualSize and SizeOfRawData also handles sections
            # whose raw representation is larger.
            section_size = max(
                section.virtual_size,
                section.raw_size,
            )

            section_end = section_start + section_size

            if section_start <= rva < section_end:
                delta = rva - section_start
                file_offset = section.raw_pointer + delta

                if file_offset >= len(self.data):
                    return None

                return file_offset

        return None

    def describe_rva(self, rva: int) -> tuple[PESection, int] | None:
        """
        Return (section, file_offset) for an RVA.
        """

        file_offset = self.rva_to_file_offset(rva)

        if file_offset is None:
            return None

        for section in self.sections:
            section_start = section.virtual_address
            section_size = max(
                section.virtual_size,
                section.raw_size,
            )

            if section_start <= rva < section_start + section_size:
                return section, file_offset

        return None


# ============================================================
# Find hexadecimal literals in C# files
# ============================================================

def find_rvas(files: list[Path]) -> list[int]:
    """
    Find hexadecimal literals such as:

        0x1234
        0x12345678
        0XABCDEF

    Only values > 0xFFF are kept.

    Duplicate values are removed.
    """

    pattern = re.compile(
        r"(?<![A-Za-z0-9_])0[xX][0-9A-Fa-f]+"
    )

    rvas: set[int] = set()

    for file_path in files:
        if not file_path.exists():
            print(f"WARNING: File not found: {file_path}")
            continue

        print(f"Scanning: {file_path}")

        text = file_path.read_text(
            encoding="utf-8",
            errors="replace",
        )

        for match in pattern.finditer(text):
            value = int(match.group(0), 16)

            if value > 0xFFF:
                rvas.add(value)

    return sorted(rvas)


# ============================================================
# C# generation
# ============================================================

def generate_csharp(data: dict[int, bytes]) -> str:
    lines = [
        "using System.Collections.Generic;",
        "namespace CatsTableMod;",
        "public static class BinaryLiterals",
        "{",
        "    // The dictionary stores the lightweight wrapper struct containing the data inline",
        "    public static readonly Dictionary<int, ByteKey64> Data = new()",
        "    {",
    ]

    for rva, byte_data in data.items():
        lines.append("        {")
        lines.append(
            f"            0x{rva:X}, new ByteKey64(new byte[64] {{"
        )

        # 8 bytes per line.
        for i in range(0, len(byte_data), 8):
            chunk = byte_data[i:i + 8]

            byte_string = ", ".join(
                f"0x{b:02X}"
                for b in chunk
            )

            lines.append(
                f"                {byte_string},"
            )

        lines.append("            })")
        lines.append("        },")

    lines.extend([
        "    };",
        "}",
        "",
    ])

    return "\n".join(lines)


# ============================================================
# Main
# ============================================================

def main() -> None:
    print("=" * 70)
    print("Mewgenics RVA Binary Data Generator")
    print("=" * 70)

    # --------------------------------------------------------
    # Find RVAs
    # --------------------------------------------------------

    print("\n[1/4] Scanning source files...")

    rvas = find_rvas(SOURCE_FILES)

    if not rvas:
        print("\nNo hexadecimal values greater than 0xFFF found.")
        return

    print(f"\nFound {len(rvas)} unique RVA(s).")

    # --------------------------------------------------------
    # Read executable
    # --------------------------------------------------------

    print("\n[2/4] Loading executable...")

    if not BINARY_PATH.exists():
        raise FileNotFoundError(
            f"Binary not found:\n{BINARY_PATH}"
        )

    binary = BINARY_PATH.read_bytes()

    print(f"Binary: {BINARY_PATH}")
    print(f"Size:   0x{len(binary):X} ({len(binary):,} bytes)")

    # --------------------------------------------------------
    # Parse PE
    # --------------------------------------------------------

    print("\n[3/4] Parsing PE headers...")

    pe = PEFile(binary)

    print(
        f"Architecture: "
        f"{'PE32+' if pe.is_64_bit else 'PE32'}"
    )

    print(f"Image base: 0x{pe.image_base:X}")

    print(f"\nSections ({len(pe.sections)}):")

    for section in pe.sections:
        print(
            f"  {section.name:<10} "
            f"RVA=0x{section.virtual_address:08X} "
            f"VS=0x{section.virtual_size:08X} "
            f"RAW=0x{section.raw_pointer:08X} "
            f"RAW_SIZE=0x{section.raw_size:08X}"
        )

    # --------------------------------------------------------
    # Convert RVAs and extract data
    # --------------------------------------------------------

    print("\nResolving RVAs...")

    extracted: dict[int, bytes] = {}

    failed = 0

    for rva in rvas:
        result = pe.describe_rva(rva)

        if result is None:
            print(
                f"  WARNING: 0x{rva:X} -> "
                f"could not map RVA to a PE section"
            )
            failed += 1
            continue

        section, file_offset = result

        end_offset = file_offset + BYTES_PER_RVA

        # Make sure the requested bytes actually exist in the
        # physical file.
        if end_offset > len(binary):
            print(
                f"  WARNING: 0x{rva:X} -> "
                f"file offset 0x{file_offset:X}, "
                f"not enough bytes remaining"
            )
            failed += 1
            continue

        # Also make sure the requested data does not extend
        # outside the section's raw data.
        section_raw_end = (
            section.raw_pointer + section.raw_size
        )

        if end_offset > section_raw_end:
            print(
                f"  WARNING: 0x{rva:X} -> "
                f"64 bytes extend beyond raw section data "
                f"({section.name})"
            )
            failed += 1
            continue

        byte_data = binary[file_offset:end_offset]

        extracted[rva] = byte_data

        print(
            f"  0x{rva:08X} -> "
            f"{section.name:<8} "
            f"file offset 0x{file_offset:08X}"
        )

    # --------------------------------------------------------
    # Generate output
    # --------------------------------------------------------

    print("\n[4/4] Generating C#...")

    output = generate_csharp(extracted)

    OUTPUT_PATH.write_text(
        output,
        encoding="utf-8",
    )

    print(f"\nCreated: {OUTPUT_PATH}")
    print(f"Entries written: {len(extracted)}")
    print(f"Entries skipped: {failed}")

    print("\nDone.")


if __name__ == "__main__":
    main()