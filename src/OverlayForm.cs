using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreathingOverlay;

internal sealed class OverlayForm : Form
{
    private const int WmNchittest = 0x84;
    private const int HtClient = 1;
    private const int HtCaption = 2;
    private const int HtLeft = 10;
    private const int HtRight = 11;
    private const int HtTop = 12;
    private const int HtTopLeft = 13;
    private const int HtTopRight = 14;
    private const int HtBottom = 15;
    private const int HtBottomLeft = 16;
    private const int HtBottomRight = 17;

    private const int ResizeBorder = 8;

    private bool _lockSquare;
    private BreathingPhase _phase = BreathingPhase.Inhale;
    private int _duration = 4;
    private int _elapsed;
    private Color _phaseColor = Color.Blue;

    public OverlayForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Location = new Point(30, 30);
        Size = new Size(320, 64);
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        MinimumSize = new Size(120, 40);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW to reduce snap/alt-tab behavior.
            return cp;
        }
    }

    public void SetSquareLock(bool lockSquare)
    {
        _lockSquare = lockSquare;
        EnforceSquare();
    }

    public void UpdateState(BreathingPhase phase, int duration, int elapsed, Color phaseColor)
    {
        _phase = phase;
        _duration = Math.Max(1, duration);
        _elapsed = Math.Clamp(elapsed, 0, _duration);
        _phaseColor = phaseColor;
        Invalidate();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmNchittest)
        {
            base.WndProc(ref m);
            if ((int)m.Result == HtClient)
            {
                var cursor = PointToClient(Cursor.Position);
                var onLeft = cursor.X <= ResizeBorder;
                var onRight = cursor.X >= Width - ResizeBorder;
                var onTop = cursor.Y <= ResizeBorder;
                var onBottom = cursor.Y >= Height - ResizeBorder;

                if (onTop && onLeft) m.Result = (IntPtr)HtTopLeft;
                else if (onTop && onRight) m.Result = (IntPtr)HtTopRight;
                else if (onBottom && onLeft) m.Result = (IntPtr)HtBottomLeft;
                else if (onBottom && onRight) m.Result = (IntPtr)HtBottomRight;
                else if (onLeft) m.Result = (IntPtr)HtLeft;
                else if (onRight) m.Result = (IntPtr)HtRight;
                else if (onTop) m.Result = (IntPtr)HtTop;
                else if (onBottom) m.Result = (IntPtr)HtBottom;
                else m.Result = (IntPtr)HtCaption;
            }
            return;
        }

        base.WndProc(ref m);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_lockSquare)
        {
            EnforceSquare();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.Clear(GetBackgroundColor());

        var rect = ClientRectangle;
        rect.Inflate(-2, -2);
        using var borderPen = new Pen(Color.Gray, 1);
        g.DrawRectangle(borderPen, rect);

        if (_phase == BreathingPhase.HoldIn || _phase == BreathingPhase.HoldOut)
        {
            DrawSymmetricHold(g, rect);
        }
        else
        {
            DrawLinear(g, rect);
        }
    }

    private void DrawLinear(Graphics g, Rectangle rect)
    {
        var segments = _duration;
        var gap = 1;
        var inner = Rectangle.Inflate(rect, -2, -2);
        var segmentWidth = Math.Max(1, (inner.Width - ((segments - 1) * gap)) / Math.Max(1, segments));

        var filled = _phase == BreathingPhase.Inhale
            ? _elapsed
            : Math.Max(0, _duration - _elapsed);

        using var brush = new SolidBrush(_phaseColor);

        for (var i = 0; i < segments; i++)
        {
            if (i >= filled)
            {
                break;
            }

            var x = inner.X + i * (segmentWidth + gap);
            var w = i == segments - 1 ? inner.Right - x : segmentWidth;
            g.FillRectangle(brush, x, inner.Y, w, inner.Height);
        }
    }

    private void DrawSymmetricHold(Graphics g, Rectangle rect)
    {
        var segments = _duration * 2;
        var gap = 1;
        var inner = Rectangle.Inflate(rect, -2, -2);
        var segmentWidth = Math.Max(1, (inner.Width - ((segments - 1) * gap)) / Math.Max(1, segments));

        using var brush = new SolidBrush(_phaseColor);

        var centerLeft = (segments / 2) - 1;
        var centerRight = segments / 2;
        var pairs = _elapsed;

        for (var p = 0; p < pairs; p++)
        {
            var li = centerLeft - p;
            var ri = centerRight + p;

            FillSegment(g, brush, inner, li, segmentWidth, gap, segments);
            FillSegment(g, brush, inner, ri, segmentWidth, gap, segments);
        }
    }

    private static void FillSegment(Graphics g, Brush brush, Rectangle inner, int index, int segmentWidth, int gap, int segments)
    {
        if (index < 0 || index >= segments)
        {
            return;
        }

        var x = inner.X + index * (segmentWidth + gap);
        var w = index == segments - 1 ? inner.Right - x : segmentWidth;
        g.FillRectangle(brush, x, inner.Y, w, inner.Height);
    }

    private Color GetBackgroundColor()
    {
        return _phase switch
        {
            BreathingPhase.HoldIn or BreathingPhase.HoldOut => Color.Black,
            _ => Color.White
        };
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
