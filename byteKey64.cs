
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size = 64)]
public readonly struct ByteKey64
{
    // Anchor field to map the start of the 64-byte block
    [FieldOffset(0)] private readonly byte _firstByte;

    public ByteKey64(ReadOnlySpan<byte> source)
    {
        if (source.Length != 64)
            throw new ArgumentException("Source must be exactly 64 bytes.");

        // Copy directly from the source span into this struct's memory
        Unsafe.CopyBlock(
            ref Unsafe.AsRef(in _firstByte), 
            ref MemoryMarshal.GetReference(source), 
            64
        );
    }

    // Safely read the 64 bytes back out without copying memory
    public ReadOnlySpan<byte> AsSpan()
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _firstByte), 64);
    }
}
