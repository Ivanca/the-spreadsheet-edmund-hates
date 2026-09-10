using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class BinaryValidator
{
    private const int BytesPerEntry = 64;

    public static bool Validate(string binaryPath)
    {
        if (!File.Exists(binaryPath))
            return false;

        try
        {
            using FileStream stream = new FileStream(
                binaryPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                options: FileOptions.SequentialScan
            );

            PEFile pe = new PEFile(stream);

            // Only allocate the 64-byte buffer once.
            byte[] actualBuffer = new byte[BytesPerEntry];

            foreach (var entry in TheSpredsheetEdmundHates.BinaryLiterals.Data)
            {
                uint rva = unchecked((uint)entry.Key);

                if (!pe.TryRvaToFileOffset(
                        rva,
                        out long fileOffset))
                {
                    TheSpredsheetEdmundHates.MewjectorApi.Log($"Failed to map RVA to file offset: {rva:X}");
                    return false;
                }

                stream.Position = fileOffset;

                int totalRead = 0;

                while (totalRead < BytesPerEntry)
                {
                    int read = stream.Read(
                        actualBuffer,
                        totalRead,
                        BytesPerEntry - totalRead
                    );

                    if (read <= 0)
                    {
                        TheSpredsheetEdmundHates.MewjectorApi.Log($"Failed to read expected number of bytes at RVA: {rva:X}");
                        return false;
                    }

                    totalRead += read;
                }

                ReadOnlySpan<byte> expected =
                    entry.Value.AsSpan();

                for (int i = 0; i < BytesPerEntry; i++)
                {
                    if (actualBuffer[i] != expected[i])
                    {
                        TheSpredsheetEdmundHates.MewjectorApi.Log($"Binary validation failed at RVA: {rva:X}");
                        return false;
                    }
                }
            }

            return true;
        }
        catch
        {
            TheSpredsheetEdmundHates.MewjectorApi.Log("Exception occurred during binary validation.");
            return false;
        }
    }


    // ========================================================
    // PE parser
    // ========================================================

    private sealed class PEFile
    {
        private readonly FileStream _stream;

        public List<PESection> Sections { get; } = new();


        public PEFile(FileStream stream)
        {
            _stream = stream;

            Parse();
        }


        private void Parse()
        {
            // ------------------------------------------------
            // DOS header
            // ------------------------------------------------

            byte[] dosHeader = new byte[64];

            _stream.Position = 0;

            ReadExactly(
                dosHeader,
                0,
                dosHeader.Length
            );

            if (ReadUInt16(dosHeader, 0) != 0x5A4D)
                throw new InvalidDataException(
                    "Invalid MZ header."
                );

            int peOffset = checked(
                (int)ReadUInt32(dosHeader, 0x3C)
            );


            // ------------------------------------------------
            // PE signature + IMAGE_FILE_HEADER
            // ------------------------------------------------

            byte[] peHeader = new byte[24];

            _stream.Position = peOffset;

            ReadExactly(
                peHeader,
                0,
                peHeader.Length
            );

            if (ReadUInt32(peHeader, 0) != 0x00004550)
                throw new InvalidDataException(
                    "Invalid PE signature."
                );

            ushort numberOfSections =
                ReadUInt16(peHeader, 6);

            ushort optionalHeaderSize =
                ReadUInt16(peHeader, 20);


            // ------------------------------------------------
            // Optional header
            // ------------------------------------------------

            byte[] optionalHeader =
                new byte[optionalHeaderSize];

            ReadExactly(
                optionalHeader,
                0,
                optionalHeader.Length
            );

            ushort magic =
                ReadUInt16(optionalHeader, 0);

            if (magic != 0x10B &&
                magic != 0x20B)
            {
                throw new InvalidDataException(
                    "Unknown PE optional header."
                );
            }


            // ------------------------------------------------
            // Section table
            // ------------------------------------------------

            byte[] sectionHeader = new byte[40];

            for (int i = 0; i < numberOfSections; i++)
            {
                ReadExactly(
                    sectionHeader,
                    0,
                    sectionHeader.Length
                );

                string name = Encoding.ASCII.GetString(
                    sectionHeader,
                    0,
                    8
                ).TrimEnd('\0');

                uint virtualSize =
                    ReadUInt32(sectionHeader, 8);

                uint virtualAddress =
                    ReadUInt32(sectionHeader, 12);

                uint rawSize =
                    ReadUInt32(sectionHeader, 16);

                uint rawPointer =
                    ReadUInt32(sectionHeader, 20);

                Sections.Add(
                    new PESection(
                        name,
                        virtualAddress,
                        virtualSize,
                        rawPointer,
                        rawSize
                    )
                );
            }
        }


        public bool TryRvaToFileOffset(
            uint rva,
            out long fileOffset)
        {
            foreach (PESection section in Sections)
            {
                ulong start = section.VirtualAddress;

                ulong size = Math.Max(
                    section.VirtualSize,
                    section.RawSize
                );

                ulong end = start + size;

                if (rva < start || rva >= end)
                    continue;

                ulong delta =
                    (ulong)rva - start;

                // An RVA can fall inside the virtual part of a
                // section without having bytes in the file.
                if (delta >= section.RawSize)
                {
                    fileOffset = -1;
                    return false;
                }

                fileOffset =
                    (long)section.RawPointer +
                    (long)delta;

                return true;
            }

            fileOffset = -1;
            return false;
        }


        private void ReadExactly(
            byte[] buffer,
            int offset,
            int count)
        {
            int totalRead = 0;

            while (totalRead < count)
            {
                int read = _stream.Read(
                    buffer,
                    offset + totalRead,
                    count - totalRead
                );

                if (read <= 0)
                    throw new EndOfStreamException();

                totalRead += read;
            }
        }


        private static ushort ReadUInt16(
            byte[] buffer,
            int offset)
        {
            return (ushort)(
                buffer[offset] |
                (buffer[offset + 1] << 8)
            );
        }


        private static uint ReadUInt32(
            byte[] buffer,
            int offset)
        {
            return
                (uint)buffer[offset] |
                ((uint)buffer[offset + 1] << 8) |
                ((uint)buffer[offset + 2] << 16) |
                ((uint)buffer[offset + 3] << 24);
        }
    }


    // ========================================================
    // PE Section
    // ========================================================

    private sealed class PESection
    {
        public string Name { get; }

        public uint VirtualAddress { get; }

        public uint VirtualSize { get; }

        public uint RawPointer { get; }

        public uint RawSize { get; }


        public PESection(
            string name,
            uint virtualAddress,
            uint virtualSize,
            uint rawPointer,
            uint rawSize)
        {
            Name = name;
            VirtualAddress = virtualAddress;
            VirtualSize = virtualSize;
            RawPointer = rawPointer;
            RawSize = rawSize;
        }
    }
}