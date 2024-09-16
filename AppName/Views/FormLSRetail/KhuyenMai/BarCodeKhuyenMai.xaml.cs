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
using AppName.Model;

namespace AppName
{
    public partial class BarCodeKhuyenMai : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(TonKhoPage),
        null);

        CTKMsGroupViewModel viewModel;

        #endregion

        public BarCodeKhuyenMai()
        {
            InitializeComponent();

            BindingContext = viewModel = new CTKMsGroupViewModel();
        }

        //async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        //{
        //    var item = args.SelectedItem as TonKhoModel;
        //    if (item == null)
        //    {
        //        return;
        //    }

        //    var dialog = new BarCodeSanPhamDetail();
        //    dialog.BindingContext = new TonKhoDetailViewModel(item);

        //    Navigation.PopModalAsync();

        //    PopupNavigation.Instance.PushAsync(dialog);

        //    ItemsListView.SelectedItem = null;
        //}

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                var checkCTKMView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "CTKM"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                if (checkCTKMView == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                }
                else
                {
                    viewModel.LoadData(thebarcode.Substring(0, thebarcode.Length - 1));
                }
            });

            base.OnAppearing();

            if (viewModel.ListCTKM.Count == 0)
                viewModel.LoadCTKMItemCommand.Execute(null);
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

        //private async void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var entry = (SearchBar)sender;

        //    try
        //    {
        //        viewModel.SearchItemsCommand.Execute(entry.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught: {0}", ex);
        //    }
        //}


        //private async void Search_CLick(object sender, EventArgs e)
        //{
        //    viewModel.SearchItemsCommand.Execute(txtsearch.Text);
        //}


        //public BarCodeKhuyenMai()
        //{
        //    InitializeComponent();

        //    MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", (sender, thebarcode) =>
        //    {
        //        var dialog = new BarCodeKhuyenMaiDetail(new CTKMsGroupViewModel());
        //        dialog.BindingContext = new CTKMsGroupViewModel(thebarcode.Substring(0, thebarcode.Length - 1));

        //        Navigation.PopModalAsync();

        //        PopupNavigation.Instance.PushAsync(dialog);
        //    });
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();

        //    MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");
        //}

        //void TimKiem_Click(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtBarcode.Text))
        //    {
        //        var dialog = new BarCodeKhuyenMaiDetail(new CTKMsGroupViewModel());
        //        dialog.BindingContext = new CTKMsGroupViewModel(txtBarcode.Text);

        //        Navigation.PopModalAsync();

        //        PopupNavigation.Instance.PushAsync(dialog);
        //    }
        //    else
        //    {
        //        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please enter barcode." };
        //        PopupNavigation.Instance.PushAsync(dialog);
        //    }
        //}
    }
}