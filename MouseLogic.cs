using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatsTableMod;

public partial class CatsTableMod
{
    // ---------------------------------------------------------------------
    // Game function pointers
    // ---------------------------------------------------------------------


    // sub_7FF6B392D180
    //
    //     MainCamera* sub_7FF6B392D180(Button* button)
    //
    unsafe static delegate* unmanaged<nint, nint> _getMouseSource;

    // sub_7FF6B402EAB0
    //
    //     double* sub_7FF6B402EAB0(
    //         MainCamera* controls,
    //         double* coordinates);
    //
    // coordinates contains X/Y on entry and is modified in-place.
    //
    unsafe static delegate* unmanaged<nint, double*, double*> _getMousePosition;

    // sub_7FF6B4022D00
    //
    //     Transform a point into a MovieClip's coordinate space.
    //
    unsafe static delegate* unmanaged<
        nint,
        double*,
        double*,
        nint,
        void
        > _transformPoint;



    unsafe private static delegate* unmanaged<nint, float*, float*> _getWorldTransform;

unsafe static delegate* unmanaged<
    nint,       // RCX = transform object
    float*,     // RDX = output 4x4 matrix
    nint        // RAX = resulting matrix
> _getTransformMatrix;

    unsafe private static delegate* unmanaged<float*, float*, float*> _invertMatrix;

    // ---------------------------------------------------------------------
    // Initialization
    // ---------------------------------------------------------------------

    internal unsafe void InitMouse()
    {


        // -------------------------------------------------------------
        // sub_7FF6B392D180
        // -------------------------------------------------------------

        _getMouseSource =
            (delegate* unmanaged<nint, nint>)
            (MewjectorApi.GameBase + (nuint)0x27d180);


        // -------------------------------------------------------------
        // sub_7FF6B402EAB0
        // -------------------------------------------------------------

        _getMousePosition =(delegate* unmanaged<nint, double*, double*>)(void*)MewjectorApi.InstallHook(
            0x9712a0, (void*)(delegate* unmanaged<nint, double*, double*>)&GetMousePositionHook);


        // -------------------------------------------------------------
        // sub_7FF6B4022D00
        // -------------------------------------------------------------

        _transformPoint =
            (delegate* unmanaged<nint, double*, double*, nint, void>)
            (MewjectorApi.GameBase + (nuint)0x972d00);

        _getWorldTransform =
            (delegate* unmanaged<nint, float*, float*>)
            (MewjectorApi.GameBase + (nuint)0x9b2b90);

            _getTransformMatrix =
                (delegate* unmanaged<nint, float*, nint>)
                (MewjectorApi.GameBase + (nuint)0x9571A0);

        _invertMatrix =
            (delegate* unmanaged<float*, float*, float*>)
            (MewjectorApi.GameBase + (nuint)0x9fd880);
    }

    unsafe static int GetRendererIndexWhereMouseIsHitting()
    {
        if (MainCamera == 0)
            return -1;
        double[] mouse = new double[2];

        fixed (double* mousePtr = mouse)
        {
            _getMousePosition(MainCamera, mousePtr);
        }
        for (int i = 0; i < rowRenderers.Count; i++)
        {
            var renderer = rowRenderers[i];

            var rootMovieclip = Read<nint>(renderer + 0x80);
            var movieclip = _getChild(rootMovieclip, GameString.Create("xxx"));
            // LogStr($"[HOOK] ClickHandlerHook: a1=0x{movieclip:X} is our test button");

            var _transformPoint =
                (delegate* unmanaged<nint, double*, double*, nint, double, void>)
                (MewjectorApi.GameBase + (nuint)0x972d00);
                

            double[] output = new double[2];

            fixed (double* inputPtr = mouse)
            fixed (double* outputPtr = output)
            {
                x = Read<nint>((nint)MewjectorApi.GameBase + 0x60);
                _transformPoint(
                    renderer,
                    outputPtr,
                    inputPtr,
                    movieclip,
                    1.0);
            }

            nint getBounds = Marshal.ReadIntPtr(Marshal.ReadIntPtr(movieclip) + 0x78); // vtable slot 15
            float* boundsRect = stackalloc float[4];

            // call getBounds:
            // LogStr($"[HOOK] ClickHandlerHook: getBounds=0x{getBounds:X}");
            x = Read<nint>((nint)MewjectorApi.GameBase + 0x60);

            ((delegate* unmanaged<nint, float*, float*>)(getBounds))(movieclip, boundsRect);
            float left   = boundsRect[0];
            float top    = boundsRect[1];
            float right  = boundsRect[2];
            float bottom = boundsRect[3];                
            // LogStr($"[HOOK] ClickHandlerHook: comparing mouse {output[0]} > {boundsRect[0]}" + "\n" +
            //         $"&& {output[0]} < {boundsRect[1]}" + "\n" +
            //     $"&& {output[1]} > {boundsRect[2]}" + "\n" +
            //         $"&& {output[1]} < {boundsRect[3]}");
            bool inside =
                output[0] > boundsRect[0] &&
                output[0] < boundsRect[1] &&
                output[1] > boundsRect[2] &&
                output[1] < boundsRect[3];

            if (inside)
            {
                LogStr($"[HOOK] ClickHandlerHook: mouse is inside bounds of renderer index {i} pointer: 0x{renderer:X}");
                return i;
            } else
            {
                // LogStr($"[HOOK] ClickHandlerHook: mouse is outside bounds");
            }
        }
        return -1;
    }

