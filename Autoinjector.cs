// using MewgenicsModSdk;
// using MewgenicsModSdk.Game;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace CatstableMod;

public partial class CatstableMod
{

    unsafe static delegate* unmanaged<nint, nint, nint, nint> _hookProcessCmds;
    static DateTime dateInstalled = DateTime.Now;

    internal unsafe void Autoinjector()
    {
        _hookProcessCmds = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x9ab0c0, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&HookProcessCmds);

    }
    
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetModuleFileName(
        IntPtr hModule,
        StringBuilder lpFilename,
        int nSize);

    [UnmanagedCallersOnly]
    private static unsafe nint HookProcessCmds(
    nint application,
    nint argc,
    nint argv)
    {
        string[] args = ReadArgv(argv, argc);

        bool hasModPaths = false;

        foreach (string arg in args)
        {
            if (string.Equals(arg, "-modpaths",
                StringComparison.OrdinalIgnoreCase))
            {
                hasModPaths = true;
                break;
            }
        }

        string[] newArgs;
        var buffer = new StringBuilder(32768);
        uint length = GetModuleFileName(
            IntPtr.Zero,
            buffer,
            buffer.Capacity);

        if (length == 0)
        {
            LogStr($"[HOOK] HookProcessCmds: GetModuleFileName failed with error {Marshal.GetLastWin32Error()}");
            return _hookProcessCmds(application, argc, argv);
        }

        string ModPath = Path.GetDirectoryName(buffer.ToString())! + "/mods/catstable";

        // get the dateCreated time of the dll file
        string dllPath = Path.Combine(ModPath, "catstable.dll");
        dateInstalled = File.GetCreationTime(dllPath);
        LogStr($"[HOOK] HookProcessCmds: ModPath={ModPath} hasModPaths={hasModPaths} args={string.Join(" ", args)} dateInstalled={dateInstalled:yyyy-MM-dd HH:mm:ss} ");

        dateInstalled = DateTime.Now.AddDays(-94); // for testing purposes, set the dateInstalled

        if (hasModPaths)
        {
            // check if "catstable" is already present in the -modpaths
            bool hasCatStable = false;
            foreach (string arg in args)
            {
                if (arg.Contains("catstable"))
                {
                    hasCatStable = true;
                    break;
                }
            }
            if (!hasCatStable)
            {
                return _hookProcessCmds(application, argc, argv);
            }
            // Existing -modpaths handling accepts additional paths.
            newArgs = new string[args.Length + 1];

            Array.Copy(args, newArgs, args.Length);

            newArgs[^1] = ModPath;
        }
        else
        {
            // Insert -modpaths + our path immediately after argv[0].
            newArgs = new string[args.Length + 2];

            newArgs[0] = args[0];
            newArgs[1] = "-modpaths";
            newArgs[2] = ModPath;

            Array.Copy(
                args,
                1,
                newArgs,
                3,
                args.Length - 1);
        }

        IntPtr newArgv = AllocateArgv(newArgs);

        try
        {
            _hookProcessCmds(
                application,
                newArgs.Length,
                newArgv);
        }
        finally
        {
            FreeArgv(newArgv, newArgs.Length);
        }
        return 0;
    }


    private static string[] ReadArgv(nint argv, nint argc)
    {
        string[] result = new string[argc];

        for (int i = 0; i < argc; i++)
        {
            IntPtr p = Marshal.ReadIntPtr(
                argv,
                i * IntPtr.Size);

            result[i] = Marshal.PtrToStringAnsi(p) ?? "";
        }

        return result;
    }

    private static IntPtr AllocateArgv(string[] args)
    {
        IntPtr argv = Marshal.AllocHGlobal(
            args.Length * IntPtr.Size);

        for (int i = 0; i < args.Length; i++)
        {
            IntPtr str = Marshal.StringToHGlobalAnsi(args[i]);

            Marshal.WriteIntPtr(
                argv,
                i * IntPtr.Size,
                str);
        }

        return argv;
    }

    private static void FreeArgv(IntPtr argv, int argc)
    {
        for (int i = 0; i < argc; i++)
        {
            IntPtr str = Marshal.ReadIntPtr(
                argv,
                i * IntPtr.Size);

            Marshal.FreeHGlobal(str);
        }

        Marshal.FreeHGlobal(argv);
    }


}