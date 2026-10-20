using System.Windows;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeStudio;

namespace ProGlassAutomation;

public partial class MainWindow : Window
{
    private readonly ThemeStudioViewModel viewModel;
    private PreviewWindow? preview;

    public MainWindow(ThemeService themes)
    {
        InitializeComponent();
        DataContext = viewModel = new ThemeStudioViewModel(themes);
        viewModel.DialogRequested += OpenPreview;
        Closed += (_, _) => { viewModel.DialogRequested -= OpenPreview; viewModel.Dispose(); };
    }

    private void OpenPreview(object? sender, EventArgs e)
    {
        if (preview is not null) { preview.Activate(); return; }
        preview = new PreviewWindow { Owner = this, DataContext = viewModel };
        preview.Closed += (_, _) => preview = null;
        preview.Show();
    }
}
