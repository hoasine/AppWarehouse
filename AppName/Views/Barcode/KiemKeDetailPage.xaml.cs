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

namespace AppName
{
    public partial class KiemKeDetailPage : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(BarocdeScanSanpPham),
        null);

        BarCodeSanPhamListViewModel viewModel;
     
        #endregion

        public KiemKeDetailPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new BarCodeSanPhamListViewModel();

            //MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            //{
            //    if (!string.IsNullOrEmpty(thebarcode))
            //    {

            //        var dialog = new BarCodeSanPhamDetail();
            //        dialog.BindingContext = new BarCodeSanPhamViewModel(thebarcode);

            //        Navigation.PopModalAsync();

            //        PopupNavigation.Instance.PushAsync(dialog);
            //    }
            //    else
            //    {
            //        Application.Current.MainPage.DisplayAlert("Thông báo!", "Barcode không đúng định dạng.", "OK");
            //    }
            //});
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as SanPhamModel;
            if (item == null)
                return;

            var dialog = new BarCodeSanPhamDetail();
            dialog.BindingContext = new BarCodeSanPhamViewModel(item.Barcode_No_);

            Navigation.PopModalAsync();

            PopupNavigation.Instance.PushAsync(dialog);

            ItemsListView.SelectedItem = null;
        }

        void printclick(object sender, System.EventArgs e)
        {
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
            //scanedCamera.CreateScanning?.Invoke();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        void TimKiem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {

                var dialog = new BarCodeSanPhamDetail();
                dialog.BindingContext = new BarCodeSanPhamViewModel(txtBarcode.Text);

                Navigation.PopModalAsync();

                PopupNavigation.Instance.PushAsync(dialog);
            }
            else
            {
                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please enter barcode." };
                 PopupNavigation.Instance.PushAsync(dialog);
            }
        }

        void TabMayQuetClick(object sender, EventArgs e)
        {
        }

        void Tab_Click(object sender, EventArgs e)
        {
        }

        void TabAll_Click(object sender, EventArgs e)
        {
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}