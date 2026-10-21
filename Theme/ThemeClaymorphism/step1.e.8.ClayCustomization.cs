using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeClaymorphism;

public enum ClayAccent { Rose, Berry, Sage }
public enum ClayVolume { Soft, Plush, Deep }
public enum ClayPlumpness { Modest, Standard, Puffy }
public enum ClayDensity { Compact, Comfortable }
public enum ClayCanvas { Pastel, Warm, Cool }
public enum ClayType { Small, Standard, Large }
public enum ClaySpacing { Compact, Comfortable, Spacious }
public enum ClayTextContrast { Standard, High }
public enum ClaySheen { Quiet, Standard, Gloss }
public enum ClayWeight { Regular, Medium, Semibold }

public sealed record ClayCustomizationState(
    ClayAccent Accent = ClayAccent.Rose,
    ClayVolume Volume = ClayVolume.Plush,
    ClayPlumpness Plumpness = ClayPlumpness.Standard,
    ClayDensity Density = ClayDensity.Comfortable,
    ClayCanvas Canvas = ClayCanvas.Pastel,
    ClayType Typography = ClayType.Standard,
    ClaySpacing Spacing = ClaySpacing.Comfortable,
    ClayTextContrast TextContrast = ClayTextContrast.Standard,
    ClaySheen Sheen = ClaySheen.Standard,
    ClayWeight Weight = ClayWeight.Regular)
{
    public string? Validate()
    {
        if (!Enum.IsDefined(Accent) || !Enum.IsDefined(Volume) || !Enum.IsDefined(Plumpness) ||
            !Enum.IsDefined(Density) || !Enum.IsDefined(Canvas) || !Enum.IsDefined(Typography) ||
            !Enum.IsDefined(Spacing) || !Enum.IsDefined(TextContrast) || !Enum.IsDefined(Sheen) ||
            !Enum.IsDefined(Weight))
            return "Unsupported clay customization choice.";
        return null;
    }
}

public sealed record ClayNamedPreset(string Name, ClayCustomizationState State, Appearance Appearance);

/// <summary>Clay-owned customization. Never mutates Bento packages or Bento state.</summary>
public static class ClayCustomization
{
    public static IReadOnlyList<ClayNamedPreset> BuiltIn { get; } =
    [
        new("Rose Plush", new ClayCustomizationState(), Appearance.Light),
        new("Berry Puffy", new ClayCustomizationState(ClayAccent.Berry, ClayVolume.Deep, ClayPlumpness.Puffy, ClayDensity.Comfortable, ClayCanvas.Pastel, ClayType.Large, ClaySpacing.Spacious, ClayTextContrast.Standard, ClaySheen.Gloss), Appearance.Light),
        new("Sage Soft", new ClayCustomizationState(ClayAccent.Sage, ClayVolume.Soft, ClayPlumpness.Modest, ClayDensity.Compact, ClayCanvas.Cool, ClayType.Small, ClaySpacing.Compact, ClayTextContrast.High, ClaySheen.Quiet), Appearance.Light),
        new("Night Berry", new ClayCustomizationState(ClayAccent.Berry, ClayVolume.Deep, ClayPlumpness.Standard, ClayDensity.Comfortable, ClayCanvas.Pastel, ClayType.Standard, ClaySpacing.Comfortable, ClayTextContrast.High, ClaySheen.Gloss), Appearance.Dark),
    ];

    public static bool IsBuiltIn(string name) => BuiltIn.Any(p => p.Name == name);

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
                package[keys[i]] = Brush(accent[i]);
            package["FocusIndicator"] = package["AccentPrimary"];
            package["StateSelected"] = package["AccentSoft"];
            package["ChartSeries1"] = package["AccentPrimary"];
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
            package["ApplicationBackground"] = Brush(canvas);

        if (state.TextContrast == ClayTextContrast.High)
        {
            package["TextPrimary"] = Brush(mode == Appearance.Light ? "#10143A" : "#FFFFFF");
            package["TextMuted"] = Brush(mode == Appearance.Light ? "#3A4270" : "#D2CCEE");
        }

        double radius = state.Plumpness switch { ClayPlumpness.Modest => 20d, ClayPlumpness.Puffy => 40d, _ => 32d };
        package["CardCornerRadius"] = new CornerRadius(radius);
        package["ControlCornerRadius"] = new CornerRadius(state.Plumpness switch { ClayPlumpness.Modest => 16d, ClayPlumpness.Puffy => 32d, _ => 24d });
        package["ChipCornerRadius"] = new CornerRadius(state.Plumpness switch { ClayPlumpness.Modest => 12d, ClayPlumpness.Puffy => 20d, _ => 16d });
        package["ChoiceCornerRadius"] = new CornerRadius(state.Plumpness switch { ClayPlumpness.Modest => 14d, ClayPlumpness.Puffy => 24d, _ => 20d });
        package["ControlMinHeight"] = state.Density == ClayDensity.Compact ? 36d : 40d;
        package["PaddingControl"] = state.Density == ClayDensity.Compact ? new Thickness(12, 8, 12, 8) : new Thickness(16, 10, 16, 10);

