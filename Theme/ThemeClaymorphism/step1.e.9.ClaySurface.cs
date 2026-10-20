using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeClaymorphism;

/// <summary>
/// Native clay volume: hue-matched drop, inner shade, top-left sheen.
/// Clay-owned. Not a Bento type and not a CSS shadow.
/// </summary>
public sealed class ClaySurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(ClaySurface),
        new FrameworkPropertyMetadata(new CornerRadius(32)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(ClaySurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty DropBrushProperty = DependencyProperty.Register(
        nameof(DropBrush), typeof(Brush), typeof(ClaySurface));
    public static readonly DependencyProperty ShadeBrushProperty = DependencyProperty.Register(
        nameof(ShadeBrush), typeof(Brush), typeof(ClaySurface));
    public static readonly DependencyProperty LightBrushProperty = DependencyProperty.Register(
        nameof(LightBrush), typeof(Brush), typeof(ClaySurface));

    public ClaySurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public Brush DropBrush { get => (Brush)GetValue(DropBrushProperty); set => SetValue(DropBrushProperty, value); }
    public Brush ShadeBrush { get => (Brush)GetValue(ShadeBrushProperty); set => SetValue(ShadeBrushProperty, value); }
    public Brush LightBrush { get => (Brush)GetValue(LightBrushProperty); set => SetValue(LightBrushProperty, value); }
}
