// using MewgenicsModSdk;
// using MewgenicsModSdk.Game;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Linq;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Diagnostics;

using System.Collections.Concurrent;
using System.Text;
namespace CatstableMod;

public partial class CatstableMod
{
    public string Id => "catstable";
    public string Name => "catstable";
    public bool IsEnabled { get; private set; } = true;

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private static CatstableMod? _instance;
    private Dictionary<string, string> _abilitiesLocalNames = new Dictionary<string, string>();


    private static volatile bool _genHooksInstalled = false;
    internal static volatile bool _genLoggingEnabled = false;
    private static nint _abilityTriggerHookTrampoline; // sub_7FF70C3F1ED0  glaiel::Ability::trigger

    private static nint _MyHook;
    private static nint _findMovieClipTrampoline;
    private static nint _removeMovieClipTrampoline;

    /// <summary>
    /// Entry point called from DllMain (DLL_PROCESS_ATTACH) after MewjectorApi.Resolve() succeeds.
    /// Installs our hooks through Mewjector so they participate in hook chaining.
    /// </summary>
    internal unsafe void MjInit()
    {
        _findMovieClipTrampoline = MewjectorApi.InstallHook(
            0x990480, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&FindChildMovieClipHook);

        _removeMovieClipTrampoline = MewjectorApi.InstallHook(
            0x99e030, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&RemoveMovieClip);

        _instance = this;
        MewjectorApi.Log("MjInit: installing hooks...");

        var location = MewjectorApi.GameBase;
        LogStr($"Gamebase at {location:X}...");


        // _abilityTriggerHookTrampoline = MewjectorApi.InstallHook(
        //     0x31ED0, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&AbilityTriggerHook);
        // MewjectorApi.Log("AbilityTriggerHook at RVA 0x31ED0 installed");


        // MewjectorApi.Log("AbilityTriggerHook at RVA 0x31ED0 installed");
        
        // MewjectorApi.Log("MjInit complete");

        // var targetAddress = Process.GetCurrentProcess().MainModule.BaseAddress + 0xA1AA50;
        // byte[] bytes = new byte[16];
        // Marshal.Copy(targetAddress, bytes, 0, 16);

        // byte[] buf = new byte[0xA0];
        // Marshal.Copy(thisPtr, buf, 0, buf.Length);


        // _instance.Log(BitConverter.ToString(bytes));
        // _MyHook = MewjectorApi.InstallHook(0xB631C0, (void*)(delegate* unmanaged<long, long, nint, nint, nint>)&MyHook);

    }

    // internal unsafe void MjInit()
    // {

    //     0xA1AA50, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&FindChildMovieClipHook);
    //     MewjectorApi.Log("AbilityTriggerHook at RVA 0x31ED0 installed");
    // }

    // 1. Thread-safe bag to hold the pointers to our SymbolClasses
    private static ConcurrentBag<nint> _symbolClasses = new ConcurrentBag<nint>();

    // Windows API for safe memory reading (prevents Access Violation crashes)
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, [Out] byte[] lpBuffer, nint dwSize, out nint lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    private static extern nint GetCurrentProcess();

    static public HashSet<string> a2ArgsUnique = new HashSet<string>();


    private static unsafe nint CallWithCustomString(
        nint trampoline,
        nint a1,
        string text,
        nint a3,
        nint a4)
    {
        StdStringSso str = default;
        StdStringSso* pStr = &str;

        byte[] utf8 = Encoding.UTF8.GetBytes(text);

        if (utf8.Length > 15)
            throw new ArgumentException("Not SSO");

        for (int i = 0; i < utf8.Length; i++)
            pStr->Buffer[i] = utf8[i];

        pStr->Buffer[utf8.Length] = 0;

        pStr->Size = (ulong)utf8.Length;
        pStr->Capacity = 15;

        return ((delegate* unmanaged<nint, nint, nint, nint, nint>)trampoline)(
            a1,
            (nint)pStr,
            a3,
            a4);
    }

    static nint movieClipModContainer = 0;
    static nint rowMovieClip = 0;

