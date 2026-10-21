using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeClaymorphism;
using ProGlassAutomation.ThemeLiquidGlass;
using ProGlassAutomation.ThemeNeumorphism;

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
        if (loaded.Neumorph?.Validate() is not null) throw new ArgumentException("Theme file contains invalid Neumorph preferences.");
        if (loaded.Liquid?.Validate() is not null) throw new ArgumentException("Theme file contains invalid Liquid Glass preferences.");
        if (!Enum.IsDefined(loaded.Appearance)) throw new ArgumentException("Theme file appearance is not supported.");
        if (string.IsNullOrWhiteSpace(loaded.Theme) || !ThemeService.InstalledThemes.Contains(loaded.Theme))
            loaded = loaded with { Theme = ThemeCatalog.BentoName };
        loaded = loaded with
        {
            Clay = loaded.Clay ?? new ClayCustomizationState(),
            Geometry = loaded.Geometry ?? new SharedGeometryState(),
            Neumorph = loaded.Neumorph ?? new NeumorphCustomizationState(),
            Font = loaded.Font ?? new FontCustomizationState(),
            Colors = loaded.Colors ?? new ColorPlateState(),
            Liquid = loaded.Liquid ?? new LiquidCustomizationState()
        };
        if (loaded.Geometry.Validate() is not null) throw new ArgumentException("Theme file contains invalid card rounding.");
        if (loaded.Font?.Validate() is not null) throw new ArgumentException("Theme file contains invalid font preferences.");
        if (loaded.Colors?.Validate() is not null) throw new ArgumentException("Theme file contains invalid color plate preferences.");
        foreach (var preset in loaded.CustomPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Bento preset.");
        foreach (var preset in loaded.ClayPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Clay preset.");
        foreach (var preset in loaded.NeumorphPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Neumorph preset.");
        foreach (var preset in loaded.LiquidPresets ?? [])
            if (string.IsNullOrWhiteSpace(preset.Name) || preset.State.Validate() is not null)
                throw new ArgumentException("Theme file contains an invalid Liquid Glass preset.");
        return loaded;
    }

    public static void Save(SavedCustomization state) => WriteFile(state, FilePath);

    public static void WriteFile(SavedCustomization state, string path)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (state.Preferences.Validate() is not null) throw new ArgumentException("Cannot persist invalid preferences.");
        if (state.Clay?.Validate() is not null) throw new ArgumentException("Cannot persist invalid clay preferences.");
        if (state.Neumorph?.Validate() is not null) throw new ArgumentException("Cannot persist invalid neumorph preferences.");
        if ((state.Geometry ?? new SharedGeometryState()).Validate() is not null) throw new ArgumentException("Cannot persist invalid card rounding.");
        if ((state.Font ?? new FontCustomizationState()).Validate() is not null) throw new ArgumentException("Cannot persist invalid font preferences.");
        if ((state.Colors ?? new ColorPlateState()).Validate() is not null) throw new ArgumentException("Cannot persist invalid color plate preferences.");
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        Directory.CreateDirectory(dir);
        var tmp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(tmp, JsonSerializer.Serialize(state with
            {
                Version = 2,
                Geometry = state.Geometry ?? new SharedGeometryState(),
                Font = state.Font ?? new FontCustomizationState(),
                Colors = state.Colors ?? new ColorPlateState()
            }, Options));
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
