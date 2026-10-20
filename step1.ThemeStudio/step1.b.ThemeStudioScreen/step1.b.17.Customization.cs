using System.Windows.Input;
using ProGlassAutomation.ThemeBento;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeStudio;

public sealed partial class ThemeStudioViewModel
{
    private BentoPreferences preferences = new();
    private int selectedWorkspace;
    public IReadOnlyList<BentoAccent> AccentChoices { get; } = Enum.GetValues<BentoAccent>();
    public IReadOnlyList<BentoDensity> DensityChoices { get; } = Enum.GetValues<BentoDensity>();
    public IReadOnlyList<BentoShape> ShapeChoices { get; } = Enum.GetValues<BentoShape>();
    public IReadOnlyList<BentoType> TypeChoices { get; } = Enum.GetValues<BentoType>();
    public BentoAccent SelectedAccent { get => preferences.Accent; set => Customize(preferences with { Accent = value }); }
    public BentoDensity SelectedDensity { get => preferences.Density; set => Customize(preferences with { Density = value }); }
    public BentoShape SelectedShape { get => preferences.Shape; set => Customize(preferences with { Shape = value }); }
    public BentoType SelectedTypography { get => preferences.Typography; set => Customize(preferences with { Typography = value }); }
    public bool CanCustomize => SelectedTheme == ThemeService.Bento;
    public string DesignSummary => !CanCustomize ? "Theme-default settings" : $"{SelectedAccent} / {SelectedDensity} / {SelectedShape} / {SelectedTypography}";
    public int SelectedWorkspace
    {
        get => selectedWorkspace;
        set { if (value is 0 or 1) { selectedWorkspace = value; Changed(nameof(SelectedWorkspace)); } }
    }
    public ICommand ResetDesign { get; private set; } = null!;
    public ICommand ShowBusiness { get; private set; } = null!;

    private void InitializeCustomization()
    {
        ResetDesign = new DelegateCommand(() => Customize(new BentoPreferences()), () => CanCustomize);
        ShowBusiness = new DelegateCommand(() => SelectedWorkspace = 1);
    }

    private void Customize(BentoPreferences candidate)
    {
        if (!CanCustomize || candidate == preferences) return;
        try
        {
            themes.Apply(SelectedTheme, themes.ActiveAppearance,
                (package, mode) => BentoCustomization.Apply(package, mode, candidate));
            preferences = candidate;
            ThemeError = "";
            Feedback = "Design applied to Studio, Business Reference and the preview window. Session only.";
        }
        catch (ThemeNotificationException error)
        {
            preferences = candidate; // Package was activated; only notification failed.
            ThemeError = error.Message;
        }
        catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.IO.IOException or System.Windows.Markup.XamlParseException)
        {
            ThemeError = $"Design unchanged: {error.Message}";
        }
        foreach (string name in new[] { nameof(SelectedAccent), nameof(SelectedDensity), nameof(SelectedShape), nameof(SelectedTypography), nameof(DesignSummary), nameof(ThemeError) }) Changed(name);
    }
}
