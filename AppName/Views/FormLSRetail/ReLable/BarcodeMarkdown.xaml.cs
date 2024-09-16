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
using System.Threading;
using tsclib;
using Rg.Plugins.Popup.Services;
using AppName.Model;

namespace AppName
{
    public partial class BarcodeMarkdown : ContentPage
    {
        public BarcodeMarkdown()
        {
            InitializeComponent();

            BindingContext = new BarCodeGiaGiamViewModel();

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", (sender, thebarcode) =>
            {
                if (CalendarPicker.SelectedItem != null && PeriodDiscoutLinePicker.SelectedItem != null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await GetItemInfo(thebarcode.Substring(0, thebarcode.Length - 1));
                    });
                }
                else
                {
                    if (CalendarPicker.SelectedItem == null)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select promotion." };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }

                    if (PeriodDiscoutLinePicker.SelectedItem == null)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select promotion type." };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            });
        }

        #region private methods

        public partial class ResuftProductInfoPrintTagModelModel
        {
            public bool Active { get; set; }
            public List<ProductInfoPrintTagModel> ListData { get; set; }
        }

        public partial class ProductInfoPrintTagModel
        {
            public string Barcode_No_;
            public string Variant;
            public string VariantName;
            public string VendorItemNo;
            public string TypeMaster;
            public string ItemNo;
            public string ItemName;
            public string No__Series;
            public string DiscountTypeName;
            public string OfferType;
            public string BrandName;
            public string SchemeDescription;
            public string Priority;
            public string SchemeNo;
            public string PriceGroup;
            public string Image;
            public int ItemYear;
            public decimal Unit_Price;
            public decimal AfterDisc;
            public decimal DiscountAmountIncludingVAT;
        }

        protected override void OnDisappearing()
        {
            try
            {
                DependencyService.Get<IBarcodeMarkDown>().DisconnectPrinter();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Notification!", "Please connect bluetooth", "OK");
            }


            //MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");
            base.OnDisappearing();
        }

        public async Task GetItemInfo(string barcode)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var store = Application.Current.Properties["UserName"].ToString();

            if (!string.IsNullOrEmpty(store))
            {
                var periodicDiscountSelect = CalendarPicker.ItemsSource[CalendarPicker.SelectedIndex] as PeriodicDiscountModel;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getiteminfo?store=" + store + "&no_po="
                    + periodicDiscountSelect.No + "&barCode=" + barcode, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var model = JsonConvert.DeserializeObject<ItemInfoUpdateGiaModel>(content);

                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.ItemNo))
                        {
                            txtMaSP.Text = model.ItemNo;
                            txtTenSP.Text = model.ItemName;
                            txtGia.Text = String.Format("{0:0,0}", System.Convert.ToDecimal(model.Unit_Price)) + " (VNĐ)";
                            txtKhuyenMai.Text = model.PromotionNow;
                            txtBarcode.Text = barcode;
                            txtDiscount.Text = System.Convert.ToInt32(model.Discout).ToString();
                            txtUnitPrice.Text = System.Convert.ToInt32(model.Unit_Price).ToString();
                        }
                        else
                        {
                            txtMaSP.Text = "";
                            txtTenSP.Text = "";
                            txtGia.Text = "";
                            txtKhuyenMai.Text = "";

                            await Application.Current.MainPage.DisplayAlert("Notification!", "No data!", "OK");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Notification!", "No data!", "OK");
                    }

                }
            }
        }

        async void ConnectPrint_Click(object sender, System.EventArgs e)
        {
            var dialog = new BluetoothPage();
            await PopupNavigation.Instance.PushAsync(dialog);
        }

        async void Printer_Click(object sender, System.EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(txtMaSP.Text) && CalendarPicker.SelectedItem != null && PeriodDiscoutLinePicker.SelectedItem != null)
                {
                                   var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    var store = Application.Current.Properties["UserName"].ToString();

                    if (!string.IsNullOrEmpty(store))
                    {
                        Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/updatespectgroup?itemno=" + txtMaSP.Text + "&no_spectOld="
                            + txtKhuyenMai.Text + "&no_spectNew=" + PeriodDiscoutLinePicker.SelectedItem);

                        var model = new ItemInfoUpdateGiaModel();

                        model.Discout = int.Parse(PeriodDiscoutLinePicker.SelectedItem.ToString().Substring(4, 2));
                        model.Barcode = txtBarcode.Text;
                        model.Unit_Price = int.Parse(txtUnitPrice.Text);

                        DependencyService.Get<IBarcodeMarkDown>().ExecuteStartPrintingISshelfTemplate(model);

                        HttpResponseMessage response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            //var model = new ItemInfoUpdateGiaModel();

                            //model.Discout = int.Parse(PeriodDiscoutLinePicker.SelectedItem.ToString().Substring(4, 2));
                            //model.Barcode = txtBarcode.Text;
                            //model.Unit_Price = int.Parse(txtUnitPrice.Text);

                            //DependencyService.Get<IBarcodeMarkDown>().ExecuteStartPrinting(model);

                            txtMaSP.Text = "";
                            txtTenSP.Text = "";
                            txtGia.Text = "";
                            txtKhuyenMai.Text = "";
                        }
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification!", "Information is not enough to update.", "OK");
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Notification!", "Please connect the printer.", "OK");

            }
        }

        #endregion
    }
}