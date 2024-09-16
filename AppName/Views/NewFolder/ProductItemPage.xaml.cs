using AppName.Model;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppName
{
    public partial class ProductItemPage : ContentPage
    {
        public ProductItemPage()
        {
            InitializeComponent();
        }

        public async void OnReadResultReceived(object sender, List<ScannedResult> results)
        {
            isScanning = false;

            if (results != null && results.Count > 0 && results[0].IsGoodRead)
            {
                await LoadData(results.LastOrDefault().ResultCode);
            }

            scanningItemID = -1;
        }

        protected async Task LoadData(string barcode)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                               var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getdata?barCode=" + barcode, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var model = JsonConvert.DeserializeObject<SanPhamModel>(content);

                    SetData(model);
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification!", "Connection errors! Contact IT for help.", "Accept");
                }
            }
            catch (Exception ex)
            {

            }

        }

        void SetData(SanPhamModel model)
        {
            if (model != null)
            {
                txtItemNo.Text = model.ItemNo;
                txtItemName.Text = model.ItemName;
                txtUniprice.Text = model.UnitPrice;
            }
            else
            {
                txtItemNo.Text = "";
                txtItemName.Text = "";
                txtCatVendor.Text = "";
                txtColour.Text = "";
                txtUniprice.Text = "";
                txtVariantCode.Text = "";
            }
        }

        private void ToggleScanner(int itemID)
        {
            scanningItemID = itemID;

            if (isScanning)
                scannerControl.StopScanning();
            else
                scannerControl.StartScanning();

            isScanning = !isScanning;
        }

        private void AddNew_Tapped(object sender, EventArgs e)
        {
            ToggleScanner(-1);
        }


        #region Build Scan

        private bool appearingFirstTime = true;
        private int scanningItemID = -1;
        private bool isScanning;

        protected async override void OnAppearing()
        {
            if (appearingFirstTime)
            {
                appearingFirstTime = false;

                scannerControl.SdkVersion();

                CreateScannerDevice();
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android && await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera) == PermissionStatus.Granted)
                {
                    scannerControl.Connect();
                }
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            scannerControl.Disconnect();

            base.OnDisappearing();
        }

        private void CreateScannerDevice()
        {
            scannerControl.GetPhoneCameraDevice(ScannerCameraMode.NoAimer, ScannerPreviewOption.Defaults, false);
            scannerControl.Connect();
        }

        private void ConfigureScannerDevice()
        {
            scannerControl.SetSymbologyEnabled(Symbology.Datamatrix, true);
            scannerControl.SetSymbologyEnabled(Symbology.C128, true);
            scannerControl.SetSymbologyEnabled(Symbology.UpcEan, true);
            scannerControl.SetSymbologyEnabled(Symbology.Codabar, false);
            scannerControl.SetSymbologyEnabled(Symbology.C93, false);

        }

        public void OnConnectionCompleted(object sender, object[] args)
        {
            // If we have valid connection error param will be null,
            // otherwise here is error that inform us about issue that we have while connecting to scanner
            if ((ScannerExceptions)args[0] != ScannerExceptions.NoException)
            {
                // ask for Camera Permission if necessary (android only, for iOS we handle permission from SDK)
                if ((ScannerExceptions)args[0] == ScannerExceptions.CameraPermissionException)
                    RequestCameraPermission();
            }
        }

        private async void RequestCameraPermission()
        {
            var result = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);

            if (result[Permission.Camera] == PermissionStatus.Granted)
            {
                scannerControl.Connect();
            }
            else
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    if (await DisplayAlert(null, "Do you accept Application camera access permissions?", "OK", "Cancel"))
                        RequestCameraPermission();
                }
            }
        }

        public void OnConnectionStateChanged(object sender, ScannerConnectionStatus status)
        {
            if (status == ScannerConnectionStatus.Connected)
            {
                ConfigureScannerDevice();
            }

            isScanning = false;
        }

        #endregion
    }
}
