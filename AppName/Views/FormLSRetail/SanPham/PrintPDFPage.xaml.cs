using Xamarin.Forms;
using AppName.Core;
using System;

namespace AppName
{
    public partial class PrintPDFPage : ContentPage
    {
        public PrintPDFPage()
        {
            InitializeComponent();

            BindingContext = new PrintPDFViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
