
using System.Runtime.InteropServices;
using System;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
unsafe struct GameString
{
    public ulong A;
    public ulong B;
    public ulong Length;
    public ulong Capacity;

    public static nint Create(string text)
    {
        GameString* s = (GameString*)Marshal.AllocHGlobal(sizeof(GameString));

        *s = default;

        byte* bytes = (byte*)s;

        for (int i = 0; i < text.Length; i++)
            bytes[i] = (byte)text[i];

        bytes[text.Length] = 0;

        s->Length = (ulong)text.Length;
        s->Capacity = 15;
        
        // TheSpredsheetEdmundHates.LogStr(sizeof(GameString).ToString());
        // TheSpredsheetEdmundHates.LogStr(((nuint)s).ToString("X"));
        return (nint)s;
    }
    

    public unsafe static nint CreateUTF16GameString(string text)
    {
        // string currentMethod = MethodBase.GetCurrentMethod().Name;
        // executionCounts[currentMethod] = executionCounts.ContainsKey(currentMethod) ? executionCounts[currentMethod] + 1 : 1;
        // Allocate enough space for the engine's std::wstring object.
        // 32 bytes is sufficient for the fields used by 370B100.
        
        nint str = Marshal.AllocHGlobal(0x30);
        // Initialize as an empty small-string.
        Buffer.MemoryCopy(null, (void*)str, 0, 0);

        *(ulong*)(str + 0x10) = 0; // length
        *(ulong*)(str + 0x18) = 7; // SSO capacity


        fixed (char* p = text)
        {
            TheSpredsheetEdmundHates.TheSpredsheetEdmundHates.assignString(str, p, (nuint)text.Length);
        }


        return str;
    }
    
}



