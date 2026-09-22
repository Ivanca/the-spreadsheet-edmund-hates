
import idaapi
import ida_bytes
import ida_funcs
import ida_name
import ida_typeinf
import ida_hexrays
import tkinter as tk

# ============================================================
# CONFIGURATION
# ============================================================

# These are VTABLE SLOT RVAs.
#
# Example:
#
#     0xF01208 + 0x50
#
# means:
#
#     GameBase + 0xF01208 + 0x50
#                       |
#                       v
#                  function pointer
#
# The resulting function RVA is what will be passed to
# MewjectorApi.InstallHook().
#

rvas = {
    "SimpleMusicPlayer": 0xF01208 + 0x50,
    "GlobalProgressionData": 0xF158B0 + 0x50,
    "MewDirector": 0xEE7A50 + 0x50,
    "MewsicController": 0xED63C0 + 0x50,
    "SpawnDatabase": 0xF0E1A0 + 0x50,
    "AudioListener": 0xED6770 + 0x58,
    "GameBase": 0x1114990 + 0x50,
    "HouseDrawerUI": 0xEFD120 + 0x48,
    "Ragdoll": 0xEDAD50 + 0x48,
    "FurnitureBuildingUI": 0xf103b0,
    "FurnitureClickHandler": 0xEE6940 + 0x50,
    "ButchBox": 0xEFCCC0 + 0x50,
    "CatStatsDrawer": 0xEE6770 + 0x50,
    "FurnitureGrid": 0xEFCDE8 + 0x50,
    "FurniturePiece": 0xEE6858 + 0x50,
    "House": 0xF10158 + 0x50,
    "HouseCat": 0xEFD038 + 0x50,
    "HouseCatClickManager": 0xF10448 + 0x50,
    "HouseCatPhysics": 0xEFDFB8 + 0x50,
    "HouseDrawerUI": 0xEFD120 + 0x50,
    "HousePipe": 0xEFC9C0 + 0x50,
    "HouseTutorialDriver": 0x1125678 + 0x50,
    "InventoryTrashDrawers": 0xEE6688 + 0x50,
    "NPCMapDrawer": 0xEE65A0 + 0x50,
    "SingingCat": 0x1114470 + 0x50,
    "Button": 0xEE2048 + 0x50,
    "AbilityTooltip": 0xED3828 + 0x50,
    "AABBTreeBroadphase": 0xEE58D0 + 0x58,
    "FurnitureBuildingUI": 0xF10360 + 0x58,
    "AudioSource": 0xED12A0 + 0x58,
    "AnimationAudioEvents": 0xEDF268 + 0x58,
    "ButchBox": 0xEFCCC0 + 0x58,
    "CatRagdoll": 0xEFE0D8 + 0x58,
    "CombatCatVoice_Emotions": 0xEDE4F0 + 0x58,
    "FurnitureGridIcons": 0xF0A5A0 + 0x58,
    "FurnitureMenuButton": 0xEE64B8 + 0x58,
    "House": 0xF10158 + 0x58,
    "HouseCat": 0xEFD038 + 0x58,
    "HouseDrawerPanel": 0xEDBD80 + 0x58,
    "HousePipe": 0xEFC9C0 + 0x58,
    "HouseTest": 0xEFD408 + 0x58,
    "TrashScreenItemBox2": 0xEFF190 + 0x58,
    "Ragdoll": 0xEDAD50 + 0x58,
    "ButtonManager": 0xEFE6F0 + 0x58,
}

# ============================================================
# HELPERS
# ============================================================

def camel_to_variable(name):
    if not name:
        return name

    return name[0].lower() + name[1:]


