

using System;
using System.Runtime.InteropServices;
using System.Text;


public static class Utils
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StdStringSso
    {
        public fixed byte Buffer[16];
        public ulong Size;
        public ulong Capacity;
    }
    // Scans each argument as a struct pointer, chasing every pointer-sized field
    // within the first STRUCT_SCAN_BYTES bytes, looking for a C-string == TARGET.
    // Reads a MSVC x64 std::string object at strObjPtr.
    // Layout: [+0x00] char* ptr (heap) OR char buf[16] (SSO), [+0x10] size_t size, [+0x18] size_t capacity.
    // SSO when capacity == 15: data is inline at +0x00.  Heap when capacity > 15: +0x00 is char*.
    public static unsafe bool TryGetStdStringLayout(nint strObjPtr, out ulong size, out ulong capacity, out nint dataPtr, bool debug = false)
    {
        size = 0;
        capacity = 0;
        dataPtr = 0;

        if (!IsLikelyPointer(strObjPtr) || !IsMemReadable(strObjPtr, 0x20)) return false;

        size = *(ulong*)(strObjPtr + 0x10);
        capacity = *(ulong*)(strObjPtr + 0x18);

        // Keep conservative bounds to avoid interpreting random structs as strings.
        if (size == 0 || size > 512) return false;

        if (capacity <= 15)
        {
            // MSVC SSO: payload lives inline at +0x00.
            if (size > capacity) return false;
            dataPtr = strObjPtr;
            if (debug)
            {
                TheSpredsheetEdmundHates.LogStr($"[SSO] std::string at 0x{strObjPtr:X} size={size} capacity={capacity} inline data=0x{dataPtr:X}");
            }
            return IsMemReadable(dataPtr, (int)size + 1);
        }

        if (capacity < size || capacity > 0x10000) return false;

        // Heap string: +0x00 stores char*.
        dataPtr = *(nint*)strObjPtr;
        if (!IsLikelyPointer(dataPtr) || !IsMemReadable(dataPtr, (int)size + 1)) return false;
        return true;
    }

    public static unsafe string? TryReadStdString(nint strObjPtr, bool debug = false)
    {
        if (!TryGetStdStringLayout(strObjPtr, out ulong size, out _, out nint dataPtr, debug)) return null;

        byte* p = (byte*)dataPtr;
        var sb = new System.Text.StringBuilder((int)size);
        for (int i = 0; i < (int)size; i++)
        {
            byte b = p[i];
            if (b < 0x20 || b > 0x7E) return null;
            sb.Append((char)b);
        }

        // Require a terminator immediately after payload. This rejects many false positives.
        if (p[(int)size] != 0) return null;

        return sb.ToString();
    }

    // Returns a printable ASCII string from ptr if it looks like one, otherwise null.
    // Reads up to maxLen chars; rejects if any non-printable byte found before '\0'.
     public static unsafe string? TryReadCString(nint ptr, int maxLen = 128)
    {
        if (!IsLikelyPointer(ptr) || !IsMemReadable(ptr, 1)) return null;
        byte* p = (byte*)ptr;
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < maxLen; i++)
        {
            // Check readability at every 4 KB page boundary crossing.
            if (i > 0 && ((ulong)(ptr + i) & 0xFFFUL) == 0 && !IsMemReadable(ptr + i, 1)) return null;
            byte b = p[i];
            if (b == 0) return sb.Length > 0 ? sb.ToString() : null;
            if (b < 0x20 || b > 0x7E) return null; // non-printable → not a plain string
            sb.Append((char)b);
        }
        return null; // no null terminator within maxLen
    }

    public static string HexToAscii(nint hexString)
    {
                // 1. Convert the integer into a byte array
        byte[] bytes = BitConverter.GetBytes(hexString);

        // 2. Convert the bytes to an ASCII string (removes trailing null characters if present)
        string asciiString = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
        return asciiString;
    }

    static int _diagCallCount = 0;
    // Key: dedup token — each unique string+path combination is logged at most once
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenStrings = new();
    static readonly System.Collections.Concurrent.ConcurrentDictionary<nint, bool> _seenGonObjects = new();

    // Pointer range of the game's PE image (code, rdata, vtables — not heap objects).
    static readonly nint IMAGE_RANGE_START = unchecked((nint)0x7FF70C3C0000L);
    static readonly nint IMAGE_RANGE_END   = unchecked((nint)0x7FF70D900000L);


    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION
    {
        public nuint BaseAddress;
        public nuint AllocationBase;
        public uint  AllocationProtect;
        public ushort PartitionId;
        private ushort _pad;
        public nuint RegionSize;
        public uint  State;
        public uint  Protect;
        public uint  Type;
        private uint _trailingPad; // native struct is 48 bytes; C# sequential omits trailing alignment pad
    }

    [DllImport("kernel32.dll", SetLastError = false)]
    public static extern unsafe nint VirtualQuery(
        nint lpAddress, MEMORY_BASIC_INFORMATION* lpBuffer, nint dwLength);

    public static unsafe bool IsMemReadable(nint ptr, int size)
    {
        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery(ptr, &mbi, (nint)sizeof(MEMORY_BASIC_INFORMATION)) == 0) return false;
        if (mbi.State != 0x1000 /* MEM_COMMIT */) return false;
        const uint PAGE_NOACCESS = 0x01, PAGE_GUARD = 0x100;
        if ((mbi.Protect & (PAGE_NOACCESS | PAGE_GUARD)) != 0) return false;
        return (ulong)ptr + (ulong)size <= (ulong)mbi.BaseAddress + (ulong)mbi.RegionSize;
    }

    // Filters out small integers and kernel-space values; passes user-mode pointers.
    public static bool IsLikelyPointer(nint val)
    {
        ulong v = (ulong)(nuint)val;
        return v >= 0x10000 && v <= 0x0000_7FFF_FFFF_FFFF;
    }
    
    public unsafe static string ReadUtf16CustomString(nint address)
    {

        byte* obj = (byte*)address;

        ulong length = *(ulong*)(obj + 0x10);
        ulong capacity = *(ulong*)(obj + 0x18);

        if (length == 0)
            return string.Empty;

        if (length > int.MaxValue)
            throw new OverflowException("String length is too large.");

        if (capacity <= 7)
        {
            // Small-string optimization:
            // UTF-16 characters are stored directly at +0x00.
            return new string(
                (char*)(obj + 0x00),
                0,
                (int)length
            );
        }

        // Heap-allocated string.
        char* str = *(char**)obj;

        if (str == null)
            return string.Empty;

        return new string(
            str,
            0,
            (int)length
        );
    }
}