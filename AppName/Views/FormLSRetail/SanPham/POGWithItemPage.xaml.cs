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
    public partial class POGWithItemPage : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(CheckItemDayPage),
        null);

        PogWithItemsViewModel viewModel;

        #endregion

        public POGWithItemPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PogWithItemsViewModel(Navigation);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as CheckSaleModel;
            //if (item == null)
            //{
            //    return;
            //}

            //var dialog = new CheckItemDayDetailPage();
            //dialog.BindingContext = new CheckItemDayDetailViewModel(item);

            //Navigation.PopModalAsync();

            //PopupNavigation.Instance.PushAsync(dialog);

            //ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }
    }
}