// the_spreadsheet_edmund_hatesBridge.cpp

#include <windows.h>
#include <cstdint>

using UINT_PTR = uintptr_t;

// -----------------------------------------------------------------------------
// Mewjector API
// -----------------------------------------------------------------------------

using MJ_InstallHook_t = int (__cdecl*)(
    UINT_PTR rva,
    int stolenBytes,
    void* hookFn,
    void** outTrampoline,
    int priority,
    const char* owner
);

using MJ_GetGameBase_t = UINT_PTR (__cdecl*)();

// -----------------------------------------------------------------------------
// Game function at RVA 0x9764B0
//
// You said this function takes two parameters and returns void.
// We don't yet know their actual types, so use the x64 ABI representation.
// -----------------------------------------------------------------------------

using GameLoaderFn = UINT_PTR (*)(UINT_PTR arg1, UINT_PTR arg2, UINT_PTR arg3);

// -----------------------------------------------------------------------------
// Globals
// -----------------------------------------------------------------------------
static HINSTANCE g_hInstance = nullptr;
static MJ_InstallHook_t g_MJ_InstallHook = nullptr;
static MJ_GetGameBase_t g_MJ_GetGameBase = nullptr;

static GameLoaderFn g_GameLoaderTrampoline = nullptr;

// 0 = not loaded
// 1 = currently loading
// 2 = successfully loaded
static volatile LONG g_AotState = 0;

static HMODULE g_AotModule = nullptr;

typedef int(__cdecl* MJ_InstallHook_t)(
    UINT_PTR rva,
    int stolenBytes,
    void* hookFn,
    void** outTrampoline,
    int priority,
    const char* owner
);

typedef int(__cdecl* MJ_InstallShortHook_t)(
    UINT_PTR rva,
    int stolenBytes,
    void* hookFn,
    void** outTrampoline,
    int priority,
    const char* owner
);

static MJ_InstallShortHook_t g_MJ_InstallShortHook = nullptr;

// -----------------------------------------------------------------------------
// Resolve Mewjector
// -----------------------------------------------------------------------------

static bool ResolveMewjector()
{
    // Mewjector is loaded as version.dll in the current setup.
    HMODULE hMewjector = GetModuleHandleA("version.dll");

    if (!hMewjector)
        return false;

    g_MJ_InstallHook =
        reinterpret_cast<MJ_InstallHook_t>(
            GetProcAddress(hMewjector, "MJ_InstallHook"));

    g_MJ_GetGameBase =
        reinterpret_cast<MJ_GetGameBase_t>(
            GetProcAddress(hMewjector, "MJ_GetGameBase"));

    g_MJ_InstallShortHook = reinterpret_cast<MJ_InstallShortHook_t>(
        GetProcAddress(
            hMewjector,
            "MJ_InstallShortHook"
        )
    );


    return g_MJ_InstallHook != nullptr &&
           g_MJ_GetGameBase != nullptr &&
           g_MJ_InstallShortHook != nullptr;
}


// -----------------------------------------------------------------------------
// AOT loader
// -----------------------------------------------------------------------------
static bool LoadAotDll()
{
    char bridgePath[MAX_PATH];

    DWORD length = GetModuleFileNameA(
        g_hInstance,
        bridgePath,
        MAX_PATH
    );

    if (length == 0 || length >= MAX_PATH)
        return false;

    // Remove the bridge DLL filename.
    char* slash = strrchr(bridgePath, '\\');

    if (!slash)
        return false;

    *(slash + 1) = '\0';

    char dllPath[MAX_PATH];

    lstrcpyA(dllPath, bridgePath);
    lstrcatA(dllPath, "the_spreadsheet_edmund_hates.dll");

    HMODULE hDll = LoadLibraryA(dllPath);

    if (!hDll)
        return false;

    g_AotModule = hDll;

    using MjInitFn = void (*)();

    auto MjInit =
        reinterpret_cast<MjInitFn>(
            GetProcAddress(hDll, "MjInit"));

    if (MjInit)
        MjInit();

    return true;
}

// -----------------------------------------------------------------------------
// Bootstrap hook
// -----------------------------------------------------------------------------

static UINT_PTR GameLoaderHook(
    UINT_PTR arg1,
    UINT_PTR arg2,
    UINT_PTR arg3)
{
    if (InterlockedCompareExchange(
            &g_AotState,
            1,
            0) == 0)
    {
        if (LoadAotDll())
            InterlockedExchange(&g_AotState, 2);
        else
            InterlockedExchange(&g_AotState, 0);
    }

    if (g_GameLoaderTrampoline)
        return g_GameLoaderTrampoline(arg1, arg2, arg3);

    return 0;
}


