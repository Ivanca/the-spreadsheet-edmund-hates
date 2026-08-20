// using MewgenicsModSdk;
// using MewgenicsModSdk.Game;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace CatstableMod;

public partial class CatstableMod
{

    public string Id => "catstable";
    public string Name => "catstable";
    public bool IsEnabled { get; private set; } = true;

    CancellationTokenSource _cts = new CancellationTokenSource();
    static CatstableMod? _instance;
    Dictionary<string, string> _abilitiesLocalNames = new Dictionary<string, string>();


    static volatile bool _genHooksInstalled = false;
    internal static volatile bool _genLoggingEnabled = false;
    static nint _abilityTriggerHookTrampoline; // sub_7FF70C3F1ED0  glaiel::Ability::trigger

    static unsafe delegate* unmanaged<nint, nint> _updatePanelLayout;
    static unsafe delegate* unmanaged<nint, nint> _createRenderer;

    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _createUiRenderer;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> _createPanel;
    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _registerCallback;
    static unsafe delegate* unmanaged<nint, nint> _statsCreator;
    static unsafe delegate* unmanaged<nint, nint, nint> _getHouseCatByOffset;
    static unsafe delegate* unmanaged<nint, nint> _gameTick;
    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _catIterator;

    static unsafe delegate* unmanaged<nint, nint, nint, nint> _findButton;
    static unsafe delegate* unmanaged<nint, nint> _initCatStatsClickCallback;
    static unsafe delegate* unmanaged<nint, nint> _toggleHouseDrawer;
    static unsafe delegate* unmanaged<nint, nint, nint> _mutationToolTip;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> _hideIconsHandler;
    static unsafe delegate* unmanaged<nint, byte> _isPanelActive;
    static unsafe delegate* unmanaged<nint, nint> _renderPanel;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> _bindCat;
    unsafe static delegate* unmanaged<nint, nint, nint> _mouseEventHandler;
    unsafe static delegate* unmanaged<nint, nint, nint, nint> _isButtonActive;
    unsafe static delegate* unmanaged<nint, nint, nint, nint> _hideChildren;

    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _removeMovieClipTrampoline;
    unsafe static delegate* unmanaged<nint, char*, nuint, nint> _assignString;

    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _globalResourceManagerLookup;
    
    unsafe static delegate* unmanaged<nint, nint, void> _setText;
    unsafe static delegate* unmanaged<nint, nint, uint, void> _attachChild;
    unsafe static delegate* unmanaged<nint, nint, nint> _createCatStatsDrawer;
    unsafe static delegate* unmanaged<nint, nint> _catStatsDrawerUpdate;
    
    
    const long RVA_CreateInstance = 0xA4E460; // DefineSprite::CreateInstance()
    const long RVA_CopyState      = 0x9b2d20; // sub_404062D20
    const long RVA_AttachChild    = 0x9901e0; // sub_40401E0

    unsafe static T Read<T>(nint p) where T : unmanaged
        => *(T*)p;

    unsafe static void Write<T>(nint address, T value) where T : unmanaged
    {
        *(T*)address = value;
    }
    
    static nint _rightStr = 0;
    static nint _leftStr = 0;
    internal unsafe void MjInit()
    {

        _removeMovieClipTrampoline = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x99e030, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&RemoveMovieClip);

