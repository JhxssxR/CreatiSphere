using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CreatiSphere.Converters
{
    public class StringToInitialsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
                }
                if (name.Length >= 2)
                {
                    return name.Substring(0, 2).ToUpper();
                }
                return name.ToUpper();
            }
            return "??";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