    static nint MainCamera = 0;

    [UnmanagedCallersOnly]
    unsafe static double* GetMousePositionHook(nint mewControls, double* point)
    {

        MainCamera = mewControls;
        return _getMousePosition(mewControls, point);
    }


    // =====================================================================
    // MOUSE POSITION
    // =====================================================================

    // =====================================================================
    // MOVIECLIP BOUNDS
    // =====================================================================

    /// <summary>
    /// Calls MovieClip vtable slot 15 (byte offset 0x78).
    ///
    /// The game uses this to obtain the four values of the "bounds"
    /// MovieClip's rectangle.
    ///
    /// The values are interpreted as:
    ///
    ///     [0] = left
    ///     [1] = right
    ///     [2] = bottom
    ///     [3] = top
    /// </summary>
    unsafe static bool TryGetMovieClipBounds(
        nint movieClip,
        out float left,
        out float right,
        out float bottom,
        out float top)
    {
        left = 0;
        right = 0;
        bottom = 0;
        top = 0;

        if (movieClip == 0)
            return false;


        nint vtable = *(nint*)movieClip;

        if (vtable == 0)
            return false;


        // vtable + 0x78 == virtual function #15
        nint functionAddress = *(nint*)(vtable + 0x78);

        if (functionAddress == 0)
            return false;


        delegate* unmanaged<nint, float*, void> getBounds =
            (delegate* unmanaged<nint, float*, void>)
            functionAddress;


        float* bounds = stackalloc float[4];

        getBounds(movieClip, bounds);


        left   = bounds[0];
        top  = bounds[1];
        right = bounds[2];
        bottom    = bounds[3];

        return true;
    }


    // =====================================================================
    // FIND "bounds"
    // =====================================================================

    /// <summary>
    /// Finds the child MovieClip named "bounds".
    ///
    /// Replace the body of this method with the MovieClip lookup routine
    /// you already use in your mod.
    /// </summary>
    static unsafe nint FindBoundsMovieClip(nint rootMovieClip)
    {
        if (rootMovieClip == 0)
            return 0;


        return _getChild(rootMovieClip, GameString.Create("bounds"));
    }


    // =====================================================================
    // TRANSFORM MOUSE INTO BOUNDS SPACE
    // =====================================================================

    /// <summary>
    /// Transforms logical screen coordinates into the local coordinate
    /// system of the bounds MovieClip.
    ///
    /// IMPORTANT:
    ///
    /// The game's 6D30 passes:
    ///
    ///     *(a1 + 56)
    ///
    /// as the first argument to 2D00.
    ///
    /// Therefore the correct displayContext must come from the object
    /// performing the hit test. It is NOT necessarily the root MovieClip.
    ///
    /// This overload lets you explicitly provide it.
    /// </summary>
    unsafe static void TransformPointToBounds(
        nint displayContext,
        nint bounds,
        double screenX,
        double screenY,
        out double localX,
        out double localY)
    {
        double* input = stackalloc double[2];
        double* output = stackalloc double[2];

        input[0] = screenX;
        input[1] = screenY;


        _transformPoint(
            displayContext,
            output,
            input,
            bounds);


        localX = output[0];
        localY = output[1];
    }


