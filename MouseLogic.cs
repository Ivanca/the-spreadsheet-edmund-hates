using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace TheSpredsheetEdmundHates;

public partial class TheSpredsheetEdmundHates
{

    unsafe static delegate* unmanaged<nint, double*, double*> _getMousePosition;

    unsafe static delegate* unmanaged<
        nint, double*, double*, nint, double, void> _transformPoint;
    
    unsafe static delegate* unmanaged<
        nint, float*, void> _getWorldTransform;

    internal unsafe void InitMouse()
    {

        _getMousePosition =(delegate* unmanaged<nint, double*, double*>)(void*)MewjectorApi.InstallHook(
            0x9796d0, (void*)(delegate* unmanaged<nint, double*, double*>)&GetMousePositionHook);


        _transformPoint =
            (delegate* unmanaged<nint, double*, double*, nint, double, void>)
            (MewjectorApi.GameBase + (nuint)0x97b130);
        _getWorldTransform = 
            (delegate* unmanaged<nint, float*, void>)(MewjectorApi.GameBase + (nuint)0x9b1880);

    }
    static nint[] hoveredAreasCache = new nint[0];
    static double[] prevMouse = new double[2];
    static double[] prevTransformedMouse = new double[2];
    static double[] prevBounds = new double[4];
    static int lastReturnedRenderer = -1;
    static nint[] renderersInsideRenderArea = new nint[0];

    unsafe static int GetRendererIndexWhereMouseIsHitting()
    {
        if (MainCamera == 0)
            return -1;
        double[] mouse = new double[2];

        fixed (double* mousePtr = mouse)
        {
            _getMousePosition(MainCamera, mousePtr);
        }
        if (prevMouse[0] == mouse[0] && prevMouse[1] == mouse[1])
        {
            return lastReturnedRenderer;
        }
        prevMouse[0] = mouse[0];
        prevMouse[1] = mouse[1];
        if (hoveredAreasCache.Length != rowRenderers.Length)
        {
            hoveredAreasCache = new nint[rowRenderers.Length];
        }
        for (int i = 0; i < rowRenderers.Length; i++)
        {
            var renderer = rowRenderers[i];
            nint movieclip = 0;
            if (hoveredAreasCache[i] != 0)
            {
                movieclip = hoveredAreasCache[i];
            } else
            {
                var rootMovieclip = Read<nint>(renderer + 0x80);
                movieclip = _getChild(rootMovieclip, GameString.Create("hover_area"));
                hoveredAreasCache[i] = movieclip;
            }

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
            prevTransformedMouse[0] = output[0];
            prevTransformedMouse[1] = output[1];


            delegate* unmanaged<nint, float*, float*> getBounds =
                (delegate* unmanaged<nint, float*, float*>)Marshal.ReadIntPtr(Marshal.ReadIntPtr(movieclip) + 0x78); // vtable slot 15
            float* boundsRect = stackalloc float[4];

            // call getBounds:
            getBounds(movieclip, boundsRect);
            // LogStr($"[HOOK] ClickHandlerHook: getBounds=0x{getBounds:X}");
            // x = Read<nint>((nint)MewjectorApi.GameBase + 0x60);

            float left   = boundsRect[0];
            float top    = boundsRect[1];
            float right  = boundsRect[2];
            float bottom = boundsRect[3];        

            prevBounds[0] = left;
            prevBounds[1] = top;
            prevBounds[2] = right;
            prevBounds[3] = bottom;
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
                lastReturnedRenderer = i;
                return i;
            }
        }
        lastReturnedRenderer = -1;
        return -1;
    }

    public static unsafe void updateRenderersInsideRenderArea()
    {
        if (renderersInsideRenderArea.Length == 0)
        {
            renderersInsideRenderArea = new nint[hoveredAreasCache.Length];
        }
        if (!ourPanelIsOpen && !panelAnimationInProgress)
        {
            Array.Clear(renderersInsideRenderArea, 0, renderersInsideRenderArea.Length);
        } else if (hoveredAreasCache.Length > 0)
        {
            Array.Clear(renderersInsideRenderArea, 0, renderersInsideRenderArea.Length);
            var lastVisibleOne = -1;
            var firstVisibleOne = -1;
            for (int i = 0; i < hoveredAreasCache.Length; i++)
            {
                if (!IsHoverAreaOutsideRenderArea(rowRenderers[i], hoveredAreasCache[i]))
                {
                    LogStr($"[HOOK] updateRenderersInsideRenderArea: renderer {i} is inside render area");
                    var renderer = rowRenderers[i];
                    LogStr($"[HOOK] updateRenderersInsideRenderArea: renderer value = 0x{renderer:X} renderersInsideRenderArea size is {renderersInsideRenderArea.Length} and i = {i}");
                    renderersInsideRenderArea[i] = renderer;
                    lastVisibleOne = i;
                    if (firstVisibleOne == -1)
                    {
                        firstVisibleOne = i;
                    }
                }
            }
            if (lastVisibleOne != -1 && lastVisibleOne < hoveredAreasCache.Length - 1)
            {
                LogStr($"[HOOK] updateRenderersInsideRenderArea: lastVisibleOne = {lastVisibleOne}");
                // add one extra as scroll buffer
                renderersInsideRenderArea[lastVisibleOne + 1] = rowRenderers[lastVisibleOne + 1];
                if (lastVisibleOne < hoveredAreasCache.Length - 2)
                {
                    // add one more extra as scroll buffer
                    renderersInsideRenderArea[lastVisibleOne + 2] = rowRenderers[lastVisibleOne + 2];
                }
            }
            if (firstVisibleOne > 0)
            {
                LogStr($"[HOOK] updateRenderersInsideRenderArea: firstVisibleOne = {firstVisibleOne}");
                // add one extra before the first visible one as scroll buffer
                renderersInsideRenderArea[firstVisibleOne - 1] = rowRenderers[firstVisibleOne - 1];
                if (firstVisibleOne > 1)
                {
                    // add one more extra before the first visible one as scroll buffer
                    renderersInsideRenderArea[firstVisibleOne - 2] = rowRenderers[firstVisibleOne - 2];
                }
            }
            LogStr($"[HOOK] updateRenderersInsideRenderArea: renderersInsideRenderArea Count = {renderersInsideRenderArea.Length}");
        } else
        {
            // copy array from rowRenderers (just first 13 or less)
            LogStr($"[HOOK] updateRenderersInsideRenderArea: using fallback for renderersInsideRenderArea, Count = {renderersInsideRenderArea.Length}");
            renderersInsideRenderArea = rowRenderers.Take(13).ToArray();
            Array.Resize(ref renderersInsideRenderArea, hoveredAreasCache.Length);
        }

        foreach (var renderer in rowRenderers)
        {
            var drawer = rowDrawers[Array.IndexOf(rowRenderers, renderer)];
            var catParts1 = Read<nint>(drawer + 0x48);
            var catParts2 = Read<nint>(drawer + 0x50);
            var catParts3 = Read<nint>(drawer + 0x58);
            var isVisible = renderersInsideRenderArea.Contains(renderer);
            // Write(rendererStruct->Transform + 0x10, isVisible ? (byte)1 : (byte)0); // enable transform
            // Write(rendererStruct->Transform + 0xF, isVisible? (byte)0 : (byte)1); // enable row transform
            Write(renderer + 0x51, isVisible? (byte)1 : (byte)0); // show renderer
            Write(renderer + 0x10, isVisible? (byte)1 : (byte)0); // enable renderer
            Write(catParts1 + 0x10, isVisible? (byte)1 : (byte)0); // enable cat part 1
            Write(catParts2 + 0x10, isVisible? (byte)1 : (byte)0); // enable cat part 2
            Write(catParts3 + 0x10, isVisible? (byte)1 : (byte)0); // enable cat part 3
        }
}

