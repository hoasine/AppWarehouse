using AppName.CustomRenderer;
using AppName.Model;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static AppName.ViewModels.APIBaseViewModel;

namespace AppName
{
    public class AddStockTakeViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }


        #region field page

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _UserStoreIDs;
        public string UserStoreIDs
        {
            get { return _UserStoreIDs; }
            set { SetProperty(ref _UserStoreIDs, value); }
        }

        private int _QtyPack;
        public int QtyPack
        {
            get { return _QtyPack; }
            set { SetProperty(ref _QtyPack, value); }
        }

        //private ObservableCollection<LocationModel> _listLocation;
        //public ObservableCollection<LocationModel> ListLocation
        //{
        //    get { return _listLocation; }
        //    set { SetProperty(ref _listLocation, value); }
        //}

        private ObservableCollection<AreasModel> _AreaModel;
        public ObservableCollection<AreasModel> AreaModel
        {
            get { return _AreaModel; }
            set { SetProperty(ref _AreaModel, value); }
        }

        private AreasModel _AreaSelected;
        public AreasModel AreaSelected
        {
            get { return _AreaSelected; }
            set { SetProperty(ref _AreaSelected, value); }
        }

        private DateTime _Date = DateTime.Today;
        public DateTime Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }

        //private LocationModel _selectedStoreFrom;
        //public LocationModel SelectedStoreFrom
        //{
        //    get { return _selectedStoreFrom; }
        //    set { SetProperty(ref _selectedStoreFrom, value); LoadArea(value.LocationCode); }
        //}

        #endregion  

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        private ObservableCollection<UserStoreStockModel> _listSelectedAlternately;
        public ObservableCollection<UserStoreStockModel> ListSelectedAlternately
        {
            get { return _listSelectedAlternately; }
            set { SetProperty(ref _listSelectedAlternately, value); }
        }

        private ObservableCollection<UserStoreStockModel> _listAlternately;
        public ObservableCollection<UserStoreStockModel> ListAlternately
        {
            get { return _listAlternately; }
            set { SetProperty(ref _listAlternately, value); }
        }

        private bool _visibleAlternately;
        public bool VisibleAlternately
        {
            get { return _visibleAlternately; }
            set { SetProperty(ref _visibleAlternately, value); }
        }

        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { SetProperty(ref _IsChecked, value); }
        }

        private bool _SelectedAlternately;
        public bool SelectedAlternately
        {
            get { return _SelectedAlternately; }
            set { SetProperty(ref _SelectedAlternately, value); }
        }



        private ObservableCollection<UserStoreModel> _staffList;
        public ObservableCollection<UserStoreModel> StaffList
        {
            get { return _staffList; }
            set { SetProperty(ref _staffList, value); }
        }

        private ObservableCollection<AreasModel> _ListAreas;
        public ObservableCollection<AreasModel> ListAreas
        {
            get { return _ListAreas; }
            set { SetProperty(ref _ListAreas, value); }
        }

        private UserStoreModel _selectedStaff;
        public UserStoreModel SelectedStaff
        {
            get { return _selectedStaff; }
            set { SetProperty(ref _selectedStaff, value); }
        }

        private ObservableCollection<UserStoreStockModel> _listAlternatelyTam;
        public ObservableCollection<UserStoreStockModel> ListAlternatelyTam
        {
            get { return _listAlternatelyTam; }
            set { SetProperty(ref _listAlternatelyTam, value); }
        }

        public Command LoadItemsCommand { get; set; }

        public ICommand ConfirmCommand { get; set; }
        public ICommand ShowAlternatelyPersonViewCommand { get; set; }
        public ICommand AddAreasCommand { get; set; }
        public ICommand ItemTappedAlternatelyCommand { get; set; }
        public ICommand HideAlternatelyPersonCommand { get; set; }
        public ICommand TextChangedAlternatelyCommand { get; set; }
        //Chuaw lafm
        public ICommand SearchAlternatelyCommand { get; set; }

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

        public AddStockTakeViewModel(INavigation navigation)
        {
            ConfirmCommand = new Command(ConfirmAsync);

            ShowAlternatelyPersonViewCommand = new Command(ShowUserMulti);

            HideAlternatelyPersonCommand = new Command(HideUsername);

            AddAreasCommand = new Command<AreasModel>(AddAreas);

            ItemTappedAlternatelyCommand = new Command<UserStoreStockModel>(SelectUser);

            Navigation = navigation;

            ListAreas = new ObservableCollection<AreasModel>();

            ListSelectedAlternately = new ObservableCollection<UserStoreStockModel>();
            ListAlternatelyTam = new ObservableCollection<UserStoreStockModel>();
            ListStockLine = new ObservableCollection<StockCountLineModel>();

            LoadItemsCommand = new Command(async () => LoadData());
        }

        public async Task LoadData()
        {
            try
            {
                await LoadArea(Application.Current.Properties["UserName"].ToString());

                LoadUserStoreCommand();
            }
            catch (Exception ex)
            {

            }
        }

        public async void SearchAlternatelyAsync(string Filter)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                ListAlternately = new ObservableCollection<UserStoreStockModel>(ListAlternatelyTam.OrderBy(o => o.RetailName).OrderByDescending(r => r.RetailName));
            }
            else
            {
                ListAlternately = new ObservableCollection<UserStoreStockModel>(ListAlternatelyTam.Where(c => c.RetailName.ToString().ToLower().
                                   Contains(Filter.ToLower())).OrderByDescending(r => r.RetailName));
            }
        }

        public async void AddAreas(AreasModel obj)
        {
            try
            {
                var dialog = new AddAreaPage();
                var viewModel = new AddAreasViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.ClosePopup += async (send, data) =>
                {
                    if (data != null)
                    {
                        try
                        {
                            if (IsBusy)
                                return;
                            IsBusy = true;

                            var temp = new List<AreasModel>(ListAreas);
                            temp.Remove(obj);
                            temp.Insert(0, data);

                            ListAreas.Clear();
                            ListAreas = new ObservableCollection<AreasModel>(temp);
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

        private async void LoadUserStoreCommand()
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

                var obj = Application.Current.Properties["LSRetailName"]?.ToString();
                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/user/GetUserStore?storecode=" + obj);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var userStore = JsonConvert.DeserializeObject<UserStoreStockModel[]>(content).OrderBy(s => s.RetailID);

                    ListAlternately = new ObservableCollection<UserStoreStockModel>(userStore);
                    ListAlternatelyTam = new ObservableCollection<UserStoreStockModel>(userStore);

                    var listStaff = userStore.Where(s => s.Permission == "MANAGER").ToList();
                    //var listStaff = userStore.ToList();
                    var listStaffResuft = new List<UserStoreModel>();

                    foreach (var item in listStaff)
                    {
                        var modelStaff = new UserStoreModel();
                        modelStaff.Permission = item.Permission;
                        modelStaff.RetailID = item.RetailID;
                        modelStaff.RetailName = item.RetailName;

                        listStaffResuft.Add(modelStaff);
                    }

                    StaffList = new ObservableCollection<UserStoreModel>(listStaffResuft);
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }


        private async void HideUsername()
        {
            VisibleAlternately = false;

            if (ListSelectedAlternately.Count() > 0)
            {
                var list = ListSelectedAlternately.Select(s => s.RetailID).ToList();

                UserStoreIDs = string.Join(",", list);
            }
            else
            {
                UserStoreIDs = "";
            }

        }

        private async void ShowUserMulti()
        {
            VisibleAlternately = true;

            if (ListSelectedAlternately.Count() > 0)
            {
                foreach (var item in ListAlternately)
                {
                    var model = ListSelectedAlternately.Where(s => s.RetailID == item.RetailID).FirstOrDefault();
                    item.IsChecked = model.IsChecked;
                }
            }
        }

        private async void SelectUser(UserStoreStockModel obj)
        {
            obj.IsChecked = !obj.IsChecked;

            if (obj.IsChecked)
            {
                ListSelectedAlternately.Add(obj);
            }
            else
            {
                ListSelectedAlternately.Remove(obj);
            }
        }

        private async void ConfirmAsync(object obj)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (string.IsNullOrEmpty(Description))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Description field data missing." };
                    PopupNavigation.Instance.PushAsync(dialog);

                    return;
                }

                if (SelectedStaff == null)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "StaffID field data missing." };
                    PopupNavigation.Instance.PushAsync(dialog);

                    return;
                }

                if (ListSelectedAlternately.Count() <= 0)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Management field data missing." };
                    PopupNavigation.Instance.PushAsync(dialog);

                    return;
                }

                var model = new LSRetail_StockCountMasterModel();
                model.Description = Description;
                model.IsSend = 0;
                model.IsDelete = 0;
                model.Status = "New";
                model.Release = 0;
                model.DateCreate = Date;
                model.AreaMask = AreaSelected.Code;
                model.RetailStaff = SelectedStaff.RetailID;
                model.StockType = "StockTake";
                model.LocationCode = Application.Current.Properties["UserName"].ToString();
                model.UserCreate = Application.Current.Properties["UserStore"]?.ToString();


                if (ListSelectedAlternately.Count() > 0)
                {
                    var stringSelect = string.Join(",", ListSelectedAlternately.Select(s => s.RetailID).ToList());

                    if (!string.IsNullOrEmpty(stringSelect))
                    {
                        model.Management = stringSelect;
                    }
                    else
                    {
                        model.Management = "";
                    }
                }

                model.QtyPack = 0;
                model.Lines = new List<LSRetail_StockCountDetailModel>(); ;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/CreateStockCount");

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(uri, requestJson);

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
                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "Create a successfuly check sheet." };
                            PopupNavigation.Instance.PushAsync(dialog);

                            try
                            {
                                await Navigation.PopAsync();
                            }
                            catch (Exception)
                            {
                            }

                            MessagingCenter.Send<App>((App)Application.Current, "ReloadStockTakePage");
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
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

        //private async Task LoadLocation()
        //{
        //    if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

        //        HttpClient client = new HttpClient();
        //        client.DefaultRequestHeaders.Authorization = authHeader;

        //        Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetLocation");

        //        HttpResponseMessage response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string content = await response.Content.ReadAsStringAsync();

        //            var listLocation = JsonConvert.DeserializeObject<LocationModel[]>(content).OrderBy(s => s.LocationName);

        //            ListLocation = new ObservableCollection<LocationModel>(listLocation);
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
        //        PopupNavigation.Instance.PushAsync(dialog, false);
        //    }
        //}

        public class AreasModel
        {
            public string Code { get; set; }
            public string MasrkName { get; set; }
        }



        private async Task LoadArea(string Store)
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

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetMark?store=" + Store);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listLocation = JsonConvert.DeserializeObject<AreasModel[]>(content).OrderBy(s => s.Code);

                    AreaModel = new ObservableCollection<AreasModel>(listLocation);
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

        public partial class LSRetail_StockCountMaster_ResuftModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public LSRetail_StockCountMasterModel ListData { get; set; }
        }

        public partial class LSRetail_StockCountMasterModel
        {
            public string ID;
            public string Description;
            public string DocumentNo;
            public string LocationCode;
            public string Status;
            public string Management;
            public string StockType;
            public int QtyPack;
            public int IsSend;
            public string AreaMask;
            public string RetailStaff;
            public int Release;
            public int IsDelete;
            public DateTime DateCreate;
            public string UserCreate;
            public List<LSRetail_StockCountDetailModel> Lines;
        }

        public partial class LSRetail_StockCountDetailModel
        {
            public string ID;
            public string DocumentNo;
            public string BarcodeNo;
            public string ItemName;
            public int Zone;
            public string POG;
            public string ItemNo;
            public DateTime DateCreate;
            public int Quantity;
            public int Quantity_Scan;
            public int IsDelete;
            public string UserCreate;
        }



        public partial class ResuftCountLineModel
        {
            public int code { get; set; }
            public string message { get; set; }

            public List<StockCountLineModel> data;
        }

        public class UserStoreStockModel : ObservableObject
        {
            public string RetailID { get; set; }
            public string RetailName { get; set; }
            public string Permission { get; set; }
            private bool _IsChecked;
            public bool IsChecked
            {
                get { return _IsChecked; }
                set { SetProperty(ref _IsChecked, value); }
            }
        }
    }
}