    [UnmanagedCallersOnly]
    private static unsafe nint FindChildMovieClipHook(nint a1, nint a2, nint a3, nint a4)
    {

        var a2Str = TryReadStdString(a2, false);
        if (a2Str == "openclose_H" && movieClipModContainer == 0)
        {
            string juanito = "juanito";
            nint subresult = CallWithCustomString(_findMovieClipTrampoline, a1, juanito, a3, a4);
            // check if its null pointer result or not
            if (subresult != 0)
            {
                // mod container "juanito" found
                // print a1 too
                MewjectorApi.Log($"[HOOK] Found 'juanito' container at 0x{subresult:X}");
                nint juanito2 = CallWithCustomString(_findMovieClipTrampoline, subresult, "juanito2", a3, a4);
                if (juanito2 != 0)
                {
                    // mod container "juanito2" found
                    MewjectorApi.Log($"[HOOK] Found 'juanito2' container at 0x{juanito2:X}");
                    nint juanito3 = CallWithCustomString(_findMovieClipTrampoline, juanito2, "juanito3", a3, a4);
                    if (juanito3 != 0)
                    {
                        MewjectorApi.Log($"[HOOK] Found 'juanito3' container at 0x{juanito3:X}");
                        nint juanito4 = CallWithCustomString(_findMovieClipTrampoline, juanito3, "juanito4", a3, a4);
                        if (juanito4 != 0)
                        {
                            MewjectorApi.Log($"[HOOK] Found 'juanito4' container at 0x{juanito4:X}");
                            movieClipModContainer = juanito4;
                            rowMovieClip = CallWithCustomString(_findMovieClipTrampoline, juanito4, "row", a3, a4);
                        }
                        else
                        {
                            MewjectorApi.Log($"[HOOK] 'juanito4' container NOT found");
                        }
                    }
                    else
                    {
                        MewjectorApi.Log($"[HOOK] 'juanito3' container NOT found");
                    }
                }
                else
                {
                    MewjectorApi.Log($"[HOOK] 'juanito2' container NOT found");
                }
            }
            
        }

        return ((delegate* unmanaged<nint, nint, nint, nint, nint>)_findMovieClipTrampoline)(a1, a2, a3, a4);
    }

    [UnmanagedCallersOnly]
 private static unsafe nint RemoveMovieClip(nint a1, nint a2, nint a3, nint a4)
    {
        if (movieClipModContainer != 0 && a1 == movieClipModContainer)
        {
            MewjectorApi.Log($"[HOOK] RemoveMovieClip called on mod container 0x{a1:X}");
            movieClipModContainer = 0;
        }
        return ((delegate* unmanaged<nint, nint, nint, nint, nint>)_removeMovieClipTrampoline)(a1, a2, a3, a4);
    }

    private static byte[] SafeReadProcessMemory(nint hProcess, nint address, long size)
    {
        byte[] buffer = new byte[size];
        long bytesReadTotal = 0;
        long chunkSize = 4096; // Standard memory page size
        
        for (long i = 0; i < size; i += chunkSize)
        {
            long toRead = Math.Min(chunkSize, size - i);
            byte[] chunk = new byte[toRead];
            
            if (ReadProcessMemory(hProcess, address + (nint)i, chunk, (nint)toRead, out nint read) && (long)read > 0)
            {
                Buffer.BlockCopy(chunk, 0, buffer, (int)i, (int)read);
                bytesReadTotal += (long)read;
            }
            else
            {
                // We hit an unallocated memory page. Stop reading, but KEEP what we successfully read!
                break; 
            }
        }
        
        if (bytesReadTotal == 0) return null;
        
        if (bytesReadTotal < size)
        {
            byte[] resized = new byte[bytesReadTotal];
            Buffer.BlockCopy(buffer, 0, resized, 0, (int)bytesReadTotal);
            return resized;
        }
        
        return buffer;
    }

    [UnmanagedCallersOnly]
    private static unsafe nint HookDictionaryInsert(nint mapPtr, nint iteratorOut, nint stringPtr)
    {
        // MSVC std::string layout: 
        // +0x10 = Length, +0x18 = Capacity.
        long length = Marshal.ReadInt64(stringPtr + 0x10);
        long capacity = Marshal.ReadInt64(stringPtr + 0x18);
        string symbolName = "";

        if (length > 0 && length < 1000) // Sanity check
        {
            if (capacity < 16)
            {
                // Small String Optimization (Inline)
                byte* chars = (byte*)stringPtr;
                symbolName = Encoding.UTF8.GetString(chars, (int)length);
            }
            else
            {
                // Heap String (Pointer)
                nint heapPtr = Marshal.ReadIntPtr(stringPtr);
                if (heapPtr != IntPtr.Zero)
                {
                    byte* chars = (byte*)heapPtr;
                    symbolName = Encoding.UTF8.GetString(chars, (int)length);
                }
            }
        }

        if (symbolName == "CombatMessage_Victory")
        {
            MewjectorApi.Log($"\n[BINGO] INTERCEPTED 'CombatMessage_Victory'!");
            MewjectorApi.Log($"  Dictionary Map Pointer: 0x{mapPtr:X}");
            MewjectorApi.Log($"  std::string Pointer: 0x{stringPtr:X}");
        }

        // Call the original function so the game doesn't break
        return ((delegate* unmanaged<nint, nint, nint, nint>)_findMovieClipTrampoline)(mapPtr, iteratorOut, stringPtr);
    }

