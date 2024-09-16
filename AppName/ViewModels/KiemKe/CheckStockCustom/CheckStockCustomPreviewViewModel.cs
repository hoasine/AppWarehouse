using AppName.Model;
using AppName.Model.CycleCount;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static AppName.ViewModels.APIBaseViewModel;

namespace AppName
{
    public class CheckStockCustomPreviewViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        private string _Release;
        public string Release
        {
            get { return _Release; }
            set { SetProperty(ref _Release, value); }
        }

        private bool _IsScan;
        public bool IsScan
        {
            get { return _IsScan; }
            set { SetProperty(ref _IsScan, value); }
        }

        private bool _IsStaff;
        public bool IsStaff
        {
            get { return _IsStaff; }
            set { SetProperty(ref _IsStaff, value); }
        }

        private string _itemno;
        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
        }

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        public Command<string> SearchItemsCommand { get; set; }

        public ICommand ReleaseCommand { get; set; }
        public ICommand DeleteCheckStockCustomLineCommand { get; set; }
        public ICommand EditCheckStockCustomLineCommand { get; set; }
        public Command LoadItemsCommand { get; set; }

        public CheckStockCustomPreviewViewModel(INavigation navigation, StockCountModel model)
        {
            ReleaseCommand = new Command(ReleaseAsync);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);
            DeleteCheckStockCustomLineCommand = new Command<StockCountLineModel>(DeleteCheckStockCustomLineAsync);

            IsScan = false;

            Navigation = navigation;

            DataModel = model;

            if (DataModel.Release == 0)
            {
                Release = "Release";
            }
            else if (DataModel.Release == 1)
            {
                Release = "ReOpen";
            }
            else if (DataModel.Release == 2)
            {
                Release = "Close";
            }
            else if (DataModel.Release == 3)
            {
                Release = "UNKNOWN";
            }

            var userName = Application.Current.Properties["UserStore"]?.ToString();
            if (model.RetailStaff.ToUpper() == userName.ToUpper())
            {
                IsStaff = true;
            }
            else
            {
                IsStaff = false;
            }

            EditCheckStockCustomLineCommand = new Command<StockCountLineModel>(EditCheckStockCustomLineAsync);
        }

        public async void DeleteCheckStockCustomLineAsync(StockCountLineModel obj)
        {
            if (DataModel.Release != 0)
            {
                var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                PopupNavigation.Instance.PushAsync(dialog3);
                return;
            }

            try
            {

                var currentData = new LSRetail_StockCountDetailModel()
                {
                    DocumentNo = obj.DocumentNo,
                    Zone = 0,
                    BarcodeNo = obj.BarcodeNo,
                    ID = obj.ID,
                    POG = "",
                    FixelID = "",
                    DateCreate = obj.DateCreate,
                    ItemNo = obj.ItemNo,
                    Quantity = obj.Quantity,
                    Quantity_Scan = obj.Quantity_Scan,
                    UserCreate = obj.UserCreate,
                    IsDelete = 1
                };

                var check = await UpsertQuantityScan(currentData);

                if (check == true)
                {
                    var temp = new List<StockCountLineModel>(ListStockLine);
                    temp.Remove(obj);

                    ListStockLine.Clear();
                    ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                    DataModel.CountItem = ListStockLine.Count();
                    DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                    DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                    DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void ExecuteSearchItemsCommand(string keyvalue)
        {
            if (keyvalue == "")
            {
                var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Input BarcodeCode to search." };
                PopupNavigation.Instance.PushAsync(dialog3);
            }
            else
            {
                CheckStockLine(keyvalue);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            LoadDataLines(DataModel.DocumentNo);
        }

        public async void EditCheckStockCustomLineAsync(StockCountLineModel obj)
        {
            try
            {
                if (DataModel.Release != 0)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                var dialog = new StockTakeLineDetail();
                var viewModel = new StockTakeLineDetailViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        viewModel.ShowLoading = true;

                        await Task.Delay(1);

                        var currentData = new LSRetail_StockCountDetailModel()
                        {
                            DocumentNo = obj.DocumentNo,
                            Zone = 0,
                            BarcodeNo = obj.BarcodeNo,
                            ID = obj.ID,
                            POG = obj.POG,
                            FixelID = obj.FixID,
                            DateCreate = obj.DateCreate,
                            ItemNo = obj.ItemNo,
                            Quantity = obj.Quantity,
                            Quantity_Scan = obj.Quantity_Scan,
                            UserCreate = obj.UserCreate,
                            IsDelete = 0
                        };

                        var check = await UpsertQuantityScan(currentData);
                        if (check == true)
                        {
                            var temp = new List<StockCountLineModel>(ListStockLine);
                            temp.Remove(obj);
                            temp.Insert(0, data);

                            ListStockLine.Clear();
                            ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                            DataModel.CountItem = ListStockLine.Count();
                            DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                            DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                            DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                        PopupNavigation.Instance.PushAsync(dialog, false);

                        return;
                    }
                    finally
                    {
                        viewModel.ShowLoading = false;
                    }
                };

                dialog.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialog);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
        }

        private async void ReleaseAsync(object obj)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                var checkRelase = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ChangeStatusStockCountReOpen"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                if (checkRelase == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission change status stock.", "OK");
                    return;
                }

                var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You must be the person who created the ticket. You do not have permission for this function. With " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                if (DataModel.Release == 2)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = DataModel.DocumentNo + " closed status cannot be edited." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/RealseDocumentScan?DocumentNo=" + DataModel.DocumentNo, string.Empty));

                var response = client.GetAsync(uri).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<RealseDocumentScanModel>(content);


                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {

                        if (dataList.Status == "Open")
                        {
                            Release = "Release";
                            DataModel.Release = 0;

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Changed status Document:=" + DataModel.DocumentNo + " Open successfully." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "Release")
                        {
                            Release = "ReOpen";
                            DataModel.Release = 1;

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Changed status Document:=" + DataModel.DocumentNo + " Release successfully." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "NoLines")
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Cycle Count lines no data to report " + DataModel.DocumentNo + "." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "Sent")
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = DataModel.DocumentNo + " has been sent so can't reopen." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else
                        {
                            Release = "UNKNOWN";

                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Changed status UNKNOWN." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                        return;
                    }
                }
                else
                {
                    return;
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

        private async void LoadDataLines(string documentNo)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetCheckStockCustomLine?masterID=" + documentNo);

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountDetailListModel>(content);


                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
                    }

                    if (dataList != null)
                    {
                        if (dataList.Code == 200)
                        {
                            if (dataList.ListData.Count() > 0)
                            {
                                foreach (var item in dataList.ListData)
                                {
                                    if (item.Quantity_Scan == item.Quantity)
                                    {
                                        item.ComparePCS = false;
                                    }
                                    else if (item.Quantity_Scan > item.Quantity)
                                    {
                                        item.ComparePCS = true;
                                    }

                                    item.ReferenceInfo = !string.IsNullOrEmpty(item.ReferenceInfo) ? "/" + item.ReferenceInfo : "";
                                };

                                DataModel.CountItem = dataList.ListData.Count();
                                DataModel.SumQuantityLine = dataList.ListData.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = dataList.ListData.Sum(q => q.Quantity_Scan);

                                ListStockLine = new ObservableCollection<StockCountLineModel>(dataList.ListData.OrderBy(s => s.ItemNo));
                            }
                            else
                            {
                                DataModel.CountItem = 0;
                                DataModel.SumQuantityLine = 0;
                                DataModel.SumQuantity_Scan = 0;

                                ListStockLine = new ObservableCollection<StockCountLineModel>();
                            }
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is empty." };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Hàm check kiểm kê
        /// </summary>
        /// <param name="itemNo"></param>
        public async void CheckStockLine(string barcodeNo)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (DataModel.Release == 0)
                {
                    var tmpCurrentItem = new StockCountLineModel();

                    if (barcodeNo.Length == 6)
                    {
                        tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == barcodeNo).FirstOrDefault();
                    }
                    else
                    {
                        tmpCurrentItem = ListStockLine.Where(q => q.BarcodeNo == barcodeNo).FirstOrDefault();
                    }

                    if (tmpCurrentItem != null)
                    {
                        tmpCurrentItem.Quantity_Scan = tmpCurrentItem.Quantity_Scan + 1;

                        if (tmpCurrentItem.Quantity_Scan == tmpCurrentItem.Quantity)
                        {
                            tmpCurrentItem.ComparePCS = false;
                        }
                        else if (tmpCurrentItem.Quantity_Scan > tmpCurrentItem.Quantity)
                        {
                            tmpCurrentItem.ComparePCS = true;
                        }

                        var currentData = new LSRetail_StockCountDetailModel()
                        {
                            DocumentNo = tmpCurrentItem.DocumentNo,
                            Zone = 0,
                            FixelID = tmpCurrentItem.FixID,
                            POG = tmpCurrentItem.POG,
                            BarcodeNo = tmpCurrentItem.BarcodeNo,
                            ID = tmpCurrentItem.ID,
                            DateCreate = tmpCurrentItem.DateCreate,
                            ItemNo = tmpCurrentItem.ItemNo,
                            Quantity = tmpCurrentItem.Quantity,
                            Quantity_Scan = tmpCurrentItem.Quantity_Scan,
                            UserCreate = tmpCurrentItem.UserCreate,
                            IsDelete = 0
                        };

                        var statusCheck = await UpsertQuantityScan(currentData);

                        if (statusCheck == true)
                        {
                            try
                            {
                                PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }

                            ListStockLine.Remove(tmpCurrentItem);
                            ListStockLine.Insert(0, tmpCurrentItem);

                            DataModel.CountItem = ListStockLine.Count();
                            DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                            DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                        }
                    }
                    //Nếu chưa có item trong danh sách
                    else
                    {
                        var userstore = Application.Current.Properties["UserName"].ToString();
                        var producyModel = LoadProduct(barcodeNo);

                        var CheckItemNoAgain = ListStockLine.Where(q => q.ItemNo == producyModel.ItemNo).FirstOrDefault();

                        if (CheckItemNoAgain != null)
                        {
                            if (CheckItemNoAgain.IsHasData == "KhongCo")
                            {
                                CheckItemNoAgain.Quantity_Scan = 0;

                                CheckItemNoAgain.ComparePCS = false;
                            }
                            else
                            {
                                CheckItemNoAgain.Quantity_Scan = CheckItemNoAgain.Quantity_Scan + 1;

                                CheckItemNoAgain.ComparePCS = true;
                            }

                            CheckItemNoAgain.ColorIsScan = "#03a9f3";
                            CheckItemNoAgain.IsHasData = "Co";

                            var currentData = new LSRetail_StockCountDetailModel()
                            {
                                DocumentNo = CheckItemNoAgain.DocumentNo,
                                POG = CheckItemNoAgain.POG,
                                FixelID = CheckItemNoAgain.FixID,
                                ItemName = CheckItemNoAgain.ItemName,
                                BarcodeNo = CheckItemNoAgain.BarcodeNo,
                                ID = CheckItemNoAgain.ID,
                                DateCreate = CheckItemNoAgain.DateCreate,
                                ItemNo = CheckItemNoAgain.ItemNo,
                                Quantity = CheckItemNoAgain.Quantity,
                                Quantity_Scan = CheckItemNoAgain.Quantity_Scan,
                                UserCreate = CheckItemNoAgain.UserCreate,
                                IsDelete = 0
                            };

                            var statusCheck = await UpsertQuantityScan(currentData);

                            if (statusCheck == true)
                            {
                                try
                                {
                                    PopupNavigation.Instance.PopAsync();
                                }
                                catch (Exception) { }

                                var temp = new List<StockCountLineModel>(ListStockLine);
                                temp.Remove(CheckItemNoAgain);
                                temp.Insert(0, CheckItemNoAgain);

                                ListStockLine.Clear();
                                ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                DataModel.CountItem = ListStockLine.Count();
                                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(producyModel.BarcodeNo))
                            {
                                try
                                {
                                    PopupNavigation.Instance.PopAsync();
                                }
                                catch (Exception) { }

                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " not found in LS Retail." };
                                PopupNavigation.Instance.PushAsync(dialog, false);
                            }
                            else
                            {
                                var tmpDataModel = new StockCountLineModel
                                {
                                    DocumentNo = DataModel.DocumentNo,
                                    ItemName = producyModel.ItemName,
                                    Zone = "0",
                                    BarcodeNo = producyModel.BarcodeNo,
                                    ID = Guid.NewGuid().ToString(),
                                    DateCreate = DateTime.Now,
                                    POG = "",
                                    FixID = "",
                                    IsDelete = 0,
                                    IsHasData = "Co",
                                    ColorIsScan = "#03a9f3",
                                    ItemNo = producyModel.ItemNo,
                                    Quantity = producyModel.Stock,
                                    Quantity_Scan = 1,
                                    UserCreate = userstore
                                };

                                var temp = new List<StockCountLineModel>(ListStockLine);
                                temp.Insert(0, tmpDataModel);

                                ListStockLine.Clear();
                                ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                DataModel.CountItem = ListStockLine.Count();
                                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);


                                HttpClient client = new HttpClient();
                                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                                client.DefaultRequestHeaders.Authorization = authHeader;

                                var model = new LSRetail_StockCountDetailModel()
                                {
                                    DocumentNo = tmpDataModel.DocumentNo,
                                    Zone = 0,
                                    POG = "",
                                    FixelID = "",
                                    ItemName = tmpDataModel.ItemName,
                                    BarcodeNo = tmpDataModel.BarcodeNo,
                                    ID = tmpDataModel.ID,
                                    DateCreate = tmpDataModel.DateCreate,
                                    IsDelete = tmpDataModel.IsDelete,
                                    ItemNo = tmpDataModel.ItemNo,
                                    Quantity = tmpDataModel.Quantity,
                                    Quantity_Scan = tmpDataModel.Quantity_Scan,
                                    UserCreate = tmpDataModel.UserCreate
                                };

                                UpsertQuantityScan(model);


                            }
                        }
                    }
                }
                else
                {
                    var tmpCurrentItem = ListStockLine.FirstOrDefault(q => q.BarcodeNo == barcodeNo || q.ItemNo == barcodeNo);
                    if (tmpCurrentItem != null)
                    {
                        var temp = new List<StockCountLineModel>(ListStockLine);
                        temp.Remove(tmpCurrentItem);
                        temp.Insert(0, tmpCurrentItem);

                        ListStockLine.Clear();
                        ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                        DataModel.CountItem = ListStockLine.Count();
                        DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                        DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                    }
                }

                IsScan = false;
            }
            catch (Exception ex)
            {
                IsScan = false;

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
            finally
            {
                IsScan = false;

                ShowLoading = false;
            }
        }

        public ProductInfoStockModel LoadProduct(string KeyValue)
        {
            var listModel = new ProductInfoStockModel();

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return listModel;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/ProductInfoStock?KeyValue=" + KeyValue + "&store=" + DataModel.LocationCode, string.Empty));

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var dataList = JsonConvert.DeserializeObject<ProductInfoStockResutModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return listModel;
                    }



                    return dataList.ListData.First();
                }

                return listModel;
            }
            catch (Exception ex)
            {
                return listModel;
            }
        }

        public partial class ProductInfoStockResutModel
        {
            public bool Active { get; set; }
            public List<ProductInfoStockModel> ListData { get; set; }
        }

        public class ProductInfoStockModel
        {
            public string LocationCode;
            public string BarcodeNo;
            public string ItemNo;
            public string ItemName;
            public string POG;
            public string FixelID;
            public int Stock;
            public int QuantityPOG;
            public decimal UnitPrice;
        }

        public async Task<bool> UpsertQuantityScan(LSRetail_StockCountDetailModel obj)
        {
            try
            {
                ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=" + CheckConnectInternet.GetmethodName() + " Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/UpsertOrDeleteStockLine", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<UpsertOrDeletePOLineModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return false;
                    }

                    if (dataList.Code == 200)
                    {
                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception)
                        {

                        }

                        if (obj.Quantity == 0 && obj.IsDelete == 0)
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "ItemNo:=" + obj.ItemNo + " SOH bằng không." };
                            await PopupNavigation.Instance.PushAsync(dialog);

                        }

                        logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=" + CheckConnectInternet.GetmethodName() + "Content:=Successfully.", "Info");

                        return true;
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        PopupNavigation.Instance.PushAsync(dialog);

                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return false;
            }
        }
    }
}
