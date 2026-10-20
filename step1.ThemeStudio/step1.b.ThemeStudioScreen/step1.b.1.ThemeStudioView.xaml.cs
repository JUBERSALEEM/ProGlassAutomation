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
        FrameworkElement[] panels = [Overview, Appearance, Metrics, Controls, TypeAndStatus, Worklist, Surfaces, Tokens];
        int[] rows = [0, 0, 1, 2, 2, 3, 4, 5];
        int[] columns = [0, 1, 0, 0, 1, 0, 0, 0];
        int[] spans = [1, 1, 2, 1, 1, 2, 2, 2];
        for (int i = 0; i < panels.Length; i++)
        {
            Grid.SetRow(panels[i], compact ? i : rows[i]);
            Grid.SetColumn(panels[i], compact ? 0 : columns[i]);
            Grid.SetColumnSpan(panels[i], compact ? 1 : spans[i]);
        }
    }
}
