using Xamarin.Forms;
using AppName.Core;
using System;

namespace AppName
{
    public partial class XuatNhapPage : ContentPage
    {
        public XuatNhapPage()
        {
            InitializeComponent();

            BindingContext = new NhapXuatViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnAppearing();
        }
    }
}
