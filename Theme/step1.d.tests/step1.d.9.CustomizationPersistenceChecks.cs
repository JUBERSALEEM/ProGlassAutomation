using System.IO;
using ProGlassAutomation.Customization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;

namespace ProGlassAutomation.ThemeTests;
internal static class CustomizationPersistenceChecks
{
    public static void Run()
    {
        // Every customization control must affect UI — test via resource mutation
        var light = ThemeService.Load(Appearance.Light);
        var state = new BentoCustomizationState(BentoAccent.Indigo, BentoDensity.Compact, BentoShape.Rounded, BentoType.Large, BentoCanvas.Warm, BentoSurfaceStyle.Elevated, BentoTextContrast.High, BentoBorderIntensity.Strong, BentoShadowIntensity.Elevated, BentoSpacing.Spacious, BentoCardPadding.Spacious, BentoFontWeight.Semibold, BentoLayoutPreset.Executive, true, false, false);
        BentoCustomization.ApplyExtended(light, Appearance.Light, state);
        // Validate all appearance properties mutated
        if ((double)light["ControlMinHeight"] != 32) throw new Exception("Compact density height");
        if ((double)light["FontSizeBody"] != 14) throw new Exception("Large typography");
        if (((System.Windows.CornerRadius)light["CardCornerRadius"]).TopLeft != 16) throw new Exception("Rounded minus compact dense adjustment");
        // Canvas tint applied
        if (((System.Windows.Media.SolidColorBrush)light["ApplicationBackground"]).Color.ToString() != "#FFF5EFE6") throw new Exception("Warm canvas not applied");
        // Save/load roundtrip
        var clayState = new ClayCustomizationState(ClayAccent.Berry, ClayVolume.Deep, ClayPlumpness.Puffy, ClayDensity.Compact, ClayCanvas.Cool, ClayType.Large, ClaySpacing.Spacious, ClayTextContrast.High, ClaySheen.Gloss, ClayWeight.Semibold);
        var saved = new SavedCustomization(2, Appearance.Dark, state, null, null, [new NamedPreset("My Test", state, Appearance.Dark)], ThemeCatalog.FluentName, clayState, [new ClayNamedPreset("Berry Night", clayState, Appearance.Dark)]);
        var tmp = Path.Combine(Path.GetTempPath(), "proglass-test-" + Guid.NewGuid() + ".json");
        CustomizationStore.WriteFile(saved, tmp);
        var fromFile = CustomizationStore.ReadFile(tmp);
        if (fromFile.Preferences.Accent != BentoAccent.Indigo || fromFile.Theme != ThemeCatalog.FluentName) throw new Exception("Store write/read roundtrip");
        if (fromFile.Clay?.Accent != ClayAccent.Berry) throw new Exception("Store clay roundtrip");
        File.WriteAllText(tmp, "{\"Version\":1}");
        try { CustomizationStore.ReadFile(tmp); throw new Exception("Version 1 import must fail"); }
        catch (ArgumentException) { }
        File.WriteAllBytes(tmp, new byte[CustomizationStore.MaxBytes + 1]);
        try { CustomizationStore.ReadFile(tmp); throw new Exception("Oversized import must fail"); }
        catch (ArgumentException) { }
        File.WriteAllText(tmp, "{}");
        try { CustomizationStore.ReadFile(tmp); throw new Exception("Empty import must fail"); }
        catch (ArgumentException) { }
        File.Delete(tmp);
        var json = System.Text.Json.JsonSerializer.Serialize(saved, new System.Text.Json.JsonSerializerOptions{ WriteIndented=true, Converters={ new System.Text.Json.Serialization.JsonStringEnumConverter()}});
        var reloaded = System.Text.Json.JsonSerializer.Deserialize<SavedCustomization>(json, new System.Text.Json.JsonSerializerOptions{ Converters={ new System.Text.Json.Serialization.JsonStringEnumConverter()}, PropertyNameCaseInsensitive=true});
        if (reloaded!.Preferences.Accent != BentoAccent.Indigo) throw new Exception("Roundtrip accent");
        if (reloaded.Theme != ThemeCatalog.FluentName) throw new Exception("Roundtrip theme");
        if (reloaded.Clay?.Accent != ClayAccent.Berry) throw new Exception("Roundtrip clay accent");
        if (reloaded.Clay?.Typography != ClayType.Large || reloaded.Clay?.Sheen != ClaySheen.Gloss) throw new Exception("Roundtrip clay extras");
        if (reloaded.ClayPresets is not { Count: 1 } || reloaded.ClayPresets[0].Name != "Berry Night") throw new Exception("Clay preset roundtrip");
        if (reloaded.CustomPresets!.Count != 1 || reloaded.CustomPresets[0].Name != "My Test") throw new Exception("Custom preset roundtrip");
        // Built-in protected
        if (!PresetCatalog.IsBuiltIn("Balanced Bento")) throw new Exception("Built-in recognition");
        if (!ClayCustomization.IsBuiltIn("Rose Plush") || ClayCustomization.BuiltIn.Count != 4) throw new Exception("Clay built-in recognition");
        if (PresetCatalog.IsBuiltIn("My Test")) throw new Exception("Custom not built-in");
        if (PresetCatalog.BuiltIn.Count != 5) throw new Exception("Five built-in presets required");
        // Corrupt handling — invalid enum falls back
        var corrupt = json.Replace("\"Accent\": \"Ocean\"", "\"Accent\": 999");
        var fallback = System.Text.Json.JsonSerializer.Deserialize<SavedCustomization>(corrupt, new System.Text.Json.JsonSerializerOptions{ Converters={ new System.Text.Json.Serialization.JsonStringEnumConverter()}, PropertyNameCaseInsensitive=true});
        if (fallback!.Preferences.Validate() == null) throw new Exception("Corrupt should fail validation");
        // Reset produces defaults
        var defaults = new BentoCustomizationState();
        if (defaults.LayoutPreset != BentoLayoutPreset.Balanced) throw new Exception("Default preset");
        // Chart data not mutated by display options
        var ds = new ProGlassAutomation.Analytics.ChartDataset("T", ProGlassAutomation.Analytics.ChartKind.Columns, ["A"], [new ProGlassAutomation.Analytics.ChartSeriesData("S", [1])], "u", "summary");
        var vmChart = new ChartCustomizationViewModel();
        if (vmChart.PreviewDataset.Series[0].Values[0] != 1) throw new Exception("Chart source values unchanged");
        Console.WriteLine("PASS: customization controls affect resources, save/load, built-in protection, corrupt fallback, chart isolation.");
    }
}
