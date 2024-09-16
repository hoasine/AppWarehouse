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

namespace AppName
{
    public class RelableTermViewModel : BaseViewModel
    {
        private ObservableCollection<ProductInfoPrintTagGroupByModel> _items;
        public ObservableCollection<ProductInfoPrintTagGroupByModel> Items
        {
            get => _items;

            set => SetProperty(ref _items, value);
        }
        public Command LoadItemsCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public ICommand ConnectBluetoothCommand { get; set; }
        public ICommand OpenSettingCommand { get; set; }
        public ICommand CommandPrinter { get; set; }

        private readonly string _barcodeID;
        private ProductInfoPrintTagGroupByModel _barcode;
        private string _itemno;
        private bool _isHasData;
        private bool _isMess;

        private string _template;
        public string Template
        {
            get => _template;

            set => SetProperty(ref _template, value);
        }

        private string _StatsBluetooth;
        public string StatsBluetooth
        {
            get => _StatsBluetooth;

            set => SetProperty(ref _StatsBluetooth, value);
        }

        private BluetoothConnetedModel _DataBluetoothConneted;
        public BluetoothConnetedModel DataBluetoothConneted
        {
            get => _DataBluetoothConneted;

            set => SetProperty(ref _DataBluetoothConneted, value);
        }

        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
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

        private ImageSource _statusConnect = "ic_bluetooth_off.png";
        public ImageSource StatusConnect
        {
            get { return _statusConnect; }
            set { SetProperty(ref _statusConnect, value); }
        }

        private DateTime _DatePrinter;

        public DateTime DatePrinter
        {
            get => _DatePrinter;

            set => SetProperty(ref _DatePrinter, value);
        }

        public RelableTermViewModel()
        {
            Template = "IStagtamplate";

            Items = new ObservableCollection<ProductInfoPrintTagGroupByModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);

            OpenSettingCommand = new Command(OpenSettingCommandAsync);
            CommandPrinter = new Command<ProductInfoPrintTagGroupByModel>(PrinterTag);

            StatsBluetooth = "Connect";

            DataBluetoothConneted = new BluetoothConnetedModel()
            {
                MacID = "",
                BlueName = "",
                IsBluetooth = false,
                Status = "Connect",
                IsConnect = false,
                Template = "IStagtamplate"
            };


            DatePrinter = DateTime.Now;
        }

