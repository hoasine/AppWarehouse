using AppName.CustomRenderer;
using AppName.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using ScanditBarcodePicker.Android;
using System.Threading;
using Android.Util;
using System.IO;
using Xamarin.Forms;
using XFUniqueIdentifier.Droid;
using Android.Graphics;

[assembly: Xamarin.Forms.Dependency(typeof(XFUniqueIdentifier.Droid.ImageCompression))]
namespace XFUniqueIdentifier.Droid
{
    public class ImageCompression : IImageCompressionServices
    {
        public byte[] CompressImage(byte[] imageData, string destionationPath, int comperssionPersion)
        {
            var resizeImage = GetResizeImage(imageData, comperssionPersion);

            return resizeImage;
        }

        private byte[] GetResizeImage(byte[] imageData, int compressionPercentage)
        {
            Bitmap oriImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            using (MemoryStream ms = new MemoryStream())
            {
                if (oriImage.Height > 1500)
                {
                    var resizeBitmap = resize(oriImage, oriImage.Width / 20, oriImage.Height / 20);

                    resizeBitmap.Compress(Bitmap.CompressFormat.Jpeg, compressionPercentage, ms);
                }
                else
                {
                    oriImage.Compress(Bitmap.CompressFormat.Jpeg, compressionPercentage, ms);
                }

                return ms.ToArray();
            }
        }

        private static Bitmap resize(Bitmap image, int maxWidth, int maxHeight)
        {
            if (maxHeight > 0 && maxWidth > 0)
            {
                int width = image.Width;
                int height = image.Height;
                float ratioBitmap = (float)width / (float)height;
                float ratioMax = (float)maxWidth / (float)maxHeight;

                int finalWidth = maxWidth;
                int finalHeight = maxHeight;
                if (ratioMax > ratioBitmap)
                {
                    finalWidth = (int)((float)maxHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)maxWidth / ratioBitmap);
                }
                image = Bitmap.CreateScaledBitmap(image, finalWidth, finalHeight, true);
                return image;
            }
            else
            {
                return image;
            }
        }
    }
}