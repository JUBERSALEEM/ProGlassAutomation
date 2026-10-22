using System.Windows;
using System.Windows.Media;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeSpatial;

/// <summary>
/// Far-plane depth field. Receding bands, not Glass blooms, not Liquid sky, not PNG.
/// </summary>
public static class SpatialWallpaper
{
    private const double Width = 800;
    private const double Height = 500;

    public static void Install(ResourceDictionary package, Appearance appearance)
    {
        ArgumentNullException.ThrowIfNull(package);
        if (!Enum.IsDefined(appearance)) throw new ArgumentOutOfRangeException(nameof(appearance));
        if (!Replace(package, "SpatialCanvasBrush", Create(appearance)))
            throw new InvalidOperationException("SpatialCanvasBrush must exist in Spatial extras.");
    }

    public static DrawingBrush Create(Appearance appearance)
    {
        var group = new DrawingGroup();
        if (appearance == Appearance.Dark) PaintDark(group);
        else PaintLight(group);
        group.Freeze();
        return new DrawingBrush(group)
        {
            Stretch = Stretch.UniformToFill,
            AlignmentX = AlignmentX.Center,
            AlignmentY = AlignmentY.Center,
            TileMode = TileMode.None
        };
    }

    private static void PaintLight(DrawingGroup group)
    {
        Fill(group, Wash(Color.FromRgb(210, 220, 232), Color.FromRgb(184, 196, 212), Color.FromRgb(156, 172, 196), Color.FromRgb(140, 160, 188)));
        Band(group, 40, 320, 720, 140, Color.FromArgb(70, 255, 255, 255), Color.FromArgb(0, 184, 196, 212));
        Band(group, 120, 380, 560, 90, Color.FromArgb(90, 255, 255, 255), Color.FromArgb(0, 156, 172, 196));
    }

    private static void PaintDark(DrawingGroup group)
    {
        Fill(group, Wash(Color.FromRgb(8, 12, 20), Color.FromRgb(12, 20, 36), Color.FromRgb(18, 28, 52), Color.FromRgb(6, 8, 16)));
        Band(group, 40, 300, 720, 150, Color.FromArgb(50, 80, 140, 200), Color.FromArgb(0, 12, 20, 36));
        Band(group, 140, 370, 520, 90, Color.FromArgb(70, 60, 90, 180), Color.FromArgb(0, 18, 28, 52));
    }

    private static LinearGradientBrush Wash(Color top, Color high, Color mid, Color deep)
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
        brush.GradientStops.Add(new GradientStop(top, 0));
        brush.GradientStops.Add(new GradientStop(high, 0.35));
        brush.GradientStops.Add(new GradientStop(mid, 0.7));
        brush.GradientStops.Add(new GradientStop(deep, 1));
        brush.Freeze();
        return brush;
    }

    private static void Fill(DrawingGroup group, Brush brush)
    {
        group.Children.Add(new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0, 0, Width, Height))));
    }

    private static void Band(DrawingGroup group, double x, double y, double w, double h, Color inner, Color outer)
    {
        if (x < 0 || y < 0 || x + w > Width || y + h > Height)
            throw new InvalidOperationException("Spatial depth band must sit fully inside the drawing.");
        var center = new Point(x + w / 2, y + h / 2);
        var radial = new RadialGradientBrush
        {
            MappingMode = BrushMappingMode.Absolute,
            Center = center,
            GradientOrigin = center,
            RadiusX = w / 2,
            RadiusY = h / 2
        };
        radial.GradientStops.Add(new GradientStop(inner, 0));
        radial.GradientStops.Add(new GradientStop(outer, 1));
        radial.Freeze();
        group.Children.Add(new GeometryDrawing(radial, null, new RectangleGeometry(new Rect(x, y, w, h), 24, 24)));
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
