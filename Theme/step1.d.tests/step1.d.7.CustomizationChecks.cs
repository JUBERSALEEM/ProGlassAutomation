using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;

namespace ProGlassAutomation.ThemeTests;

internal static class CustomizationChecks
{
    public static void Run()
    {
        int combinations = 0;
        foreach (var mode in Enum.GetValues<Appearance>())
        foreach (var accent in Enum.GetValues<BentoAccent>())
        foreach (var density in Enum.GetValues<BentoDensity>())
        foreach (var shape in Enum.GetValues<BentoShape>())
        foreach (var type in Enum.GetValues<BentoType>())
        {
            var preferences = new BentoPreferences(accent, density, shape, type);
            var package = ThemeService.Load(mode, configure: (candidate, appearance) => BentoCustomization.Apply(candidate, appearance, preferences));
            ResourceValidation.Validate(package);
            ContractChecks.Contrast(package, "TextOnAccent", "AccentPrimary", 4.5);
            ContractChecks.Contrast(package, "AccentPrimary", "AccentSoft", 4.5);
            if (package[typeof(ProGlassAutomation.Analytics.BentoChart)] is not Style chartStyle || !chartStyle.IsSealed)
                throw new Exception("Chart adapter style must be sealed.");
            combinations++;
        }
        var resources = new ResourceDictionary();
        var service = new ThemeService();
        service.Initialize(resources);
        service.Apply(ThemeService.Bento, Appearance.Light,
            (candidate, appearance) => BentoCustomization.Apply(candidate, appearance, new BentoPreferences(BentoAccent.Teal)));
        service.Apply(ThemeService.Bento, Appearance.Dark);
        if (((SolidColorBrush)service.GetToken("AccentPrimary")).Color != (Color)ColorConverter.ConvertFromString("#75DFCD"))
            throw new Exception("Mode changes discarded the active theme customization.");
        object before = service.GetToken("SurfacePrimary");
        ContractChecks.ExpectFailure(() => service.Apply(ThemeService.Bento, Appearance.Light,
            (candidate, _) => candidate["SpacingMedium"] = double.NaN), "invalid customization");
        if (service.ActiveAppearance != Appearance.Dark || !ReferenceEquals(before, service.GetToken("SurfacePrimary")))
            throw new Exception("Rejected customization changed the active package.");
        ContractChecks.ExpectFailure(() => ThemeService.Load(Appearance.Light, configure: (candidate, appearance) =>
            BentoCustomization.Apply(candidate, appearance, new BentoPreferences((BentoAccent)999))), "invalid preset");
        int notified = 0;
        PropertyChangedEventHandler failing = (_, _) => throw new InvalidOperationException("Deliberate consumer failure");
        service.PropertyChanged += failing;
        service.PropertyChanged += (_, _) => notified++;
        try { service.Apply(ThemeService.Bento, Appearance.Light); throw new Exception("Notification failure was not reported."); }
        catch (ThemeNotificationException)
        {
            if (service.ActiveAppearance != Appearance.Light || notified != 2)
                throw new Exception("Notification failure hid activation or prevented other consumers from refreshing.");
        }
        finally { service.PropertyChanged -= failing; }
        int clayCombinations = 0;
        foreach (var mode in Enum.GetValues<Appearance>())
        foreach (var accent in Enum.GetValues<ClayAccent>())
        foreach (var volume in Enum.GetValues<ClayVolume>())
        {
            var clay = new ClayCustomizationState(accent, volume);
            var package = ThemeService.Load(mode, ThemeService.Clay, (candidate, appearance) => ClayCustomization.Apply(candidate, appearance, clay));
            ResourceValidation.Validate(package);
            ContractChecks.Contrast(package, "TextOnAccent", "AccentPrimary", 4.5);
            ContractChecks.Contrast(package, "AccentPrimary", "AccentSoft", 4.5);
            clayCombinations++;
        }
        var clayFull = new ClayCustomizationState(ClayAccent.Sage, ClayVolume.Deep, ClayPlumpness.Puffy, ClayDensity.Compact, ClayCanvas.Cool, ClayType.Large, ClaySpacing.Spacious, ClayTextContrast.High, ClaySheen.Gloss, ClayWeight.Semibold);
        var clayPackage = ThemeService.Load(Appearance.Light, ThemeService.Clay, (candidate, appearance) => ClayCustomization.Apply(candidate, appearance, clayFull));
        ResourceValidation.Validate(clayPackage);
        if ((double)clayPackage["FontSizeBody"] != 14) throw new Exception("Clay large type");
        if (((CornerRadius)clayPackage["CardCornerRadius"]).TopLeft != 40) throw new Exception("Clay puffy radius");
        if ((double)clayPackage["ControlMinHeight"] != 36) throw new Exception("Clay compact height");
        if ((double)clayPackage["SpacingMedium"] != 12) throw new Exception("Clay spacious spacing");
        if (((SolidColorBrush)clayPackage["ApplicationBackground"]).Color != (Color)ColorConverter.ConvertFromString("#E8EEF9")) throw new Exception("Clay cool canvas");
        if (((SolidColorBrush)clayPackage["TextPrimary"]).Color != (Color)ColorConverter.ConvertFromString("#10143A")) throw new Exception("Clay high text");
        ContractChecks.Contrast(clayPackage, "TextPrimary", "ApplicationBackground", 4.5);
        if (ClayCustomization.BuiltIn.Count != 4) throw new Exception("Four clay built-in presets required");
        ContractChecks.ExpectFailure(() => ThemeService.Load(Appearance.Light, ThemeService.Clay, (candidate, appearance) =>
            ClayCustomization.Apply(candidate, appearance, new ClayCustomizationState((ClayAccent)999))), "invalid clay preset");
        Console.WriteLine($"PASS: {combinations} customization/mode combinations, {clayCombinations} clay combinations, immutable helpers, preference retention, rejected candidates and notification-failure semantics.");
    }
}
