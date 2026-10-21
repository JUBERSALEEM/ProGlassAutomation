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
        if (Compact)
        {
            // Theme Studio: many small tiles in the same slot — not two stretched charts.
            AnalyticsGrid.Columns = 4;
            Show(KpiRevenue, 0, 0, 1, 1);
            Show(KpiOrders, 1, 0, 1, 1);
            Show(KpiConversion, 2, 0, 1, 1);
            Show(StatusCard, 3, 0, 1, 1);
            Show(RevenueCard, 0, 1, 2, 2);
            Show(MixCard, 2, 1, 1, 2);
            Show(TargetCard, 3, 1, 1, 2);
            Hide(TrendCard); Hide(ActivityCard); Hide(TableCard);
            return;
        }
        AnalyticsGrid.Columns = 4;
        Show(KpiRevenue, 0, 0, 2, 1);
        Show(KpiOrders, 2, 0, 1, 1);
        Show(KpiConversion, 3, 0, 1, 1);
        Show(RevenueCard, 0, 1, 2, 2);
        Show(MixCard, 2, 1, 1, 2);
        Show(TargetCard, 3, 1, 1, 2);
        Show(TrendCard, 0, 3, 2, 2);
        Show(ActivityCard, 2, 3, 1, 2);
        Show(StatusCard, 3, 3, 1, 2);
        Show(TableCard, 0, 5, 4, 2);
        if (vm is null) return;
        switch (vm.SelectedLayoutPreset)
        {
            case BentoLayoutPreset.Executive:
                Hide(ActivityCard); Hide(TargetCard);
                Show(MixCard, 2, 1, 2, 2);
                break;
            case BentoLayoutPreset.AnalyticsFocus:
                Hide(ActivityCard); Hide(StatusCard);
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
