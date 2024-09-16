using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaoPhieuXuatPage : ContentPage
    {
        TaoPhieuXuatViewModel viewModel;

        public TaoPhieuXuatPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new TaoPhieuXuatViewModel(Navigation, this);
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                if (viewModel.IsScan == false)
                {
                    viewModel.IsScan = true;

                    viewModel.OpenSanPhamDetailAsync(thebarcode.Substring(0, thebarcode.Length - 1));
                }

            });


            viewModel.LoadItemsCommand.Execute(null);

            base.OnAppearing();
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
         
        }
    }
}