using System.Windows;
using ProGlassAutomation.Customization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;

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
                var theme = ThemeService.InstalledThemes.Contains(saved.Theme) ? saved.Theme : ThemeService.Bento;
                Action<System.Windows.ResourceDictionary, Appearance>? configure = theme == ThemeService.Bento
                    ? (pkg, mode) => BentoCustomization.ApplyExtended(pkg, mode, saved.Preferences)
                    : theme == ThemeService.Clay
                    ? (pkg, mode) => ClayCustomization.Apply(pkg, mode, saved.Clay ?? new ClayCustomizationState())
                    : null;
                Themes.Apply(theme, saved.Appearance, configure);
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
