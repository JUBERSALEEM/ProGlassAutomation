using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Windows.Media;

namespace ProGlassAutomation.ThemeNeumorphism;

/// <summary>
/// Second shadow for StatusBadge. Border allows one Effect; the light plate is a sibling.
/// Contract stays Border. Not a NeumorphSurface.
/// </summary>
public static class NeumorphBadgeGlow
{
    public static readonly DependencyProperty LightGlowProperty = DependencyProperty.RegisterAttached(
        "LightGlow",
        typeof(bool),
        typeof(NeumorphBadgeGlow),
        new PropertyMetadata(false, OnLightGlowChanged));

    public static void SetLightGlow(DependencyObject element, bool value) => element.SetValue(LightGlowProperty, value);
    public static bool GetLightGlow(DependencyObject element) => (bool)element.GetValue(LightGlowProperty);

    private static readonly DependencyProperty LightPlateProperty = DependencyProperty.RegisterAttached(
        "LightPlate",
        typeof(Border),
        typeof(NeumorphBadgeGlow));

    private static void OnLightGlowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Border badge) return;
        if (e.NewValue is true)
            badge.Loaded += OnLoaded;
        else
            badge.Loaded -= OnLoaded;
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Border badge) return;
        badge.Loaded -= OnLoaded;
        if (badge.Parent is not Panel panel) return;
        if (badge.GetValue(LightPlateProperty) is Border) return;

        Border light = new()
        {
            IsHitTestVisible = false,
            SnapsToDevicePixels = true,
            UseLayoutRounding = true,
        };
        light.SetBinding(Border.BackgroundProperty, new Binding(nameof(Border.Background)) { Source = badge });
        light.SetBinding(Border.CornerRadiusProperty, new Binding(nameof(Border.CornerRadius)) { Source = badge });
        light.SetBinding(FrameworkElement.WidthProperty, new Binding(nameof(FrameworkElement.ActualWidth)) { Source = badge });
        light.SetBinding(FrameworkElement.HeightProperty, new Binding(nameof(FrameworkElement.ActualHeight)) { Source = badge });
        light.SetBinding(FrameworkElement.MarginProperty, new Binding(nameof(FrameworkElement.Margin)) { Source = badge });
        light.HorizontalAlignment = badge.HorizontalAlignment;
        light.VerticalAlignment = badge.VerticalAlignment;
        Grid.SetColumn(light, Grid.GetColumn(badge));
        Grid.SetRow(light, Grid.GetRow(badge));
        Grid.SetColumnSpan(light, Grid.GetColumnSpan(badge));
        Grid.SetRowSpan(light, Grid.GetRowSpan(badge));
        if (panel is DockPanel)
            DockPanel.SetDock(light, DockPanel.GetDock(badge));

        DropShadowEffect effect = new()
        {
            BlurRadius = 12,
            ShadowDepth = 6,
            Direction = 125,
            RenderingBias = RenderingBias.Quality,
        };
        if (badge.TryFindResource("NeumorphLightColor") is Color color)
            effect.Color = color;
        if (badge.TryFindResource("NeumorphLightOpacity") is double opacity)
            effect.Opacity = opacity;
        light.Effect = effect;

        int index = panel.Children.IndexOf(badge);
        if (index < 0) return;
        panel.Children.Insert(index, light);
        badge.SetValue(LightPlateProperty, light);
    }
}
