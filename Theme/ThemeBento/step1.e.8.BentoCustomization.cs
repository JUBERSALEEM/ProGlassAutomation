using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeBento;

public enum BentoAccent { Ocean, Indigo, Teal }
public enum BentoDensity { Compact, Comfortable }
public enum BentoShape { Crisp, Soft, Rounded }
public enum BentoType { Small, Standard, Large }
public sealed record BentoPreferences(BentoAccent Accent = BentoAccent.Ocean,
    BentoDensity Density = BentoDensity.Compact, BentoShape Shape = BentoShape.Soft,
    BentoType Typography = BentoType.Standard);

/// <summary>Curated, theme-owned customization. No screen palette or mutable global preferences.</summary>
public static class BentoCustomization
{
    public static void Apply(ResourceDictionary package, Appearance mode, BentoPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(preferences);
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if (!Enum.IsDefined(preferences.Accent) || !Enum.IsDefined(preferences.Density) ||
            !Enum.IsDefined(preferences.Shape) || !Enum.IsDefined(preferences.Typography))
            throw new ArgumentException("Unsupported Bento customization choice.");
        string[]? accent = (preferences.Accent, mode) switch
        {
            (BentoAccent.Indigo, Appearance.Light) => ["#5440C8", "#4632B5", "#382690", "#EDE9FF", "#FFFFFF"],
            (BentoAccent.Indigo, Appearance.Dark) => ["#B5A5FF", "#C8BCFF", "#A08FE8", "#2A2448", "#191331"],
            (BentoAccent.Teal, Appearance.Light) => ["#006B61", "#00594F", "#00473F", "#E1F4EF", "#FFFFFF"],
            (BentoAccent.Teal, Appearance.Dark) => ["#75DFCD", "#99EDDE", "#62C9B7", "#173F3B", "#092923"],
            _ => null // Ocean is supplied by the complete Light/Dark resource dictionaries.
        };
        if (accent is not null)
        {
            string[] keys = ["AccentPrimary", "AccentHover", "AccentPressed", "AccentSoft", "TextOnAccent"];
            for (int i = 0; i < keys.Length; i++) package[keys[i]] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(accent[i]));
            package["FocusIndicator"] = package["AccentPrimary"];
            package["StateSelected"] = package["AccentSoft"];
        }
        bool compact = preferences.Density == BentoDensity.Compact;
        package["ControlMinHeight"] = compact ? 32d : 38d;
        package["PaddingControl"] = compact ? new Thickness(10, 5, 10, 5) : new Thickness(14, 8, 14, 8);
        package["PaddingCard"] = new Thickness(compact ? 12 : 18);
        package["PaddingPlate"] = new Thickness(compact ? 10 : 14);
        package["PaddingSelection"] = compact ? new Thickness(10, 5, 32, 5) : new Thickness(14, 8, 38, 8);
        double size = preferences.Typography switch { BentoType.Small => 12d, BentoType.Large => 14d, _ => 13d };
        package["FontSizeBody"] = size;
        package["FontSizeCaption"] = 12d;
        package["FontSizeSection"] = size + 3;
        double radius = preferences.Shape switch { BentoShape.Crisp => 4d, BentoShape.Rounded => 18d, _ => 12d };
        package["CardCornerRadius"] = new CornerRadius(radius);
        package["ControlCornerRadius"] = new CornerRadius(radius / 2);
    }
}
