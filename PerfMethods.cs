using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class TheSpredsheetEdmundHates
{

    static bool debugPerfTime = false;
    const bool delayedDebugPerfTime = false;
    
    class PerfData
    {
        public int calls;
        public float total;
        public float average;
        public float max;
        public float min;
    }

    static long startPerfLogTime;
    static Dictionary<string, PerfData> perfData = new Dictionary<string, PerfData>();

    static Dictionary<string, long> perfLogStartTimes = new Dictionary<string, long>();
    static public void StartPerfLog(string? id = null)
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
        debugLogging = false;
        startPerfLogTime = Stopwatch.GetTimestamp();

        // MewjectorApi.Log(id);
    }

    static long startGlobalPerfLogTime;
    static public void StartGlobalPerfLog(string? id = null)
    {
        if (!debugPerfTime)
            return;
        startGlobalPerfLogTime = Stopwatch.GetTimestamp();
        debugLogging = false;
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
        debugLogging = true;
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
        debugLogging = true;
        long endGlobalPerfLogTime = Stopwatch.GetTimestamp();
        double elapsedMilliseconds = (endGlobalPerfLogTime - startGlobalPerfLogTime) * 1000.0 / Stopwatch.Frequency;
        HandlePerfLog(id, elapsedMilliseconds);
    }
    internal static void handleDelayedPerfTime()
    {
        // allow unreachable code:
        #pragma warning disable CS0162
        if (delayedDebugPerfTime)
        {
            if (MjInitStartTime != null && MjInitStartTime.Value.AddMinutes(31) <= DateTime.Now)
            {
                // disable 1 minute after
                LogStr("[HOOK] GameTickHook: 1 minute has passed since performance measurement initialization, disabling it.");
                debugPerfTime = false;
                MjInitStartTime = null;
                // print all performance data
                foreach (var kvp in perfData)
                {
                    var id = kvp.Key;
                    var data = kvp.Value;
                    MewjectorApi.Log($"[PERF] {id}: total={data.total} ms, average={data.average} ms, max={data.max} ms, min={data.min} ms, calls={data.calls}");
                }

            }
            else if (MjInitStartTime != null && MjInitStartTime.Value.AddMinutes(30) <= DateTime.Now)
            {
                if (!debugPerfTime)
                {
                    LogStr("[HOOK] GameTickHook: 30 minutes have passed since MjInitStartTime, initializing performance measurement.");
                    PerfInit();
                    debugPerfTime = true; // for perf measuring, remember to delete this line later
                }
            }
        }
        #pragma warning restore CS0162
    }
}