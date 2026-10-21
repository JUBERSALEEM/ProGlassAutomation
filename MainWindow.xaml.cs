using System.Windows;
using System.Windows.Media.Effects;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeStudio;

namespace ProGlassAutomation;

public partial class MainWindow : Window
{
    private readonly ThemeStudioViewModel viewModel;
    private PreviewWindow? preview;
    private readonly BlurEffect spotlightBlur = new() { Radius = 16 };

    public MainWindow(ThemeService themes)
    {
        InitializeComponent();
        DataContext = viewModel = new ThemeStudioViewModel(themes);
        viewModel.DialogRequested += OpenPreview;
        OverflowSpotlight.Changed += OnSpotlight;
        Closed += (_, _) =>
        {
            OverflowSpotlight.Changed -= OnSpotlight;
            viewModel.DialogRequested -= OpenPreview;
            viewModel.Dispose();
        };
    }

    private void OnSpotlight(bool on)
    {
        ShellBody.Effect = on ? spotlightBlur : null;
        SpotlightScrim.Visibility = on ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OpenPreview(object? sender, EventArgs e)
    {
        if (preview is not null) { preview.Activate(); return; }
        preview = new PreviewWindow { Owner = this, DataContext = viewModel };
        preview.Closed += (_, _) => preview = null;
        preview.Show();
    }
}
