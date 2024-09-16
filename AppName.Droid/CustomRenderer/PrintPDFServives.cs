using Android.Bluetooth;
using Android.Device;
using Android.OS;
using Android.Widget;
using AppName.Droid.CustomRenderer;
using Android.Content;
using Android.Print;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Print.PrintAttributes;

[assembly: Xamarin.Forms.Dependency(typeof(PrintPDFServives))]
namespace AppName.Droid.CustomRenderer
{
    class PrintPDFServives : IPrintService
    {
        public void Print(Stream inputStream, string fileName)
        {
            if (inputStream.CanSeek)
                //Reset the position of PDF document stream to be printed
                inputStream.Position = 0;
            string createdFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            using (var dest = System.IO.File.OpenWrite(createdFilePath))
                inputStream.CopyTo(dest);
            string filePath = createdFilePath;
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            PrintManager printManager = (PrintManager)activity.GetSystemService(Context.PrintService);
            PrintDocumentAdapter pda = new CustomPrintDocumentAdapter(filePath);

            if (fileName.Contains("ShelfTalker"))
            {
                var attribute = new PrintAttributes.Builder().SetMediaSize(MediaSize.IsoA4.AsLandscape()).Build();

                printManager.Print(fileName, pda, attribute);
            }
            else
            {
                var attribute = new PrintAttributes.Builder().SetMediaSize(MediaSize.IsoA4.AsPortrait()).Build();

                printManager.Print(fileName, pda, attribute);

            }
        }
    }
}