    private static void DumpSymbolClassStrings()
    {
        MewjectorApi.Log($"[DUMP] Analyzing {_symbolClasses.Count} captured objects...");
        nint hProcess = GetCurrentProcess();

        foreach (nint symbolClass in _symbolClasses)
        {
            int count = Marshal.ReadInt32(symbolClass + 0x28); 
            nint dataPtr = Marshal.ReadIntPtr(symbolClass + 0x30);

            if (dataPtr == IntPtr.Zero || count <= 0 || count > 100000) continue;

            // Use our new safe read method. Assume each element is 40 bytes.
            long expectedSize = count * 40L; 
            byte[] arrayMem = SafeReadProcessMemory(hProcess, dataPtr, expectedSize);

            if (arrayMem == null)
            {
                MewjectorApi.Log($"  -> [!] Totally failed to read array memory at 0x{dataPtr:X} for {count} elements.");
                continue;
            }

            int foundCount = 0;
            bool targetFound = false;

            // Parse the C++ array: struct { uint16_t id; char padding[6]; std::string name; }
            for (int i = 0; i < count; i++)
            {
                int offset = i * 40;
                if (offset + 40 > arrayMem.Length) break;

                // The std::string is at offset 0
                long strLength = BitConverter.ToInt64(arrayMem, offset + 16);
                long strCapacity = BitConverter.ToInt64(arrayMem, offset + 24);
                
                // The ID is at offset 32 (right after the 32-byte std::string)
                ushort id = BitConverter.ToUInt16(arrayMem, offset + 32);

                string s = null;

                if (strCapacity >= 0 && strCapacity < 16)
                {
                    // Small String Optimization (Inline)
                    int nullIdx = Array.IndexOf(arrayMem, (byte)0, offset, 16);
                    int len = nullIdx == -1 ? (int)strLength : (nullIdx - offset);
                    if (len > 0 && len <= 15)
                    {
                        s = Encoding.UTF8.GetString(arrayMem, offset, len);
                    }
                }
                else if (strCapacity >= 16 && strCapacity < 1000000 && strLength > 0 && strLength <= strCapacity)
                {
                    // Heap String (Pointer)
                    nint strPtr = (nint)BitConverter.ToInt64(arrayMem, offset);
                    if (strPtr != IntPtr.Zero)
                    {
                        byte[] heapBuf = new byte[strLength];
                        if (ReadProcessMemory(hProcess, strPtr, heapBuf, (nint)strLength, out _))
                        {
                            s = Encoding.UTF8.GetString(heapBuf);
                        }
                    }
                }

                if (s == "CombatMessage_Victory")
                {
                    MewjectorApi.Log($"\n=== [EUREKA] TARGET FOUND! ===");
                    MewjectorApi.Log($"  SymbolClass Obj: 0x{symbolClass:X}");
                    MewjectorApi.Log($"  Array Pointer: 0x{dataPtr:X}");
                    MewjectorApi.Log($"  Symbol ID: {id} (This is the SWF Character ID!)");
                    MewjectorApi.Log($"  Index in Array: {i}");
                    return; 
                }
                else if (!string.IsNullOrEmpty(s) && s.Length > 1 && foundCount < 10)
                {
                    // Print the first 10 valid strings so we can confirm it's parsing beautifully now
                    if (foundCount == 0) MewjectorApi.Log($"\n=== SymbolClass 0x{symbolClass:X} ({count} elements) ===");
                    MewjectorApi.Log($"    Index [{i}] ID [{id}] -> \"{s}\"");
                    foundCount++;
                }
            }

            if (!targetFound && foundCount > 0)
            {
                MewjectorApi.Log($"    ... (Parsed {count} symbols successfully)");
            }
        }
    }
    // Helper 1: Extracts any sequence of 4+ printable ASCII characters from a raw byte block
    private static string ExtractPrintableStrings(byte[] data)
    {
        StringBuilder result = new StringBuilder();
        StringBuilder current = new StringBuilder();

        foreach (byte b in data)
        {
            // Check if byte is a standard printable ASCII character (space to tilde)
            if (b >= 32 && b <= 126) 
            {
                current.Append((char)b);
            }
            else
            {
                if (current.Length >= 4) 
                    result.AppendLine($"    \"{current.ToString()}\"");
                current.Clear();
            }
        }
        if (current.Length >= 4) result.AppendLine($"    \"{current.ToString()}\"");
        return result.ToString();
    }

    // Helper 2: Extracts a clean C-style null-terminated string from a pointer buffer
    private static string ExtractFirstPrintableString(byte[] data)
    {
        StringBuilder current = new StringBuilder();
        foreach (byte b in data)
        {
            if (b == 0) break; // Stop at null terminator
            if (b >= 32 && b <= 126)
            {
                current.Append((char)b);
            }
            else
            {
                // If we hit garbage before a null terminator, this wasn't a valid string pointer
                return ""; 
            }
        }
        return current.ToString();
    }


