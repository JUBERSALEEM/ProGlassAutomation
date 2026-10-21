using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeClaymorphism;

public enum ClayRank { Chip, Control, Card }

/// <summary>
/// Canvas-melted clay: stadium corners, 3D face curve, hue-matched Gaussian glow.
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
