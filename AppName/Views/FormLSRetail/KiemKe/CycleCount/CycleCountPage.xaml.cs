using Rg.Plugins.Popup.Services;
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

    public partial class CycleCountPage : ContentPage
    {
        CycleCountViewModel viewModel;

        public CycleCountPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new CycleCountViewModel(Navigation);
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<App>((App)Application.Current, "RefreshCountStock");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            if (viewModel.ListStockCount.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
            }

            base.OnAppearing();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as StockCountModel;
            if (item == null)
                return;

            await PopupNavigation.Instance.PushAsync(new CycleCountDetail(item));

            listView.SelectedItem = null;
        }
    }
}