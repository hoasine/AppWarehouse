using Xamarin.Forms;
using AppName.Core;

using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AppName
{
    public partial class PageAppNameCogNexPage : ContentPage
    {
        public static Page myPage;
        public PageAppNameCogNexPage()
        {
            StackLayout slContent = new StackLayout() { Orientation = StackOrientation.Vertical };

            Button btnScan = new Button
            {
                Text = String.Format("Tap to scan!")
            };

            btnScan.Clicked += async (sender, args) =>
            {
                DependencyService.Get<IScanInterface>().Scan();
            };

            slContent.Children.Add(btnScan);

            Button btnScanFromImage = new Button
            {
                Text = String.Format("Scan from image!")
            };

            btnScanFromImage.Clicked += async (sender, args) =>
            {
                DependencyService.Get<IScanInterface>().ScanFromImage();
            };

            slContent.Children.Add(btnScanFromImage);

            Content = slContent;

            myPage = this;
        }
    }
}