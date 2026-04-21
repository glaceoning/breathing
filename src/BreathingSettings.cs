using System.Drawing;

namespace BreathingOverlay;

internal sealed class BreathingSettings
{
    public int InhaleSeconds { get; set; } = 4;
    public int HoldInSeconds { get; set; } = 4;
    public int ExhaleSeconds { get; set; } = 4;
    public int HoldOutSeconds { get; set; } = 4;

    public bool UseHoldIn { get; set; } = true;
    public bool UseHoldOut { get; set; } = true;

    public bool LockSquare { get; set; } = false;

    public Color InhaleColor { get; set; } = Color.FromArgb(0, 0, 255);
    public Color HoldInColor { get; set; } = Color.FromArgb(255, 0, 0);
    public Color ExhaleColor { get; set; } = Color.FromArgb(0, 255, 0);
    public Color HoldOutColor { get; set; } = Color.FromArgb(255, 255, 255);
}
