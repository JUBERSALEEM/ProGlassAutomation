using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProGlassAutomation.ThemeLiquidGlass;

public enum LiquidRank { Chip, Control, Card }

/// <summary>
/// Liquid Glass material: SDF lens refraction, chromatic rim, specular sheen, card lift.
/// Ports Aghajari / dragosniamtu optics onto an in-app watery field.
/// Not FluentSurface acrylic, not ClaySurface, not CSS backdrop-filter,
/// not a desktop-capture shader and not an extra NuGet.
/// </summary>
public sealed class LiquidGlassSurface : ContentControl
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(new CornerRadius(18), OnVisualChanged));
    public static readonly DependencyProperty ConcaveProperty = DependencyProperty.Register(
        nameof(Concave), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty LiftedProperty = DependencyProperty.Register(
        nameof(Lifted), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty RankProperty = DependencyProperty.Register(
        nameof(Rank), typeof(LiquidRank), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(LiquidRank.Card, OnVisualChanged));
    public static readonly DependencyProperty ShowFaceProperty = DependencyProperty.Register(
        nameof(ShowFace), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(true, OnVisualChanged));
    public static readonly DependencyProperty ReduceTransparencyProperty = DependencyProperty.Register(
        nameof(ReduceTransparency), typeof(bool), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(false, OnVisualChanged));
    public static readonly DependencyProperty FaceBrushProperty = DependencyProperty.Register(
        nameof(FaceBrush), typeof(Brush), typeof(LiquidGlassSurface),
        new FrameworkPropertyMetadata(null, OnVisualChanged));
    public static readonly DependencyProperty SpecularBrushProperty = DependencyProperty.Register(
        nameof(SpecularBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty RimBrushProperty = DependencyProperty.Register(
        nameof(RimBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty StrokeBrushProperty = DependencyProperty.Register(
        nameof(StrokeBrush), typeof(Brush), typeof(LiquidGlassSurface));
    public static readonly DependencyProperty RefractionBrushProperty = DependencyProperty.Register(
        nameof(RefractionBrush), typeof(Brush), typeof(LiquidGlassSurface));

    private WriteableBitmap? bitmap;
    private ImageBrush? imageBrush;
    private bool queued;
    private Size bitmapSize;

    public LiquidGlassSurface()
    {
        Focusable = false;
        UseLayoutRounding = true;
        SnapsToDevicePixels = true;
        ClipToBounds = false;
        Loaded += (_, _) => QueueRebuild();
        Unloaded += (_, _) =>
        {
            queued = false;
            bitmap = null;
            imageBrush = null;
            RefractionBrush = null;
        };
        SizeChanged += (_, _) => QueueRebuild();
        IsVisibleChanged += (_, e) => { if (e.NewValue is true) QueueRebuild(); };
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
    public Brush? RefractionBrush { get => (Brush?)GetValue(RefractionBrushProperty); set => SetValue(RefractionBrushProperty, value); }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiquidGlassSurface surface) surface.QueueRebuild();
    }

    private void QueueRebuild()
    {
        if (queued) return;
        queued = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(Rebuild));
    }

    private void Rebuild()
    {
        queued = false;
        if (!IsLoaded || ReduceTransparency || !ShowFace || Rank == LiquidRank.Chip)
        {
            bitmap = null;
            imageBrush = null;
            RefractionBrush = null;
            return;
        }

        double width = ActualWidth;
        double height = ActualHeight;
        if (width < 24 || height < 16) { RefractionBrush = null; return; }

        double scale = Rank == LiquidRank.Card ? 0.5 : 0.42;
        int pixelW = Math.Max(8, (int)Math.Round(width * scale));
        int pixelH = Math.Max(8, (int)Math.Round(height * scale));
        if (bitmap is null || bitmapSize.Width != pixelW || bitmapSize.Height != pixelH)
        {
            bitmap = new WriteableBitmap(pixelW, pixelH, 96, 96, PixelFormats.Pbgra32, null);
            bitmapSize = new Size(pixelW, pixelH);
            imageBrush = new ImageBrush(bitmap) { Stretch = Stretch.Fill, AlignmentX = AlignmentX.Center, AlignmentY = AlignmentY.Center };
            RefractionBrush = imageBrush;
        }

        double worldX = 0, worldY = 0;
        try
        {
            if (PresentationSource.FromVisual(this) is not null)
            {
                Point origin = PointToScreen(new Point(0, 0));
                worldX = origin.X;
                worldY = origin.Y;
            }
        }
        catch (InvalidOperationException) { }

        Color canvas = TryFindResource("ApplicationBackground") is SolidColorBrush canvasBrush
            ? canvasBrush.Color
            : Color.FromRgb(0x62, 0xB4, 0xE8);
        ReadTint(out Color tint, out double tintStrength);
        var request = new LiquidRefraction.Request(
            width,
            height,
            CornerRadius.TopLeft,
            ReadDouble("LiquidFrostOpacity", 0.40),
            ReadDouble("LiquidRefractionStrength", 1.0),
            ReadDouble("LiquidLensDisplacement", 40),
            ReadDouble("LiquidRefractionWidth", 0.30),
            ReadDouble("LiquidChromaticAberration", 1.0),
            ReadDouble("LiquidEdgeHighlight", 0.22),
            worldX,
            worldY,
            canvas,
            tint,
            tintStrength);
        if (bitmap is null || imageBrush is null) return;
        LiquidRefraction.Fill(bitmap, request);
        if (!ReferenceEquals(RefractionBrush, imageBrush)) RefractionBrush = imageBrush;
    }

    private double ReadDouble(string key, double fallback) => TryFindResource(key) switch
    {
        double value => value,
        float value => value,
        _ => fallback
    };

    private void ReadTint(out Color tint, out double tintStrength)
    {
        tint = Colors.White;
        tintStrength = 0.08;
        if (FaceBrush is SolidColorBrush solid && solid.Color.A > 24)
        {
            tint = solid.Color;
            tintStrength = Math.Clamp(solid.Color.A / 255.0 * 0.35, 0, 0.45);
            return;
        }
        if (FaceBrush is GradientBrush gradient && gradient.GradientStops.Count > 0)
        {
            tint = gradient.GradientStops[0].Color;
            tintStrength = Math.Clamp(tint.A / 255.0 * 0.20, 0, 0.22);
        }
    }
}
