using System.Windows;
using System.Windows.Controls;
namespace ProGlassAutomation.ThemeStudio;
public partial class BusinessReferenceView : UserControl
{
    private bool? compact;
    public BusinessReferenceView() => InitializeComponent();
    private void BusinessModules_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool next = e.NewSize.Width < 1100;
        if (compact == next) return;
        compact = next;
        BusinessModules.ColumnDefinitions[1].Width = next ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        Grid.SetRow(Health, next ? 1 : 0); Grid.SetColumn(Health, next ? 0 : 1);
        Grid.SetRow(Work, next ? 2 : 1);
        Grid.SetRow(Details, next ? 3 : 1); Grid.SetColumn(Details, next ? 0 : 1);
        Grid.SetRow(Layers, next ? 4 : 2); Grid.SetColumnSpan(Layers, next ? 1 : 2);
    }
}
