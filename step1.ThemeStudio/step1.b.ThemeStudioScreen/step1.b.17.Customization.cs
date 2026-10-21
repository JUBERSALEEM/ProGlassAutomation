using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ProGlassAutomation.Customization;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.ThemeClaymorphism;
using ProGlassAutomation.ThemeNeumorphism;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeStudio;

public sealed partial class ThemeStudioViewModel
{
    private BentoPreferences preferences = new();
    private BentoCustomizationState extended = new();
    private ClayCustomizationState clay = new();
    private NeumorphCustomizationState neumorph = new();
    private SharedGeometryState geometry = new();
    private SavedCustomization lastSaved = CustomizationStore.Load();
    private BentoCustomizationState draft => extended;
    private int selectedWorkspace = 1;
    private bool isMenuOpen = true;
    private string customizationCategory = "Appearance";

    private string selectedComponentId = "";
    private string presetNameInput = "";
    private string? lastSavedSnapshot;
    private string? lastSavedClaySnapshot;
    private string? lastSavedNeumorphSnapshot;
    public ProGlassAutomation.Customization.ChartCustomizationViewModel ChartEditor { get; } = new();
    public IReadOnlyList<BentoAccent> AccentChoices { get; } = Enum.GetValues<BentoAccent>();
    public IReadOnlyList<BentoDensity> DensityChoices { get; } = Enum.GetValues<BentoDensity>();
    public IReadOnlyList<BentoShape> ShapeChoices { get; } = Enum.GetValues<BentoShape>();
    public IReadOnlyList<CardRounding> RoundingChoices { get; } = Enum.GetValues<CardRounding>();
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
    public IReadOnlyList<ClayLighting> ClayLightingChoices { get; } = Enum.GetValues<ClayLighting>();
    public IReadOnlyList<ClayRelief> ClayReliefChoices { get; } = Enum.GetValues<ClayRelief>();
    public IReadOnlyList<ClayEffects> ClayEffectsChoices { get; } = Enum.GetValues<ClayEffects>();
    public IReadOnlyList<NeumorphDepth> NeumorphDepthChoices { get; } = Enum.GetValues<NeumorphDepth>();
    public IReadOnlyList<NeumorphBlur> NeumorphBlurChoices { get; } = Enum.GetValues<NeumorphBlur>();
    public IReadOnlyList<NeumorphIntensity> NeumorphIntensityChoices { get; } = Enum.GetValues<NeumorphIntensity>();
    public IReadOnlyList<NeumorphNamedPreset> NeumorphBuiltInPresets => NeumorphCustomization.BuiltIn;
    public IReadOnlyList<NeumorphNamedPreset> NeumorphCustomPresets => lastSaved.NeumorphPresets ?? [];
    public IReadOnlyList<string> NeumorphPresetNames => NeumorphCustomization.BuiltIn.Select(p => p.Name).Concat((lastSaved.NeumorphPresets ?? []).Select(p => p.Name)).ToList();
    private string selectedNeumorphPresetName = NeumorphCustomization.BuiltIn[0].Name;
    public string SelectedNeumorphPresetName
    {
        get => selectedNeumorphPresetName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value == selectedNeumorphPresetName) return;
            selectedNeumorphPresetName = value;
            Changed(nameof(SelectedNeumorphPresetName));
            ApplyPreset.Execute(value);
        }
    }
    public NeumorphDepth SelectedNeumorphDepth { get => neumorph.Depth; set => UpdateNeumorph(neumorph with { Depth = value }); }
    public NeumorphBlur SelectedNeumorphBlur { get => neumorph.Blur; set => UpdateNeumorph(neumorph with { Blur = value }); }
    public NeumorphIntensity SelectedNeumorphIntensity { get => neumorph.Intensity; set => UpdateNeumorph(neumorph with { Intensity = value }); }
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
    public ClayLighting SelectedClayLighting { get => clay.Lighting; set => UpdateClay(clay with { Lighting = value }); }
    public ClayRelief SelectedClayRelief { get => clay.Relief; set => UpdateClay(clay with { Relief = value }); }
    public ClayEffects SelectedClayEffects { get => clay.Effects; set => UpdateClay(clay with { Effects = value }); }
    public IReadOnlyList<NamedPreset> BuiltInPresets => PresetCatalog.BuiltIn;
    public IReadOnlyList<NamedPreset> CustomPresets => lastSaved.CustomPresets ?? [];
    public BentoAccent SelectedAccent { get => extended.Accent; set => UpdateExtended(extended with { Accent = value }); }
    public BentoDensity SelectedDensity { get => extended.Density; set => UpdateExtended(extended with { Density = value }); }
    public BentoShape SelectedShape
    {
        get => extended.Shape;
        set
        {
            if (!CanCustomizeBento || value == extended.Shape) return;
            geometry = geometry with { Rounding = SharedGeometry.FromShape(value) };
            UpdateExtended(extended with { Shape = value });
            Changed(nameof(SelectedRounding));
        }
    }
    public CardRounding SelectedRounding
    {
        get => geometry.Rounding;
        set
        {
            if (!CanCustomizeGeometry || value == geometry.Rounding) return;
            geometry = geometry with { Rounding = value };
            if (CanCustomizeBento)
                UpdateExtended(extended with { Shape = SharedGeometry.ToShape(value) });
            else
                ApplySharedOnly();
            Changed(nameof(SelectedRounding));
            Changed(nameof(DesignSummary));
            Changed(nameof(IsDirty));
        }
    }
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
    public bool IsNeumorph => SelectedTheme == ThemeService.Neumorphism;
    public bool CanCustomizeBento => SelectedTheme == ThemeService.Bento;
    public bool CanCustomizeClay => IsClay;
    public bool CanCustomizeNeumorph => IsNeumorph;
    public bool CanCustomizeGeometry => ThemeCatalog.Find(SelectedTheme).Geometry == ThemeGeometry.SharedTokens;
    public bool CanCustomize => CanCustomizeBento || CanCustomizeClay || CanCustomizeNeumorph;
    public string DesignSummary => CanCustomizeClay
        ? $"{SelectedClayAccent} / {SelectedClayVolume} / {SelectedClayPlumpness} / {SelectedClayTypography}"
        : CanCustomizeBento ? $"{SelectedAccent} / {SelectedDensity} / {SelectedShape} / {SelectedTypography} · {SelectedLayoutPreset}"
        : IsFluent ? $"Fluent / {SelectedRounding}"
        : IsNeumorph ? $"Neumorphism / {SelectedRounding} / {SelectedNeumorphDepth} / {SelectedNeumorphBlur}"
        : "Theme-default settings";
    public bool IsDirty =>
        themes.ActiveTheme != lastSaved.Theme
        || themes.ActiveAppearance != lastSaved.Appearance
        || geometry != (lastSaved.Geometry ?? new SharedGeometryState())
        || (CanCustomizeClay && clay != (lastSaved.Clay ?? new ClayCustomizationState()))
        || (CanCustomizeNeumorph && neumorph != (lastSaved.Neumorph ?? new NeumorphCustomizationState()))
        || (CanCustomizeBento && extended != lastSaved.Preferences);
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
    public ICommand ImportCustomization { get; private set; } = null!;
    public ICommand ExportCustomization { get; private set; } = null!;

    private void InitializeCustomization()
    {
        // Restore last saved; if none, defaults
        extended = lastSaved.Preferences ?? new BentoCustomizationState();
        clay = lastSaved.Clay ?? new ClayCustomizationState();
        neumorph = lastSaved.Neumorph ?? new NeumorphCustomizationState();
        geometry = lastSaved.Geometry ?? new SharedGeometryState(SharedGeometry.FromShape(extended.Shape));
        if (lastSaved.Geometry is null) lastSaved = lastSaved with { Geometry = geometry };
        preferences = extended.ToLegacy();
        var restored = ThemeService.InstalledThemes.Contains(lastSaved.Theme) ? lastSaved.Theme : ThemeService.Bento;
        try
        {
            themes.Apply(restored, lastSaved.Appearance, SharedGeometry.Compose(restored, extended, clay, geometry, neumorph));
        }
        catch { }
        ResetDesign = new DelegateCommand(() =>
        {
            geometry = new SharedGeometryState();
            if (IsClay) ApplyClayState(new ClayCustomizationState());
            else if (IsNeumorph) ApplyNeumorphState(new NeumorphCustomizationState());
            else if (CanCustomizeBento) ApplyState(new BentoCustomizationState());
            else ApplySharedOnly();
            Changed(nameof(SelectedRounding));
        }, () => CanCustomize || CanCustomizeGeometry);
        ToggleMenu = new DelegateCommand(() => IsMenuOpen = !IsMenuOpen);
        ToggleAppearance = new DelegateCommand(() => { if (IsLight) IsDark = true; else IsLight = true; });
        ShowStudio = new DelegateCommand(() => SelectedWorkspace = 0);
        ShowBusiness = new DelegateCommand(() => SelectedWorkspace = 1);
        ApplyPreview = new DelegateCommand(() => { try { themes.Apply(SelectedTheme, themes.ActiveAppearance, LiveConfigure()); ThemeError=""; Feedback="Preview applied — unsaved changes present."; } catch(Exception ex){ ThemeError=ex.Message; } Changed(nameof(ThemeError)); }, () => CanCustomize || CanCustomizeGeometry);
        SaveCustomization = new DelegateCommand(() =>
        {
            if (extended.Validate() is not null || (clay ?? new ClayCustomizationState()).Validate() is not null || (neumorph ?? new NeumorphCustomizationState()).Validate() is not null)
            {
                ThemeError = "Cannot save invalid customization.";
                Changed(nameof(ThemeError));
                return;
            }
            var candidate = CaptureLive();
            try
            {
                CustomizationStore.Save(candidate);
                lastSaved = candidate;
                ThemeError = "";
                Feedback = "Customization saved — will restore on next launch.";
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or InvalidOperationException)
            {
                ThemeError = "Save failed; previous saved package kept. " + ex.Message;
            }
            Changed(nameof(ThemeError));
            Changed(nameof(IsDirty));
        }, () => extended.Validate() is null && (clay ?? new ClayCustomizationState()).Validate() is null && (neumorph ?? new NeumorphCustomizationState()).Validate() is null);
        DiscardChanges = new DelegateCommand(() =>
        {
            RestoreSaved(lastSaved);
            Feedback = "Discarded unsaved changes.";
        }, () => IsDirty);
        ImportCustomization = new DelegateCommand(() =>
        {
            try
            {
                var dialog = new OpenFileDialog { Filter = "ProGlass theme (*.json)|*.json", Title = "Import customization" };
                if (dialog.ShowDialog() != true) return;
                ApplyImported(CustomizationStore.ReadFile(dialog.FileName));
                Feedback = "Imported into the live application — use Save to persist, Discard to revert.";
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or InvalidOperationException or JsonException)
            {
                ThemeError = "Import rejected: " + ex.Message;
                Changed(nameof(ThemeError));
            }
        });
        ExportCustomization = new DelegateCommand(() =>
        {
            try
            {
                var dialog = new SaveFileDialog { Filter = "ProGlass theme (*.json)|*.json", Title = "Export customization", FileName = "proglass-theme.json" };
                if (dialog.ShowDialog() != true) return;
                CustomizationStore.WriteFile(CaptureLive(), dialog.FileName);
                Feedback = "Exported current live customization.";
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or InvalidOperationException)
            {
                ThemeError = "Export failed: " + ex.Message;
                Changed(nameof(ThemeError));
            }
        });
        ResetSection = new DelegateCommand(() =>
        {
            if (IsClay)
            {
                var defaults = new ClayCustomizationState();
                var next = customizationCategory switch
                {
                    "Appearance" => clay with { Accent = defaults.Accent, Canvas = defaults.Canvas, Volume = defaults.Volume, Plumpness = defaults.Plumpness, TextContrast = defaults.TextContrast, Sheen = defaults.Sheen, Lighting = defaults.Lighting, Relief = defaults.Relief, Effects = defaults.Effects },
                    "Layout" => clay with { Density = defaults.Density, Spacing = defaults.Spacing },
                    "Typography" => clay with { Typography = defaults.Typography, Weight = defaults.Weight },
                    _ => defaults
                };
                ApplyClayState(next);
                Feedback = $"Reset {customizationCategory} to clay defaults.";
                return;
            }
            if (IsNeumorph)
            {
                ApplyNeumorphState(new NeumorphCustomizationState());
                Feedback = $"Reset {customizationCategory} to neumorph defaults.";
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
            geometry = new SharedGeometryState();
            Changed(nameof(SelectedRounding));
            if (IsClay)
            {
                ApplyClayState(new ClayCustomizationState());
                lastSaved = lastSaved with { Clay = new ClayCustomizationState(), Theme = ThemeService.Clay, Appearance = themes.ActiveAppearance, Geometry = geometry };
                CustomizationStore.Save(lastSaved);
                Feedback = "Clay restored to defaults.";
                return;
            }
            if (IsNeumorph)
            {
                ApplyNeumorphState(new NeumorphCustomizationState());
                lastSaved = lastSaved with { Neumorph = new NeumorphCustomizationState(), Theme = ThemeService.Neumorphism, Appearance = themes.ActiveAppearance, Geometry = geometry };
                CustomizationStore.Save(lastSaved);
                Feedback = "Neumorphism restored to defaults.";
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
            if (IsNeumorph)
            {
                if (lastSavedNeumorphSnapshot == null)
                {
                    lastSavedNeumorphSnapshot = System.Text.Json.JsonSerializer.Serialize(neumorph);
                    ApplyNeumorphState(new NeumorphCustomizationState());
                    Feedback = "Previewing neumorph defaults — toggle again to restore.";
                }
                else
                {
                    var prev = System.Text.Json.JsonSerializer.Deserialize<NeumorphCustomizationState>(lastSavedNeumorphSnapshot);
                    lastSavedNeumorphSnapshot = null;
                    if (prev != null) ApplyNeumorphState(prev);
                    Feedback = "Restored your neumorph preview.";
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
            if (IsNeumorph)
            {
                var neuPreset = NeumorphCustomization.BuiltIn.Concat(lastSaved.NeumorphPresets ?? []).FirstOrDefault(p => p.Name == name);
                if (neuPreset == null) return;
                if (neuPreset.Appearance != themes.ActiveAppearance) SelectAppearance(ThemeService.Neumorphism, neuPreset.Appearance);
                ApplyNeumorphState(neuPreset.State);
                Feedback = $"Neumorph preset applied: {neuPreset.Name}";
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
            if (IsNeumorph)
            {
                if (NeumorphCustomization.IsBuiltIn(n))
                {
                    ThemeError = "Enter unique name not conflicting with built-in presets.";
                    Changed(nameof(ThemeError));
                    return;
                }
                var neuList = (lastSaved.NeumorphPresets ?? []).ToList();
                neuList.RemoveAll(p => p.Name == n);
                neuList.Add(new NeumorphNamedPreset(n, neumorph, themes.ActiveAppearance));
                lastSaved = lastSaved with { NeumorphPresets = neuList, Neumorph = neumorph, Theme = themes.ActiveTheme, Appearance = themes.ActiveAppearance };
                CustomizationStore.Save(lastSaved);
                PresetNameInput = "";
                Feedback = $"Neumorph preset saved: {n}";
                Changed(nameof(ThemeError));
                Changed(nameof(NeumorphCustomPresets));
                Changed(nameof(NeumorphPresetNames));
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
            if (IsNeumorph)
            {
                if (NeumorphCustomization.IsBuiltIn(name)) return;
                var neuList = (lastSaved.NeumorphPresets ?? []).ToList();
                neuList.RemoveAll(p => p.Name == name);
                lastSaved = lastSaved with { NeumorphPresets = neuList };
                CustomizationStore.Save(lastSaved);
                Feedback = $"Deleted neumorph preset: {name}";
                Changed(nameof(NeumorphCustomPresets));
                Changed(nameof(NeumorphPresetNames));
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
            if (IsNeumorph)
            {
                if (NeumorphCustomization.IsBuiltIn(n)) return;
                var neuList = (lastSaved.NeumorphPresets ?? []).ToList();
                var neuIdx = neuList.FindIndex(p => p.Name == pair.oldName);
                if (neuIdx >= 0)
                {
                    neuList[neuIdx] = neuList[neuIdx] with { Name = n };
                    lastSaved = lastSaved with { NeumorphPresets = neuList };
                    CustomizationStore.Save(lastSaved);
                    Feedback = $"Renamed to {n}";
                    Changed(nameof(NeumorphCustomPresets));
                    Changed(nameof(NeumorphPresetNames));
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
    private Action<ResourceDictionary, Appearance> LiveConfigure() =>
        SharedGeometry.Compose(SelectedTheme, extended, clay, geometry, neumorph);
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
            themes.Apply(ThemeService.Bento, themes.ActiveAppearance, SharedGeometry.Compose(ThemeService.Bento, next, clay, geometry, neumorph));
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
            themes.Apply(ThemeService.Clay, themes.ActiveAppearance, SharedGeometry.Compose(ThemeService.Clay, extended, next, geometry, neumorph));
            clay = next; ThemeError=""; Feedback="Live clay preview updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { clay = next; ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.IO.IOException or System.Windows.Markup.XamlParseException) { ThemeError=$"Design unchanged: {error.Message}"; }
        foreach (var n in new[] { nameof(SelectedClayAccent), nameof(SelectedClayVolume), nameof(SelectedClayPlumpness), nameof(SelectedClayDensity), nameof(SelectedClayCanvas), nameof(SelectedClayTypography), nameof(SelectedClaySpacing), nameof(SelectedClayTextContrast), nameof(SelectedClaySheen), nameof(SelectedClayWeight), nameof(SelectedClayLighting), nameof(SelectedClayRelief), nameof(SelectedClayEffects), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
    }

    private void UpdateNeumorph(NeumorphCustomizationState next)
    {
        if (!CanCustomizeNeumorph || next == neumorph) return;
        ApplyNeumorphState(next);
    }
    private void ApplyNeumorphState(NeumorphCustomizationState next)
    {
        try
        {
            themes.Apply(ThemeService.Neumorphism, themes.ActiveAppearance, SharedGeometry.Compose(ThemeService.Neumorphism, extended, clay, geometry, next));
            neumorph = next; ThemeError = ""; Feedback = "Live neumorph preview updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { neumorph = next; ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or IOException or System.Windows.Markup.XamlParseException) { ThemeError = $"Design unchanged: {error.Message}"; }
        foreach (var n in new[] { nameof(SelectedNeumorphDepth), nameof(SelectedNeumorphBlur), nameof(SelectedNeumorphIntensity), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
    }

    private SavedCustomization CaptureLive() => new(
        2,
        themes.ActiveAppearance,
        extended,
        lastSaved.ChartOptions,
        lastSaved.ComponentStyles,
        lastSaved.CustomPresets,
        themes.ActiveTheme,
        clay,
        lastSaved.ClayPresets,
        geometry,
        neumorph,
        lastSaved.NeumorphPresets);

    private void ApplySharedOnly()
    {
        try
        {
            themes.Apply(SelectedTheme, themes.ActiveAppearance, LiveConfigure());
            ThemeError = "";
            Feedback = "Live card rounding updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or IOException or System.Windows.Markup.XamlParseException)
        {
            ThemeError = $"Design unchanged: {error.Message}";
        }
        Changed(nameof(ThemeError));
        Changed(nameof(IsDirty));
        Changed(nameof(DesignSummary));
    }

    private void ApplyImported(SavedCustomization loaded)
    {
        ArgumentNullException.ThrowIfNull(loaded);
        AdoptPackage(loaded, persist: false);
    }

    private void RestoreSaved(SavedCustomization saved) => AdoptPackage(saved, persist: false);

    private void AdoptPackage(SavedCustomization package, bool persist)
    {
        var nextBento = package.Preferences ?? new BentoCustomizationState();
        var nextClay = package.Clay ?? new ClayCustomizationState();
        var nextNeumorph = package.Neumorph ?? new NeumorphCustomizationState();
        var nextGeometry = package.Geometry ?? new SharedGeometryState(SharedGeometry.FromShape(nextBento.Shape));
        var theme = ThemeService.InstalledThemes.Contains(package.Theme) ? package.Theme : ThemeService.Bento;
        var previousBento = extended;
        var previousClay = clay;
        var previousNeumorph = neumorph;
        var previousGeometry = geometry;
        try
        {
            extended = nextBento;
            clay = nextClay;
            neumorph = nextNeumorph;
            geometry = nextGeometry;
            preferences = nextBento.ToLegacy();
            themes.Apply(theme, package.Appearance, SharedGeometry.Compose(theme, nextBento, nextClay, nextGeometry, nextNeumorph));
            ThemeError = "";
            if (persist)
            {
                CustomizationStore.Save(package);
                lastSaved = package;
            }
        }
        catch (ThemeNotificationException error)
        {
            ThemeError = error.Message;
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or IOException or System.Windows.Markup.XamlParseException)
        {
            extended = previousBento;
            clay = previousClay;
            neumorph = previousNeumorph;
            geometry = previousGeometry;
            preferences = previousBento.ToLegacy();
            ThemeError = error.Message;
        }
        foreach (var n in new[] { nameof(SelectedAccent), nameof(SelectedDensity), nameof(SelectedShape), nameof(SelectedTypography), nameof(SelectedCanvas), nameof(SelectedSurface), nameof(SelectedTextContrast), nameof(SelectedBorder), nameof(SelectedShadow), nameof(SelectedSpacing), nameof(SelectedPadding), nameof(SelectedWeight), nameof(SelectedLayoutPreset), nameof(HeroCompact), nameof(ShowKpiTrend), nameof(SelectedRounding), nameof(SelectedClayAccent), nameof(SelectedClayVolume), nameof(SelectedClayPlumpness), nameof(SelectedClayDensity), nameof(SelectedClayCanvas), nameof(SelectedClayTypography), nameof(SelectedClaySpacing), nameof(SelectedClayTextContrast), nameof(SelectedClaySheen), nameof(SelectedClayWeight), nameof(SelectedClayLighting), nameof(SelectedClayRelief), nameof(SelectedClayEffects), nameof(SelectedNeumorphDepth), nameof(SelectedNeumorphBlur), nameof(SelectedNeumorphIntensity), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
        CommandManager.InvalidateRequerySuggested();
    }
}
