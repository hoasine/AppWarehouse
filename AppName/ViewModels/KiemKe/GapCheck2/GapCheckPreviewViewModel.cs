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
    public class GapCheckPreviewViewModel : BaseViewModel
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

        private ObservableCollection<POGModel> _listFixelID;
        public ObservableCollection<POGModel> ListFixelID
        {
            get { return _listFixelID; }
            set { SetProperty(ref _listFixelID, value); }
        }

        private POGModel _selectedFixelID;
        public POGModel SelectedFixelID
        {
            get { return _selectedFixelID; }
            set { SetProperty(ref _selectedFixelID, value); }
        }

        public ICommand ChangeFixelIDCommand { get; set; }

        private ObservableCollection<POGModel> _listPOG;
        public ObservableCollection<POGModel> ListPOG
        {
            get { return _listPOG; }
            set { SetProperty(ref _listPOG, value); }
        }

        private POGModel _selectedPOG;
        public POGModel SelectedPOG
        {
            get { return _selectedPOG; }
            set { SetProperty(ref _selectedPOG, value); }
        }

        public Command<object> ChangePOGCommand { get; set; }

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

        private ObservableCollection<StockCountLineModel> _listStockLineFull;
        public ObservableCollection<StockCountLineModel> ListStockLineFull
        {
            get { return _listStockLineFull; }
            set { SetProperty(ref _listStockLineFull, value); }
        }
        public ICommand ReleaseCommand { get; set; }
        public ICommand EditGapCheckLineCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }
        public ICommand DeleteGapCheckLineCommand { get; set; }
        public Command LoadItemsCommand { get; set; }

        public class POGModel
        {
            public string Store { get; set; }
            public string FixelID { get; set; }
            public string POG { get; set; }
            public int Quantity { get; set; }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public ObservableCollection<PartnersGrouping<string, StockCountLineModel>> PartnersGrouped { get; set; }


        public GapCheckPreviewViewModel(INavigation navigation, StockCountModel model)
        {
            ReleaseCommand = new Command(ReleaseAsync);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

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

            EditGapCheckLineCommand = new Command<StockCountLineModel>(EditGapCheckLineAsync);
            DeleteGapCheckLineCommand = new Command<StockCountLineModel>(DeletGapCheckLineAsync);
            PartnersGrouped = new ObservableCollection<PartnersGrouping<string, StockCountLineModel>>();
            ChangeFixelIDCommand = new Command(ChangFixelIDAsync);
            ChangePOGCommand = new Command<object>(ChangePOGAsync);
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);
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
            try
            {
                ShowLoading = true;

                await Task.Delay(10);

                await LoadPOG();

                await LoadDataLines(DataModel.DocumentNo);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog);
            }
            finally
            {
                ShowLoading = false;
            }
        }

        public async void DeletGapCheckLineAsync(StockCountLineModel obj)
        {
            try
            {
                if (DataModel.Release != 0)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

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
                    Quantity_Scan = 0,
                    UserCreate = obj.UserCreate,
                    IsDelete = 1
                };

                var check = await UpsertQuantityScan(currentData);
                if (check == true)
                {
                    obj.Quantity_Scan = 0;

                    obj.ColorIsScan = "#555555";
                    obj.IsHasData = "KhongCo";

                    //Upsert ListLine
                    var temp = new List<StockCountLineModel>(ListStockLine);
                    temp.Remove(obj);
                    temp.Insert(0, obj);

                    ListStockLine.Clear();
                    ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                    //Upsert Group
                    var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == obj.BarcodeNo).OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();
                    foreach (var item in sortedPartners)
                    {
                        var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                        var modelRemove = PartnersGrouped.Where(s => s.Key == obj.BarcodeNo).FirstOrDefault();

                        PartnersGrouped.Remove(modelRemove);
                        PartnersGrouped.Insert(0, modelUp);
                    }

                    //Upsert Full
                    ListStockLineFull.Remove(obj);
                    ListStockLineFull.Insert(0, obj);

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

        public async void EditGapCheckLineAsync(StockCountLineModel obj)
        {
            try
            {
                if (DataModel.Release != 0)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                var dialog = new GapCheckLineDetail();
                var viewModel = new StockTakeLineDetailViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        viewModel.ShowLoading = true;

                        await Task.Delay(1);

                        data.ColorIsScan = "#03a9f3";
                        data.IsHasData = "Co";

                        var currentData = new LSRetail_StockCountDetailModel()
                        {
                            DocumentNo = obj.DocumentNo,
                            Zone = 0,
                            BarcodeNo = obj.BarcodeNo,
                            ID = obj.ID,
                            POG = obj.POG,
                            FixelID = obj.FixID,
                            QuantityPOG = obj.QuantityPOG,
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

                            #region reload group

                            var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == obj.BarcodeNo).OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();
                            foreach (var item in sortedPartners)
                            {
                                var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                                var modelRemove = PartnersGrouped.Where(s => s.Key == obj.BarcodeNo).FirstOrDefault();

                                PartnersGrouped.Remove(modelRemove);
                                PartnersGrouped.Insert(0, modelUp);
                            }

                            #endregion

                            ListStockLineFull.Remove(obj);
                            ListStockLineFull.Insert(0, data);

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
            }
        }

        public async void ChangFixelIDAsync()
        {
            try
            {
                ItemNo = "";

                if (SelectedFixelID != null && !string.IsNullOrEmpty(SelectedFixelID.FixelID))
                {
                    IsEnablePOG = true;
                    var POGList = ListPOGFull.Where(s => s.FixelID == SelectedFixelID.FixelID).OrderBy(s => s.FixelID).ToList();
                    ListPOG = new ObservableCollection<POGModel>(POGList);
                }
                else
                {
                    IsEnablePOG = false;
                    ListPOG = new ObservableCollection<POGModel>();
                }

                PartnersGrouped.Clear();
                ListStockLine = new ObservableCollection<StockCountLineModel>();

                DataModel.CountItem = ListStockLine.Count();
                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
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
                ShowLoading = true;

                await Task.Delay(1);

                ItemNo = "";

                var calendar = (Picker)obj;
                calendar.Unfocus();

                if (SelectedPOG != null && !string.IsNullOrEmpty(SelectedPOG.POG))
                {
                    var listInPOG = ListStockLineFull.Where(s => s.POG == SelectedPOG.POG && s.FixID == SelectedFixelID.FixelID).ToList();

                    var selectItem = listInPOG.Select(s => s.ItemNo).ToList();

                    PartnersGrouped.Clear();
                    var sortedPartners = listInPOG.OrderByDescending(s => s.Quantity_Scan).GroupBy(y => y.BarcodeNo).ToList();
                    foreach (var item in sortedPartners)
                    {
                        var listItem = listInPOG.Where(x => x.BarcodeNo == item.Key);

                        PartnersGrouped.Add(new PartnersGrouping<string, StockCountLineModel>(item.Key, listItem));
                    }

                    ListStockLine = new ObservableCollection<StockCountLineModel>(listInPOG);
                }
                else
                {
                    ListStockLine = ListStockLineFull;
                }

                DataModel.CountItem = ListStockLine.Count();
                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
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

        private async void ReleaseAsync(object obj)
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;


                var checkRelase = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ChangeStatusGapCheckReOpen"
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
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Gap Check lines no data to report " + DataModel.DocumentNo + "." };
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

        private async Task LoadPOG()
        {
            try
            {
                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetPOGWithStore?store=" + DataModel.LocationCode);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<List<POGModel>>(content);

                    var groupByFixelID = dataList.GroupBy(s => s.FixelID).Select(s => s.FirstOrDefault()).OrderBy(s => s.FixelID);
                    ListFixelID = new ObservableCollection<POGModel>(groupByFixelID);

                    ListPOGFull = new ObservableCollection<POGModel>(dataList);
                }
                else
                {
                    ListPOG = new ObservableCollection<POGModel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadDataLines(string documentNo)
        {
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

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetGapCheckLine?masterID=" + documentNo);

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


                                ListStockLine = new ObservableCollection<StockCountLineModel>();
                                ListStockLineFull = new ObservableCollection<StockCountLineModel>(dataList.ListData.OrderBy(s => s.ItemNo));
                            }
                            else
                            {
                                DataModel.CountItem = 0;
                                DataModel.SumQuantityLine = 0;
                                DataModel.SumQuantity_Scan = 0;

                                ListStockLine = new ObservableCollection<StockCountLineModel>();
                                ListStockLineFull = new ObservableCollection<StockCountLineModel>();
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
                else
                {
                    DataModel.CountItem = 0;
                    DataModel.SumQuantityLine = 0;
                    DataModel.SumQuantity_Scan = 0;

                    ListStockLine = new ObservableCollection<StockCountLineModel>();
                    ListStockLineFull = new ObservableCollection<StockCountLineModel>();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
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

                if (SelectedPOG == null)
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select a POG." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedPOG.POG))
                {
                    var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please select a POG." };
                    PopupNavigation.Instance.PushAsync(dialog3);
                    return;
                }

                //var tmpCurrentItem = ListStockLine.FirstOrDefault(q => (q.BarcodeNo == barcodeNo || q.ItemNo == barcodeNo) && q.POG == SelectedPOG.POG && q.FixID == SelectedFixelID.FixelID);

                var tmpCurrentItem = new StockCountLineModel();

                if (barcodeNo.Length == 6)
                {
                    tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == barcodeNo && q.POG == SelectedPOG.POG && q.FixID == SelectedFixelID.FixelID).FirstOrDefault();
                }
                else
                {
                    var getItemNo = ListStockLine.Where(q => q.BarcodeNo == barcodeNo && q.POG == SelectedPOG.POG && q.FixID == SelectedFixelID.FixelID).FirstOrDefault();

                    if (getItemNo != null)
                    {
                        tmpCurrentItem = ListStockLine.Where(q => q.ItemNo == getItemNo.ItemNo && q.POG == SelectedPOG.POG && q.FixID == SelectedFixelID.FixelID).FirstOrDefault();
                    }
                    else
                    {
                        tmpCurrentItem = null;
                    }
                }

                if (DataModel.Release == 0 && (DataModel.NoLine == "1" || (DataModel.NoLine != "1" && tmpCurrentItem == null)))
                {
                    if (tmpCurrentItem != null)
                    {
                        if (tmpCurrentItem.IsHasData == "KhongCo")
                        {
                            tmpCurrentItem.Quantity_Scan = 0;

                            tmpCurrentItem.ComparePCS = false;
                        }
                        else
                        {
                            tmpCurrentItem.Quantity_Scan = tmpCurrentItem.Quantity_Scan + 1;

                            tmpCurrentItem.ComparePCS = true;
                        }

                        tmpCurrentItem.ColorIsScan = "#03a9f3";
                        tmpCurrentItem.IsHasData = "Co";

                        var currentData = new LSRetail_StockCountDetailModel()
                        {
                            DocumentNo = tmpCurrentItem.DocumentNo,
                            POG = tmpCurrentItem.POG,
                            FixelID = tmpCurrentItem.FixID,
                            ItemName = tmpCurrentItem.ItemName,
                            BarcodeNo = tmpCurrentItem.BarcodeNo,
                            IsHasData = "Co",
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

                            var temp = new List<StockCountLineModel>(ListStockLine);
                            temp.Remove(tmpCurrentItem);
                            temp.Insert(0, tmpCurrentItem);

                            var modelRemoveFull = ListStockLineFull.Where(s => (s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo) && s.POG == SelectedPOG.POG && s.FixID == SelectedFixelID.FixelID).FirstOrDefault();
                            ListStockLineFull.Remove(modelRemoveFull);
                            ListStockLineFull.Insert(0, tmpCurrentItem);

                            ListStockLine.Clear();
                            ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                            #region reload group

                            var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo).OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();
                            foreach (var item in sortedPartners)
                            {
                                var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                                var modelRemove = PartnersGrouped.Where(s => s.Key == barcodeNo || s.FirstOrDefault().ItemNo == barcodeNo).FirstOrDefault();

                                PartnersGrouped.Remove(modelRemove);
                                PartnersGrouped.Insert(0, modelUp);
                            }

                            #endregion

                            DataModel.CountItem = ListStockLine.Count();
                            DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                            DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " added." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        var userstore = Application.Current.Properties["UserName"].ToString();
                        var producyModel = LoadProduct(barcodeNo);

                        var CheckItemNoAgain = ListStockLine.Where(q => q.ItemNo == producyModel.ItemNo && q.POG == SelectedPOG.POG && q.FixID == SelectedFixelID.FixelID).FirstOrDefault();

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
                                IsHasData = "Co",
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

                                var modelRemoveFull = ListStockLineFull.Where(s => (s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo) && s.POG == SelectedPOG.POG && s.FixID == SelectedFixelID.FixelID).FirstOrDefault();
                                ListStockLineFull.Remove(modelRemoveFull);
                                ListStockLineFull.Insert(0, CheckItemNoAgain);

                                ListStockLine.Clear();
                                ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                #region reload group

                                var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo).OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();
                                foreach (var item in sortedPartners)
                                {
                                    var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                                    var modelRemove = PartnersGrouped.Where(s => s.Key == barcodeNo || s.FirstOrDefault().ItemNo == barcodeNo).FirstOrDefault();

                                    PartnersGrouped.Remove(modelRemove);
                                    PartnersGrouped.Insert(0, modelUp);
                                }

                                #endregion

                                DataModel.CountItem = ListStockLine.Count();
                                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);

                                var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " added." };
                                PopupNavigation.Instance.PushAsync(dialog, false);
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
                                    POG = SelectedPOG.POG,
                                    FixID = SelectedFixelID.FixelID,
                                    IsDelete = 0,
                                    IsHasData = "Co",
                                    ColorIsScan = "#03a9f3",
                                    ItemNo = producyModel.ItemNo,
                                    Quantity = producyModel.Stock,
                                    Quantity_Scan = 0,
                                    UserCreate = userstore
                                };

                                ListStockLineFull.Insert(0, tmpDataModel);

                                var temp = new List<StockCountLineModel>(ListStockLine);
                                temp.Insert(0, tmpDataModel);

                                ListStockLine.Clear();
                                ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                                #region reload group

                                var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo).OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();
                                foreach (var item in sortedPartners)
                                {
                                    var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                                    var modelRemove = PartnersGrouped.Where(s => s.Key == barcodeNo || s.FirstOrDefault().ItemNo == barcodeNo).FirstOrDefault();

                                    PartnersGrouped.Remove(modelRemove);
                                    PartnersGrouped.Insert(0, modelUp);
                                }

                                #endregion

                                DataModel.CountItem = ListStockLine.Count();
                                DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);


                                HttpClient client = new HttpClient();
                                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                                client.DefaultRequestHeaders.Authorization = authHeader;

                                var model = new LSRetail_StockCountMasterModel();
                                model.DocumentNo = DataModel.DocumentNo;
                                model.Description = DataModel.Desciption;
                                model.IsDelete = 3;
                                model.Lines = new List<LSRetail_StockCountDetailModel>()
                            {
                                new LSRetail_StockCountDetailModel()
                                {
                                    DocumentNo = tmpDataModel.DocumentNo,
                                    Zone =0,
                                    POG = SelectedPOG.POG,
                                    FixelID = SelectedFixelID.FixelID,
                                    ItemName = tmpDataModel.ItemName,
                                    BarcodeNo = tmpDataModel.BarcodeNo,
                                    ID = tmpDataModel.ID,
                                    DateCreate = tmpDataModel.DateCreate,
                                    IsDelete = tmpDataModel.IsDelete,
                                    ItemNo = tmpDataModel.ItemNo,
                                    Quantity = tmpDataModel.Quantity,
                                    Quantity_Scan = tmpDataModel.Quantity_Scan,
                                    UserCreate = tmpDataModel.UserCreate
                                }
                            };

                                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/UpsertStockCount", string.Empty));

                                var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                var response = client.PostAsync(uri, requestJson).Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    string content = await response.Content.ReadAsStringAsync();

                                    var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountMaster_ResuftModel>(content);

                                    if (dataList.Active == false)
                                    {
                                        Application.Current.Properties["IsLogin"] = false;

                                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
                                    }

                                    if (dataList != null)
                                    {
                                        if (dataList.Code == 200)
                                        {
                                            DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                            DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);

                                            try
                                            {
                                                PopupNavigation.Instance.PopAsync();
                                            }
                                            catch (Exception) { }

                                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " added." };
                                            PopupNavigation.Instance.PushAsync(dialog, false);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                PopupNavigation.Instance.PopAsync();
                                            }
                                            catch (Exception) { }

                                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                                            PopupNavigation.Instance.PushAsync(dialog);
                                        }
                                    }
                                    else
                                    {
                                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is empty." };
                                        PopupNavigation.Instance.PushAsync(dialog);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    #region reload group

                    var sortedPartners = ListStockLine.Where(s => s.BarcodeNo == barcodeNo || s.ItemNo == barcodeNo && s.POG == SelectedPOG.POG && s.FixID == SelectedFixelID.FixelID)
                        .OrderBy(x => x.BarcodeNo).GroupBy(y => y.BarcodeNo).ToList();

                    if (sortedPartners != null)
                    {
                        foreach (var item in sortedPartners)
                        {
                            var modelUp = new PartnersGrouping<string, StockCountLineModel>(item.Key, item.ToList());
                            var modelRemove = PartnersGrouped.Where(s => s.Key == barcodeNo || s.FirstOrDefault().ItemNo == barcodeNo).FirstOrDefault();

                            PartnersGrouped.Remove(modelRemove);
                            PartnersGrouped.Insert(0, modelUp);
                        }

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

                    #endregion
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

        public async Task<bool> UpsertQuantityScan(LSRetail_StockCountDetailModel obj)
        {
            try
            {
                ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=Upsert Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

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
                        }
                        catch (Exception)
                        {
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
            public int Zone;
            public int QuantityPOG;
            public string POG;
            public string FixelID;
            public string DocumentNo;
            public string BarcodeNo;
            public string Image;
            public string ItemName;
            public string IsHasData;
            public string ItemNo;
            public DateTime DateCreate;
            public int Quantity;
            public int Quantity_Scan;
            public int IsDelete;
            public string UserCreate;
        }
    }
}
