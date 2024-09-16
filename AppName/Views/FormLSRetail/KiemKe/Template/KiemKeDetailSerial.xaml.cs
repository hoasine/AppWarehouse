using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using AppName.Core;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using AppName.Model;

namespace AppName
{
    public partial class KiemKeDetailSerial : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(KiemKeDetailSerial),
        null);

        //private KiemKeDetailViewModel ViewModel
        //{
        //    get { return (KiemKeDetailViewModel)BindingContext; }
        //    set { BindingContext = value; }
        //}

        public Command<List<string>> DidScannedCommand
        {
            get { return (Command<List<string>>)GetValue(DidScannedCommandProperty); }
            set { SetValue(DidScannedCommandProperty, value); }
        }
        #endregion

        public KiemKeDetailSerial(KiemKeDetailViewModel viewModel)
        {
            InitializeComponent();

            DidScannedCommand = new Command<List<string>>(OnDidScanned);
            scanedCamera.DidScannedCommand = DidScannedCommand;

            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await CrossMedia.Current.Initialize();

                        var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                        if (cameraStatus != PermissionStatus.Granted)
                        {
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                            if (results != null)
                                cameraStatus = results[Permission.Camera];
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            scanedCamera.StartScanning?.Invoke();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            scanedCamera.StopScanning?.Invoke();
        }

        void OnDidScanned(List<string> scannedCodes)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (ScanType.IsToggled == true)
                {
                    lbBarcode.Text = scannedCodes?.LastOrDefault();
                }
                else
                {
                    lbSerialNo.Text = scannedCodes?.LastOrDefault();
                }
            });
        }

        public async Task GetItemInfo(string barcode, string itemno, string venderitemno)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                check_status.IsChecked = false;

                               var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                if (!string.IsNullOrEmpty(store))
                {
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/get_iteminfo?barcode=" + barcode + "&masterID=" + masterID.Text, string.Empty));

                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var model = JsonConvert.DeserializeObject<ItemInfoInventoryModel>(content);

                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ItemNo))
                            {
                                lbBarcode.Text = model.Barcode_No_;
                                lbItemName.Text = model.ItemName;
                                lbItemNo.Text = model.ItemNo;
                                lbVariantCode.Text = model.Variant_Code;
                                lbVenderItemNo.Text = model._Vendor_Item_No_;
                                lbLSRetailCode.Text = model.LSRetail_Code;
                                lbNo2.Text = model.No__2;
                                txtQuatity.Text = (model.ScanQuatity + 1).ToString();
                            }
                            else
                            {
                                lbBarcode.Text = "";
                                lbItemName.Text = "";
                                lbItemNo.Text = "";
                                lbVariantCode.Text = "";
                                lbVenderItemNo.Text = "";
                                txtQuatity.Text = "0";
                                lbLSRetailCode.Text = "";
                                lbNo2.Text = "";

                                Application.Current.MainPage.DisplayAlert("Notification!", "No data.", "OK");
                            }
                        }
                        else
                        {
                            Application.Current.MainPage.DisplayAlert("Notification!", "No data.", "OK");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Notification!", ex.ToString(), "OK");
            }

        }

        async void Submit_Click(object sender, System.EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(lbItemNo.Text))
                {
                    var detail = new Mobile_CheckInventoryDetailModel();
                    detail.ItemNo = lbItemNo.Text;
                    detail.MasterInventoryID = Guid.Parse(masterID.Text);
                    detail.ScanQuatity = int.Parse(txtQuatity.Text);
                    detail.VariantCode = lbVariantCode.Text;
                    detail.LSRetailCode = lbLSRetailCode.Text;
                    detail.No2 = lbNo2.Text;
                    detail.BarcodeCode = lbBarcode.Text;

                                   var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/submit_inventory", string.Empty));

                    var requestJson = new StringContent(JsonConvert.SerializeObject(detail));
                    requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(uri, requestJson).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        check_status.IsChecked = true;
                        lbBarcode.Text = "";
                        lbItemName.Text = "";
                        lbItemNo.Text = "";
                        lbVariantCode.Text = "";
                        lbVenderItemNo.Text = "";
                        txtQuatity.Text = "0";
                        lbLSRetailCode.Text = "";
                        lbNo2.Text = "";
                    }
                    else
                    {

                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification!", "Please enter data before submitting.", "OK");
                }
            }
            catch (Exception ex)
            {

            }
        }

        async void Done_Click(object sender, System.EventArgs e)
        {
            try
            {
                Navigation.PopAsync();

            }
            catch (Exception)
            {
            }
        }

        async void Search_Click(object sender, System.EventArgs e)
        {

            if (!string.IsNullOrEmpty(lbBarcode.Text))
            {
                GetItemInfo(lbBarcode.Text, "", "");
                return;
            }
            else if (!string.IsNullOrEmpty(lbVenderItemNo.Text))
            {

                GetItemInfo("", lbVenderItemNo.Text, "");
                return;
            }

            if (!string.IsNullOrEmpty(lbItemNo.Text))
            {

                GetItemInfo("", "", lbItemNo.Text);
                return;
            }

            Application.Current.MainPage.DisplayAlert("Notification!", "Please enter barcode or itemno or vendoritemno.", "OK");
        }

        async void Add_click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbBarcode.Text) && !string.IsNullOrEmpty(lbItemNo.Text))
            {
                if (string.IsNullOrEmpty(txtQuatity.Text))
                {
                    txtQuatity.Text = "0";
                }
                else
                {
                    txtQuatity.Text = (int.Parse(txtQuatity.Text) + 1).ToString();
                }
            }


            //var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString());

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = authHeader;

            //if (!string.IsNullOrEmpty(lbItemNo.Text) && !string.IsNullOrEmpty(lbVariantCode.Text) && !string.IsNullOrEmpty(masterID.Text))
            //{
            //    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/trans_quatity_inventory?itemno="
            //        + lbItemNo.Text + "&variantcode=" + lbVariantCode.Text + "&masterID=" + masterID + "&type=add", string.Empty));

            //    HttpResponseMessage response = await client.GetAsync(uri);
            //    if (response.IsSuccessStatusCode)
            //    {

            //    }
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Thông báo!", "Thiếu dữ liệu.", "OK");
            //}
        }

        async void UnAdd_click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbBarcode.Text) && !string.IsNullOrEmpty(lbItemNo.Text))
            {
                if (string.IsNullOrEmpty(txtQuatity.Text))
                {
                    txtQuatity.Text = "0";
                }
                else
                {
                    var value = int.Parse(txtQuatity.Text) - 1;

                    txtQuatity.Text = (value < 0 ? 0 : value).ToString();
                }
            }

            //var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString());

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = authHeader;

            //if (!string.IsNullOrEmpty(lbItemNo.Text) && !string.IsNullOrEmpty(lbVariantCode.Text) && !string.IsNullOrEmpty(masterID.Text))
            //{
            //    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/trans_quatity_inventory?itemno="
            //        + lbItemNo.Text + "&variantcode=" + lbVariantCode.Text + "&masterID=" + masterID + "&type=unadd", string.Empty));

            //    HttpResponseMessage response = await client.GetAsync(uri);
            //    if (response.IsSuccessStatusCode)
            //    {

            //    }
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Thông báo!", "Thiếu dữ liệu.", "OK");
            //}
        }
    }
}
