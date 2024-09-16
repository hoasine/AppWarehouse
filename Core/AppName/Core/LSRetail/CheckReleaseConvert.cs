using System;
using System.Globalization;
using Xamarin.Forms;

namespace AppName.Core
{
    public class CheckReleaseConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == "0")
            {
                return "Open";
            }
            else if (value?.ToString() == "1")
            {
                return "Release";
            }
            else if (value?.ToString() == "2")
            {
                return "Close";
            }
            else
            {
                return "UnKnown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == "0")
            {
                return "Open";
            }
            else if (value?.ToString() == "1")
            {
                return "Release";
            }
            else if (value?.ToString() == "2")
            {
                return "Close";
            }
            else
            {
                return "UnKnown";
            }
        }
    }
}
