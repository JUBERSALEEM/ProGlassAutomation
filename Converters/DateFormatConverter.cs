using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProGlassAutomation.Converters;

public sealed class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dt)
            return string.Empty;

        var format = parameter as string ?? "dd/MM/yyyy";
        return dt.ToString(format, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && DateTime.TryParse(s, culture, DateTimeStyles.None, out var result))
            return result;
        return DependencyProperty.UnsetValue;
    }
}