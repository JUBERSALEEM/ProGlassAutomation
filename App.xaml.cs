using System.Windows;
using ProGlassAutomation.Customization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeBento;

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
            // Restore persisted customization before any window shows — validate, never crash on corrupt file.
            try
            {
                var saved = CustomizationStore.Load();
                if (saved.Appearance != Themes.ActiveAppearance || saved.Preferences.Validate() is null)
                    Themes.Apply(ThemeService.Bento, saved.Appearance, (pkg, mode) => BentoCustomization.ApplyExtended(pkg, mode, saved.Preferences));
            }
            catch { /* safe fallback already handled in store */ }
            MainWindow = new MainWindow(Themes);
            MainWindow.Show();
        }
        catch (Exception error)
        {
            MessageBox.Show(error.ToString(), "ProGlass could not start", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
