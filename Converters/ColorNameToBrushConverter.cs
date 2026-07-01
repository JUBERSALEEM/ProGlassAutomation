using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.Converters;

/// <summary>
/// Converts a color name string (e.g. "Bronze") to a WPF Brush.
/// Used by SheetStore, but lives in root because it can be reused.
/// </summary>
public sealed class ColorNameToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorName)
            return Brushes.Gray;

        return ColorHelper.GetBrush(colorName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}