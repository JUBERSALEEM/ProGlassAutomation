using System.Windows.Input;
using ProGlassAutomation.Customization;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeStudio;

public sealed partial class ThemeStudioViewModel
{
    private BentoPreferences preferences = new();
    private BentoCustomizationState extended = new();
    private SavedCustomization lastSaved = CustomizationStore.Load();
    private BentoCustomizationState draft => extended;
    private int selectedWorkspace = 1;
    private bool isMenuOpen = true;
    private string customizationCategory = "Appearance";

    private string selectedComponentId = "";
    private string presetNameInput = "";
    private string? lastSavedSnapshot;
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
    public bool CanCustomize => SelectedTheme == ThemeService.Bento;
    public string DesignSummary => !CanCustomize ? "Theme-default settings" : $"{SelectedAccent} / {SelectedDensity} / {SelectedShape} / {SelectedTypography} · {SelectedLayoutPreset}";
    public bool IsDirty => extended != lastSaved.Preferences;
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
        preferences = extended.ToLegacy();
        var restored = ThemeService.InstalledThemes.Contains(lastSaved.Theme) ? lastSaved.Theme : ThemeService.Bento;
        try { themes.Apply(restored, lastSaved.Appearance, restored == ThemeService.Bento ? (p,m)=>BentoCustomization.ApplyExtended(p,m,extended) : null); } catch {}
        ResetDesign = new DelegateCommand(() => ApplyState(new BentoCustomizationState()), () => CanCustomize);
        ToggleMenu = new DelegateCommand(() => IsMenuOpen = !IsMenuOpen);
        ToggleAppearance = new DelegateCommand(() => { if (IsLight) IsDark = true; else IsLight = true; });
        ShowStudio = new DelegateCommand(() => SelectedWorkspace = 0);
        ShowBusiness = new DelegateCommand(() => SelectedWorkspace = 1);
        ApplyPreview = new DelegateCommand(() => { try { themes.Apply(SelectedTheme, themes.ActiveAppearance, (pkg,mode)=>BentoCustomization.ApplyExtended(pkg,mode,extended)); ThemeError=""; Feedback="Preview applied — unsaved changes present."; } catch(Exception ex){ ThemeError=ex.Message; } Changed(nameof(ThemeError)); }, () => CanCustomize);
        SaveCustomization = new DelegateCommand(() => { lastSaved = new SavedCustomization(2, themes.ActiveAppearance, extended, lastSaved.ChartOptions, lastSaved.ComponentStyles, lastSaved.CustomPresets, themes.ActiveTheme); CustomizationStore.Save(lastSaved); Feedback="Customization saved — will restore on next launch."; Changed(nameof(IsDirty)); }, () => CanCustomize && extended.Validate() is null);
        DiscardChanges = new DelegateCommand(() => { ApplyState(lastSaved.Preferences); Feedback="Discarded unsaved changes."; }, () => IsDirty);
        ResetSection = new DelegateCommand(() => { var defaults = new BentoCustomizationState(); var next = customizationCategory switch { "Appearance" => extended with { Canvas = defaults.Canvas, SurfaceStyle = defaults.SurfaceStyle, TextContrast = defaults.TextContrast, BorderIntensity = defaults.BorderIntensity, Shadow = defaults.Shadow }, "Layout" => extended with { LayoutPreset = defaults.LayoutPreset, Spacing = defaults.Spacing, CardPadding = defaults.CardPadding, HeroCompact = defaults.HeroCompact }, "Typography" => extended with { Typography = defaults.Typography, FontWeight = defaults.FontWeight, Spacing = defaults.Spacing }, _ => defaults }; ApplyState(next); Feedback=$"Reset {customizationCategory} to defaults."; }, () => CanCustomize);
        ResetTheme = new DelegateCommand(() => { ApplyState(new BentoCustomizationState()); lastSaved = new SavedCustomization(); CustomizationStore.Save(lastSaved); Feedback="Theme restored to Bento defaults."; }, () => CanCustomize);
        PreviewDefault = new DelegateCommand(() => { if (lastSavedSnapshot == null) { lastSavedSnapshot = System.Text.Json.JsonSerializer.Serialize(extended); ApplyState(new BentoCustomizationState()); Feedback="Previewing defaults — toggle again to restore."; } else { var prev = System.Text.Json.JsonSerializer.Deserialize<BentoCustomizationState>(lastSavedSnapshot); lastSavedSnapshot = null; if(prev!=null) ApplyState(prev); Feedback="Restored your preview."; } }, () => CanCustomize);
        ApplyPreset = new DelegateCommand<string>(name => { var preset = BuiltInPresets.Concat(CustomPresets).FirstOrDefault(p=>p.Name==name); if(preset!=null){ ApplyState(preset.State); if(preset.Appearance!=themes.ActiveAppearance) SelectAppearance(ThemeService.Bento, preset.Appearance); Feedback=$"Preset applied: {preset.Name}"; }});
        SavePreset = new DelegateCommand(() => { var n = presetNameInput.Trim(); if(string.IsNullOrEmpty(n) || PresetCatalog.IsBuiltIn(n)){ ThemeError="Enter unique name not conflicting with built-in presets."; Changed(nameof(ThemeError)); return; } var list = (lastSaved.CustomPresets ?? []).ToList(); list.RemoveAll(p=>p.Name==n); list.Add(new NamedPreset(n, extended, themes.ActiveAppearance)); lastSaved = lastSaved with { CustomPresets = list }; CustomizationStore.Save(lastSaved); PresetNameInput=""; Feedback=$"Preset saved: {n}"; Changed(nameof(ThemeError)); }, () => !string.IsNullOrWhiteSpace(presetNameInput));
        DeletePreset = new DelegateCommand<string>(name => { if(PresetCatalog.IsBuiltIn(name)) return; var list = (lastSaved.CustomPresets ?? []).ToList(); list.RemoveAll(p=>p.Name==name); lastSaved = lastSaved with { CustomPresets = list }; CustomizationStore.Save(lastSaved); Feedback=$"Deleted preset: {name}"; });
        RenamePreset = new DelegateCommand<(string oldName,string newName)>(pair => { var n=pair.newName.Trim(); if(string.IsNullOrEmpty(n)||PresetCatalog.IsBuiltIn(n)) return; var list = (lastSaved.CustomPresets ?? []).ToList(); var idx=list.FindIndex(p=>p.Name==pair.oldName); if(idx>=0){ list[idx]=list[idx] with { Name=n }; lastSaved = lastSaved with { CustomPresets = list }; CustomizationStore.Save(lastSaved); Feedback=$"Renamed to {n}"; }});
    }
    private void Customize(BentoPreferences candidate)
    {
        if (!CanCustomize || candidate == preferences) return;
        UpdateExtended(new BentoCustomizationState(candidate.Accent, candidate.Density, candidate.Shape, candidate.Typography, extended.Canvas, extended.SurfaceStyle, extended.TextContrast, extended.BorderIntensity, extended.Shadow, extended.Spacing, extended.CardPadding, extended.FontWeight, extended.LayoutPreset, extended.HeroCompact, extended.ShowKpiTrend, extended.ShowTableStatus));
    }
    private void UpdateExtended(BentoCustomizationState next)
    {
        if (!CanCustomize || next == extended) return;
        ApplyState(next);
    }
    private void ApplyState(BentoCustomizationState next)
    {
        try
        {
            themes.Apply(SelectedTheme, themes.ActiveAppearance, (pkg,mode)=>BentoCustomization.ApplyExtended(pkg,mode,next));
            extended = next; preferences = next.ToLegacy(); ThemeError=""; Feedback="Live preview updated — use Save to persist.";
        }
        catch (ThemeNotificationException error) { extended = next; preferences = next.ToLegacy(); ThemeError = error.Message; }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.IO.IOException or System.Windows.Markup.XamlParseException) { ThemeError=$"Design unchanged: {error.Message}"; }
        foreach (var n in new[] { nameof(SelectedAccent), nameof(SelectedDensity), nameof(SelectedShape), nameof(SelectedTypography), nameof(SelectedCanvas), nameof(SelectedSurface), nameof(SelectedTextContrast), nameof(SelectedBorder), nameof(SelectedShadow), nameof(SelectedSpacing), nameof(SelectedPadding), nameof(SelectedWeight), nameof(SelectedLayoutPreset), nameof(HeroCompact), nameof(ShowKpiTrend), nameof(DesignSummary), nameof(ThemeError), nameof(IsDirty) }) Changed(n);
    }
}
