using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeClaymorphism;

public enum ClayAccent { Rose, Berry, Sage }
public enum ClayVolume { Soft, Plush, Deep }
public enum ClayPlumpness { Modest, Standard, Puffy }
public enum ClayDensity { Compact, Comfortable }
public enum ClayCanvas { Pastel, Warm, Cool }

public sealed record ClayCustomizationState(
    ClayAccent Accent = ClayAccent.Rose,
    ClayVolume Volume = ClayVolume.Plush,
    ClayPlumpness Plumpness = ClayPlumpness.Standard,
    ClayDensity Density = ClayDensity.Comfortable,
    ClayCanvas Canvas = ClayCanvas.Pastel)
{
    public string? Validate()
    {
        if (!Enum.IsDefined(Accent) || !Enum.IsDefined(Volume) || !Enum.IsDefined(Plumpness) ||
            !Enum.IsDefined(Density) || !Enum.IsDefined(Canvas))
            return "Unsupported clay customization choice.";
        return null;
    }
}

/// <summary>Clay-owned customization. Never mutates Bento packages or Bento state.</summary>
public static class ClayCustomization
{
    public static void Apply(ResourceDictionary package, Appearance mode, ClayCustomizationState state)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(state);
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());

        string[]? accent = (state.Accent, mode) switch
        {
            (ClayAccent.Berry, Appearance.Light) => ["#5B3CC4", "#4C30A8", "#3D268C", "#EDE4FF", "#FFFFFF"],
            (ClayAccent.Berry, Appearance.Dark) => ["#C4B5FF", "#D4C8FF", "#A898E8", "#2E2450", "#1A1730"],
            (ClayAccent.Sage, Appearance.Light) => ["#146C45", "#0F5738", "#0C452C", "#D4F5E4", "#FFFFFF"],
            (ClayAccent.Sage, Appearance.Dark) => ["#5EE0C0", "#7AE8CC", "#4BC8A8", "#1A3A30", "#1A1730"],
            _ => null
        };
        if (accent is not null)
        {
            string[] keys = ["AccentPrimary", "AccentHover", "AccentPressed", "AccentSoft", "TextOnAccent"];
            for (int i = 0; i < keys.Length; i++)
                package[keys[i]] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(accent[i]));
            package["FocusIndicator"] = package["AccentPrimary"];
            package["StateSelected"] = package["AccentSoft"];
        }

        string? canvas = (state.Canvas, mode) switch
        {
            (ClayCanvas.Warm, Appearance.Light) => "#F7EFE6",
            (ClayCanvas.Cool, Appearance.Light) => "#E8EEF9",
            (ClayCanvas.Warm, Appearance.Dark) => "#241820",
            (ClayCanvas.Cool, Appearance.Dark) => "#141A30",
            _ => null
        };
        if (canvas is not null)
            package["ApplicationBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(canvas));

        double radius = state.Plumpness switch { ClayPlumpness.Modest => 20d, ClayPlumpness.Puffy => 40d, _ => 32d };
        package["CardCornerRadius"] = new CornerRadius(radius);
        package["ControlCornerRadius"] = new CornerRadius(Math.Max(12, radius - 8));
        package["ChipCornerRadius"] = new CornerRadius(Math.Max(8, radius / 2));
        package["ControlMinHeight"] = state.Density == ClayDensity.Compact ? 36d : 40d;
        package["PaddingControl"] = state.Density == ClayDensity.Compact ? new Thickness(12, 8) : new Thickness(16, 10);

        string drop = state.Volume switch { ClayVolume.Soft => "33", ClayVolume.Deep => "99", _ => "66" };
        string shade = state.Volume switch { ClayVolume.Soft => "22", ClayVolume.Deep => "55", _ => "33" };
        string hue = mode == Appearance.Light ? "5B4A9A" : "0A0818";
        package["ClayDropBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{drop}{hue}"));
        package["ClayInsetShadeBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{shade}{hue}"));
    }
}
