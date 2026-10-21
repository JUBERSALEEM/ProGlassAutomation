using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ProGlassAutomation.Customization;
using ProGlassAutomation.ThemeBento;

namespace ProGlassAutomation.ThemeStudio;
public partial class AnalyticsOverview : UserControl
{
    public static readonly DependencyProperty CompactProperty = DependencyProperty.Register(
        nameof(Compact), typeof(bool), typeof(AnalyticsOverview),
        new PropertyMetadata(false));
    private ThemeStudioViewModel? vm;
    public AnalyticsOverview()
    {
        InitializeComponent();
        Loaded += (_, _) => ApplyLayout();
    }
    public bool Compact { get => (bool)GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == CompactProperty && IsLoaded) ApplyLayout();
    }
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        DataContextChanged += (_, _) => HookVm();
        Loaded += (_, _) => HookVm();
    }
    private void HookVm()
    {
        if (vm != null) vm.PropertyChanged -= VmChanged;
        vm = DataContext as ThemeStudioViewModel;
        if (vm != null) vm.PropertyChanged += VmChanged;
        if (IsLoaded) ApplyLayout();
    }
    private void VmChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeStudioViewModel.SelectedLayoutPreset))
            ApplyLayout();
    }
    private void ApplyLayout()
    {
        if (AnalyticsGrid is null) return;
        AnalyticsGrid.FillAvailable = false;
        AnalyticsGrid.Columns = 4;
        // Right-side board: equal 1×1 tiles (4 columns), then a compact table on Home.
        Show(KpiRevenue, 0, 0, 1, 1);
        Show(KpiOrders, 1, 0, 1, 1);
        Show(KpiConversion, 2, 0, 1, 1);
        Show(StatusCard, 3, 0, 1, 1);
        Show(RevenueCard, 0, 1, 1, 1);
        Show(MixCard, 1, 1, 1, 1);
        Show(TargetCard, 2, 1, 1, 1);
        Show(TrendCard, 3, 1, 1, 1);
        if (Compact)
        {
            Hide(ActivityCard); Hide(TableCard);
            return;
        }
        Show(ActivityCard, 0, 2, 1, 1);
        Show(TableCard, 1, 2, 3, 2);
        if (vm is null) return;
        switch (vm.SelectedLayoutPreset)
        {
            case BentoLayoutPreset.Executive:
                Hide(ActivityCard);
                Show(TableCard, 0, 2, 4, 2);
                break;
            case BentoLayoutPreset.AnalyticsFocus:
                Hide(ActivityCard);
                Show(TableCard, 0, 2, 4, 2);
                break;
            case BentoLayoutPreset.CompactDense:
                Hide(TableCard); Hide(ActivityCard);
                break;
        }
    }
    private static void Show(UIElement tile, int column, int row, int columnSpan, int rowSpan)
    {
        tile.Visibility = Visibility.Visible;
        BentoGrid.SetColumn(tile, column);
        BentoGrid.SetRow(tile, row);
        BentoGrid.SetColumnSpan(tile, columnSpan);
        BentoGrid.SetRowSpan(tile, rowSpan);
    }
    private static void Hide(UIElement tile) => tile.Visibility = Visibility.Collapsed;
}
