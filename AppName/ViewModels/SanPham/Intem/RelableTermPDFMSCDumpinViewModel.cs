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
    public class RelableTermPDFMSCDumpinViewModel : BaseViewModel
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

        protected INavigation Navigation { get; private set; }
        public ICommand OpenSettingCommand { get; set; }

        public RelableTermPDFMSCDumpinViewModel(INavigation navigation)
        {
            Navigation = navigation;

            Items = new ObservableCollection<ItemPromotionDataModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);
            DeleteItemCommand = new Command<ItemPromotionDataModel>(DeleteItemAsync);
            ClearDataCommand = new Command(ClearDataAsync);

            LablePromotion = "PRINT PROMOTION(0)";
            LableNotPromotion = "PRINT PRICE TAG(0)";

            CommandPrinter = new Command(PrinterTag);
            CommandPrinterPromotion = new Command(PrinterTagPromotion);

            OpenSettingCommand = new Command(OpenSettingCommandAsync);

            SettingPrintValue = SettingPrint();

            DatePrinter = DateTime.Now;
        }

        private TemplatePrinterValueModel SettingPrint()
        {
            var resuft = new TemplatePrinterValueModel();

            var reaml = RealmHelper.Instance;

            var getdData = reaml.All<TemplatePrinterModel>().Where(s => s.TypeTemplate == "MSC3Tag").FirstOrDefault();


            if (getdData == null)
            {
                using (var transaction = reaml.BeginWrite())
                {
                    var model = new TemplatePrinterModel();
                    model.TypeTemplate = "MSC3Tag";
                    model.Left = "0";
                    model.Top = "0.4";
                    model.Right = "0";
                    model.Bottom = "0";

                    resuft.TypeTemplate = "MSC3Tag";
                    resuft.Left = "0";
                    model.Top = "0.4";
                    resuft.Right = "0";
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
                        var model = reaml.All<TemplatePrinterModel>().Where(s => s.TypeTemplate == "MSC3Tag").FirstOrDefault();
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
            var checkData = Items.Where(s => s.Discout > 0 || s.AfterDiscMember > 0).ToList();

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
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTagMSC3TemPDF"));

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

                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTagMSC3TemNotPromotionPDF"));

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
                LoadDataAll();
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
                LoadData(keyvalue);
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

        protected async void LoadDataAll()
        {
            try
            {
                var itemTam = RealmHelper.GetItemPromotionPDFMSC3TagAll().Select(item => new ItemPromotionDataModel()
                {
                    ID = item.ID,
                    Discout = Convert.ToInt32(item.Discout),
                    Barcode = item.Barcode,
                    ItemNo = item.ItemNo,
                    ItemName = item.ItemName,
                    Unit_Price = Convert.ToInt32(item.Unit_Price),
                    POG = item.POG,
                    SchemeDescriptionMixMatch = item.SchemeDescriptionMixMatch,
                    DatetimeDiscount = item.DatetimeDiscount,
                    DatetimeMixMatch = item.DatetimeMixMatch,
                    SchemeDescriptionMultiBuy = item.SchemeDescriptionMultiBuy,
                    DatetimeMultil = item.DatetimeMultil,
                    ReturnType = item.ReturnType,
                    Unit_PriceStr = string.Format("{0:0,0}", item.Unit_Price),
                    DiscoutStr = string.Format("{0:0,0}", item.Discout),
                    IsDiscPercent = item.Discout == 0 ? false : true,
                    IsDiscPercentNguoc = item.Discout == 0 ? true : false,
                    IsMixMatch = string.IsNullOrEmpty(item.SchemeDescriptionMixMatch) ? false : true,
                    IsMultil = string.IsNullOrEmpty(item.SchemeDescriptionMultiBuy) ? false : true,
                    DateDiscountMember = item.DateDiscountMember,
                    AfterDiscMember = item.AfterDiscMember.Value
                }).ToList();

                Items.Clear();

                Items = new ObservableCollection<ItemPromotionDataModel>(itemTam);

                var checkPromotion = Items.Where(s => s.Discout > 0 || s.AfterDiscMember > 0).ToList();
                var checkNotPromotion = Items.Where(s => s.Discout == 0 && s.AfterDiscMember == 0).ToList();

                LablePromotion = "PRINT PROMOTION(" + checkPromotion.Count() + ")";
                LableNotPromotion = "PRINT PRICE TAG(" + checkNotPromotion.Count() + ")";
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        protected async void ClearDataAsync()
        {
            try
            {
                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("You confirm the current data delete?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            //Đóng popup detail trước
                            try
                            {
                                await PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }


                            var realm = RealmHelper.Instance;
                            using (var transaction = realm.BeginWrite())
                            {
                                realm.RemoveAll<ItemPromotionPDFMSC3TagModel>();
                                transaction.Commit();
                            }

                            Items.Clear();

                            var checkPromotion = Items.Where(s => s.Discout > 0 || s.AfterDiscMember > 0).ToList();
                            var checkNotPromotion = Items.Where(s => s.Discout == 0 && s.AfterDiscMember == 0).ToList();

                            LablePromotion = "PRINT PROMOTION(" + checkPromotion.Count() + ")";
                            LableNotPromotion = "PRINT PRICE TAG(" + checkNotPromotion.Count() + ")";

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Clear data sucessfully." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    catch (Exception exx)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                };

                dialogCheck.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialogCheck);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        protected async void DeleteItemAsync(ItemPromotionDataModel model)
        {
            try
            {

                var realm = RealmHelper.Instance;

                var dataCheck = RealmHelper.Instance.All<ItemPromotionPDFMSC3TagModel>().Where(s => s.Barcode == model.Barcode).FirstOrDefault();

                using (var transaction = realm.BeginWrite())
                {
                    realm.Remove(dataCheck);
                    transaction.Commit();
                }

                Items.Remove(model);

                var checkPromotion = Items.Where(s => s.Discout > 0 || s.AfterDiscMember > 0).ToList();
                var checkNotPromotion = Items.Where(s => s.Discout == 0 && s.AfterDiscMember == 0).ToList();

                LablePromotion = "PRINT PROMOTION(" + checkPromotion.Count() + ")";
                LableNotPromotion = "PRINT PRICE TAG(" + checkNotPromotion.Count() + ")";

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Delete Items:=" + model.Barcode + " sucessfully." };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void LoadData(string KeyValue)
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
                if (DatePrinter == null)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Date printer." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                else
                {
                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoPrintTag?KeyValue=" + KeyValue + "&datePrinter=" + DatePrinter.ToString("yyyy-MM-dd"), string.Empty));

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

                        var item = dataList.ListData.FirstOrDefault();

                        if (item != null)
                        {
                            //var itemTam = RealmHelper.GetItemPromotionPDFSmall(KeyValue);
                            //if (itemTam == null)
                            //{
                            var listItemUpdate = new ItemPromotionPDFMSC3TagModel
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

                            //item.Unit_PriceStr = item.Unit_Price.ToString("#,##");
                            //item.AfterDiscStr = item.AfterDisc.ToString("#,##");

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

                            RealmHelper.UpdateModel(listItemUpdate, false);

                            Items.Insert(0, listItem);

                            var checkPromotion = Items.Where(s => s.Discout > 0 || s.AfterDiscMember > 0).ToList();
                            var checkNotPromotion = Items.Where(s => s.Discout == 0 && s.AfterDiscMember == 0).ToList();

                            LablePromotion = "PRINT PROMOTION(" + checkPromotion.Count() + ")";
                            LableNotPromotion = "PRINT PRICE TAG(" + checkNotPromotion.Count() + ")";

                            try
                            {
                                PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + item.Barcode_No_ + " added." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                        else
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this view :=" + KeyValue };
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
