using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreathingOverlay;

internal sealed class ControlForm : Form
{
    private readonly OverlayForm _overlay;
    private readonly BreathingSettings _settings;
    private readonly BreathingEngine _engine;
    private readonly System.Windows.Forms.Timer _timer;

    private readonly Label _status;

    private readonly NumericUpDown _inhaleSeconds;
    private readonly NumericUpDown _holdInSeconds;
    private readonly NumericUpDown _exhaleSeconds;
    private readonly NumericUpDown _holdOutSeconds;

    private readonly CheckBox _useHoldIn;
    private readonly CheckBox _useHoldOut;
    private readonly CheckBox _lockSquare;

    private readonly Button _inhaleColor;
    private readonly Button _holdInColor;
    private readonly Button _exhaleColor;
    private readonly Button _holdOutColor;

    public ControlForm(OverlayForm overlay)
    {
        _overlay = overlay;
        _settings = new BreathingSettings();
        _engine = new BreathingEngine(_settings);

        Text = "Breathing Overlay Controls";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = true;
        Width = 430;
        Height = 430;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 10,
            Padding = new Padding(10)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

        Controls.Add(layout);

        _inhaleSeconds = CreateSecondsNumeric(4);
        _holdInSeconds = CreateSecondsNumeric(4);
        _exhaleSeconds = CreateSecondsNumeric(4);
        _holdOutSeconds = CreateSecondsNumeric(4);

        _useHoldIn = new CheckBox { Text = "Use hold after inhale", Checked = true, AutoSize = true };
        _useHoldOut = new CheckBox { Text = "Use hold after exhale", Checked = true, AutoSize = true };
        _lockSquare = new CheckBox { Text = "Lock overlay to square", Checked = false, AutoSize = true };

        _inhaleColor = CreateColorButton(_settings.InhaleColor);
        _holdInColor = CreateColorButton(_settings.HoldInColor);
        _exhaleColor = CreateColorButton(_settings.ExhaleColor);
        _holdOutColor = CreateColorButton(_settings.HoldOutColor);

        AddRow(layout, 0, "Inhale seconds", _inhaleSeconds, _inhaleColor);
        AddRow(layout, 1, "Hold-in seconds", _holdInSeconds, _holdInColor);
        AddRow(layout, 2, "Exhale seconds", _exhaleSeconds, _exhaleColor);
        AddRow(layout, 3, "Hold-out seconds", _holdOutSeconds, _holdOutColor);

        layout.Controls.Add(_useHoldIn, 0, 4);
        layout.SetColumnSpan(_useHoldIn, 3);

        layout.Controls.Add(_useHoldOut, 0, 5);
        layout.SetColumnSpan(_useHoldOut, 3);

        layout.Controls.Add(_lockSquare, 0, 6);
        layout.SetColumnSpan(_lockSquare, 3);

        var startButton = new Button { Text = "Start", Dock = DockStyle.Fill, Height = 35 };
        var stopButton = new Button { Text = "Stop", Dock = DockStyle.Fill, Height = 35 };

        layout.Controls.Add(startButton, 0, 7);
        layout.Controls.Add(stopButton, 1, 7);
        layout.SetColumnSpan(stopButton, 2);

        _status = new Label
        {
            Text = "Stopped",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(_status, 0, 8);
        layout.SetColumnSpan(_status, 3);

        var tip = new Label
        {
            Text = "Move/resize the overlay window directly. It always stays on top.",
            AutoSize = true,
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(tip, 0, 9);
        layout.SetColumnSpan(tip, 3);

        startButton.Click += (_, _) => Start();
        stopButton.Click += (_, _) => Stop();

        _timer = new System.Windows.Forms.Timer { Interval = 1000 };
        _timer.Tick += (_, _) =>
        {
            _engine.Tick();
            ApplyPhase();
        };

        FormClosing += (_, _) =>
        {
            _timer.Stop();
            _overlay.Close();
        };

        ApplySettings();
        ApplyPhase();
    }

    private void Start()
    {
        ApplySettings();
        _timer.Start();
        ApplyPhase();
    }

    private void Stop()
    {
        _timer.Stop();
        _status.Text = "Stopped";
    }

    private void ApplySettings()
    {
        _settings.InhaleSeconds = (int)_inhaleSeconds.Value;
        _settings.HoldInSeconds = (int)_holdInSeconds.Value;
        _settings.ExhaleSeconds = (int)_exhaleSeconds.Value;
        _settings.HoldOutSeconds = (int)_holdOutSeconds.Value;

        _settings.UseHoldIn = _useHoldIn.Checked;
        _settings.UseHoldOut = _useHoldOut.Checked;
        _settings.LockSquare = _lockSquare.Checked;

        _settings.InhaleColor = _inhaleColor.BackColor;
        _settings.HoldInColor = _holdInColor.BackColor;
        _settings.ExhaleColor = _exhaleColor.BackColor;
        _settings.HoldOutColor = _holdOutColor.BackColor;

        _overlay.SetSquareLock(_settings.LockSquare);
        _engine.Rebuild();
    }

    private void ApplyPhase()
    {
        var step = _engine.CurrentStep;
        _overlay.SetColor(step.Color);
        _overlay.TopMost = true;
        _status.Text = $"{step.Phase} ({_engine.RemainingSeconds}s)";
    }

    private static NumericUpDown CreateSecondsNumeric(int value)
    {
        return new NumericUpDown
        {
            Minimum = 0,
            Maximum = 300,
            Value = value,
            Dock = DockStyle.Fill
        };
    }

    private Button CreateColorButton(Color initial)
    {
        var button = new Button
        {
            BackColor = initial,
            Dock = DockStyle.Fill,
            Text = "Set"
        };

        button.Click += (_, _) =>
        {
            using var picker = new ColorDialog
            {
                FullOpen = true,
                Color = button.BackColor
            };

            if (picker.ShowDialog(this) == DialogResult.OK)
            {
                button.BackColor = picker.Color;
                if (_timer.Enabled)
                {
                    ApplySettings();
                    ApplyPhase();
                }
            }
        };

        return button;
    }

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control numeric, Control color)
    {
        layout.Controls.Add(new Label
        {
            Text = labelText,
            Anchor = AnchorStyles.Left,
            AutoSize = true
        }, 0, row);
        layout.Controls.Add(numeric, 1, row);
        layout.Controls.Add(color, 2, row);
    }
}
