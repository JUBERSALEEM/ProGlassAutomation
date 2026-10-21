using System.Windows;
using System.Windows.Input;
using ProGlassAutomation.Customization;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeStudio;

public sealed partial class ThemeStudioViewModel
{
    private BentoPreferences preferences = new();
    private BentoCustomizationState extended = new();
    private ClayCustomizationState clay = new();
    private SavedCustomization lastSaved = CustomizationStore.Load();
    private BentoCustomizationState draft => extended;
    private int selectedWorkspace = 1;
    private bool isMenuOpen = true;
    private string customizationCategory = "Appearance";

    private string selectedComponentId = "";
    private string presetNameInput = "";
    private string? lastSavedSnapshot;
    private string? lastSavedClaySnapshot;
    public ProGlassAutomation.Customization.ChartCustomizationViewModel ChartEditor { get; } = new();
    public IReadOnlyList<BentoAccent> AccentChoices { get; } = Enum.GetValues<BentoAccent>();
    public IReadOnlyList<BentoDensity> DensityChoices { get; } = Enum.GetValues<BentoDensity>();
    public IReadOnlyList<BentoShape> ShapeChoices { get; } = Enum.GetValues<BentoShape>();
    public IReadOnlyList<BentoType> TypeChoices { get; } = Enum.GetValues<BentoType>();
    public IReadOnlyList<BentoCanvas> CanvasChoices { get; } = Enum.GetValues<BentoCanvas>();
    public IReadOnlyList<BentoSurfaceStyle> SurfaceChoices { get; } = Enum.GetValues<BentoSurfaceStyle>();
    public IReadOnlyList<BentoTextContrast> TextContrastChoices { get; } = Enum.GetValues<BentoTextContrast>();
    public IReadOnlyList<BentoBorderIntensity> BorderChoices { get; } = Enum.GetValues<BentoBorderIntensity>();
    public IReadOnlyList<BentoShadowIntensity> ShadowChoices { get; } = Enum.GetValues<BentoShadowIntensity>();
    public IReadOnlyList<BentoSpacing> SpacingChoices { get; } = Enum.GetValues<BentoSpacing>();
    public IReadOnlyList<BentoCardPadding> PaddingChoices { get; } = Enum.GetValues<BentoCardPadding>();
    public IReadOnlyList<BentoFontWeight> WeightChoices { get; } = Enum.GetValues<BentoFontWeight>();
    public IReadOnlyList<string> AvailableCategories => CustomizationCategories.All;
    public IReadOnlyList<BentoLayoutPreset> LayoutPresetChoices { get; } = Enum.GetValues<BentoLayoutPreset>();
    public IReadOnlyList<ClayAccent> ClayAccentChoices { get; } = Enum.GetValues<ClayAccent>();
    public IReadOnlyList<ClayVolume> ClayVolumeChoices { get; } = Enum.GetValues<ClayVolume>();
    public IReadOnlyList<ClayPlumpness> ClayPlumpnessChoices { get; } = Enum.GetValues<ClayPlumpness>();
    public IReadOnlyList<ClayDensity> ClayDensityChoices { get; } = Enum.GetValues<ClayDensity>();
    public IReadOnlyList<ClayCanvas> ClayCanvasChoices { get; } = Enum.GetValues<ClayCanvas>();
    public IReadOnlyList<ClayType> ClayTypeChoices { get; } = Enum.GetValues<ClayType>();
    public IReadOnlyList<ClaySpacing> ClaySpacingChoices { get; } = Enum.GetValues<ClaySpacing>();
    public IReadOnlyList<ClayTextContrast> ClayTextContrastChoices { get; } = Enum.GetValues<ClayTextContrast>();
    public IReadOnlyList<ClaySheen> ClaySheenChoices { get; } = Enum.GetValues<ClaySheen>();
    public IReadOnlyList<ClayWeight> ClayWeightChoices { get; } = Enum.GetValues<ClayWeight>();
    public IReadOnlyList<ClayNamedPreset> ClayBuiltInPresets => ClayCustomization.BuiltIn;
    public IReadOnlyList<ClayNamedPreset> ClayCustomPresets => lastSaved.ClayPresets ?? [];
    public IReadOnlyList<string> ClayPresetNames => ClayCustomization.BuiltIn.Select(p => p.Name).Concat((lastSaved.ClayPresets ?? []).Select(p => p.Name)).ToList();
    private string selectedClayPresetName = ClayCustomization.BuiltIn[0].Name;
    public string SelectedClayPresetName
    {
        get => selectedClayPresetName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value == selectedClayPresetName) return;
            selectedClayPresetName = value;
            Changed(nameof(SelectedClayPresetName));
            ApplyPreset.Execute(value);
        }
    }
    public ClayAccent SelectedClayAccent { get => clay.Accent; set => UpdateClay(clay with { Accent = value }); }
    public ClayVolume SelectedClayVolume { get => clay.Volume; set => UpdateClay(clay with { Volume = value }); }
    public ClayPlumpness SelectedClayPlumpness { get => clay.Plumpness; set => UpdateClay(clay with { Plumpness = value }); }
    public ClayDensity SelectedClayDensity { get => clay.Density; set => UpdateClay(clay with { Density = value }); }
    public ClayCanvas SelectedClayCanvas { get => clay.Canvas; set => UpdateClay(clay with { Canvas = value }); }
    public ClayType SelectedClayTypography { get => clay.Typography; set => UpdateClay(clay with { Typography = value }); }
    public ClaySpacing SelectedClaySpacing { get => clay.Spacing; set => UpdateClay(clay with { Spacing = value }); }
    public ClayTextContrast SelectedClayTextContrast { get => clay.TextContrast; set => UpdateClay(clay with { TextContrast = value }); }
    public ClaySheen SelectedClaySheen { get => clay.Sheen; set => UpdateClay(clay with { Sheen = value }); }
    public ClayWeight SelectedClayWeight { get => clay.Weight; set => UpdateClay(clay with { Weight = value }); }
    public IReadOnlyList<NamedPreset> BuiltInPresets => PresetCatalog.BuiltIn;
    public IReadOnlyList<NamedPreset> CustomPresets => lastSaved.CustomPresets ?? [];
    public BentoAccent SelectedAccent { get => extended.Accent; set => UpdateExtended(extended with { Accent = value }); }
    public BentoDensity SelectedDensity { get => extended.Density; set => UpdateExtended(extended with { Density = value }); }
    public BentoShape SelectedShape { get => extended.Shape; set => UpdateExtended(extended with { Shape = value }); }
    public BentoType SelectedTypography { get => extended.Typography; set => UpdateExtended(extended with { Typography = value }); }
    public BentoCanvas SelectedCanvas { get => extended.Canvas; set => UpdateExtended(extended with { Canvas = value }); }
    public BentoSurfaceStyle SelectedSurface { get => extended.SurfaceStyle; set => UpdateExtended(extended with { SurfaceStyle = value }); }
    public BentoTextContrast SelectedTextContrast { get => extended.TextContrast; set => UpdateExtended(extended with { TextContrast = value }); }
    public BentoBorderIntensity SelectedBorder { get => extended.BorderIntensity; set => UpdateExtended(extended with { BorderIntensity = value }); }
    public BentoShadowIntensity SelectedShadow { get => extended.Shadow; set => UpdateExtended(extended with { Shadow = value }); }
    public BentoSpacing SelectedSpacing { get => extended.Spacing; set => UpdateExtended(extended with { Spacing = value }); }
    public BentoCardPadding SelectedPadding { get => extended.CardPadding; set => UpdateExtended(extended with { CardPadding = value }); }
    public BentoFontWeight SelectedWeight { get => extended.FontWeight; set => UpdateExtended(extended with { FontWeight = value }); }
    public BentoLayoutPreset SelectedLayoutPreset { get => extended.LayoutPreset; set => UpdateExtended(extended with { LayoutPreset = value }); }
    public bool HeroCompact { get => extended.HeroCompact; set => UpdateExtended(extended with { HeroCompact = value }); }
    public bool ShowKpiTrend { get => extended.ShowKpiTrend; set => UpdateExtended(extended with { ShowKpiTrend = value }); }
    public string CustomizationCategory { get => customizationCategory; set { customizationCategory = value; Changed(nameof(CustomizationCategory)); } }
    public string SelectedComponentId { get => selectedComponentId; set { selectedComponentId = value; Changed(nameof(SelectedComponentId)); Changed(nameof(ContextualTitle)); } }
    public string ContextualTitle => string.IsNullOrEmpty(selectedComponentId) ? "Select a card in the preview" : $"Editing: {selectedComponentId}";
    public string PresetNameInput { get => presetNameInput; set { presetNameInput = value; Changed(nameof(PresetNameInput)); } }
    public bool IsClay => SelectedTheme == ThemeService.Clay;
    public bool IsFluent => SelectedTheme == ThemeService.Fluent;
    public bool CanCustomizeBento => SelectedTheme == ThemeService.Bento;
    public bool CanCustomizeClay => IsClay;
    public bool CanCustomize => CanCustomizeBento || CanCustomizeClay;
    public string DesignSummary => CanCustomizeClay
        ? $"{SelectedClayAccent} / {SelectedClayVolume} / {SelectedClayPlumpness} / {SelectedClayTypography}"
        : CanCustomizeBento ? $"{SelectedAccent} / {SelectedDensity} / {SelectedShape} / {SelectedTypography} · {SelectedLayoutPreset}"
        : "Theme-default settings";
    public bool IsDirty => CanCustomizeClay ? clay != (lastSaved.Clay ?? new ClayCustomizationState()) : extended != lastSaved.Preferences;
    public int SelectedWorkspace
    {
        get => selectedWorkspace;
        set
        {
            if (value is 0 or 1)
            {
                selectedWorkspace = value;
                Changed(nameof(SelectedWorkspace));
                Changed(nameof(IsStudio));
                Changed(nameof(IsBusiness));
                Changed(nameof(WorkspaceTitle));
                Changed(nameof(WorkspaceSubtitle));
            }
        }
    }
    public bool IsStudio { get => selectedWorkspace == 0; set { if (value) SelectedWorkspace = 0; } }
    public bool IsBusiness { get => selectedWorkspace == 1; set { if (value) SelectedWorkspace = 1; } }
    public string WorkspaceTitle => IsStudio ? "Theme Studio" : "Home";
    public string WorkspaceSubtitle => IsStudio ? "Settings · Theme engine" : "Home";
    public bool IsMenuOpen { get => isMenuOpen; set { isMenuOpen = value; Changed(nameof(IsMenuOpen)); } }
    public string ShellClock => DateTime.Now.ToString("dd MMM yyyy / HH:mm", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
    public ICommand ToggleMenu { get; private set; } = null!;
    public ICommand ToggleAppearance { get; private set; } = null!;
    public ICommand ResetDesign { get; private set; } = null!;
    public ICommand ShowStudio { get; private set; } = null!;
    public ICommand ShowBusiness { get; private set; } = null!;
    public ICommand ApplyPreview { get; private set; } = null!;
    public ICommand SaveCustomization { get; private set; } = null!;
    public ICommand DiscardChanges { get; private set; } = null!;
    public ICommand ResetSection { get; private set; } = null!;
    public ICommand ResetTheme { get; private set; } = null!;
    public ICommand PreviewDefault { get; private set; } = null!;
    public ICommand ApplyPreset { get; private set; } = null!;
    public ICommand SavePreset { get; private set; } = null!;
    public ICommand DeletePreset { get; private set; } = null!;
    public ICommand RenamePreset { get; private set; } = null!;

    private void InitializeCustomization()
    {
        // Restore last saved; if none, defaults
        extended = lastSaved.Preferences ?? new BentoCustomizationState();
        clay = lastSaved.Clay ?? new ClayCustomizationState();
        preferences = extended.ToLegacy();
        var restored = ThemeService.InstalledThemes.Contains(lastSaved.Theme) ? lastSaved.Theme : ThemeService.Bento;
        try
        {
            themes.Apply(restored, lastSaved.Appearance,
                restored == ThemeService.Bento ? (p,m)=>BentoCustomization.ApplyExtended(p,m,extended)
                : restored == ThemeService.Clay ? (p,m)=>ClayCustomization.Apply(p,m,clay)
                : null);
        }
        catch { }
        ResetDesign = new DelegateCommand(() => { if (IsClay) ApplyClayState(new ClayCustomizationState()); else ApplyState(new BentoCustomizationState()); }, () => CanCustomize);
        ToggleMenu = new DelegateCommand(() => IsMenuOpen = !IsMenuOpen);
        ToggleAppearance = new DelegateCommand(() => { if (IsLight) IsDark = true; else IsLight = true; });
        ShowStudio = new DelegateCommand(() => SelectedWorkspace = 0);
        ShowBusiness = new DelegateCommand(() => SelectedWorkspace = 1);
        ApplyPreview = new DelegateCommand(() => { try { themes.Apply(SelectedTheme, themes.ActiveAppearance, LiveConfigure()); ThemeError=""; Feedback="Preview applied — unsaved changes present."; } catch(Exception ex){ ThemeError=ex.Message; } Changed(nameof(ThemeError)); }, () => CanCustomize);
        SaveCustomization = new DelegateCommand(() => { lastSaved = new SavedCustomization(2, themes.ActiveAppearance, extended, lastSaved.ChartOptions, lastSaved.ComponentStyles, lastSaved.CustomPresets, themes.ActiveTheme, clay, lastSaved.ClayPresets); CustomizationStore.Save(lastSaved); Feedback="Customization saved — will restore on next launch."; Changed(nameof(IsDirty)); }, () => CanCustomize && (CanCustomizeClay ? clay.Validate() is null : extended.Validate() is null));
        DiscardChanges = new DelegateCommand(() => { if (IsClay) ApplyClayState(lastSaved.Clay ?? new ClayCustomizationState()); else ApplyState(lastSaved.Preferences); Feedback="Discarded unsaved changes."; }, () => IsDirty);
        ResetSection = new DelegateCommand(() =>
        {
            if (IsClay)
            {
                var defaults = new ClayCustomizationState();
                var next = customizationCategory switch
                {
                    "Appearance" => clay with { Accent = defaults.Accent, Canvas = defaults.Canvas, Volume = defaults.Volume, Plumpness = defaults.Plumpness, TextContrast = defaults.TextContrast, Sheen = defaults.Sheen },
                    "Layout" => clay with { Density = defaults.Density, Spacing = defaults.Spacing },
                    "Typography" => clay with { Typography = defaults.Typography, Weight = defaults.Weight },
                    _ => defaults
                };
                ApplyClayState(next);
                Feedback = $"Reset {customizationCategory} to clay defaults.";
                return;
            }
            var bentoDefaults = new BentoCustomizationState();
            var bentoNext = customizationCategory switch
            {
                "Appearance" => extended with { Canvas = bentoDefaults.Canvas, SurfaceStyle = bentoDefaults.SurfaceStyle, TextContrast = bentoDefaults.TextContrast, BorderIntensity = bentoDefaults.BorderIntensity, Shadow = bentoDefaults.Shadow },
                "Layout" => extended with { LayoutPreset = bentoDefaults.LayoutPreset, Spacing = bentoDefaults.Spacing, CardPadding = bentoDefaults.CardPadding, HeroCompact = bentoDefaults.HeroCompact },
                "Typography" => extended with { Typography = bentoDefaults.Typography, FontWeight = bentoDefaults.FontWeight, Spacing = bentoDefaults.Spacing },
                _ => bentoDefaults
            };
            ApplyState(bentoNext);
            Feedback = $"Reset {customizationCategory} to defaults.";
        }, () => CanCustomize);
        ResetTheme = new DelegateCommand(() =>
        {
            if (IsClay)
            {
                ApplyClayState(new ClayCustomizationState());
                lastSaved = lastSaved with { Clay = new ClayCustomizationState(), Theme = ThemeService.Clay, Appearance = themes.ActiveAppearance };
                CustomizationStore.Save(lastSaved);
                Feedback = "Clay restored to defaults.";
                return;
            }
            ApplyState(new BentoCustomizationState());
            lastSaved = new SavedCustomization();
            CustomizationStore.Save(lastSaved);
            Feedback = "Theme restored to Bento defaults.";
        }, () => CanCustomize);
        PreviewDefault = new DelegateCommand(() =>
        {
            if (IsClay)
            {
                if (lastSavedClaySnapshot == null)
                {
                    lastSavedClaySnapshot = System.Text.Json.JsonSerializer.Serialize(clay);
                    ApplyClayState(new ClayCustomizationState());
                    Feedback = "Previewing clay defaults — toggle again to restore.";
                }
                else
                {
                    var prev = System.Text.Json.JsonSerializer.Deserialize<ClayCustomizationState>(lastSavedClaySnapshot);
                    lastSavedClaySnapshot = null;
                    if (prev != null) ApplyClayState(prev);
                    Feedback = "Restored your clay preview.";
                }
                return;
            }
            if (lastSavedSnapshot == null)
            {
                lastSavedSnapshot = System.Text.Json.JsonSerializer.Serialize(extended);
                ApplyState(new BentoCustomizationState());
                Feedback = "Previewing defaults — toggle again to restore.";
            }
            else
            {
                var prev = System.Text.Json.JsonSerializer.Deserialize<BentoCustomizationState>(lastSavedSnapshot);
                lastSavedSnapshot = null;
                if (prev != null) ApplyState(prev);
                Feedback = "Restored your preview.";
            }
        }, () => CanCustomize);
        ApplyPreset = new DelegateCommand<string>(name =>
        {
            if (IsClay)
            {
                var clayPreset = ClayCustomization.BuiltIn.Concat(lastSaved.ClayPresets ?? []).FirstOrDefault(p => p.Name == name);
                if (clayPreset == null) return;
                if (clayPreset.Appearance != themes.ActiveAppearance) SelectAppearance(ThemeService.Clay, clayPreset.Appearance);
                ApplyClayState(clayPreset.State);
                Feedback = $"Clay preset applied: {clayPreset.Name}";
                return;
            }
            var preset = BuiltInPresets.Concat(CustomPresets).FirstOrDefault(p => p.Name == name);
            if (preset != null)
            {
                ApplyState(preset.State);
                if (preset.Appearance != themes.ActiveAppearance) SelectAppearance(ThemeService.Bento, preset.Appearance);
                Feedback = $"Preset applied: {preset.Name}";
            }
        });
        SavePreset = new DelegateCommand(() =>
        {
            var n = presetNameInput.Trim();
            if (string.IsNullOrEmpty(n))
            {
                ThemeError = "Enter unique name not conflicting with built-in presets.";
                Changed(nameof(ThemeError));
                return;
            }
            if (IsClay)
            {
                if (ClayCustomization.IsBuiltIn(n))
                {
                    ThemeError = "Enter unique name not conflicting with built-in presets.";
                    Changed(nameof(ThemeError));
                    return;
                }
                var clayList = (lastSaved.ClayPresets ?? []).ToList();
                clayList.RemoveAll(p => p.Name == n);
                clayList.Add(new ClayNamedPreset(n, clay, themes.ActiveAppearance));
                lastSaved = lastSaved with { ClayPresets = clayList, Clay = clay, Theme = themes.ActiveTheme, Appearance = themes.ActiveAppearance };
                CustomizationStore.Save(lastSaved);
                PresetNameInput = "";
                Feedback = $"Clay preset saved: {n}";
                Changed(nameof(ThemeError));
                Changed(nameof(ClayCustomPresets));
                Changed(nameof(ClayPresetNames));
                return;
            }
            if (PresetCatalog.IsBuiltIn(n))
            {
                ThemeError = "Enter unique name not conflicting with built-in presets.";
                Changed(nameof(ThemeError));
                return;
            }
            var list = (lastSaved.CustomPresets ?? []).ToList();
            list.RemoveAll(p => p.Name == n);
            list.Add(new NamedPreset(n, extended, themes.ActiveAppearance));
            lastSaved = lastSaved with { CustomPresets = list };
            CustomizationStore.Save(lastSaved);
            PresetNameInput = "";
            Feedback = $"Preset saved: {n}";
            Changed(nameof(ThemeError));
        }, () => !string.IsNullOrWhiteSpace(presetNameInput) && CanCustomize);
        DeletePreset = new DelegateCommand<string>(name =>
        {
            if (IsClay)
            {
                if (ClayCustomization.IsBuiltIn(name)) return;
                var clayList = (lastSaved.ClayPresets ?? []).ToList();
                clayList.RemoveAll(p => p.Name == name);
                lastSaved = lastSaved with { ClayPresets = clayList };
                CustomizationStore.Save(lastSaved);
                Feedback = $"Deleted clay preset: {name}";
                Changed(nameof(ClayCustomPresets));
                Changed(nameof(ClayPresetNames));
                return;
            }
            if (PresetCatalog.IsBuiltIn(name)) return;
            var list = (lastSaved.CustomPresets ?? []).ToList();
            list.RemoveAll(p => p.Name == name);
            lastSaved = lastSaved with { CustomPresets = list };
            CustomizationStore.Save(lastSaved);
            Feedback = $"Deleted preset: {name}";
        });
        RenamePreset = new DelegateCommand<(string oldName, string newName)>(pair =>
        {
            var n = pair.newName.Trim();
            if (string.IsNullOrEmpty(n)) return;
            if (IsClay)
            {
                if (ClayCustomization.IsBuiltIn(n)) return;
                var clayList = (lastSaved.ClayPresets ?? []).ToList();
                var clayIdx = clayList.FindIndex(p => p.Name == pair.oldName);
                if (clayIdx >= 0)
                {
                    clayList[clayIdx] = clayList[clayIdx] with { Name = n };
                    lastSaved = lastSaved with { ClayPresets = clayList };
                    CustomizationStore.Save(lastSaved);
                    Feedback = $"Renamed to {n}";
                    Changed(nameof(ClayCustomPresets));
                    Changed(nameof(ClayPresetNames));
                }
                return;
            }
            if (PresetCatalog.IsBuiltIn(n)) return;
            var list = (lastSaved.CustomPresets ?? []).ToList();
            var idx = list.FindIndex(p => p.Name == pair.oldName);
            if (idx >= 0)
            {
                list[idx] = list[idx] with { Name = n };
                lastSaved = lastSaved with { CustomPresets = list };
                CustomizationStore.Save(lastSaved);
                Feedback = $"Renamed to {n}";
            }
        });
    }
    private Action<ResourceDictionary, Appearance>? LiveConfigure() =>
        CanCustomizeBento ? (package, mode) => BentoCustomization.ApplyExtended(package, mode, extended)
        : CanCustomizeClay ? (package, mode) => ClayCustomization.Apply(package, mode, clay)
        : null;
    private void Customize(BentoPreferences candidate)
    {
        if (!CanCustomizeBento || candidate == preferences) return;
        UpdateExtended(new BentoCustomizationState(candidate.Accent, candidate.Density, candidate.Shape, candidate.Typography, extended.Canvas, extended.SurfaceStyle, extended.TextContrast, extended.BorderIntensity, extended.Shadow, extended.Spacing, extended.CardPadding, extended.FontWeight, extended.LayoutPreset, extended.HeroCompact, extended.ShowKpiTrend, extended.ShowTableStatus));
    }
    private void UpdateExtended(BentoCustomizationState next)
    {
        if (!CanCustomizeBento || next == extended) return;
        ApplyState(next);
    }
    private void UpdateClay(ClayCustomizationState next)
    {
        if (!CanCustomizeClay || next == clay) return;
        ApplyClayState(next);
    }
    private void ApplyState(BentoCustomizationState next)
    {
        try
        {
            themes.Apply(ThemeService.Bento, themes.ActiveAppearance, (pkg,mode)=>BentoCustomization.ApplyExtended(pkg,mode,next));
            extended = next; preferences = next.ToLegacy(); ThemeError=""; Feedback="Live preview updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { extended = next; preferences = next.ToLegacy(); ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.IO.IOException or System.Windows.Markup.XamlParseException) { ThemeError=$"Design unchanged: {error.Message}"; }
        foreach (var n in new[] { nameof(SelectedAccent), nameof(SelectedDensity), nameof(SelectedShape), nameof(SelectedTypography), nameof(SelectedCanvas), nameof(SelectedSurface), nameof(SelectedTextContrast), nameof(SelectedBorder), nameof(SelectedShadow), nameof(SelectedSpacing), nameof(SelectedPadding), nameof(SelectedWeight), nameof(SelectedLayoutPreset), nameof(HeroCompact), nameof(ShowKpiTrend), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
    }
    private void ApplyClayState(ClayCustomizationState next)
    {
        try
        {
            themes.Apply(ThemeService.Clay, themes.ActiveAppearance, (pkg,mode)=>ClayCustomization.Apply(pkg,mode,next));
            clay = next; ThemeError=""; Feedback="Live clay preview updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { clay = next; ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.IO.IOException or System.Windows.Markup.XamlParseException) { ThemeError=$"Design unchanged: {error.Message}"; }
        foreach (var n in new[] { nameof(SelectedClayAccent), nameof(SelectedClayVolume), nameof(SelectedClayPlumpness), nameof(SelectedClayDensity), nameof(SelectedClayCanvas), nameof(SelectedClayTypography), nameof(SelectedClaySpacing), nameof(SelectedClayTextContrast), nameof(SelectedClaySheen), nameof(SelectedClayWeight), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
    }
}
