using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Linq;


public partial class TheSpredsheetEdmundHates
{
    static bool debugLogging = false;


    static unsafe delegate* unmanaged<nint, nint> updatePanelLayout;

    static unsafe delegate* unmanaged<nint, nint, nint, Renderer*> createUiRenderer;
    static unsafe delegate* unmanaged<nint, nint, nint, nint> createCatsDrawerHousePanel;
    public static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> registerButton;
    static unsafe delegate* unmanaged<nint, nint> statsCreator;
    static unsafe delegate* unmanaged<nint, nint, nint> getHouseCatByOffset;
    static unsafe delegate* unmanaged<nint, nint> gameTick;
    /*
        This function iterates all the following 
class glaiel::GameBase <class glaiel::GameBase> 
class glaiel::FurnitureBuildingUI <class glaiel::FurnitureBuildingUI> 
class glaiel::FurnitureClickHandler <class glaiel::FurnitureClickHandler> 
class glaiel::ButchBox <class glaiel::ButchBox> 
class glaiel::CatStatsDrawer <class glaiel::CatStatsDrawer> 
class glaiel::FurnitureGrid <class glaiel::FurnitureGrid> 
class glaiel::FurniturePiece <class glaiel::FurniturePiece> 
class glaiel::House <class glaiel::House> 
class glaiel::HouseCat <class glaiel::HouseCat> 
class glaiel::HouseCatClickManager <class glaiel::HouseCatClickManager> 
class glaiel::HouseCatPhysics <class glaiel::HouseCatPhysics> 
class glaiel::HouseDrawerUI <class glaiel::HouseDrawerUI> 
class glaiel::HousePipe <class glaiel::HousePipe> 
class glaiel::HouseTutorialDriver <class glaiel::HouseTutorialDriver> 
class glaiel::InventoryTrashDrawers <class glaiel::InventoryTrashDrawers> 
class glaiel::NPCMapDrawer <class glaiel::NPCMapDrawer> 
class glaiel::SingingCat <class glaiel::SingingCat> 
class glaiel::Button <class glaiel::Button> 
class glaiel::AbilityTooltip <class glaiel::AbilityTooltip> 
class glaiel::DialogController <class glaiel::DialogController> 
class glaiel::Tutorial <class glaiel::Tutorial> 
class glaiel::DebugDisplay <class glaiel::DebugDisplay> 
class glaiel::MewControls <class glaiel::MewControls> 
class glaiel::SimpleMusicPlayer <class glaiel::SimpleMusicPlayer> 
class glaiel::GlobalProgressionData <class glaiel::GlobalProgressionData> 
class glaiel::MewDirector <class glaiel::MewDirector> 
class glaiel::MewsicController <class glaiel::MewsicController> 
class glaiel::SpawnDatabase <class glaiel::SpawnDatabase> 

    */

    static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> catIterator;

    static unsafe delegate* unmanaged<nint, nint, nint, nint> findButton;
    static unsafe delegate* unmanaged<CatStatsDrawerLambda*, nint> initCatStatsClickCallback;
    static unsafe delegate* unmanaged<nint, nint> toggleHouseDrawer;
    static unsafe delegate* unmanaged<nint, nint, nint> clickHandler;
    static unsafe delegate* unmanaged<nint, nint, nint> mutationTooltip;

    unsafe static delegate* unmanaged<nint, nint, nint> mouseEventHandler;


    // static unsafe delegate* unmanaged<nint, nint, nint, nint, nint> _globalResourceManagerLookup;

    unsafe static delegate* unmanaged<nint, nint> endDay;
    unsafe static delegate* unmanaged<nint, nint> goToMainMenu;
    unsafe static delegate* unmanaged<nint, nint, nint, nint, nint> setCatData;
    unsafe static delegate* unmanaged<nint, nint, nint, nint, nint> fetchTranslation;
    unsafe static delegate* unmanaged<nint, nint, nint, nint> furnitureGridCreator;

    unsafe static delegate* unmanaged<nint, nint, nint, nint, nint> keyPress;
    unsafe static delegate* unmanaged<nint, nint> catStatsDrawerUpdate;



    static Dictionary<string, int> executionCounts = new();

    private static void CountExecution(string methodName)
    {
        if (debugLogging)
        {
            executionCounts[methodName] = executionCounts.GetValueOrDefault(methodName) + 1;
        }
    }

    static DateTime? MjInitStartTime = null;
    internal unsafe void MjInit()
    {
        MjInitStartTime = DateTime.Now;

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
        else
        {
            LogStr("Binary validation succeeded.");
        }

        Autoinjector();

        InitMouse();

        updatePanelLayout = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x204320, (void*)(delegate* unmanaged<nint, nint>)&UpdatePanelLayoutHook);

        createUiRenderer = (delegate* unmanaged<nint, nint, nint, Renderer*>)(void*)MewjectorApi.InstallHook(
            0x5A3D0, (void*)(delegate* unmanaged<nint, nint, nint, Renderer*>)&CreateUiRendererHook);

