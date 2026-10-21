using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeLiquidGlass;

public enum LiquidRank { Chip, Control, Card }

/// <summary>
/// Liquid Glass material: translucent face, specular inset, darkened rim, card lift.
/// Liquid-owned. Not FluentSurface acrylic, not ClaySurface, not CSS backdrop-filter,
/// not a desktop-capture shader.
/// </summary>
public sealed class LiquidGlassSurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(new CornerRadius(18)));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty LiftedProperty = DependencyProperty.Register(
        nameof(Lifted), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty RankProperty = DependencyProperty.Register(
        nameof(Rank), typeof(LiquidRank), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(LiquidRank.Card));
    public static readonly DependencyProperty ShowFaceProperty = DependencyProperty.Register(
        nameof(ShowFace), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(true));
    public static readonly DependencyProperty ReduceTransparencyProperty = DependencyProperty.Register(
        nameof(ReduceTransparency), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty FaceBrushProperty = DependencyProperty.Register(
        nameof(FaceBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty SpecularBrushProperty = DependencyProperty.Register(
        nameof(SpecularBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty RimBrushProperty = DependencyProperty.Register(
        nameof(RimBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register(
        nameof(StrokeBrush), typeof(Brush), typeof(LiquidGlassSurface));

    public LiquidGlassSurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
        ClipToBounds = false;
    }

    protected override Geometry? GetLayoutClip(Size layoutSlotSize) => null;

    public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool Concave { get => (bool)GetValue(ConcaveProperty); set => SetValue(ConcaveProperty, value); }
    public bool Lifted { get => (bool)GetValue(LiftedProperty); set => SetValue(LiftedProperty, value); }
    public LiquidRank Rank { get => (LiquidRank)GetValue(RankProperty); set => SetValue(RankProperty, value); }
    public bool ShowFace { get => (bool)GetValue(ShowFaceProperty); set => SetValue(ShowFaceProperty, value); }
    public bool ReduceTransparency { get => (bool)GetValue(ReduceTransparencyProperty); set => SetValue(ReduceTransparencyProperty, value); }
    public Brush FaceBrush { get => (Brush)GetValue(FaceBrushProperty); set => SetValue(FaceBrushProperty, value); }
    public Brush SpecularBrush { get => (Brush)GetValue(SpecularBrushProperty); set => SetValue(SpecularBrushProperty, value); }
    public Brush RimBrush { get => (Brush)GetValue(RimBrushProperty); set => SetValue(RimBrushProperty, value); }
    public Brush StrokeBrush { get => (Brush)GetValue(StrokeBrushProperty); set => SetValue(StrokeBrushProperty, value); }
}
