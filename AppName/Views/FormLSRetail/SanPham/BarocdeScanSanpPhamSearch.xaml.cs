using System;
using Xamarin.Forms;
using AppName.Core;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Permissions;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System.IO;
using System.Text;

namespace AppName
{
    public partial class BarocdeScanSanpPhamSearch : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(BarocdeScanSanpPham),
        null);

        BarCodeSanPhamListView2Model viewModel;

        #endregion

        public BarocdeScanSanpPhamSearch()
        {
            InitializeComponent();

            BindingContext = viewModel = new BarCodeSanPhamListView2Model();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            try
            {
                viewModel.ShowLoading = true;

                await Task.Delay(100);
                var item = args.SelectedItem as SanPhamModel;
                if (item == null)
                {
                    return;
                }

                var dialog = new BarCodeSanPhamDetail();
                dialog.BindingContext = new BarCodeSanPhamNotGetDataViewModel(item);

                Navigation.PopModalAsync();

                PopupNavigation.Instance.PushAsync(dialog);

                ItemsListView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                viewModel.ShowLoading = false;
            }
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            //scanedCamera.CreateScanning?.Invoke();
            base.OnAppearing();
        }

        private async void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (SearchBar)sender;

            try
            {
                if (string.IsNullOrEmpty(entry.Text))
                {
                    viewModel.SearchItemsCommand.Execute(entry.Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}", ex);
            }
        }


        private async void ScanCamera_CLick(object sender, EventArgs e)
        {
            var dialog = new CameraScanPage();
            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }
    }
}