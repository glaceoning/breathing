using System;
using System.Collections.Generic;
using System.Drawing;

namespace BreathingOverlay;

internal enum BreathingPhase
{
    Inhale,
    HoldIn,
    Exhale,
    HoldOut
}

internal readonly record struct PhaseStep(BreathingPhase Phase, int Seconds, Color Color);

internal sealed class BreathingEngine
{
    private readonly BreathingSettings _settings;
    private readonly List<PhaseStep> _steps = new();
    private int _stepIndex;
    private int _elapsedSeconds;

    public BreathingEngine(BreathingSettings settings)
    {
        _settings = settings;
        Rebuild();
    }

    public PhaseStep CurrentStep => _steps.Count == 0
        ? new PhaseStep(BreathingPhase.Inhale, 1, _settings.InhaleColor)
        : _steps[_stepIndex];

    public int ElapsedInStep => _elapsedSeconds;

    public void Rebuild()
    {
        _steps.Clear();

        AddIfValid(BreathingPhase.Inhale, _settings.InhaleSeconds, _settings.InhaleColor);
        if (_settings.UseHoldIn)
        {
            AddIfValid(BreathingPhase.HoldIn, _settings.HoldInSeconds, _settings.HoldInColor);
        }

        AddIfValid(BreathingPhase.Exhale, _settings.ExhaleSeconds, _settings.ExhaleColor);
        if (_settings.UseHoldOut)
        {
            AddIfValid(BreathingPhase.HoldOut, _settings.HoldOutSeconds, _settings.HoldOutColor);
        }

        if (_steps.Count == 0)
        {
            _steps.Add(new PhaseStep(BreathingPhase.Inhale, 1, _settings.InhaleColor));
        }

        _stepIndex = 0;
        _elapsedSeconds = 0;
    }

    public void AdvanceOneSecond()
    {
        if (_steps.Count == 0)
        {
            return;
        }

        var current = _steps[_stepIndex];
        _elapsedSeconds++;

        if (_elapsedSeconds >= current.Seconds)
        {
            _stepIndex = (_stepIndex + 1) % _steps.Count;
            _elapsedSeconds = 0;
        }
    }

    private void AddIfValid(BreathingPhase phase, int seconds, Color color)
    {
        if (seconds <= 0)
        {
            return;
        }

        _steps.Add(new PhaseStep(phase, seconds, color));
    }
}
