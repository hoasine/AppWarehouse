using Xamarin.Forms;
using AppName.Core;
using System;

namespace AppName
{
    public partial class PrintPDFExpireDatePage : ContentPage
    {
        public PrintPDFExpireDatePage()
        {
            InitializeComponent();

            BindingContext = new PrintPDFExpireDateViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
