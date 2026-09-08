// using MewgenicsModSdk;
// using MewgenicsModSdk.Api;
using System.Runtime.InteropServices;


public static class Exports
{
    private static readonly CatsTableMod.CatsTableMod _mod = new();

    [UnmanagedCallersOnly(EntryPoint = "MjInit")]
    public static void MjInit()
    {
        if (CatsTableMod.MewjectorApi.Resolve())
            _mod.MjInit();
    }
}