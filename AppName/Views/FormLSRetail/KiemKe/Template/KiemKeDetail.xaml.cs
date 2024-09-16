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
    public partial class KiemKeDetail : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(KiemKeDetail),
        null);

        BarcodeKiemKeDetailViewModel viewModel;

        public InventoryMasterModel modelMaster;

        //private KiemKeDetailViewModel ViewModel
        //{
        //    get { return (KiemKeDetailViewModel)BindingContext; }
        //    set { BindingContext = value; }
        //}

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as SanPhamModel;
            if (item == null)
                return;

            var dialog = new BarCodeSanPhamDetail();
            dialog.BindingContext = new BarCodeSanPhamViewModel(item.ItemNo);

            Navigation.PopModalAsync();

            PopupNavigation.Instance.PushAsync(dialog);

            ItemsListView.SelectedItem = null;
        }

        #endregion

        public KiemKeDetail(InventoryMasterModel masterModel)
        {
            InitializeComponent();

            modelMaster = masterModel;

            if (masterModel.CheckType == "0")
            {
                lbVenderItemNo.IsEnabled = false;

            }
            else
            {
                lbVenderItemNo.IsEnabled = true;
            }

            lbItemName.IsEnabled = false;
            lbVariantCode.IsEnabled = false;
        }

        public async void LoadData(string barcode, string itemno, string venderitemno, string type)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            check_status.IsChecked = false;

                           var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var store = Application.Current.Properties["UserName"].ToString();

            if (!string.IsNullOrEmpty(store))
            {
                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/get_iteminfo?barcode=" + barcode + "&masterID=" + masterID.Text
                    + "&itemno=" + itemno + "&venderitemno=" + venderitemno + "&type=" + type, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var model = JsonConvert.DeserializeObject<ItemInfoInventoryModel>(content);

                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ItemNo))
                        {
                            lbBarcode.Placeholder = model.Barcode_No_;
                            lbItemNo.Placeholder = model.ItemNo;
                            lbItemName.Text = model.ItemName;
                            lbVariantCode.Text = model.Variant_Code;
                            lbVenderItemNo.Text = model._Vendor_Item_No_;
                            lbLSRetailCode.Text = model.LSRetail_Code;
                            lbNo2.Text = model.No__2;
                            txtQuatity.Text = (model.ScanQuatity + 1).ToString();
                        }
                        else
                        {
                            lbBarcode.Placeholder = "Barcode...";
                            lbItemNo.Placeholder = "ItemNo...";
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

        void BarcodeChange(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            lbItemNo.Text = "";
            lbItemNo.Placeholder = "ItemNo...";
        }

        void Clear_Click(object sender, System.EventArgs e)
        {
            lbBarcode.Placeholder = "Barcode...";
            lbItemNo.Placeholder = "ItemNo...";
            lbBarcode.Text = "";
            lbItemName.Text = "";
            lbItemNo.Text = "";
            lbVariantCode.Text = "";
            lbVenderItemNo.Text = "";
            txtQuatity.Text = "";
            lbLSRetailCode.Text = "";
            lbNo2.Text = "";
        }

        void ItemnoChange(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            lbBarcode.Text = "";
            lbBarcode.Placeholder = "Barcode...";
        }

        void Done_Click(object sender, System.EventArgs e)
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
            if (modelMaster.CheckType == "0")
            {
                if (!string.IsNullOrEmpty(lbBarcode.Text) || !string.IsNullOrEmpty(lbItemNo.Text))
                {
                    if (!string.IsNullOrEmpty(lbBarcode.Text))
                    {
                        LoadData(lbBarcode.Text, "", "", "Barcode");
                    }
                    else if (!string.IsNullOrEmpty(lbItemNo.Text))
                    {
                        LoadData("", lbItemNo.Text, "", "ItemNo");
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification!", "Please enter Barcode or ItemNo", "OK");
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lbBarcode.Text) || !string.IsNullOrEmpty(lbItemNo.Text))
                {
                    if (string.IsNullOrEmpty(lbVenderItemNo.Text))
                    {
                        Application.Current.MainPage.DisplayAlert("Notification!", "Please enter Serial", "OK");
                        return;
                    }

                    if (!string.IsNullOrEmpty(lbBarcode.Text))
                    {
                        LoadData(lbBarcode.Text, "", lbVenderItemNo.Text, "SerialBarocde");
                    }
                    else if (!string.IsNullOrEmpty(lbItemNo.Text))
                    {
                        LoadData("", lbItemNo.Text, lbVenderItemNo.Text, "SerialItemNo");
                    }

                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification!", "Please enter Barcode or ItemNo", "OK");
                    return;
                }
            }
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
        }
    }
}