private static unsafe bool IsHoverAreaOutsideRenderArea(
    nint renderer,
    nint hoverArea)
{
    // These are NOT screen pixels.
    //
    // They are the coordinate-space values returned by
    // _getMousePosition(MainCamera, ...).
    //
    // Measured from the four screen corners on this machine.

    const double viewportMouseLeft   = -21.33333396911621;
    const double viewportMouseRight  =  21.31111269940933;
    const double viewportMouseTop    =  12.0;
    const double viewportMouseBottom = -11.977779028316341;

    float* bounds = stackalloc float[4];

delegate* unmanaged<nint, float*, float*> GetBounds =
                (delegate* unmanaged<nint, float*, float*>)Marshal.ReadIntPtr(Marshal.ReadIntPtr(hoverArea) + 0x78); // vtable slot 15
    // GetBounds layout:
    //
    // bounds[0] = left
    // bounds[1] = top
    // bounds[2] = right
    // bounds[3] = bottom
    //
    // Therefore:
    //
    // X = bounds[0] .. bounds[1]
    // Y = bounds[2] .. bounds[3]
    GetBounds(hoverArea, bounds);

    double hoverLeft   = bounds[0];
    double hoverRight  = bounds[1];
    double hoverTop    = bounds[2];
    double hoverBottom = bounds[3];

    double* input = stackalloc double[2];
    double* output = stackalloc double[2];

    double minX = double.PositiveInfinity;
    double maxX = double.NegativeInfinity;
    double minY = double.PositiveInfinity;
    double maxY = double.NegativeInfinity;

    void TransformPoint(double x, double y)
    {
        input[0] = x;
        input[1] = y;

        _transformPoint(
            renderer,
            output,
            input,
            hoverArea,
            1.0);

        minX = Math.Min(minX, output[0]);
        maxX = Math.Max(maxX, output[0]);

        minY = Math.Min(minY, output[1]);
        maxY = Math.Max(maxY, output[1]);
    }

    // Top-left
    TransformPoint(
        viewportMouseLeft,
        viewportMouseTop);

    // Top-right
    TransformPoint(
        viewportMouseRight,
        viewportMouseTop);

    // Bottom-left
    TransformPoint(
        viewportMouseLeft,
        viewportMouseBottom);

    // Bottom-right
    TransformPoint(
        viewportMouseRight,
        viewportMouseBottom);

    bool outside =
        maxX <= hoverLeft ||
        minX >= hoverRight ||
        maxY <= hoverTop ||
        minY >= hoverBottom;

    LogStr(
        $"hover_area: " +
        $"viewport local X={minX}..{maxX}, " +
        $"Y={minY}..{maxY}, " +
        $"bounds X={hoverLeft}..{hoverRight}, " +
        $"Y={hoverTop}..{hoverBottom}, " +
        $"outside={outside}");

    return outside;
}
    static nint MainCamera = 0;

    [UnmanagedCallersOnly]
    unsafe static double* GetMousePositionHook(nint mewControls, double* point)
    {

        MainCamera = mewControls;
        return _getMousePosition(mewControls, point);
    }

}