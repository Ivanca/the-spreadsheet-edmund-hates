using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class TheSpredsheetEdmundHates
{

    static bool debugPerfTime = false;
    static DateTime? debugPerfStartTime = null;
    const bool delayedDebugPerfTime = false;

    static bool debugLoggingBeforePerf = false;
    
    class PerfData
    {
        public int calls;
        public float total;
        public float average;
        public float max;
        public float min;
        public float uniqueCalls;
    }

    static long startPerfLogTime;
    static Dictionary<string, PerfData> perfData = new Dictionary<string, PerfData>();

    static Dictionary<string, Dictionary<nint, int>> uniqueCallsByArg = new Dictionary<string, Dictionary<nint, int>>();

    static Dictionary<string, long> perfLogStartTimes = new Dictionary<string, long>();
    static public void StartPerfLog(string? id = null, int? argument = null)
    {
        if (id == null)
            return;
        if (!debugPerfTime)
            return;
        if (!perfData.ContainsKey(id))
        {
            perfData[id] = new PerfData();
        }
        perfData[id].calls++;
        // debugLogging = false;
        debugLoggingBeforePerf = debugLogging;
        debugLogging = false;
        if (argument != null)
        {
            if (!uniqueCallsByArg.ContainsKey(id))
            {
                uniqueCallsByArg[id] = new Dictionary<nint, int>();
            }
            if (!uniqueCallsByArg[id].ContainsKey((nint)argument.Value))
            {
                uniqueCallsByArg[id][(nint)argument.Value] = 0;
                perfData[id].uniqueCalls++;
            }
            uniqueCallsByArg[id][(nint)argument.Value]++;
        }
        startPerfLogTime = Stopwatch.GetTimestamp();
    
        // MewjectorApi.Log(id);
    }

    static long startGlobalPerfLogTime;
    static public void StartGlobalPerfLog(string? id = null)
    {
        if (!debugPerfTime)
            return;
        startGlobalPerfLogTime = Stopwatch.GetTimestamp();
        if (id == null)
            return;
        if (!perfData.ContainsKey(id))
        {
            perfData[id] = new PerfData();
        }
        perfData[id].calls++;
        // MewjectorApi.Log(id);
    }

    static public void EndPerfLog(string id)
    {
        if (!debugPerfTime)
            return;
        long endPerfLogTime = Stopwatch.GetTimestamp();
        double elapsedMilliseconds = (endPerfLogTime - startPerfLogTime) * 1000.0 / Stopwatch.Frequency;
        debugLogging = debugLoggingBeforePerf;
        HandlePerfLog(id, elapsedMilliseconds);
    }

    static public void HandlePerfLog(string id, double elapsedMilliseconds)
    {
        // MewjectorApi.Log($"{id} (Elapsed: {elapsedMilliseconds} ms)");
        if (perfData.ContainsKey(id))
        {
            PerfData data = perfData[id];
            data.total += (float)elapsedMilliseconds;
            data.average = data.total / data.calls;
            if (elapsedMilliseconds > data.max)
                data.max = (float)elapsedMilliseconds;
            if (elapsedMilliseconds < data.min || data.calls == 1)
                data.min = (float)elapsedMilliseconds;
        }
    }

    static public void EndGlobalPerfLog(string id)
    {
        if (!debugPerfTime)
            return;
        // debugLogging = true;
        long endGlobalPerfLogTime = Stopwatch.GetTimestamp();
        double elapsedMilliseconds = (endGlobalPerfLogTime - startGlobalPerfLogTime) * 1000.0 / Stopwatch.Frequency;
        HandlePerfLog(id, elapsedMilliseconds);
    }

    internal static void handleDelayedPerfTime()
    {
        if (debugPerfStartTime != null && debugPerfStartTime.Value.AddSeconds(5) <= DateTime.Now)
        {
            // disable 1 minute after
            LogStr("[HOOK] GameTickHook: 1 minute has passed since performance measurement initialization, disabling it.");
            debugPerfTime = false;
            debugPerfStartTime = null;
            MjInitStartTime = null;
            // print all performance data
            foreach (var kvp in perfData)
            {
                var id = kvp.Key;
                var data = kvp.Value;
                MewjectorApi.Log($"[PERF] {id.PadExact(28)}: total={data.total.ToString().PadExact(28)} ms, "
                + $"average={data.average.ToString().PadExact(28)} ms, max={data.max.ToString().PadExact(28)} ms, "
                + $"min={data.min.ToString().PadExact(28)} ms, calls={data.calls}");
            }

        }
        // allow unreachable code:
        #pragma warning disable CS0162
        if (delayedDebugPerfTime && debugPerfStartTime == null && MjInitStartTime != null && MjInitStartTime.Value.AddMinutes(10) <= DateTime.Now)
        {
            debugPerfStartTime = DateTime.Now;
            if (!debugPerfTime)
            {
                LogStr("[HOOK] GameTickHook: 2 minutes have passed since MjInitStartTime, initializing performance measurement.");
                PerfInit();
                debugPerfTime = true; // for perf measuring, remember to delete this line later
            }
        }
        #pragma warning restore CS0162
    }

    static public void ForceEnablePerfLog()
    {
        debugPerfStartTime = DateTime.Now;
        debugPerfTime = true;
        MewjectorApi.Log("Force enable start perf");
        PerfInit();
    }
}