        double spacing = state.Spacing switch { ClaySpacing.Compact => 0.85, ClaySpacing.Spacious => 1.2, _ => 1 };
        package["SpacingSmall"] = 6 * spacing;
        package["SpacingMedium"] = 10 * spacing;
        package["SpacingLarge"] = 18 * spacing;
        package["SpacingSection"] = 28 * spacing;
        package["PaddingCard"] = new Thickness(state.Spacing == ClaySpacing.Compact ? 14 : state.Spacing == ClaySpacing.Spacious ? 22 : 18);
        package["PaddingPlate"] = new Thickness(state.Spacing == ClaySpacing.Compact ? 10 : state.Spacing == ClaySpacing.Spacious ? 16 : 14);
        package["BentoGutter"] = 14 * spacing;
        package["BentoRowHeight"] = (state.Density == ClayDensity.Compact ? 172d : 184d) * spacing;

        double size = state.Typography switch { ClayType.Small => 12d, ClayType.Large => 14d, _ => 13d };
        package["FontSizeBody"] = size;
        package["FontSizeCaption"] = 12d;
        package["FontSizeSection"] = size + 3;
        package["FontWeightHeading"] = state.Weight switch { ClayWeight.Medium => FontWeights.Medium, ClayWeight.Semibold => FontWeights.SemiBold, _ => FontWeights.Bold };
        package["FontWeightBody"] = state.Weight switch { ClayWeight.Medium => FontWeights.Medium, ClayWeight.Semibold => FontWeights.SemiBold, _ => FontWeights.Normal };

        string drop = mode == Appearance.Light
            ? state.Volume switch { ClayVolume.Soft => "33", ClayVolume.Deep => "99", _ => "66" }
            : state.Volume switch { ClayVolume.Soft => "66", ClayVolume.Deep => "CC", _ => "99" };
        if (mode == Appearance.Light)
        {
            string shade = state.Volume switch { ClayVolume.Soft => "22", ClayVolume.Deep => "55", _ => "33" };
            Tint(package, "ClayDropBrush", drop, "5B4A9A");
            Tint(package, "ClayInsetShadeBrush", shade, "5B4A9A");
            Tint(package, "ClayDropPositive", drop, "146C45");
            Tint(package, "ClayShadePositive", shade, "146C45");
            Tint(package, "ClayDropFeature", drop, "5B4A9A");
            Tint(package, "ClayShadeFeature", shade, "5B4A9A");
            Tint(package, "ClayDropInformation", drop, "2158A8");
            Tint(package, "ClayShadeInformation", shade, "2158A8");
            Tint(package, "ClayDropAttention", drop, "C27A00");
            Tint(package, "ClayShadeAttention", shade, "C27A00");
            Tint(package, "ClayDropAccent", drop, "C81E1E");
            Tint(package, "ClayShadeAccent", shade, "C81E1E");
            Tint(package, "ClayDropHero", drop, "7B5BB5");
            Tint(package, "ClayShadeHero", shade, "7B5BB5");
        }
        else
        {
            string darkShade = state.Volume switch { ClayVolume.Soft => "44", ClayVolume.Deep => "88", _ => "66" };
            Tint(package, "ClayDropBrush", drop, "0A0818");
            Tint(package, "ClayInsetShadeBrush", darkShade, "000000");
            Tint(package, "ClayDropPositive", drop, "1A3A30");
            Tint(package, "ClayShadePositive", darkShade, "1A3A30");
            Tint(package, "ClayDropFeature", drop, "2E2450");
            Tint(package, "ClayShadeFeature", darkShade, "2E2450");
            Tint(package, "ClayDropInformation", drop, "1E2A48");
            Tint(package, "ClayShadeInformation", darkShade, "1E2A48");
            Tint(package, "ClayDropAttention", drop, "3A2A20");
            Tint(package, "ClayShadeAttention", darkShade, "3A2A20");
            Tint(package, "ClayDropAccent", drop, "FF8A80");
            Tint(package, "ClayShadeAccent", darkShade, "FF8A80");
            Tint(package, "ClayDropHero", drop, "4A3A80");
            Tint(package, "ClayShadeHero", darkShade, "4A3A80");
        }

        SetOwned(package, "ClayInsetLightBrush", Brush(mode == Appearance.Light
            ? state.Sheen switch { ClaySheen.Quiet => "#66FFFFFF", ClaySheen.Gloss => "#FFFFFFFF", _ => "#E6FFFFFF" }
            : state.Sheen switch { ClaySheen.Quiet => "#22FFFFFF", ClaySheen.Gloss => "#88FFFFFF", _ => "#55FFFFFF" }));
    }

    private static SolidColorBrush Brush(string color) =>
        new((Color)ColorConverter.ConvertFromString(color));

    private static void Tint(ResourceDictionary package, string key, string alpha, string rgb) =>
        SetOwned(package, key, Brush($"#{alpha}{rgb}"));

    /// <summary>
    /// Replace a key in the dictionary that already owns it. Assigning extras onto the
    /// package root duplicates merged keys and Clay fails validation on switch.
    /// </summary>
    private static void SetOwned(ResourceDictionary package, string key, object value)
    {
        if (ReplaceOwn(package, key, value)) return;
        package[key] = value;
    }

    private static bool ReplaceOwn(ResourceDictionary dictionary, string key, object value)
    {
        foreach (object existing in dictionary.Keys)
        {
            if (!Equals(existing, key)) continue;
            dictionary[key] = value;
            return true;
        }
        foreach (ResourceDictionary child in dictionary.MergedDictionaries)
            if (ReplaceOwn(child, key, value)) return true;
        return false;
    }
}
