using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeClaymorphism;

public enum ClayRank { Chip, Control, Card }

/// <summary>
/// Inflated clay: a hue-matched outer drop plus a rounded-geometry rim
/// (highlight wraps the top-left arc, shade wraps the bottom-right).
/// Inner ranks keep gutter 0 so nested plates are not a card inside a card.
/// Clay-owned. Not a Bento type, not Neumorph gray plates, not CSS.
/// </summary>
public sealed class ClaySurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(ClaySurface),
        new FrameworkPropertyMetadata(new CornerRadius(32)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(ClaySurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty LiftedProperty = DependencyProperty.Register(
        nameof(Lifted), typeof(bool), typeof(ClaySurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty RankProperty = DependencyProperty.Register(
        nameof(Rank), typeof(ClayRank), typeof(ClaySurface),
        new FrameworkPropertyMetadata(ClayRank.Card));
    public static readonly DependencyProperty DropBrushProperty = DependencyProperty.Register(
        nameof(DropBrush), typeof(Brush), typeof(ClaySurface));
    public static readonly DependencyProperty ShadeBrushProperty = DependencyProperty.Register(
        nameof(ShadeBrush), typeof(Brush), typeof(ClaySurface));
    public static readonly DependencyProperty LightBrushProperty = DependencyProperty.Register(
        nameof(LightBrush), typeof(Brush), typeof(ClaySurface));
    public static readonly DependencyProperty ShowDropProperty = DependencyProperty.Register(
        nameof(ShowDrop), typeof(bool), typeof(ClaySurface),
        new FrameworkPropertyMetadata(true));

    public ClaySurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
        ClipToBounds = false;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public bool Lifted { get => (bool)GetValue(LiftedProperty); set => SetValue(LiftedProperty, value); }
    public ClayRank Rank { get => (ClayRank)GetValue(RankProperty); set => SetValue(RankProperty, value); }
    public bool ShowDrop { get => (bool)GetValue(ShowDropProperty); set => SetValue(ShowDropProperty, value); }
    public Brush DropBrush { get => (Brush)GetValue(DropBrushProperty); set => SetValue(DropBrushProperty, value); }
    public Brush ShadeBrush { get => (Brush)GetValue(ShadeBrushProperty); set => SetValue(ShadeBrushProperty, value); }
    public Brush LightBrush { get => (Brush)GetValue(LightBrushProperty); set => SetValue(LightBrushProperty, value); }
}

/// <summary>
/// Inset clay rim drawn as a rounded-rect ring (outer minus inner geometry).
/// Follows the corner arcs; not a four-sided BorderThickness fake.
/// </summary>
public sealed class ClayCurveRim : FrameworkElement
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(ClayCurveRim),
        new FrameworkPropertyMetadata(new CornerRadius(32), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty LightBrushProperty = DependencyProperty.Register(
        nameof(LightBrush), typeof(Brush), typeof(ClayCurveRim),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ShadeBrushProperty = DependencyProperty.Register(
        nameof(ShadeBrush), typeof(Brush), typeof(ClayCurveRim),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(ClayCurveRim),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RimWidthProperty = DependencyProperty.Register(
        nameof(RimWidth), typeof(double), typeof(ClayCurveRim),
        new FrameworkPropertyMetadata(6d, FrameworkPropertyMetadataOptions.AffectsRender));

    public ClayCurveRim()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SnapsToDevicePixels = true;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public Brush LightBrush { get => (Brush)GetValue(LightBrushProperty); set => SetValue(LightBrushProperty, value); }
    public Brush ShadeBrush { get => (Brush)GetValue(ShadeBrushProperty); set => SetValue(ShadeBrushProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public double RimWidth { get => (double)GetValue(RimWidthProperty); set => SetValue(RimWidthProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        double width = ActualWidth;
        double height = ActualHeight;
        if (width < 4 || height < 4) return;

        double rim = Math.Max(1.5, RimWidth);
        if (rim * 2 >= Math.Min(width, height))
            rim = Math.Min(width, height) / 4d;

        var outerRect = new Rect(0, 0, width, height);
        var innerRect = new Rect(rim, rim, Math.Max(0, width - rim * 2), Math.Max(0, height - rim * 2));
        if (innerRect.Width < 1 || innerRect.Height < 1) return;

        var outer = Rounded(outerRect, CornerRadius);
        var inner = Rounded(innerRect, Shrink(CornerRadius, rim));
        var ring = new CombinedGeometry(GeometryCombineMode.Exclude, outer, inner);
        ring.Freeze();

        Color light = ColorOf(LightBrush, Color.FromArgb(0xF2, 0xFF, 0xFF, 0xFF));
        Color shade = ColorOf(ShadeBrush, Color.FromArgb(0x59, 0x5B, 0x4A, 0x9A));
        var fill = new LinearGradientBrush
        {
            StartPoint = Concave ? new Point(1, 1) : new Point(0, 0),
            EndPoint = Concave ? new Point(0, 0) : new Point(1, 1),
            MappingMode = BrushMappingMode.RelativeToBoundingBox
        };
        fill.GradientStops.Add(new GradientStop(light, 0));
        fill.GradientStops.Add(new GradientStop(Color.FromArgb(0x28, light.R, light.G, light.B), 0.42));
        fill.GradientStops.Add(new GradientStop(shade, 1));
        fill.Freeze();
        dc.DrawGeometry(fill, null, ring);
    }

    private static CornerRadius Shrink(CornerRadius radius, double rim) => new(
        Math.Max(0, radius.TopLeft - rim),
        Math.Max(0, radius.TopRight - rim),
        Math.Max(0, radius.BottomRight - rim),
        Math.Max(0, radius.BottomLeft - rim));

    private static Color ColorOf(Brush? brush, Color fallback) =>
        brush is SolidColorBrush solid ? solid.Color : fallback;

    private static Geometry Rounded(Rect rect, CornerRadius radius)
    {
        double width = rect.Width;
        double height = rect.Height;
        if (width <= 0 || height <= 0) return Geometry.Empty;

        double tl = Clamp(radius.TopLeft, width, height);
        double tr = Clamp(radius.TopRight, width, height);
        double br = Clamp(radius.BottomRight, width, height);
        double bl = Clamp(radius.BottomLeft, width, height);

        var geometry = new StreamGeometry { FillRule = FillRule.Nonzero };
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(rect.Left + tl, rect.Top), true, true);
            ctx.LineTo(new Point(rect.Right - tr, rect.Top), true, false);
            if (tr > 0)
                ctx.ArcTo(new Point(rect.Right, rect.Top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise, true, false);
            else
                ctx.LineTo(new Point(rect.Right, rect.Top), true, false);
            ctx.LineTo(new Point(rect.Right, rect.Bottom - br), true, false);
            if (br > 0)
                ctx.ArcTo(new Point(rect.Right - br, rect.Bottom), new Size(br, br), 0, false, SweepDirection.Clockwise, true, false);
            else
                ctx.LineTo(new Point(rect.Right, rect.Bottom), true, false);
            ctx.LineTo(new Point(rect.Left + bl, rect.Bottom), true, false);
            if (bl > 0)
                ctx.ArcTo(new Point(rect.Left, rect.Bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise, true, false);
            else
                ctx.LineTo(new Point(rect.Left, rect.Bottom), true, false);
            ctx.LineTo(new Point(rect.Left, rect.Top + tl), true, false);
            if (tl > 0)
                ctx.ArcTo(new Point(rect.Left + tl, rect.Top), new Size(tl, tl), 0, false, SweepDirection.Clockwise, true, false);
        }
        geometry.Freeze();
        return geometry;
    }

    private static double Clamp(double radius, double width, double height) =>
        Math.Max(0, Math.Min(radius, Math.Min(width, height) / 2d));
}
