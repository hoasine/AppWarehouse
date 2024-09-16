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
    public partial class CycleCountPreviewPage : ContentPage
    {
        CycleCountPreviewViewModel viewModel;

        public CycleCountPreviewPage(StockCountModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = new CycleCountPreviewViewModel(Navigation, model);
        }

        protected override void OnAppearing()
        {
            viewModel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                if (viewModel.IsScan == false)
                {
                    viewModel.IsScan = true;
                    viewModel.ItemNo = "";

                    viewModel.CheckStockLine(thebarcode.Substring(0, thebarcode.Length - 1));
                }
            });

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Application.Current, "ReloadStockPage");

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }

        private async void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (SearchBar)sender;

            try
            {
                if (entry.Text == "")
                {
                    viewModel.SearchItemsCommand.Execute("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}", ex);
            }
        }
    }
}