using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.Customization;

/// <summary>
/// Shared accent overlay. A plate is one hue with many named values (50–800).
/// Role hex overrides let the user keep an exact value. Not a material engine.
/// </summary>
public sealed record ColorStop(string Name, string Hex);

public sealed record ColorPlateDefinition(
    string Name,
    IReadOnlyList<ColorStop> Stops,
    IReadOnlyDictionary<string, string> LightRoles,
    IReadOnlyDictionary<string, string> DarkRoles);

public sealed record ColorPlateCard(
    string Name,
    IReadOnlyList<ColorStop> Stops,
    IReadOnlyList<Brush> Swatches,
    Brush Sample,
    string Summary);

public sealed record ColorPlateState(
    string Plate = "Ocean",
    Dictionary<string, string>? RoleHex = null,
    string CardTint = "#C4B0D4",
    int CardSaturation = 0)
{
    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Plate) || Plate.Length > 40)
            return "Color plate name is required.";
        if (!ColorPlateService.Exists(Plate))
            return "Unknown color plate.";
        foreach (var pair in RoleHex ?? [])
        {
            if (!ColorPlateService.Roles.Contains(pair.Key))
                return "Unknown color role.";
            if (ColorPlateService.ParseHex(pair.Value) is null)
                return "Color values must be #RRGGBB.";
        }
        if (ColorPlateService.ParseHex(CardTint) is null)
            return "Card tint must be #RRGGBB.";
        if (CardSaturation is < 0 or > 100)
            return "Card saturation must be 0–100.";
        return null;
    }
}

public static class ColorPlateService
{
    public static readonly IReadOnlyList<string> Roles =
    [
        "AccentPrimary", "AccentHover", "AccentPressed", "AccentSoft", "AccentSecondary",
        "FocusIndicator", "ChartSeries1", "ChartSeries2", "ChartSeries3", "ChartSeries4"
    ];

    public static readonly IReadOnlyList<string> CardRoles =
    [
        "SurfacePrimary", "SurfaceSecondary", "SurfaceElevated"
    ];

    public static readonly IReadOnlyList<string> CardTintStrengths = ["Off", "Subtle", "Balanced", "Vivid"];

    public static IReadOnlyList<ColorPlateDefinition> BuiltIn { get; } =
    [
        Plate("Ocean",
            ("50", "#E8F0FE"), ("100", "#C5D7F7"), ("200", "#8EB0EE"),
            ("400", "#4D82D6"), ("600", "#2F5AA8"), ("800", "#1E3C72")),
        Plate("Forest",
            ("50", "#E6F4EC"), ("100", "#C4E4D2"), ("200", "#86C8A4"),
            ("400", "#3D9A6A"), ("600", "#1F6B4A"), ("800", "#144632")),
        Plate("Sunset",
            ("50", "#FDECE8"), ("100", "#F8C9BE"), ("200", "#F08A72"),
            ("400", "#E05A3A"), ("600", "#C43C1E"), ("800", "#8A2410")),
        Plate("Slate",
            ("50", "#EEF1F5"), ("100", "#D5DCE6"), ("200", "#A9B4C4"),
            ("400", "#6B7789"), ("600", "#445060"), ("800", "#1B2430")),
        Plate("Violet",
            ("50", "#F3EEFA"), ("100", "#DDD0F2"), ("200", "#B89EE3"),
            ("400", "#8B63C7"), ("600", "#6B3FA0"), ("800", "#452868")),
        Plate("Sand",
            ("50", "#F8F1E8"), ("100", "#EAD9C3"), ("200", "#D4B48A"),
            ("400", "#C4954A"), ("600", "#A67C3A"), ("800", "#6E4F22")),
        new ColorPlateDefinition(
            "Theme",
            [new ColorStop("theme", "#888888")],
            new Dictionary<string, string>(),
            new Dictionary<string, string>()),
    ];

    public static bool IsThemePlate(string? name) =>
        string.Equals(name, "Theme", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<ColorPlateCard> Cards { get; } = BuiltIn.Select(CardOf).ToArray();

    public static bool Exists(string name) =>
        BuiltIn.Any(plate => string.Equals(plate.Name, name, StringComparison.OrdinalIgnoreCase));

    public static ColorPlateDefinition Find(string name) =>
        BuiltIn.FirstOrDefault(plate => string.Equals(plate.Name, name, StringComparison.OrdinalIgnoreCase))
        ?? BuiltIn[0];

    public static Color? ParseHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        string value = hex.Trim();
        if (value[0] != '#') value = "#" + value;
        try
        {
            object converted = ColorConverter.ConvertFromString(value)
                ?? throw new FormatException();
            return (Color)converted;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            return null;
        }
    }

