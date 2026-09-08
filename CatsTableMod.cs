// using MewgenicsModSdk;
// using MewgenicsModSdk.Game;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace CatsTableMod;


public partial class CatsTableMod
{
    static bool _debugLogging = true;

    static unsafe delegate* unmanaged<nint, nint> _updatePanelLayout;

    static unsafe delegate* unmanaged<nint, nint, nint, nint> _createUiRenderer;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> _createCatsDrawerHousePanel;
    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _registerButton;
    static unsafe delegate* unmanaged<nint, nint> _statsCreator;
    static unsafe delegate* unmanaged<nint, nint, nint> _getHouseCatByOffset;
    static unsafe delegate* unmanaged<nint, nint> _gameTick;
    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _catIterator;

    static unsafe delegate* unmanaged<nint, nint, nint, nint> _findButton;
    static unsafe delegate* unmanaged<nint, nint> _initCatStatsClickCallback;
    static unsafe delegate* unmanaged<nint, nint> _toggleHouseDrawer;
    static unsafe delegate* unmanaged<nint, nint, nint> _clickHandler;
    static unsafe delegate* unmanaged<nint, nint, nint> _mutationToolTip;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> _createMenuPanel;

    unsafe static delegate* unmanaged<nint, nint, nint> _mouseEventHandler;

    public unsafe static delegate* unmanaged<nint, char*, nuint, nint> assignString;

    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _globalResourceManagerLookup;
    
    unsafe static delegate* unmanaged<nint, nint, nint> _setText;
    unsafe static delegate* unmanaged<nint, nint> _endDay;
    unsafe static delegate* unmanaged<nint, nint> _goToMainMenu;
    unsafe static delegate* unmanaged<nint, nint, nint, nint, nint> _setCatData;
    unsafe static delegate* unmanaged<nint, nint, nint> _getChild;
    unsafe static delegate* unmanaged<nint, nint, nint, void> _trackMovieclipChildParentOffset;

    unsafe static delegate* unmanaged<nint, nint, uint, void> _attachChild;
    unsafe static delegate* unmanaged<nint, nint, nint> _createCatStatsDrawer;
    unsafe static delegate* unmanaged<nint, nint> _catStatsDrawerUpdate;
    
    unsafe static T Read<T>(nint p) where T : unmanaged
        => *(T*)p;

    unsafe static void Write<T>(nint address, T value) where T : unmanaged
    {
        *(T*)address = value;
    }

    private static unsafe bool TryReadPointer(nint address, out nint value)
    {
        value = 0;

        if (!IsMemReadable(address, IntPtr.Size))
            return false;

        value = Read<nint>(address);
        return value != 0;
    }

    private static bool TryFollow(nint address, out nint result, params nuint[] offsets)
    {
        result = address;

        foreach (var offset in offsets)
        {
            if (!TryReadPointer(result + (nint)offset, out result))
                return false;
        }

        return true;
    }
    
    static nint _rightStr = 0;
    static nint _leftStr = 0;
    static Dictionary<string, int> executionCounts = new();

    private static void CountExecution(string methodName)
    {
        if (_debugLogging)
        {
            executionCounts[methodName] = executionCounts.GetValueOrDefault(methodName) + 1;
        }
    }

    internal unsafe void MjInit()
    {
        // var tenSecondsAfterNow = DateTime.Now.AddSeconds(15);
        // LogStr($"Waiting for 15 seconds to connect using x64dbg...");
        // while (DateTime.Now < tenSecondsAfterNow)
        // {
        //     Thread.Sleep(1);
        // }

        var buffer = new StringBuilder(32768);
        uint length = GetModuleFileName(IntPtr.Zero, buffer, buffer.Capacity);
        var gamePath = buffer.ToString();
        LogStr($"MjInit: gamePath={gamePath} length={length}");
        bool valid = BinaryValidator.Validate(gamePath);

        if (!valid)
        {
            LogStr("Binary validation failed.");
            return;
        }

        Autoinjector();

        InitMouse();

        _updatePanelLayout = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x204320, (void*)(delegate* unmanaged<nint, nint>)&UpdatePanelLayoutHook);
        
        // _globalResourceManagerLookup = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
        //     0x9adc50, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&GlobalResourceManagerLookupHook);

