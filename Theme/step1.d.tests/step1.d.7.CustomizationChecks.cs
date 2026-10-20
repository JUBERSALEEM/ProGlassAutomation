using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeBento;

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
            if (package["ActivityTrendGeometry"] is not Geometry geometry || !geometry.IsFrozen)
                throw new Exception("Theme-private geometry must be immutable.");
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
        Console.WriteLine($"PASS: {combinations} customization/mode combinations, immutable helpers, preference retention, rejected candidates and notification-failure semantics.");
    }
}