    public static string ToHex(Color color) =>
        $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    public static void Apply(ResourceDictionary package, Appearance mode, ColorPlateState state, string? theme = null)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(state);
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());
        if (IsThemePlate(state.Plate))
        {
            if (!string.IsNullOrWhiteSpace(theme))
                RestoreRoles(package, theme, mode);
            ApplyCardTint(package, mode, state, theme);
            return;
        }
        var plate = Find(state.Plate);
        var roles = mode == Appearance.Dark ? plate.DarkRoles : plate.LightRoles;
        var stops = plate.Stops.ToDictionary(stop => stop.Name, stop => stop.Hex, StringComparer.OrdinalIgnoreCase);
        foreach (string role in Roles)
        {
            string hex;
            if (state.RoleHex is not null && state.RoleHex.TryGetValue(role, out string? over) && ParseHex(over) is not null)
                hex = over.StartsWith('#') ? over : "#" + over;
            else if (roles.TryGetValue(role, out string? stop) && stops.TryGetValue(stop, out string? mapped))
                hex = mapped;
            else
                continue;
            if (ParseHex(hex) is not Color color) continue;
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            Set(package, role, brush);
        }
        ApplyCardTint(package, mode, state, theme);
    }

    public static string StrengthName(int saturation) => saturation switch
    {
        <= 0 => "Off",
        <= 22 => "Subtle",
        <= 36 => "Balanced",
        _ => "Vivid"
    };

    public static int StrengthValue(string? name) => name switch
    {
        "Subtle" => 18,
        "Balanced" => 32,
        "Vivid" => 48,
        _ => 0
    };

    public static IReadOnlyList<ColorStop> StopsOf(string plate) => Find(plate).Stops;

    public static string ResolvedHex(ColorPlateState state, Appearance mode, string role)
    {
        if (state.RoleHex is not null && state.RoleHex.TryGetValue(role, out string? over) && ParseHex(over) is Color)
            return over.StartsWith('#') ? over : "#" + over;
        var plate = Find(state.Plate);
        var roles = mode == Appearance.Dark ? plate.DarkRoles : plate.LightRoles;
        var stops = plate.Stops.ToDictionary(stop => stop.Name, stop => stop.Hex, StringComparer.OrdinalIgnoreCase);
        if (roles.TryGetValue(role, out string? stop) && stops.TryGetValue(stop, out string? hex))
            return hex;
        return "#000000";
    }

    private static ColorPlateDefinition Plate(string name, params (string Stop, string Hex)[] stops)
    {
        var list = stops.Select(item => new ColorStop(item.Stop, item.Hex)).ToArray();
        return new ColorPlateDefinition(
            name,
            list,
            new Dictionary<string, string>
            {
                ["AccentPrimary"] = "600",
                ["AccentHover"] = "800",
                ["AccentPressed"] = "800",
                ["AccentSoft"] = "100",
                ["AccentSecondary"] = "400",
                ["FocusIndicator"] = "600",
                ["ChartSeries1"] = "600",
                ["ChartSeries2"] = "400",
                ["ChartSeries3"] = "800",
                ["ChartSeries4"] = "200",
            },
            new Dictionary<string, string>
            {
                ["AccentPrimary"] = "400",
                ["AccentHover"] = "200",
                ["AccentPressed"] = "200",
                ["AccentSoft"] = "800",
                ["AccentSecondary"] = "200",
                ["FocusIndicator"] = "400",
                ["ChartSeries1"] = "400",
                ["ChartSeries2"] = "200",
                ["ChartSeries3"] = "50",
                ["ChartSeries4"] = "600",
            });
    }

    private static void RestoreRoles(ResourceDictionary package, string theme, Appearance mode)
    {
        var fresh = ThemeService.Load(mode, theme);
        foreach (string role in Roles)
        {
            if (fresh[role] is not SolidColorBrush source) continue;
            var brush = new SolidColorBrush(source.Color);
            brush.Freeze();
            Set(package, role, brush);
        }
    }

    private static ColorPlateCard CardOf(ColorPlateDefinition plate)
    {
        if (IsThemePlate(plate.Name))
        {
            var identity = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88));
            identity.Freeze();
            return new ColorPlateCard(plate.Name, plate.Stops, [identity], identity, "Active theme accent. No overlay.");
        }
        var swatches = new List<Brush>(plate.Stops.Count);
        foreach (var stop in plate.Stops)
        {
            var brush = new SolidColorBrush(ParseHex(stop.Hex) ?? Color.FromRgb(0x44, 0x50, 0x60));
            brush.Freeze();
            swatches.Add(brush);
        }
        string sampleHex = plate.Stops.FirstOrDefault(stop => stop.Name == "600")?.Hex ?? plate.Stops[0].Hex;
        var sample = new SolidColorBrush(ParseHex(sampleHex) ?? Color.FromRgb(0x2F, 0x5A, 0xA8));
        sample.Freeze();
        return new ColorPlateCard(
            plate.Name,
            plate.Stops,
            swatches,
            sample,
            string.Join("  ·  ", plate.Stops.Select(stop => stop.Name + " " + stop.Hex)));
    }

    private static void ApplyCardTint(ResourceDictionary package, Appearance mode, ColorPlateState state, string? theme)
    {
        if (string.IsNullOrWhiteSpace(theme)) return;
        var fresh = ThemeService.Load(mode, theme);
        Color? tint = ParseHex(state.CardTint);
        double amount = Math.Clamp(state.CardSaturation, 0, 100) / 100d;
        foreach (string role in CardRoles)
        {
            if (fresh[role] is not SolidColorBrush source) continue;
            Color mixed = amount <= 0 || tint is null
                ? source.Color
                : MixKeepLuma(source.Color, tint.Value, amount);
            var brush = new SolidColorBrush(mixed);
            brush.Freeze();
            Set(package, role, brush);
        }
    }

    private static Color MixKeepLuma(Color original, Color tint, double amount)
    {
        var (_, s0, l0) = ToHsl(original);
        var (h1, s1, _) = ToHsl(tint);
        double s = Math.Min(0.52, s0 * (1 - amount) + s1 * amount);
        return FromHsl(h1, s, l0);
    }

    private static (double H, double S, double L) ToHsl(Color color)
    {
        double r = color.R / 255d, g = color.G / 255d, b = color.B / 255d;
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double l = (max + min) / 2d;
        if (Math.Abs(max - min) < 0.0001) return (0, 0, l);
        double d = max - min;
        double s = l > 0.5 ? d / (2d - max - min) : d / (max + min);
        double h = max == r ? (g - b) / d + (g < b ? 6 : 0)
            : max == g ? (b - r) / d + 2
            : (r - g) / d + 4;
        return (h / 6d, s, l);
    }

    private static Color FromHsl(double h, double s, double l)
    {
        s = Math.Clamp(s, 0, 1);
        l = Math.Clamp(l, 0, 1);
        byte Unit(double x) => (byte)Math.Round(Math.Clamp(x, 0, 1) * 255);
        if (s <= 0)
        {
            byte gray = Unit(l);
            return Color.FromRgb(gray, gray, gray);
        }
        double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        double p = 2 * l - q;
        return Color.FromRgb(Unit(Hue(p, q, h + 1d / 3d)), Unit(Hue(p, q, h)), Unit(Hue(p, q, h - 1d / 3d)));
    }

    private static double Hue(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1d / 6d) return p + (q - p) * 6 * t;
        if (t < 0.5) return q;
        if (t < 2d / 3d) return p + (q - p) * (2d / 3d - t) * 6;
        return p;
    }

    private static void Set(ResourceDictionary package, string key, object value)
    {
        if (Replace(package, key, value)) return;
        package[key] = value;
    }

    private static bool Replace(ResourceDictionary dictionary, string key, object value)
    {
        foreach (object existing in dictionary.Keys)
        {
            if (!Equals(existing, key)) continue;
            dictionary[key] = value;
            return true;
        }
        foreach (ResourceDictionary child in dictionary.MergedDictionaries)
            if (Replace(child, key, value)) return true;
        return false;
    }
}
