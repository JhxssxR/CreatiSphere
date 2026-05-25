using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CreatiSphere.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                status = status.ToUpper();
                string type = parameter as string ?? "Background";

                if (type == "Background")
                {
                    return status switch
                    {
                        "WON" => Color.FromArgb("#DCFCE7"),
                        "NEGOTIATION" => Color.FromArgb("#DBEAFE"),
                        "HOT LEAD" => Color.FromArgb("#FEE2E2"),
                        "QUALIFIED" => Color.FromArgb("#F3E8FF"),
                        _ => Color.FromArgb("#F1F5F9")
                    };
                }
                else // Text
                {
                    return status switch
                    {
                        "WON" => Color.FromArgb("#166534"),
                        "NEGOTIATION" => Color.FromArgb("#1E40AF"),
                        "HOT LEAD" => Color.FromArgb("#991B1B"),
                        "QUALIFIED" => Color.FromArgb("#6B21A8"),
                        _ => Color.FromArgb("#475569")
                    };
                }
            }
            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
