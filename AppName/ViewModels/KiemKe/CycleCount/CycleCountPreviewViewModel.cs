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
    public class CycleCountPreviewViewModel : BaseViewModel
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

        private ObservableCollection<StockCountLineModel> _listPOG;
        public ObservableCollection<StockCountLineModel> ListPOG
        {
            get { return _listPOG; }
            set { SetProperty(ref _listPOG, value); }
        }

        private ObservableCollection<StockCountLineModel> _listFixelID;
        public ObservableCollection<StockCountLineModel> ListFixelID
        {
            get { return _listFixelID; }
            set { SetProperty(ref _listFixelID, value); }
        }

        private StockCountLineModel _selectedFixelID;
        public StockCountLineModel SelectedFixelID
        {
            get { return _selectedFixelID; }
            set { SetProperty(ref _selectedFixelID, value); }
        }

        public ICommand ChangeFixelIDCommand { get; set; }

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

        private ObservableCollection<StockCountLineModel> _listPOGFull;
        public ObservableCollection<StockCountLineModel> ListPOGFull
        {
            get { return _listPOGFull; }
            set { SetProperty(ref _listPOGFull, value); }
        }

        private StockCountLineModel _selectedPOG;
        public StockCountLineModel SelectedPOG
        {
            get { return _selectedPOG; }
            set { SetProperty(ref _selectedPOG, value); }
        }

        public Command<object> ChangePOGCommand { get; set; }
        public Command<object> ChangePOGCommandStaff { get; set; }

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        private ObservableCollection<StockCountLineModel> _listStockLineFull;
        public ObservableCollection<StockCountLineModel> ListStockLineFull
        {
            get { return _listStockLineFull; }
            set { SetProperty(ref _listStockLineFull, value); }
        }
        public Command<string> SearchItemsCommand { get; set; }

        public ICommand ReleaseCommand { get; set; }
        public ICommand EditCycleCountLineCommand { get; set; }
        public Command LoadItemsCommand { get; set; }

        public CycleCountPreviewViewModel(INavigation navigation, StockCountModel model)
        {
            ReleaseCommand = new Command(ReleaseAsync);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);

            Navigation = navigation;
            IsScan = false;
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

            EditCycleCountLineCommand = new Command<StockCountLineModel>(EditCycleCountLineAsync);
            ChangeFixelIDCommand = new Command(ChangFixelIDAsync);
            ChangePOGCommand = new Command<object>(ChangePOGAsync);
        }

        public async void ChangFixelIDAsync()
        {
            try
            {
                if (SelectedFixelID != null && !string.IsNullOrEmpty(SelectedFixelID.FixID))
                {
                    IsEnablePOG = true;

                    var POGList = ListPOGFull.Where(s => s.FixID == SelectedFixelID.FixID).OrderBy(s => s.FixID).ToList();
                    ListPOG = new ObservableCollection<StockCountLineModel>(POGList);
                }
                else
                {
                    IsEnablePOG = false;
                    ListPOG = new ObservableCollection<StockCountLineModel>();
                }

                ListStockLine = new ObservableCollection<StockCountLineModel>();
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


        public async void ChangePOGAsync(object obj)
        {
            try
            {
                var calendar = (Picker)obj;
                calendar.Unfocus();

                if (IsBusy)
                    return;

                IsBusy = true;

                if (SelectedPOG != null && !string.IsNullOrEmpty(SelectedPOG.POG))
                {
                    var listInPOG = new List<StockCountLineModel>();

                    if (SelectedPOG.POG == "Empty" && SelectedFixelID.FixID == "Empty")
                    {
                        listInPOG = ListStockLineFull.Where(s => s.POG == "" && s.FixID == "").ToList();
                    }
                    else
                    {
                        listInPOG = ListStockLineFull.Where(s => s.POG == SelectedPOG.POG && s.FixID == SelectedFixelID.FixID).ToList();
                    }

                    var selectItem = listInPOG.Select(s => s.ItemNo).ToList();

                    ListStockLine = new ObservableCollection<StockCountLineModel>(listInPOG);
                }
                else
                {
                    ListStockLine = ListStockLineFull;
                }

                DataModel.CountItem = ListStockLine.Count();
                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
            }
            catch (Exception ex)
            {
                IsBusy = false;

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void EditCycleCountLineAsync(StockCountLineModel obj)
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

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetCycleCountLine?masterID=" + documentNo);

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
                                ListStockLineFull = new ObservableCollection<StockCountLineModel>(dataList.ListData.OrderBy(s => s.ItemNo));

                                var POGList = dataList.ListData.GroupBy(s => new { s.POG, s.FixID }).Select(d => d.FirstOrDefault()).OrderBy(s => s.POG).ToList();

                                var FixIDList = dataList.ListData.Where(s => s.FixID != "").GroupBy(s => s.FixID).Select(d => d.FirstOrDefault()).OrderBy(s => s.FixID).ToList();

                                var all = new StockCountLineModel()
                                {
                                    POG = "Empty",
                                    FixID = "Empty"
                                };

                                POGList.Insert(0, all);

                                var allFixelID = new StockCountLineModel()
                                {
                                    POG = "Empty",
                                    FixID = "Empty"
                                };

                                FixIDList.Insert(0, allFixelID);

                                ListPOG = new ObservableCollection<StockCountLineModel>(POGList);

                                ListFixelID = new ObservableCollection<StockCountLineModel>(FixIDList);

                                ListPOGFull = new ObservableCollection<StockCountLineModel>(POGList);

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

                if (DataModel.Release == 0 && DataModel.NoLine == "1")
                {
                    //var tmpCurrentItem = ListStockLine.FirstOrDefault(q => (q.BarcodeNo == barcodeNo || q.ItemNo == barcodeNo)
                    //&& q.FixID == (SelectedFixelID.FixID == "Empty" ? "" : SelectedFixelID.FixID)
                    //&& q.POG == (SelectedPOG.POG == "Empty" ? "" : SelectedPOG.POG));

                    var tmpCurrentItem = new StockCountLineModel();

                    if (barcodeNo.Length == 6)
                    {
                        tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == barcodeNo
                                        && q.FixID == (SelectedFixelID.FixID == "Empty" ? "" : SelectedFixelID.FixID)
                                        && q.POG == (SelectedPOG.POG == "Empty" ? "" : SelectedPOG.POG)).FirstOrDefault();
                    }
                    else
                    {
                        var getItemNo = ListStockLine.Where(q => q.BarcodeNo == barcodeNo
                                        && q.FixID == (SelectedFixelID.FixID == "Empty" ? "" : SelectedFixelID.FixID)
                                        && q.POG == (SelectedPOG.POG == "Empty" ? "" : SelectedPOG.POG)).FirstOrDefault();

                        if (getItemNo != null)
                        {
                            tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == getItemNo.ItemNo
                                        && q.FixID == (SelectedFixelID.FixID == "Empty" ? "" : SelectedFixelID.FixID)
                                        && q.POG == (SelectedPOG.POG == "Empty" ? "" : SelectedPOG.POG)).FirstOrDefault();
                        }
                        else
                        {
                            tmpCurrentItem = null;
                        }
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

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " added." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        try
                        {
                            PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " not found in " + DataModel.DocumentNo + "." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
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
                    else
                    {
                        try
                        {
                            PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Not found :=" + barcodeNo + "." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
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

                        return true;

                        logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=" + CheckConnectInternet.GetmethodName() + "Content:=Successfully.", "Info");
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
