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
    public partial class GapCheckPage : ContentPage
    {
        GapCheckViewModel viewModel;

        public GapCheckPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new GapCheckViewModel(Navigation);
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

            await PopupNavigation.Instance.PushAsync(new GapCheckDetail(item));

            listView.SelectedItem = null;
        }
    }
}