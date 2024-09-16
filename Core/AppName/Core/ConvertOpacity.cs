using System;
using System.Globalization;
using Xamarin.Forms;

namespace AppName.Core
{
    public class ConvertOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return 1;
            }
            else
            {
                return 0.3;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return 1;
            }
            else
            {
                return 0.3;
            }
        }
    }
}
