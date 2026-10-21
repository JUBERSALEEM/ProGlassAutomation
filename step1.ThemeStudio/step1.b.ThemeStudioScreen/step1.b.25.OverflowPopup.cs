using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ProGlassAutomation.Analytics;

namespace ProGlassAutomation.ThemeStudio;

/// <summary>Raises a full-window spotlight while an overflow flyout is open.</summary>
public static class OverflowSpotlight
{
    public static event Action<bool>? Changed;
    public static void Set(bool on) => Changed?.Invoke(on);
}

/// <summary>
/// Compact mosaic host. When the child needs more height than the slot,
/// hovering opens a connected popup with a full, scrollable copy.
/// </summary>
public sealed class OverflowPopup : ContentControl
{
    private static OverflowPopup? active;
    private readonly Popup popup = new();
    private readonly ScrollViewer scroller = new();
    private readonly ContentControl frame = new();
    private readonly DispatcherTimer hide = new() { Interval = TimeSpan.FromMilliseconds(250) };
    private bool overflowing;

    public OverflowPopup()
    {
        Focusable = false;
        ClipToBounds = true;
        scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        frame.Content = scroller;
        popup.AllowsTransparency = true;
        popup.StaysOpen = true;
        popup.Placement = PlacementMode.Bottom;
        popup.Child = frame;
        hide.Tick += (_, _) => CloseFlyout();
        MouseEnter += (_, _) => ShowFlyout();
        MouseLeave += (_, _) => { if (!popup.IsOpen) hide.Start(); };
        popup.MouseEnter += (_, _) => hide.Stop();
        popup.MouseLeave += (_, _) => hide.Start();
        OverflowSpotlight.Changed += OnSpotlight;
        Loaded += (_, _) =>
        {
            if (TryFindResource("Card") is Style card) frame.Style = card;
            popup.PlacementTarget = this;
        };
        Unloaded += (_, _) =>
        {
            OverflowSpotlight.Changed -= OnSpotlight;
            CloseFlyout();
        };
    }

    protected override Size MeasureOverride(Size available)
    {
        if (popup.IsOpen) return base.MeasureOverride(available);
        if (Content is UIElement child)
        {
            double width = double.IsInfinity(available.Width) ? 240 : Math.Max(0, available.Width);
            child.Measure(new Size(width, double.PositiveInfinity));
            overflowing = !double.IsInfinity(available.Height) && child.DesiredSize.Height > available.Height + 1;
            Cursor = overflowing ? Cursors.Hand : Cursors.Arrow;
        }
        return base.MeasureOverride(available);
    }

    private void OnSpotlight(bool on)
    {
        if (!on && popup.IsOpen) CloseFlyout();
    }

    private void ShowFlyout()
    {
        hide.Stop();
        if (!overflowing || popup.IsOpen || ActualWidth <= 0 || Content is not FrameworkElement source) return;
        if (source.GetType().GetConstructor(Type.EmptyTypes) is null) return;
        if (Activator.CreateInstance(source.GetType()) is not FrameworkElement copy) return;
        copy.DataContext = source.DataContext;
        if (source is BentoChart chart && copy is BentoChart clone)
        {
            clone.Data = chart.Data;
            clone.PlotHeight = chart.PlotHeight;
        }
        if (active is not null && !ReferenceEquals(active, this)) active.CloseFlyout();
        scroller.Content = copy;
        scroller.Width = Math.Max(ActualWidth, 280);
        scroller.MaxHeight = TryFindResource("PopupMaxHeight") is double cap && cap > 0 ? Math.Max(cap, 360) : 360;
        popup.PlacementTarget = this;
        popup.IsOpen = true;
        active = this;
        OverflowSpotlight.Set(true);
    }

    private void CloseFlyout()
    {
        hide.Stop();
        if (!popup.IsOpen) return;
        popup.IsOpen = false;
        scroller.Content = null;
        if (!ReferenceEquals(active, this)) return;
        active = null;
        OverflowSpotlight.Set(false);
    }
}
