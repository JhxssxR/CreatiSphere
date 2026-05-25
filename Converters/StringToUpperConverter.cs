using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Converters
{
    public class StringToUpperConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
                return str.ToUpper();
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
