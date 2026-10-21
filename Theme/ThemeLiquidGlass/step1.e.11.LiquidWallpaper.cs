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
            Color.FromRgb(232, 242, 250),
            Color.FromRgb(181, 212, 236),
            Color.FromRgb(111, 163, 208),
            Color.FromRgb(58, 111, 176),
            Color.FromRgb(30, 74, 136)));
        Bloom(group, 620, 80, 420, 300, Color.FromArgb(210, 255, 248, 230), Color.FromArgb(0, 181, 212, 236));
        Bloom(group, 500, 270, 460, 110, Color.FromArgb(140, 240, 216, 168), Color.FromArgb(0, 111, 163, 208));
        Bloom(group, 90, 210, 340, 280, Color.FromArgb(120, 126, 184, 232), Color.FromArgb(0, 58, 111, 176));
        Bloom(group, 400, 320, 520, 80, Color.FromArgb(70, 255, 255, 255), Color.FromArgb(0, 181, 212, 236));
        Bloom(group, 400, 510, 440, 140, Color.FromArgb(200, 22, 58, 112), Color.FromArgb(0, 30, 74, 136));
    }

    private static void PaintNight(DrawingGroup group)
    {
        Fill(group, Sky(
            Color.FromRgb(8, 22, 48),
            Color.FromRgb(12, 36, 72),
            Color.FromRgb(16, 48, 92),
            Color.FromRgb(10, 36, 78),
            Color.FromRgb(4, 14, 32)));
        Bloom(group, 600, 90, 380, 260, Color.FromArgb(160, 186, 214, 236), Color.FromArgb(0, 12, 36, 72));
        Bloom(group, 480, 260, 420, 100, Color.FromArgb(90, 120, 160, 210), Color.FromArgb(0, 16, 48, 92));
        Bloom(group, 80, 200, 300, 260, Color.FromArgb(110, 32, 72, 128), Color.FromArgb(0, 8, 22, 48));
        Bloom(group, 400, 510, 440, 140, Color.FromArgb(220, 2, 10, 28), Color.FromArgb(0, 4, 14, 32));
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
