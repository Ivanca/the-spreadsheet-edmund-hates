// catstableBridge.cpp

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

    return g_MJ_InstallHook != nullptr &&
           g_MJ_GetGameBase != nullptr;
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
    lstrcatA(dllPath, "catstable.dll");

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

    void* trampoline = nullptr;

    constexpr UINT_PTR GAME_LOADER_RVA = 0x9ABE80;

    int result = g_MJ_InstallHook(
        GAME_LOADER_RVA,
        0,
        reinterpret_cast<void*>(&GameLoaderHook),
        &trampoline,
        0,
        "CatsTableBridge"
    );

    if (!result || !trampoline)
        return false;

    g_GameLoaderTrampoline =
        reinterpret_cast<GameLoaderFn>(trampoline);

    return true;
}

// -----------------------------------------------------------------------------
// Proxy API exposed to catstable.dll
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
        // Do not LoadLibrary(catstable.dll) here.
        // Do not create a thread here.
        //
        // We only install the Mewjector hook. The actual AOT load happens
        // later when the game executes RVA 0x9764B0.
        g_hInstance = hInstance;
        InstallBootstrapHook();
    }

    return TRUE;
}