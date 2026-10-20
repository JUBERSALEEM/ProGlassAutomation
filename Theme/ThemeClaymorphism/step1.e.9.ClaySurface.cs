using System.Windows;
using System.Windows.Controls;

namespace ProGlassAutomation.ThemeClaymorphism;

/// <summary>
/// Native clay volume: colored drop, inner shade, top-left sheen.
/// Clay-owned. Not a Bento type and not a CSS shadow.
/// </summary>
public sealed class ClaySurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(ClaySurface),
        new FrameworkPropertyMetadata(new CornerRadius(24)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(ClaySurface),
        new FrameworkPropertyMetadata(false));

    public ClaySurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
}
