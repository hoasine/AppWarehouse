using Xamarin.Forms;
using AppName.Core;
using System;
using AppName.Model;
using Rg.Plugins.Popup.Services;

namespace AppName
{
    public partial class SanPhamPage : ContentPage
    {
        SanPhamListViewModel viewModel;

        public SanPhamPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new SanPhamListViewModel(Navigation);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                var model = (NavigationItemData)e.SelectedItem;

                viewModel.ClickMenu(model);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }

            //var dialog = new ShipmentTODetail((NavigationItemData)e.SelectedItem);

            //PopupNavigation.Instance.PushAsync(dialog);

            //listView.SelectedItem = null;
        }

    }
}
