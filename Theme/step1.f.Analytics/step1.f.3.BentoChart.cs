using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Data;
using System.Windows.Threading;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.WPF;

namespace ProGlassAutomation.Analytics;

/// <summary>Reusable chart presentation; no demo data, business arithmetic, palette or resource lookup.</summary>
public sealed partial class BentoChart
{
    private readonly CartesianChart cartesian = new() { AnimationsSpeed = TimeSpan.Zero, LegendPosition = LegendPosition.Hidden };
    private readonly PieChart pie = new() { AnimationsSpeed = TimeSpan.Zero, LegendPosition = LegendPosition.Hidden };
    private readonly Grid plot = new();
    private readonly Grid root = new() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
    private readonly WrapPanel legend = new();
    private readonly DockPanel footer = new() { LastChildFill = true };
    private readonly TextBlock message = new() { TextWrapping = TextWrapping.Wrap };
    private readonly TextBlock center = new() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
    private readonly CheckBox inspectToggle = new() { Content = "Show source values" };
    private readonly TextBlock values = new() { TextWrapping = TextWrapping.Wrap, Visibility = Visibility.Collapsed };
    private readonly TextBlock summary = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Dictionary<string, bool> visibility = new();
    private ISeries[] rendered = [];
    private bool dataDirty = true;
    private bool queued;
    private bool sideLegend;
    public IReadOnlyList<ISeries> RenderedSeries => rendered;
    public string DisplayMessage => message.Text;
    static BentoChart()
    {
        // Skia GPU surfaces clear to black and BitmapCache cannot keep a transparent plot.
        LiveCharts.RenderingSettings.UseGPU = false;
        LiveCharts.RenderingSettings.TryUseVSync = false;
    }
    public BentoChart()
    {
        VerticalContentAlignment = VerticalAlignment.Stretch;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        cartesian.HorizontalAlignment = HorizontalAlignment.Stretch;
        cartesian.VerticalAlignment = VerticalAlignment.Stretch;
        cartesian.MinHeight = 0;
        pie.HorizontalAlignment = HorizontalAlignment.Stretch;
        pie.VerticalAlignment = VerticalAlignment.Stretch;
        pie.MinHeight = 0;
        plot.MinHeight = 0;
        cartesian.Background = System.Windows.Media.Brushes.Transparent;
        pie.Background = System.Windows.Media.Brushes.Transparent;
        plot.Background = System.Windows.Media.Brushes.Transparent;
        Background = System.Windows.Media.Brushes.Transparent;
        BorderBrush = System.Windows.Media.Brushes.Transparent;
        plot.Children.Add(cartesian);
        plot.Children.Add(pie);
        plot.Children.Add(center);
        inspectToggle.VerticalAlignment = VerticalAlignment.Center;
        inspectToggle.HorizontalAlignment = HorizontalAlignment.Left;
        inspectToggle.MinHeight = 0;
        inspectToggle.Padding = new Thickness(4, 0, 0, 0);
        values.Margin = new Thickness(0, 8, 0, 8);
        DockPanel.SetDock(inspectToggle, Dock.Right);
        footer.Children.Add(inspectToggle);
        footer.Children.Add(legend);
        root.Children.Add(plot);
        root.Children.Add(footer);
        root.Children.Add(message);
        root.Children.Add(summary);
        root.Children.Add(values);
        LayoutChrome(circular: false);
        inspectToggle.Checked += (_, _) => { values.Visibility = Visibility.Visible; BudgetPlot(); };
        inspectToggle.Unchecked += (_, _) => { values.Visibility = Visibility.Collapsed; BudgetPlot(); };
        Content = root;
        center.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(CenterFontSize)) { Source = this });
        values.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(AxisBrush)) { Source = this });
        summary.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(AxisBrush)) { Source = this });
        message.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(AxisBrush)) { Source = this });
        Loaded += (_, _) => Schedule(true);
        IsVisibleChanged += (_, _) => { if (IsVisible) Schedule(false); };
        SizeChanged += (_, _) => { if (IsLoaded) { BudgetPlot(); Schedule(false); } };
    }

    /// <summary>
    /// Donut: plot | four-tick legend, then inspect toggle, then source values.
    /// The long "Show source values" label must not share the legend column (it zeroed the ticks).
    /// Cartesian: plot fills the card; PlotHeight is not a MaxHeight cap (that left hollow Chart Lab / KPI strips).
    /// </summary>
    private void LayoutChrome(bool circular)
    {
        bool side = circular && ShowLegend;
        if (side == sideLegend && root.RowDefinitions.Count == 5) return;
        sideLegend = side;
        root.RowDefinitions.Clear();
        root.ColumnDefinitions.Clear();
        legend.Orientation = side ? Orientation.Vertical : Orientation.Horizontal;
        legend.VerticalAlignment = VerticalAlignment.Top;
        if (side)
        {
            Host(root, legend);
            Host(root, inspectToggle);
            inspectToggle.HorizontalAlignment = HorizontalAlignment.Left;
            inspectToggle.Margin = new Thickness(0, 8, 0, 0);
            footer.Visibility = Visibility.Collapsed;
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 96 });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 108 });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Place(plot, 0, 0, 1);
            Place(legend, 0, 1, 1);
            Place(inspectToggle, 1, 0, 2);
            Place(message, 2, 0, 2);
            Place(summary, 3, 0, 2);
            Place(values, 4, 0, 2);
            BudgetPlot();
        }
        else
        {
            Host(footer, inspectToggle);
            Host(footer, legend);
            DockPanel.SetDock(inspectToggle, Dock.Right);
            inspectToggle.HorizontalAlignment = HorizontalAlignment.Right;
            inspectToggle.Margin = new Thickness(4, 0, 0, 0);
            footer.Visibility = Visibility.Visible;
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Place(plot, 0, 0, 1);
            Place(footer, 1, 0, 1);
            Place(message, 2, 0, 1);
            Place(summary, 3, 0, 1);
            Place(values, 4, 0, 1);
            BudgetPlot();
        }
    }

    /// <summary>
    /// Mix sits in a fixed Bento cell. LiveCharts pie min-size plus Auto source lines overflow and clip.
    /// Cap the plot row so the four source-value lines always measure inside the card.
    /// </summary>
    private void BudgetPlot()
    {
        if (root.RowDefinitions.Count < 1) return;
        var plotRow = root.RowDefinitions[0];
        if (values.Visibility != Visibility.Visible || ActualHeight <= 1)
        {
            plotRow.MaxHeight = double.PositiveInfinity;
            return;
        }
        double width = Math.Max(1, ActualWidth);
        inspectToggle.Measure(new Size(width, double.PositiveInfinity));
        values.Measure(new Size(width, double.PositiveInfinity));
        double reserved = values.DesiredSize.Height + 16;
        if (sideLegend) reserved += inspectToggle.DesiredSize.Height;
        else
        {
            footer.Measure(new Size(width, double.PositiveInfinity));
            reserved += footer.DesiredSize.Height;
        }
        plotRow.MaxHeight = Math.Max(88, ActualHeight - reserved);
    }

    private static void Host(Panel parent, FrameworkElement child)
    {
        if (ReferenceEquals(child.Parent, parent)) return;
        if (child.Parent is Panel current) current.Children.Remove(child);
        parent.Children.Add(child);
    }

    private static void Place(UIElement child, int row, int column, int columnSpan)
    {
        Grid.SetRow(child, row);
        Grid.SetColumn(child, column);
        Grid.SetColumnSpan(child, columnSpan);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == FontSizeProperty || e.Property == FontFamilyProperty || e.Property == ShowLegendProperty) Schedule(false);
        if (e.Property == DataProperty) Schedule(true);
    }

    private void Schedule(bool rebuild)
    {
        dataDirty |= rebuild;
        if (queued || !IsLoaded) return;
        queued = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
        {
            queued = false;
            if (!IsLoaded) return;
            Refresh();
        }));
    }

    private void Refresh()
    {
        summary.Text = Data?.Summary ?? "No chart data supplied.";
        AutomationProperties.SetName(this, Data?.Title ?? "Chart");
        AutomationProperties.SetHelpText(this, summary.Text);
        string? error = Data?.ValidationError();
        message.Text = Data?.State == ChartState.Loading ? "Loading data…" : Data?.State == ChartState.Error
            ? "Data could not be loaded. Use the page's retry action." : error ?? (Data is null || Data.IsEmpty ? "No observations available for this selection." : "");
        bool ready = message.Text.Length == 0;
        bool circular = Data?.Kind is ChartKind.Donut or ChartKind.Pie;
        LayoutChrome(circular);
        cartesian.Visibility = ready && !circular ? Visibility.Visible : Visibility.Collapsed;
        pie.Visibility = ready && circular ? Visibility.Visible : Visibility.Collapsed;
        center.Visibility = ready && Data?.Kind == ChartKind.Donut ? Visibility.Visible : Visibility.Collapsed;
        center.Text = Data?.CenterText ?? "";
        if (!ready)
        {
            cartesian.Series = []; pie.Series = []; rendered = []; legend.Children.Clear();
            dataDirty = true;
            inspectToggle.Visibility = Visibility.Collapsed;
            legend.Visibility = Visibility.Collapsed;
            footer.Visibility = Visibility.Collapsed;
            values.Text = ""; message.Visibility = Visibility.Visible;
            return;
        }
        inspectToggle.Visibility = ShowLegend ? Visibility.Visible : Visibility.Collapsed;
        legend.Visibility = ShowLegend ? Visibility.Visible : Visibility.Collapsed;
        footer.Visibility = ShowLegend && !sideLegend ? Visibility.Visible : Visibility.Collapsed;
        message.Visibility = ready ? Visibility.Collapsed : Visibility.Visible;
        if (dataDirty)
        {
            values.Text = string.Join("\n", Data!.Series.Select(s => s.Name + ": " + string.Join(" / ", s.Values.Select((v, i) => $"{(i < Data.Categories.Count ? Data.Categories[i] : s.Name)} {(v.HasValue ? v.Value.ToString("N1") : "missing")} {Data.Unit}"))));
            rendered = BuildSeries(Data!);
            cartesian.Series = circular ? [] : rendered;
            pie.Series = circular ? rendered : [];
            BuildLegend(Data!);
            dataDirty = false;
        }
        summary.Visibility = legend.Children.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        ApplyPaints(Data!);
    }

    private void BuildLegend(ChartDataset data)
    {
        legend.Children.Clear();
        foreach (string stale in visibility.Keys.Except(data.Series.Select(s => s.Name)).ToArray()) visibility.Remove(stale);
        for (int i = 0; i < rendered.Length; i++)
        {
            var series = rendered[i];
            var swatch = new Border { VerticalAlignment = VerticalAlignment.Center };
            swatch.SetBinding(BackgroundProperty, new Binding($"Series{i + 1}") { Source = this });
            swatch.SetBinding(WidthProperty, new Binding(nameof(FontSize)) { Source = this });
            swatch.SetBinding(HeightProperty, new Binding(nameof(FontSize)) { Source = this });
            var caption = new TextBlock { Text = "  " + data.Series[i].Name };
            caption.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });
            var label = new StackPanel { Orientation = Orientation.Horizontal };
            label.Children.Add(swatch); label.Children.Add(caption);
            var item = new CheckBox
            {
                Content = label,
                IsChecked = visibility.GetValueOrDefault(data.Series[i].Name, true),
                MinHeight = 0,
                FontSize = FontSize,
                Padding = new Thickness(2, 0, 8, 0),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            series.IsVisible = item.IsChecked == true;
            AutomationProperties.SetName(item, $"Show series {data.Series[i].Name}");
            item.Checked += (_, _) => { series.IsVisible = true; visibility[series.Name!] = true; };
            item.Unchecked += (_, _) => { series.IsVisible = false; visibility[series.Name!] = false; };
            item.ToolTip = string.Join(", ", data.Series[i].Values.Select((v, n) => $"{(n < data.Categories.Count ? data.Categories[n] : data.Series[i].Name)}: {(v.HasValue ? v.Value.ToString("N1") : "missing")} {data.Unit}"));
            legend.Children.Add(item);
        }
    }
}
