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
using AppName.CustomRenderer;

namespace AppName
{
    public partial class BarCodeCheckSize : ContentPage
    {
        public BarCodeCheckSize()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", (sender, thebarcode) =>
            {
                if (Status_Check.IsChecked == false)
                {
                    var dialog = new BarCodeCheckSizeDetail();
                    dialog.BindingContext = new BarCodeCheckSizeViewModel(thebarcode.Substring(0, thebarcode.Length - 1), 1);

                    Navigation.PopModalAsync();

                    PopupNavigation.Instance.PushAsync(dialog);
                }
                else
                {
                    var dialog = new BarCodeCheckSizeDetail();
                    dialog.BindingContext = new BarCodeCheckSizeViewModel(thebarcode.Substring(0, thebarcode.Length - 1), 0);

                    Navigation.PopModalAsync();

                    PopupNavigation.Instance.PushAsync(dialog);
                }
            });
        }

        protected override void OnDisappearing()
        {
            //MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }

        void TimKiem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcode.Text))
            {
                if (Status_CheckInput.IsChecked == false)
                {
                    var dialog = new BarCodeCheckSizeDetail();
                    dialog.BindingContext = new BarCodeCheckSizeViewModel(txtBarcode.Text, 1);

                    Navigation.PopModalAsync();

                    PopupNavigation.Instance.PushAsync(dialog);
                }
                else
                {
                    var dialog = new BarCodeCheckSizeDetail();
                    dialog.BindingContext = new BarCodeCheckSizeViewModel(txtBarcode.Text, 0);

                    Navigation.PopModalAsync();

                    PopupNavigation.Instance.PushAsync(dialog);
                }
            }
            else
            {
                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please enter barcode." };
                PopupNavigation.Instance.PushAsync(dialog);
            }
        }
    }
}