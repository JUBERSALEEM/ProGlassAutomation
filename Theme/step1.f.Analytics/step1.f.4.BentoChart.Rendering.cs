using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ProGlassAutomation.Analytics;

public sealed partial class BentoChart
{
    private ISeries[] BuildSeries(ChartDataset data) => data.Series.Select(source =>
    {
        double?[] values = source.Values.ToArray();
        ISeries series = data.Kind switch
        {
            ChartKind.HorizontalBars => new RowSeries<double?> { Values = values },
            ChartKind.StackedColumns => new StackedColumnSeries<double?> { Values = values },
            ChartKind.Lines or ChartKind.Area or ChartKind.Sparkline => new LineSeries<double?> { Values = values, LineSmoothness = 0 },
            ChartKind.Donut or ChartKind.Pie => new PieSeries<double?> { Values = values },
            _ => new ColumnSeries<double?> { Values = values }
        };
        series.Name = source.Name;
        return series;
    }).ToArray();
    private static SKColor ColorOf(Brush brush)
    {
        if (brush is not SolidColorBrush solid) throw new InvalidOperationException("Chart paint roles require SolidColorBrush values.");
        var c = solid.Color;
        return new SKColor(c.R, c.G, c.B, c.A);
    }
    private static SolidColorPaint Paint(Brush brush, double thickness = 1) => new(ColorOf(brush)) { StrokeThickness = (float)thickness };
    private SolidColorPaint TextPaint(Brush brush) => new(ColorOf(brush)) { SKTypeface = SKTypeface.FromFamilyName(FontFamily.Source) };
    private void ApplyPaints(ChartDataset data)
    {
        Brush[] palette = [Series1, Series2, Series3, Series4];
        for (int i = 0; i < rendered.Length; i++)
        {
            switch (rendered[i])
            {
                case ColumnSeries<double?> s: s.Fill = Paint(palette[i]); s.Stroke = null; break;
                case StackedColumnSeries<double?> s: s.Fill = Paint(palette[i]); s.Stroke = null; break;
                case RowSeries<double?> s: s.Fill = Paint(palette[i]); s.Stroke = null; break;
                case LineSeries<double?> s:
                    s.Stroke = Paint(palette[i], LineWidth);
                    s.Fill = data.Kind == ChartKind.Area ? new SolidColorPaint(ColorOf(palette[i]).WithAlpha((byte)(Math.Clamp(AreaOpacity, 0, 1) * 255))) : null;
                    s.GeometrySize = data.Kind == ChartKind.Sparkline ? 0 : PointSize;
                    s.GeometryFill = Paint(palette[i]); s.GeometryStroke = Paint(palette[i]);
                    break;
                case PieSeries<double?> s:
                    s.Fill = Paint(palette[i]); s.Stroke = Paint(GridBrush, LineWidth);
                    s.InnerRadius = data.Kind == ChartKind.Donut ? DonutRadius : 0;
                    break;
            }
        }
        cartesian.TooltipBackgroundPaint = Paint(TooltipSurface); cartesian.TooltipTextPaint = TextPaint(TooltipText);
        pie.TooltipBackgroundPaint = Paint(TooltipSurface); pie.TooltipTextPaint = TextPaint(TooltipText);
        cartesian.TooltipTextSize = FontSize; pie.TooltipTextSize = FontSize;
        center.Foreground = AxisBrush; message.Foreground = AxisBrush; summary.Foreground = AxisBrush;
        bool horizontal = data.Kind == ChartKind.HorizontalBars;
        bool spark = data.Kind == ChartKind.Sparkline;
        // Clamp numeric inputs — never crash on extreme values; validation already ensures finite
        var category = new Axis
        {
            Labels = data.Categories.ToArray(), LabelsPaint = spark ? null : TextPaint(AxisBrush), SeparatorsPaint = null, TextSize = FontSize, MinStep = 1,
            TicksPaint = null, SubseparatorsPaint = null, SubticksPaint = null, ZeroPaint = null,
            Padding = new LiveChartsCore.Drawing.Padding(4, 0, 4, 8)
        };
        var numeric = new Axis
        {
            Name = data.Unit, NameTextSize = FontSize, TicksPaint = null, SubseparatorsPaint = null, SubticksPaint = null, ZeroPaint = null,
            NamePaint = spark ? null : TextPaint(AxisBrush), LabelsPaint = spark ? null : TextPaint(AxisBrush),
            SeparatorsPaint = spark ? null : Paint(GridBrush), TextSize = FontSize,
            MinLimit = !spark && data.Series.SelectMany(s => s.Values).Where(v => v.HasValue).All(v => v >= 0) ? 0 : null,
            Padding = new LiveChartsCore.Drawing.Padding(8, 8, 4, 8),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 12)
        };
        if (!ShowGrid) { numeric.SeparatorsPaint = null; category.SeparatorsPaint = null; }
        if (!ShowAxis) { numeric.LabelsPaint = null; numeric.NamePaint = null; category.LabelsPaint = null; }
        // BarCorner maps to corner rounding where supported — LiveCharts column geometry exposes Rx/Ry; we keep validated range 0-12 via DP without business impact
        foreach (var ser in rendered) { if (ser is LiveChartsCore.SkiaSharpView.ColumnSeries<double?> cs) { cs.Rx = (float)Math.Clamp(BarCorner, 0, 12); cs.Ry = (float)Math.Clamp(BarCorner, 0, 12); } }
        // Tooltip visibility handled via chart tooltip paint nullability (LiveCharts hides when null)
        cartesian.XAxes = [horizontal ? numeric : category]; cartesian.YAxes = [horizontal ? category : numeric];
    }
}
