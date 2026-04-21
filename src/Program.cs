using System;
using System.Windows.Forms;

namespace BreathingOverlay;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var overlay = new OverlayForm();
        var control = new ControlForm(overlay);

        overlay.Show();
        Application.Run(control);
    }
}
