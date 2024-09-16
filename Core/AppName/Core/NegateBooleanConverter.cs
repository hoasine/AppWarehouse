using System;
using System.Globalization;
using Xamarin.Forms;

namespace AppName.Core
{
	public class CheckBluetoothConvert : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.ToString() == "Connected")
			{
				return true;
            }
            else
			{
				return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.ToString() == "Connected")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