        createCatsDrawerHousePanel = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xef570, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&CreatePanelHook);

        registerButton = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x97c070, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&RegisterCallbackHook);

        statsCreator = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xE9AC0, (void*)(delegate* unmanaged<nint, nint>)&CreateCatStatsDrawerHook);

        getHouseCatByOffset = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xEAC70, (void*)(delegate* unmanaged<nint, nint, nint>)&GetHouseCatByOffsetHook);

        findButton = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x980E60, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&FindButtonHook);

        initCatStatsClickCallback = (delegate* unmanaged<CatStatsDrawerLambda*, nint>)(void*)MewjectorApi.InstallHook(
            0xEF3D0, (void*)(delegate* unmanaged<CatStatsDrawerLambda*, nint>)&InitCatStatsCallbackHook);

        toggleHouseDrawer = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x203C80, (void*)(delegate* unmanaged<nint, nint>)&ToggleHouseDrawerHook);

        clickHandler = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x97E8E0, (void*)(delegate* unmanaged<nint, nint, nint>)&ClickHandlerHook);

        gameTick = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x96AC50, (void*)(delegate* unmanaged<nint, nint>)&GameTickHook);

        catIterator = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xED220, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&CatIteratorHook);

        mutationTooltip = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xE5500, (void*)(delegate* unmanaged<nint, nint, nint>)&MutationTooltipHook);

        endDay = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x1f8ea0, (void*)(delegate* unmanaged<nint, nint>)&EndDayHook);

        goToMainMenu = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0x29cef0, (void*)(delegate* unmanaged<nint, nint>)&GoToMainMenuHook);

        setCatData = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xe1a00, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&SetCatDataHook);

        mouseEventHandler = (delegate* unmanaged<nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xc36110, (void*)(delegate* unmanaged<nint, nint, nint>)&MouseWheelHook);

        catStatsDrawerUpdate = (delegate* unmanaged<nint, nint>)(void*)MewjectorApi.InstallHook(
            0xeb170, (void*)(delegate* unmanaged<nint, nint>)&CatStatsDrawerUpdateHook);

        fetchTranslation = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x4C340, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&FetchTranslationHook);

        furnitureGridCreator = (delegate* unmanaged<nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0x1f6db0, (void*)(delegate* unmanaged<nint, nint, nint, nint>)&FurnitureGridCreatorHook);

        keyPress = (delegate* unmanaged<nint, nint, nint, nint, nint>)(void*)MewjectorApi.InstallHook(
            0xc083a0, (void*)(delegate* unmanaged<nint, nint, nint, nint, nint>)&KeyPressHook);
        // LogStr($"Gamebase at {MewjectorApi.GameBase:X}...");
        InitNativeFunctions();

    }

    static List<nint> rooms = new List<nint>();

    [UnmanagedCallersOnly]
    static unsafe nint FurnitureGridCreatorHook(nint a1, nint a2, nint a3)
    {
        var result = furnitureGridCreator(a1, a2, a3);
        rooms.Add(result);
        return result;
    }

    static string sortByStat = "";
    private enum SortDirection
    {
        Ascending,
        Descending
    }

    static SortDirection sortByStatDirection = SortDirection.Descending;
    static Dictionary<nint, double> averages = new();

    static nint ageTranslated = 0;
    static nint lvTranslated = 0;
    static bool IsOurPanelEnabled = true;

    static int lastNumKeyPressed = -1;
    static bool ctrlPressed = false;

    [UnmanagedCallersOnly]
    static unsafe nint KeyPressHook(nint a1, nint a2, nint a3, nint a4)
    {
        // rcx=470A62 rdx=100 r8=37 r9=80001 #7
        if (currentlyHoveredIndex != -1)
        {
            // LogStr($"[HOOK] KeyPressHook: a3=0x{a3:X}");
            if (a2 == 0x100 && (a3 >= 0x30) && (a3 <= 0x39))
            {
                lastNumKeyPressed = (int)(a3 - 0x30);
            }
            else if (a2 == 0x101 && (a3 >= 0x30) && (a3 <= 0x39))
            {
                lastNumKeyPressed = -1;
            }
        }
        else if (a3 == 0x11 && a2 == 0x100)
        {
            ctrlPressed = true;
        }
        else if (a3 == 0x11 && a2 == 0x101)
        {
            ctrlPressed = false;
        }
        // LogStr($"[HOOK] KeyPressHook: a1=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        return keyPress(a1, a2, a3, a4);
    }

    [UnmanagedCallersOnly]
    static unsafe nint FetchTranslationHook(nint a1, nint a2, nint a3, nint a4)
    {
        // return fetchTranslation(a1, a2, a3, a4);
        // return result;
        if (!IsLikelyPointer(a3) || headersRenderer == null)
        {
            return fetchTranslation(a1, a2, a3, a4);
        }
        if (lvTranslated != 0 && ageTranslated != 0)
        {
            return fetchTranslation(a1, a2, a3, a4);
        }
        var key = TryReadCString(Read<nint>(a3));
        var target = key == "HOUSE_CAT_INFO_LEVEL" ? "level" : key == "HOUSE_CAT_INFO_AGE" ? "age" : "";
        var result = fetchTranslation(a1, a2, a3, a4);
        if (target != "")
        {
            var text = ReadUtf16CustomString(Read<nint>(result + 0x8) + 0x30);
            if (target == "level")
            {
                lvTranslated = GameString.CreateUTF16GameString(text);
            }
            else
            {
                ageTranslated = GameString.CreateUTF16GameString(text.Replace("{age}", "").Replace(": ", ""));
            }
        }
        return result;
    }


    // [UnmanagedCallersOnly]
    // static unsafe nint GetChildHook(nint a1, nint a2)
    // {
    // return _getChild(a1, a2);
    //     // var name = Try
    //     var needle = TryReadCString(a2);
    //     return _getChild(a1, a2);
    // }

    [UnmanagedCallersOnly]
    static unsafe nint SetCatDataHook(nint a1, nint a2, nint a3, nint a4)
    {
        // return setCatData(a1, a2, a3, a4);
        LogStr($"[HOOK] SetCatDataHook: a1=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        // InsideSetCatData = true;
        var result = setCatData(a1, a2, a3, a4);
        LogStr($"[HOOK] SetCatDataHook: result=0x{result:X}");
        // InsideSetCatData = false;
        return result;
    }

    [UnmanagedCallersOnly]
    static unsafe nint GoToMainMenuHook(nint a1)
    {
        // return goToMainMenu(a1);
        LogStr($"[HOOK] GoToMainMenuHook: a1=0x{a1:X}");
        ClearState();
        return goToMainMenu(a1);
    }

    [UnmanagedCallersOnly]
    static unsafe nint EndDayHook(nint a1)
    {
        // return endDay(a1);
        CountExecution(nameof(EndDayHook));
        LogStr($"[HOOK] EndDayHook: a1=0x{a1:X}");
        ClearState();
        return endDay(a1);
    }

    [UnmanagedCallersOnly]
    static unsafe nint ClickHandlerHook(nint a1, nint a2)
    {
        var handledByUs = ButtonManager.handleNativeClick(a1);
        if (handledByUs)
        {
            return 0;
        }
        if (catAbilitiesButtons.Contains(a1))
        {
            // return 0;
            var text = ReadUtf16CustomString(a1 + 0x1B8) ?? "";
            var abilityId = text;
            LogStr($"[HOOK] ClickHandlerHook: abilityId={abilityId}");
            PutFirstInListByAbilityId(abilityId);
        }
        if (a1 == openCloseBtn && !ourPanelIsOpen)
        {
            if (ctrlPressed)
            {
                IsOurPanelEnabled = false;
                if (originalHousePanel != null)
                {
                    originalHousePanel->Effects = 0x0000000000000000;
                }
                ctrlPressed = false;
            }
            else
            {
                if (originalHousePanel != null)
                {
                    originalHousePanel->Effects = 0x0000000001000101;
                }
                IsOurPanelEnabled = true;
            }
        }
        return clickHandler(a1, a2);
    }

    static void PutFirstInListByAbilityId(string abilityId)
    {
        if (!string.IsNullOrEmpty(abilityId))
        {
            // print the whole abilities list, for debugging purposes
            foreach (var cat in sortedCats)
            {
                var abilities = catAbilities[cat];
                LogStr($"[HOOK] PutFirstInListByAbilityId: cat={cat} abilities={string.Join(", ", abilities)}");
            }

            sortedCats = sortedCats
                .OrderByDescending(
                    ren =>
                    {
                        var found = catAbilities[ren].Contains(abilityId) ? 1 : 0;
                        LogStr($"[HOOK] PutFirstInListByAbilityId: ren={ren} found={found} looking for abilityId={abilityId}");
                        return found;
                    }
                ).ToArray();

            sortedPositions = new Dictionary<nint, int>(sortedCats.Length);

            LogStr($"sortedPositions count = {sortedPositions.Count}");
            for (var i = 0; i < sortedCats.Length; i++)
                sortedPositions[sortedCats[i]] = i;

            LogStr($"Setting positionDirty=5 at PutFirstInListByAbilityId");
            positionDirty = 5;
        }
    }
    static void SortRows()
    {
        CountExecution(nameof(SortRows));
        forcedCatStatsUpdatePending = rowRenderers.Length;
        if (sortByStat == "")
        {
            sortedCats = catStats.Select(e => e.Key).ToArray();
            // print the full SortedCats array
            LogStr($"[HOOK] SortRows: sortByStat is empty, sortedCats = {string.Join(", ", sortedCats.Select(e => e.ToString("X")))}");
        }
        else
        {
            var sorted = sortByStat == "avg"
                ? catStats.OrderBy(e => averages.ContainsKey(e.Key) ? averages[e.Key] : 0)
                : catStats.OrderBy(e => e.Value.ContainsKey(sortByStat) ? e.Value[sortByStat] : 0);
            LogStr($"[HOOK] SortRows: sorted (before filtering by activeRowsRenderers) = {string.Join(", ", sorted.Select(e => e.Key.ToString("X")))}");
            sortedCats = sorted.Select(e => e.Key).Where(e => activeRowsRenderers.Contains(e)).ToArray();
            if (sortByStatDirection == SortDirection.Descending)
            {
                sortedCats = sortedCats.Reverse().ToArray();
            }
            positionDirty = 10;
            LogStr($"[HOOK] SortRows: sortByStat={sortByStat} sortByStatDirection={sortByStatDirection}, sortedCats = {string.Join(", ", sortedCats.Select(e => e.ToString("X")))}");

        }

        sortedPositions = new Dictionary<nint, int>(sortedCats.Length);
        LogStr($"sortedPositions count = {sortedPositions.Count}");
        for (var i = 0; i < sortedCats.Length; i++)
            sortedPositions[sortedCats[i]] = i;
        LogStr($"sortedCats = {string.Join(", ", sortedCats)}");
    }

    static nint currentlyDrawerWithOpenIconsPanel = 0;
    static nint currentlyHoveredIndex = -1;
    static nint previouslyHoveredIndex = -1;
    static nint forcedCatStatsUpdatePending = 0;

    [UnmanagedCallersOnly]
    static unsafe nint CatStatsDrawerUpdateHook(nint a1)
    {
        // return catStatsDrawerUpdate(a1);
        if (previouslyHoveredIndex != -1 && rowDrawers[previouslyHoveredIndex] == a1)
        {
            // continue with the rest of the function (once)
            previouslyHoveredIndex = -1;
        }
        else if (forcedCatStatsUpdatePending > 0)
        {
            forcedCatStatsUpdatePending--;
        }
        else
        {
            if (currentlyDrawerWithOpenIconsPanel == 0)
            {
                if (currentlyHoveredIndex == -1 || rowDrawers[currentlyHoveredIndex] != a1)
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

        var result = catStatsDrawerUpdate(a1);
        var iconsPanelIsOpen = Read<byte>(a1 + 0x71);
        if (iconsPanelIsOpen == 1)
        {
            currentlyDrawerWithOpenIconsPanel = a1;
        }
        else
        {
            currentlyDrawerWithOpenIconsPanel = 0;
        }
        return result;
    }


    static nint[] cachedVisibleCats = Array.Empty<nint>();

    [UnmanagedCallersOnly]
    static unsafe nint CatIteratorHook(nint a1, nint a2, nint a3, nint a4)
    {
        if (cachedVisibleCats.Length > 0)
        {
            // we already have cached visible cats, the first one is the important
            return catIterator(a1, a2, a3, a4);
        }
        // return catIterator (a1, a2, a3, a4);
        CountExecution(nameof(CatIteratorHook));

        cachedVisibleCats = new nint[a3];
        LogStr($"[HOOK] CatIteratorHook: a1={a1:X} a2={a2:X} a3={a3} a4={a4:X}");

        var j = 0;
        for (int i = 0; i < a3; i++)
        {
            // if (a1 + j * 8 == a2)
            // {
            //     j = j - (int)a3 ;
            // }
            cachedVisibleCats[j] = Marshal.ReadIntPtr(a1 + j * 8);
            LogStr($"[HOOK] CatIteratorHook: cachedVisibleCats[{j}] = {cachedVisibleCats[j]:X}");
            j++;
        }

        var result = catIterator(a1, a2, a3, a4);
        return result;
    }

    static unsafe void setActiveAllOurButtons(bool active, nint? specificRenderer = null)
    {
        var indexOnly = -1;
        if (specificRenderer.HasValue)
        {
            indexOnly = Array.IndexOf(rowRenderers, specificRenderer.Value);
        }
        for (var i = 0; i < buttonsInsideOurDrawers.Count; i++)
        {
            if (indexOnly != -1 && i != indexOnly)
            {
                continue;
            }
            var buttonContainer = buttonsInsideOurDrawers[i];
            var btnCount = 0;
            foreach (var button in buttonContainer)
            {
                var btn = (Button*)button;
                // Write(button + 0x10, (byte)(active ? 1 : 0)); // set button active state
                btn->Enabled = (byte)(active ? 1 : 0); // set button active state
                btnCount++;
                for (int k = 0; k < btn->Entity->ComponentsCount; k++)
                {
                    Component** slot = btn->Entity->ComponentsList + k;
                    Component* component = *slot;
                    nint vtable = Read<nint>((nint)component);
                    LogStr("Looking for component at " + ((nint)component).ToString("X"));
                    if (component != null && vtable == _audioSourceVtable)
                    {
                        // is a AudioSource component
                        LogStr($"[HOOK] Found AudioSource component at {((nint)component):X}");
                        var audioSource = (AudioSource*)component;
                        audioSource->Enabled = (byte)(active ? 1 : 0);
                    }
                    // Do something with the component if needed
                }
            }
            // LogStr($"[HOOK] Setting {btnCount} buttons in container at buttonContainer[{i}] to {(active ? "active" : "inactive")}");
        }
    }


    static Dictionary<nint, byte> visibilityBefore = new Dictionary<nint, byte>();
    [UnmanagedCallersOnly]
    static unsafe nint ToggleHouseDrawerHook(nint a1)
    {
        // return toggleHouseDrawer(a1);
        if (originalHousePanel == null || !IsOurPanelEnabled)
        {
            return toggleHouseDrawer(a1);
        }
        CountExecution(nameof(ToggleHouseDrawerHook));

        nint panel = Marshal.ReadIntPtr(a1 + 0x58);
        nint state = Marshal.ReadIntPtr(a1 + 0x50);
        var changed = false;

        if ((HousePanel*)panel != originalHousePanel && ourPanelIsOpen && state == 0)
        {
            LogStr($"[HOOK] Panel Close, state={state:X}");
            ourPanelIsOpen = false;
            lastNumKeyPressed = -1;
            changed = true;
            catPanelAnimationInProgress = true;
            waitingForPanelStatus = PanelStateClosed;
            setActiveAllOurButtons(false);
            foreach (var catPartPtr in catPartsInsideOurDrawers)
            {
                if (catPartPtr != 0)
                {
                    // LogStr($"[HOOK] Disabling cat part at {catPartPtr:X}");
                    var catPart = (CatPart*)catPartPtr;
                    catPart->Enabled = 0; // disable cat part
                }
            }

        }
        else if ((HousePanel*)panel == originalHousePanel && !ourPanelIsOpen && state == 1)
        {
            // print executionCounts for debugging:
            LogStr($"[HOOK] ToggleHouseDrawerHook: state={state:X}");
            executionCounts = executionCounts.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
            LogStr($"[HOOK] ToggleHouseDrawerHook: executionCounts = {string.Join(", ", executionCounts.Select(kv => $"{kv.Key}={kv.Value}"))}");
            // OPEN
            LogStr($"[HOOK] Panel Open");
            yOffset = 0;
            yOffsetTarget = 0;
            ourPanelIsOpen = true;

            updateRenderersInsideScreenArea(true);
            LogStr($"[HOOK] Before initializing buttons");
            forcedCatStatsUpdatePending = rowRenderers.Length;
            initializeButtons();
            LogStr($"[HOOK] After initializing buttons");
            changed = true;
            catPanelAnimationInProgress = true;
            waitingForPanelStatus = PanelStateOpen;

        }
        else
        {
            // LogStr($"[HOOK] Panel state unchanged, panel={panel:X}, state={state:X} originalHousePanel={originalHousePanel:X} ourPanelIsOpen={ourPanelIsOpen}");
        }
        if (changed)
        {
            LogStr(
                $"[HOOK] Panel changed, ourPanelIsOpen={ourPanelIsOpen}, xOffset={xOffset}, xOffsetTarget={xOffsetTarget}, xMoveAni="
                    + (xMoveAni == null ? "null" : xMoveAni.Tick()) + " xMoveTvalue=" + (xMoveAni == null ? "null" : xMoveAni.t));
            xOffsetTarget = ourPanelIsOpen ? 10 : -92;
            xMoveAni = new FloatAnimator(xOffset, xOffsetTarget, 0.4f);
            positionDirty = 10;
            LogStr($"Setting positionDirty=10 at ToggleHouseDrawerHook");
        }
        return toggleHouseDrawer(a1);
    }


    static nint lastButtonCSD = 0;
    static nint lastButtonReturned = 0;
    [UnmanagedCallersOnly]
    static unsafe nint FindButtonHook(nint a1, nint a2, nint a3)
    {
        // return findButton(a1, a2, a3);
        var result = findButton(a1, a2, a3);
        if (!ourPanelIsOpen)
        {
            return result;
        }
        CountExecution(nameof(FindButtonHook));
        if (result != 0)
        {
            if (lastButtonReturned != result)
            {
                lastButtonCSD = Read<nint>(Read<nint>(Read<nint>(Read<nint>(result + 0x38) + 0x18) + 0x28) + 0x10);
                lastButtonReturned = result;
            }
            // LogStr($"[HOOK] result!=0 {result:X} x={x:X}");
        }
        else
        {
            // LogStr($"[HOOK] EMPTY! lastButtonCSD = 0");
            lastButtonCSD = 0;
            lastButtonReturned = 0;
        }
        return result;
    }

    [UnmanagedCallersOnly]
    static unsafe nint MutationTooltipHook(nint a1, nint a2)
    {
        // return mutationTooltip(a1, a2);
        // MutationTooltip has an issue that it wasn't build with multiple catStatsDrawers instances in mind
        // We need to compare it with the CatStatDrawer instance we get from FindButtonHook to make it work
        CountExecution(nameof(MutationTooltipHook));

        if (a1 == 0)
        {
            return 0;
        }
        if (lastButtonCSD != 0)
        {
            var opt1 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28) + 0x10);
            var opt2 = Read<nint>(Read<nint>(Read<nint>(a1 + 0x18) + 0x28));
            // LogStr($"[HOOK] opt1={opt1:X} opt1={opt2:X} lastButtonCSD={lastButtonCSD:X}");
            if (opt1 == lastButtonCSD || opt2 == lastButtonCSD)
            {
                // LogStr($"[HOOK] MATCH!");
                return mutationTooltip(a1, a2);
            }
            else
            {
                // LogStr($"[HOOK] MutationTooltipHook: Return zero !");
                return 0;
            }
        }
        // LogStr($"[HOOK] lastButtonCSD == 0");
        return mutationTooltip(a1, a2);
    }



    static bool initializedButtons = false;
    // static List<nint> headerButtons = new();
    static Dictionary<string, nint> OurHeaderbuttons = new Dictionary<string, nint>();
    static unsafe Button* footerButton = null;

    static unsafe void handleFooterClick(nint btn, ButtonData? footerData)
    {
        LogStr($"[HOOK] ClickHandlerHook: a1=0x{btn:X} is our footer button, opening kofi link");
        Process.Start(new ProcessStartInfo
        {
            FileName = showingChimplantsPromo ? "https://www.chimplants.com/" : "https://ko-fi.com/chimplants",
            UseShellExecute = true
        });
    }

    static unsafe void handleHeaderClick(nint btn, ButtonData? headerData)
    {
        // if (catStats.Count > 0 && catStats.FirstOrDefault().Value.Count > 0)
        // {
        //     var ren = catStats.FirstOrDefault().Key;
        //     LogStr($"[HOOK] ClickHandlerHook: ren=0x{ren:X}");
        //     noop = Read<byte>((nint)MewjectorApi.GameBase + 0x60);

        //     var mucMc = GameString.Create("mutations.mutationcount");
        //     var bdcMc = GameString.Create("mutations.birthdefectcount");
        //     var muc = ReadNumberOrZero(_getChildByPath(Read<nint>(ren + 0x80), mucMc));
        //     var bdc =ReadNumberOrZero(_getChildByPath(Read<nint>(ren + 0x80), bdcMc));
        //     var muts = _getChild(ren + 0x80, GameString.Create("mutations"));
        //     LogStr($"[HOOK] ClickHandlerHook: 1st mutations.mutationcount={muc}, mutations.birthdefectcount={bdc} mucMc={mucMc}, bdcMc={bdcMc} muts=0x{muts:X}");
        // }

        // MovieClip* movieclip = (MovieClip*)Read(btn + 0x48);
        // LogStr($"[HOOK] ClickHandlerHook: movieclip=0x{(nint)movieclip:X} movieclip->Name=0x{movieclip->Name:X}");
        // get first 3 letters
        var subname = headerData?.id.Replace("_btn", "");
        if (subname == sortByStat)
        {
            sortByStatDirection = sortByStatDirection == SortDirection.Ascending
                ? SortDirection.Descending
                : SortDirection.Ascending;
        }
        else
        {
            sortByStatDirection = SortDirection.Descending;
        }
        sortByStat = subname!;
        SortRows();
        LogStr($"[HOOK] ClickHandlerHook: sorting by {sortByStat} {sortByStatDirection}");

    }

    class ButtonData
    {
        public string id;
        // constructor:
        public ButtonData(string id)
        {
            this.id = id;
        }
    }

    static unsafe void setHoveredCatRoom(int newIndex)
    {
        if (newIndex >= rooms.Count || newIndex < 0)
        {
            LogStr($"[HOOK] New room index out of bounds: newIndex={newIndex}");
            return;
        }
        if (currentlyHoveredIndex == -1)
        {
            LogStr($"[HOOK] No cat is currently hovered: currentlyHoveredIndex={currentlyHoveredIndex}");
            return;
        }
        var cat = ((CatStatsDrawer*)rowDrawers[currentlyHoveredIndex])->HouseCat;
        LogStr($"[HOOK] New room index: newIndex={newIndex}");
        var room = rooms[newIndex];
        catRooms[currentlyHoveredIndex] = room;
        var movieclip = ((Renderer*)rowRenderers[currentlyHoveredIndex])->MovieClip;
        var roomTextbox = _getChild((nint)movieclip, GameString.Create("room_number"));
        var roomStr = room == -1 ? "-" : $"{rooms.IndexOf(room) + 1}";
        _setText(roomTextbox, GameString.CreateUTF16GameString(roomStr));
        LogStr($"[HOOK] Assigning room: room=0x{room:X} cat=0x{(nint)cat:X}");
        _assingRoomFn(room, (nint)cat);
        // var cat = Read<nint>((nint)MewjectorApi.GameBase + 0x60);
        // var room = Read<nint>((nint)MewjectorApi.GameBase + 0x68);
        // LogStr($"[HOOK] Room button clicked: room=0x{room:X} cat=0x{cat:X}");

    }
    static unsafe void initializeButtons()
    {
        if (initializedButtons)
            return;

        for (int i = 0; i < rowRenderers.Length; i++)
        {

            var root = (nint)((Renderer*)rowRenderers[i])->MovieClip;
            var menupanel = (nint)((CatStatsDrawer*)rowDrawers[i])->MenuPanel;

            var roomMovieclip = _getChild(root, GameString.Create("room_btn"));
            ButtonManager.createButton("room_btn", menupanel, (nint btn, ButtonData? data) =>
            {
                if (currentlyHoveredIndex == -1)
                {
                    LogStr($"[HOOK] ClickHandlerHook: currentlyHoveredIndex is -1, weird");
                    return;
                }
                var cat = ((CatStatsDrawer*)rowDrawers[currentlyHoveredIndex])->HouseCat;
                var room = (nint)((HouseCat*)cat)->Room;
                var newIndex = rooms.IndexOf(room) + 1;
                if (newIndex == rooms.Count)
                    newIndex = 0;
                LogStr($"[HOOK] New room index: newIndex={newIndex}");
                setHoveredCatRoom(newIndex);
                // var cat = Read<nint>((nint)MewjectorApi.GameBase + 0x60);
                // var room = Read<nint>((nint)MewjectorApi.GameBase + 0x68);
                // LogStr($"[HOOK] Room button clicked: room=0x{room:X} cat=0x{cat:X}");

                LogStr($"[HOOK] Room button clicked: btn=0x{btn:X} data.id={data?.id}");
            }, new ButtonData("room_btn"));
        }

        initializedButtons = true;
        LogStr($"Initializing buttons..., headers renderer = {(nint)headersRenderer:X}");
        if (headersRenderer == null)
        {
            LogStr("headersRenderer is null, cannot initialize buttons.");
            return;
        }
        nint headersEntity = Marshal.ReadIntPtr((nint)headersRenderer + 0x18);

        LogStr("Creating headerMenuPanel...");

        LogStr("Calling _createMenuPanel for headerMenuPanel...");
        nint headerMenuPanel = _createMenuPanel(
            headersEntity,
            GameString.Create("row_headers")
        );

        LogStr($"Created headerMenuPanel = 0x{headerMenuPanel:X}");

        if (headerMenuPanel == 0)
            return;

        for (int i = 0; i < buttonList.Length; i++)
        {
            var btnName = buttonList[i] + "_btn";
            ButtonManager.createButton(btnName, headerMenuPanel, handleHeaderClick, new ButtonData(btnName));
        }

        if (footerRenderer != null)
        {
            nint footerEntity = footerRenderer->Entity;
            nint footerMenuPanel = _createMenuPanel(
                footerEntity,
                GameString.Create("row_footer")
            );

            var id = showingChimplantsPromo ? "chimp_btn" : "kofi_btn";
            ButtonManager.createButton(id, footerMenuPanel, handleFooterClick, new ButtonData(id));
            LogStr($"Registered footer btn with the game's register-newButton function, result = 0x{footerMenuPanel:X}");
        }

    }

    [UnmanagedCallersOnly]
    static nint TestButtonCallback(nint callbackObject)
    {
        // return _callbackObject);
        // This is never executed, don't know why, doesn't matter because
        // we just intercept our buttons at ClickHandlerHook
        return 0;
    }

    private static float xOffset = -300f;
    private static float xOffsetTarget = -300f;
    static FloatAnimator? xMoveAni;

    private static float yOffset = 0;
    private static float yOffsetTarget = 0;
    static FloatAnimator? yScrollAni;
    [UnmanagedCallersOnly]
    static unsafe nint MouseWheelHook(nint a1, nint a2)
    {
        // return mouseEventHandler(a1, a2);
        if (ourPanelIsOpen && IsLikelyPointer(a2))
        {
            var somethingCoveringOurPanel = Read<bool>(originalDrawerParent + 0x4DA);

            var a2Val = Marshal.ReadInt32(a2);
            if (a2Val == 1027 && !somethingCoveringOurPanel)
            {
                CountExecution(nameof(MouseWheelHook));
                if (activeRowsRenderers.Count > 10)
                {
                    var intValue = Marshal.ReadInt32(a2 + 0x1C);
                    var scrollable = activeRowsRenderers.Count - 10;
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
                        updateRowTransforms();
                    }
                }
                return 0;
            }
        }
        // return 0;
        return mouseEventHandler(a1, a2);
    }

    // unsafe static bool _insideHouseCatByOffset = false;
    unsafe static int totalCatsCount = -1;

    [UnmanagedCallersOnly]
    static unsafe nint GetHouseCatByOffsetHook(nint a1, nint a2)
    {
        // return getHouseCatByOffset(a1, a2);
        CountExecution(nameof(GetHouseCatByOffsetHook));

        // _insideHouseCatByOffset = true;
        nint result;
        if (isIteratingOurDrawers)
        {
            LogStr($"[HOOK] GetHouseCatByOffsetHook: returning cached cat at index {catIndex}");
            if ((uint)catIndex >= (uint)cachedVisibleCats.Length)
            {
                LogStr($"[HOOK] GetHouseCatByOffsetHook: catIndex {catIndex} is out of range for cachedVisibleCats.Length {cachedVisibleCats.Length}");
                // _insideHouseCatByOffset = false;
                return 0;
            }

            result = cachedVisibleCats[catIndex];
            LogStr($"[HOOK] GetHouseCatByOffsetHook: cachedVisibleCats result={result:X}");
        }
        else
        {
            LogStr($"Getting cat at {a1:X} {a2:X}");
            result = getHouseCatByOffset(a1, a2);
        }
        // _insideHouseCatByOffset = false;
        LogStr($"[HOOK] GetHouseCatByOffsetHook: result={result:X}");
        return result;
    }


    unsafe static int catIndex = 0;
    unsafe static bool isIteratingOurDrawers = false;
    unsafe static List<IntPtr> activeRowsRenderers = new();
    static int disableButtonsInTicks = -1;
    static List<nint> alreadyInitializedRenderers = new();
    [UnmanagedCallersOnly]
    static unsafe nint InitCatStatsCallbackHook(CatStatsDrawerLambda* a1)
    {

        // foreach (var btn in theirButtons)
        // {
        //     // Disable the button if needed
        //     if (btn == openCloseBtn)
        //     {
        //         continue;
        //     }
        //     Write(btn + 0x10, (byte)0); 
        // }
        // return initCatStatsClickCallback(a1);
        CountExecution(nameof(InitCatStatsCallbackHook));

        LogStr($"[HOOK] InitCatStatsCallbackHook called: a1=0x{(nint)a1:X}");
        a1->CatStatsDrawer = (CatStatsDrawer*)originalDrawer;
        var prevCachedVisibleCatsCount = cachedVisibleCats.Length;
        cachedVisibleCats = new nint[0];
        var result = initCatStatsClickCallback(a1);
        // if (cachedVisibleCats.Length != prevCachedVisibleCatsCount || cachedVisibleCats.Length != totalCatsCount)
        // {
        if (IsOurPanelEnabled)
        {
            pendingOurInitCatStats = true;
            btnInitCatStatsCallback = a1;
            yScrollAni = new FloatAnimator(0f, 0f, 1f);
        }
        // } else
        // {
        //     disableButtonsInTicks = 10;
        // }


        return result;
    }

    static bool pendingOurInitCatStats = false;
    static unsafe CatStatsDrawerLambda* btnInitCatStatsCallback = null;


    [UnmanagedCallersOnly]
    static unsafe nint CreatePanelHook(nint a1, nint a2, nint a3)
    {
        // return createCatsDrawerHousePanel(a1, a2, a3);
        CountExecution(nameof(CreatePanelHook));

        if (originalHousePanel != null && insideOurCatInstantiation)
        {
            LogStr($"CreatePanelHook returning originalPanel a1={a1:X} a2={a2:X} a3={a3:X}");
            return (nint)originalHousePanel;
        }
        var result = createCatsDrawerHousePanel(a1, a2, a3);
        LogStr($"CreatePanelHook called a1={a1:X} a2={a2:X} a3={a3:X} result={result:X}");
        originalHousePanel = (HousePanel*)result;
        // This write is to set blur effect on click:
        // Write<nint>(originalHousePanel + 0xC8, 0x0000000001000101);
        originalHousePanel->Effects = 0x0000000001000101;
        return result;
    }


    static unsafe HousePanel* originalHousePanel = null;

    unsafe static delegate* unmanaged<nint, void> buttonCallbackResolverPtr = null;
    static bool isInsideCreateCatStatsDrawerHook = false;
    static unsafe CatStatsDrawer* originalDrawer = null;
    static nint originalDrawerParent = 0;
    static nint framesSinceInitialCatStatsDrawer = 0;
    [UnmanagedCallersOnly]
    static unsafe nint CreateCatStatsDrawerHook(nint a1)
    {
        // return statsCreator(a1);
        CountExecution(nameof(CreateCatStatsDrawerHook));
        LogStr($"[HOOK] CreateCatStatsDrawerHook called: a1=0x{a1:X}");
        isInsideCreateCatStatsDrawerHook = true;
        nint result = statsCreator(a1);
        isInsideCreateCatStatsDrawerHook = false;

        if (originalDrawer == null)
        {
            framesSinceInitialCatStatsDrawer = 0;
            originalDrawer = (CatStatsDrawer*)a1;
            originalDrawerParent = Read<nint>(a1 + 0x20);
        }

        LogStr($"[HOOK] CreateCatStatsDrawerHookkkk: a1=0x{a1:X}, result=0x{result:X}");
        return result;
    }


    static nint openCloseBtn = 0;
    static List<nint> theirButtons = new List<nint>();
    static List<List<nint>> buttonsInsideOurDrawers = new List<List<nint>>();
    static nint[] catPartsInsideOurDrawers = new nint[0];
    [UnmanagedCallersOnly]
    static unsafe nint RegisterCallbackHook(nint menuPanel, nint a2, nint a3, nint a4)
    {
        // return registerButton(menuPanel, a2, a3, a4);
        var result = registerButton(menuPanel, a2, a3, a4);
        var entityAddr = menuPanel + 0x18;
        var rendererAddr = menuPanel + 0x38;
        if (!IsMemReadable(rendererAddr, 8) || !IsMemReadable(entityAddr, 8))
        {
            return result;
        }
        var renderer = (Renderer*)Read(rendererAddr);
        if (isIteratingOurDrawers || insideOurCatInstantiation)
        {
            var index = Array.IndexOf(rowRenderers, (nint)renderer);
            if (index != -1)
            {
                LogStr($"[HOOK] RegisterCallbackHook: isIteratingOurDrawers is {isIteratingOurDrawers}, index={index}, menuPanel=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}");
                buttonsInsideOurDrawers[index].Add(result);
            }
            else
            {
                LogStr($"[HOOK] RegisterCallbackHook: isIteratingOurDrawers is {isIteratingOurDrawers}, index not found, menuPanel=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}");
            }
        }
        else
        {
            theirButtons.Add(result);
        }
        if (!isInsideCreateCatStatsDrawerHook || !IsMemReadable(rendererAddr, 8) || !IsMemReadable(entityAddr, 8))
        {
            // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}");
            return result;
        }
        CountExecution(nameof(RegisterCallbackHook));
        var btnName = TryReadCString(a2);
        var rendererName = renderer->Name;
        // LogStr($"[HOOK] RegisterCallbackHook inside CatStats: a1=0x{menuPanel:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}, result=0x{result:X}, btnName=\"{btnName}\", renderer=0x{renderer:X}, rendererName={rendererName}");
        if (rendererName != "CatMenu")
        {

            LogStr($"[HOOK] RegisterCallbackHook: rendererName is not CatMenu, rendererName={rendererName} menuPanel=0x{menuPanel:X} a2=0x{a2:X} a3=0x{a3:X} a4=0x{a4:X} result=0x{result:X}");
            return result;
        }
        var entity = Marshal.ReadIntPtr(entityAddr);
        var componentsList = Marshal.ReadIntPtr(entity + 0x28);
        var houseDrawerPanel = Marshal.ReadIntPtr(componentsList + 0x0);
        LogStr($"[HOOK] RegisterCallbackHook inside CatMenu: entity=0x{entity:X}, componentsList=0x{componentsList:X}, houseDrawerPanel=0x{houseDrawerPanel:X}");
        var movieclipPtr = result + 0x48;
        if (!IsMemReadable(movieclipPtr, 8))
        {
            LogStr($"[HOOK] RegisterCallbackHook: movieclip is not readable at 0x{movieclipPtr:X}, returning result=0x{result:X}");
            return result;
        }

        var mcPtr = (MovieClip*)Read(movieclipPtr);
        var name = TryReadCString(mcPtr->Name);
        if (name != "openclose")
        {
            LogStr($"[HOOK] RegisterCallbackHook: openclose button not found, name={name} at 0x{mcPtr->Name:X}, mcPtr=0x{(nint)mcPtr:X}, movieclip=0x{movieclipPtr:X}");
            return result;
        }
        openCloseBtn = (nint)result;

        LogStr($"[HOOK] RegisterCallbackHook: CatMenu found at 0x{(nint)renderer:X}, openclose button found at 0x{(nint)mcPtr:X}");
        if (originalHousePanel == null)
        {
            // noop = Read<byte>((nint)MewjectorApi.GameBase + 0x60);
            originalHousePanel = (HousePanel*)houseDrawerPanel;
        }
        // LogStr($"[HOOK] RegisterCallbackHook: menuPanel=0x{a1:X}, a2=0x{a2:X}, a3=0x{a3:X}, a4=0x{a4:X}");
        return result;
    }

    static readonly string[] buttonList = ["spd", "cha", "int", "str", "lck", "con", "dex", "avg", "bdc", "muc", "lev", "age", "hou"];

    static Dictionary<nint, Dictionary<string, int>> catStats = new();
    static Dictionary<nint, List<string>> catAbilities = new();
    static List<nint> catAbilitiesButtons = new();
    static Dictionary<nint, nint> catRooms = new();
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
        if (name == "age")
        {
            return "age";
        }
        if (name == "level")
        {
            return "lev";
        }
        if (name == "total")
        {
            var parentMovieclip = (MovieClip*)Read(dynamicTextBox + 0x38);
            var parentName = TryReadCString(parentMovieclip->Name) ?? "";
            // get first 3 letters
            return parentName.Length >= 3 ? parentName[..3] : "";
        }
        return "";
    }

    static unsafe int ReadNumberOrZero(nint dynamicTextBox)
    {
        if (dynamicTextBox == 0)
        {
            return 0;
        }
        var str = ReadUtf16CustomString(dynamicTextBox + 0xB8);
        if (str == "")
        {
            return 0;
        }
        return int.Parse(new string(str.Where(char.IsDigit).ToArray()));
    }

    static unsafe void InitOurCatStats()
    {
        // Implementation of InitOurCatStats goes here

        // nint result = 0;
        isIteratingOurDrawers = true;
        catIndex = 0;
        activeRowsRenderers.Clear();
        catPartsInsideOurDrawers = new nint[rowDrawers.Length * 3];
        setActiveAllOurButtons(true);

        for (int i = 0; i < rowDrawers.Length; i++)
        {
            LogStr($"[HOOK] InitCatStatsCallbackHook: iterating drawer {i} at address {(nint)rowDrawers[i]:X}");
            var drawer = (CatStatsDrawer*)rowDrawers[i];
            drawer->enabled = 1;
            btnInitCatStatsCallback->CatStatsDrawer = (CatStatsDrawer*)drawer;
            LogStr($"[HOOK] InitCatStatsCallbackHook: iterating drawer {i} at address {(nint)drawer:X}");
            activeRowsRenderers.Add(drawer->Renderer.Address);
            // if (!alreadyInitializedRenderers.Contains(renderer))
            // {
            initCatStatsClickCallback(btnInitCatStatsCallback);
            LogStr($"[HOOK] InitCatStatsCallbackHook: after read cat parts for drawer {i} at address {(nint)drawer:X}" +
                $" catParts1={drawer->catsParts1:X} catParts2={drawer->catsParts2:X} catParts3={drawer->catsParts3:X}");
            catPartsInsideOurDrawers[i * 3] = drawer->catsParts1;
            catPartsInsideOurDrawers[i * 3 + 1] = drawer->catsParts2;
            catPartsInsideOurDrawers[i * 3 + 2] = drawer->catsParts3;
            // }
            catIndex++;

            if (catIndex == cachedVisibleCats.Length)
                break;
        }


        foreach (var ren in activeRowsRenderers)
        {
            LogStr($"[HOOK] Processing renderer {ren:X}");
            var drawer = rowDrawers[Array.IndexOf(rowRenderers, ren)];
            LogStr($"[HOOK] drawer=0x{(nint)drawer:X}");
            var movieclip = ((Renderer*)ren)->MovieClip;
            LogStr($"[HOOK] movieclip=0x{(nint)movieclip:X}");
            var houseCat = ((CatStatsDrawer*)drawer)->HouseCat;
            var catRoomPtrLong = houseCat->Room;
            var catRoomPtr = (nint)catRoomPtrLong;
            LogStr($"[HOOK] catRoomPtr=0x{catRoomPtr:X} houseCat=0x{(nint)houseCat:X}");
            if (!catStats[ren].ContainsKey("hou") || catStats[ren]["hou"] != rooms.IndexOf(catRoomPtr))
            {
                LogStr($"[HOOK] catRoomPtr=0x{catRoomPtr:X}");
                var catRoom = rooms.IndexOf(catRoomPtr);
                LogStr($"[HOOK] catRoom={catRoom}");
                var roomTextbox = _getChild((nint)movieclip, GameString.Create("room_number"));
                var roomStr = catRoomPtr == -1 ? "-" : $"{catRoom + 1}";
                LogStr($"[HOOK] GameTickHook: catRoomPtr=0x{catRoomPtr:X} catRoom={catRoom} drawer=0x{drawer:X} roomTextbox=0x{(nint)roomTextbox:X}");
                _setText(roomTextbox, GameString.CreateUTF16GameString(roomStr));
                // catRooms[ren] = catRoomPtr;
                catStats[ren]["hou"] = catRoom;
            }
            var menupanel = ((CatStatsDrawer*)drawer)->MenuPanel;

            LogStr($"Looking buttons in menu panel at 0x{menupanel:X}");
            List<string> abilityIds = new List<string>();
            nint[] abilities = [
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "attack"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell0"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell1"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell2"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell3"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell4"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "spell5"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "passive0"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "passive1"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "passive2"),
                (nint)FindButtonInPanel((MenuPanel*)menupanel, "passive3"),
            ];
            catAbilities[ren] = new List<string>();
            for (int i = 0; i < abilities.Length; i++)
            {
                var btn = abilities[i];
                if (btn == 0)
                {
                    continue;
                }
                var abilityName = ReadUtf16CustomString(btn + 0x1B8) ?? "";
                if (string.IsNullOrEmpty(abilityName))
                {
                    LogStr($"[HOOK] Ability name is empty for button at address 0x{btn:X} for drawer 0x{drawer:X}");
                    continue;
                }
                catAbilitiesButtons.Add(btn);
                var output = Marshal.AllocHGlobal(0x30); // Allocate 256 bytes for the output
                NativeMemory.Clear((void*)output, 0x30);
                LogStr($"[HOOK] Allocated output buffer at address 0x{(nint)output:X}");
                _getSpellIdFromButton(output, btn + 0x8);
                LogStr($"[HOOK] Retrieved spell from button at address 0x{btn:X} into output at address 0x{(nint)output:X}");
                catAbilities[ren].Add(abilityName);
                LogStr($"[HOOK] Ability id retrieved: {abilityName}");
            }
        }

        SortRows();
        isIteratingOurDrawers = false;

        // var mc = ((CatStatsDrawer*)rowDrawers[0])->Renderer.Value.MovieClip;
        // LogStr($"[HOOK] InitCatStatsCallbackHook: first drawer's MovieClip at address {(nint)mc:X}");
        // var attack = _getChild((nint)mc, GameString.Create("attack"));
        // var menuPanel = Marshal.ReadIntPtr(rowDrawers[0] + 0x68);
        // LogStr($"[HOOK] InitCatStatsCallbackHook: first drawer's menu panel at address {(nint)menuPanel:X}");
        // // Empty callback storage for the experiment.

        // LogStr($"[HOOK] InitCatStatsCallbackHook: first drawer's attack child at address {(nint)attack:X}");
        // noop = Read<byte>((nint)MewjectorApi.GameBase + 0x60);

        // var btn = registerButton(menuPanel, GameString.Create("attack"), callbackStorage, callback);
        // LogStr($"[HOOK] InitCatStatsCallbackHook: registered attack button at address {(nint)btn:X}");


        disableButtonsInTicks = 10;
        // Write(btnInitCatStatsCallback + 0x8, originalDrawer);
        btnInitCatStatsCallback->CatStatsDrawer = originalDrawer;
        LogStr($"[HOOK] InitCatStatsCallbackHook: set original drawer to btnInitCatStatsCallback at address {(nint)btnInitCatStatsCallback:X} and points to {(nint)btnInitCatStatsCallback->CatStatsDrawer:X}");

    }

    static int positionDirty = 0;
    static List<nint> panelsWithData = new();
    static DateTime ticksStartTime;
    [UnmanagedCallersOnly]
    static unsafe nint GameTickHook(nint a1)
    {

        if (!IsOurPanelEnabled)
        {
            return gameTick(a1);
        }

        handleDelayedPerfTime();
        // return gameTick(a1);
        // call mewgenics.7FF647368A30
        // [[[rax+0x38]+18]+58] 
        if (pendinUpdateRenderersInsideScreen.HasValue && pendinUpdateRenderersInsideScreen <= DateTime.Now)
        {
            LogStr("[HOOK] GameTickHook: Updating renderers inside screen area (pendingRenderersInsideScreen)");
            updateRenderersInsideScreenArea();
            pendinUpdateRenderersInsideScreen = null;
        }
        if (pendinUpdateTransformInTick.HasValue && pendinUpdateTransformInTick <= DateTime.Now)
        {
            LogStr("[HOOK] GameTickHook: Updating renderers inside screen area");
            updateRowTransforms(true);
            pendinUpdateRenderersInsideScreen = DateTime.Now.AddMilliseconds(10);
            pendinUpdateTransformInTick = null;
        }
        if (pendingOurInitCatStats)
        {
            LogStr("[HOOK] GameTickHook: Initializing our cat statsNames");
            InitOurCatStats();
            pendingOurInitCatStats = false;
        }
        StartGlobalPerfLog("GameTick");
        var result = gameTick(a1);
        EndGlobalPerfLog("GameTick");
        StartGlobalPerfLog("PostGameTick");
        if (disableButtonsInTicks != -1)
        {
            disableButtonsInTicks--;
            if (disableButtonsInTicks == 0)
            {
                LogStr("[HOOK] Disabling all our buttons after countdown reached zero");
                setActiveAllOurButtons(false);
                disableButtonsInTicks = -1;
            }
        }
        if (originalDrawer != null)
        {
            if (ticksStartTime == default)
            {
                ticksStartTime = DateTime.Now;
            }
            // else if (ticksStartTime.AddSeconds(15) <= DateTime.Now)
            // {
            //     LogStr($"[HOOK] In 15 seconds there were number of ticks: {framesSinceInitialCatStatsDrawer} so the ~fps: {framesSinceInitialCatStatsDrawer / 15.0}");
            //     ticksStartTime = DateTime.Now;
            //     framesSinceInitialCatStatsDrawer = 0;
            // }
            framesSinceInitialCatStatsDrawer++;
            // if (framesSinceInitialCatStatsDrawer % 1000 != 0)
            // {
            //     return result;
            // }
            // if (framesSinceInitialCatStatsDrawer >= 100000)
            // {
            //     framesSinceInitialCatStatsDrawer = 0;
            // }
        }

        // 00007FF646AD4C40
        // [[[rcx+0x18]+0x28]] or [[[rcx+0x18]+0x28]+10]



        if (originalDrawer != null && totalCatsCount == -1)
        {
            CountExecution(nameof(GameTickHook));

            var pointer = Marshal.ReadIntPtr((nint)originalDrawer + 0x20);
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
            totalCatsCount = count;
            // totalCatsCount = 1; // just for debugging!

            // LogStr($"cat count (tick): {count}");
            CreateRows();
            // LogStr($"before read! {originalDrawer:X}");
        }

        if (cachedVisibleCats.Length > 0)
        {
            handlePositionOfOurPanel();
        }


        if (ourPanelIsOpen && framesSinceInitialCatStatsDrawer % 20 == 0 && !ourAnimationInProgress)
        {
            if (lastNumKeyPressed != -1)
            {
                setHoveredCatRoom(lastNumKeyPressed - 1);
            }

            // LogStr($"ourPanelIsOpen: {ourPanelIsOpen}");
            var index = GetRendererIndexWhereMouseIsHitting();
            // LogStr($"index under mouse: {index}");
            if (currentlyHoveredIndex != index)
            {
                // Handle logic for when the hovered drawer changes
                // LogStr($"Hovered drawer changed from {currentlyHoveredIndex} to {index}");
                if (currentlyHoveredIndex != -1)
                {
                    LogStr($"Deactivate buttons for previously hovered index next CatStatsUpdate (to give time for the frame change)");
                    previouslyHoveredIndex = currentlyHoveredIndex;
                }
                if (index != -1)
                {
                    LogStr($"Activating buttons for newly hovered index: {index}");
                    setActiveAllOurButtons(true, (nint)rowRenderers[index]);
                    // log prev state
                    LogStr($"Previous transformed mouse: ({prevTransformedMouse[0]}, {prevTransformedMouse[1]})");
                    LogStr($"Previous bounds: (left: {prevBounds[0]}, top: {prevBounds[1]}, right: {prevBounds[2]}, bottom: {prevBounds[3]})");
                    LogStr($"Current mouse position: ({prevMouse[0]}, {prevMouse[1]})");
                }
            }
            if (index != -1)
            {
                // LogStr($"catStatDrawer: {rowDrawers[index]}");
                var catStatDrawer = rowDrawers[index];
                currentlyHoveredIndex = index;
            }
            else
            {
                currentlyHoveredIndex = -1;
            }

            if (panelsWithData.Count != totalCatsCount)
            {
                foreach (var ren in activeRowsRenderers)
                {
                    if (!panelsWithData.Contains(ren))
                    {
                        var movieclip = (MovieClip*)Read(ren + 0x80);
                        catStats[ren]["dex"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("dexfancy.total")));
                        catStats[ren]["spd"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("spdfancy.total")));
                        catStats[ren]["cha"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("chafancy.total")));
                        catStats[ren]["str"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("strfancy.total")));
                        catStats[ren]["int"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("intfancy.total")));
                        catStats[ren]["lck"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("lckfancy.total")));
                        catStats[ren]["con"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("confancy.total")));
                        averages[ren] = (double)Math.Round(catStats[ren].Where(kv => kv.Key != "hou").Average(kv => kv.Value), 1);
                        var mutations = _getChild((nint)movieclip, GameString.Create("mutations"));

                        catStats[ren]["muc"] = ReadNumberOrZero(_getChild(mutations, GameString.Create("mutationcount")));
                        catStats[ren]["bdc"] = ReadNumberOrZero(_getChild(mutations, GameString.Create("birthdefectcount")));
                        catStats[ren]["age"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("age")));
                        catStats[ren]["lev"] = ReadNumberOrZero(_getChildByPath((nint)movieclip, GameString.Create("level")));


                        var avgTextbox = _getChild((nint)movieclip, GameString.Create("average"));
                        _setText(avgTextbox, GameString.CreateUTF16GameString($"{averages[ren]}"));

                        LogStr($"[HOOK] GameTickHook: dex={catStats[ren]["dex"]} spd={catStats[ren]["spd"]} cha={catStats[ren]["cha"]} str={catStats[ren]["str"]} int={catStats[ren]["int"]} lck={catStats[ren]["lck"]} con={catStats[ren]["con"]} muc={catStats[ren]["muc"]} bdc={catStats[ren]["bdc"]} for ren 0x{ren:X}");
                        panelsWithData.Add(ren);
                    }
                }
            }

            if (lvTranslated > 0 && ageTranslated > 0)
            {
                var rootMovieclip = (MovieClip*)Read<nint>((nint)headersRenderer + 0x80);
                LogStr($"[HOOK] GameTickHook: rootMovieclip=0x{(nint)rootMovieclip:X}");
                var textbox = _getChild((nint)rootMovieclip, GameString.Create("level_header"));
                _setText(textbox, lvTranslated);
                textbox = _getChild((nint)rootMovieclip, GameString.Create("age_header"));
                _setText(textbox, ageTranslated);
                lvTranslated = -1;
                ageTranslated = -1;
            }

        }
        EndGlobalPerfLog("GameTick");
        return result;
    }

    static Dictionary<nint, int> sortedPositions = new Dictionary<nint, int>();
    static DateTime? pendinUpdateTransformInTick = null;
    static DateTime? pendinUpdateRenderersInsideScreen = null;
    static bool ourAnimationInProgress = false;

    static unsafe void handlePositionOfOurPanel()
    {
        if (yScrollAni == null) return;
        if (xMoveAni == null) return;
        var newYOffset = yScrollAni.Tick();
        var newXOffset = xMoveAni.Tick();
        var yChanged = newYOffset != yOffset || yScrollAni.justFinished;
        var xChanged = newXOffset != xOffset || xMoveAni.justFinished;
        if (!yChanged && !xChanged && positionDirty == 0)
        {
            return;
        }
        if (positionDirty > 0)
        {
            positionDirty--;
            LogStr($"Decrementing positionDirty, new value is {positionDirty}");
        }
        var xIsFinished = xMoveAni == null || xMoveAni.finished;
        var yIsFinished = yScrollAni == null || yScrollAni.finished;
        CountExecution(nameof(handlePositionOfOurPanel));
        // var ourAnimationJustEnded = false;
        ourAnimationInProgress = true;

        // if (xJustFinished || yJustFinished)
        // {
        //     LogStr($"Our animation just ended: xOffset={xOffset} yOffset={yOffset} newXOffset={newXOffset} newYOffset={newYOffset}");
        //     ourAnimationJustEnded = true;
        // }

        if (xIsFinished && yIsFinished)
        {
            LogStr($"Both x and y animations just finished: xOffset={xOffset} yOffset={yOffset} newXOffset={newXOffset} newYOffset={newYOffset}");
            ourAnimationInProgress = false;
        }
        LogStr($"newXOffset {newXOffset} newYOffset {newYOffset}, old one xOffset={xOffset} yOffset={yOffset}, xMoveAni=" + (xMoveAni == null ? "null" : xMoveAni.Tick()) + " yOffsetTarget=" + (yScrollAni == null ? "null" : yScrollAni.targetValue)
         + " xMoveTvalue=" + (xMoveAni == null ? "null" : xMoveAni.t));
        yOffset = newYOffset;
        xOffset = newXOffset;
        LogStr($"xOffset {xOffset} yOffset {yOffset}");
        double yPos = -1.0 + yOffset;
        double xPos = xOffset;
        double headerXpos = cachedVisibleCats.Length > 0 ? xPos - 10.0 : -300.0;
        LogStr($"headerXpos {headerXpos}");

        if (headersRenderer != null)
        {
            headersRenderer->Transform->X = headerXpos;
            LogStr($"headerTransform->X set to {headerXpos} headerTransform=0x{(nint)headersRenderer->Transform:X}");
            if (xMoveAni != null && xMoveAni.justFinished)
            {
                LogStr($"Tracking movieclip child parent offset for originalHousePanel=0x{(nint)originalHousePanel:X} headerTransform=0x{(nint)headersRenderer->Transform:X}");
                _trackMovieclipChildParentOffset((nint)originalHousePanel, (nint)headersRenderer->Transform, 1);
                LogStr($"Tracked movieclip child parent offset for originalHousePanel=0x{(nint)originalHousePanel:X} headerTransform=0x{(nint)headersRenderer->Transform:X}");
                LogStr("Setting positionDirty to 20 at handlePositionOfOurPanel (headerTransform update)");
                positionDirty = 20;
            }
        }
        if (footerRenderer != null)
        {
            var footerTransform = footerRenderer->Transform;
            footerTransform->X = headerXpos;
            footerTransform->Y = -1.0 + yOffset - (1.8 * activeRowsRenderers.Count);
            if (!catPanelAnimationInProgress && (xOffset == 10 || xOffset == -92))
            {
                LogStr($"Tracking movieclip child parent offset for originalHousePanel=0x{(nint)originalHousePanel:X} footerTransform=0x{(nint)footerTransform:X}");
                _trackMovieclipChildParentOffset((nint)originalHousePanel, (nint)footerTransform, 1);
            }
        }
        if (positionDirty > 0 || yChanged || (xMoveAni != null && xMoveAni.justFinished))
        {
            LogStr($"Updating renderers inside screen area with yOffset={yOffset} xOffset={xOffset}");
            pendinUpdateTransformInTick = DateTime.Now.AddMilliseconds(10);
        }
    }

    static unsafe void updateRowTransforms(bool forceTrackUpdate = false)
    {
        double xPos = xOffset;
        double headerXpos = cachedVisibleCats.Length > 0 ? xPos - 10.0 : -300.0;
        if (headersRenderer != null)
        {
            headersRenderer->Transform->X = headerXpos;

        }
        if (footerRenderer != null)
        {
            footerRenderer->Transform->X = headerXpos;
        }
        // LogStr($"Updating row transforms with xPos={xPos} yOffset={yOffset}");
        for (var i = 0; i < totalCatsCount && i < rowRenderers.Length && i < rowTransforms.Length; i++)
        {
            var index = sortedPositions.TryGetValue((nint)rowRenderers[i], out var sortedIndex)
                ? sortedIndex
                : -1;
            var transform = (Transform*)rowTransforms[i];
            if (transform == null)
            {
                // LogStr($"Transform is null for rowRenderers[{i}] = 0x{(nint)rowRenderers[i]:X}");
                continue;
            }
            if (index != -1)
            {
                transform->X = xPos;
                double yPos = -1.0 + yOffset - (1.8 * index);
                transform->Y = yPos;
            }
            else
            {
                transform->X = -300.0;
            }
        }
        // LogStr($"Finished updating row transforms");

        for (var i = 0; i < totalCatsCount && i < rowDrawers.Length; i++)
        {
            var drawer = rowDrawers[i];
            // LogStr($"Processing rowDrawer[{i}] = 0x{(nint)drawer:X}");
            if (drawer == 0)
                continue;
            var renderer = (Renderer*)Read(drawer + 0x40);
            if (Read<nint>((nint)renderer + 0x40) == 0)
            {
                // LogStr($"Renderer is null for rowDrawers[{i}] = 0x{(nint)drawer:X}");
                continue;
            }
            var transform = renderer->Transform;
            if (!catPanelAnimationInProgress || positionDirty > 0 || forceTrackUpdate)
            {
                _trackMovieclipChildParentOffset((nint)originalHousePanel, (nint)transform, 1);
            }
        }
        // LogStr($"Finished updating row drawers");

    }


    static nint[] rowDrawers = Array.Empty<nint>();
    static nint[] rowTransforms = Array.Empty<nint>();
    static unsafe nint[] rowRenderers = Array.Empty<nint>();

    unsafe static Renderer* headersRenderer = null;
    unsafe static Renderer* footerRenderer = null;

    static bool insideOurCatInstantiation = false;
    static bool showingChimplantsPromo = false;

    // static nint noop = 0;

    static unsafe void CreateRows()
    {
        LogStr($"Creating rows");
        CountExecution(nameof(CreateRows));

        DateTime currentDateTime = DateTime.Now;

        IntPtr headers = Marshal.StringToHGlobalAnsi("RowHeaders");
        // var _strBtn = _findMovieClipTrampoline(CreateUTF16GameString("x"));
        LogStr($"header string: RowHeaders");
        var headersEntity = _createEntity(scenePtr);
        LogStr($"headersEntity 0x{headersEntity:X}");

        try
        {
            LogStr($"Creating headersRendere with arguments: scenePtr=0x{scenePtr:X}, headersEntity=0x{headersEntity:X}, headers=0x{headers:X}");
            headersRenderer = createUiRenderer(scenePtr, headersEntity, headers);
        }
        finally
        {
            Marshal.FreeHGlobal(headers);
        }
        LogStr($"headersRenderer created: 0x{(nint)headersRenderer:X}");

        if (currentDateTime > dateInstalled.AddDays(3))
        {
            LogStr($"Creating footerRenderer because more than 3 days have passed since installation");
            showingChimplantsPromo = currentDateTime > dateInstalled.AddDays(60) || currentDateTime > new DateTime(2027, 7, 1);
            IntPtr footer = Marshal.StringToHGlobalAnsi(showingChimplantsPromo ? "Chimplants" : "BuyMeACoffee");
            var footerEntity = _createEntity(scenePtr);
            try
            {
                footerRenderer = createUiRenderer(
                    scenePtr,
                    footerEntity,
                    footer);
            }
            finally
            {
                Marshal.FreeHGlobal(footer);
            }

            footerRenderer->Flags = 0x0000002400000101;
            var footerTransform = footerRenderer->Transform;
            // Write(footerTransform + 0x80, -300.0);
            footerTransform->X = -300.0;
        }

        // Write((nint)headersRenderer + 0x50, 0x0000002400000101);
        headersRenderer->Flags = 0x0000002400000101;
        // Write(headerTransform + 0x80, -300.0);
        headersRenderer->Transform->X = -300.0;
        LogStr($"headersRenderer 0x{(nint)headersRenderer:X}");
        // totalCatsCount = 1;// for debugging only;
        rowRenderers = new nint[totalCatsCount];
        rowTransforms = new nint[totalCatsCount];
        rowDrawers = new nint[totalCatsCount];
        buttonsInsideOurDrawers = new List<List<nint>>(totalCatsCount);
        for (int i = 0; i < totalCatsCount; i++)
        {
            buttonsInsideOurDrawers.Add(new List<nint>());
            LogStr($"Creating row {i + 1}/{totalCatsCount}");
            nint rowEntity = _createEntity(scenePtr);

            int componentCount = *(int*)(rowEntity + 36);
            nint componentArray = *(nint*)(rowEntity + 40);


            var rendererFound = _getRenderer(rowEntity);
            IntPtr name = Marshal.StringToHGlobalAnsi("RowCatStatus");

            Renderer* rowRenderer = createUiRenderer(
                scenePtr,
                rowEntity,
                name);

            Marshal.FreeHGlobal(name);
            LogStr($"Creating CatStatsDrawer {i + 1}/{totalCatsCount}: Renderer {(nint)rowRenderer:X} {rowEntity:X} {scenePtr:X}");
            rendererFound = _getRenderer(rowEntity);
            rowRenderers[i] = (nint)rowRenderer;
            if (i < 13)
            {
                activeRowsRenderers.Add((nint)rowRenderer);
            }
            var transform = Marshal.ReadIntPtr((nint)rowRenderer + 0x40);
            rowTransforms[i] = transform;
            insideOurCatInstantiation = true;
            // sleep for 0.1 seconds
            var rowDrawer = _createCatStatsDrawer(
                scenePtr,
                rowEntity);
            insideOurCatInstantiation = false;

            // Write(rowDrawer + 0x38, originalHousePanel);

            // var iconsPanel = Marshal.ReadIntPtr(rowDrawer + 0x68);
            // var panelB = Marshal.ReadIntPtr(rowDrawer + 0x68);
            LogStr($"Created new CallStatsDrawer: Renderer {(nint)rowRenderer:X} Drawer:{rowDrawer:X} Entity: {rowEntity:X} {scenePtr:X} transform: {transform:X}");
            // rowIconPanels.Add(iconsPanel);
            rowDrawers[i] = rowDrawer;

            Dictionary<string, int> dict = new();
            catStats[(nint)rowRenderer] = dict;

            rowRenderer->Visible = 0;
            rowRenderer->enabled = 0;
            LogStr($"Initialized catStats dictionary for renderer {(nint)rowRenderer:X}");

            // var menuPanel = Marshal.ReadIntPtr(rowDrawer + 0x60);
            // // debug only ahead:
            // nint callback = Marshal.AllocHGlobal(0x40);
            // NativeMemory.Clear((void*)callback, 0x40);

            // testbutton = registerButton(
            //     menuPanel,
            //     GameString.Create("xxx"),
            //     callbackStorage,
            //     callback
            // );
            // Write(testbutton + 0x50, 0x000003EA);

        }
        forcedCatStatsUpdatePending = rowRenderers.Length;

        // Write((nint)originalHousePanel + 0x118, originalDrawer);
        originalHousePanel->CatStatsDrawer = originalDrawer;
        setActiveAllOurButtons(false);

    }
    // static nint testbutton = 0;

    static nint scenePtr = 0;
    [UnmanagedCallersOnly]
    static unsafe Renderer* CreateUiRendererHook(nint a1, nint entity, nint namePtr)
    {
        CountExecution(nameof(CreateUiRendererHook));
        var name = TryReadCString(namePtr);
        if (name == "HouseCatStatus")
        {
            LogStr($"[HOOK] CreateUiRendererHook: a1=0x{a1:X}, entity=0x{entity:X}, name=\"{name}\"");
            scenePtr = a1;
        }

        return createUiRenderer(a1, entity, namePtr);
    }

    static bool ourPanelIsOpen = false;
    static bool catPanelAnimationInProgress = false;
    private const int PanelStateClosed = 36;
    private const int PanelStateOpen = 37;
    static int waitingForPanelStatus = 0;
    static nint catMenuPanel = 0;

    [UnmanagedCallersOnly]
    static unsafe nint UpdatePanelLayoutHook(nint a1)
    {
        // return updatePanelLayout(a1);
        if (originalDrawer == null || (catMenuPanel != 0 && catMenuPanel != a1))
        {
            return updatePanelLayout(a1);
        }
        CountExecution(nameof(UpdatePanelLayoutHook));


        var renderer = Marshal.ReadIntPtr(a1 + 0x58);
        if (catMenuPanel == 0)
        {
            // read as dword:
            var rendererName = TryReadStdString(renderer + 0xA8, false);
            if (rendererName != "CatMenu")
            {
                return updatePanelLayout(a1);
            }
            LogStr($"[HOOK] UpdatePanelLayoutHook: CatMenu panel detected: a1=0x{a1:X}, renderer=0x{renderer:X}, rendererName=\"{rendererName}\"");
            catMenuPanel = a1;
        }

        var result = updatePanelLayout(a1);

        var rendererState = Marshal.ReadInt32(renderer + 0x54);
        if (rendererState == waitingForPanelStatus)
        {
            if (waitingForPanelStatus == PanelStateOpen)
            {
                waitingForPanelStatus = 0;
                catPanelAnimationInProgress = false;
                positionDirty = 10;
                LogStr("Setting positionDirty=10 at UpdatePanelLayoutHook");
                setActiveAllOurButtons(true);
                LogStr($"[HOOK] 1- Panel is open and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
            else if (waitingForPanelStatus == PanelStateClosed)
            {
                waitingForPanelStatus = 0;
                catPanelAnimationInProgress = false;
                for (int i = 0; i < rowDrawers.Length; i++)
                {
                    var drawer = rowDrawers[i];
                    // Write(drawer + 0x78, (nint)0); // setting HouseCat reference to zero
                }
                foreach (Renderer* rendererToHide in rowRenderers)
                {
                    rendererToHide->enabled = 0;
                    rendererToHide->Visible = 0;
                }

                // foreach (var btn in theirButtons)
                // {
                //     // Enable their buttons
                //     if (btn == openCloseBtn)
                //     {
                //         continue;
                //     }
                //     Write(btn + 0x10, (byte)1); 
                // }
                positionDirty = 10;
                LogStr("Setting positionDirty=10 at UpdatePanelLayoutHook (2)");
                LogStr($"[HOOK] 2- Pane is closed and animation finished: xOffset {xOffset} xOffsetTarget {xOffsetTarget}");
            }
        }

        if (ourPanelIsOpen || catPanelAnimationInProgress)
        {
            // LogStr($"[HOOK] Updating row transforms: ourPanelIsOpen {ourPanelIsOpen} catPanelAnimationInProgress {catPanelAnimationInProgress} pendinUpdateTransformInTick {pendinUpdateTransformInTick}");
            updateRowTransforms();
        }
        return result;
    }

    static unsafe void ClearState()
    {
        LogStr($"ClearState called");
        // LogStr($"[HOOK] RemoveMovieClip called on mod container 0x{a1:X}");
        rooms.Clear();
        buttonsInsideOurDrawers.Clear();
        alreadyInitializedRenderers.Clear();
        catPartsInsideOurDrawers = Array.Empty<nint>();
        sortedPositions = new Dictionary<nint, int>();
        scenePtr = 0;
        catMenuPanel = 0;
        originalHousePanel = null;
        originalDrawer = null;
        hoveredAreasCache = Array.Empty<nint>();
        rowTransforms = Array.Empty<nint>();
        rowDrawers = Array.Empty<nint>();
        rowRenderers = Array.Empty<nint>();
        lvTranslated = 0;
        ageTranslated = 0;
        totalCatsCount = -1;
        headersRenderer = null;
        footerRenderer = null;
        ourPanelIsOpen = false;
        cachedVisibleCats = Array.Empty<nint>();
        sortedCats = Array.Empty<nint>();
        activeRowsRenderers.Clear();
        initializedButtons = false;
        yOffset = 0;
        yOffsetTarget = 0;
        averages.Clear();
        xOffset = -300;
        xOffsetTarget = -300;
        positionDirty = 0;
        catStats.Clear();
        yScrollAni = null;
        xMoveAni = null;
    }


    static public void LogStr(string message)
    {
        if (!debugLogging)
            return;
        MewjectorApi.Log(message);
    }
    

};
