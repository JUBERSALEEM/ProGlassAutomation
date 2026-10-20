using System.Windows;
using System.Windows.Controls;
namespace ProGlassAutomation.ThemeStudio;
public partial class ThemeStudioView : UserControl
{
    private bool? compact;
    public ThemeStudioView() => InitializeComponent();
    private void Modules_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool next = e.NewSize.Width < 1040;
        if (compact == next) return;
        compact = next;
        Modules.ColumnDefinitions[1].Width = next ? new GridLength(0) : new GridLength(3, GridUnitType.Star);
        Modules.ColumnDefinitions[0].Width = next ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
        if (next) Settings.ClearValue(WidthProperty);
        else Settings.SetResourceReference(WidthProperty, "StudioSidebarWidth");
        Grid.SetRowSpan(Settings, next ? 1 : 2);
        Grid.SetRow(Controls, next ? 1 : 0); Grid.SetColumn(Controls, next ? 0 : 1);
        Grid.SetRow(Signals, next ? 2 : 1); Grid.SetColumn(Signals, next ? 0 : 1);
        Grid.SetRow(Tokens, next ? 3 : 2); Grid.SetColumnSpan(Tokens, next ? 1 : 2);
    }
}
