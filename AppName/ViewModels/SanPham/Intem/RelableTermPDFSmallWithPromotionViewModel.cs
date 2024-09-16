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

namespace AppName
{
    public class RelableTermPDFSmallWithPromotionViewModel : BaseViewModel
    {
        private ObservableCollection<ItemPromotionDataModel> _items;
        public ObservableCollection<ItemPromotionDataModel> Items
        {
            get => _items;

            set => SetProperty(ref _items, value);
        }
        public Command LoadItemsCommand { get; set; }
        public Command ClearDataCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public Command<ItemPromotionDataModel> DeleteItemCommand { get; set; }
        public ICommand CommandPrinter { get; set; }
        public ICommand CommandPrinterPromotion { get; set; }

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

        private DateTime _DatePrinter;

        public DateTime DatePrinter
        {
            get => _DatePrinter;

            set => SetProperty(ref _DatePrinter, value);
        }

        private DateTime _DatePrice;

        public DateTime DatePrice
        {
            get => _DatePrice;

            set => SetProperty(ref _DatePrice, value);
        }

        private string _lableNotPromotion;

        public string LableNotPromotion
        {
            get => _lableNotPromotion;

            set => SetProperty(ref _lableNotPromotion, value);
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

        private ObservableCollection<POGMWithPromotionodel> _listFixelID;
        public ObservableCollection<POGMWithPromotionodel> ListFixelID
        {
            get { return _listFixelID; }
            set { SetProperty(ref _listFixelID, value); }
        }

        private ObservableCollection<PrintWithPromotionModel> _ListPromotion;
        public ObservableCollection<PrintWithPromotionModel> ListPromotion
        {
            get { return _ListPromotion; }
            set { SetProperty(ref _ListPromotion, value); }
        }

        private POGMWithPromotionodel _selectedFixelID;
        public POGMWithPromotionodel SelectedFixelID
        {
            get { return _selectedFixelID; }
            set { SetProperty(ref _selectedFixelID, value); }
        }
        public ICommand ChangeFixelIDCommand { get; set; }

        private ObservableCollection<POGMWithPromotionodel> _listPOG;
        public ObservableCollection<POGMWithPromotionodel> ListPOG
        {
            get { return _listPOG; }
            set { SetProperty(ref _listPOG, value); }
        }

        private POGMWithPromotionodel _selectedPOG;
        public POGMWithPromotionodel SelectedPOG
        {
            get { return _selectedPOG; }
            set { SetProperty(ref _selectedPOG, value); }
        }

        private PrintWithPromotionModel _selectedPromotion;
        public PrintWithPromotionModel SelectedPromotion
        {
            get { return _selectedPromotion; }
            set { SetProperty(ref _selectedPromotion, value); }
        }

        public Command<object> ChangePOGCommand { get; set; }
        public Command<object> ChangePromotionCommand { get; set; }


        private ObservableCollection<POGMWithPromotionodel> _listPOGFull;
        public ObservableCollection<POGMWithPromotionodel> ListPOGFull
        {
            get { return _listPOGFull; }
            set { SetProperty(ref _listPOGFull, value); }
        }

        private bool _IsEnablePOG;
        public bool IsEnablePOG
        {
            get { return _IsEnablePOG; }
            set { SetProperty(ref _IsEnablePOG, value); }
        }

        protected INavigation Navigation { get; private set; }
        public ICommand OpenSettingCommand { get; set; }

        public RelableTermPDFSmallWithPromotionViewModel(INavigation navigation)
        {
            Navigation = navigation;

            Items = new ObservableCollection<ItemPromotionDataModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ChangePromotionCommand = new Command<object>(ChangPromotionAsync);
            ChangeFixelIDCommand = new Command(ChangFixelIDAsync);
            ChangePOGCommand = new Command<object>(ChangePOGAsync);

            LablePromotion = "PRINT PROMOTION(0)";
            LableNotPromotion = "PRINT PRICE TAG(0)";

            CommandPrinter = new Command(PrinterTag);
            CommandPrinterPromotion = new Command(PrinterTagPromotion);

            OpenSettingCommand = new Command(OpenSettingCommandAsync);

            SettingPrintValue = SettingPrint();

            DatePrinter = DateTime.Now;
            DatePrice = DateTime.Now;

            //LoadPOG();
            LoadPromotion();
        }

        public class POGMWithPromotionodel
        {
            public string Store { get; set; }
            public string FixelID { get; set; }
            public string POG { get; set; }
        }

        public async void ChangFixelIDAsync()
        {
            try
            {
                if (SelectedFixelID != null && !string.IsNullOrEmpty(SelectedFixelID.FixelID))
                {
                    IsEnablePOG = true;
                    var POGList = ListPOGFull.Where(s => s.FixelID == SelectedFixelID.FixelID).OrderBy(s => s.FixelID).ToList();
                    ListPOG = new ObservableCollection<POGMWithPromotionodel>(POGList);
                }
                else
                {
                    IsEnablePOG = false;
                    ListPOG = new ObservableCollection<POGMWithPromotionodel>();
                }

                LablePromotion = "PRINT PROMOTION(0)";
                LableNotPromotion = "PRINT PRICE TAG(0)";

                Items.Clear();
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void ChangePOGAsync(object obj)
        {
            try
            {
                var calendar = (Picker)obj;
                calendar.Unfocus();

                if (SelectedPOG != null && !string.IsNullOrEmpty(SelectedPOG.POG))
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void ChangPromotionAsync(object obj)
        {
            try
            {
                var calendar = (Picker)obj;
                calendar.Unfocus();

                if (SelectedPromotion != null && !string.IsNullOrEmpty(SelectedPromotion.ProNo))
                {
                    LoadFixelID();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadPromotion()
        {
            try
            {
                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetPromotionWithStore?LocationCode=" + store);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftPrintWithPromotionModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    ListPromotion = new ObservableCollection<PrintWithPromotionModel>(dataList.ListData);
                }
                else
                {
                    ListPromotion = new ObservableCollection<PrintWithPromotionModel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadFixelID()
        {
            try
            {
                if (SelectedPromotion == null || string.IsNullOrEmpty(SelectedPromotion.ProNo))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select combobox promotion!" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }

                if (DatePrinter == null)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select date print!" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }


                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetPOGWithPromotion?LocationCode=" + store
                    + "&promotion=" + SelectedPromotion.ProNo
                    + "&datePOG=" + DatePrinter.ToString("yyyy-MM-dd"));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<List<POGMWithPromotionodel>>(content);

                    var groupByFixelID = dataList.GroupBy(s => s.FixelID).Select(s => s.FirstOrDefault()).OrderBy(s => s.FixelID);

                    ListFixelID = new ObservableCollection<POGMWithPromotionodel>(groupByFixelID);

                    ListPOGFull = new ObservableCollection<POGMWithPromotionodel>(dataList);
                }
                else
                {
                    ListFixelID = new ObservableCollection<POGMWithPromotionodel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private TemplatePrinterValueModel SettingPrint()
        {
            var resuft = new TemplatePrinterValueModel();

            var reaml = RealmHelper.Instance;

            var getdData = reaml.All<TemplatePrinterModel>().Where(s => s.TypeTemplate == "SmallTemplate").FirstOrDefault();


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


        private async void PrinterTagPromotion()
        {
            var checkData = Items.Where(s => s.Discout > 0).ToList();

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
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTagPDFSmallStream"));

                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    foreach (var item in checkData)
                    {
                        item.LeftMargin = SettingPrintValue.Left;
                        item.TopMargin = SettingPrintValue.Top;
                        item.RightMargin = SettingPrintValue.Right;
                        item.BottomMargin = SettingPrintValue.Bottom;
                    }

                    var requestJson = new StringContent(JsonConvert.SerializeObject(checkData));
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

        private async void PrinterTag()
        {
            var checkData = Items.Where(s => s.Discout == 0).ToList();

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
                    foreach (var item in checkData)
                    {
                        item.LeftMargin = SettingPrintValue.Left;
                        item.TopMargin = SettingPrintValue.Top;
                        item.RightMargin = SettingPrintValue.Right;
                        item.BottomMargin = SettingPrintValue.Bottom;
                    }

                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTagPDFSmallStreamNotPromotion"));

                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    var requestJson = new StringContent(JsonConvert.SerializeObject(checkData));
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

        public partial class ResuftProductInfoPrintTagModelModel
        {
            public int code { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
            public List<ProductInfoPrintTagGroupByModel> ListData { get; set; }
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

        public partial class ResuftPrintWithPromotionModel
        {
            public bool Active { get; set; }
            public List<PrintWithPromotionModel> ListData { get; set; }
        }

        public partial class PrintWithPromotionModel
        {
            public string ProNo { get; set; }
            public string Description { get; set; }
            public string Scenario { get; set; }
        }

        public partial class ResuftGetPOGWithPromotionModel
        {
            public int code { get; set; }
            public bool success { get; set; }
            public string message { get; set; }

            public List<POGMWithPromotionodel> data;
        }

        public async void LoadData()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            if (IsBusy)
                return;
            IsBusy = true;

            try
            {

                if (DatePrice == null)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Date price." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedPromotion.ProNo))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select promotion!" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                else
                {
                    var userstore = Application.Current.Properties["UserName"].ToString();

                    Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/Products/ProductInfoPrintTagWithPromotion?Store=" + userstore
                            + "&pog=" + Uri.EscapeDataString(SelectedPOG.POG)
                            + "&dateprice=" + DatePrice.ToString("yyyy-MM-dd")
                            + "&datePog=" + DatePrinter.ToString("yyyy-MM-dd")
                            + "&fixelID=" + Uri.EscapeDataString(SelectedFixelID.FixelID)
                            + "&promotion=" + SelectedPromotion.ProNo
                        );

                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var dataList = JsonConvert.DeserializeObject<ResuftProductInfoPrintTagModelModel>(content);

                        if (dataList.Active == false)
                        {
                            Application.Current.Properties["IsLogin"] = false;

                            Application.Current.MainPage = new NavigationPage(new LoginFrm());

                            Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                            return;
                        }

                        Items.Clear();

                        if (dataList.ListData.Count() > 0)
                        {
                            foreach (var item in dataList.ListData)
                            {
                                var listItemUpdate = new ItemPromotionPDFSmallModel
                                {
                                    Discout = Convert.ToInt32(item.AfterDisc),
                                    Barcode = item.Barcode_No_,
                                    ItemNo = item.ItemNo,
                                    ItemName = item.ItemName,
                                    Unit_Price = Convert.ToInt32(item.Unit_Price),
                                    POG = item.POG,
                                    SchemeDescriptionMixMatch = item.SchemeDescriptionMixMatch,
                                    DatetimeMixMatch = item.DatetimeMixMatch,
                                    DatetimeDiscount = item.DatetimeDiscount,
                                    SchemeDescriptionMultiBuy = item.SchemeDescriptionMultiBuy,
                                    DatetimeMultil = item.DatetimeMultil,
                                    ReturnType = item.ReturnType,
                                    DateDiscountMember = item.DateDiscountMember,
                                    AfterDiscMember = item.AfterDiscMember
                                };

                                var listItem = new ItemPromotionDataModel
                                {
                                    Discout = Convert.ToInt32(item.AfterDisc),
                                    Barcode = item.Barcode_No_,
                                    ItemNo = item.ItemNo,
                                    ItemName = item.ItemName,
                                    Unit_Price = Convert.ToInt32(item.Unit_Price),
                                    POG = item.POG,
                                    SchemeDescriptionMixMatch = item.SchemeDescriptionMixMatch,
                                    DatetimeMixMatch = item.DatetimeMixMatch,
                                    SchemeDescriptionMultiBuy = item.SchemeDescriptionMultiBuy,
                                    DatetimeDiscount = item.DatetimeDiscount,
                                    DatetimeMultil = item.DatetimeMultil,
                                    ReturnType = item.ReturnType,
                                    Unit_PriceStr = string.Format("{0:0,0}", item.Unit_Price),
                                    DiscoutStr = string.Format("{0:0,0}", item.AfterDisc),
                                    DateDiscountMember = item.DateDiscountMember,
                                    AfterDiscMember = item.AfterDiscMember
                                };

                                if (listItem.Discout == 0)
                                {
                                    listItem.IsDiscPercent = false;
                                    listItem.IsDiscPercentNguoc = true;
                                }
                                else
                                {
                                    listItem.IsDiscPercent = true;
                                    listItem.IsDiscPercentNguoc = false;
                                }

                                if (string.IsNullOrEmpty(listItem.SchemeDescriptionMixMatch))
                                {
                                    listItem.IsMixMatch = false;
                                }
                                else
                                {
                                    listItem.IsMixMatch = true;
                                }

                                if (string.IsNullOrEmpty(listItem.SchemeDescriptionMultiBuy))
                                {
                                    listItem.IsMultil = false;
                                }
                                else
                                {
                                    listItem.IsMultil = true;
                                }

                                Items.Insert(0, listItem);

                                var checkPromotion = Items.Where(s => s.Discout > 0).ToList();
                                var checkNotPromotion = Items.Where(s => s.Discout == 0).ToList();

                                LablePromotion = "PRINT PROMOTION(" + checkPromotion.Count() + ")";
                                LableNotPromotion = "PRINT PRICE TAG(" + checkNotPromotion.Count() + ")";
                            }
                        }
                        else
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this view :=" + SelectedFixelID.FixelID };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Server connect failed." };
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
}
