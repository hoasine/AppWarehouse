using AppName.Model;
using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class TaoPhieuXuatViewModel : BaseViewModel
    {
        #region Properties

        protected INavigation Navigation { get; private set; }
        protected Page Page { get; private set; }

        private bool _IsScan;
        public bool IsScan
        {
            get { return _IsScan; }
            set { SetProperty(ref _IsScan, value); }
        }

        private bool _checkedStep1 = true;
        /// <summary>
        /// Status Step 1
        /// </summary>
        public bool CheckedStep1
        {
            get { return _checkedStep1; }
            set { SetProperty(ref _checkedStep1, value); }
        }

        private bool _checkedStep2;
        /// <summary>
        /// Status Step 2
        /// </summary>
        public bool CheckedStep2
        {
            get { return _checkedStep2; }
            set { SetProperty(ref _checkedStep2, value); }
        }

        private bool _checkedStep3;
        /// <summary>
        /// Status Step 3
        /// </summary>
        public bool CheckedStep3
        {
            get { return _checkedStep3; }
            set { SetProperty(ref _checkedStep3, value); }
        }

        private bool _visibleStep1 = true;
        /// <summary>
        /// View Step 1
        /// </summary>
        public bool VisibleStep1
        {
            get { return _visibleStep1; }
            set { SetProperty(ref _visibleStep1, value); }
        }

        private bool _visibleStep2;
        /// <summary>
        /// View Step 2
        /// </summary>
        public bool VisibleStep2
        {
            get { return _visibleStep2; }
            set { SetProperty(ref _visibleStep2, value); }
        }

        private bool _visibleStep3;
        /// <summary>
        /// View Step 3
        /// </summary>
        public bool VisibleStep3
        {
            get { return _visibleStep3; }
            set { SetProperty(ref _visibleStep3, value); }
        }

        private bool _visibleNotification;
        /// <summary>
        /// Ẩn/Hiện View thông báo sau khi xác nhận
        /// </summary>
        public bool VisibleNotification
        {
            get { return _visibleNotification; }
            set { SetProperty(ref _visibleNotification, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private ObservableCollection<SanPhamModel> _items;
        public ObservableCollection<SanPhamModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private ObservableCollection<BrandModel> _listBrand;
        public ObservableCollection<BrandModel> ListBrand
        {
            get { return _listBrand; }
            set { SetProperty(ref _listBrand, value); }
        }

        private ObservableCollection<CompanyModel> _listCompany;
        public ObservableCollection<CompanyModel> ListCompany
        {
            get { return _listCompany; }
            set { SetProperty(ref _listCompany, value); }
        }

        private ObservableCollection<LocationModel> _listLocation;
        public ObservableCollection<LocationModel> ListLocation
        {
            get { return _listLocation; }
            set { SetProperty(ref _listLocation, value); }
        }

        private ObservableCollection<ReasonModel> _ReasonList;
        public ObservableCollection<ReasonModel> ReasonList
        {
            get { return _ReasonList; }
            set { SetProperty(ref _ReasonList, value); }
        }

        private LocationModel _selectedStoreFrom;
        public LocationModel SelectedStoreFrom
        {
            get { return _selectedStoreFrom; }
            set { SetProperty(ref _selectedStoreFrom, value); }
        }

        private LocationModel _selectedStoreTo;
        public LocationModel SelectedStoreTo
        {
            get { return _selectedStoreTo; }
            set { SetProperty(ref _selectedStoreTo, value); }
        }

        private DateTime _postDate = DateTime.Today;
        public DateTime PostDate
        {
            get { return _postDate; }
            set { SetProperty(ref _postDate, value); }
        }

        private CompanyModel _selectedCompany;
        public CompanyModel SelectedCompany
        {
            get { return _selectedCompany; }
            set { SetProperty(ref _selectedCompany, value); }
        }

        private ReasonModel _SelectedReason;
        public ReasonModel SelectedReason
        {
            get { return _SelectedReason; }
            set { SetProperty(ref _SelectedReason, value); }
        }

        private BrandModel _selectedBrand;
        public BrandModel SelectedBrand
        {
            get { return _selectedBrand; }
            set { SetProperty(ref _selectedBrand, value); }
        }

        public ICommand NextStep2Command { get; set; }
        public ICommand NextStep3Command { get; set; }
        public ICommand TapStepCommand { get; set; }
        public ICommand OpenSanPhamDetailCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ICommand EditSanPhamCommand { get; set; }
        public ICommand DeleteSanPhamCommand { get; set; }

        public Command ClearDataCommand { get; set; }
        public Command ChangeDescription { get; set; }
        public Command ChangeStoreFrom { get; set; }
        public Command ChangeStoreTo { get; set; }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        #endregion

        #region Constructors

        public Command LoadItemsCommand { get; set; }

        public TaoPhieuXuatViewModel(INavigation navigation, Page page)
        {
            IsScan = false;

            Navigation = navigation;
            Page = page;

            ClearDataCommand = new Command(ClearDataAsync);
            ChangeDescription = new Command(ChangeDescriptionAsync);
            ChangeStoreFrom = new Command(ChangeStoreFromAsync);
            ChangeStoreTo = new Command(ChangeStoreToAsync);

            Items = new ObservableCollection<SanPhamModel>();

            //NextStep2Command = new Command(() =>
            //{
            //    VisibleStep1 = VisibleStep3 = false;
            //    VisibleStep2 = CheckedStep2 = true;
            //});

            NextStep2Command = new Command(() =>
            {
                VisibleStep1 = VisibleStep2 = false;
                VisibleStep3 = CheckedStep3 = true;
            });

            // stepIndex chỉ bằng 1 hoặc 2 hoặc 3
            TapStepCommand = new Command<object>(stepIndex =>
            {
                if (stepIndex.Equals("1"))
                {
                    VisibleStep1 = CheckedStep1 = true;

                    CheckedStep2 = CheckedStep3 = VisibleStep2 = VisibleStep3 = false;
                }
                //else if (stepIndex.Equals("2"))
                //{
                //    CheckedStep1 = VisibleStep2 = CheckedStep2 = true;

                //    VisibleStep1 = CheckedStep3 = VisibleStep3 = false;
                //}
                else
                {
                    CheckedStep1 = CheckedStep2 = VisibleStep3 = CheckedStep3 = true;

                    VisibleStep1 = VisibleStep2 = false;
                }
            });

            OpenSanPhamDetailCommand = new Command<string>(OpenSanPhamDetailAsync);

            ConfirmCommand = new Command(ConfirmAsync);
            CloseCommand = new Command(CloseAsync);

            EditSanPhamCommand = new Command<SanPhamModel>(EditSanPhamAsync);
            DeleteSanPhamCommand = new Command<SanPhamModel>(DeleteSanPhamAsync);
            SelectedReason = new ReasonModel();
            LoadItemsCommand = new Command(async () => await LoadCategoryData());
        }

        public void ChangeDescriptionAsync()
        {
            try
            {
                using (var transaction = RealmHelper.Instance.BeginWrite())
                {

                    var updateTO = RealmHelper.GetItemTOHeader();
                    if (updateTO != null)
                    {
                        updateTO.Vietnamese_Description = Description;

                        RealmHelper.Instance.Add(updateTO, true);
                    }
                    else
                    {
                        var model = new CreateTOHeaderModel();
                        model.Vietnamese_Description = Description;
                        model.Posting_Date = DateTime.Now.ToString("yyyy-MM-dd");

                        if (SelectedStoreFrom != null)
                        {
                            model.Store_from = SelectedStoreFrom.LocationCode;
                            model.Store_from_address = SelectedStoreFrom.Address;
                            model.Store_from_name = SelectedStoreFrom.LocationName;
                        }
                        else
                        {
                            model.Store_from = "";
                            model.Store_from_address = "";
                            model.Store_from_name = "";
                        }

                        if (SelectedStoreTo != null)
                        {
                            model.Store_to = SelectedStoreTo.LocationCode;
                            model.Store_to_address = SelectedStoreTo.Address;
                            model.Store_to_name = SelectedStoreTo.LocationName;
                        }
                        else
                        {
                            model.Store_to = "";
                            model.Store_to_address = "";
                            model.Store_to_name = "";
                        }

                        RealmHelper.Instance.Add(model, false);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public void ChangeStoreFromAsync()
        {
            try
            {
                using (var transaction = RealmHelper.Instance.BeginWrite())
                {
                    var updateTO = RealmHelper.GetItemTOHeader();
                    if (updateTO != null)
                    {
                        if (SelectedStoreFrom != null)
                        {
                            updateTO.Store_from = SelectedStoreFrom.LocationCode;
                            updateTO.Store_from_address = SelectedStoreFrom.Address;
                            updateTO.Store_from_name = SelectedStoreFrom.LocationName;
                        }

                        RealmHelper.Instance.Add(updateTO, true);
                    }
                    else
                    {
                        var model = new CreateTOHeaderModel();
                        model.Vietnamese_Description = "";
                        model.Posting_Date = DateTime.Now.ToString("yyyy-MM-dd");

                        if (SelectedStoreFrom != null)
                        {
                            model.Store_from = SelectedStoreFrom.LocationCode;
                            model.Store_from_address = SelectedStoreFrom.Address;
                            model.Store_from_name = SelectedStoreFrom.LocationName;
                        }
                        else
                        {
                            model.Store_from = "";
                            model.Store_from_address = "";
                            model.Store_from_name = "";
                        }

                        if (SelectedStoreTo != null)
                        {
                            model.Store_to = SelectedStoreTo.LocationCode;
                            model.Store_to_address = SelectedStoreTo.Address;
                            model.Store_to_name = SelectedStoreTo.LocationName;
                        }
                        else
                        {
                            model.Store_to = "";
                            model.Store_to_address = "";
                            model.Store_to_name = "";
                        }

                        RealmHelper.Instance.Add(model, false);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public void ChangeStoreToAsync()
        {
            try
            {
                using (var transaction = RealmHelper.Instance.BeginWrite())
                {
                    var updateTO = RealmHelper.GetItemTOHeader();
                    if (updateTO != null)
                    {
                        if (SelectedStoreTo != null)
                        {
                            updateTO.Store_to = SelectedStoreTo.LocationCode;
                            updateTO.Store_to_address = SelectedStoreTo.Address;
                            updateTO.Store_to_name = SelectedStoreTo.LocationName;
                        }

                        RealmHelper.Instance.Add(updateTO, true);
                    }
                    else
                    {
                        var model = new CreateTOHeaderModel();
                        model.Vietnamese_Description = "";
                        model.Posting_Date = DateTime.Now.ToString("yyyy-MM-dd");

                        if (SelectedStoreFrom != null)
                        {
                            model.Store_from = SelectedStoreFrom.LocationCode;
                            model.Store_from_address = SelectedStoreFrom.Address;
                            model.Store_from_name = SelectedStoreFrom.LocationName;
                        }
                        else
                        {
                            model.Store_from = "";
                            model.Store_from_address = "";
                            model.Store_from_name = "";
                        }

                        if (SelectedStoreTo != null)
                        {
                            model.Store_to = SelectedStoreTo.LocationCode;
                            model.Store_to_address = SelectedStoreTo.Address;
                            model.Store_to_name = SelectedStoreTo.LocationName;
                        }
                        else
                        {
                            model.Store_to = "";
                            model.Store_to_address = "";
                            model.Store_to_name = "";
                        }

                        RealmHelper.Instance.Add(model, false);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        #endregion

        #region Operators

        private async void CloseAsync(object obj)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception) { }
        }


        private void ConfirmAsync(object obj)
        {
            // Sử dụng các Property: SelectedStoreFrom,SelectedStoreTo,PostDate,SelectedCompany,SelectedBrand,Items để viết request dữ liệu xuống api
            try
            {


                ShowLoading = true;

                Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/CreateShipmentTO", string.Empty));

                var modelInput = new Retail_Transfer_OrderModel();
                modelInput.Posting_Date = PostDate;
                modelInput.Vietnamese_Description = Description;
                modelInput.Store_from = SelectedStoreFrom.LocationCode;
                modelInput.Store_to = SelectedStoreTo.LocationCode;

                var resean = "";
                if (SelectedReason != null)
                {
                    if (!string.IsNullOrEmpty(SelectedReason.ReasonCode))
                    {
                        resean = SelectedReason.ReasonCode;
                    }
                }

                modelInput.Transaction_Type = resean;
                modelInput.TOLines = new List<TransferLinesModel>();

                foreach (var item in Items)
                {
                    var line = new TransferLinesModel();
                    line.ExpireDate = item.ExpireDate;
                    line.Item_No = item.ItemNo;
                    line.Quantity = Convert.ToInt32(item.Quantity);
                    modelInput.TOLines.Add(line);
                }

                var requestJson = new StringContent(JsonConvert.SerializeObject(modelInput));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var dataList = JsonConvert.DeserializeObject<Retail_Transfer_Order_API_Model>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {
                        VisibleStep1 = VisibleStep2 = VisibleStep3 = false;

                        try
                        {
                            Navigation.PopAsync();
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                        }

                        try
                        {
                            var model = new TOHeaderModel();
                            model.BuyerID = Application.Current.Properties["LSRetailName"].ToString().ToUpper();
                            model.DocumentNo = dataList.ListData.No;
                            model.ItemCount = 0;
                            model.PostingDate = modelInput.Posting_Date.ToString("dd/MM/yyyy");
                            model.Quantity = 0;
                            model.TransferfromCode = modelInput.Store_from;
                            model.TransfertoCode = modelInput.Store_to;
                            model.VietnameseDescription = modelInput.Vietnamese_Description;

                            Navigation.PushAsync(new ShipmentTOLinePage(model));
                        }
                        catch (Exception) { }

                        var realm = RealmHelper.Instance;
                        using (var transaction = realm.BeginWrite())
                        {
                            realm.RemoveAll<CreateTOModel>();
                            transaction.Commit();
                        }

                        Items.Clear();

                        using (var transaction = realm.BeginWrite())
                        {
                            realm.RemoveAll<CreateTOHeaderModel>();
                            transaction.Commit();
                        }

                        //Application.Current.MainPage.DisplayAlert("Notification !", "Create transfer order successfully", "OK");
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Notification !", dataList.Content, "OK");
                    }
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
                ShowLoading = false;

            }
        }

        public async void OpenSanPhamDetailAsync(string obj)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoNoImage?KeyValue=" + obj + "&pageSize=1", string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftMasterFileModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    var model = dataList.ListData.FirstOrDefault();

                    if (model != null)
                    {
                        if (model.Stock == 0)
                        {
                            var dialogStock = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Item:=" + model.ItemNo + " system inventory is lower than the actual quantity. Please increase the inventory." };
                            PopupNavigation.Instance.PushAsync(dialogStock);
                            return;
                        }

                        var modelData = Items.FirstOrDefault(q => q.ItemNo == model.ItemNo);

                        if (modelData == null)
                        {
                            modelData = new SanPhamModel()
                            {
                                Barcode_No_ = model.Barcode_No_,
                                ItemName = model.ItemName + " " + model.ItemNo,
                                ItemNo = model.ItemNo,
                                UnitPrice = model.Unit_Price.Value.ToString("#,##"),
                                URLImage = "",
                            };
                        }

                        var dialog = new SanPhamDetail();
                        var viewModel = new BarCodeSanPhamNotGetDataViewModel(modelData);
                        viewModel.ClosePopup += (send, barCode) =>
                        {
                            try
                            {
                                var index = Items.IndexOf(modelData);

                                if (index == -1)
                                {
                                    Items.Add(barCode);

                                    var listItemUpdate = new CreateTOModel
                                    {
                                        Barcode_No_ = modelData.Barcode_No_,
                                        ItemName = modelData.ItemName,
                                        ItemNo = modelData.ItemNo,
                                        UnitPrice = modelData.UnitPrice,
                                        URLImage = modelData.Image,
                                        Quantity = modelData.Quantity
                                    };

                                    RealmHelper.UpdateModel(listItemUpdate, false);
                                }
                                else
                                {
                                    Items.RemoveAt(index);
                                    Items.Insert(index, barCode);

                                    var updateTO = RealmHelper.GetItemTO(barCode.ItemNo);
                                    if (updateTO != null)
                                    {
                                        RealmHelper.UpdateModel(updateTO, true);
                                    }
                                }

                                IsScan = false;

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
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Notification !", "Not found product " + obj + ".", "OK");
                    }
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "Not found product " + obj + ".", "OK");
                }

                IsScan = false;
            }
            catch (Exception ex)
            {
                IsScan = false;

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                ShowLoading = false;
            }
        }

        private void EditSanPhamAsync(SanPhamModel obj)
        {
            try
            {
                var dialog = new SanPhamDetail();
                var viewModel = new BarCodeSanPhamNotGetDataViewModel(obj);
                viewModel.IsEditData = true;
                viewModel.ClosePopup += async (send, barCode) =>
                {
                    try
                    {
                        viewModel.ShowLoading = true;

                        await Task.Delay(1);

                        var index = Items.IndexOf(obj);

                        Items.RemoveAt(index);
                        Items.Insert(index, barCode);

                        var updateTO = RealmHelper.GetItemTO(barCode.ItemNo);
                        if (updateTO != null)
                        {
                            updateTO.Quantity = barCode.Quantity;

                            RealmHelper.UpdateModel(updateTO, true);
                        }
                    }
                    catch (Exception exx)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
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

        private async void DeleteSanPhamAsync(SanPhamModel obj)
        {
            try
            {
                var result = await Page.DisplayAlert("Thông báo", "Bạn có muốn xóa dòng sản phẩm này?", "Có", "Không");

                if (result)
                {
                    Items.Remove(obj);

                    using (var transaction = RealmHelper.Instance.BeginWrite())
                    {
                        var objDelete = RealmHelper.GetItemTO(obj.ItemNo);

                        RealmHelper.Instance.Remove(objDelete);

                        transaction.Commit();
                    }
                }
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
                                realm.RemoveAll<CreateTOModel>();
                                transaction.Commit();
                            }

                            using (var transaction = realm.BeginWrite())
                            {
                                realm.RemoveAll<CreateTOHeaderModel>();
                                transaction.Commit();
                            }

                            Items.Clear();

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

        private async Task LoadCategoryData()
        {
            await LoadLocation();

            await LoadItem();

            await LoadReason();
        }

        private async Task LoadBrand()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetBrand");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listBrand = JsonConvert.DeserializeObject<BrandModel[]>(content).OrderBy(s => s.Name);

                    ListBrand = new ObservableCollection<BrandModel>(listBrand);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadLocation()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetLocation");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listLocation = JsonConvert.DeserializeObject<LocationModel[]>(content).OrderBy(s => s.LocationName);

                    foreach (var item in listLocation)
                    {
                        item.LocationDisplay = item.LocationCode + " - " + item.LocationName;
                    }

                    ListLocation = new ObservableCollection<LocationModel>(listLocation);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadReason()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/GetReason");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listType = JsonConvert.DeserializeObject<ResuftReasonModel>(content);

                    if (listType.ListData != null)
                    {
                        var listReasonForTransfer = listType.ListData.Where(s => s.ReasonCode.Contains("TO_")).ToList();

                        ReasonList = new ObservableCollection<ReasonModel>(listReasonForTransfer);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadItem()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var itemTam = RealmHelper.GetItemTOAll().Select(modelData => new SanPhamModel()
                {
                    Barcode_No_ = modelData.Barcode_No_,
                    ItemName = modelData.ItemName,
                    ItemNo = modelData.ItemNo,
                    UnitPrice = modelData.UnitPrice,
                    URLImage = modelData.Image,
                    Quantity = modelData.Quantity
                }).ToList();

                Items.Clear();

                Items = new ObservableCollection<SanPhamModel>(itemTam);

                var headerModel = RealmHelper.GetItemTOHeader();
                if (headerModel != null)
                {
                    Description = headerModel.Vietnamese_Description;

                    var getStoreFrom = ListLocation.Where(s => s.LocationCode == headerModel.Store_from).FirstOrDefault();
                    if (getStoreFrom != null)
                    {
                        SelectedStoreFrom = getStoreFrom;
                    }
                    var getStoreTo = ListLocation.Where(s => s.LocationCode == headerModel.Store_to).FirstOrDefault();
                    if (getStoreTo != null)
                    {
                        SelectedStoreTo = getStoreTo;
                    }

                    PostDate = DateTime.Parse(headerModel.Posting_Date);
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }


        private async Task LoadCompany()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetCompany");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listCompany = JsonConvert.DeserializeObject<CompanyModel[]>(content).OrderBy(s => s.Name);

                    ListCompany = new ObservableCollection<CompanyModel>(listCompany);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public partial class ResuftReasonModel
        {
            public bool Active { get; set; }
            public List<ReasonModel> ListData { get; set; }
        }

        public class ReasonModel
        {
            public string ReasonCode { get; set; }
            public string ReasonName { get; set; }
        }
        #endregion
    }
}
