using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreathingOverlay;

internal sealed class OverlayForm : Form
{
    private bool _lockSquare;

    public OverlayForm()
    {
        Text = "Breathing Overlay";
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        StartPosition = FormStartPosition.Manual;
        Location = new Point(30, 30);
        Size = new Size(180, 180);
        TopMost = true;
        BackColor = Color.Blue;
        ShowInTaskbar = true;
        MinimumSize = new Size(40, 40);
    }

    public void SetColor(Color color)
    {
        BackColor = color;
    }

    public void SetSquareLock(bool lockSquare)
    {
        _lockSquare = lockSquare;
        EnforceSquare();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_lockSquare)
        {
            EnforceSquare();
        }
    }

    private void EnforceSquare()
    {
        if (!_lockSquare)
        {
            return;
        }

        var size = Math.Min(Width, Height);
        if (Width != size || Height != size)
        {
            Size = new Size(size, size);
        }
    }
}
