using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickUPLineDetail : ContentPage
    {
        PickUPLineDetailViewModel viewModel;

        public PickUPLineDetail()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopModalAsync();
            }
            catch (Exception) { }
        }

        private async void BtnCam_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                    Directory = "Pickup Pictures",
                    SaveToAlbum = true,
                    CompressionQuality = 30,
                    Name = $"{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_pickup.jpg",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
                });

                if (photo != null)
                {
                    imgCam.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

                    imgCam.IsVisible = true;
                    imageIsSource.IsVisible = false;

                    pathImage.Text = photo.Path;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
        }
    }
}