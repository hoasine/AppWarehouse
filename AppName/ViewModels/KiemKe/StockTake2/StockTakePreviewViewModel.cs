using AppName.CustomRenderer;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class StockTakePreviewViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _IsScan;
        public bool IsScan
        {
            get { return _IsScan; }
            set { SetProperty(ref _IsScan, value); }
        }

        private string _Release;
        public string Release
        {
            get { return _Release; }
            set { SetProperty(ref _Release, value); }
        }

        private string _DataLocal;
        public string DataLocal
        {
            get { return _DataLocal; }
            set { SetProperty(ref _DataLocal, value); }
        }

        private string _tesstValue;
        public string tesstValue
        {
            get { return _tesstValue; }
            set { SetProperty(ref _tesstValue, value); }
        }

        private string _itemno;
        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
        }

        private bool _IsEnablePOG;
        public bool IsEnablePOG
        {
            get { return _IsEnablePOG; }
            set { SetProperty(ref _IsEnablePOG, value); }
        }

        private ObservableCollection<POGModel> _listPOGFull;
        public ObservableCollection<POGModel> ListPOGFull
        {
            get { return _listPOGFull; }
            set { SetProperty(ref _listPOGFull, value); }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set
            {
                _showLoading = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AreasModel> _listZone;
        public ObservableCollection<AreasModel> ListZone
        {
            get { return _listZone; }
            set { SetProperty(ref _listZone, value); }
        }

        private ObservableCollection<AreasModel> _ListZoneTam;
        public ObservableCollection<AreasModel> ListZoneTam
        {
            get { return _ListZoneTam; }
            set { SetProperty(ref _ListZoneTam, value); }
        }

        private AreasModel _selectedZone;
        public AreasModel SelectedZone
        {
            get { return _selectedZone; }
            set { SetProperty(ref _selectedZone, value); }
        }

        public Command<object> ChangeZoneCommand { get; set; }

        private bool _IsStaff;
        public bool IsStaff
        {
            get { return _IsStaff; }
            set { SetProperty(ref _IsStaff, value); }
        }

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        public ICommand ShowAlternatelyPersonViewCommand { get; set; }


        private ObservableCollection<StockCountLineModel> _ListStockLineSelect;
        public ObservableCollection<StockCountLineModel> ListStockLineSelect
        {
            get { return _ListStockLineSelect; }
            set { SetProperty(ref _ListStockLineSelect, value); }
        }
        public ICommand ReleaseCommand { get; set; }
        public ICommand EditStockTakeLineCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public ICommand DeleteStockTakeLineCommand { get; set; }
        public ICommand SyncDataLocalCommand { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand SearchAlternatelyCommand { get; set; }

        public class POGModel
        {
            public string Store { get; set; }
            public string FixelID { get; set; }
            public string POG { get; set; }
            public int Quantity { get; set; }
        }

        private bool _visibleAlternately;
        public bool VisibleAlternately
        {
            get { return _visibleAlternately; }
            set { SetProperty(ref _visibleAlternately, value); }
        }

        private ObservableCollection<AreasModel> _listAlternately;
        public ObservableCollection<AreasModel> ListAlternately
        {
            get { return _listAlternately; }
            set { SetProperty(ref _listAlternately, value); }
        }

        private async void ShowUserMulti()
        {
            try
            {
                VisibleAlternately = true;
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void SearchAlternatelyAsync(string Filter)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                ListZone = new ObservableCollection<AreasModel>(ListZoneTam.OrderBy(o => o.AreasName).OrderByDescending(r => r.AreasName));
            }
            else
            {
                ListZone = new ObservableCollection<AreasModel>(ListZoneTam.Where(c => c.AreasName.ToString().ToLower().
                                   Contains(Filter.ToLower())).OrderByDescending(r => r.AreasName));
            }
        }

        public StockTakePreviewViewModel(INavigation navigation, StockCountModel model)
        {
            //DependencyService.Get<IDevice>().ShowAlert("Only Retail Staff can release document. You d Only Retail Staff can release document. ");

            AutoSyncData();

            ReleaseCommand = new Command(ReleaseAsync);

            IsScan = false;

            ShowAlternatelyPersonViewCommand = new Command(ShowUserMulti);
            HideAlternatelyPersonCommand = new Command(HideUsername);

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

            ListZone = new ObservableCollection<AreasModel>();

            EditStockTakeLineCommand = new Command<StockCountLineModel>(EditStockTakeLineAsync);
            DeleteStockTakeLineCommand = new Command<StockCountLineModel>(DeletStockTakeLineAsync);

            ChangeZoneCommand = new Command<object>(ChangeAreasAsync);
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);

            SyncDataLocalCommand = new Command(SyncData);

            LoadItemsCommand = new Command(async () => LoadAreas());
        }

        private async void HideUsername()
        {
            VisibleAlternately = false;
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

        public async void DeletStockTakeLineAsync(StockCountLineModel obj)
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
                    Zone = SelectedZone.Code,
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

                var check = InsertDataBangTam(currentData);
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

                    try
                    {
                        PopupNavigation.Instance.PopAsync();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void EditStockTakeLineAsync(StockCountLineModel obj)
        {
            if (DataModel.Release != 0)
            {
                var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                PopupNavigation.Instance.PushAsync(dialog3);
                return;
            }

            try
            {
                var quantityBanDau = obj.Quantity_Scan;

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
                            Zone = SelectedZone.Code,
                            BarcodeNo = obj.BarcodeNo,
                            ID = obj.ID,
                            DateCreate = obj.DateCreate,
                            ItemNo = obj.ItemNo,
                            Quantity = obj.Quantity,
                            Quantity_Scan = data.Quantity_Scan,
                            UserCreate = obj.UserCreate,
                            IsDelete = 0,
                            POG = "",
                            FixelID = "",
                        };

                        var check = InsertDataBangTam(currentData);
                        if (check == true)
                        {
                            PopupNavigation.Instance.PopAsync();

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
                        else
                        {
                            obj.Quantity_Scan = quantityBanDau;
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
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void ChangeAreasAsync(object obj)
        {
            var calendar = (Picker)obj;
            calendar.Unfocus();

            LoadDataLines(DataModel.DocumentNo, SelectedZone.Code);
        }

        private async void ReleaseAsync(object obj)
        {

            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                var checkRelase = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ChangeStatusStockTakeReOpen"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                if (checkRelase == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission change status stock.", "OK");
                    return;
                }

                var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Only Retail Staff can release document. You do not have permission for this function. With " + DataModel.DocumentNo + "." };
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

                var response = await client.GetAsync(uri);

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
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Stock take lines no data to report " + DataModel.DocumentNo + "." };
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

        private async Task LoadAreas()
        {
            try
            {
                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetAreas?document=" + DataModel.AreMask);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<List<AreasModel>>(content).OrderByDescending(r => r.AreasName).ToList();

                    ListZone = new ObservableCollection<AreasModel>(dataList);
                    ListZoneTam = new ObservableCollection<AreasModel>(dataList);
                }
                else
                {
                    ListZone = new ObservableCollection<AreasModel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public class AreasModel
        {
            public string Code { get; set; }
            public string AreasName { get; set; }
        }

        public class AreDetailModel
        {
            public string AreaName { get; set; }
            public string Code { get; set; }
        }

        public ICommand HideAlternatelyPersonCommand { get; set; }


        public async void LoadDataLines(string documentNo, string Zone)
        {
                try
                {
                    ShowLoading = true;

                    await Task.Delay(1);

                    if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                    {
                        return;
                    }

                    HttpClient client = new HttpClient();
                    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    var userstore = Application.Current.Properties["UserStore"].ToString();

                    Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetStockTakeLine?masterID=" + documentNo + "&zone=" + Zone);

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
                                        if (item.IsHasData == "Co")
                                        {
                                            item.ColorIsScan = "#03a9f3";
                                        }
                                        else
                                        {
                                            item.ColorIsScan = "#555555";
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
                    ShowLoading = false;
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

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception) { }

                if (SelectedZone == null)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select a POG." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                if (DataModel.Release == 0)
                {
                    var tmpCurrentItem = new StockCountLineModel();

                    if (barcodeNo.Length == 6)
                    {
                        tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == barcodeNo && q.Zone == SelectedZone.Code).FirstOrDefault();
                    }
                    else
                    {
                        var getItemNo = ListStockLine.Where(q => q.BarcodeNo == barcodeNo && q.Zone == SelectedZone.Code).FirstOrDefault();

                        if (getItemNo != null)
                        {
                            tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == getItemNo.ItemNo && q.Zone == SelectedZone.Code).FirstOrDefault();
                        }
                        else
                        {
                            tmpCurrentItem = null;
                        }
                    }

                    if (tmpCurrentItem != null)
                    {
                        var quantityBanDau = tmpCurrentItem.Quantity_Scan;

                        var dialog = new StockTakeLineDetail();
                        var viewModel = new StockTakeLineDetailViewModel(tmpCurrentItem);
                        viewModel.IsUpdate = true;
                        viewModel.ClosePopup += async (send, data) =>
                        {
                            try
                            {
                                viewModel.ShowLoading = true;
                                viewModel.IsEnabledButton = false;

                                await Task.Delay(100);

                                var currentData = new LSRetail_StockCountDetailModel()
                                {
                                    DocumentNo = tmpCurrentItem.DocumentNo,
                                    Zone = SelectedZone.Code,
                                    BarcodeNo = tmpCurrentItem.BarcodeNo,
                                    ID = tmpCurrentItem.ID,
                                    DateCreate = DateTime.Now,
                                    ItemName = tmpCurrentItem.ItemName,
                                    ItemNo = tmpCurrentItem.ItemNo,
                                    Quantity = tmpCurrentItem.Quantity,
                                    Quantity_Scan = tmpCurrentItem.Quantity_Scan,
                                    UserCreate = tmpCurrentItem.UserCreate,
                                    IsDelete = 0,
                                    POG = "",
                                    FixelID = "",
                                };

                                var check = InsertDataBangTam(currentData);
                                if (check == true)
                                {
                                    var temp = new List<StockCountLineModel>(ListStockLine);
                                    temp.Remove(tmpCurrentItem);
                                    temp.Insert(0, data);

                                    ListStockLine.Clear();
                                    ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                    DataModel.CountItem = ListStockLine.Count();
                                    DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                    DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                                    DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
                                }
                                else
                                {
                                    tmpCurrentItem.Quantity_Scan = quantityBanDau;
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
                                viewModel.IsEnabledButton = true;
                                viewModel.ShowLoading = false;
                            }
                        };

                        dialog.BindingContext = viewModel;

                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        var userstore = Application.Current.Properties["UserStore"].ToString();
                        var producyModel = await LoadProduct(barcodeNo);

                        var CheckItemNoAgain = ListStockLine.Where(q => q.ItemNo == producyModel.ItemNo && q.Zone == SelectedZone.Code).FirstOrDefault();

                        if (CheckItemNoAgain != null)
                        {
                            var quantityBanDau = CheckItemNoAgain.Quantity_Scan;

                            var dialog = new StockTakeLineDetail();
                            var viewModel = new StockTakeLineDetailViewModel(CheckItemNoAgain);
                            viewModel.IsUpdate = true;
                            viewModel.ClosePopup += async (send, data) =>
                            {
                                try
                                {
                                    viewModel.ShowLoading = true;
                                    viewModel.IsEnabledButton = false;

                                    await Task.Delay(100);

                                    var currentData = new LSRetail_StockCountDetailModel()
                                    {
                                        DocumentNo = CheckItemNoAgain.DocumentNo,
                                        Zone = SelectedZone.Code,
                                        BarcodeNo = CheckItemNoAgain.BarcodeNo,
                                        ID = CheckItemNoAgain.ID,
                                        DateCreate = DateTime.Now,
                                        ItemName = CheckItemNoAgain.ItemName,
                                        ItemNo = CheckItemNoAgain.ItemNo,
                                        Quantity = CheckItemNoAgain.Quantity,
                                        Quantity_Scan = CheckItemNoAgain.Quantity_Scan,
                                        UserCreate = CheckItemNoAgain.UserCreate,
                                        IsDelete = 0,
                                        POG = "",
                                        FixelID = "",
                                    };

                                    var check = InsertDataBangTam(currentData);
                                    if (check == true)
                                    {
                                        var temp = new List<StockCountLineModel>(ListStockLine);
                                        temp.Remove(CheckItemNoAgain);
                                        temp.Insert(0, data);

                                        ListStockLine.Clear();
                                        ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                        DataModel.CountItem = ListStockLine.Count();
                                        DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                        DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                                        DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
                                    }
                                    else
                                    {
                                        tmpCurrentItem.Quantity_Scan = quantityBanDau;
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
                                    viewModel.IsEnabledButton = true;
                                    viewModel.ShowLoading = false;
                                }
                            };

                            dialog.BindingContext = viewModel;

                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else
                        {
                            try
                            {
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }

                            if (string.IsNullOrEmpty(producyModel.BarcodeNo))
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " not found product." };
                                PopupNavigation.Instance.PushAsync(dialog, false);
                            }
                            else
                            {
                                var tmpDataModel = new StockCountLineModel
                                {
                                    DocumentNo = DataModel.DocumentNo,
                                    ItemName = producyModel.ItemName,
                                    Zone = SelectedZone.Code,
                                    BarcodeNo = producyModel.BarcodeNo,
                                    ID = Guid.NewGuid().ToString(),
                                    DateCreate = DateTime.Now,
                                    POG = "",
                                    FixID = "",
                                    IsDelete = 0,
                                    ItemNo = producyModel.ItemNo,
                                    Quantity = producyModel.Stock,
                                    Quantity_Scan = 0,
                                    UserCreate = userstore
                                };

                                var dialog = new StockTakeLineDetail();
                                var viewModel = new StockTakeLineDetailViewModel(tmpDataModel);
                                viewModel.IsUpdate = true;
                                viewModel.ClosePopup += async (send, data) =>
                                {
                                    await PopupNavigation.Instance.PopAsync();

                                    try
                                    {
                                        viewModel.IsEnabledButton = false;
                                        viewModel.ShowLoading = true;

                                        await Task.Delay(100);

                                        var model = new LSRetail_StockCountMasterModel();
                                        model.DocumentNo = DataModel.DocumentNo;
                                        model.Description = DataModel.Desciption;
                                        model.IsDelete = 3;
                                        model.Lines = new List<LSRetail_StockCountDetailModel>()
                                    {
                                        new LSRetail_StockCountDetailModel()
                                        {
                                            DocumentNo = tmpDataModel.DocumentNo,
                                            Zone = SelectedZone.Code,
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
                                            UserCreate = tmpDataModel.UserCreate,
                                        }
                                    };

                                        var check = InsertDataBangTam(model.Lines.FirstOrDefault());

                                        if (check == true)
                                        {
                                            var temp = new List<StockCountLineModel>(ListStockLine);
                                            temp.Insert(0, tmpDataModel);

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
                                        viewModel.IsEnabledButton = true;
                                        viewModel.ShowLoading = false;
                                    }
                                };

                                dialog.BindingContext = viewModel;
                                await PopupNavigation.Instance.PushAsync(dialog);
                            }
                        }
                    }
                }
                else
                {
                    var tmpCurrentItem = new StockCountLineModel();

                    if (!string.IsNullOrEmpty(barcodeNo))
                    {
                        if (barcodeNo.Length == 6)
                        {
                            tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == barcodeNo && q.Zone == SelectedZone.Code).FirstOrDefault();
                        }
                        else
                        {
                            var getItemNo = ListStockLine.Where(q => q.BarcodeNo == barcodeNo && q.Zone == SelectedZone.Code).FirstOrDefault();

                            if (getItemNo != null)
                            {
                                tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == getItemNo.ItemNo && q.Zone == SelectedZone.Code).FirstOrDefault();
                            }
                        }

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
                        else
                        {
                            var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "No products found. DocumentNo has been released so it cannot be added." };
                            PopupNavigation.Instance.PushAsync(dialog3);
                            return;
                        }
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
                ShowLoading = false;
                IsScan = false;
            }
        }

        public async Task<ProductInfoStockModel> LoadProduct(string KeyValue)
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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/ProductInfoStockTake?KeyValue=" + KeyValue + "&store=" + DataModel.LocationCode + "&DocumentNo=" + DataModel.DocumentNo, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

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

        public bool InsertDataBangTam(LSRetail_StockCountDetailModel obj)
        {
            try
            {
                ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=Upsert Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

                var reaml = RealmHelper.Instance;

                using (var transaction = reaml.BeginWrite())
                {
                    var upModel = new StockTakeLocalModel()
                    {
                        IDItem = obj.ID,
                        Zone = obj.Zone,
                        QuantityPOG = obj.QuantityPOG,
                        FixelID = obj.FixelID,
                        POG = obj.POG,
                        DocumentRoot = DataModel.DocumentRoot,
                        DocumentNo = obj.DocumentNo,
                        BarcodeNo = obj.BarcodeNo,
                        Image = obj.Image,
                        ItemName = obj.ItemName,
                        ItemNo = obj.ItemNo,
                        DateCreate = obj.DateCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                        Quantity = obj.Quantity,
                        Quantity_Scan = obj.Quantity_Scan,
                        IsDelete = obj.IsDelete,
                        UserCreate = obj.UserCreate,
                        IsSync = false
                    };

                    reaml.Add(upModel, true);

                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return false;
            }
        }


        public Boolean _running = true;

        public void AutoSyncData()
        {
            try
            {
                var t = new Thread(() =>
                {
                    while (_running)
                    {
                        //await Task.Delay(60000); //60 seconds

                        SyncData();

                        Thread.Sleep(5000);
                    }
                });

                t.Start();
            }
            catch (Exception ex)
            {

            }
        }

        public void SyncData()
        {
            var reaml = RealmHelper.Instance;

            var getData = reaml.All<StockTakeLocalModel>().Where(s => s.DocumentNo == DataModel.DocumentNo).ToList();

            DataLocal = getData.Count().ToString();

            if (getData.Count() == 0)
            {
                //Không làm gì nếu k có dữ liệu mới
            }
            else
            {
                var listResuft = new List<LSRetail_StockCountDetailModel>();

                foreach (var obj in getData)
                {
                    var model = new LSRetail_StockCountDetailModel();
                    model.ID = obj.IDItem;
                    model.Zone = obj.Zone;
                    model.QuantityPOG = obj.QuantityPOG;
                    model.FixelID = obj.FixelID;
                    model.POG = obj.POG;
                    model.DocumentNo = obj.DocumentNo;
                    model.BarcodeNo = obj.BarcodeNo;
                    model.Image = obj.Image;
                    model.ItemName = obj.ItemName;
                    model.ItemNo = obj.ItemNo;
                    model.DateCreate = DateTime.Parse(obj.DateCreate);
                    model.Quantity = obj.Quantity;
                    model.Quantity_Scan = obj.Quantity_Scan;
                    model.IsDelete = obj.IsDelete;
                    model.UserCreate = obj.UserCreate;

                    var resuft = UpsertQuantityScan(model);

                    if (resuft == false)
                    {
                        ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                        logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=Upsert Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

                        //DependencyService.Get<IDevice>().ShowAlert();
                    }
                    else
                    {
                        using (var transaction = reaml.BeginWrite())
                        {
                            var updateOItem = RealmHelper.GetItemStockTake(model.ID);

                            reaml.Remove(updateOItem);

                            transaction.Commit();
                        }
                    }
                }
            }
        }

        public bool UpsertQuantityScan(LSRetail_StockCountDetailModel obj)
        {
            try
            {
                ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=Upsert Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

                //var check = CheckConnectInternet.IsConnectedNotClearCookie();
                //if (check == false)
                //{
                //    return false;
                //}

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/UpsertOrDeleteStockLine", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

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
                        //try
                        //{
                        //    PopupNavigation.Instance.PopAsync();
                        //}
                        //catch (Exception) { }

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

        public partial class LSRetail_StockCountMaster_ResuftModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public LSRetail_StockCountMasterModel ListData { get; set; }
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

        public partial class LSRetail_StockCountMasterModel
        {
            public string ID;
            public string Description;
            public string DocumentNo;
            public string StockType;
            public string LocationCode;
            public string Status;
            public string Management;
            public int IsSend;
            public int Release;
            public int IsDelete;
            public int QtyPack;
            public DateTime DateCreate;
            public string UserCreate;
            public List<LSRetail_StockCountDetailModel> Lines;
        }

        public partial class LSRetail_StockCountDetailModel
        {
            public string ID;
            public string Zone;
            public int QuantityPOG;
            public string FixelID;
            public string POG;
            public string DocumentNo;
            public string BarcodeNo;
            public string Image;
            public string ItemName;
            public string ItemNo;
            public DateTime DateCreate;
            public int Quantity;
            public int Quantity_Scan;
            public int IsDelete;
            public string UserCreate;
        }
    }
}
