using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeGlassmorphism;

/// <summary>
/// Native glassmorphism material: solid inner fill, frost overlay, luminance hairline.
/// Theme-owned. Not LiquidGlassSurface, not FluentSurface, no BlurEffect, no CSS backdrop-filter.
/// </summary>
public sealed class GlassSurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(GlassSurface),
        new FrameworkPropertyMetadata(new CornerRadius(20)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(GlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty TintBrushProperty = DependencyProperty.Register(
        nameof(TintBrush), typeof(Brush), typeof(GlassSurface));
    public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register(
        nameof(StrokeBrush), typeof(Brush), typeof(GlassSurface));
    public static readonly DependencyProperty LuminosityBrushProperty = DependencyProperty.Register(
        nameof(LuminosityBrush), typeof(Brush), typeof(GlassSurface));
    public static readonly DependencyProperty ShowTintProperty = DependencyProperty.Register(
        nameof(ShowTint), typeof(bool), typeof(GlassSurface),
        new FrameworkPropertyMetadata(true));
    public static readonly DependencyProperty ReduceTransparencyProperty = DependencyProperty.Register(
        nameof(ReduceTransparency), typeof(bool), typeof(GlassSurface),
        new FrameworkPropertyMetadata(false));

    public GlassSurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
    }

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public bool ShowTint { get => (bool)GetValue(ShowTintProperty); set => SetValue(ShowTintProperty, value); }
    public bool ReduceTransparency { get => (bool)GetValue(ReduceTransparencyProperty); set => SetValue(ReduceTransparencyProperty, value); }
    public Brush TintBrush { get => (Brush)GetValue(TintBrushProperty); set => SetValue(TintBrushProperty, value); }
    public Brush StrokeBrush { get => (Brush)GetValue(StrokeBrushProperty); set => SetValue(StrokeBrushProperty, value); }
    public Brush LuminosityBrush { get => (Brush)GetValue(LuminosityBrushProperty); set => SetValue(LuminosityBrushProperty, value); }
}
