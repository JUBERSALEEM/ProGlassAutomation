using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ProGlassAutomation.Customization;
using ProGlassAutomation.ThemeBento;

namespace ProGlassAutomation.ThemeStudio;
public partial class AnalyticsOverview : UserControl
{
    private ThemeStudioViewModel? vm;
    public AnalyticsOverview() => InitializeComponent();
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
        ApplyLayoutPreset();
    }
    private void VmChanged(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeStudioViewModel.SelectedLayoutPreset))
            ApplyLayoutPreset();
    }
    private void ApplyLayoutPreset()
    {
        if (vm == null) return;
        BentoGrid.SetColumn(RevenueCard, 0); BentoGrid.SetRow(RevenueCard, 1); BentoGrid.SetColumnSpan(RevenueCard, 2); BentoGrid.SetRowSpan(RevenueCard, 2);
        BentoGrid.SetColumn(MixCard, 2); BentoGrid.SetRow(MixCard, 1); BentoGrid.SetColumnSpan(MixCard, 1); BentoGrid.SetRowSpan(MixCard, 2);
        BentoGrid.SetColumnSpan(TableCard, 4);
        switch (vm.SelectedLayoutPreset)
        {
            case BentoLayoutPreset.Executive:
                ActivityCard.Visibility = Visibility.Collapsed; StatusCard.Visibility = Visibility.Visible; TableCard.Visibility = Visibility.Visible;
                BentoGrid.SetColumnSpan(MixCard, 2); TargetCard.Visibility = Visibility.Collapsed;
                break;
            case BentoLayoutPreset.AnalyticsFocus:
                ActivityCard.Visibility = Visibility.Collapsed; StatusCard.Visibility = Visibility.Collapsed; TableCard.Visibility = Visibility.Visible;
                TargetCard.Visibility = Visibility.Visible;
                break;
            case BentoLayoutPreset.CompactDense:
                TableCard.Visibility = Visibility.Collapsed; ActivityCard.Visibility = Visibility.Collapsed; TargetCard.Visibility = Visibility.Visible; StatusCard.Visibility = Visibility.Visible;
                break;
            default:
                TableCard.Visibility = Visibility.Visible; ActivityCard.Visibility = Visibility.Visible; StatusCard.Visibility = Visibility.Visible; TargetCard.Visibility = Visibility.Visible;
                break;
        }
    }
}
