using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Extensions.DependencyInjection;
using ProGlassAutomation.SheetStore.Navigation;
using ProGlassAutomation.SheetStore.Views;

namespace ProGlassAutomation;

public partial class MainWindow : Window
{
    private readonly INavigationService _navigation;

    private ToggleButton? _sidebarToggle;
    private ToggleButton? _expandToggle;
    private Button? _dashboardButton;
    private Button? _sheetStoreButton;

    public MainWindow()
    {
        InitializeComponent();
        _navigation = App.ServiceProvider.GetRequiredService<INavigationService>();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Force template application so FindName works
        SidebarHost.ApplyTemplate();
        ExpandButtonHost.ApplyTemplate();

        // ─── Set up the navigation viewport ───
        try
        {
            _navigation.SetContentControl(ViewportHost);
            Debug.WriteLine("[MainWindow] Navigation viewport set successfully");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to setup navigation: {ex.Message}", "Navigation Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // ─── Find named controls inside the templates ───
        _sidebarToggle = SidebarHost.Template?.FindName("SidebarToggle", SidebarHost) as ToggleButton;
        _expandToggle = ExpandButtonHost.Template?.FindName("ExpandToggle", ExpandButtonHost) as ToggleButton;
        _dashboardButton = SidebarHost.Template?.FindName("DashboardButton", SidebarHost) as Button;
        _sheetStoreButton = SidebarHost.Template?.FindName("SheetStoreButton", SidebarHost) as Button;

        // ─── Wire sidebar collapse/expand toggles ───
        if (_sidebarToggle != null && _expandToggle != null)
        {
            _sidebarToggle.Checked += SidebarToggle_Changed;
            _sidebarToggle.Unchecked += SidebarToggle_Changed;
            _expandToggle.Checked += SidebarToggle_Changed;
            _expandToggle.Unchecked += SidebarToggle_Changed;
            UpdateExpandButtonVisibility();
        }

        // ─── Wire sidebar module buttons ───
        if (_dashboardButton != null)
            _dashboardButton.Click += Dashboard_Click;

        if (_sheetStoreButton != null)
        {
            _sheetStoreButton.Click += SheetStore_Click;
            Debug.WriteLine("[MainWindow] SheetStore button wired successfully");
        }
        else
        {
            Debug.WriteLine("[MainWindow] WARNING: SheetStore button not found in template");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // SIDEBAR TOGGLE LOGIC
    // ═══════════════════════════════════════════════════════════

    private void SidebarToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton source && _sidebarToggle != null && _expandToggle != null)
        {
            bool isChecked = source.IsChecked == true;

            if (source == _sidebarToggle && _expandToggle.IsChecked != isChecked)
                _expandToggle.IsChecked = isChecked;

            if (source == _expandToggle && _sidebarToggle.IsChecked != isChecked)
                _sidebarToggle.IsChecked = isChecked;

            UpdateExpandButtonVisibility();
        }
    }

    private void UpdateExpandButtonVisibility()
    {
        if (_expandToggle == null) return;
        _expandToggle.Visibility = (_sidebarToggle?.IsChecked == true)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    // ═══════════════════════════════════════════════════════════
    // MODULE NAVIGATION
    // ═══════════════════════════════════════════════════════════

    private void Dashboard_Click(object sender, RoutedEventArgs e)
    {
        ViewportHost.Content = new Border
        {
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0B1120")),
            Child = new TextBlock
            {
                Text = "Dashboard — Coming Soon",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private void SheetStore_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Debug.WriteLine("[MainWindow] SheetStore button clicked, navigating...");
            _navigation.NavigateTo<SheetStoreView>();
            Debug.WriteLine("[MainWindow] Navigation completed");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindow] Navigation FAILED: {ex}");
            MessageBox.Show(
                $"Failed to open SheetStore:\n\n{ex.Message}\n\nCheck the Output window for full details.",
                "Navigation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}