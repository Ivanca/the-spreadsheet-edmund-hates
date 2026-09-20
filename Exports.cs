// using MewgenicsModSdk;
// using MewgenicsModSdk.Api;
using System.Runtime.InteropServices;


public static class Exports
{
    private static readonly TheSpredsheetEdmundHates _mod = new();

    [UnmanagedCallersOnly(EntryPoint = "MjInit")]
    public static void MjInit()
    {
        if (MewjectorApi.Resolve())
            _mod.MjInit();
    }
}