    private static bool firedAlready = false;
    private static List<ushort> charactersAlreadySeen = new List<ushort>();
    ushort firstOne = 0;
    [UnmanagedCallersOnly]
    static unsafe nint  MyHook(
    long a1,
    long a2,
    nint a3,
    nint a4)
    {
        long obj = a1;
        var result = ((delegate* unmanaged<long, long, nint, nint, nint>)_MyHook)(a1, a2, a3, a4);

        // byte version = *(byte*)(obj + 0x38);
        // ushort depth = *(ushort*)(obj + 0x3A);
        ushort characterId = *(ushort*)(obj + 0x3C);
        if (charactersAlreadySeen.Contains(characterId))
            return result;
        charactersAlreadySeen.Add(characterId);

        byte* p = (byte*)a1;
        for (int off = 0; off < 0x90; off += 8)
        {
            ulong ptr = *(ulong*)(p + off);

            if (ptr > 0x10000 && ptr < 0x0000_7FFF_FFFF_FFFF)
            {
                // byte[] bytes = new byte[64];
                // Marshal.Copy((IntPtr)ptr, bytes, 0, bytes.Length);
                // _instance.Log(
                    // $"ptr @ +0x{off:X2} = 0x{ptr:X}");
                // _instance.Log(
                //     System.Text.Encoding.ASCII.GetString(bytes));
            }
        }
        return result;
    }
    [UnmanagedCallersOnly]
    private static unsafe nint AbilityTriggerHook(nint a1, nint a2, nint a3, nint a4)
    {
        if (_instance.IsEnabled) {
            try
            {
                string? name = ReadAbilityName(a1);
                string? localizedName = "???";
                Dictionary<string, object?> gonFields = new Dictionary<string, object>();
                // Log the GonObject fields (ability+0x28 = GonObject*)
                if (IsLikelyPointer(a1) && IsMemReadable(a1 + 0x28, 8))
                {
                    nint gonPtr = *(nint*)(a1 + 0x28);
                    if (IsLikelyPointer(gonPtr))
                    {
                        _instance?.Log($"[GON-PTR] 0x{gonPtr:X}");
                        LogGonObject(gonPtr, 0, "  ");
                        gonFields = GonObjectToDictionary(gonPtr);
                        // get field with name "meta", inside it field with name "name", then get its value
                        if (gonFields.TryGetValue("meta", out var metaObj) && metaObj is Dictionary<string, object> metaDict &&
                            metaDict.TryGetValue("name", out var nameObj) && nameObj is string gonName)
                        {
                            _instance?.Log($"[GON-NAME] {gonName}");
                            name = gonName; // override with name from GonObject if available
                            localizedName = name != null ? LookupLocalized(name) : null;
                        }
                    }
                }
                _instance?.Log($"[ABILITY-NAME] \"{name ?? "???"}\" localized=\"{localizedName ?? name ?? "???"}\" this=0x{a1:X}");
            }
            catch (Exception ex)
            {
                _instance?.Log($"[ABILITY-NAME-ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        return ((delegate* unmanaged<nint, nint, nint, nint, nint>)_abilityTriggerHookTrampoline)(a1, a2, a3, a4);
    }


    public async Task RunEverySecond(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(0.5));
        while (await timer.WaitForNextTickAsync(ct))
        {
            EverySecond();
        }
    }

    private static bool _active;   // static — accessible from [UnmanagedCallersOnly]



    protected void EverySecond()
    {
        if (!IsEnabled) return;

        // if (fightChars.Count == 0) return;
        // LogDifferences();
    }

    // protected void OnFightEnd(FightEndEvent e)
    // {
    //     fightChars.Clear();
    //     trackedCats.Clear();
    // }

    // private void OnKeyDown(KeyEventArgs e)
    // {
    //     if (e.IsRepeat) return;
    //     if (e.Scancode != SDL_Scancode.F5) return;

    //     if (!_genHooksInstalled)
    //     {
    //         _genHooksInstalled = true;
    //         _genLoggingEnabled = true;
    //         Log("[F5] Key detected — installing generated hooks...");
    //         // InstallGeneratedHooks();
            

    //         Log($"[F5] {_genHooks.Length} hooks installed. Logging ON.");
    //     }
    //     else
    //     {
    //         _genLoggingEnabled = !_genLoggingEnabled;
    //         Log($"[F5] Logging {(_genLoggingEnabled ? "ON" : "OFF")}.");
    //     }
    // }

    protected void OnEnable()
    {
        Log("Catstable enabled");
        // OverlayManager.Start(Log);
        Log("OverlayManager started");
    }

    protected void OnDisable()
    {
        Log("Catstable disabled");
        // OverlayManager.Stop();
        Log("OverlayManager stopped");
    }

    private static readonly string LogFilePath = @"E:\Documents\catstable\log.txt";
    private static readonly StreamWriter _logWriter = new StreamWriter(
        new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read),
        System.Text.Encoding.UTF8, bufferSize: 4096, leaveOpen: false) { AutoFlush = true };
    private static readonly object _logLock = new();

    private new void Log(string message)
    {
        lock (_logLock)
            _logWriter.WriteLine(message);
        // File.AppendAllText(LogFilePath, message + Environment.NewLine);
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenFns = new();

    static public void LogStr(string message)
    {
        // Parse the sub_ name from "[N|0xRVA|sub_XXXX] ..."
        // var m = System.Text.RegularExpressions.Regex.Match(message, @"\|sub_([0-9A-Fa-f]+)\]");
        // if (m.Success)
        // {
        //     if (!_seenFns.TryAdd(m.Value, true))
        //         return; // already logged this function before
        // }
        // File.AppendAllText(LogFilePath, message + Environment.NewLine);
        lock (_logLock)
            _logWriter.WriteLine(message);
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct StdStringSso
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
    private static unsafe bool TryGetStdStringLayout(nint strObjPtr, out ulong size, out ulong capacity, out nint dataPtr, bool debug = false)
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
                LogStr($"[SSO] std::string at 0x{strObjPtr:X} size={size} capacity={capacity} inline data=0x{dataPtr:X}");
            }
            return IsMemReadable(dataPtr, (int)size + 1);
        }

        if (capacity < size || capacity > 0x10000) return false;

        // Heap string: +0x00 stores char*.
        dataPtr = *(nint*)strObjPtr;
        if (!IsLikelyPointer(dataPtr) || !IsMemReadable(dataPtr, (int)size + 1)) return false;
        return true;
    }

