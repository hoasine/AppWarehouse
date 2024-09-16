using Xamarin.Forms;
using AppName.Core;
using Realms;
using AppName.Model;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Plugin.Media;
using Plugin.Media.Abstractions;
using AppName.CustomRenderer;

namespace AppName
{
    public class ConverterBase64ImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string base64Image = (string)value;

                if (base64Image == null)
                    return null;

                base64Image = base64Image.Replace('-', '+').Replace('_', '/').PadRight(4 * ((base64Image.Length + 3) / 4), '=');

                // Convert base64Image from string to byte-array
                var imageBytes = System.Convert.FromBase64String(base64Image);

                //var a = System.Text.Encoding.UTF8.GetString(imageBytes);

                IImageCompressionServices device = DependencyService.Get<IImageCompressionServices>();
                var resize = device.CompressImage(imageBytes, "", 50);

                // Return a new ImageSource

                var image = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(resize);
                });

                return image;

            }
            catch (Exception)
            {
                return ImageSource.FromStream(() => { return new MemoryStream(new byte()); });
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}