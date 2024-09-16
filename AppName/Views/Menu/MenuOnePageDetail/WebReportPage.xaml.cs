using Xamarin.Forms;
using AppName.Core;
using System;

namespace AppName
{
    public partial class WebReportPage : ContentPage
    {
        public WebReportPage()
        {
            InitializeComponent();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }
    }
}
