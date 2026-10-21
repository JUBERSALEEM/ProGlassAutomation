using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeNeumorphism;

public enum NeumorphDepth { Soft, Standard, Deep }
public enum NeumorphBlur { Tight, Standard, Diffuse }
public enum NeumorphIntensity { Quiet, Standard, Strong }

public sealed record NeumorphCustomizationState(
    NeumorphDepth Depth = NeumorphDepth.Standard,
    NeumorphBlur Blur = NeumorphBlur.Standard,
    NeumorphIntensity Intensity = NeumorphIntensity.Standard)
{
    public string? Validate()
    {
        if (!Enum.IsDefined(Depth) || !Enum.IsDefined(Blur) || !Enum.IsDefined(Intensity))
            return "Unsupported neumorph customization choice.";
        return null;
    }
}

public sealed record NeumorphNamedPreset(string Name, NeumorphCustomizationState State, Appearance Appearance);

/// <summary>Neumorph-owned depth engine. Never mutates Bento, Clay, or Fluent packages.</summary>
public static class NeumorphCustomization
{
    public static IReadOnlyList<NeumorphNamedPreset> BuiltIn { get; } =
    [
        new("Standard Lift", new NeumorphCustomizationState(), Appearance.Light),
        new("Soft Quiet", new NeumorphCustomizationState(NeumorphDepth.Soft, NeumorphBlur.Tight, NeumorphIntensity.Quiet), Appearance.Light),
        new("Deep Diffuse", new NeumorphCustomizationState(NeumorphDepth.Deep, NeumorphBlur.Diffuse, NeumorphIntensity.Strong), Appearance.Light),
        new("Night Deep", new NeumorphCustomizationState(NeumorphDepth.Deep, NeumorphBlur.Standard, NeumorphIntensity.Strong), Appearance.Dark),
    ];

    public static bool IsBuiltIn(string name) => BuiltIn.Any(p => p.Name == name);

    public static void Apply(ResourceDictionary package, Appearance mode, NeumorphCustomizationState state)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(state);
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());

        double distance = state.Depth switch { NeumorphDepth.Soft => 3d, NeumorphDepth.Deep => 10d, _ => 6d };
        double blur = state.Blur switch { NeumorphBlur.Tight => 8d, NeumorphBlur.Diffuse => 24d, _ => 16d };
        double darkOpacity = state.Intensity switch { NeumorphIntensity.Quiet => 0.22, NeumorphIntensity.Strong => 0.55, _ => 0.4 };
        double lightOpacity = state.Intensity switch { NeumorphIntensity.Quiet => 0.5, NeumorphIntensity.Strong => 0.95, _ => 0.8 };
        double gutter = Math.Ceiling(distance + blur * 0.4);
        SetOwned(package, "NeumorphDistance", distance);
        SetOwned(package, "NeumorphBlur", blur);
        SetOwned(package, "NeumorphLiftDistance", distance + 3d);
        SetOwned(package, "NeumorphDarkOpacity", darkOpacity);
        SetOwned(package, "NeumorphLightOpacity", lightOpacity);
        SetOwned(package, "NeumorphGutter", new Thickness(gutter));
        if (mode == Appearance.Light)
        {
            SetOwned(package, "NeumorphDarkColor", (Color)ColorConverter.ConvertFromString("#B0BAC6"));
            SetOwned(package, "NeumorphLightColor", (Color)ColorConverter.ConvertFromString("#FFFFFF"));
        }
        else
        {
            SetOwned(package, "NeumorphDarkColor", (Color)ColorConverter.ConvertFromString("#161A20"));
            SetOwned(package, "NeumorphLightColor", (Color)ColorConverter.ConvertFromString("#4A5362"));
        }
    }

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