def resolve_vtable_slot(vtable_rva):
    """
    vtable_rva is the RVA of the exact vtable slot.

    Example:

        vtable_rva = 0xF10360 + 0x50
                   = 0xF103B0

        GameBase + 0xF103B0
                   |
                   v
        0x7FF6B45C03B0
                   |
                   v
        dq offset sub_7FF6B3855AB0
                   |
                   v
        0x7FF6B3855AB0

    Returns:
        (function_ea, slot_ea)
    """

    image_base = idaapi.get_imagebase()

    # The dictionary value is ALREADY the exact slot RVA.
    slot_rva = int(vtable_rva)

    # Convert RVA -> absolute address.
    slot_ea = image_base + slot_rva

    if not ida_bytes.is_loaded(slot_ea):
        raise RuntimeError(
            f"VTable slot 0x{slot_ea:X} "
            f"(RVA 0x{slot_rva:X}) is not loaded"
        )

    # Read EXACTLY the qword at the requested slot.
    function_ea = ida_bytes.get_qword(slot_ea)

    if function_ea == 0:
        raise RuntimeError(
            f"VTable slot 0x{slot_ea:X} contains NULL"
        )

    print(
        f"    Slot RVA     : 0x{slot_rva:X}"
    )

    print(
        f"    Slot address  : 0x{slot_ea:X}"
    )

    print(
        f"    Slot contents : 0x{function_ea:X}"
    )

    return function_ea, slot_ea


def get_name(ea):
    name = ida_name.get_name(ea)

    if name:
        return name

    return f"sub_{ea:X}"


# ============================================================
# ARGUMENT DETECTION
# ============================================================

def get_argument_count_from_tinfo(ea):
    """
    Try IDA's stored type information first.

    Returns:
        int or None
    """

    tif = ida_typeinf.tinfo_t()

    try:
        if not idaapi.get_tinfo(tif, ea):
            return None

        if not tif.is_func():
            return None

        func_data = ida_typeinf.func_type_data_t()

        if not tif.get_func_details(func_data):
            return None

        return len(func_data)

    except Exception:
        return None


def get_argument_count_from_hexrays(ea):
    """
    Ask Hex-Rays for the function arguments.

    This works even when the function doesn't have a
    fully populated stored tinfo/prototype.
    """

    if not ida_hexrays.init_hexrays_plugin():
        return None, None

    try:
        cfunc = ida_hexrays.decompile(ea)

        if cfunc is None:
            return None, None

        arguments = []

        for lvar in cfunc.arguments:
            try:
                if lvar.is_arg_var():
                    arguments.append(lvar)
            except Exception:
                pass

        return len(arguments), cfunc

    except Exception as e:
        print(
            f"    [Hex-Rays exception] {e}"
        )

        return None, None


def get_argument_count(ea):
    """
    First use IDA type info.

    If unavailable, fall back to Hex-Rays.
    """

    count = get_argument_count_from_tinfo(ea)

    if count is not None:
        return count, "tinfo", None

    count, cfunc = get_argument_count_from_hexrays(ea)

    if count is not None:
        return count, "hexrays", cfunc

    return None, None, None


# ============================================================
# C# GENERATION
# ============================================================

def make_original_declaration(variable_name, argument_count):

    args = ["nint"] * argument_count

    args.append("void")

    signature = ", ".join(args)

    return (
        f"private static delegate* unmanaged<{signature}> "
        f"_{variable_name}Original;"
    )


def make_hook(class_name, variable_name, argument_count):

    arguments = ", ".join(
        f"nint a{i + 1}"
        for i in range(argument_count)
    )

    call_arguments = ", ".join(
        f"a{i + 1}"
        for i in range(argument_count)
    )

    if call_arguments:
        call = (
            f"    _{variable_name}Original({call_arguments});"
        )
    else:
        call = (
            f"    _{variable_name}Original();"
        )

    return f"""[UnmanagedCallersOnly]
private static void {class_name}Hook({arguments})
{{
    StartPerfLog("{class_name}");
    {call}
    EndPerfLog("{class_name}");
}}"""


def make_install(variable_name, class_name, argument_count):

    args = ["nint"] * argument_count
    args.append("void")

    signature = ", ".join(args)

    return f"""        _{variable_name}Original =
            (delegate* unmanaged<{signature}>)
                MewjectorApi.InstallHook(
                    (long){variable_name}Rva,
                    (delegate* unmanaged<{signature}>)&{class_name}Hook
                );"""

# ============================================================
# MAIN
# ============================================================

