using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp;

/// <summary>
/// Converts an Open Weather API icon code to a public url in order to fetch the weather forecast image.
/// </summary>
public class IconUrlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string || value is null) return "";

        var iconCode = value as string;
        return iconCode != null
            ? $"https://openweathermap.org/img/wn/{iconCode}@2x.png"
            : "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

