
using System;
using System.Globalization;
using Xamarin.Forms;

namespace AppName.Core
{
    public class ConvertScanQuantityStockColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == "KhongCo")
            {
                return "#555555";
            }
            else
            {
                return "#03a9f3";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == "KhongCo")
            {
                return "#555555";
            }
            else
            {
                return "#03a9f3";
            }
        }
    }
}
