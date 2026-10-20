using System.Windows;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation;

public partial class App : Application
{
    public ThemeService Themes { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            Themes.Initialize(Resources);
            MainWindow = new MainWindow(Themes);
            MainWindow.Show();
        }
        catch (Exception error)
        {
            // Startup may fail before themed resources exist; use the native error dialog.
            MessageBox.Show(error.ToString(), "ProGlass could not start", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
