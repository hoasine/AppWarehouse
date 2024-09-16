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
    public class PrintPDFExpireDateViewModel : BaseViewModel
    {
        private ObservableCollection<ExpireDateDetailModel> _items;
        public ObservableCollection<ExpireDateDetailModel> Items
        {
            get => _items;

            set => SetProperty(ref _items, value);
        }
        public Command ClearDataCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public ICommand CommandPrinter { get; set; }
        public ICommand ChangeDateCommand { get; set; }

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

        private string _LablePrinter;

        public string LablePrinter
        {
            get => _LablePrinter;

            set => SetProperty(ref _LablePrinter, value);
        }

        private bool _IsEnableHeader;
        public bool IsEnableHeader
        {
            get { return _IsEnableHeader; }
            set { SetProperty(ref _IsEnableHeader, value); }
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

        private ObservableCollection<ExpireDateHeaderModel> _ListHeader;
        public ObservableCollection<ExpireDateHeaderModel> ListHeader
        {
            get { return _ListHeader; }
            set { SetProperty(ref _ListHeader, value); }
        }

        public ICommand ChangeHeaderCommand { get; set; }


        private ExpireDateHeaderModel _selectedHeader;
        public ExpireDateHeaderModel SelectedHeader
        {
            get { return _selectedHeader; }
            set { SetProperty(ref _selectedHeader, value); }
        }


        private ObservableCollection<DiscountModel> _ListPromotion;
        public ObservableCollection<DiscountModel> ListPromotion
        {
            get { return _ListPromotion; }
            set { SetProperty(ref _ListPromotion, value); }
        }

        private DiscountModel _SelectPromotion;
        public DiscountModel SelectPromotion
        {
            get { return _SelectPromotion; }
            set { SetProperty(ref _SelectPromotion, value); }
        }

        protected INavigation Navigation { get; private set; }
        public ICommand OpenSettingCommand { get; set; }

        public PrintPDFExpireDateViewModel(INavigation navigation)
        {
            Navigation = navigation;

            Items = new ObservableCollection<ExpireDateDetailModel>();

            ChangeHeaderCommand = new Command(ChangeHeaderSync);
            ChangeDateCommand = new Command(ChangeDateAsync);

            LablePrinter = "PRINT EXPIREDATE(0)";

            CommandPrinter = new Command(PrinterTag);

            OpenSettingCommand = new Command(OpenSettingCommandAsync);
            SettingPrintValue = SettingPrint();

            DatePrinter = DateTime.Now;

            LoadHeader();
            LoadPromotion();

            IsEnableHeader = false;
        }

        public partial class ExpireDateHeaderModel
        {
            public string DocumentNo { get; set; }
        }

        public partial class ResuftExpireDateHeaderModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
            public List<ExpireDateHeaderModel> ListData { get; set; }
        }

        public partial class DiscountModel
        {
            public string OfferNo { get; set; }
            public string Description { get; set; }
        }

        public partial class ExpireDateDetailModel
        {
            public string DocumentNo { get; set; }
            public string ItemName { get; set; }
            public string ItemNo { get; set; }
            public string BarcodeNo { get; set; }
            public string ExpireDate { get; set; }
            public string UnitCode { get; set; }
            public int Quantity { get; set; }
            public double DiscountAmount { get; set; }
            public string LeftMargin { get; set; }
            public string TopMargin { get; set; }
            public string RightMargin { get; set; }
            public string BottomMargin { get; set; }
        }

        public partial class ResuftExpireDateDetailModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
            public List<ExpireDateDetailModel> ListData { get; set; }
        }

        public async void ChangeHeaderSync(object obj)
        {
            try
            {
                var calendar = (Picker)obj;
                calendar.Unfocus();

                if (SelectPromotion != null && string.IsNullOrEmpty(SelectPromotion.OfferNo))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please, Select a promotion!" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                if (SelectedHeader != null && !string.IsNullOrEmpty(SelectedHeader.DocumentNo))
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


        public async void ChangeDateAsync()
        {
            try
            {
                if (DatePrinter != null)
                {
                    IsEnableHeader = false;

                    LoadHeader();

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

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/expiredate/GetDiscount");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<List<DiscountModel>>(content);

                    ListPromotion = new ObservableCollection<DiscountModel>(dataList);
                }
                else
                {
                    ListPromotion = new ObservableCollection<DiscountModel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }


        private async Task LoadHeader()
        {
            try
            {
                if (DatePrinter == null)
                {
                    ListHeader = new ObservableCollection<ExpireDateHeaderModel>();

                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please Date Printer price." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/expiredate/GetExpireDateHeader?Date=" + DatePrinter.Date.ToString("yyyy-MM-dd"));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftExpireDateHeaderModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.ListData.Count() > 0)
                    {
                        ListHeader = new ObservableCollection<ExpireDateHeaderModel>(dataList.ListData);
                    }
                    else
                    {
                        ListHeader = new ObservableCollection<ExpireDateHeaderModel>();

                        //var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Not found data with Date:=" + DatePrinter.Date.ToString("dd/MM/yyyy") };
                        //PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
                else
                {
                    ListHeader = new ObservableCollection<ExpireDateHeaderModel>();
                }
            }
            catch (Exception ex)
            {
                ListHeader = new ObservableCollection<ExpireDateHeaderModel>();

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

        private async void PrinterTag()
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
                    foreach (var item in checkData)
                    {
                        item.LeftMargin = SettingPrintValue.Left;
                        item.TopMargin = SettingPrintValue.Top;
                        item.RightMargin = SettingPrintValue.Right;
                        item.BottomMargin = SettingPrintValue.Bottom;
                    }

                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/expiredate/PrintExpireDate"));

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

                if (DatePrinter == null)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Date printer." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                else
                {
                    var userstore = Application.Current.Properties["UserName"].ToString();

                    Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/expiredate/GetExpireDateDetail?DocumentNo=" + Uri.EscapeDataString(SelectedHeader.DocumentNo) + "&offerNo=" + SelectPromotion.OfferNo);

                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var dataList = JsonConvert.DeserializeObject<ResuftExpireDateDetailModel>(content);

                        if (dataList.Active == false)
                        {
                            Application.Current.Properties["IsLogin"] = false;

                            Application.Current.MainPage = new NavigationPage(new LoginFrm());

                            Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                            return;
                        }

                        if (dataList.ListData.Count() > 0)
                        {
                            Items = new ObservableCollection<ExpireDateDetailModel>(dataList.ListData);

                            LablePrinter = "PRINT EXPIREDATE(" + Items.Count() + ")";
                        }
                        else
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this view :=" + SelectedHeader.DocumentNo };
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
