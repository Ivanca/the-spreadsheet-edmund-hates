using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CatsTableMod;

/// <summary>
/// C# binding for the Mewjector chainloader API (version.dll).
///
/// Mirrors all typedefs in mewjector.h.  Call <see cref="Resolve"/> once
/// during DLL init (DLL_PROCESS_ATTACH) before using anything else.
///
/// Hook chaining: when multiple mods hook the same RVA, Mewjector chains
/// them by priority (lower = called first).  Each hook's trampoline calls
/// the NEXT hook in the chain, not necessarily the original game function.
/// </summary>
internal static unsafe class MewjectorApi
{
    private const int    MJ_API_VERSION = 3;
    private const string MOD_NAME       = "catstable";

    // ── Win32 ────────────────────────────────────────────────────────────────

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    private static extern nint GetModuleHandleA(string? lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    private static extern nint GetProcAddress(nint hModule, string lpProcName);

    // ── Function pointer fields (resolved at runtime from version.dll) ───────
    //
    // Each field maps 1-to-1 to a typedef in mewjector.h.

    // int __cdecl (UINT_PTR rva, int stolenBytes, void* hookFn,
    //              void** outTrampoline, int priority, const char* owner)
    private static delegate* unmanaged[Cdecl]<nuint, int, void*, void**, int, byte*, int> _installHook;

    // int __cdecl (UINT_PTR rva)
    private static delegate* unmanaged[Cdecl]<nuint, int> _queryHook;

    // UINT_PTR __cdecl (const char* owner)
    private static delegate* unmanaged[Cdecl]<byte*, nuint> _allocTypeIdPair;

    // int __cdecl (const char* category, const char* name, const char* owner)
    private static delegate* unmanaged[Cdecl]<byte*, byte*, byte*, int> _registerName;

    // const char* __cdecl (const char* category, const char* name)
    private static delegate* unmanaged[Cdecl]<byte*, byte*, byte*> _lookupName;

    // UINT_PTR __cdecl (void)
    private static delegate* unmanaged[Cdecl]<nuint> _getGameBase;

    // void __cdecl (const char* owner, const char* fmt, ...)
    // We call it with no varargs — message is pre-formatted and passed as fmt.
    // cdecl callee-clean convention makes this safe; we just escape any literal %.
    private static delegate* unmanaged[Cdecl]<byte*, byte*, void> _log;

    // int __cdecl (void)
    private static delegate* unmanaged[Cdecl]<int> _verifyHooks;

    // int __cdecl (void)
    private static delegate* unmanaged[Cdecl]<int> _getVersion;

    // ── State ────────────────────────────────────────────────────────────────

    /// <summary>True after a successful <see cref="Resolve"/> call.</summary>
    public static bool IsAvailable { get; private set; }

    /// <summary>Game executable base address, as reported by mewjector.</summary>
    public static nuint GameBase { get; private set; }

    // ── Resolve ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Resolves all Mewjector API functions from the running version.dll.
    /// Returns <c>true</c> if successful.  Safe to call when Mewjector is absent.
    /// Idempotent — subsequent calls return the cached result immediately.
    /// </summary>
    public static bool Resolve()
    {
        if (IsAvailable) return true;

        nint hMJ = GetModuleHandleA("version.dll");
        
        if (hMJ == 0) {
            using (var writer = new System.IO.StreamWriter("./text.txt", append: true))
            {
                writer.WriteLine("Catstable DllMain failed to get handle for version.dll");
            }
            return false;
        }


        // Version gate: reject older chainloaders before touching any other export.
        nint fnVer = GetProcAddress(hMJ, "MJ_GetVersion");
        if (fnVer == 0) {
            using (var writer = new System.IO.StreamWriter("./text.txt", append: true))
            {
                writer.WriteLine("Catstable DllMain failed to get address for MJ_GetVersion");
            }
            return false;
        }
        _getVersion = (delegate* unmanaged[Cdecl]<int>)fnVer;
        if (_getVersion() < MJ_API_VERSION) return false;

        if (!Bind(hMJ, "MJ_InstallHook",     out nint p0)) return false;
        if (!Bind(hMJ, "MJ_QueryHook",       out nint p1)) return false;
        if (!Bind(hMJ, "MJ_AllocTypeIdPair", out nint p2)) return false;
        if (!Bind(hMJ, "MJ_RegisterName",    out nint p3)) return false;
        if (!Bind(hMJ, "MJ_LookupName",      out nint p4)) return false;
        if (!Bind(hMJ, "MJ_GetGameBase",     out nint p5)) return false;
        if (!Bind(hMJ, "MJ_Log",             out nint p6)) return false;
        if (!Bind(hMJ, "MJ_VerifyHooks",     out nint p7)) return false;

        _installHook     = (delegate* unmanaged[Cdecl]<nuint, int, void*, void**, int, byte*, int>)p0;
        _queryHook       = (delegate* unmanaged[Cdecl]<nuint, int>)p1;
        _allocTypeIdPair = (delegate* unmanaged[Cdecl]<byte*, nuint>)p2;
        _registerName    = (delegate* unmanaged[Cdecl]<byte*, byte*, byte*, int>)p3;
        _lookupName      = (delegate* unmanaged[Cdecl]<byte*, byte*, byte*>)p4;
        _getGameBase     = (delegate* unmanaged[Cdecl]<nuint>)p5;
        _log             = (delegate* unmanaged[Cdecl]<byte*, byte*, void>)p6;
        _verifyHooks     = (delegate* unmanaged[Cdecl]<int>)p7;

        GameBase    = _getGameBase();
        IsAvailable = true;
        return true;
    }

    private static bool Bind(nint hMod, string name, out nint fn)
    {
        fn = GetProcAddress(hMod, name);
        return fn != 0;
    }

    // ── Public wrappers ──────────────────────────────────────────────────────

    /// <summary>
    /// Sends a message to the shared Mewjector log.
    /// Any literal <c>%</c> in <paramref name="message"/> is escaped so that
    /// mewjector's internal printf doesn't misinterpret it.
    /// </summary>
    public static void Log(string message)
    {
        if (!IsAvailable) return;
        // Escape % → %% so printf in mewjector treats them as literals.
        string safe = message.IndexOf('%') >= 0 ? message.Replace("%", "%%") : message;
        fixed (byte* pOwner = Utf8Z(MOD_NAME))
        fixed (byte* pMsg   = Utf8Z(safe))
            _log(pOwner, pMsg);
    }

    /// <summary>
    /// Installs a chainable function detour at <paramref name="rva"/> via Mewjector.
    /// Pass <paramref name="stolenBytes"/>=0 (default) to let Mewjector's built-in
    /// LDE auto-detect the stolen byte count from the function prologue.
    /// </summary>
    /// <param name="rva">Target RVA inside the game executable.</param>
    /// <param name="hookFn">
    ///   Pointer to an <c>[UnmanagedCallersOnly]</c> static method whose signature
    ///   matches the target function.
    /// </param>
    /// <param name="priority">
    ///   Hook priority — lower value fires first in the chain (default 10).
    /// </param>
    /// <param name="stolenBytes">
    ///   Override the stolen byte count.  0 = auto-detect via LDE.
    /// </param>
    /// <returns>
    ///   Trampoline pointer to cast and call as the original function,
    ///   or zero if the installation failed.
    /// </returns>
    public static unsafe delegate* unmanaged<long, long, nint, nint, nint> InstallHook(long rva, void* hookFn, int priority = 10, int stolenBytes = 0)
    {
        if (!IsAvailable)
            throw new InvalidOperationException("MewjectorApi.Resolve() has not been called or failed");
        void* trampoline = null;
        fixed (byte* pOwner = Utf8Z(MOD_NAME))
            _installHook((nuint)rva, stolenBytes, hookFn, &trampoline, priority, pOwner);
        return (delegate* unmanaged<long, long, nint, nint, nint>)trampoline;
    }

    /// <returns>1 if any mod has hooked <paramref name="rva"/>, 0 otherwise.</returns>
    public static int QueryHook(long rva) => IsAvailable ? _queryHook((nuint)rva) : 0;

    /// <returns>Pass count from Mewjector's trampoline integrity check.</returns>
    public static int VerifyHooks() => IsAvailable ? _verifyHooks() : 0;

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static byte[] Utf8Z(string s) => Encoding.UTF8.GetBytes(s + "\0");
}
