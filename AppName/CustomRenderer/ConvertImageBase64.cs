using AppName.Model;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppName.CustomRenderer
{
    public class ConvertImageBase64
    {
        public static ImageSource ConvertImage(string value)
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
                var resize = device.CompressImage(imageBytes, "", 10);

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
    }
}