        _updatePanelLayout = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x2038b0, (void*)(delegate* unmanaged<nint, nint>)&UpdatePanelLayoutHook);

        _createRenderer = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x5A580, (void*)(delegate* unmanaged<nint, nint>)&CreateRendererHook);
        
        // _globalResourceManagerLookup = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
        //     0x9adc50, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&GlobalResourceManagerLookupHook);

        _createUiRenderer = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x5a380, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&CreateUiRendererHook);

        _createPanel = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xeecb0, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&CreatePanelHook);

        _registerCallback = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x973c40, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&RegisterCallbackHook);

        _statsCreator = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xE9200, (void*)(delegate* unmanaged<nint, nint>)&CreateCatStatsDrawerHook);

        _getHouseCatByOffset = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xEA3B0, (void*)(delegate* unmanaged<nint, nint, nint>)&GetHouseCatByOffsetHook);

        _findButton = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x978a30, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&FindButtonHook);

        _initCatStatsClickCallback = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xEEB10, (void*)(delegate* unmanaged<nint, nint>)&InitCatStatsCallbackHook);

        _toggleHouseDrawer = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x203210, (void*)(delegate* unmanaged<nint, nint>)&ToggleHouseDrawerHook);
        
        _gameTick = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x962820, (void*)(delegate* unmanaged<nint, nint>)&GameTickHook);

        _catIterator = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xec960, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&CatIteratorHook);

        _mutationToolTip = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xe4c40, (void*)(delegate* unmanaged<nint, nint, nint>)&MutationTooltipHook); 


        var location = MewjectorApi.GameBase;
        LogStr($"Gamebase at {location:X}...");
        // LogStr("MjInit: installing hooks...");
        _setText = (delegate* unmanaged<nint, nint, void>)(MewjectorApi.GameBase + (nuint)0x986470);
        _assignString = (delegate* unmanaged<nint,char*,nuint,nint>)(MewjectorApi.GameBase + 0x5b100);
        

        _attachChild = (delegate* unmanaged<nint, nint, uint, void>)(MewjectorApi.GameBase + (nuint)RVA_AttachChild);
        _createCatStatsDrawer =  (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x1ac430);

        _mouseEventHandler = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xc2c390, (void*)(delegate* unmanaged<nint, nint, nint>)&MouseWheelHook);
        
        _catStatsDrawerUpdate = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xea8b0, (void*)(delegate* unmanaged<nint, nint>)&CatStatsDrawerUpdateHook);


    }


    static nint currentlyDrawerWithOpenIconsPanel = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CatStatsDrawerUpdateHook(nint a1)
    {

        if (currentlyDrawerWithOpenIconsPanel != 0 && currentlyDrawerWithOpenIconsPanel != a1)
        {
            return 0;
        }
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


    static nint[] cachedVisibleCats = new nint[0];


    [UnmanagedCallersOnly]
    static unsafe nint CatIteratorHook (nint a1, nint a2, nint a3, nint a4)
    {
        cachedVisibleCats = new nint[a3];
        LogStr($"[HOOK] CatIteratorHook: a1={a1:X} a2={a2:X} a3={a3} a4={a4:X}");

        for (int i = 0; i < a3; i++) {
            cachedVisibleCats[i] = Marshal.ReadIntPtr(a1 + i * 8);
            LogStr($"[HOOK] CatIteratorHook: cachedVisibleCats[{i}] = {cachedVisibleCats[i]:X}");
        }
            
        var result = _catIterator(a1, a2, a3, a4);
        return result;
    }


    static List<nint> _renderersSoFar = new List<nint>();
    [UnmanagedCallersOnly]
    static unsafe nint CreateRendererHook(nint a1)
    {
        var result = _createRenderer(a1);
        // _renderersSoFar.Add(result);
        return result;
    }

    static Dictionary<nint, byte> _visibilityBefore = new Dictionary<nint, byte>();
    [UnmanagedCallersOnly]
    static unsafe nint ToggleHouseDrawerHook(nint a1)
    {
        nint panel = Marshal.ReadIntPtr(a1 + 0x58);
        // nint state = Marshal.ReadIntPtr(a1 + 0x50);
        var changed = false;

        if (panel != _originalPanel && _panelIsOpen)
        {
            MewjectorApi.Log($"[HOOK] Panel Close");
            _panelIsOpen = false;
            changed = true;
            panelAnimationInProgress = true;
            waitingForPanelStatus = 36;

        }
        else if (panel == _originalPanel && !_panelIsOpen)
        {
            // OPEN
            MewjectorApi.Log($"[HOOK] Panel Open");
            _panelIsOpen = true;
            changed = true;
            panelAnimationInProgress = true;
            waitingForPanelStatus = 37;
            if (rowRenderers.Count >= cachedVisibleCats.Length)
            {
                for (int i = 0; i < cachedVisibleCats.Length; i++)
                {
                    var rowRenderer = rowRenderers[i];
                    // Write(rowRenderer + 0x51, (byte)1);
                }
            }
        }
        if (changed)
        {
            xOffsetTarget = _panelIsOpen ? 10 : -35;
            xMoveAni = new FloatAnimator(xOffset, xOffsetTarget, 0.5f);
        }
        return _toggleHouseDrawer(a1);
    }

    static nint _lastButtonCSD = 0;
    [UnmanagedCallersOnly]
    static unsafe nint FindButtonHook(nint a1, nint a2, nint a3)
    {
        var result = _findButton(a1, a2, a3);
        if (result != 0)
        {
            // MewjectorApi.Log($"[HOOK] result!=0 {result:X} x={x:X}");
            _lastButtonCSD = Read<nint>(Read<nint>(Read<nint>(Read<nint>(result + 0x38) + 0x18) + 0x28) + 0x10);
        } else
        {
            // MewjectorApi.Log($"[HOOK] EMPTY! _lastButtonCSD = 0");
            _lastButtonCSD = 0;
        }
        return result;
    }

    [UnmanagedCallersOnly]
    static unsafe nint MutationTooltipHook(nint a1, nint a2)
    {
        if (a1 == 0)
        {
            return 0;
        }
        if (_lastButtonCSD != 0)
        {
            var opt1 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28) + 0x10);
            var opt2 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28));
            // MewjectorApi.Log($"[HOOK] opt1={opt1:X} opt1={opt2:X} _lastButtonCSD={_lastButtonCSD:X}");
            if (opt1 == _lastButtonCSD || opt2 == _lastButtonCSD)
            {
                // MewjectorApi.Log($"[HOOK] MATCH!");
                return _mutationToolTip(a1, a2);
            }
            else
            {
                // MewjectorApi.Log($"[HOOK] MutationTooltipHook: Return zero !");
                return 0;
            }
        }
        // MewjectorApi.Log($"[HOOK] _lastButtonCSD == 0");
        return _mutationToolTip(a1, a2);
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
        if(_panelIsOpen && IsLikelyPointer(a2))
        {   
            var a2Val = Marshal.ReadInt32(a2);
            if (a2Val == 1027 && cachedVisibleCats.Length > 10)
            {
                // var y = 
                var intValue = Marshal.ReadInt32(a2 + 0x1C);
                float floatValue = BitConverter.Int32BitsToSingle(intValue);
                // LogStr($"Scroll! {floatValue}");
                var _yOffsetTarget = yOffset - 5 * (int)floatValue;
                if (_yOffsetTarget < 0)
                {
                    _yOffsetTarget = 0;
                }
                if (_yOffsetTarget > cachedVisibleCats.Length * 0.8)
                {
                    _yOffsetTarget = (int)((double)cachedVisibleCats.Length * 0.8);
                }
                if (_yOffsetTarget != yOffsetTarget)
                {
                    yOffsetTarget = _yOffsetTarget;
                    yScrollAni = new FloatAnimator(yOffset, _yOffsetTarget, 0.1f);
                }
                return 0;
            }
        }
        // return 0;
        return _mouseEventHandler(a1, a2);
    }

    unsafe static bool _insideHouseCatByOffset = false;
    unsafe static int _totalCatsCount = -1;
    unsafe static bool _refreshPending = false;

    [UnmanagedCallersOnly]
    static unsafe nint GetHouseCatByOffsetHook(nint a1, nint a2)
    {
        _insideHouseCatByOffset = true;
        nint result;
        if (isIteratingOurDrawers)
        {
            // LogStr($"[HOOK] GetHouseCatByOffsetHook: returning cached cat at index {_catIndex}");
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
    unsafe static List<nint> alreadyInitializedDrawers = new List<nint>();
    unsafe static bool isIteratingOurDrawers = false;
    [UnmanagedCallersOnly]
    static unsafe nint InitCatStatsCallbackHook(nint a1)
    {   
        Write(a1 + 0x8, _originalDrawer);
        var result = _initCatStatsClickCallback(a1);
        // nint result = 0;
        isIteratingOurDrawers = true;
        _catIndex = 0;
        foreach (var drawer in rowDrawers)
        {
            Write(a1 + 0x8, drawer);
            result = _initCatStatsClickCallback(a1);
            _catIndex++;
            if (_catIndex == cachedVisibleCats.Length)
            {
                break;
            }
        }

        isIteratingOurDrawers = false;

        return result;
    }


    [UnmanagedCallersOnly]
    static unsafe nint CreatePanelHook(nint a1, nint a2, nint a3)
    {
        if (_originalPanel != 0)
        {
            LogStr($"CreatePanelHook returning originalPanel a1={a1:X} a2={a2:X} a3={a3:X}");
            return _originalPanel;
        }
        var result = _createPanel(a1, a2, a3);
        LogStr($"CreatePanelHook called a1={a1:X} a2={a2:X} a3={a3:X} result={result:X}");
        _originalPanel = result;
        return result;
    }

     [UnmanagedCallersOnly]
    static unsafe void openCloseButtonCallbackHook(nint a1)
    {
        if (a1 != _ourBtnMetadataCallbackPtr && a1 != _btnCallbackMetadataPtr)
        {
            LogStr($"[HOOK] openCloseButtonCallbackHook: a1=0x{a1:X} is not our callback, calling original");
            _buttonCallbackResolverPtr(a1);
            return;
        }
        
        _buttonCallbackResolverPtr(_ourBtnMetadataCallbackPtr);
        LogStr($"[HOOK] openCloseButtonCallbackHook: a1=0x{_ourBtnMetadataCallbackPtr:X} our finished");
        // _renderPanel(_originalPanel);
        // LogStr($"[HOOK] _renderPanel for original a1=0x{_originalPanel:X} finished");

    }

    static nint _originalPanel = 0;
    static nint _ourPanel = 0;
    static nint _btnCallbackMetadataPtr = 0;
    static nint _ourBtnMetadataCallbackPtr = 0;

    unsafe static delegate* unmanaged<nint, void>  _buttonCallbackResolverPtr = null;
    static bool isInsideCreateCatStatsDrawerHook = false;
    static nint _originalDrawer = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CreateCatStatsDrawerHook(nint a1)
    {
        LogStr($"[HOOK] CreateCatStatsDrawerHook called: a1=0x{a1:X}");
        isInsideCreateCatStatsDrawerHook = true;
        var result = _statsCreator(a1);
        isInsideCreateCatStatsDrawerHook = false;
        if (_originalDrawer == 0)
        {
            _originalDrawer = a1;
        } 

        LogStr($"[HOOK] CreateCatStatsDrawerHookkkk: a1=0x{a1:X}, result=0x{result:X}");
        return result;
    }

    
    static nint openCloseBtn = 0;
    [UnmanagedCallersOnly]
    static unsafe nint RegisterCallbackHook(nint menuPanel, nint a2, nint a3, nint a4)
    {
        var result = _registerCallback(menuPanel, a2, a3, a4);
        var entityAddr = menuPanel + 0x18;
        var rendererAddr = menuPanel + 0x38;
        if (!isInsideCreateCatStatsDrawerHook || !IsMemReadable(rendererAddr, 8) || !IsMemReadable(entityAddr, 8))
        {
            // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}");
            return result;
        }
        var btnName = TryReadCString(a2);
        var renderer = Marshal.ReadIntPtr(rendererAddr);
        var rendererName = TryReadStdString(renderer + 0xA8);
        LogStr($"[HOOK] RegisterCallbackHook inside CatStats: a1=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}, btnName=\"{btnName}\", renderer=0x{renderer:X}, rendererName={rendererName}");
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

        var callbackMetaPtr = result + 0xB8;
        LogStr($"[HOOK] RegisterCallbackHook: CatMenu found at 0x{renderer:X}, openclose button found at 0x{mcPtr:X}, callbackMetaPtr=0x{callbackMetaPtr:X}");
        if (_btnCallbackMetadataPtr == 0)
        {
            openCloseBtn = mcPtr;
            catMenuRenderer = renderer;
            // var openCloseBtnRenderer = Marshal.ReadIntPtr(mcPtr + 0x18);
            // var openCloseBtnTransform = Marshal.ReadIntPtr(openCloseBtnRenderer + 0x38);
            // var openCloseX = Read<float>(openCloseBtnTransform + 0x80);
            // var openCloseY = Read<float>(openCloseBtnTransform + 0x88);
            // LogStr($"[HOOK] RegisterCallbackHook: openclose button transform at 0x{openCloseBtnTransform:X}, x={openCloseX}, y={openCloseY}");
            _btnCallbackMetadataPtr = callbackMetaPtr;
            _originalPanel = houseDrawerPanel;
            // var parent = Read<nint>(mcPtr + 0x38);
            // uint depth = Read<uint>(parent + 0xAC);
            // _attachChild(parent, clone, depth);

        } else if (_ourBtnMetadataCallbackPtr == 0)
        {
            LogStr($"[HOOK] RegisterCallbackHook: _ourBtnMetadataCallbackPtr is null, setting to 0x{callbackMetaPtr:X}");
            _ourBtnMetadataCallbackPtr = callbackMetaPtr;
            _ourPanel = houseDrawerPanel;
            var callbackResolver = Marshal.ReadIntPtr(Marshal.ReadIntPtr(callbackMetaPtr) + 0x10);
            // _buttonCallbackResolverPtr = (delegate* unmanaged<nint, void>)(void*)MewjectorApi.InstallHook(
            //     callbackResolver - (nint)MewjectorApi.GameBase, (void*)(delegate* unmanaged<nint, void>)&openCloseButtonCallbackHook);
            LogStr($"[HOOK] RegisterCallbackHook: callbackResolver=0x{callbackResolver:X}, installed hook at 0x{callbackResolver - (nint)MewjectorApi.GameBase:X}, real one at 0x{(nint)_buttonCallbackResolverPtr:X}");
        } else
        {
            LogStr($"[HOOK] RegisterCallbackHook: Should never reach here");
        }
        // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        return result;
    }
   

    [UnmanagedCallersOnly]
    static unsafe nint GameTickHook(nint a1)
    {
        // call mewgenics.7FF647368A30
        // [[[rax+0x38]+18]+58] 
        _gameTick(a1);

        // 00007FF646AD4C40
        // [[[rcx+0x18]+0x28]] or [[[rcx+0x18]+0x28]+10]

        if (_originalDrawer != 0) {
            handleScrollAtUpdatePanelLayout();
            var pointer = Marshal.ReadIntPtr(_originalDrawer + 0x20);
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
            if (_totalCatsCount == count)
            {
                // already updated, do nothing
                // LogStr($"no change cat count (tick): {count} reading from 0x{pointer + 0xC:X}");
                return 0;
            }
            if (count == 0 || _totalCatsCount == 0)
            {
                return 0;
            }
            // _totalCatsCount = 0; // just for debugging!
            _totalCatsCount = count;

            // LogStr($"cat count (tick): {count}");
            CreateRows();
            // LogStr($"before read! {_originalDrawer:X}");
        }
        return 0;
    }
    
    static unsafe void handleScrollAtUpdatePanelLayout()
    {
        var _updateBtn = (delegate* unmanaged<nint, void>)(MewjectorApi.GameBase + 0x9768e0);
        yOffset = yScrollAni == null ? yOffset : yScrollAni.Tick();
        xOffset = xMoveAni == null ? xOffset : xMoveAni.Tick();
        // LogStr($"xOffset {xOffset}");
        double yPos = -1.0 + yOffset;
        double xPos = xOffset;
        double headerXpos = cachedVisibleCats.Length > 0 ? xPos - 10.0 : -300.0;
        var _findButton = (delegate* unmanaged<nint, nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x9740c0);
        var _trackMovieclipChildParentOffset = (delegate* unmanaged<nint, nint, nint, void>)(MewjectorApi.GameBase + (nuint)0x204010);
        if (headersRenderer != 0)
        {
            var headerTransform = Marshal.ReadIntPtr(headersRenderer + 0x40);
            Write(headerTransform + 0x80, headerXpos);
        }
        for (var i = 0; i < _totalCatsCount; i++)
        {
            var transform = rowTransforms[i];
            if (i < cachedVisibleCats.Length)
            {    
                Write(transform + 0x80, xPos);
                Write(transform + 0x88, yPos);
                yPos -= 1.8;
            } else
            {
                Write(transform + 0x80, (double)-300.0);
            }
        }

        for (var i = 0; i < _totalCatsCount; i++)
        {
            var drawer = rowDrawers[i];
            var renderer = Marshal.ReadIntPtr(drawer + 0x40);
            var transform = Marshal.ReadIntPtr(renderer + 0x40);
            _trackMovieclipChildParentOffset(_originalPanel, transform, 1);

        }

        
    }


    static List<nint> rowDrawers = new List<nint>();
    static List<nint> rowTransforms = new List<nint>();
    static List<nint> rowRenderers = new List<nint>();

    static nint headersRenderer = 0;
    
    static nint catMenuRenderer = 0;
    static nint emptyRenderer = 0;
    static nint attachRendererIteration = 0;
    static bool insideOurCatInstantiation = false;

    static unsafe void CreateRows()
    {
        var _getRenderer = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x6bea0);
        var _createEntity = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x962fb0);

        IntPtr headers = Marshal.StringToHGlobalAnsi("RowHeaders");
        // var _strBtn = _findMovieClipTrampoline(CreateUTF16GameString("x"));

        var headersEntity = _createEntity(scenePtr);

        headersRenderer = _createUiRenderer(
            scenePtr,
            headersEntity,
            headers,
        0);
        Write(headersRenderer + 0x50, 0x0000002600000101);

        LogStr($"headersRenderer 0x{headersRenderer:X}");
        for (int i = 0; i < _totalCatsCount; i++)
        {
            

            nint rowEntity = _createEntity(scenePtr);

            int componentCount = *(int*)(rowEntity + 36);
            nint componentArray = *(nint*)(rowEntity + 40);

            
            var rendererFound = _getRenderer(rowEntity);
            IntPtr name = Marshal.StringToHGlobalAnsi("RowCatStatus");

            nint rowRenderer = _createUiRenderer(
                scenePtr,
                rowEntity,
                name,
                0);

            Marshal.FreeHGlobal(name);
            LogStr($"Creating CatStatsDrawer {i + 1}/{_totalCatsCount}: Renderer {rowRenderer:X} {rowEntity:X} {scenePtr:X}");
            rendererFound = _getRenderer(rowEntity);
            insideOurCatInstantiation = true;
            attachRendererIteration = 0;
            var rowDrawer = _createCatStatsDrawer(
                scenePtr,
                rowEntity);
            insideOurCatInstantiation = false;
            // var iconsPanel = Marshal.ReadIntPtr(rowDrawer + 0x68);
            // var panelB = Marshal.ReadIntPtr(rowDrawer + 0x68);
            var transform = Marshal.ReadIntPtr(rowRenderer + 0x40);
            LogStr($"Created new CallStatsDrawer: Renderer {rowRenderer:X} Drawer:{rowDrawer:X} Entity: {rowEntity:X} {scenePtr:X} transform: {transform:X}");
            // rowIconPanels.Add(iconsPanel);
            rowTransforms.Add(transform);
            rowDrawers.Add(rowDrawer);
            rowRenderers.Add(rowRenderer);
        }

        IntPtr emptyPlaceholder = Marshal.StringToHGlobalAnsi("EmptyPlaceholder");
        var emptyPlaceholderEntity = _createEntity(scenePtr);

        var emptyPlaceholderRenderer = _createUiRenderer(
            scenePtr,
            emptyPlaceholderEntity,
            emptyPlaceholder,
        0);
        emptyRenderer = emptyPlaceholderRenderer;
        Marshal.FreeHGlobal(emptyPlaceholder);
        Write(emptyPlaceholderRenderer + 0x50, 0x0000002600000101);
        LogStr($"emptyPlaceholderRenderer 0x{emptyPlaceholderRenderer:X}");

        Write(_originalPanel + 0x118, _originalDrawer);

        // IntPtr bolatest = Marshal.StringToHGlobalAnsi("bolatest");
        // var bolatestEntity = _createEntity(scenePtr);

        // var bolatestRenderer = _createUiRenderer(
        //     scenePtr,
        //     bolatestEntity,
        //     bolatest,
        // 0);
        // Marshal.FreeHGlobal(bolatest);
        // Write(bolatestRenderer + 0x50, 0x0000002600000101);
        // LogStr($"bolatestRenderer 0x{bolatestRenderer:X}");
        
    }


    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _changeCloneText;

    static nint houseStatusEntityPtr = 0;
    static nint scenePtr = 0;
    static nint rowStatsRenderer = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CreateUiRendererHook(nint a1, nint entity, nint namePtr, nint a4)
    {
        var name = TryReadCString(namePtr);
        if (name == "HouseCatStatus")
        {
            if (rowStatsRenderer > 0)
            {
                return rowStatsRenderer;
            }
            LogStr($"[HOOK] CreateUiRendererHook: a1=0x{a1:X}, entity=0x{entity:X}, name=\"{name}\", a4=0x{a4:X}");
            scenePtr = a1;
            houseStatusEntityPtr = entity;
        }

        return _createUiRenderer(a1, entity, namePtr, a4);
    }

    static bool _panelIsOpen = false;
    static bool panelAnimationInProgress = false;
    static int waitingForPanelStatus = 0;
    static nint catMenuMc = 0;

    [UnmanagedCallersOnly]
    static unsafe nint UpdatePanelLayoutHook(nint a1)
    {

        var result = _updatePanelLayout(a1);

        var renderer = Marshal.ReadIntPtr(a1 + 0x58);
        // read as dword:
        var rendererName = TryReadStdString(renderer + 0xA8, false);
        if (rendererName != "CatMenu")
        {
            return result;
        }

        catMenuMc = Marshal.ReadIntPtr(renderer + 0x80);

        var rendererState = Marshal.ReadInt32(renderer + 0x54);
        if (rendererState == waitingForPanelStatus)
        {
            if (waitingForPanelStatus == 37)
            {
                waitingForPanelStatus = 0;
                panelAnimationInProgress = false;
                MewjectorApi.Log($"[HOOK] 1- Panel is open and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
            else if (waitingForPanelStatus == 36)
            {
                waitingForPanelStatus = 0;
                panelAnimationInProgress = false;
                for (int i = 0; i < rowDrawers.Count; i++)
                {
                    var drawer = rowDrawers[i];
                    Write(drawer + 0x78, (nint)0); // setting HouseCat reference to zero
                }

                MewjectorApi.Log($"[HOOK] 2- Pane is closed and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
        }
        return result;
    }

    [DllImport("kernel32.dll")]
    static extern nint GetCurrentProcess();

    static nint movieClipModContainer = 0;

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


    unsafe static nint CreateUTF16GameString(string text)
    {
        // Allocate enough space for the engine's std::wstring object.
        // 32 bytes is sufficient for the fields used by 370B100.
        
        nint str = Marshal.AllocHGlobal(0x30);
        LogStr($"[HOOK] Allocated std::wstring at 0x{str:X} for text '{text}'");
        // Initialize as an empty small-string.
        Buffer.MemoryCopy(null, (void*)str, 0, 0);

        *(ulong*)(str + 0x10) = 0; // length
        *(ulong*)(str + 0x18) = 7; // SSO capacity

        LogStr($"[HOOK] Writing text '{text}' to std::wstring at 0x{str:X}");

        fixed (char* p = text)
        {
            _assignString(str, p, (nuint)text.Length);
        }

        LogStr($"[HOOK] Finished writing text '{text}' to std::wstring at 0x{str:X}");

        return str;
    }
    
    [UnmanagedCallersOnly]
    static unsafe nint RemoveMovieClip(nint a1, nint a2, nint a3, nint a4)
    {
        if (catMenuMc != 0 && a1 == catMenuMc)
        {
            LogStr($"[HOOK] RemoveMovieClip called on mod container 0x{a1:X}");
            movieClipModContainer = 0;
            catMenuMc = 0;
            rowStatsRenderer = 0;
            scenePtr = 0;
            _btnCallbackMetadataPtr = 0;
            _ourBtnMetadataCallbackPtr = 0;
            _originalPanel = 0;
            _ourPanel = 0;
            _originalDrawer = 0;
            rowTransforms.Clear();
            rowDrawers.Clear();
            _totalCatsCount = -1;
            headersRenderer = 0;
            yOffset = 0;
            _renderersSoFar.Clear();
            alreadyInitializedDrawers.Clear();
        }
        return _removeMovieClipTrampoline(a1, a2, a3, a4);
    }
    

    static bool _active;   // static — accessible from [UnmanagedCallersOnly]


    protected void OnEnable()
    {
        Log("Catstable enabled");
    }

    protected void OnDisable()
    {
        Log("Catstable disabled");
    }

    static readonly string LogFilePath = @"E:\Documents\catstable\log.txt";
    static readonly StreamWriter _logWriter = new StreamWriter(
        new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read),
        System.Text.Encoding.UTF8, bufferSize: 4096, leaveOpen: false) { AutoFlush = true };
    static readonly object _logLock = new();

    private new void Log(string message)
    {
        lock (_logLock)
            _logWriter.WriteLine(message);
        // File.AppendAllText(LogFilePath, message + Environment.NewLine);
    }

    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _seenFns = new();

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
        // lock (_logLock)
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


public class FloatAnimator
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private readonly float _startValue;
    private readonly float _targetValue;
    private readonly float _durationSeconds;
    private readonly long _startTicks;

    public FloatAnimator(float startValue, float targetValue, float durationSeconds)
    {
        _startValue = startValue;
        _targetValue = targetValue;
        _durationSeconds = durationSeconds;
        _startTicks = _stopwatch.ElapsedTicks;
    }

    /// <summary>
    /// Current interpolated value.
    /// </summary>
    public float Tick()
    {
        float elapsedSeconds =
            (float)(_stopwatch.ElapsedTicks - _startTicks) / Stopwatch.Frequency;

        float t = Math.Min(elapsedSeconds / _durationSeconds, 1.0f);

        return Lerp(_startValue, _targetValue, t);
    }

    /// <summary>
    /// Returns true once the animation has finished.
    /// </summary>
    public bool IsFinished =>
        (_stopwatch.ElapsedTicks - _startTicks) >=
        _durationSeconds * Stopwatch.Frequency;

    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}