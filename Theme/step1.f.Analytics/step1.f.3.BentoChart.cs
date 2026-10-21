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
    private readonly WrapPanel legend = new();
    private readonly TextBlock message = new() { TextWrapping = TextWrapping.Wrap };
    private readonly TextBlock center = new() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
    private readonly CheckBox inspectToggle = new() { Content = "Show source values" };
    private readonly TextBlock values = new() { TextWrapping = TextWrapping.Wrap, Visibility = Visibility.Collapsed };
    private readonly TextBlock summary = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Dictionary<string, bool> visibility = new();
    private ISeries[] rendered = [];
    private bool dataDirty = true;
    private bool queued;
    public IReadOnlyList<ISeries> RenderedSeries => rendered;
    public string DisplayMessage => message.Text;
    static BentoChart()
    {
        // Skia GPU surfaces clear to black and BitmapCache cannot keep a transparent plot.
        // CPU + vsync-off is the path that preserves the card behind the donut hole.
        LiveCharts.RenderingSettings.UseGPU = false;
        LiveCharts.RenderingSettings.TryUseVSync = false;
    }
    public BentoChart()
    {
        var root = new StackPanel();
        // Ensure no white surfaces — chart must be transparent and reveal the dark card behind, including the donut hole.
        cartesian.Background = System.Windows.Media.Brushes.Transparent;
        pie.Background = System.Windows.Media.Brushes.Transparent;
        plot.Background = System.Windows.Media.Brushes.Transparent;
        Background = System.Windows.Media.Brushes.Transparent;
        BorderBrush = System.Windows.Media.Brushes.Transparent;
        plot.Children.Add(cartesian); plot.Children.Add(pie); plot.Children.Add(center);
        root.Children.Add(plot); root.Children.Add(legend); root.Children.Add(message); root.Children.Add(summary); root.Children.Add(inspectToggle); root.Children.Add(values);
        inspectToggle.Checked += (_, _) => values.Visibility = Visibility.Visible;
        inspectToggle.Unchecked += (_, _) => values.Visibility = Visibility.Collapsed;
        plot.SetBinding(HeightProperty, new Binding(nameof(PlotHeight)) { Source = this });
        Content = root;
        center.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(CenterFontSize)) { Source = this });
        Loaded += (_, _) => Schedule(true);
        IsVisibleChanged += (_, _) => { if (IsVisible) Schedule(false); };
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == FontSizeProperty || e.Property == FontFamilyProperty) Schedule(false);
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
        cartesian.Visibility = ready && !circular ? Visibility.Visible : Visibility.Collapsed;
        pie.Visibility = ready && circular ? Visibility.Visible : Visibility.Collapsed;
        center.Visibility = ready && Data?.Kind == ChartKind.Donut ? Visibility.Visible : Visibility.Collapsed;
        center.Text = Data?.CenterText ?? "";
        if (!ready)
        {
            cartesian.Series = []; pie.Series = []; rendered = []; legend.Children.Clear();
            dataDirty = true; inspectToggle.Visibility = Visibility.Collapsed; values.Text = "";
            return;
        }
        inspectToggle.Visibility = ready ? Visibility.Visible : Visibility.Collapsed;
        if (dataDirty)
        {
            values.Text = string.Join("\n", Data!.Series.Select(s => s.Name + ": " + string.Join(" / ", s.Values.Select((v, i) => $"{(i < Data.Categories.Count ? Data.Categories[i] : s.Name)} {(v.HasValue ? v.Value.ToString("N1") : "missing")} {Data.Unit}"))));
            rendered = BuildSeries(Data!);
            cartesian.Series = circular ? [] : rendered;
            pie.Series = circular ? rendered : [];
            BuildLegend(Data!);
            dataDirty = false;
        }
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
            var label = new StackPanel { Orientation = Orientation.Horizontal };
            label.Children.Add(swatch); label.Children.Add(new TextBlock { Text = "  " + data.Series[i].Name });
            var item = new CheckBox { Content = label, IsChecked = visibility.GetValueOrDefault(data.Series[i].Name, true) };
            series.IsVisible = item.IsChecked == true;
            AutomationProperties.SetName(item, $"Show series {data.Series[i].Name}");
            item.Checked += (_, _) => { series.IsVisible = true; visibility[series.Name!] = true; };
            item.Unchecked += (_, _) => { series.IsVisible = false; visibility[series.Name!] = false; };
            // Source values are available to keyboard users as well as pointer tooltips.
            item.ToolTip = string.Join(", ", data.Series[i].Values.Select((v, n) => $"{(n < data.Categories.Count ? data.Categories[n] : data.Series[i].Name)}: {(v.HasValue ? v.Value.ToString("N1") : "missing")} {data.Unit}"));
            legend.Children.Add(item);
        }
    }
}
