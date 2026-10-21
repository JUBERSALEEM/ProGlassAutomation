using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeClaymorphism;

namespace ProGlassAutomation.Customization;

public static class CustomizationStore
{
    public const int MaxBytes = 64 * 1024;
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
            return ReadFile(FilePath);
        }
        catch (Exception ex) when (ex is IOException or JsonException or ArgumentException or InvalidOperationException)
        {
            return Defaults();
        }
    }

    public static SavedCustomization ReadFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var info = new FileInfo(path);
        if (!info.Exists) throw new FileNotFoundException("Theme file not found.", path);
        if (info.Length > MaxBytes) throw new ArgumentException("Theme files must be at most 64 KB.");
        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<SavedCustomization>(json, Options);
        if (loaded is null || loaded.Version != 2) throw new ArgumentException("Unsupported or empty ProGlass theme file.");
        if (loaded.Preferences is null || loaded.Preferences.Validate() is not null) throw new ArgumentException("Theme file contains invalid Bento preferences.");
        if (loaded.Clay?.Validate() is not null) throw new ArgumentException("Theme file contains invalid Clay preferences.");
        if (!Enum.IsDefined(loaded.Appearance)) throw new ArgumentException("Theme file appearance is not supported.");
        if (string.IsNullOrWhiteSpace(loaded.Theme) || !ThemeService.InstalledThemes.Contains(loaded.Theme))
            loaded = loaded with { Theme = ThemeCatalog.BentoName };
        loaded = loaded with
        {
            Clay = loaded.Clay ?? new ClayCustomizationState(),
            Geometry = loaded.Geometry ?? new SharedGeometryState()
        };
        if (loaded.Geometry.Validate() is not null) throw new ArgumentException("Theme file contains invalid card rounding.");
        foreach (var preset in loaded.CustomPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Bento preset.");
        foreach (var preset in loaded.ClayPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Clay preset.");
        return loaded;
    }

    public static void Save(SavedCustomization state) => WriteFile(state, FilePath);

    public static void WriteFile(SavedCustomization state, string path)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (state.Preferences.Validate() is not null) throw new ArgumentException("Cannot persist invalid preferences.");
        if (state.Clay?.Validate() is not null) throw new ArgumentException("Cannot persist invalid clay preferences.");
        if ((state.Geometry ?? new SharedGeometryState()).Validate() is not null) throw new ArgumentException("Cannot persist invalid card rounding.");
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        Directory.CreateDirectory(dir);
        var tmp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(tmp, JsonSerializer.Serialize(state with { Version = 2, Geometry = state.Geometry ?? new SharedGeometryState() }, Options));
            File.Move(tmp, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(tmp)) File.Delete(tmp);
        }
    }

    public static SavedCustomization Defaults() => new();

    public static void ClearCorrupt()
    {
        try { if (File.Exists(FilePath)) File.Delete(FilePath); } catch { }
    }
}
