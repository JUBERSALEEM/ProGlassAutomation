using System.Windows;
using System.Windows.Controls;
namespace ProGlassAutomation.ThemeStudio;
public partial class ChartGallery : UserControl
{
    private bool? compact;
    public ChartGallery() => InitializeComponent();
    private void GalleryGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        bool next = e.NewSize.Width < 1000;
        if (next == compact) return;
        compact = next;
        GalleryGrid.ColumnDefinitions[1].Width = next ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
        Grid.SetColumn(StackCard, next ? 0 : 1); Grid.SetRow(StackCard, next ? 1 : 0);
        Grid.SetRow(ChannelCard, next ? 2 : 1);
        Grid.SetColumn(MixCard, next ? 0 : 1); Grid.SetRow(MixCard, next ? 3 : 1);
        while (GalleryGrid.RowDefinitions.Count < 4) GalleryGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    }
}
