global using static Utils;

// using MewgenicsModSdk;
// using MewgenicsModSdk.Game;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Linq;


static class TestButtonCallbackClass
{
    [UnmanagedCallersOnly]
    public unsafe static nint TestButtonCallback(nint callbackObject)
    {
        // return _callbackObject);
        // This is never executed, don't know why, doesn't matter because
        // we just intercept our buttons at ClickHandlerHook
        return 0;
    }
}

public interface OurCallback
{
    void Invoke();
}


class ModButton<T>: OurCallback where T : class? {
    
    public nint MovieClipAddress;

    public Action<nint, T?> _callback;
    public T? _data;

    public ModButton(T? data, Action<nint, T?> callback)
    {
        _data = data;
        _callback = callback;
    }

    public void Invoke()
    {
        _callback?.Invoke(MovieClipAddress, _data);
    }
}

public static class ButtonManager
{

    // createButton overload that doesn't require initial data
    public static unsafe void createButton(string id, nint menupanel, Action<nint> ourCallback)
    {
        createButton<object>(id, menupanel, (nint btn, object? _) => ourCallback(btn), initialData: null);
    }
    
    public static unsafe void createButton<T>(string id, nint menupanel, Action<nint, T?> ourCallback, T? initialData) where T : class?
    {
        nint callbackVtable = Marshal.AllocHGlobal(0x30);
        // Empty callback storage for the experiment.
        nint callbackStorage = Marshal.AllocHGlobal(0x20);
        NativeMemory.Clear((void*)callbackStorage, 0x20);

        Marshal.WriteIntPtr(callbackStorage + 0x00, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x08, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x10, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x18, 15);

        Buffer.MemoryCopy(
            (void*)(MewjectorApi.GameBase + 0xee7e50),
            (void*)callbackVtable,
            0x30,
            0x30
        );
        Marshal.WriteIntPtr(
            callbackVtable + 0x10,
            (nint)(delegate* unmanaged<nint, nint>)&TestButtonCallbackClass.TestButtonCallback
        );
        nint fakeCallback = Marshal.AllocHGlobal(0x40);
        NativeMemory.Clear((void*)fakeCallback, 0x40);

        Marshal.WriteIntPtr(fakeCallback + 0x00, callbackVtable);
        Marshal.WriteIntPtr(fakeCallback + 0x38, fakeCallback);
        // ------------------------------------------------------------
        ModButton<T> button = new ModButton<T>(initialData, ourCallback);
        var buttonAddress = (nint)TheSpredsheetEdmundHates.registerButton(
            menupanel,
            GameString.Create(id),
            callbackStorage,
            fakeCallback
        );
        button.MovieClipAddress = buttonAddress;
        ((Button*)buttonAddress)->Flags = 0x000003EA;
        allOurButtons[buttonAddress] = button;
        // allOurButtons.Add(buttonAddress, button);
    }
    
    public static bool handleNativeClick(nint btnAddress)
    {
        if (allOurButtons.TryGetValue(btnAddress, out var button))
        {
            button.Invoke();
            return true;
        }
        return false;
    }

    static Dictionary<nint, OurCallback> allOurButtons = new Dictionary<nint, OurCallback>();

}