    // =====================================================================
    // CHECK ONE RENDERER
    // =====================================================================
    static nint x = 0;

unsafe private static bool IsMouseInsideRenderer(nint renderer, nint mewControls)
{
    try
    {
        LogStr("");
        LogStr("========== HITTEST BEGIN ==========");

        if (renderer == 0 || mewControls == 0)
        {
            LogStr(
                $"[HitTest] INVALID: " +
                $"renderer=0x{renderer:X}, " +
                $"mewControls=0x{mewControls:X}");

            return false;
        }

        nint renderCore = *(nint*)(renderer + 0x38);
        nint transform = *(nint*)(renderer + 0x40);
        nint rootMovieClip = *(nint*)(renderer + 0x80);

        LogStr($"[HitTest] renderer       = 0x{renderer:X}");
        LogStr($"[HitTest] mewControls    = 0x{mewControls:X}");
        LogStr($"[HitTest] renderer+38    = 0x{renderCore:X}");
        LogStr($"[HitTest] renderer+40    = 0x{transform:X}");
        LogStr($"[HitTest] renderer+80    = 0x{rootMovieClip:X}");

        if (rootMovieClip == 0)
        {
            LogStr("[HitTest] rootMovieClip == NULL");
            return false;
        }

        nint bounds = FindBoundsMovieClip(rootMovieClip);

        // vtable slot

        LogStr($"[HitTest] bounds         = 0x{bounds:X}");

        if (bounds == 0)
        {
            LogStr("[HitTest] bounds == NULL");
            return false;
        }

        // ------------------------------------------------------------
        // Mouse position
        // ------------------------------------------------------------

        double[] mouse = new double[2];

        fixed (double* mousePtr = mouse)
        {
            _getMousePosition(mewControls, mousePtr);
        }

        LogStr(
            $"[HitTest] Mouse position = " +
            $"({mouse[0]:R}, {mouse[1]:R})");

        // ------------------------------------------------------------
        // bounds transform
        // ------------------------------------------------------------

        float[] boundsTransform = new float[6];

        fixed (float* p = boundsTransform)
        {
            _getWorldTransform(bounds, p);
        }

        LogStr("[HitTest] 4062B90(bounds):");

        for (int i = 0; i < 6; i++)
        {
            LogStr(
                $"[HitTest]   [{i}] = {boundsTransform[i]:R} " +
                $"(0x{BitConverter.SingleToInt32Bits(boundsTransform[i]):X8})");
        }

        LogStr(
            $"[HitTest] transform ptr = 0x{transform:X}");

        // ------------------------------------------------------------
        // DO NOT call 471A0 from here yet.
        //
        // We have only established that native 2D00 calls it as:
        //
        //     RCX = [renderer + 0x40]
        //
        // but we have NOT established its complete ABI/signature.
        // ------------------------------------------------------------

        LogStr(
            "[HitTest] Skipping 471A0: " +
            "signature not yet established.");

        // ------------------------------------------------------------
        // 2D00
        //
        // RCX = renderer
        // RDX = output
        // R8  = mouse input
        // R9  = bounds MovieClip
        // ------------------------------------------------------------

        double[] output = new double[2];

        LogStr(
            $"[HitTest] BEFORE 2D00: " +
            $"RCX(renderer)=0x{renderer:X}, " +
            $"R8(mouse)=({mouse[0]:R},{mouse[1]:R}), " +
            $"R9(bounds)=0x{bounds:X}");

        fixed (double* inputPtr = mouse)
        fixed (double* outputPtr = output)
        {
            _transformPoint(
                renderer,
                outputPtr,
                inputPtr,
                bounds);
        }

        LogStr(
            $"[HitTest] AFTER 2D00: " +
            $"output=({output[0]:R}, {output[1]:R})");

        LogStr(
            $"[HitTest] finite: " +
            $"X={double.IsFinite(output[0])}, " +
            $"Y={double.IsFinite(output[1])}");

        // ------------------------------------------------------------
        // Existing bounds rectangle
        // ------------------------------------------------------------

        float left = 0.0f;
        float right = 978.45f;
        float bottom = 0.0f;
        float top = 75.85f;

        TryGetMovieClipBounds(bounds, out left, out right, out bottom, out top);

        LogStr(
            $"[HitTest] Bounds rect: " +
            $"L={left:R} R={right:R} " +
            $"B={bottom:R} T={top:R}");

        bool xInside =
            double.IsFinite(output[0]) &&
            output[0] >= left &&
            output[0] <= right;

        bool yInside =
            double.IsFinite(output[1]) &&
            output[1] >= bottom &&
            output[1] <= top;

        LogStr(
            $"[HitTest] X check: " +
            $"{output[0]:R} >= {left:R} && " +
            $"{output[0]:R} <= {right:R} = {xInside}");

        LogStr(
            $"[HitTest] Y check: " +
            $"{output[1]:R} >= {bottom:R} && " +
            $"{output[1]:R} <= {top:R} = {yInside}");

        bool inside = xInside && yInside;

        LogStr($"[HitTest] Result: inside={inside}");
        LogStr("========== HITTEST END ==========");

        return inside;
    }
    catch (Exception ex)
    {
        LogStr($"[HitTest] EXCEPTION: {ex}");
        return false;
    }
}
    // =====================================================================
    // FIND WHICH ROW IS UNDER THE MOUSE
    // =====================================================================

    /// <summary>
    /// Returns the first row renderer whose "bounds" contains the mouse.
    ///
    /// If rowRenderers are ordered from visually-topmost to bottommost,
    /// this is enough.
    /// </summary>
    unsafe static nint GetHoveredRowRenderer()
    {
        foreach (nint renderer in rowRenderers)
        {
            if (IsMouseInsideRenderer(
                    renderer,
                    MainCamera))
            {
                return renderer;
            }
        }


        return 0;
    }


}