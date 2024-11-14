using System;
using Xamarin.Forms;
using AppName.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using AppName.Model;
using AppName.Services;
using System.Diagnostics;
using System.Windows.Input;
using System.IO;
using System.Text;
using static AppName.ViewModels.APIBaseViewModel;
using System.Linq;
using System.Reflection;
using static AppName.RelableTermPDFSmallViewModel;
using Xamarin.Essentials;
using System.Net;
using static AppName.PrintPDFExpireDateWithPromotionPageViewModel;

namespace AppName
{
    public class ReLablePDFDiscounrWithImportExcelViewModel : BaseViewModel
    {
        private ObservableCollection<ExpireDateDetailModel> _items;
        public ObservableCollection<ExpireDateDetailModel> Items
        {
            get => _items;

            set => SetProperty(ref _items, value);
        }
        public Command LoadItemsCommand { get; set; }
        public Command ClearDataCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public Command<ItemPromotionDataModel> DeleteItemCommand { get; set; }
        public ICommand CommandPrinterQRMD { get; set; }
        private readonly string _barcodeID;
        private ProductInfoPrintTagGroupByModel _barcode;
        private string _itemno;
        private int _Quantity;
        private bool _isHasData;
        private bool _isMess;

        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
        }


        private string _lablePromotion;

        public string LablePromotion
        {
            get => _lablePromotion;

            set => SetProperty(ref _lablePromotion, value);
        }

        private TemplatePrinterValueModel _settingPrint;

        public TemplatePrinterValueModel SettingPrintValue
        {
            get => _settingPrint;

            set => SetProperty(ref _settingPrint, value);
        }

        public int Quantity
        {
            get => _Quantity;

            set => SetProperty(ref _Quantity, value);
        }

        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        public bool IsHasData
        {
            get => _isHasData;

            set => SetProperty(ref _isHasData, value);
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }
        protected INavigation Navigation { get; private set; }
        public ICommand OpenSettingCommand { get; set; }
        public ICommand PickerFileCommand { get; set; }

        public ReLablePDFDiscounrWithImportExcelViewModel(INavigation navigation)
        {
            Navigation = navigation;

            Items = new ObservableCollection<ExpireDateDetailModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);

            LablePromotion = "PRINT DISCOUNT(0)";

            CommandPrinterQRMD = new Command(CommandPrinterQRMDAsync);

            OpenSettingCommand = new Command(OpenSettingCommandAsync);

            SettingPrintValue = SettingPrint();

