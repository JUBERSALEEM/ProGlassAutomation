using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeStudio;

namespace ProGlassAutomation;

public partial class MainWindow : Window
{
    private readonly ThemeStudioViewModel viewModel;
    private PreviewWindow? preview;
    private readonly BlurEffect spotlightBlur = new() { Radius = 16, KernelType = KernelType.Gaussian, RenderingBias = RenderingBias.Performance };

    public MainWindow(ThemeService themes)
    {
        InitializeComponent();
        if (FindResource("AppMark") is ImageSource mark) Icon = mark;
        DataContext = viewModel = new ThemeStudioViewModel(themes);
        viewModel.DialogRequested += OpenPreview;
        OverflowSpotlight.Changed += OnSpotlight;
        SpotlightScrim.PreviewMouseLeftButtonDown += (_, _) => OverflowSpotlight.Set(false);
        Closed += (_, _) =>
        {
            OverflowSpotlight.Changed -= OnSpotlight;
            viewModel.DialogRequested -= OpenPreview;
            viewModel.Dispose();
        };
    }

    private void OnSpotlight(bool on)
    {
        if (!on)
        {
            SpotlightSnap.Effect = null;
            SpotlightSnap.Background = null;
            SpotlightScrim.Visibility = Visibility.Collapsed;
            return;
        }
        SpotlightSnap.Effect = null;
        try
        {
            var dpi = VisualTreeHelper.GetDpi(ShellBody);
            int width = Math.Max(1, (int)Math.Ceiling(ShellBody.ActualWidth * dpi.DpiScaleX));
            int height = Math.Max(1, (int)Math.Ceiling(ShellBody.ActualHeight * dpi.DpiScaleY));
            var bitmap = new RenderTargetBitmap(width, height, 96 * dpi.DpiScaleX, 96 * dpi.DpiScaleY, PixelFormats.Pbgra32);
            bitmap.Render(ShellBody);
            bitmap.Freeze();
            SpotlightSnap.Background = new ImageBrush(bitmap) { Stretch = Stretch.Fill, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top };
            SpotlightSnap.Effect = spotlightBlur;
        }
        catch (InvalidOperationException)
        {
            SpotlightSnap.SetResourceReference(Border.BackgroundProperty, "ApplicationBackground");
        }
        SpotlightScrim.Visibility = Visibility.Visible;
    }

    protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
    {
        e.Handled = true;
    }

    private void OpenPreview(object? sender, EventArgs e)
    {
        if (preview is not null) { preview.Activate(); return; }
        preview = new PreviewWindow { Owner = this, DataContext = viewModel };
        preview.Closed += (_, _) => preview = null;
        preview.Show();
    }
}