def main():

    image_base = idaapi.get_imagebase()

    print("")
    print("=" * 70)
    print("PERF HOOK GENERATOR - IDA 9.0")
    print("=" * 70)
    print(f"Image base: 0x{image_base:X}")
    print("")

    results = {}

    # --------------------------------------------------------
    # Resolve all functions
    # --------------------------------------------------------

    for class_name, vtable_rva in rvas.items():

        variable_name = camel_to_variable(class_name)

        print(f"[{class_name}]")

        try:

            function_ea, slot_ea = resolve_vtable_slot(
                vtable_rva
            )

            function_rva = function_ea - image_base

            function_name = get_name(function_ea)

            ida_func = ida_funcs.get_func(function_ea)

            if ida_func is None:

                print(
                    f"    [ERROR] 0x{function_ea:X} "
                    f"is not recognized as an IDA function"
                )

                results[class_name] = None
                continue

            print(
                f"    VTable slot : 0x{slot_ea:X}"
            )

            print(
                f"    Function    : 0x{function_ea:X}"
            )

            print(
                f"    Function RVA: 0x{function_rva:X}"
            )

            print(
                f"    IDA name    : {function_name}"
            )

            # ------------------------------------------------
            # Arguments
            # ------------------------------------------------

            argument_count, method, cfunc = get_argument_count(
                function_ea
            )

            if argument_count is None:

                print(
                    "    [ERROR] Could not determine arguments"
                )

                results[class_name] = None
                continue

            print(
                f"    Arguments   : {argument_count}"
            )

            print(
                f"    Source      : {method}"
            )

            # ------------------------------------------------
            # Show prototype from Hex-Rays if available
            # ------------------------------------------------

            if cfunc is not None:

                try:
                    print(
                        f"    Prototype   : {cfunc.type.get_type_name()}"
                    )
                except Exception:
                    pass

            results[class_name] = {
                "variable": variable_name,
                "vtable_rva": vtable_rva,
                "function_ea": function_ea,
                "function_rva": function_rva,
                "function_name": function_name,
                "arg_count": argument_count,
            }

            print("")

        except Exception as e:

            print(
                f"    [ERROR] {e}"
            )

            results[class_name] = None

            print("")


    # ========================================================
    # GENERATE OUTPUT
    # ========================================================

    output = []

    output.append(
        "// ============================================================"
    )

    output.append(
        "// GENERATED BY IDA PERF HOOK GENERATOR"
    )

    output.append(
        "// ============================================================"
    )

    output.append("")


    # --------------------------------------------------------
    # RVA constants
    # --------------------------------------------------------

    output.append("// Function RVAs")
    output.append("")

    for class_name, result in results.items():

        if result is None:
            output.append(
                f"// ERROR: {class_name}"
            )
            continue

        variable = result["variable"]
        function_rva = result["function_rva"]

        output.append(
            f"private const nuint {variable}Rva = "
            f"0x{function_rva:X};"
        )

    output.append("")


    # --------------------------------------------------------
    # Original function pointers
    # --------------------------------------------------------

    output.append(
        "// Original / next-hook function pointers"
    )

    output.append("")

    for class_name, result in results.items():

        if result is None:
            continue

        output.append(
            make_original_declaration(
                result["variable"],
                result["arg_count"]
            )
        )

    output.append("")


    # --------------------------------------------------------
    # PerfInit
    # --------------------------------------------------------

    output.append(
        "internal unsafe void PerfInit()"
    )

    output.append("{")

    for class_name, result in results.items():

        if result is None:
            continue

        output.append("")

        output.append(
            make_install(
                result["variable"],
                class_name,
                result["arg_count"]
            )
        )

    output.append("}")
    output.append("")


    # --------------------------------------------------------
    # Hook functions
    # --------------------------------------------------------

    for class_name, result in results.items():

        if result is None:
            continue

        output.append(
            make_hook(
                class_name,
                result["variable"],
                result["arg_count"]
            )
        )

        output.append("")


    generated = "\n".join(output)


    # ========================================================
    # PRINT
    # ========================================================

    print("")
    print("=" * 70)
    print("GENERATED C#")
    print("=" * 70)
    print("")
    print(generated)
    print("")
    print("=" * 70)


    # ========================================================
    # COPY TO CLIPBOARD
    # ========================================================

    try:
        r = tk.Tk()
        r.withdraw()
        r.clipboard_clear()
        r.clipboard_append(generated)
        r.update()
        print(
            "[+] Generated C# copied to clipboard."
        )

    except Exception as e:

        print(
            f"[!] Clipboard copy failed: {e}"
        )


# ============================================================
# RUN
# ============================================================

main()