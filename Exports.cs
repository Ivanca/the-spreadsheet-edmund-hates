// using MewgenicsModSdk;
// using MewgenicsModSdk.Api;
using System.Runtime.InteropServices;


public static class Exports
{
    private static readonly CatsTableMod.CatsTableMod _mod = new();

    [UnmanagedCallersOnly(EntryPoint = "MjInit")]
    public static void MjInit()
    {
        using (var writer = new System.IO.StreamWriter("./text.txt", append: true))
        {
            writer.WriteLine("Catstable DllMain failed to get handle for version.dll");
        }
        if (CatsTableMod.MewjectorApi.Resolve())
            _mod.MjInit();
    }
}