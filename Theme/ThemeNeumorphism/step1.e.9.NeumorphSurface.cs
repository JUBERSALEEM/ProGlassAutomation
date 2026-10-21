using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeNeumorphism;

/// <summary>
/// Raised or inset volume from light/dark offset plates. Neumorph-owned.
/// Not Fluent acrylic, not Clay plump extras, not a CSS inset shadow.
/// </summary>
public sealed class NeumorphSurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(NeumorphSurface),
        new FrameworkPropertyMetadata(new CornerRadius(16)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(NeumorphSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty LightBrushProperty = DependencyProperty.Register(
        nameof(LightBrush), typeof(Brush), typeof(NeumorphSurface));
    public static readonly DependencyProperty DarkBrushProperty = DependencyProperty.Register(
        nameof(DarkBrush), typeof(Brush), typeof(NeumorphSurface));

    public NeumorphSurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public Brush LightBrush { get => (Brush)GetValue(LightBrushProperty); set => SetValue(LightBrushProperty, value); }
    public Brush DarkBrush { get => (Brush)GetValue(DarkBrushProperty); set => SetValue(DarkBrushProperty, value); }
}
