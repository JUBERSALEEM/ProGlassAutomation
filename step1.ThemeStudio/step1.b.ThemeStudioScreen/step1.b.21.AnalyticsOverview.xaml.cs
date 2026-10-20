using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
namespace ProGlassAutomation.ThemeStudio;
public partial class AnalyticsOverview : UserControl
{
    private bool? compact;
    public AnalyticsOverview() => InitializeComponent();
    private ProGlassAutomation.ThemeStudio.ThemeStudioViewModel? vm;
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        DataContextChanged += (s,_) => HookVm();
        Loaded += (s,_) => HookVm();
    }
    private void HookVm()
    {
        if (vm != null) vm.PropertyChanged -= VmChanged;
        vm = DataContext as ProGlassAutomation.ThemeStudio.ThemeStudioViewModel;
        if (vm != null) vm.PropertyChanged += VmChanged;
        ApplyLayoutPreset();
    }
    private void VmChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProGlassAutomation.ThemeStudio.ThemeStudioViewModel.SelectedLayoutPreset) || e.PropertyName == nameof(ProGlassAutomation.ThemeStudio.ThemeStudioViewModel.HeroCompact) || e.PropertyName == nameof(ProGlassAutomation.ThemeStudio.ThemeStudioViewModel.ShowKpiTrend))
            ApplyLayoutPreset();
    }
    private void ApplyLayoutPreset()
    {
        if (vm == null) return;
        // Five presets must genuinely change placement/proportions/visibility, not just title
        switch (vm.SelectedLayoutPreset)
        {
            case ProGlassAutomation.Customization.BentoLayoutPreset.Executive:
                AnalyticsGrid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                AnalyticsGrid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
                Grid.SetColumnSpan(TableCard, 2); TableCard.Visibility = Visibility.Visible; ActivityCard.Visibility = Visibility.Collapsed;
                break;
            case ProGlassAutomation.Customization.BentoLayoutPreset.AnalyticsFocus:
                AnalyticsGrid.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
                AnalyticsGrid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
                ActivityCard.Visibility = Visibility.Collapsed; StatusCard.Visibility = Visibility.Collapsed; TableCard.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(TableCard, 2); break;
            case ProGlassAutomation.Customization.BentoLayoutPreset.ProductionOps:
                Grid.SetColumnSpan(TableCard, 2); TableCard.Visibility = Visibility.Visible; ActivityCard.Visibility = Visibility.Visible; StatusCard.Visibility = Visibility.Visible;
                AnalyticsGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star); AnalyticsGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                break;
            case ProGlassAutomation.Customization.BentoLayoutPreset.CompactDense:
                AnalyticsGrid.ColumnDefinitions[1].Width = new GridLength(0); TableCard.Visibility = Visibility.Collapsed;
                Grid.SetColumn(MixCard, 0); Grid.SetRow(MixCard, 2);
                ActivityCard.Visibility = Visibility.Collapsed;
                break;
            default: // Balanced
                AnalyticsGrid.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
                AnalyticsGrid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
                TableCard.Visibility = Visibility.Visible; ActivityCard.Visibility = Visibility.Visible; StatusCard.Visibility = Visibility.Visible;
                Grid.SetColumn(MixCard, 1); Grid.SetRow(MixCard, 0);
                Grid.SetColumnSpan(TableCard, 2);
                break;
        }
        // HeroCompact in studio affects center hero in CustomizeWorkspace, not this Business reference overview
    }
    private void AnalyticsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool next = e.NewSize.Width < 1000;
        if (compact == next) return;
        compact = next;
        AnalyticsGrid.ColumnDefinitions[1].Width = next ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
        while (AnalyticsGrid.RowDefinitions.Count < 7) AnalyticsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetColumn(MixCard, next ? 0 : 1); Grid.SetRow(MixCard, next ? 1 : 0);
        Grid.SetRow(TrendCard, next ? 2 : 1); Grid.SetColumn(TargetCard, next ? 0 : 1); Grid.SetRow(TargetCard, next ? 3 : 1);
        Grid.SetRow(TableCard, next ? 4 : 2); Grid.SetColumnSpan(TableCard, next ? 1 : 2);
        Grid.SetRow(ActivityCard, next ? 5 : 3); Grid.SetRow(StatusCard, next ? 6 : 3); Grid.SetColumn(StatusCard, next ? 0 : 1);
    }
}
