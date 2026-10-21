using System.Windows;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;
using ProGlassAutomation.ThemeNeumorphism;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.Customization;

/// <summary>
/// Studio-owned card geometry written into the 102-token contract.
/// Future modules inherit rounding by binding CardCornerRadius / ControlCornerRadius.
/// This is not a material engine and does not run on themes that own their radius.
/// </summary>
public enum CardRounding { Crisp, Soft, Rounded, Pill }

public sealed record SharedGeometryState(CardRounding Rounding = CardRounding.Rounded)
{
    public string? Validate() => Enum.IsDefined(Rounding) ? null : "Unsupported card rounding.";
}

public static class SharedGeometry
{
    public static double CardRadius(CardRounding rounding) => rounding switch
    {
        CardRounding.Crisp => 6d,
        CardRounding.Rounded => 18d,
        CardRounding.Pill => 26d,
        _ => 12d
    };

    public static CardRounding FromShape(BentoShape shape) => shape switch
    {
        BentoShape.Crisp => CardRounding.Crisp,
        BentoShape.Rounded => CardRounding.Rounded,
        BentoShape.Pill => CardRounding.Pill,
        _ => CardRounding.Soft
    };

    public static BentoShape ToShape(CardRounding rounding) => rounding switch
    {
        CardRounding.Crisp => BentoShape.Crisp,
        CardRounding.Rounded => BentoShape.Rounded,
        CardRounding.Pill => BentoShape.Pill,
        _ => BentoShape.Soft
    };

    public static void Apply(ResourceDictionary package, SharedGeometryState state)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(state);
        if (state.Validate() is not null) throw new ArgumentException(state.Validate());
        double card = CardRadius(state.Rounding);
        package["CardCornerRadius"] = new CornerRadius(card);
        package["ControlCornerRadius"] = new CornerRadius(card / 2);
        package["ChipCornerRadius"] = new CornerRadius(Math.Max(4d, Math.Round(card / 3d, MidpointRounding.AwayFromZero)));
        package["ChoiceCornerRadius"] = new CornerRadius(Math.Max(6d, Math.Round(card * 0.75, MidpointRounding.AwayFromZero)));
    }

    public static Action<ResourceDictionary, Appearance> Compose(
        string theme,
        BentoCustomizationState bento,
        ClayCustomizationState clay,
        SharedGeometryState geometry,
        NeumorphCustomizationState? neumorph = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        ArgumentNullException.ThrowIfNull(bento);
        ArgumentNullException.ThrowIfNull(clay);
        ArgumentNullException.ThrowIfNull(geometry);
        var neu = neumorph ?? new NeumorphCustomizationState();
        return (package, mode) =>
        {
            if (theme == ThemeService.Bento)
                BentoCustomization.ApplyExtended(package, mode, bento);
            else if (theme == ThemeService.Clay)
                ClayCustomization.Apply(package, mode, clay);
            else if (theme == ThemeService.Neumorphism)
                NeumorphCustomization.Apply(package, mode, neu);
            if (ThemeCatalog.Find(theme).Geometry != ThemeGeometry.SharedTokens)
                return;
            Apply(package, geometry);
            if (theme == ThemeService.Bento && bento.LayoutPreset == BentoLayoutPreset.CompactDense)
            {
                double card = Math.Max(6d, CardRadius(geometry.Rounding) - 2d);
                package["CardCornerRadius"] = new CornerRadius(card);
                package["ControlCornerRadius"] = new CornerRadius(card / 2);
            }
        };
    }
}
