using System.ComponentModel;
using System.IO;
using System.Windows.Markup;
using System.Windows;

namespace ProGlassAutomation.Theming;

public enum Appearance { Light, Dark }

/// <summary>One complete theme-package slot; loading/validation precede replacement.</summary>
public sealed class ThemeService : INotifyPropertyChanged
{
    public const string Bento = ThemeCatalog.BentoName;
    public static IReadOnlyList<string> InstalledThemes { get; } = Array.AsReadOnly(ThemeCatalog.Installed.Select(theme => theme.Name).ToArray());
    public Appearance ActiveAppearance { get; private set; } = Appearance.Light;
    public string ActiveTheme { get; private set; } = Bento;
    public event PropertyChangedEventHandler? PropertyChanged;
    private ResourceDictionary? applicationResources;
    private ResourceDictionary? activeResources;

    public void Initialize(ResourceDictionary resources)
    {
        Application.Current.Dispatcher.VerifyAccess();
        if (applicationResources is not null)
            throw new InvalidOperationException("The theme engine is already initialized.");
        var package = Load(Appearance.Light);
        // One complete theme package; no visual dictionary survives a theme replacement.
        resources.MergedDictionaries.Add(package);
        applicationResources = resources;
        activeResources = package;
    }

    public void Apply(string theme, Appearance appearance)
    {
        Application.Current.Dispatcher.VerifyAccess();
        if (!Enum.IsDefined(appearance))
            throw new ArgumentException("Only defined appearance modes can be selected.");
        if (applicationResources is null || activeResources is null)
            throw new InvalidOperationException("Initialize the theme engine before selecting an appearance.");
        var candidate = Load(appearance, theme);
        int slot = applicationResources.MergedDictionaries.IndexOf(activeResources);
        if (slot < 0)
            throw new InvalidOperationException("The active theme resource slot was removed.");
        try { applicationResources.MergedDictionaries[slot] = candidate; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or XamlParseException)
        {
            applicationResources.MergedDictionaries[slot] = activeResources;
            throw new InvalidOperationException($"Could not activate {theme} / {appearance}; previous package restored.", error);
        }
        activeResources = candidate;
        ActiveTheme = theme;
        ActiveAppearance = appearance;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveAppearance)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveTheme)));
    }

    public object GetToken(string key) => activeResources?[key]
        ?? throw new InvalidOperationException($"No active token named {key}.");

    public static ResourceDictionary Load(Appearance appearance, string theme = Bento)
    {
        if (!Enum.IsDefined(appearance)) throw new ArgumentOutOfRangeException(nameof(appearance));
        var definition = ThemeCatalog.Find(theme);
        try
        {
            var resources = definition.LoadResources(appearance)
                ?? throw new InvalidOperationException("The theme returned no resource package.");
            ResourceValidation.Validate(resources, definition.Name + " / " + appearance);
            return resources;
        }
        catch (Exception error) when (error is IOException or XamlParseException or InvalidOperationException or ArgumentException)
        {
            throw new InvalidOperationException($"Cannot load {definition.Name} / {appearance}: {error.Message}", error);
        }
    }
}
