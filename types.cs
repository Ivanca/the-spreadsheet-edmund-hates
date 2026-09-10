using System.Runtime.InteropServices;

namespace TheSpredsheetEdmundHates;

/// <summary>
/// Inferred layout of the game's MovieClip object.
/// Only fields observed directly in the mod are represented.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct MovieClip
{
    // Define explicit or implicit cast to nint
    public static explicit operator nint(MovieClip value)
    {
        // Return the specific property or calculation you want
        return value.self; 
    }
    [FieldOffset(0x00)] public nint self;
    [FieldOffset(0x08)] public nint unknown2;

    // Current display-list depth.
    [FieldOffset(0x0C)] public uint Depth;

    [FieldOffset(0x10)] public nint unknown3;
    [FieldOffset(0x18)] public nint unknown4;
    [FieldOffset(0x20)] public nint unknown5;
    [FieldOffset(0x28)] public nint unknown6;
    [FieldOffset(0x30)] public nint unknown7;

    // Parent MovieClip.
    [FieldOffset(0x38)] public nint Parent;

    // Renderer associated with this MovieClip.
    [FieldOffset(0x40)] public nint Renderer;

    // Pointer to the object's name / C-string.
    [FieldOffset(0x48)] public nint Name;

    [FieldOffset(0x50)] public nint unknown8;
    [FieldOffset(0x58)] public nint unknown9;
    [FieldOffset(0x60)] public nint unknown10;
    [FieldOffset(0x68)] public nint unknown11;
    [FieldOffset(0x70)] public nint unknown12;
    [FieldOffset(0x78)] public nint unknown13;
    [FieldOffset(0x80)] public nint unknown14;
    [FieldOffset(0x88)] public nint unknown15;
    [FieldOffset(0x90)] public nint unknown16;
    [FieldOffset(0x98)] public nint unknown17;
    [FieldOffset(0xA0)] public nint unknown18;
    [FieldOffset(0xA8)] public nint unknown19;
    [FieldOffset(0xB0)] public nint unknown20;
    [FieldOffset(0xB8)] public nint unknown21;
    [FieldOffset(0xC0)] public nint unknown22;
    [FieldOffset(0xC8)] public nint unknown23;
    [FieldOffset(0xD0)] public nint Runtime;
}


/// <summary>
/// Inferred layout of a UI renderer.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
struct Renderer
{
    [FieldOffset(0x00)] public nint self;
    [FieldOffset(0x08)] public nint unknown2;
    [FieldOffset(0x10)] public nint unknown3;
    [FieldOffset(0x18)] public nint Entity;
    [FieldOffset(0x20)] public nint unknown5;
    [FieldOffset(0x28)] public nint unknown6;
    [FieldOffset(0x30)] public nint unknown7;
    [FieldOffset(0x38)] public nint unknown8;

    // Transform used to position the renderer.
    [FieldOffset(0x40)] public nint Transform;

    [FieldOffset(0x48)] public nint unknown9;

    // Renderer state / flags.
    // The code writes 0x0000002400000101 here.
    [FieldOffset(0x50)] public ulong Flags;

    // The code separately writes a byte at +0x51.
    // This overlaps State and therefore should not be considered
    // an independent field until the native layout is confirmed.
    [FieldOffset(0x51)] public byte Visible;

    [FieldOffset(0x58)] public nint unknown10;
    [FieldOffset(0x60)] public nint unknown11;
    [FieldOffset(0x68)] public nint unknown12;
    [FieldOffset(0x70)] public nint unknown13;
    [FieldOffset(0x78)] public nint unknown14;

    // Root MovieClip.
    [FieldOffset(0x80)] public nint MovieClip;

    [FieldOffset(0x88)] public nint unknown15;
    [FieldOffset(0x90)] public nint unknown16;
    [FieldOffset(0x98)] public nint unknown17;
    [FieldOffset(0xA0)] public nint unknown18;

    // MSVC std::string containing the renderer name.
    // For example: "CatMenu".
    [FieldOffset(0xA8)] public StdString Name;

}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct StdString
{
    [FieldOffset(0x00)]
    public nint HeapPointer;

    [FieldOffset(0x00)]
    public fixed byte InlineBuffer[16];

    [FieldOffset(0x10)]
    public ulong Size;

    [FieldOffset(0x18)]
    public ulong Capacity;

    public bool IsInline => Capacity <= 15;

    public byte* Data
    {
        get
        {
            fixed (byte* inline = InlineBuffer)
                return IsInline ? inline : (byte*)HeapPointer;
        }
    }

    public string Value
    {
        get => Marshal.PtrToStringAnsi((nint)Data, checked((int)Size)) ?? "";
    }

    public static implicit operator string(StdString value)
    {
        return value.Value;
    }
}