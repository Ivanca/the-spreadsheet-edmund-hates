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
        nint, double*, double*, nint, double, void> _transformPoint;






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
            (MewjectorApi.GameBase + (nuint)0x27dc50);


        // -------------------------------------------------------------
        // sub_7FF6B402EAB0
        // -------------------------------------------------------------

        _getMousePosition =(delegate* unmanaged<nint, double*, double*>)(void*)MewjectorApi.InstallHook(
            0x9796d0, (void*)(delegate* unmanaged<nint, double*, double*>)&GetMousePositionHook);


        // -------------------------------------------------------------
        // sub_7FF6B4022D00
        // -------------------------------------------------------------

        _transformPoint =
            (delegate* unmanaged<nint, double*, double*, nint, double, void>)
            (MewjectorApi.GameBase + (nuint)0x97b130);



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
        for (int i = 0; i < rowRenderers.Length; i++)
        {
            var renderer = rowRenderers[i];

            var rootMovieclip = Read<nint>(renderer + 0x80);
            var movieclip = _getChild(rootMovieclip, GameString.Create("xxx"));
            // LogStr($"[HOOK] ClickHandlerHook: a1=0x{movieclip:X} is our test button");


                

            double[] output = new double[2];

            fixed (double* inputPtr = mouse)
            fixed (double* outputPtr = output)
            {
                // x = Read<nint>((nint)MewjectorApi.GameBase + 0x60);
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
            // x = Read<nint>((nint)MewjectorApi.GameBase + 0x60);

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
                // LogStr($"[HOOK] ClickHandlerHook: mouse is inside bounds of renderer index {i} pointer: 0x{renderer:X}");
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


   
}