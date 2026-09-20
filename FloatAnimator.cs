using System;
using System.Diagnostics;

public class FloatAnimator
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private readonly float _startValue;
    public readonly float targetValue;
    public float t;
    private readonly float _durationSeconds;
    private readonly long _startTicks;
    public bool justFinished = false;
    public bool finished = false;

    public FloatAnimator(float startValue, float targetValue, float durationSeconds)
    {
        _startValue = startValue;
        this.targetValue = targetValue;
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
        var oldT = t;
        t = Math.Min(elapsedSeconds / _durationSeconds, 1.0f);
        if (t > oldT && t >= 1.0f)
        {
            justFinished = true;
            finished = true;
        } else
        {
            justFinished = false;
        }

        return MathF.Round(LerpWithEaseInAndOut(_startValue, this.targetValue, t), 2);
    }


    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    static float LerpWithEaseInAndOut(float a, float b, float t)
    {
        // Ease in and out using a cubic function
        t = t * t * (3f - 2f * t);
        return Lerp(a, b, t);
    }

}