// -----------------------------------------------------------------------------
// Install bootstrap hook
// -----------------------------------------------------------------------------
static bool InstallBootstrapHook()
{
    if (!ResolveMewjector())
        return false;

    constexpr UINT_PTR GAME_LOADER_RVA = 0x9b9970;

    // Exact function signature expected at the hook location.
    static const uint8_t EXPECTED_BYTES[] = {
        0x48, 0x8B, 0xC4, 0x48, 0x89, 0x58, 0x10, 0x48,
        0x89, 0x48, 0x08, 0x55, 0x56, 0x57, 0x41, 0x54,
        0x41, 0x55, 0x41, 0x56, 0x41, 0x57, 0x48, 0x8D,
        0x6C, 0x24, 0xB0, 0x48, 0x81, 0xEC, 0x50, 0x01,
        0x00, 0x00, 0x0F, 0x29, 0x70, 0xB8, 0x0F, 0x29,
        0x78, 0xA8, 0x44, 0x0F, 0x29, 0x40, 0x98, 0x49,
        0x8B, 0xD8, 0x8B, 0xFA, 0x45, 0x33, 0xED, 0x48,
        0x8B, 0x35, 0x82, 0xB0, 0xA0, 0x00, 0x48, 0x8B
    };

    constexpr size_t EXPECTED_SIZE = sizeof(EXPECTED_BYTES);

    UINT_PTR gameBase = g_MJ_GetGameBase();

    if (!gameBase)
        return false;

    const uint8_t* hookAddress =
        reinterpret_cast<const uint8_t*>(gameBase + GAME_LOADER_RVA);

    // -------------------------------------------------------------------------
    // Safety check
    // -------------------------------------------------------------------------

    for (size_t i = 0; i < EXPECTED_SIZE; ++i)
    {
        if (hookAddress[i] != EXPECTED_BYTES[i])
        {
            // We found a different game version.
            //
            // Do NOT install the hook and, consequently, do NOT load
            // CatsTable.dll.

            // If you have CLog available in the bridge, this is preferable:
            //
            // CLog(
            //     "[MewjectorBridge] SAFETY CHECK FAILED at RVA 0x%llX: "
            //     "offset +0x%zX expected %02X, found %02X",
            //     (unsigned long long)GAME_LOADER_RVA,
            //     i,
            //     EXPECTED_BYTES[i],
            //     hookAddress[i]
            // );

            return false;
        }
    }

    // -------------------------------------------------------------------------
    // Signature matches -- safe to install hook.
    // -------------------------------------------------------------------------

    void* trampoline = nullptr;

    int result = g_MJ_InstallHook(
        GAME_LOADER_RVA,
        0,
        reinterpret_cast<void*>(&GameLoaderHook),
        &trampoline,
        0,
        "MewjectorBridge"
    );

    if (!result || !trampoline)
        return false;

    g_GameLoaderTrampoline =
        reinterpret_cast<GameLoaderFn>(trampoline);

    return true;
}

// -----------------------------------------------------------------------------
// Proxy API exposed to the_spreadsheet_edmund_hates.dll
// -----------------------------------------------------------------------------

extern "C"
__declspec(dllexport)
int __cdecl MJ_InstallHook(
    UINT_PTR rva,
    int stolenBytes,
    void* hookFn,
    void** outTrampoline,
    int priority,
    const char* owner)
{
    if (!g_MJ_InstallHook)
    {
        if (!ResolveMewjector())
            return 0;
    }

    return g_MJ_InstallHook(
        rva,
        stolenBytes,
        hookFn,
        outTrampoline,
        priority,
        owner
    );
}


extern "C"
__declspec(dllexport)
UINT_PTR __cdecl MJ_GetGameBase()
{
    if (!g_MJ_GetGameBase)
    {
        if (!ResolveMewjector())
            return 0;
    }

    return g_MJ_GetGameBase();
}


// -----------------------------------------------------------------------------
// DLL entry point
// -----------------------------------------------------------------------------

BOOL WINAPI DllMain(
    HINSTANCE hInstance,
    DWORD reason,
    LPVOID reserved)
{
    if (reason == DLL_PROCESS_ATTACH)
    {
        // IMPORTANT:
        //
        // Do not LoadLibrary(the_spreadsheet_edmund_hates.dll) here.
        // Do not create a thread here.
        //
        // We only install the Mewjector hook. The actual AOT load happens
        // later when the game executes RVA 0x9764B0.
        g_hInstance = hInstance;
        InstallBootstrapHook();
    }

    return TRUE;
}