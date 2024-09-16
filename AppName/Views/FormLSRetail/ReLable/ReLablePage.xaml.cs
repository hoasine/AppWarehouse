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
    public partial class ReLablePage : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(ReLablePage),
        null);

        RelableTermViewModel viewModel;

        #endregion

        public ReLablePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new RelableTermViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
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

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                viewModel.ItemNo = thebarcode.Substring(0, thebarcode.Length - 1);

                viewModel.LoadData(thebarcode.Substring(0, thebarcode.Length - 1));
            });

            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
            //scanedCamera.CreateScanning?.Invoke();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            MessagingCenter.Send<App>((App)Application.Current, "DisconnectBLue");


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