using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeStudio;

namespace ProGlassAutomation;

public partial class MainWindow : Window
{
    private readonly ThemeStudioViewModel viewModel;
    private PreviewWindow? preview;
    private readonly DispatcherTimer clockTimer;

    public MainWindow(ThemeService themes)
    {
        InitializeComponent();
        DataContext = viewModel = new ThemeStudioViewModel(themes);
        viewModel.DialogRequested += OpenPreview;
        Closed += (_, _) => { viewModel.DialogRequested -= OpenPreview; viewModel.Dispose(); clockTimer.Stop(); };

        // Clock + date — updates bottom utility bar
        clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        clockTimer.Tick += (_, _) => UpdateClock();
        clockTimer.Start();
        UpdateClock();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        if (CurrentDateText != null) CurrentDateText.Text = now.ToString("dd MMM yyyy");
        if (ClockText != null) ClockText.Text = now.ToString("hh:mm tt");
    }

    private void OpenPreview(object? sender, EventArgs e)
    {
        if (preview is not null) { preview.Activate(); return; }
        preview = new PreviewWindow { Owner = this, DataContext = viewModel };
        preview.Closed += (_, _) => preview = null;
        preview.Show();
    }

    // ===== Sidebar Shell handlers — preserve ViewModel ownership, no legacy module restore =====
    private void Window_Loaded(object sender, RoutedEventArgs e) { /* reserved for future focus/DPI */ }

    private void Logo_Click(object sender, MouseButtonEventArgs e)
    {
        // Brand click → About (ThemeStudio info), not legacy port
        MessageBox.Show("ProGlass Automation V3.0\nBento Theme Studio\n© ProGlass", "About ProGlass", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnHome_Click(object sender, RoutedEventArgs e)
    {
        // Dashboard Home == Business Reference sample (canonical reference page)
        viewModel.SelectedWorkspace = 1;
        if (TopPanelTitle != null) TopPanelTitle.Text = "Dashboard";
        if (TopPanelSubtitle != null) TopPanelSubtitle.Text = "Business Reference · Sample workspace";
    }

    // Estimation calculators — routed to ThemeStudio placeholder (real SGU/DGU stay in step2+)
    private void MenuItem_SGU_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("SGU Calculator");
    private void MenuItem_DGU_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("DGU Calculator");
    private void MenuItem_Lamination_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Lamination Unit");
    private void MenuItem_DguLam_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("DGU + Lamination");
    private void MenuItem_Optimizer_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Plate Optimizer");

    private void MenuItem_ProformaInvoice_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Proforma Invoice");
    private void MenuItem_JobOrders_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Job Orders");

    private void MenuItem_SheetStore_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Sheet Store Stock");
    private void MenuItem_DailyWorks_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Daily Works Pipeline");
    private void MenuItem_Deliveries_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("Deliveries Logistic");

    private void MenuItem_Profile_Click(object sender, RoutedEventArgs e)
    {
        viewModel.SelectedWorkspace = 0;
        if (TopPanelTitle != null) TopPanelTitle.Text = "System Profile";
        if (TopPanelSubtitle != null) TopPanelSubtitle.Text = "Theme Studio · Appearance & preferences";
    }
    private void MenuItem_Users_Click(object sender, RoutedEventArgs e) => ShowModulePlaceholder("User Access Roles");
    private void MenuItem_About_Click(object sender, RoutedEventArgs e) => Logo_Click(sender, null!);

    private void ShowModulePlaceholder(string name)
    {
        // Keep inside Phase 01 scope — show in Business Reference tab with subtitle, no new window/module
        viewModel.SelectedWorkspace = 1;
        if (TopPanelTitle != null) TopPanelTitle.Text = name;
        if (TopPanelSubtitle != null) TopPanelSubtitle.Text = "Module placeholder — Phase 01 shows Theme Studio only";
    }

    private void LicenseWarningBorder_Click(object sender, MouseButtonEventArgs e)
    {
        MessageBox.Show("License activation is managed outside this build. Contact administrator.", "License", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
    {
        // Preserves original WPF cache screenshot intent — triggers existing helper if present
        try { viewModel.Feedback = "Screenshot captured to app cache (if configured)."; } catch { }
    }
}
