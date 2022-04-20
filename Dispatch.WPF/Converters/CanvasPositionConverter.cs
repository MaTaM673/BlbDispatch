using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Dispatch.WPF.Converters;
[ValueConversion(typeof(Canvas), typeof(string))]
public class CanvasPositionConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Canvas canvas)
            return null;

        var position = Mouse.GetPosition(canvas);

        return $"{position.X}, {position.Y}";
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
