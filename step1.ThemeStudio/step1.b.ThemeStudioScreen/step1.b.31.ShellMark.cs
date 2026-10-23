using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProGlassAutomation.Customization;

namespace ProGlassAutomation.ThemeStudio;

/// <summary>Draws a nav mark from <see cref="IconService"/>. Never hosts Effect or PNG.</summary>
public sealed class ShellMark : Control
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(IconKind), typeof(ShellMark),
        new FrameworkPropertyMetadata(IconKind.Home, FrameworkPropertyMetadataOptions.AffectsRender));

    public IconKind Kind { get => (IconKind)GetValue(KindProperty); set => SetValue(KindProperty, value); }

    public ShellMark()
    {
        Focusable = false;
        IsHitTestVisible = false;
        SnapsToDevicePixels = true;
        UseLayoutRounding = true;
        Template = null;
        Background = Brushes.Transparent;
        Loaded += (_, _) => IconService.Changed += OnIcons;
        Unloaded += (_, _) => IconService.Changed -= OnIcons;
    }

    private void OnIcons(object? sender, EventArgs e) => InvalidateVisual();

    protected override void OnRender(DrawingContext dc)
    {
        if (ActualWidth < 2 || ActualHeight < 2) return;
        var recipe = IconService.Recipe(IconService.Active.Style);
        Brush ink = Foreground ?? Brushes.Black;
        double box = Math.Min(ActualWidth, ActualHeight);
        double size = box * Math.Clamp(recipe.Scale, 0.6, 1.2);
        double origin = (box - size) / 2;
        if (recipe.Plate)
        {
            var plate = ink.CloneCurrentValue();
            plate.Opacity = 0.16;
            if (plate.CanFreeze) plate.Freeze();
            dc.DrawEllipse(plate, null, new Point(ActualWidth / 2, ActualHeight / 2), box / 2, box / 2);
        }
        dc.PushTransform(new TranslateTransform(origin, origin));
        dc.PushTransform(new ScaleTransform(size / 24d, size / 24d));
        var geometry = IconService.Mark(Kind, IconService.Active.Style);
        if (recipe.Filled)
        {
            Brush fill = ink;
            if (recipe.Duotone)
            {
                fill = ink.CloneCurrentValue();
                fill.Opacity = 0.32;
                if (fill.CanFreeze) fill.Freeze();
            }
            dc.DrawGeometry(fill, null, geometry);
        }
        double stroke = recipe.Stroke;
        if (Kind == IconKind.About && stroke <= 0) stroke = 1.3;
        if (stroke > 0)
        {
            var pen = new Pen(ink, stroke)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            };
            pen.Freeze();
            dc.DrawGeometry(null, pen, geometry);
        }
        dc.Pop();
        dc.Pop();
    }
}
