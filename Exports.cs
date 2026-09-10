// using MewgenicsModSdk;
// using MewgenicsModSdk.Api;
using System.Runtime.InteropServices;


public static class Exports
{
    private static readonly TheSpredsheetEdmundHates.TheSpredsheetEdmundHates _mod = new();

    [UnmanagedCallersOnly(EntryPoint = "MjInit")]
    public static void MjInit()
    {
        if (TheSpredsheetEdmundHates.MewjectorApi.Resolve())
            _mod.MjInit();
    }
}