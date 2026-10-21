using System.Windows;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;
using ProGlassAutomation.ThemeFluent;

namespace ProGlassAutomation.Theming;

public enum ThemeGeometry { SharedTokens, ThemeOwned }

public sealed record ThemeDefinition(
    string Name,
    string Folder,
    Func<Appearance, ResourceDictionary> LoadResources,
    ThemeGeometry Geometry = ThemeGeometry.SharedTokens);

/// <summary>Register only complete, authorized themes. No placeholder entries.</summary>
public static class ThemeCatalog
{
    public const string BentoName = "Bento Grid / Modular";
    public const string ClayName = "Claymorphism";
    public const string FluentName = "Fluent / Acrylic";
    public static IReadOnlyList<ThemeDefinition> Installed { get; } =
        Array.AsReadOnly(new[]
        {
            new ThemeDefinition(BentoName, "ThemeBento", BentoResources.Load),
            new ThemeDefinition(ClayName, "ThemeClaymorphism", ClayResources.Load, ThemeGeometry.ThemeOwned),
            new ThemeDefinition(FluentName, "ThemeFluent", FluentResources.Load),
        });

    public static ThemeDefinition Find(string name) =>
        Installed.FirstOrDefault(theme => theme.Name == name)
        ?? throw new ArgumentException($"Theme '{name}' is not installed.", nameof(name));
}
