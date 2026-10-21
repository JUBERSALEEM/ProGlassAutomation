using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeLiquidGlass;

/// <summary>
/// One Liquid-owned iOS / macOS Tahoe scene: sky, water, sun/moon bloom.
/// Drawn in code (no PNG, no wallpaper picker, no scene catalog).
/// </summary>
public static class LiquidWallpaper
{
    private const double Width = 800;
    private const double Height = 500;

    public static void Install(ResourceDictionary package, Appearance appearance)
    {
        ArgumentNullException.ThrowIfNull(package);
        if (!Enum.IsDefined(appearance)) throw new ArgumentOutOfRangeException(nameof(appearance));
        if (!Replace(package, "LiquidCanvasBrush", Create(appearance)))
            throw new InvalidOperationException("LiquidCanvasBrush must exist in Liquid extras.");
    }

    public static DrawingBrush Create(Appearance appearance)
    {
        var group = new DrawingGroup();
        if (appearance == Appearance.Dark) PaintNight(group);
        else PaintDay(group);
        return new DrawingBrush(group)
        {
            Stretch = Stretch.UniformToFill,
            AlignmentX = AlignmentX.Center,
            AlignmentY = AlignmentY.Center
        };
    }

    private static void PaintDay(DrawingGroup group)
    {
        Fill(group, Sky(
            Color.FromRgb(244, 248, 252),
            Color.FromRgb(212, 230, 244),
            Color.FromRgb(158, 196, 228),
            Color.FromRgb(90, 146, 200),
            Color.FromRgb(26, 74, 136)));
        Bloom(group, 680, 50, 520, 360, Color.FromArgb(230, 255, 250, 232), Color.FromArgb(0, 212, 230, 244));
        Bloom(group, 540, 240, 520, 140, Color.FromArgb(170, 242, 220, 176), Color.FromArgb(0, 158, 196, 228));
        Bloom(group, 60, 160, 260, 200, Color.FromArgb(70, 186, 216, 236), Color.FromArgb(0, 90, 146, 200));
        Bloom(group, 400, 300, 640, 70, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(0, 212, 230, 244));
        Bloom(group, 400, 530, 560, 180, Color.FromArgb(210, 20, 58, 112), Color.FromArgb(0, 26, 74, 136));
    }

    private static void PaintNight(DrawingGroup group)
    {
        Fill(group, Sky(
            Color.FromRgb(10, 24, 48),
            Color.FromRgb(14, 40, 78),
            Color.FromRgb(18, 52, 96),
            Color.FromRgb(12, 40, 80),
            Color.FromRgb(4, 12, 28)));
        Bloom(group, 640, 70, 440, 300, Color.FromArgb(170, 198, 220, 240), Color.FromArgb(0, 14, 40, 78));
        Bloom(group, 500, 250, 480, 120, Color.FromArgb(100, 140, 176, 214), Color.FromArgb(0, 18, 52, 96));
        Bloom(group, 80, 180, 240, 200, Color.FromArgb(80, 36, 72, 120), Color.FromArgb(0, 10, 24, 48));
        Bloom(group, 400, 530, 560, 180, Color.FromArgb(230, 2, 8, 22), Color.FromArgb(0, 4, 12, 28));
    }

    private static LinearGradientBrush Sky(Color top, Color high, Color mid, Color water, Color deep)
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0.18, 0), EndPoint = new Point(0.82, 1) };
        brush.GradientStops.Add(new GradientStop(top, 0));
        brush.GradientStops.Add(new GradientStop(high, 0.28));
        brush.GradientStops.Add(new GradientStop(mid, 0.52));
        brush.GradientStops.Add(new GradientStop(water, 0.76));
        brush.GradientStops.Add(new GradientStop(deep, 1));
        brush.Freeze();
        return brush;
    }

    private static void Fill(DrawingGroup group, Brush brush)
    {
        group.Children.Add(new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0, 0, Width, Height))));
    }

    private static void Bloom(DrawingGroup group, double x, double y, double radiusX, double radiusY, Color inner, Color outer)
    {
        var center = new Point(x, y);
        var radial = new RadialGradientBrush
        {
            MappingMode = BrushMappingMode.Absolute,
            Center = center,
            GradientOrigin = center,
            RadiusX = radiusX,
            RadiusY = radiusY
        };
        radial.GradientStops.Add(new GradientStop(inner, 0));
        radial.GradientStops.Add(new GradientStop(Color.FromArgb((byte)(inner.A / 3), inner.R, inner.G, inner.B), 0.45));
        radial.GradientStops.Add(new GradientStop(outer, 1));
        radial.Freeze();
        group.Children.Add(new GeometryDrawing(radial, null, new EllipseGeometry(center, radiusX, radiusY)));
    }

    private static bool Replace(ResourceDictionary dictionary, string key, object value)
    {
        foreach (object existing in dictionary.Keys)
        {
            if (!Equals(existing, key)) continue;
            dictionary[key] = value;
            return true;
        }
        foreach (ResourceDictionary child in dictionary.MergedDictionaries)
            if (Replace(child, key, value)) return true;
        return false;
    }
}
