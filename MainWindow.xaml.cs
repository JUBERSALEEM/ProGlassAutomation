using System.Windows;
using System.Windows.Controls.Primitives;

namespace ProGlassAutomation
{
    public partial class MainWindow : Window
    {
        private ToggleButton? _sidebarToggle;
        private ToggleButton? _expandToggle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Find the toggle buttons inside the templates
            _sidebarToggle = SidebarHost.Template?.FindName("SidebarToggle", SidebarHost) as ToggleButton;
            _expandToggle = ExpandButtonHost.Template?.FindName("ExpandToggle", ExpandButtonHost) as ToggleButton;

            if (_sidebarToggle != null && _expandToggle != null)
            {
                // Wire them together: clicking one updates the other
                _sidebarToggle.Checked += SidebarToggle_Changed;
                _sidebarToggle.Unchecked += SidebarToggle_Changed;
                _expandToggle.Checked += SidebarToggle_Changed;
                _expandToggle.Unchecked += SidebarToggle_Changed;

                // Show/hide expand button based on sidebar state
                UpdateExpandButtonVisibility();
            }
        }

        private void SidebarToggle_Changed(object sender, RoutedEventArgs e)
        {
            // Sync both toggles
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

            // Show expand button only when sidebar is collapsed
            _expandToggle.Visibility = (_sidebarToggle?.IsChecked == true)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}