    private static unsafe string? TryReadStdString(nint strObjPtr, bool debug = false)
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
    private static unsafe string? TryReadCString(nint ptr, int maxLen = 128)
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

    private static int _diagCallCount = 0;
    // Key: dedup token — each unique string+path combination is logged at most once
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenStrings = new();
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<nint, bool> _seenGonObjects = new();

    // Pointer range of the game's PE image (code, rdata, vtables — not heap objects).
    private static readonly nint IMAGE_RANGE_START = unchecked((nint)0x7FF70C3C0000L);
    private static readonly nint IMAGE_RANGE_END   = unchecked((nint)0x7FF70D900000L);

    // Returns true if ptr falls inside the game's loaded image.
    // Such pointers are vtable / function pointers; they are not heap structs worth scanning.
    private static bool IsImagePointer(nint ptr)
    {
        ulong v = (ulong)(nuint)ptr;
        return v >= (ulong)(nuint)IMAGE_RANGE_START && v < (ulong)(nuint)IMAGE_RANGE_END;
    }

    // Recursively scans [basePtr, basePtr+maxBytes) for the TARGET string.
    //   depth=1 → no further recursion after this level.
    //   visited  → prevents re-entering the same base address (cycle guard).
    // At each 8-byte slot the method tries (in order):
    //   1. Inline std::string object starting at the slot.
    //   2. The slot value as a raw pointer → try C-string, then std::string object.
    //   3. If depth > 1 and the pointer isn't an image pointer: recurse into that struct.
    private static unsafe void ScanRegionForTarget(
        string funcName, string path, nint basePtr, int maxBytes, int depth, System.Collections.Generic.HashSet<nint> visited)
    {
        const string TARGET = "CrowFlutter";
        if (!IsLikelyPointer(basePtr) || IsImagePointer(basePtr)) return;
        if (!visited.Add(basePtr)) return; // cycle guard

        int slots = maxBytes / 8;
        bool fullRegion = IsMemReadable(basePtr, maxBytes);

        for (int s = 0; s < slots; s++)
        {
            nint slotAddr = basePtr + s * 8;
            if (!fullRegion && !IsMemReadable(slotAddr, 0x20)) continue;

            // Case 1: inline std::string starting at this slot
            if (TryGetStdStringLayout(slotAddr, out _, out _, out _))
            {
                string? str = TryReadStdString(slotAddr);
                if (str != null)
                {
                    string key = $"str|{funcName}|{path}+0x{s * 8:X}|{str}";
                    if (_seenStrings.TryAdd(key, true))
                        _instance?.Log($"[STR] {funcName} {path}+0x{s * 8:X} \"{str}\"");
                    if (str == TARGET)
                        _instance?.Log($"[MATCH] {funcName} {path}+0x{s * 8:X} (inline std::string)");
                }
            }

            // Case 2: pointer-valued slot
            nint fieldPtr = *(nint*)slotAddr;
            if (!IsLikelyPointer(fieldPtr)) continue;
            // Skip vtable / function pointers — they produce garbage C-string false positives
            // and are never the heap objects we are looking for.
            if (IsImagePointer(fieldPtr)) continue;

            // 2a: pointed-to bytes look like a C-string
            string? cstr = TryReadCString(fieldPtr, 128);
            if (cstr != null)
            {
                if (cstr.Length >= 4) // 1-3 char results are almost always garbage (padding, small ints)
                {
                    string key = $"cstr|{funcName}|{path}+0x{s * 8:X}|{cstr}";
                    if (_seenStrings.TryAdd(key, true))
                        _instance?.Log($"[CSTR] {funcName} {path}+0x{s * 8:X} -> \"{cstr}\"");
                    if (cstr == TARGET)
                        _instance?.Log($"[MATCH] {funcName} {path}+0x{s * 8:X} -> cstr");
                    continue; // real string → treat as leaf, don't recurse
                }
                // Too short — fall through to try as std::string object, then recurse
            }

            // 2b: fieldPtr is itself a std::string object
            string? pstd = TryReadStdString(fieldPtr);
            if (pstd != null)
            {
                string key = $"pstd|{funcName}|{path}+0x{s * 8:X}|{pstd}";
                if (_seenStrings.TryAdd(key, true))
                    _instance?.Log($"[PTR-STR] {funcName} {path}+0x{s * 8:X} -> \"{pstd}\"");
                if (pstd == TARGET)
                    _instance?.Log($"[MATCH] {funcName} {path}+0x{s * 8:X} -> ptr-stdstr");
                continue; // treat as leaf
            }

            // Case 3: fieldPtr points into an unknown struct — recurse if depth permits
            if (depth > 1 && !IsImagePointer(fieldPtr))
            {
                int subBytes = Math.Max(64, maxBytes / 4);
                ScanRegionForTarget(funcName, $"{path}+0x{s * 8:X}->", fieldPtr, subBytes, depth - 1, visited);
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORY_BASIC_INFORMATION
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
    private static extern unsafe nint VirtualQuery(
        nint lpAddress, MEMORY_BASIC_INFORMATION* lpBuffer, nint dwLength);

    private static unsafe bool IsMemReadable(nint ptr, int size)
    {
        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery(ptr, &mbi, (nint)sizeof(MEMORY_BASIC_INFORMATION)) == 0) return false;
        if (mbi.State != 0x1000 /* MEM_COMMIT */) return false;
        const uint PAGE_NOACCESS = 0x01, PAGE_GUARD = 0x100;
        if ((mbi.Protect & (PAGE_NOACCESS | PAGE_GUARD)) != 0) return false;
        return (ulong)ptr + (ulong)size <= (ulong)mbi.BaseAddress + (ulong)mbi.RegionSize;
    }

    // Filters out small integers and kernel-space values; passes user-mode pointers.
    private static bool IsLikelyPointer(nint val)
    {
        ulong v = (ulong)(nuint)val;
        return v >= 0x10000 && v <= 0x0000_7FFF_FFFF_FFFF;
    }

    // Reads and logs all fields of a GonObject instance.
    // GonObject layout (MSVC x64, sizeof=0xB0) — from Tyler Glaiel's open-source GON library:
    //   +0x00: unordered_map<string,int> children_map  (56 bytes; element count at +0x08)
    //   +0x38: vector<GonObject> children_array         (first ptr +0x38, last ptr +0x40)
    //   +0x50: int    int_data
    //   +0x58: double float_data
    //   +0x60: bool   bool_data
    //   +0x68: string string_data
    //   +0x88: string name                              ← IDA-confirmed
    //   +0xA8: int    type  (0=NULL,1=STRING,2=NUMBER,3=OBJECT,4=ARRAY,5=BOOL)
    private static unsafe void LogGonObject(nint gonPtr, int depth, string indent)
    {
        if (depth > 4) return;
        if (!IsLikelyPointer(gonPtr) || !IsMemReadable(gonPtr, 0xB0)) return;
        if (!_seenGonObjects.TryAdd(gonPtr, true)) return; // log each address only once

        int typeVal = *(int*)(gonPtr + 0xA8);
        string typeName = typeVal switch {
            0 => "NULLGON",
            1 => "STRING",
            2 => "NUMBER",
            3 => "OBJECT",
            4 => "ARRAY",
            5 => "BOOL",
            _ => $"TYPE({typeVal})"
        };

        string gonName    = TryReadStdString(gonPtr + 0x88) ?? "<noname>";
        string stringData = TryReadStdString(gonPtr + 0x68) ?? "";
        int    intData    = *(int*)(gonPtr + 0x50);
        double floatData  = *(double*)(gonPtr + 0x58);
        bool   boolData   = *(byte*)(gonPtr + 0x60) != 0;

        // unordered_map element count lives at offset +0x08 from map start (+0x00 of GonObject)
        long mapCount = *(long*)(gonPtr + 0x08);
        if (mapCount < 0 || mapCount > 100000) mapCount = -1;

        // vector<GonObject>: first ptr at +0x38, past-last ptr at +0x40
        nint arrFirst = *(nint*)(gonPtr + 0x38);
        nint arrLast  = *(nint*)(gonPtr + 0x40);
        long arrCount = -1;
        if (IsLikelyPointer(arrFirst) && IsLikelyPointer(arrLast) && (long)(arrLast - arrFirst) >= 0)
        {
            arrCount = (long)(arrLast - arrFirst) / 0xB0;
            if (arrCount > 10000) arrCount = -1;
        }

        string valueStr = typeName switch {
            "STRING" => $" value=\"{stringData}\"",
            "NUMBER" => $" int={intData} float={floatData:G}",
            "BOOL"   => $" bool={boolData}",
            _        => ""
        };

        _instance?.Log($"{indent}[GON] name=\"{gonName}\" type={typeName}{valueStr} arr_children={arrCount} map_children={mapCount}");

        if ((typeName == "OBJECT" || typeName == "ARRAY") && arrCount > 0 && arrCount <= 200 && IsLikelyPointer(arrFirst))
        {
            for (long i = 0; i < arrCount; i++)
            {
                nint childPtr = arrFirst + (nint)(i * 0xB0);
                LogGonObject(childPtr, depth + 1, indent + "  ");
            }
        }
    }

    private static unsafe Dictionary<string, object?> GonObjectToDictionary(nint gonPtr)
    {
        var result = new Dictionary<string, object?>();
        if (!IsLikelyPointer(gonPtr) || !IsMemReadable(gonPtr, 0xB0)) return result;

        int typeVal = *(int*)(gonPtr + 0xA8);
        if (typeVal != 3 /* OBJECT */) return result;

        nint arrFirst = *(nint*)(gonPtr + 0x38);
        nint arrLast  = *(nint*)(gonPtr + 0x40);
        if (!IsLikelyPointer(arrFirst) || !IsLikelyPointer(arrLast)) return result;

        long arrCount = (long)(arrLast - arrFirst) / 0xB0;
        if (arrCount <= 0 || arrCount > 10000) return result;

        for (long i = 0; i < arrCount; i++)
        {
            nint childPtr = arrFirst + (nint)(i * 0xB0);
            if (!IsMemReadable(childPtr, 0xB0)) continue;

            string childName = TryReadStdString(childPtr + 0x88) ?? "";
            int childType    = *(int*)(childPtr + 0xA8);

            object? value = childType switch {
                0 => null,
                1 => TryReadStdString(childPtr + 0x68),
                2 => *(double*)(childPtr + 0x58),
                3 => GonObjectToDictionary(childPtr),
                4 => GonArrayToList(childPtr),
                5 => *(byte*)(childPtr + 0x60) != 0,
                _ => null
            };

            result[childName] = value;
        }

        return result;
    }

    private static unsafe List<object?> GonArrayToList(nint gonPtr)
    {
        var result = new List<object?>();
        if (!IsLikelyPointer(gonPtr) || !IsMemReadable(gonPtr, 0xB0)) return result;

        int typeVal = *(int*)(gonPtr + 0xA8);
        if (typeVal != 4 /* ARRAY */) return result;

        nint arrFirst = *(nint*)(gonPtr + 0x38);
        nint arrLast  = *(nint*)(gonPtr + 0x40);
        if (!IsLikelyPointer(arrFirst) || !IsLikelyPointer(arrLast)) return result;

        long arrCount = (long)(arrLast - arrFirst) / 0xB0;
        if (arrCount <= 0 || arrCount > 10000) return result;

        for (long i = 0; i < arrCount; i++)
        {
            nint childPtr = arrFirst + (nint)(i * 0xB0);
            if (!IsMemReadable(childPtr, 0xB0)) continue;

            int childType = *(int*)(childPtr + 0xA8);
            object? value = childType switch {
                0 => null,
                1 => TryReadStdString(childPtr + 0x68),
                2 => *(double*)(childPtr + 0x58),
                3 => GonObjectToDictionary(childPtr),
                4 => GonArrayToList(childPtr),
                5 => *(byte*)(childPtr + 0x60) != 0,
                _ => null
            };

            result.Add(value);
        }

        return result;
    }

    // ── Localization lookup via game's localization manager ──────────────────
    //
    // RVAs (verified against online reference implementation):
    //   0x13B2590  localization manager global (NOT 0xBB2590 — that was wrong)
    //   0x52640    sub_7FF70C412640 — init narrow std::string from C-string
    //   0x956270   sub_7FF70CD16270 — 3-arg lookup wrapper: (mgr*, out_wstr*, key_str*) void
    //   0x522D0    sub_7FF70C4122D0 — destroy narrow std::string (frees heap if cap>15)
    //   0x51DE0    sub_7FF70C411DE0 — destroy wide std::wstring (frees heap if cap>7)
    //
    // MSVC std::string / std::wstring layout (x64, 32 bytes):
    //   [+0x00..+0x0F]  inline buf (SSO) OR heap pointer
    //   [+0x10]         size_t size
    //   [+0x18]         size_t capacity  (SSO threshold: 15 narrow, 7 wide)

    private const ulong LOCALIZATION_MGR_RVA    = 0x13B2590UL; // global localization manager
    private const ulong NARROW_STR_INIT_RVA     = 0x52640UL;   // init narrow std::string
    private const ulong LOCALIZE_KEY_RVA        = 0x956270UL;  // 3-arg lookup wrapper
    private const ulong NARROW_STR_DESTROY_RVA  = 0x522D0UL;   // destroy narrow std::string
    private const ulong WIDE_STR_DESTROY_RVA    = 0x51DE0UL;   // destroy wide std::wstring

    // Constructs an empty MSVC std::wstring (SSO) at the given 32-byte allocation.
    // Constructor (sub_7FF70C412640) zeroes all 32 bytes itself, but we pre-zero the
    // output wstring so sub_7FF70CD15510 sees a valid empty string if it assigns into it.
    private static unsafe void InitEmptyWString(byte* ws)
    {
        for (int i = 0; i < 32; i++) ws[i] = 0;
        *(ulong*)(ws + 0x18) = 7UL;   // capacity = 7 → SSO sentinel
    }

    // Reads a std::wstring from the 32-byte MSVC layout into a .NET string.
    // size at +0x10, capacity at +0x18; SSO inline data starts at +0x00.
    private static unsafe string? ReadWString(byte* ws)
    {
        ulong size = *(ulong*)(ws + 0x10);
        ulong cap  = *(ulong*)(ws + 0x18);
        if (size == 0) return "";
        if (size > 65536) return null;

        char* data;
        if (cap <= 7)
        {
            data = (char*)ws;  // SSO: inline buffer at +0x00
        }
        else
        {
            data = *(char**)ws;  // heap ptr at +0x00
            if (!IsLikelyPointer((nint)data) || !IsMemReadable((nint)data, (int)size * 2 + 2))
                return null;
        }
        return new string(data, 0, (int)size);
    }

    private static unsafe string? LookupLocalized(string key)
    {
        nuint gameBase = MewjectorApi.GameBase;
        if (gameBase == 0) return null;

        nint locMgr      = (nint)(gameBase + LOCALIZATION_MGR_RVA);
        nint initFn      = (nint)(gameBase + NARROW_STR_INIT_RVA);
        nint lookupFn    = (nint)(gameBase + LOCALIZE_KEY_RVA);
        nint destroyNarrow = (nint)(gameBase + NARROW_STR_DESTROY_RVA);
        nint destroyWide   = (nint)(gameBase + WIDE_STR_DESTROY_RVA);

        _instance?.Log($"[LKP-1] key=\"{key}\" mgr=0x{locMgr:X} lookup=0x{lookupFn:X}");

        nint keyAlloc = Marshal.AllocHGlobal(32);
        nint outAlloc = Marshal.AllocHGlobal(32);
        bool keyInited = false;

        try
        {
            // Zero both structs (mirrors C reference: memset to 0)
            for (int i = 0; i < 32; i++) ((byte*)keyAlloc)[i] = 0;
            for (int i = 0; i < 32; i++) ((byte*)outAlloc)[i] = 0;

            // Build narrow std::string for the key
            byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(key + "\0");
            fixed (byte* kp = keyBytes)
                ((delegate* unmanaged<nint, nint, void>)initFn)(keyAlloc, (nint)kp);
            keyInited = true;

            _instance?.Log($"[LKP-2] key_str: size=0x{*(ulong*)((byte*)keyAlloc+0x10):X} cap=0x{*(ulong*)((byte*)keyAlloc+0x18):X}");
            _instance?.Log($"[LKP-3] calling lookup...");

            // 3-arg lookup: (localization_manager*, out_wstring*, key_string*) → void
            ((delegate* unmanaged<nint, nint, nint, void>)lookupFn)(locMgr, outAlloc, keyAlloc);

            ulong outSize = *(ulong*)((byte*)outAlloc + 0x10);
            ulong outCap  = *(ulong*)((byte*)outAlloc + 0x18);
            _instance?.Log($"[LKP-4] out_wstr: size=0x{outSize:X} cap=0x{outCap:X}");

            string? result = outSize > 0 ? ReadWString((byte*)outAlloc) : null;
            _instance?.Log($"[LKP-5] result=\"{result ?? "<empty>"}\"");

            // Destroy output wstring buffer (frees heap data if cap > 7)
            ((delegate* unmanaged<nint, void>)destroyWide)(outAlloc);

            // Destroy narrow key string
            ((delegate* unmanaged<nint, void>)destroyNarrow)(keyAlloc);
            keyInited = false;

            return result;
        }
        catch (Exception ex)
        {
            _instance?.Log($"[LKP-EX] {ex.GetType().Name}: {ex.Message}");
            return null;
        }
        finally
        {
            if (keyInited)
            {
                try { ((delegate* unmanaged<nint, void>)destroyNarrow)(keyAlloc); } catch { }
            }
            Marshal.FreeHGlobal(outAlloc);
            Marshal.FreeHGlobal(keyAlloc);
        }
    }

    // Reads the ability localization key from an Ability* pointer.
    // Layout: ability+0x28 = GonObject*, children vector at +0x38/+0x40 (GonObject, stride=0xB0).
    // We look for a child node whose name (+0x88) == "name" and read its string_data (+0x68).
    // Falls back to the root GON node's own name if the "name" child isn't found.
    private static unsafe string? ReadAbilityName(nint abilityPtr)
    {
        if (!IsLikelyPointer(abilityPtr) || !IsMemReadable(abilityPtr + 0x28, 8)) return null;
        nint gonPtr = *(nint*)(abilityPtr + 0x28);
        if (!IsLikelyPointer(gonPtr) || !IsMemReadable(gonPtr, 0xB0)) return null;

        // Walk the inline children vector (GON objects stored by value, stride = 0xB0)
        nint arrFirst = *(nint*)(gonPtr + 0x38);
        nint arrLast  = *(nint*)(gonPtr + 0x40);
        if (IsLikelyPointer(arrFirst) && IsLikelyPointer(arrLast))
        {
            long count = (long)(arrLast - arrFirst) / 0xB0;
            if (count >= 0 && count <= 1000)
            {
                for (long i = 0; i < count; i++)
                {
                    nint childPtr = arrFirst + (nint)(i * 0xB0);
                    if (!IsMemReadable(childPtr, 0xB0)) continue;
                    string? childName = TryReadStdString(childPtr + 0x88);
                    if (childName == "name" && *(int*)(childPtr + 0xA8) == 1 /* FieldType.STRING */)
                        return TryReadStdString(childPtr + 0x68); // string_data of the "name" field
                }
            }
        }

        // Fallback: root GON node name (e.g. "BasicMelee_Fighter" — not a localization key)
        return TryReadStdString(gonPtr + 0x88);
    }
}