        _createUiRenderer = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x5A3D0, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&CreateUiRendererHook);

        _createCatsDrawerHousePanel = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xef570, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&CreatePanelHook);

        _registerButton = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x97C070, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&RegisterCallbackHook);

        _statsCreator = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xE9AC0, (void*)(delegate* unmanaged<nint, nint>)&CreateCatStatsDrawerHook);

        _getHouseCatByOffset = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xEAC70, (void*)(delegate* unmanaged<nint, nint, nint>)&GetHouseCatByOffsetHook);

        _findButton = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x980E60, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&FindButtonHook);

        _initCatStatsClickCallback = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xEF3D0, (void*)(delegate* unmanaged<nint, nint>)&InitCatStatsCallbackHook);

        _toggleHouseDrawer = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x203C80, (void*)(delegate* unmanaged<nint, nint>)&ToggleHouseDrawerHook);

        _clickHandler = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x97E8E0, (void*)(delegate* unmanaged<nint, nint, nint>)&ClickHandlerHook);
        
        _gameTick = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x96AC50, (void*)(delegate* unmanaged<nint, nint>)&GameTickHook);

        _catIterator = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xED220, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&CatIteratorHook);

        _mutationToolTip = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xE5500, (void*)(delegate* unmanaged<nint, nint, nint>)&MutationTooltipHook); 

        _setText = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x98E8A0, (void*)(delegate* unmanaged<nint, nint, nint>)&SetTextHook);

        // EndDayHook and GoToMainMenuHook are the only places where mod state is reset

        _endDay = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x1f8ea0, (void*)(delegate* unmanaged<nint, nint>)&EndDayHook);

        _goToMainMenu = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x29cef0, (void*)(delegate* unmanaged<nint, nint>)&GoToMainMenuHook);
        
        _setCatData = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xe1a00, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&SetCatDataHook);

        _mouseEventHandler = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xc36110, (void*)(delegate* unmanaged<nint, nint, nint>)&MouseWheelHook);
        
        _catStatsDrawerUpdate = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xeb170, (void*)(delegate* unmanaged<nint, nint>)&CatStatsDrawerUpdateHook);
        

        LogStr($"Gamebase at {MewjectorApi.GameBase:X}...");

        assignString = (delegate* unmanaged<nint,char*,nuint,nint>)(MewjectorApi.GameBase + 0x5b150);
        _getChild = (delegate* unmanaged<nint,nint,nint>)(MewjectorApi.GameBase +0x99a0e0);
        _trackMovieclipChildParentOffset = (delegate* unmanaged<nint, nint, nint, void>)(MewjectorApi.GameBase + (nuint)0x204a80);
        _attachChild = (delegate* unmanaged<nint, nint, uint, void>)(MewjectorApi.GameBase + (nuint)0x999e40);
        _createCatStatsDrawer =  (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x1ace50);

    }

    static List<nint> cachedPointers = new();

    static string sortByStat = "";
    private enum SortDirection
    {
        Ascending,
        Descending
    }

    static SortDirection sortByStatDirection = SortDirection.Descending;
    static Dictionary<nint, double> averages = new();
    static nint lastCamera = 0;
    static bool InsideSetCatData = false;

    [UnmanagedCallersOnly]
    static unsafe nint SetCatDataHook(nint a1, nint a2, nint a3, nint a4)
    {
        LogStr($"[HOOK] SetCatDataHook: a1=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        InsideSetCatData = true;
        var result = _setCatData(a1, a2, a3, a4);
        InsideSetCatData = false;
        return result;
    }

    [UnmanagedCallersOnly]
    static unsafe nint GetMouseSourceHook(nint a1)
    {
        LogStr($"[HOOK] GetMouseSourceHook: a1=0x{a1:X}");
        lastCamera = a1;
        return _getMouseSource(a1);
    }

    [UnmanagedCallersOnly]
    static unsafe nint GoToMainMenuHook(nint a1)
    {
        LogStr($"[HOOK] GoToMainMenuHook: a1=0x{a1:X}");
        ClearState();
        return _goToMainMenu(a1);
    }

    [UnmanagedCallersOnly]
    static unsafe nint EndDayHook(nint a1)
    {
        // CountExecution(nameof(EndDayHook));
        LogStr($"[HOOK] EndDayHook: a1=0x{a1:X}");
        ClearState();
        return _endDay(a1);
    }

    [UnmanagedCallersOnly]
    static unsafe nint SetTextHook (nint a1, nint a2)
    {
        if (!InsideSetCatData)
        {
            return _setText(a1, a2);
        }
        CountExecution(nameof(SetTextHook));

        var renderer = GetRenderer(a1);
        if (rowRenderers.Length > 0 && rowRenderers.Contains(renderer))
        {
            if (!cachedPointers.Contains(a1)) {
                cachedPointers.Add(a1);
            } else {
                // LogStr($"[HOOK] SetTextHook: a1=0x{a1:X} is already cached, skipping");
                return _setText(a1, a2);
            }
            var stat = getStat(a1);
            if (stat != "")
            {
                // LogStr($"[HOOK] SetTextHook: a1=0x{a1:X} a2=0x{a2:X} is one of our row renderers, stat={stat}, renderer={renderer:X}");
                var str = ReadUtf16CustomString(a2);
                if (str == "")
                {
                    str = "0";
                }
                catStats[renderer][stat] = int.Parse(str);
                noop = Read<byte>((nint)MewjectorApi.GameBase + 0x60);

                LogStr($"[HOOK] SetTextHook: updated catStats for renderer 0x{renderer:X}, stat={stat}, value={catStats[renderer][stat]}");
                var index = Array.IndexOf(rowRenderers, renderer);
                double average = catStats[renderer].Where(kv => kv.Key != "bdc" && kv.Key != "muc").Average(kv => kv.Value);
                // force it to have 1 single decimal place:
                average = Math.Round(average, 1);
                averages[renderer] = (double)average;
                averagesDirty = true;
                // LogStr($"[HOOK] SetTextHook: average={average} for renderer 0x{renderer:X} at index {index}");
            }
            // LogStr($"[HOOK] SetTextHook: a1=0x{a1:X} is one of our row renderers, text={str}, renderer={renderer:X}");
        }
        return _setText(a1, a2);
    }

    [UnmanagedCallersOnly]
    static unsafe nint ClickHandlerHook(nint a1, nint a2)
    {
        CountExecution(nameof(ClickHandlerHook));

        if (a1 != 0 && IsMemReadable(a1 + 0x48, 8))
        {
            if (OurHeaderbuttons.ContainsValue(a1))
            {            
                LogStr($"[HOOK] ClickHandlerHook: a1=0x{a1:X} is one of our header buttons, ignoring click");
                var movieclip = Read<nint>(a1 + 0x48);
                var movieclipname = TryReadCString(Read<nint>(movieclip + 0x48));
                // get first 3 letters
                var subname = movieclipname.Substring(0, Math.Min(3, movieclipname.Length));
                if (stats.Contains(subname) || subname == "avg")
                {
                    if (sortByStat == subname)
                    {
                        sortByStatDirection = sortByStatDirection == SortDirection.Ascending
                            ? SortDirection.Descending
                            : SortDirection.Ascending;
                    } else
                    {
                        sortByStat = subname;
                        sortByStatDirection = SortDirection.Descending;
                    }
                    SortCats();
                    LogStr($"[HOOK] ClickHandlerHook: sorting by {sortByStat} {sortByStatDirection}");
                }
                return 0;
            } else if (a1 == footerButton)
            {
                LogStr($"[HOOK] ClickHandlerHook: a1=0x{a1:X} is our footer button, opening kofi link");
                Process.Start(new ProcessStartInfo
                {
                    FileName = showingChimplantsPromo ? "https://www.chimplants.com/" : "https://ko-fi.com/chimplants",
                    UseShellExecute = true
                });
                return 0;
            }
        }
        var result = _clickHandler(a1, a2);
        return result;
    }


    static unsafe void SortCats()
    {
        CountExecution(nameof(SortCats));
        positionDirty = true;
        if (sortByStat == "")
        {
            sortedCats = catStats.Select(e => e.Key).ToArray();
            // print the full SortedCats array
            LogStr($"[HOOK] SortCats: sortByStat is empty, sortedCats = {string.Join(", ", sortedCats.Select(e => e.ToString("X")))}");
            return;
        }

        var sorted = sortByStat == "avg"
            ? catStats.OrderBy(e => averages.ContainsKey(e.Key) ? averages[e.Key] : 0)
            : catStats.OrderBy(e => e.Value.ContainsKey(sortByStat) ? e.Value[sortByStat] : 0);
        sortedCats = sorted.Select(e => e.Key).Where(e => visibleRenderers.Contains(e)).ToArray();
        if (sortByStatDirection == SortDirection.Descending)
        {
            sortedCats = sortedCats.Reverse().ToArray();
        }
        LogStr($"[HOOK] SortCats: sortByStat={sortByStat} sortByStatDirection={sortByStatDirection}, sortedCats = {string.Join(", ", sortedCats.Select(e => e.ToString("X")))}");
    }

    static nint currentlyDrawerWithOpenIconsPanel = 0;
    static nint currentlyHoveredDrawer = 0;
    static nint forcedCatStatsUpdatePending = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CatStatsDrawerUpdateHook(nint a1)
    {
        if (forcedCatStatsUpdatePending > 0)
        {
            forcedCatStatsUpdatePending--;
        } else
        {
            if (currentlyDrawerWithOpenIconsPanel == 0)
            {
                if (currentlyHoveredDrawer != a1)
                {
                    return 0;
                }
            }
            if (currentlyDrawerWithOpenIconsPanel != 0 && currentlyDrawerWithOpenIconsPanel != a1)
            {
                return 0;
            }
            if (!ourPanelIsOpen)
            {
                return 0;
            }
        }

        CountExecution(nameof(CatStatsDrawerUpdateHook));

        var result = _catStatsDrawerUpdate(a1);
        var iconsPanelIsOpen = Read<byte>(a1 + 0x71);
        if (iconsPanelIsOpen == 1)
        {
            currentlyDrawerWithOpenIconsPanel = a1;
        } else
        {
            currentlyDrawerWithOpenIconsPanel = 0;
        }
        return result;
    }


    static nint[] cachedVisibleCats = Array.Empty<nint>();

    [UnmanagedCallersOnly]
    static unsafe nint CatIteratorHook (nint a1, nint a2, nint a3, nint a4)
    {
        CountExecution(nameof(CatIteratorHook));

        cachedVisibleCats = new nint[a3];
        LogStr($"[HOOK] CatIteratorHook: a1={a1:X} a2={a2:X} a3={a3} a4={a4:X}");

        for (int i = 0; i < a3; i++) {
            cachedVisibleCats[i] = Marshal.ReadIntPtr(a1 + i * 8);
            LogStr($"[HOOK] CatIteratorHook: cachedVisibleCats[{i}] = {cachedVisibleCats[i]:X}");
        }
            
        var result = _catIterator(a1, a2, a3, a4);
        return result;
    }


    static Dictionary<nint, byte> _visibilityBefore = new Dictionary<nint, byte>();
    static bool buttonsInitialized = false;
    [UnmanagedCallersOnly]
    static unsafe nint ToggleHouseDrawerHook(nint a1)
    {
        if (originalHousePanel == 0)
        {
            return _toggleHouseDrawer(a1);
        }
        CountExecution(nameof(ToggleHouseDrawerHook));

        nint panel = Marshal.ReadIntPtr(a1 + 0x58);
        nint state = Marshal.ReadIntPtr(a1 + 0x50);
        var changed = false;

        if (panel != originalHousePanel && ourPanelIsOpen && state == 0)
        {
            LogStr($"[HOOK] Panel Close, state={state:X}");
            ourPanelIsOpen = false;
            changed = true;
            panelAnimationInProgress = true;
            waitingForPanelStatus = PanelStateClosed;
        }
        else if (panel == originalHousePanel && !ourPanelIsOpen && state == 1)
        {
            // print executionCounts for debugging:
            LogStr($"[HOOK] ToggleHouseDrawerHook: state={state:X}");
            executionCounts = executionCounts.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
            LogStr($"[HOOK] ToggleHouseDrawerHook: executionCounts = {string.Join(", ", executionCounts.Select(kv => $"{kv.Key}={kv.Value}"))}");
            // OPEN
            LogStr($"[HOOK] Panel Open");
            yOffset = 0;
            yOffsetTarget = 0;
            yScrollAni = null;
            ourPanelIsOpen = true;
            cachedPointers.Clear();
            LogStr($"[HOOK] Before initializing buttons");
            forcedCatStatsUpdatePending = rowRenderers.Length;
            initializeButtons();
            LogStr($"[HOOK] After initializing buttons");
            changed = true;
            panelAnimationInProgress = true;
            waitingForPanelStatus = PanelStateOpen;
            if (rowRenderers.Length >= cachedVisibleCats.Length)
            {
                for (int i = 0; i < cachedVisibleCats.Length; i++)
                {
                    var rowRenderer = rowRenderers[i];
                    Write(rowRenderer + 0x51, (byte)1);
                }
            }
        } else
        {
            // LogStr($"[HOOK] Panel state unchanged, panel={panel:X}, state={state:X} originalHousePanel={originalHousePanel:X} ourPanelIsOpen={ourPanelIsOpen}");
        }
        if (changed)
        {
            xOffsetTarget = ourPanelIsOpen ? 10 : -35;
            xMoveAni = new FloatAnimator(xOffset, xOffsetTarget, 0.5f);
            positionDirty = true;
        }
        return _toggleHouseDrawer(a1);
    }


    static nint _lastButtonCSD = 0;
    [UnmanagedCallersOnly]
    static unsafe nint FindButtonHook(nint a1, nint a2, nint a3)
    {
        var result = _findButton(a1, a2, a3);
        if (!ourPanelIsOpen)
        {
            return result;
        }
        CountExecution(nameof(FindButtonHook));
        if (result != 0)
        {
            // LogStr($"[HOOK] result!=0 {result:X} x={x:X}");
            _lastButtonCSD = Read<nint>(Read<nint>(Read<nint>(Read<nint>(result + 0x38) + 0x18) + 0x28) + 0x10);
        } else
        {
            // LogStr($"[HOOK] EMPTY! _lastButtonCSD = 0");
            _lastButtonCSD = 0;
        }
        return result;
    }

    [UnmanagedCallersOnly]
    static unsafe nint MutationTooltipHook(nint a1, nint a2)
    {
        CountExecution(nameof(MutationTooltipHook));

        if (a1 == 0)
        {
            return 0;
        }
        if (_lastButtonCSD != 0)
        {
            var opt1 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28) + 0x10);
            var opt2 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28));
            // LogStr($"[HOOK] opt1={opt1:X} opt1={opt2:X} _lastButtonCSD={_lastButtonCSD:X}");
            if (opt1 == _lastButtonCSD || opt2 == _lastButtonCSD)
            {
                // LogStr($"[HOOK] MATCH!");
                return _mutationToolTip(a1, a2);
            }
            else
            {
                // LogStr($"[HOOK] MutationTooltipHook: Return zero !");
                return 0;
            }
        }
        // LogStr($"[HOOK] _lastButtonCSD == 0");
        return _mutationToolTip(a1, a2);
    }

    

    static bool initializedButtons = false;
    // static List<nint> headerButtons = new();
    static Dictionary<string, nint> OurHeaderbuttons = new Dictionary<string, nint>();
    static nint footerButton = 0;
    static unsafe void initializeButtons()
    {
        if (initializedButtons)
            return;

        initializedButtons = true;

        nint headersEntity = Marshal.ReadIntPtr(headersRenderer + 0x18);
        nint callbackVtable = Marshal.AllocHGlobal(0x30);

        Buffer.MemoryCopy(
            (void*)(MewjectorApi.GameBase + 0xee7e50),
            (void*)callbackVtable,
            0x30,
            0x30
        );

        Marshal.WriteIntPtr(
            callbackVtable + 0x10,
            (nint)(delegate* unmanaged<nint, nint>)&TestButtonCallback
        );
        nint callback = Marshal.AllocHGlobal(0x40);
        NativeMemory.Clear((void*)callback, 0x40);

        Marshal.WriteIntPtr(callback + 0x00, callbackVtable);
        Marshal.WriteIntPtr(callback + 0x38, callback);
        // ------------------------------------------------------------
        // Create a MenuPanel for the RowHeaders renderer/entity.
        // ------------------------------------------------------------
        // var menuEntity = _createEntity(headersEntity);
        var _createMenuPanel =
            (delegate* unmanaged<nint, nint, nint>)
            (MewjectorApi.GameBase + 0xe9340);;

        nint headerMenuPanel = _createMenuPanel(
            headersEntity,
            GameString.Create("row_headers")
        );

        LogStr($"Created headerMenuPanel = 0x{headerMenuPanel:X}");

        if (headerMenuPanel == 0)
            return;

        // Empty callback storage for the experiment.
        nint callbackStorage = Marshal.AllocHGlobal(0x20);
        NativeMemory.Clear((void*)callbackStorage, 0x20);

        Marshal.WriteIntPtr(callbackStorage + 0x00, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x08, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x10, 0);
        Marshal.WriteIntPtr(callbackStorage + 0x18, 15);

        for (int i = 0; i < buttonList.Length; i++)
        {
            var btnName = buttonList[i];
            nint newButton = _registerButton(
                headerMenuPanel,
                GameString.Create(btnName),
                callbackStorage,
                callback
            );
            LogStr($"Registered {btnName} with the game's register-newButton function, result = 0x{newButton:X}");
            Write(newButton + 0x50, 0x000003EA);
            OurHeaderbuttons[btnName] = newButton;
        }   

        if (footerRenderer != 0)
        {
            nint footerEntity = Marshal.ReadIntPtr(footerRenderer + 0x18);
            nint footerMenuPanel = _createMenuPanel(
                footerEntity,
                GameString.Create("row_footer")
            );
            LogStr($"Created footerMenuPanel MenuPanel = 0x{footerMenuPanel:X}");
            footerButton = _registerButton(
                footerMenuPanel,
                GameString.Create(showingChimplantsPromo ? "chimp_btn" : "kofi_btn"),
                callbackStorage,
                callback
            );
            Write(footerButton + 0x50, 0x000003EA);
            LogStr($"Registered footer btn with the game's register-newButton function, result = 0x{footerButton:X}");
        }

    }

    [UnmanagedCallersOnly]
    static nint TestButtonCallback(nint callbackObject)
    {
        // This is never executed, don't know why, doesn't matter because
        // we just intercept our buttons at ClickHandlerHook
        return 0;
    }
    
    private unsafe static float xOffset = -300f;
    private unsafe static float xOffsetTarget = -300f;
    static FloatAnimator? xMoveAni;

    private unsafe static float yOffset = 0;
    private unsafe static float yOffsetTarget = 0;
    static FloatAnimator? yScrollAni;
    [UnmanagedCallersOnly]
    static unsafe nint MouseWheelHook(nint a1, nint a2)
    {
        if(ourPanelIsOpen && IsLikelyPointer(a2))
        {   
            var somethingCoveringOurPanel = Read<bool>(Read<nint>(originalDrawer + 0x20) + 0x4DA);

            var a2Val = Marshal.ReadInt32(a2);
            if (a2Val == 1027 && !somethingCoveringOurPanel)
            {
                CountExecution(nameof(MouseWheelHook));
                if (visibleRenderers.Count > 10)
                {
                    var intValue = Marshal.ReadInt32(a2 + 0x1C);
                    var scrollable = visibleRenderers.Count - 10;
                    float floatValue = BitConverter.Int32BitsToSingle(intValue);
                    // LogStr($"Scroll! {floatValue}");
                    var _yOffsetTarget = yOffset - 5 * (int)floatValue;
                    if (_yOffsetTarget < 0)
                    {
                        _yOffsetTarget = 0;
                    }
                    // cuando son 12 llega demasiado lejos (16)
                    // cuando son 23 llega a la distance correcta (27)
                    if (_yOffsetTarget > scrollable * 1.8 + 4)
                    {
                        _yOffsetTarget = (int)((double)scrollable * 1.8 + 4);
                        LogStr($"_yOffsetTarget clamped to max value: {_yOffsetTarget} when cachedVisibleCats.Length = {cachedVisibleCats.Length}");
                    }
                    if (_yOffsetTarget != yOffsetTarget)
                    {
                        yOffsetTarget = _yOffsetTarget;
                        yScrollAni = new FloatAnimator(yOffset, _yOffsetTarget, 0.1f);
                    }
                } else
                {
                    yScrollAni = null;
                }
                return 0;
            }
        }
        // return 0;
        return _mouseEventHandler(a1, a2);
    }

    unsafe static bool _insideHouseCatByOffset = false;
    unsafe static int totalCatsCount = -1;
    unsafe static bool _refreshPending = false;

    [UnmanagedCallersOnly]
    static unsafe nint GetHouseCatByOffsetHook(nint a1, nint a2)
    {
        CountExecution(nameof(GetHouseCatByOffsetHook));

        _insideHouseCatByOffset = true;
        nint result;
        if (isIteratingOurDrawers)
        {
            // LogStr($"[HOOK] GetHouseCatByOffsetHook: returning cached cat at index {_catIndex}");
            if ((uint)_catIndex >= (uint)cachedVisibleCats.Length)
            {
                _insideHouseCatByOffset = false;
                return 0;
            }

            result = cachedVisibleCats[_catIndex];
            // LogStr($"[HOOK] GetHouseCatByOffsetHook: cachedVisibleCats result={result:X}");
        } else
        {
            // LogStr($"Getting cat at {a1:X} {a2:X}");
            result = _getHouseCatByOffset(a1, a2);
        }
        _insideHouseCatByOffset = false;
        // LogStr($"[HOOK] GetHouseCatByOffsetHook: result={result:X}");
        return result;
    }


    unsafe static int _catIndex = 0;
    unsafe static bool isIteratingOurDrawers = false;
    unsafe static List<nint> visibleRenderers = new();
    [UnmanagedCallersOnly]
    static unsafe nint InitCatStatsCallbackHook(nint a1)
    {   
        CountExecution(nameof(InitCatStatsCallbackHook));

        LogStr($"[HOOK] InitCatStatsCallbackHook called: a1=0x{a1:X}");
        Write(a1 + 0x8, originalDrawer);
        var result = _initCatStatsClickCallback(a1);

        // nint result = 0;
        isIteratingOurDrawers = true;
        _catIndex = 0;
        visibleRenderers = new();

        try
        {
            foreach (var drawer in rowDrawers)
            {
                Write(a1 + 0x8, drawer);
                result = _initCatStatsClickCallback(a1);
                visibleRenderers.Add(Marshal.ReadIntPtr(drawer + 0x40));
                _catIndex++;

                if (_catIndex == cachedVisibleCats.Length)
                    break;
            }

            SortCats();
        }
        finally
        {
            isIteratingOurDrawers = false;
        }
        Write(a1 + 0x8, originalDrawer);

        return result;
    }


    [UnmanagedCallersOnly]
    static unsafe nint CreatePanelHook(nint a1, nint a2, nint a3)
    {
        CountExecution(nameof(CreatePanelHook));

        if (originalHousePanel != 0 && insideOurCatInstantiation)
        {
            LogStr($"CreatePanelHook returning originalPanel a1={a1:X} a2={a2:X} a3={a3:X}");
            return originalHousePanel;
        }
        var result = _createCatsDrawerHousePanel(a1, a2, a3);
        LogStr($"CreatePanelHook called a1={a1:X} a2={a2:X} a3={a3:X} result={result:X}");
        originalHousePanel = result;
        // This write is to set blur effect on click:
        Write<nint>(originalHousePanel + 0xC8, 0x0000000001000101);
        return result;
    }


    static nint originalHousePanel = 0;

    unsafe static delegate* unmanaged<nint, void>  _buttonCallbackResolverPtr = null;
    static bool isInsideCreateCatStatsDrawerHook = false;
    static nint originalDrawer = 0;
    static nint framesSinceInitialCatStatsDrawer = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CreateCatStatsDrawerHook(nint a1)
    {
        CountExecution(nameof(CreateCatStatsDrawerHook));
        LogStr($"[HOOK] CreateCatStatsDrawerHook called: a1=0x{a1:X}");
        isInsideCreateCatStatsDrawerHook = true;
        nint result = _statsCreator(a1);
        isInsideCreateCatStatsDrawerHook = false;
        
        if (originalDrawer == 0)
        {
            framesSinceInitialCatStatsDrawer = 0;
            originalDrawer = a1;
        } 

        LogStr($"[HOOK] CreateCatStatsDrawerHookkkk: a1=0x{a1:X}, result=0x{result:X}");
        return result;
    }

    unsafe static string ReadUtf16CustomString(nint address)
    {
        CountExecution(nameof(ReadUtf16CustomString));
        byte* obj = (byte*)address;

        ulong length = *(ulong*)(obj + 0x10);

        if (length == 0)
            return string.Empty;

        if (length <= 8)
        {
            // Inline UTF-16 data begins at +0x00.
            return new string(
                (char*)(obj + 0x00),
                0,
                checked((int)length)
            );
        }

        // Long string: +0x00 contains the pointer.
        char* str = *(char**)obj;

        if (str == null)
            return string.Empty;

        return new string(
            str,
            0,
            checked((int)length)
        );
    }
    static nint openCloseBtn = 0;
    [UnmanagedCallersOnly]
    static unsafe nint RegisterCallbackHook(nint menuPanel, nint a2, nint a3, nint a4)
    {
        var result = _registerButton(menuPanel, a2, a3, a4);
        var entityAddr = menuPanel + 0x18;
        var rendererAddr = menuPanel + 0x38;
        if (!isInsideCreateCatStatsDrawerHook || !IsMemReadable(rendererAddr, 8) || !IsMemReadable(entityAddr, 8))
        {
            // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}");
            return result;
        }
        CountExecution(nameof(RegisterCallbackHook));
        var btnName = TryReadCString(a2);
        var renderer = Marshal.ReadIntPtr(rendererAddr);
        var rendererName = TryReadStdString(renderer + 0xA8);
        // LogStr($"[HOOK] RegisterCallbackHook inside CatStats: a1=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}, btnName=\"{btnName}\", renderer=0x{renderer:X}, rendererName={rendererName}");
        if (rendererName != "CatMenu")
        {
            return result;
        }
        var entity = Marshal.ReadIntPtr(entityAddr);
        var componentsList = Marshal.ReadIntPtr(entity + 0x28);
        var houseDrawerPanel = Marshal.ReadIntPtr(componentsList + 0x0);
        LogStr($"[HOOK] RegisterCallbackHook inside CatMenu: entity=0x{entity:X}, componentsList=0x{componentsList:X}, houseDrawerPanel=0x{houseDrawerPanel:X}");
        var movieclip = result + 0x48;
        if (!IsMemReadable(movieclip, 8)) {
            LogStr($"[HOOK] RegisterCallbackHook: movieclip is not readable at 0x{movieclip:X}, returning result=0x{result:X}");
            return result;
        }

        var mcPtr = Marshal.ReadIntPtr(movieclip);
        var namePtr = Marshal.ReadIntPtr(mcPtr + 0x48);
        var name = TryReadCString(namePtr);
        if (name != "openclose")
        {
            LogStr($"[HOOK] RegisterCallbackHook: openclose button not found, name={name} at 0x{namePtr:X}, mcPtr=0x{mcPtr:X}, movieclip=0x{movieclip:X}");
            return result;
        }

        LogStr($"[HOOK] RegisterCallbackHook: CatMenu found at 0x{renderer:X}, openclose button found at 0x{mcPtr:X}");
        if (originalHousePanel == 0)
        {
            // noop = Read<byte>((nint)MewjectorApi.GameBase + 0x60);
            originalHousePanel = houseDrawerPanel;
        } 
        // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        return result;
    }

    static string[] buttonList = ["spd_btn", "cha_btn", "int_btn", "str_btn", "lck_btn", "con_btn", "dex_btn", "avg_btn", "bdc_btn", "muc_btn"];
    static string[] stats = ["spd", "cha", "int", "str", "lck", "con", "dex", "bdc", "muc"];
    static bool allStatsAlreadyFound = false;
    
    static Dictionary<nint, Dictionary<string, int>> catStats = new();
    // static sortedCats:
    static nint[] sortedCats = Array.Empty<nint>();
    
    static unsafe string getStat(nint dynamicTextBox)
    {
        if (!IsMemReadable(dynamicTextBox + 0x48, 8))
        {
            return "";
        }
        CountExecution(nameof(getStat));
        var name = TryReadCString(Marshal.ReadIntPtr(dynamicTextBox + 0x48));
        if (name == "birthdefectcount")
        {
            LogStr($"[HOOK] getStat: birthdefectcount found for dynamicTextBox=0x{dynamicTextBox:X}");
            return "bdc";
        }
        if (name == "mutationcount")
        {
            LogStr($"[HOOK] getStat: mutationcount found for dynamicTextBox=0x{dynamicTextBox:X}");
            return "muc";
        }
        if (name == "total")
        {
            var parentMovieclip = Marshal.ReadIntPtr(dynamicTextBox + 0x38);
            var parentName = TryReadCString(Marshal.ReadIntPtr(parentMovieclip + 0x48));
            // get first 3 letters
            return parentName.Length >= 3 ? parentName[..3] : "";
        }
        return "";
    }

    static unsafe nint GetRenderer(nint movieClip)
    {
        var parentMovieclip = Marshal.ReadIntPtr(movieClip + 0x38);
        if (parentMovieclip != 0 && IsMemReadable(parentMovieclip, 8))
        {
            return GetRenderer(parentMovieclip);
        }
        return Marshal.ReadIntPtr(movieClip + 0x40); // renderer
    }
   
    static bool averagesDirty = false;
    static bool positionDirty = false;
    [UnmanagedCallersOnly]
    static unsafe nint GameTickHook(nint a1)
    {
        // call mewgenics.7FF647368A30
        // [[[rax+0x38]+18]+58] 
        var result = _gameTick(a1);

        // 00007FF646AD4C40
        // [[[rcx+0x18]+0x28]] or [[[rcx+0x18]+0x28]+10]
        if (averagesDirty)
        {
            // LogStr($"[HOOK] GameTickHook: updating averages for {averages.Count} renderers");
            foreach (var renderer in averages.Keys)
            {
                var average = averages[renderer];
                var movieclip = Marshal.ReadIntPtr(renderer + 0x80);
                var avgTextbox = _getChild(movieclip, GameString.Create("average"));
                if (avgTextbox == 0)
                {
                    LogStr($"[HOOK] GameTickHook: WARNING! avgTextbox is 0 for renderer 0x{renderer:X}");
                    return 0;
                }
                _setText(avgTextbox, GameString.CreateUTF16GameString($"{average}"));
                // LogStr($"[HOOK] GameTickHook: average={average} for renderer 0x{renderer:X}");
            }
            averagesDirty = false;
            
            // LogStr($"[HOOK] GameTickHook: average={average} total={total} count={count}");
        }


        if (originalDrawer != 0 && totalCatsCount == -1) {
            CountExecution(nameof(GameTickHook));
            
            var pointer = Marshal.ReadIntPtr(originalDrawer + 0x20);
            // LogStr($"pointer 0: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer + 0x78);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 1: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 2: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer + 0x18);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 3: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer + 0x8);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 4: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer + 0x20);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 5: {pointer:X}");
            pointer = Marshal.ReadIntPtr(pointer + 0x4480);
            if (pointer == 0)
            {
                return 0;
            }
            // LogStr($"pointer 6: {pointer:X}");
            var count = Marshal.ReadInt32(pointer + 0xC);
            if (totalCatsCount == count)
            {
                // already updated, do nothing
                // LogStr($"no change cat count (tick): {count} reading from 0x{pointer + 0xC:X}");
                return 0;
            }
            if (count == 0 || totalCatsCount == 0)
            {
                return 0;
            }
            // totalCatsCount = 0; // just for debugging!
            totalCatsCount = count;

            // LogStr($"cat count (tick): {count}");
            CreateRows();
            // LogStr($"before read! {originalDrawer:X}");
        }

        if (cachedVisibleCats.Length > 0)
        {
            handlePositionOfOurPanel();
        }
        if (originalDrawer != 0 && framesSinceInitialCatStatsDrawer < 10000)
        {
            framesSinceInitialCatStatsDrawer++;
        }

        if (ourPanelIsOpen)
        {
            var index = GetRendererIndexWhereMouseIsHitting();
            if (index != -1)
            {
                var catStatDrawer = rowDrawers[index];
                currentlyHoveredDrawer = catStatDrawer;
            } else
            {
                currentlyHoveredDrawer = 0;
            }
        }

        return result;
    }
    
    static Dictionary<nint, int> sortedPositions = new Dictionary<nint, int>();

    static unsafe void handlePositionOfOurPanel()
    {
        var newYOffset = yScrollAni == null ? yOffset : yScrollAni.Tick();
        var newXOffset = xMoveAni == null ? xOffset : xMoveAni.Tick();
        if (newYOffset == yOffset && newXOffset == xOffset && !positionDirty)
        {
            return;
        }
        positionDirty = false;
        CountExecution(nameof(handlePositionOfOurPanel));
        yOffset = newYOffset;
        xOffset = newXOffset;
        LogStr($"xOffset {xOffset} yOffset {yOffset}");
        double yPos = -1.0 + yOffset;
        double xPos = xOffset;
        double headerXpos = cachedVisibleCats.Length > 0 ? xPos - 10.0 : -300.0;
        var _findButton = (delegate* unmanaged<nint, nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x97c4f0);
        if (headersRenderer != 0)
        {
            var headerTransform = Marshal.ReadIntPtr(headersRenderer + 0x40);
            Write(headerTransform + 0x80, headerXpos);
            if (!panelAnimationInProgress)
            {
                _trackMovieclipChildParentOffset(originalHousePanel, headerTransform, 1);
            }
        }
        if (footerRenderer != 0)
        {
            var footerTransform = Marshal.ReadIntPtr(footerRenderer + 0x40);
            Write(footerTransform + 0x80, headerXpos);
            Write(footerTransform + 0x88,  -1.0 + yOffset - (1.8 * visibleRenderers.Count));
            if (!panelAnimationInProgress)
            {
                _trackMovieclipChildParentOffset(originalHousePanel, footerTransform, 1);
            }
        }
        sortedPositions = new Dictionary<nint, int>(sortedCats.Length);
        for (var i = 0; i < sortedCats.Length; i++)
            sortedPositions[sortedCats[i]] = i;
       
    }

    static unsafe void updateRowTransforms()
    {
        double yPos = -1.0 + yOffset;
        double xPos = xOffset;
        for (var i = 0; i < totalCatsCount && i < rowRenderers.Length && i < rowTransforms.Length; i++)
        {
            var index = sortedPositions.TryGetValue(rowRenderers[i], out var sortedIndex)
                ? sortedIndex
                : -1;
            var transform = rowTransforms[i];
            // if (Array.IndexOf(sortedCats, cat) != -1)
            // {
            //     transform = rowTransforms[Array.IndexOf(sortedCats, cat)];
            // }
            if (index != -1)
            {    
                Write(transform + 0x80, xPos);
                yPos = -1.0 + yOffset - (1.8 * index);
                Write(transform + 0x88, yPos);
                // if ( Array.IndexOf(sortedCats, rowRenderers[i]) == -1)
                // {
                //     LogStr($"[HOOK] handlePositionOfOurPanel: rowRenderer 0x{rowRenderers[i]:X} not found in sortedCats, yPos={yPos}");
                // } else
                // {
                //     LogStr($"[HOOK] handlePositionOfOurPanel: rowRenderer 0x{rowRenderers[i]:X} found in sortedCats at index {Array.IndexOf(sortedCats, rowRenderers[i])}, yPos={yPos}");
                // }
            } else
            {
                Write(transform + 0x80, (double)-300.0);
            }
        }

        for (var i = 0; i < totalCatsCount && i < rowDrawers.Length; i++)
        {
            var drawer = rowDrawers[i];
            var renderer = Marshal.ReadIntPtr(drawer + 0x40);
            var transform = Marshal.ReadIntPtr(renderer + 0x40);
            if (!panelAnimationInProgress)
            {
                _trackMovieclipChildParentOffset(originalHousePanel, transform, 1);
            }
        }
        
    }


    static nint[] rowDrawers = Array.Empty<nint>();
    static nint[] rowTransforms = Array.Empty<nint>();
    static nint[] rowRenderers = Array.Empty<nint>();

    static nint headersRenderer = 0;
    static nint footerRenderer = 0;
    
    static nint attachRendererIteration = 0;
    static bool insideOurCatInstantiation = false;
    static bool showingChimplantsPromo = false;

    static nint noop = 0;

    static unsafe void CreateRows()
    {
        LogStr($"Creating rows");
        CountExecution(nameof(CreateRows));
        var _getRenderer = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x224cd0);
        var _createEntity = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x96b3e0);
        DateTime currentDateTime = DateTime.Now; 

        IntPtr headers = Marshal.StringToHGlobalAnsi("RowHeaders");
        // var _strBtn = _findMovieClipTrampoline(CreateUTF16GameString("x"));
        LogStr($"header string: RowHeaders");
        var headersEntity = _createEntity(scenePtr);
        LogStr($"headersEntity 0x{headersEntity:X}");

        try
        {
            LogStr($"Creating headersRendere with arguments: scenePtr=0x{scenePtr:X}, headersEntity=0x{headersEntity:X}, headers=0x{headers:X}");            
            headersRenderer = _createUiRenderer(scenePtr, headersEntity, headers);
        }
        finally
        {
            Marshal.FreeHGlobal(headers);
        }
        LogStr($"headersRenderer created: 0x{headersRenderer:X}");

        if (currentDateTime > dateInstalled.AddDays(3))
        {
            LogStr($"Creating footerRenderer because more than 3 days have passed since installation");
            showingChimplantsPromo = currentDateTime > dateInstalled.AddDays(60) || currentDateTime > new DateTime(2027, 7, 1);
            IntPtr footer = Marshal.StringToHGlobalAnsi(showingChimplantsPromo ? "Chimplants" : "BuyMeACoffee");
            var footerEntity = _createEntity(scenePtr);
            try
            {
                footerRenderer = _createUiRenderer(
                    scenePtr,
                    footerEntity,
                    footer);
            }
            finally
            {
                Marshal.FreeHGlobal(footer);
            }

            Write(footerRenderer + 0x50, 0x0000002400000101);
            var footerTransform = Marshal.ReadIntPtr(footerRenderer + 0x40);
            Write(footerTransform + 0x80, -300.0);
        }
        
        Write(headersRenderer + 0x50, 0x0000002400000101);
        var headerTransform = Marshal.ReadIntPtr(headersRenderer + 0x40);
        Write(headerTransform + 0x80, -300.0);
        LogStr($"headersRenderer 0x{headersRenderer:X}");
        // totalCatsCount = 1;// for debugging only;
        rowRenderers = new nint[totalCatsCount];
        rowTransforms = new nint[totalCatsCount];
        rowDrawers = new nint[totalCatsCount];
        for (int i = 0; i < totalCatsCount; i++)
        {
            LogStr($"Creating row {i + 1}/{totalCatsCount}");
            nint rowEntity = _createEntity(scenePtr);

            int componentCount = *(int*)(rowEntity + 36);
            nint componentArray = *(nint*)(rowEntity + 40);

            
            var rendererFound = _getRenderer(rowEntity);
            IntPtr name = Marshal.StringToHGlobalAnsi("RowCatStatus");

            nint rowRenderer = _createUiRenderer(
                scenePtr,
                rowEntity,
                name);

            Marshal.FreeHGlobal(name);
            LogStr($"Creating CatStatsDrawer {i + 1}/{totalCatsCount}: Renderer {rowRenderer:X} {rowEntity:X} {scenePtr:X}");
            rendererFound = _getRenderer(rowEntity);
            insideOurCatInstantiation = true;
            attachRendererIteration = 0;
            var rowDrawer = _createCatStatsDrawer(
                scenePtr,
                rowEntity);
            insideOurCatInstantiation = false;
            
            // Write(rowDrawer + 0x38, originalHousePanel);

            // var iconsPanel = Marshal.ReadIntPtr(rowDrawer + 0x68);
            // var panelB = Marshal.ReadIntPtr(rowDrawer + 0x68);
            var transform = Marshal.ReadIntPtr(rowRenderer + 0x40);
            LogStr($"Created new CallStatsDrawer: Renderer {rowRenderer:X} Drawer:{rowDrawer:X} Entity: {rowEntity:X} {scenePtr:X} transform: {transform:X}");
            // rowIconPanels.Add(iconsPanel);
            rowDrawers[i] = rowDrawer;
            rowRenderers[i] = rowRenderer;
            rowTransforms[i] = transform;

            Dictionary<string, int> dict = new();
            catStats[rowRenderer] = dict;

            Write(rowRenderer + 0x51, (byte)0);
            LogStr($"Initialized catStats dictionary for renderer {rowRenderer:X}");

            // var menuPanel = Marshal.ReadIntPtr(rowDrawer + 0x60);
            // // debug only ahead:
            // nint callback = Marshal.AllocHGlobal(0x40);
            // NativeMemory.Clear((void*)callback, 0x40);
            
            // nint callbackStorage = Marshal.AllocHGlobal(0x20);
            // NativeMemory.Clear((void*)callbackStorage, 0x20);

            // Marshal.WriteIntPtr(callbackStorage + 0x00, 0);
            // Marshal.WriteIntPtr(callbackStorage + 0x08, 0);
            // Marshal.WriteIntPtr(callbackStorage + 0x10, 0);
            // Marshal.WriteIntPtr(callbackStorage + 0x18, 15);

            // testbutton = _registerButton(
            //     menuPanel,
            //     GameString.Create("xxx"),
            //     callbackStorage,
            //     callback
            // );
            // Write(testbutton + 0x50, 0x000003EA);

        }

        Write(originalHousePanel + 0x118, originalDrawer);
        
    }
    // static nint testbutton = 0;


    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _changeCloneText;

    static nint scenePtr = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CreateUiRendererHook(nint a1, nint entity, nint namePtr)
    {
        CountExecution(nameof(CreateUiRendererHook));
        var name = TryReadCString(namePtr);
        if (name == "HouseCatStatus")
        {
            LogStr($"[HOOK] CreateUiRendererHook: a1=0x{a1:X}, entity=0x{entity:X}, name=\"{name}\"");
            scenePtr = a1;
        }

        return _createUiRenderer(a1, entity, namePtr);
    }

    static bool ourPanelIsOpen = false;
    static bool panelAnimationInProgress = false;
    private const int PanelStateClosed = 36;
    private const int PanelStateOpen = 37;
    static int waitingForPanelStatus = 0;
    static nint catMenuPanel = 0;

    [UnmanagedCallersOnly]
    static unsafe nint UpdatePanelLayoutHook(nint a1)
    {
        if (originalDrawer == 0 ||(catMenuPanel != 0 && catMenuPanel != a1))
        {
            return _updatePanelLayout(a1);
        }
        CountExecution(nameof(UpdatePanelLayoutHook));

       
        var renderer = Marshal.ReadIntPtr(a1 + 0x58);
        if (catMenuPanel == 0)
        {
            // read as dword:
            var rendererName = TryReadStdString(renderer + 0xA8, false);
            if (rendererName != "CatMenu")
            {
                return _updatePanelLayout(a1);
            }
            LogStr($"[HOOK] UpdatePanelLayoutHook: CatMenu panel detected: a1=0x{a1:X}, renderer=0x{renderer:X}, rendererName=\"{rendererName}\"");
            catMenuPanel = a1;
        }

        var result = _updatePanelLayout(a1);
        updateRowTransforms();


        var rendererState = Marshal.ReadInt32(renderer + 0x54);
        if (rendererState == waitingForPanelStatus)
        {
            if (waitingForPanelStatus == PanelStateOpen)
            {
                waitingForPanelStatus = 0;
                panelAnimationInProgress = false;
                positionDirty = true;
                LogStr($"[HOOK] 1- Panel is open and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
            else if (waitingForPanelStatus == PanelStateClosed)
            {
                waitingForPanelStatus = 0;
                panelAnimationInProgress = false;
                for (int i = 0; i < rowDrawers.Length; i++)
                {
                    var drawer = rowDrawers[i];
                    Write(drawer + 0x78, (nint)0); // setting HouseCat reference to zero
                }
                positionDirty = true;
                LogStr($"[HOOK] 2- Pane is closed and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
        }
        return result;
    }

    [DllImport("kernel32.dll")]
    static extern nint GetCurrentProcess();

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate nint CreateInstanceDelegate(nint defineSprite);

    // RVA 0x040401E0
    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    private delegate nint AttachChildDelegate(
        nint parentMovieClip,
        nint childMovieClip,
        uint depth);

    unsafe public static nint DuplicateMovieClip(nint rowMovieClip, nint moduleBase)
    {
        // MovieClip fields
        nint parent  = Marshal.ReadIntPtr(rowMovieClip + 0x38);
        nint runtime = Marshal.ReadIntPtr(rowMovieClip + 0xD0);
        LogStr($"[HOOK] DuplicateMovieClip: rowMovieClip=0x{rowMovieClip:X}, parent=0x{parent:X}, runtime=0x{runtime:X}");

        // runtime == DefineSprite+0x60
        nint defineSprite = runtime - 0x60;

        // current depth
        uint depth = *(uint*)(rowMovieClip + 0x0C);

        // call DefineSprite::CreateInstance()
        LogStr($"[HOOK] DuplicateMovieClip: calling CreateInstance for DefineSprite 0x{defineSprite:X}...");
        var create =
            Marshal.GetDelegateForFunctionPointer<CreateInstanceDelegate>(
                moduleBase + 0x10FE460);

        nint clone = create(defineSprite);

        if (clone == 0)
            return 0;

        // attach beside the original
        var attach =
            Marshal.GetDelegateForFunctionPointer<AttachChildDelegate>(
                moduleBase + 0x040401E0);
        LogStr($"[HOOK] DuplicateMovieClip: attaching clone 0x{clone:X} to parent 0x{parent:X} at depth {depth + 1}...");
        attach(parent, clone, depth + 1);

        return clone;
    }


    static void ClearState()
    {
        // CountExecution(nameof(RemoveMovieClip));
        LogStr($"ClearState called");
        // LogStr($"[HOOK] RemoveMovieClip called on mod container 0x{a1:X}");
        sortedPositions = new Dictionary<nint, int>();
        catMenuPanel = 0;
        originalHousePanel = 0;
        originalDrawer = 0;
        rowTransforms = Array.Empty<nint>();
        rowDrawers = Array.Empty<nint>();
        rowRenderers = Array.Empty<nint>();
        totalCatsCount = -1;
        headersRenderer = 0;
        footerRenderer = 0;
        ourPanelIsOpen = false;
        cachedVisibleCats = Array.Empty<nint>();
        sortedCats = Array.Empty<nint>();
        visibleRenderers.Clear();
        initializedButtons = false;
        yOffset = 0;
        yOffsetTarget = 0;
        averages.Clear();
        xOffset = -300;
        xOffsetTarget = -300;
        positionDirty = false;
        averagesDirty = false;
        cachedPointers.Clear();
        catStats.Clear();
        yScrollAni = null;
        xMoveAni = null;
    }


    static bool _active;   // static — accessible from [UnmanagedCallersOnly]


    protected void OnEnable()
    {
        LogStr("CatsTable enabled");
    }

    protected void OnDisable()
    {
        LogStr("CatsTable disabled");
    }

    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenFns = new();

    static public void LogStr(string message)
    {
        if (!_debugLogging)
            return;
        MewjectorApi.Log(message);
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
    static unsafe bool TryGetStdStringLayout(nint strObjPtr, out ulong size, out ulong capacity, out nint dataPtr, bool debug = false)
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

    static unsafe string? TryReadStdString(nint strObjPtr, bool debug = false)
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
    static unsafe string? TryReadCString(nint ptr, int maxLen = 128)
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

    static int _diagCallCount = 0;
    // Key: dedup token — each unique string+path combination is logged at most once
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenStrings = new();
    static readonly System.Collections.Concurrent.ConcurrentDictionary<nint, bool> _seenGonObjects = new();

    // Pointer range of the game's PE image (code, rdata, vtables — not heap objects).
    static readonly nint IMAGE_RANGE_START = unchecked((nint)0x7FF70C3C0000L);
    static readonly nint IMAGE_RANGE_END   = unchecked((nint)0x7FF70D900000L);


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
    static extern unsafe nint VirtualQuery(
        nint lpAddress, MEMORY_BASIC_INFORMATION* lpBuffer, nint dwLength);

    static unsafe bool IsMemReadable(nint ptr, int size)
    {
        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery(ptr, &mbi, (nint)sizeof(MEMORY_BASIC_INFORMATION)) == 0) return false;
        if (mbi.State != 0x1000 /* MEM_COMMIT */) return false;
        const uint PAGE_NOACCESS = 0x01, PAGE_GUARD = 0x100;
        if ((mbi.Protect & (PAGE_NOACCESS | PAGE_GUARD)) != 0) return false;
        return (ulong)ptr + (ulong)size <= (ulong)mbi.BaseAddress + (ulong)mbi.RegionSize;
    }

    // Filters out small integers and kernel-space values; passes user-mode pointers.
    static bool IsLikelyPointer(nint val)
    {
        ulong v = (ulong)(nuint)val;
        return v >= 0x10000 && v <= 0x0000_7FFF_FFFF_FFFF;
    }


};
