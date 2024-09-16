using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static AppName.StockTakePreviewViewModel;

namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StockTakePreviewPage : ContentPage
    {
        StockTakePreviewViewModel viewModel;

        public StockTakePreviewPage(StockCountModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = new StockTakePreviewViewModel(Navigation, model);
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                if(viewModel.IsScan == false)
                {
                    viewModel.IsScan = true;
                    viewModel.ItemNo = "";

                     viewModel.CheckStockLine(thebarcode.Substring(0, thebarcode.Length - 1));
                }
            });

            if (viewModel.ListZone.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }

            viewModel._running = true;

            base.OnAppearing();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var item = e.SelectedItem as AreasModel;
                if (item == null)
                    return;

                var viewmodel = BindingContext as StockTakePreviewViewModel;
                viewmodel.SelectedZone = item;
                viewmodel.LoadDataLines(viewmodel.DataModel.DocumentNo, item.Code);

                listViewStockTake.SelectedItem = null;

                viewmodel.VisibleAlternately = false;
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Application.Current, "ReloadStockTakePage");

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            viewModel._running = false;

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
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async void TextChangedAlternately_Tapped(object sender, EventArgs e)
        {
            try
            {
                var search = sender as SearchBar;

                var viewmodel = BindingContext as StockTakePreviewViewModel;

                viewmodel.SearchAlternatelyAsync(search.Text);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }
    }
}