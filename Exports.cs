// using MewgenicsModSdk;
// using MewgenicsModSdk.Api;
using System.Runtime.InteropServices;

namespace CatsTableMod;

internal static unsafe class Exports
{
    private static readonly CatsTableMod _mod = new();

    // [UnmanagedCallersOnly(EntryPoint = "MewMod_GetInfo")]
    // public static ModInfo* GetInfo()       { try { return ModInfoHelper.GetInfo(_mod); } catch { return null; } }

    // [UnmanagedCallersOnly(EntryPoint = "MewMod_Init")]
    // public static void Init(MewgenicsApi* api) { try { _mod.InternalLoad(api); } catch { } }

    // [UnmanagedCallersOnly(EntryPoint = "MewMod_Enable")]
    // public static void Enable()           { try { _mod.InternalEnable(); } catch { } }

    // [UnmanagedCallersOnly(EntryPoint = "MewMod_Disable")]
    // public static void Disable()          { try { _mod.InternalDisable(); } catch { } }

    // [UnmanagedCallersOnly(EntryPoint = "MewMod_ConfigReload")]
    // public static void ConfigReload()     { try { _mod.InternalConfigReload(); } catch { } }

    /// <summary>
    /// DLL entry point — called by Windows when the mod DLL is loaded into the game process.
    /// This is where mewjector is resolved and our hooks are installed, independently of the
    /// MewgenicsModSdk lifecycle.  MewMod_Init is still called later by the SDK loader and
    /// provides access to Gon, GameEvents, etc.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "DllMain")]
    public static bool DllMain(nint hModule, uint reason, nint reserved)
    {
        const uint DLL_PROCESS_ATTACH = 1;
        if (reason == DLL_PROCESS_ATTACH)
        {
            try
            {
                if (MewjectorApi.Resolve())
                    _mod.MjInit();
            }
            catch { }
        }
        return true;
    }
}