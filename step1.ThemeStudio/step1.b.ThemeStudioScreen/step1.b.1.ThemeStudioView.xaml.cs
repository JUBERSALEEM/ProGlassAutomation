using System.Windows;
using System.Windows.Controls;

namespace ProGlassAutomation.ThemeStudio;

public partial class ThemeStudioView : UserControl
{
    private bool? wasCompact;
    public ThemeStudioView() => InitializeComponent();

    private void Modules_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool compact = e.NewSize.Width < 980;
        if (wasCompact == compact) return;
        wasCompact = compact;
        Modules.ColumnDefinitions[1].Width = compact ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
        FrameworkElement[] panels = [Overview, Appearance, Controls, TypeAndStatus, Surfaces, Tokens];
        for (int i = 0; i < panels.Length; i++)
        {
            Grid.SetRow(panels[i], compact ? i : i < 4 ? i / 2 : i - 2);
            Grid.SetColumn(panels[i], compact || i >= 4 ? 0 : i % 2);
            Grid.SetColumnSpan(panels[i], compact || i < 4 ? 1 : 2);
        }
    }
}