        private async void PrinterTag(ProductInfoPrintTagGroupByModel obj)
        {
            if (IsBusy)
                return;
            IsBusy = true;

            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                logger.Info("Printer tag price item:=" + obj.ItemNo + ".", "Info");

                if (DataBluetoothConneted.Template == "IStagtamplate")
                {
                    if (DataBluetoothConneted.IsBluetooth == true)
                    {
                        var model = new ItemInfoUpdateGiaModel();

                        model.Discout = Convert.ToInt32(obj.AfterDisc);
                        model.Barcode = obj.Barcode_No_;
                        model.ItemNo = obj.ItemNo;
                        model.ItemName = obj.ItemName;
                        model.Unit_Price = Convert.ToInt32(obj.Unit_Price);
                        model.POG = obj.POG;
                        model.SchemeDescriptionMixMatch = obj.SchemeDescriptionMixMatch;
                        model.DatetimeMixMatch = obj.DatetimeMixMatch;
                        model.SchemeDescriptionMultiBuy = obj.SchemeDescriptionMultiBuy;
                        model.DatetimeDiscount = obj.DatetimeDiscount;
                        model.DatetimeMultil = obj.DatetimeMultil;
                        model.ReturnType = obj.ReturnType;

                        var status = DependencyService.Get<IBarcodeMarkDown>().ExecuteStartPrintingIStagtamplate(model);

                        if (status == false)
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "ReConnect the printer before starting stamp printing." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Connect the printer before starting stamp printing." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                    }

                }

                if (DataBluetoothConneted.Template == "ISshelfTemplate")
                {
                    if (DataBluetoothConneted.IsBluetooth == true)
                    {
                        var model = new ItemInfoUpdateGiaModel();

                        model.Discout = Convert.ToInt32(obj.AfterDisc);
                        model.Barcode = obj.Barcode_No_;
                        model.ItemName = obj.ItemName;
                        model.ItemNo = obj.ItemNo;
                        model.Unit_Price = Convert.ToInt32(obj.Unit_Price);
                        model.POG = obj.POG;
                        model.DatetimeDiscount = obj.DatetimeDiscount;
                        model.SchemeDescriptionMixMatch = obj.SchemeDescriptionMixMatch;
                        model.DatetimeMixMatch = obj.DatetimeMixMatch;
                        model.SchemeDescriptionMultiBuy = obj.SchemeDescriptionMultiBuy;
                        model.DatetimeMultil = obj.DatetimeMultil;
                        model.ReturnType = obj.ReturnType;

                        var status = DependencyService.Get<IBarcodeMarkDown>().ExecuteStartPrintingISshelfTemplate(model);

                        if (status == false)
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "ReConnect the printer before starting stamp printing." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Connect the printer before starting stamp printing." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                    }
                }

                if (DataBluetoothConneted.Template == "ISISCTemplate")
                {
                    var model = new ItemInfoUpdateGiaModel();

                    model.Discout = Convert.ToInt32(obj.AfterDisc);
                    model.Barcode = obj.Barcode_No_;
                    model.ItemNo = obj.ItemNo;
                    model.ItemName = obj.ItemName;
                    model.Unit_Price = Convert.ToInt32(obj.Unit_Price);
                    model.POG = obj.POG;
                    model.SchemeDescriptionMixMatch = obj.SchemeDescriptionMixMatch;
                    model.DatetimeMixMatch = obj.DatetimeMixMatch;
                    model.SchemeDescriptionMultiBuy = obj.SchemeDescriptionMultiBuy;
                    model.DatetimeMultil = obj.DatetimeMultil;
                    model.DatetimeDiscount = obj.DatetimeDiscount;
                    model.ReturnType = obj.ReturnType;


                    var listItem = new ItemPromotionModel
                    {
                        Discout = Convert.ToInt32(obj.AfterDisc),
                        Barcode = obj.Barcode_No_,
                        ItemNo = obj.ItemNo,
                        ItemName = obj.ItemName,
                        Unit_Price = Convert.ToInt32(obj.Unit_Price),
                        POG = obj.POG,
                        SchemeDescriptionMixMatch = obj.SchemeDescriptionMixMatch,
                        DatetimeDiscount = obj.DatetimeDiscount,
                        DatetimeMixMatch = obj.DatetimeMixMatch,
                        SchemeDescriptionMultiBuy = obj.SchemeDescriptionMultiBuy,
                        DatetimeMultil = obj.DatetimeMultil,
                        ReturnType = obj.ReturnType
                    };

                    RealmHelper.UpdateModel(listItem, false);
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

        public partial class ResuftProductInfoPrintTagModelModel
        {
            public int code { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
            public List<ProductInfoPrintTagGroupByModel> ListData { get; set; }
        }

        private async void OpenSettingCommandAsync(object obj)
        {
            var dialog = new SettingRelable(Template);
            var viewModel = new SettingViewModel(DataBluetoothConneted);

            viewModel.ClosePopup += (send, data) =>
            {
                try
                {
                    DataBluetoothConneted = data;

                    Template = data.Template;

                    if (data.IsBluetooth == true)
                    {
                        StatsBluetooth = "Connected";
                        StatusConnect = "ic_bluetooth.png";
                    }
                    else
                    {
                        StatsBluetooth = "Connect";
                        StatusConnect = "ic_bluetooth_off.png";
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

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var model = new ProductInfoPrintTagGroupByModel();
                model.Barcode_No_ = "9580255442516";
                model.TypeMaster = "TypeMaster";
                model.ItemNo = "100002";
                model.ItemName = "Watsons Travel Set Large";
                model.OfferType = "OfferType";
                model.SchemeDescriptionMultiBuy = "Multibuy discount";
                model.DatetimeDiscount = "Date Effect";
                model.DatetimeMultil = "Date Effect";
                model.SchemeDescriptionMixMatch = "Mix & Match discount";
                model.DatetimeMixMatch = "Date Effect";
                model.PriceGroup = "PriceGroup";
                model.Image = "Image";
                model.Unit_Price = 50000;
                model.Unit_PriceStr = "50.000";
                model.AfterDisc = 25000;
                model.AfterDiscStr = "25.000";
                model.DiscPercent = 50;
                model.DiscountAmountIncludingVAT = 25000;
                model.IsDiscPercent = true;
                model.IsMixMatch = true;
                model.IsMultil = true;
                model.POG = "NG09-4";

                Items.Add(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
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
            public bool IsMultil { get; set; }
            public string SchemeDescriptionMixMatch { get; set; }
            public string DatetimeMixMatch { get; set; }
            public bool IsMixMatch { get; set; }
            public string PriceGroup { get; set; }
            public string Image { get; set; }
            public decimal Unit_Price { get; set; }
            public string Unit_PriceStr { get; set; }
            public decimal AfterDisc { get; set; }
            public string AfterDiscStr { get; set; }
            public string POG { get; set; }
            public string ReturnType { get; set; }
            public decimal DiscPercent { get; set; }
            public bool IsDiscPercent { get; set; }
            public bool IsDiscPercentNguoc { get; set; }
            public decimal DiscountAmountIncludingVAT { get; set; }
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
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "Please input Date printer." };
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

                        Items.Clear();

                        if (dataList.ListData != null && dataList.ListData.Count > 0)
                        {
                            foreach (var item in dataList.ListData)
                            {
                                item.Unit_PriceStr = item.Unit_Price.ToString("#,##");
                                item.AfterDiscStr = item.AfterDisc.ToString("#,##");

                                if (item.DiscPercent == 0)
                                {
                                    item.IsDiscPercent = false;
                                    item.IsDiscPercentNguoc = true;
                                }
                                else
                                {
                                    item.IsDiscPercent = true;
                                    item.IsDiscPercentNguoc = false;
                                }

                                if (string.IsNullOrEmpty(item.SchemeDescriptionMixMatch))
                                {
                                    item.IsMixMatch = false;
                                }
                                else
                                {
                                    item.IsMixMatch = true;
                                }

                                if (string.IsNullOrEmpty(item.SchemeDescriptionMultiBuy))
                                {
                                    item.IsMultil = false;
                                }
                                else
                                {
                                    item.IsMultil = true;
                                }
                            }

                            Items = new ObservableCollection<ProductInfoPrintTagGroupByModel>(dataList.ListData);
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
