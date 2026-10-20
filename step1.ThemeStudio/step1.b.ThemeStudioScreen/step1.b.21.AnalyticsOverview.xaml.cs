using System.Windows;
using System.Windows.Controls;
namespace ProGlassAutomation.ThemeStudio;
public partial class AnalyticsOverview : UserControl
{
    private bool? compact;
    public AnalyticsOverview() => InitializeComponent();
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
