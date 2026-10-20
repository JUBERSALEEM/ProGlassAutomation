using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace ProGlassAutomation.ThemeStudio;

public partial class ThemeStudioView : UserControl
{
    private int activeColumns;
    public ThemeStudioView()
    {
        InitializeComponent();
        // Keep inherited view-model bindings stable while cards move between lanes.
        foreach (FrameworkElement card in LaneOne.Children)
            card.SetBinding(DataContextProperty, new Binding(nameof(DataContext)) { Source = this });
    }

    private void Modules_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        int columns = e.NewSize.Width < 980 ? 1 : e.NewSize.Width < 1500 ? 2 : 3;
        if (e.NewSize.Width <= 0 || activeColumns == columns) return;
        activeColumns = columns;
        StackPanel[] lanes = [LaneOne, LaneTwo, LaneThree];
        FrameworkElement[] cards = [Overview, Metrics, Controls, Worklist, Appearance, TypeAndStatus, Surfaces, Tokens];
        // Keep actual controls/view models alive. Only redistribute when crossing a breakpoint;
        // native panels handle continuous resizing and height changes inside each lane.
        IInputElement? focus = cards.Any(card => card.IsKeyboardFocusWithin) ? Keyboard.FocusedElement : null;
        double width = e.NewSize.Width / columns;
        var sizes = cards.Select(card =>
        {
            card.Measure(new Size(width, double.PositiveInfinity));
            return card.DesiredSize.Height;
        }).ToArray();
        foreach (var lane in lanes) lane.Children.Clear();
        for (int i = 0; i < lanes.Length; i++)
            Modules.ColumnDefinitions[i].Width = i < columns ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        var heights = new double[columns];
        for (int i = 0; i < cards.Length; i++)
        {
            int lane = Array.IndexOf(heights, heights.Min());
            lanes[lane].Children.Add(cards[i]);
            heights[lane] += sizes[i];
        }
        focus?.Focus();
    }
}
