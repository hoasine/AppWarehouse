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
    public partial class CheckItemDayPage : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(CheckItemDayPage),
        null);

        CheckItemDayPageViewModel viewModel;

        #endregion

        public CheckItemDayPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new CheckItemDayPageViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as CheckSaleModel;
            if (item == null)
            {
                return;
            }

            var dialog = new CheckItemDayDetailPage();
            dialog.BindingContext = new CheckItemDayDetailViewModel(item);

            Navigation.PopModalAsync();

            PopupNavigation.Instance.PushAsync(dialog);

            ItemsListView.SelectedItem = null;
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