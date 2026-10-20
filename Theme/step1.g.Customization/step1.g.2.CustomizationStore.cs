using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeClaymorphism;

namespace ProGlassAutomation.Customization;

public static class CustomizationStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ProGlassAutomation", "bento-customization.json");

    public static SavedCustomization Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return Defaults();
            var json = File.ReadAllText(FilePath);
            var loaded = JsonSerializer.Deserialize<SavedCustomization>(json, Options);
            if (loaded is null || loaded.Version != 2) return Defaults();
            // Validate strictly — invalid stored values must fall back, never crash app.
            if (loaded.Preferences?.Validate() is not null) return Defaults();
            if (loaded.Clay?.Validate() is not null) return Defaults();
            if (!Enum.IsDefined(loaded.Appearance)) return Defaults();
            if (string.IsNullOrWhiteSpace(loaded.Theme) || !ThemeService.InstalledThemes.Contains(loaded.Theme))
                loaded = loaded with { Theme = ThemeCatalog.BentoName };
            loaded = loaded with { Clay = loaded.Clay ?? new ClayCustomizationState() };
            foreach (var preset in loaded.CustomPresets ?? [])
                if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null) return Defaults();
            return loaded;
        }
        catch (Exception ex) when (ex is IOException or JsonException or ArgumentException or InvalidOperationException)
        {
            return Defaults();
        }
    }

    public static void Save(SavedCustomization state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (state.Preferences.Validate() is not null) throw new ArgumentException("Cannot persist invalid preferences.");
        var dir = Path.GetDirectoryName(FilePath)!;
        Directory.CreateDirectory(dir);
        // atomic write to avoid corrupt half-written file
        var tmp = FilePath + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(state with { Version = 2 }, Options));
        File.Move(tmp, FilePath, overwrite: true);
    }

    public static SavedCustomization Defaults() => new();

    public static void ClearCorrupt()
    {
        try { if (File.Exists(FilePath)) File.Delete(FilePath); } catch { }
    }
}
