using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeLiquidGlass;

public enum LiquidFrost { Clear, Standard, Frosted }
public enum LiquidSpecular { Quiet, Standard, Bright }
public enum LiquidTint { Cool, Neutral, Warm }

public sealed record LiquidCustomizationState(
    LiquidFrost Frost = LiquidFrost.Standard,
    LiquidSpecular Specular = LiquidSpecular.Standard,
    LiquidTint Tint = LiquidTint.Cool)
{
    public string? Validate()
    {
        if (!Enum.IsDefined(Frost) || !Enum.IsDefined(Specular) || !Enum.IsDefined(Tint))
            return "Unsupported liquid glass customization choice.";
        return null;
    }
}

public sealed record LiquidNamedPreset(string Name, LiquidCustomizationState State, Appearance Appearance);

/// <summary>Liquid-owned frost engine. Never mutates Bento, Clay, Fluent or Neumorph packages.</summary>
public static class LiquidCustomization
{
    public static IReadOnlyList<LiquidNamedPreset> BuiltIn { get; } =
    [
        new("Standard Glass", new LiquidCustomizationState(), Appearance.Light),
        new("Clear Cool", new LiquidCustomizationState(LiquidFrost.Clear, LiquidSpecular.Bright, LiquidTint.Cool), Appearance.Light),
        new("Frosted Neutral", new LiquidCustomizationState(LiquidFrost.Frosted, LiquidSpecular.Quiet, LiquidTint.Neutral), Appearance.Light),
        new("Night Tint", new LiquidCustomizationState(LiquidFrost.Standard, LiquidSpecular.Bright, LiquidTint.Cool), Appearance.Dark),
    ];

    public static bool IsBuiltIn(string name) => BuiltIn.Any(p => p.Name == name);

    public static void Apply(ResourceDictionary package, Appearance mode, LiquidCustomizationState state)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(state);
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());

        double frost = state.Frost switch { LiquidFrost.Clear => 0.45, LiquidFrost.Frosted => 0.92, _ => mode == Appearance.Light ? 0.72 : 0.70 };
        double lift = state.Frost switch { LiquidFrost.Clear => 10d, LiquidFrost.Frosted => 4d, _ => mode == Appearance.Light ? 8d : 10d };
        double blur = state.Frost switch { LiquidFrost.Clear => 28d, LiquidFrost.Frosted => 16d, _ => mode == Appearance.Light ? 24d : 28d };
        double liftOpacity = state.Frost switch { LiquidFrost.Clear => 0.28, LiquidFrost.Frosted => 0.12, _ => mode == Appearance.Light ? 0.22 : 0.36 };
        byte specular = state.Specular switch { LiquidSpecular.Quiet => (byte)(mode == Appearance.Light ? 0x99 : 0x40), LiquidSpecular.Bright => 0xFF, _ => (byte)(mode == Appearance.Light ? 0xE6 : 0x73) };
        byte hover = state.Specular switch { LiquidSpecular.Quiet => (byte)(mode == Appearance.Light ? 0xCC : 0x66), _ => 0xFF };

        Color face = Face(mode, state.Tint, frost);
        SetOwned(package, "LiquidFrostOpacity", frost);
        SetOwned(package, "LiquidLiftDepth", lift);
        SetOwned(package, "LiquidLiftBlur", blur);
        SetOwned(package, "LiquidLiftOpacity", liftOpacity);
        SetOwned(package, "LiquidFaceBrush", new SolidColorBrush(face));
        SetOwned(package, "LiquidSpecularBrush", new SolidColorBrush(Color.FromArgb(specular, 255, 255, 255)));
        SetOwned(package, "LiquidSpecularHoverBrush", new SolidColorBrush(Color.FromArgb(hover, 255, 255, 255)));
        SetOwned(package, "LiquidReduceTransparency", false);
    }

    private static Color Face(Appearance mode, LiquidTint tint, double frost)
    {
        byte alpha = (byte)Math.Clamp((int)Math.Round(frost * 255), 1, 255);
        Color baseFace = mode == Appearance.Light
            ? tint switch { LiquidTint.Warm => Color.FromRgb(255, 244, 232), LiquidTint.Neutral => Color.FromRgb(242, 244, 246), _ => Colors.White }
            : tint switch { LiquidTint.Warm => Color.FromRgb(40, 28, 22), LiquidTint.Neutral => Color.FromRgb(32, 36, 40), _ => Color.FromRgb(26, 44, 64) };
        return Color.FromArgb(alpha, baseFace.R, baseFace.G, baseFace.B);
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
