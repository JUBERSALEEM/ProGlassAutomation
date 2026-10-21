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

        double frost = state.Frost switch { LiquidFrost.Clear => 0.22, LiquidFrost.Frosted => 0.62, _ => mode == Appearance.Light ? 0.40 : 0.28 };
        double lift = state.Frost switch { LiquidFrost.Clear => 12d, LiquidFrost.Frosted => 6d, _ => mode == Appearance.Light ? 10d : 12d };
        double blur = state.Frost switch { LiquidFrost.Clear => 32d, LiquidFrost.Frosted => 18d, _ => mode == Appearance.Light ? 28d : 32d };
        double liftOpacity = state.Frost switch { LiquidFrost.Clear => 0.36, LiquidFrost.Frosted => 0.18, _ => mode == Appearance.Light ? 0.32 : 0.48 };
        byte sheen = state.Specular switch { LiquidSpecular.Quiet => (byte)(mode == Appearance.Light ? 0x99 : 0x59), LiquidSpecular.Bright => 0xFF, _ => (byte)(mode == Appearance.Light ? 0xE6 : 0x99) };
        byte hover = state.Specular switch { LiquidSpecular.Quiet => (byte)(mode == Appearance.Light ? 0xCC : 0x73), _ => 0xFF };
        Color rgb = TintRgb(mode, state.Tint);

        SetOwned(package, "LiquidFrostOpacity", frost);
        SetOwned(package, "LiquidLiftDepth", lift);
        SetOwned(package, "LiquidLiftBlur", blur);
        SetOwned(package, "LiquidLiftOpacity", liftOpacity);
        SetOwned(package, "LiquidFaceBrush", FaceGradient(rgb, frost));
        SetOwned(package, "LiquidLensBrush", LensGradient(mode, frost));
        SetOwned(package, "LiquidSpecularSheenBrush", SheenGradient(sheen));
        SetOwned(package, "LiquidSpecularBrush", new SolidColorBrush(Color.FromArgb(sheen, 255, 255, 255)));
        SetOwned(package, "LiquidSpecularHoverBrush", SheenGradient(hover));
        SetOwned(package, "LiquidReduceTransparency", false);
    }

    private static Color TintRgb(Appearance mode, LiquidTint tint) => mode == Appearance.Light
        ? tint switch { LiquidTint.Warm => Color.FromRgb(255, 244, 232), LiquidTint.Neutral => Color.FromRgb(242, 244, 246), _ => Colors.White }
        : tint switch { LiquidTint.Warm => Color.FromRgb(255, 236, 220), LiquidTint.Neutral => Color.FromRgb(236, 240, 244), _ => Colors.White };

    private static LinearGradientBrush FaceGradient(Color rgb, double frost)
    {
        byte A(double amount) => (byte)Math.Clamp((int)Math.Round(amount * 255), 1, 255);
        var brush = new LinearGradientBrush { StartPoint = new Point(0.18, 0), EndPoint = new Point(0.82, 1) };
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(A(Math.Min(1, frost + 0.25)), rgb.R, rgb.G, rgb.B), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(A(frost), rgb.R, rgb.G, rgb.B), 0.42));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(A(frost * 0.55), rgb.R, rgb.G, rgb.B), 1));
        return brush;
    }

    private static LinearGradientBrush LensGradient(Appearance mode, double frost)
    {
        byte top = (byte)Math.Clamp((int)Math.Round((frost + 0.15) * 255), 1, 255);
        byte bottom = (byte)(mode == Appearance.Light ? 0x33 : 0x66);
        var brush = new LinearGradientBrush { StartPoint = new Point(0.15, 0), EndPoint = new Point(0.85, 1) };
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(top, 255, 255, 255), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.38));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(bottom, 10, 54, 88), 1));
        return brush;
    }

    private static LinearGradientBrush SheenGradient(byte alpha)
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alpha, 255, 255, 255), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 1));
        return brush;
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
