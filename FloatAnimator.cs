using System;
using System.Diagnostics;

public class FloatAnimator
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private readonly float _startValue;
    public readonly float targetValue;
    private readonly float _durationSeconds;
    private readonly long _startTicks;

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

        float t = Math.Min(elapsedSeconds / _durationSeconds, 1.0f);

        return Lerp(_startValue, this.targetValue, t);
    }

    /// <summary>
    /// Returns true once the animation has finished.
    /// </summary>
    public bool IsFinished =>
        (_stopwatch.ElapsedTicks - _startTicks) >=
        _durationSeconds * Stopwatch.Frequency;

    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

}

