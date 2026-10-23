using System.Windows;
using System.Windows.Media;

namespace ProGlassAutomation.Customization;

public enum IconKind { Home, Theme, Texts, Colors, About, Marks }

public enum IconMarkStyle
{
    FluentOutline,
    FluentFilled,
    MdlFilled,
    LucideThin,
    MaterialRound,
    Duotone,
    SoftPlate,
    Contrast,
    Compact,
    LineHeavy
}

public sealed record IconCustomizationState(IconMarkStyle Style = IconMarkStyle.FluentOutline)
{
    public string? Validate() => Enum.IsDefined(Style) ? null : "Unsupported icon style.";
}

public sealed record IconStyleCard(IconMarkStyle Style, string Name, string Caption);

/// <summary>
/// Shared icon overlay. Marks are native Path geometry, not PNG and not a tenth theme.
/// </summary>
public static class IconService
{
    public static IconCustomizationState Active { get; private set; } = new();
    public static event EventHandler? Changed;

    public static readonly IReadOnlyList<IconStyleCard> Cards =
    [
        new(IconMarkStyle.FluentOutline, "Fluent outline", "Windows 11 line marks. Default."),
        new(IconMarkStyle.FluentFilled, "Fluent filled", "Solid glyphs, stronger at a glance."),
        new(IconMarkStyle.MdlFilled, "MDL2 filled", "Squarer Windows 10 blocks."),
        new(IconMarkStyle.LucideThin, "Lucide thin", "Editorial 1px stroke."),
        new(IconMarkStyle.MaterialRound, "Material round", "Filled marks on a soft plate."),
        new(IconMarkStyle.Duotone, "Duotone", "Tinted fill plus a crisp outline."),
        new(IconMarkStyle.SoftPlate, "Soft plate", "Outline sitting in a circular chip."),
        new(IconMarkStyle.Contrast, "Contrast", "Larger solid marks."),
        new(IconMarkStyle.Compact, "Compact", "Smaller outline for dense nav."),
        new(IconMarkStyle.LineHeavy, "Line heavy", "Bold stroke, still outline.")
    ];

    public static void Apply(IconCustomizationState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());
        Active = state;
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static IconRecipe Recipe(IconMarkStyle style) => style switch
    {
        IconMarkStyle.FluentFilled => new(true, 0, false, false, 1),
        IconMarkStyle.MdlFilled => new(true, 0, false, false, 1),
        IconMarkStyle.LucideThin => new(false, 1.15, false, false, 1),
        IconMarkStyle.MaterialRound => new(true, 0, false, true, 0.86),
        IconMarkStyle.Duotone => new(true, 1.45, true, false, 1),
        IconMarkStyle.SoftPlate => new(false, 1.5, false, true, 0.78),
        IconMarkStyle.Contrast => new(true, 0, false, false, 1.12),
        IconMarkStyle.Compact => new(false, 1.4, false, false, 0.78),
        IconMarkStyle.LineHeavy => new(false, 2.45, false, false, 0.96),
        _ => new(false, 1.6, false, false, 1)
    };

    public static Geometry Mark(IconKind kind, IconMarkStyle style)
    {
        bool filled = Recipe(style).Filled;
        bool mdl = style == IconMarkStyle.MdlFilled;
        string data = (kind, filled, mdl) switch
        {
            (IconKind.Home, false, _) => "M 3,11 L 12,3.2 L 21,11 V 20.6 H 14.7 V 13.5 H 9.3 V 20.6 H 3 Z",
            (IconKind.Home, true, true) => "M 2.5,11 L 12,2.8 L 21.5,11 V 21 H 13.8 V 13.2 H 10.2 V 21 H 2.5 Z",
            (IconKind.Home, true, _) => "M 3,11 L 12,3.2 L 21,11 V 20.6 H 14.7 V 13.5 H 9.3 V 20.6 H 3 Z",
            (IconKind.Theme, false, _) => "M 4,7 H 20 M 8,7 A 2,2 0 1 1 7.99,7 M 4,12 H 20 M 15,12 A 2,2 0 1 1 14.99,12 M 4,17 H 20 M 11,17 A 2,2 0 1 1 10.99,17",
            (IconKind.Theme, true, _) => "M 3.5,5.7 H 20.5 V 8.3 H 3.5 Z M 3.5,10.7 H 20.5 V 13.3 H 3.5 Z M 3.5,15.7 H 20.5 V 18.3 H 3.5 Z",
            (IconKind.Texts, false, _) => "M 6,19 L 12,5 L 18,19 M 8.5,13.5 H 15.5",
            (IconKind.Texts, true, _) => "M 12,4.6 L 18.4,19.2 H 15.5 L 13.7,14.4 H 10.3 L 8.5,19.2 H 5.6 Z M 10.7,12.6 H 13.3 L 12,9 Z",
            (IconKind.Colors, _, _) => "M 12,3.2 C 12,3.2 5.2,11 5.2,15.2 A 6.8,6.8 0 0 0 18.8,15.2 C 18.8,11 12,3.2 12,3.2 Z",
            (IconKind.About, _, _) => "M 12,3.2 A 8.8,8.8 0 1 1 11.99,3.2 Z M 12,10.2 V 16.6 M 12,7 V 8.2",
            (IconKind.Marks, false, _) => "M 4,4 H 10 V 10 H 4 Z M 14,4 H 20 V 10 H 14 Z M 4,14 H 10 V 20 H 4 Z M 14,14 H 20 V 20 H 14 Z",
            _ => "M 4,4 H 10.4 V 10.4 H 4 Z M 13.6,4 H 20 V 10.4 H 13.6 Z M 4,13.6 H 10.4 V 20 H 4 Z M 13.6,13.6 H 20 V 20 H 13.6 Z"
        };
        var geometry = Geometry.Parse(data);
        geometry.Freeze();
        return geometry;
    }
}

public readonly record struct IconRecipe(bool Filled, double Stroke, bool Duotone, bool Plate, double Scale);
