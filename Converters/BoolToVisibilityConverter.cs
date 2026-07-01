using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProGlassAutomation.Converters;

/// <summary>
/// Converts a boolean to Visibility. Parameter "invert" reverses the logic.
/// Usage: {Binding IsLoading, Converter={StaticResource BoolToVisibilityConverter}}
/// Usage: {Binding IsHidden, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=invert}
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var flag = value is bool b && b;
        if (parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase))
            flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility vis)
        {
            var flag = vis == Visibility.Visible;
            if (parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase))
                flag = !flag;
            return flag;
        }
        return DependencyProperty.UnsetValue;
    }
}