            PickerFileCommand = new Command(PickerFile);
        }

        private async void PickerFile(object obj)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            // kiểm tra quyền trước khi vào hàm pick file
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                var options = new PickOptions
                {
                    FileTypes = new FilePickerFileType(
                 new Dictionary<DevicePlatform, IEnumerable<string>>
                 {
                        { DevicePlatform.Android, new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" } },
                 }),
                };

                var fileResult = await FilePicker.PickAsync(options);

                if (fileResult != null)
                {
                    var form = new MultipartFormDataContent();

                    FileStream fileStream = File.OpenRead(fileResult.FullPath);
                    var stream = new StreamContent(fileStream);
                    form.Add(stream, Path.GetFileNameWithoutExtension(fileResult.FileName), Path.GetFileName(fileResult.FileName));

                    // gọi hàm send api
                    try
                    {


                        var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization = authHeader;
                        Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTagDiscountWithImportExcel");

                        var response = client.PostAsync(uri, form).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            var dataList = JsonConvert.DeserializeObject<ResuftImportPrintDiscountModel>(content);

                            Items.Clear();

                            if (dataList.data.Count() > 0)
                            {
                                foreach (var item in dataList.data)
                                {
                                    if (item.Qty > 0)
                                    {
                                        var listItem = new ExpireDateDetailModel
                                        {
                                            DiscountAmount = Convert.ToInt32(item.Disc),
                                            BarcodeNo = item.Barcode,
                                            ItemNo = item.ItemNo,
                                            ItemName = item.ItemName,
                                            ExpireDate = item.ExpireDate,
                                            DocumentNo = "",
                                            Quantity = item.Qty
                                        };

                                        Items.Insert(0, listItem);
                                    }

                                    LablePromotion = "PRINT DISCOUNT(" + Items.Count() + ")";
                                }
                            }
                            else
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this view file" };
                                PopupNavigation.Instance.PushAsync(dialog, false);
                            }
                        }
                        else
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Server connect failed." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    catch (WebException ex)
                    {
                        IsBusy = false;

                        using (WebResponse response = ex.Response)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)response;
                            using (Stream data = response.GetResponseStream())
                            using (var reader = new StreamReader(data))
                            {
                                string text = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                ShowLoading = false;
            }
        }

        public class ResuftImportPrintDiscountModel
        {
            public int code { get; set; }
            public string message { get; set; }

            public List<ItemNoImportDiscountModel> data;

        }
        private TemplatePrinterValueModel SettingPrint()
        {
            var resuft = new TemplatePrinterValueModel();

            var reaml = RealmHelper.Instance;

            var getdData = reaml.All<TemplatePrinterModel>().Where(s => s.TypeTemplate == "ShelfTalker").FirstOrDefault();


            if (getdData == null)
            {
                using (var transaction = reaml.BeginWrite())
                {
                    var model = new TemplatePrinterModel();
                    model.TypeTemplate = "SmallTemplate";
                    model.Left = "0.88";
                    model.Top = "0.35";
                    model.Right = "0.75";
                    model.Bottom = "0";

                    resuft.TypeTemplate = "SmallTemplate";
                    resuft.Left = "0.88";
                    resuft.Top = "0.35";
                    resuft.Right = "0.75";
                    resuft.Bottom = "0";

                    reaml.Add(model);
                    transaction.Commit();
                }
            }
            else
            {
                resuft.TypeTemplate = getdData.TypeTemplate;
                resuft.Left = getdData.Left;
                resuft.Top = getdData.Top;
                resuft.Right = getdData.Right;
                resuft.Bottom = getdData.Bottom;
            }

            return resuft;
        }

        private async void OpenSettingCommandAsync()
        {
            var reaml = RealmHelper.Instance;

            var dialog = new SettingPrint();
            var viewModel = new SettingPrintModel(SettingPrintValue);

            viewModel.ClosePopup += (send, data) =>
            {
                try
                {
                    SettingPrintValue = data;

                    using (var transaction = reaml.BeginWrite())
                    {
                        var model = reaml.All<TemplatePrinterModel>().Where(s => s.TypeTemplate == "SmallTemplate").FirstOrDefault();
                        model.Left = data.Left;
                        model.Top = data.Top;
                        model.Right = data.Right;
                        model.Bottom = data.Bottom;

                        reaml.Add(model, true);
                        transaction.Commit();
                    }
                }
                catch (Exception exx)
                {
                    var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                    PopupNavigation.Instance.PushAsync(dialog);
                }
            };

            dialog.BindingContext = viewModel;

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        public partial class ResuftPDFModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public string content { get; set; }
            public string fileName { get; set; }
        }


        private async void CommandPrinterQRMDAsync()
        {
            var checkData = Items.ToList();

            if (checkData.Count() <= 0)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "No data to print pdf." };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            else
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                try
                {
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/expiredate/PrintExpireDate"));

                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    var listPrint = new List<ExpireDateDetailModel>();

                    foreach (var item in checkData)
                    {
                        if (item.Quantity > 0)
                        {
                            for (int i = 0; i < item.Quantity; i++)
                            {
                                var model = new ExpireDateDetailModel();
                                model.DiscountAmount = item.DiscountAmount;
                                model.BarcodeNo = item.BarcodeNo;
                                model.ItemNo = item.ItemNo;
                                model.ItemName = item.ItemName;
                                model.ExpireDate = item.ExpireDate;
                                model.DocumentNo = "";
                                model.LeftMargin = SettingPrintValue.Left;
                                model.TopMargin = SettingPrintValue.Top;
                                model.RightMargin = SettingPrintValue.Right;
                                model.BottomMargin = SettingPrintValue.Bottom;


                                listPrint.Add(model);
                            }
                        }
                    }

                    var requestJson = new StringContent(JsonConvert.SerializeObject(listPrint));
                    requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync(uri, requestJson);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var dataList = JsonConvert.DeserializeObject<ResuftPDFModel>(content);

                        if (dataList.code == 200)
                        {
                            var dialogDetail = new PDFViewPage(dataList.content, dataList.fileName);
                            Navigation.PushAsync(dialogDetail);
                        }
                        else
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.message };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public class ResuftProductInfoPrintTagModelModel
        {
            public int code { get; set; }
            public string message { get; set; }

            public List<ProductInfoPrintTagGroupByModel> data;
        }

        public partial class ItemNoImportDiscountModel
        {
            public string ItemNo { get; set; }
            public string Barcode { get; set; }
            public string ItemName { get; set; }
            public string ExpireDate { get; set; }
            public int Qty { get; set; }
            public string UnitPrice { get; set; }
            public string Disc { get; set; }
            public string DiscType { get; set; }
            public string RetailPrice { get; set; }
        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public class ItemPromotionDataModel
        {
            public MongoDB.Bson.ObjectId ID { get; set; }
            public string Barcode { get; set; }
            public string ItemName { get; set; }
            public string ItemNo { get; set; }
            public string POG { get; set; }
            public string ReturnType { get; set; }
            public string DatetimeDiscount { get; set; }

            public string SchemeDescriptionMixMatch { get; set; }
            public string DatetimeMixMatch { get; set; }
            public string SchemeDescriptionMultiBuy { get; set; }
            public string DatetimeMultil { get; set; }
            public decimal? Unit_Price { get; set; }
            public decimal? Discout { get; set; }
            public string PromotionNow { get; set; }
            public string PromotionUpdate { get; set; }
            public bool IsMixMatch { get; set; }
            public bool IsMultil { get; set; }
            public bool IsDiscPercent { get; set; }
            public bool IsDiscPercentNguoc { get; set; }
            public string Unit_PriceStr { get; set; }
            public string DiscoutStr { get; set; }
            public string LeftMargin { get; set; }
            public string TopMargin { get; set; }
            public string RightMargin { get; set; }
            public string BottomMargin { get; set; }
            public decimal AfterDiscMember { get; set; }
            public string DateDiscountMember { get; set; }
        }

        public ProductInfoPrintTagGroupByModel BarCode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }

        public async void ExecuteSearchItemsCommand(string keyvalue)
        {
            if (keyvalue == "")
            {
                Items.Clear();
            }
            else
            {
            }
        }

        public partial class ProductInfoPrintTagGroupByModel
        {
            public string StartDate { get; set; }
            public string EndingDate { get; set; }
            public string Barcode_No_ { get; set; }
            public string TypeMaster { get; set; }
            public string ItemNo { get; set; }
            public string ItemName { get; set; }
            public string OfferType { get; set; }
            public string SchemeDescriptionMultiBuy { get; set; }
            public string DatetimeDiscount { get; set; }
            public string DatetimeMultil { get; set; }
            public string SchemeDescriptionMixMatch { get; set; }
            public string DatetimeMixMatch { get; set; }
            public string PriceGroup { get; set; }
            public string Image { get; set; }
            public decimal Unit_Price { get; set; }
            public string Unit_PriceStr { get; set; }
            public decimal AfterDisc { get; set; }
            public string AfterDiscStr { get; set; }
            public string POG { get; set; }
            public string ReturnType { get; set; }
            public decimal DiscPercent { get; set; }
            public decimal DiscountAmountIncludingVAT { get; set; }
            public decimal AfterDiscMember { get; set; }
            public string DateDiscountMember { get; set; }
        }
    }
}
