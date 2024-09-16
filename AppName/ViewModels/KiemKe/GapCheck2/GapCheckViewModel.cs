using AppName.Model;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static AppName.ViewModels.APIBaseViewModel;

namespace AppName
{
    public class GapCheckViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public ICommand NavigateAddGapCheckCommand { get; set; }
        public ICommand SearchItemsCommand { get; set; }
        public ICommand EditGapCheckCommand { get; set; }
        public ICommand DeleteGapCheckCommand { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public ObservableCollection<PartnersGrouping<string, StockCountModel>> ListStockCount { get; set; }

        public GapCheckViewModel(INavigation navigation)
        {
            NavigateAddGapCheckCommand = new Command(async () => await NavigateAddGapCheckAsync());
            SearchItemsCommand = new Command<string>(SearchItemsAsync);
            EditGapCheckCommand = new Command<StockCountModel>(EditGapCheckAsync);
            DeleteGapCheckCommand = new Command<StockCountModel>(DeleteGapCheckAsync);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ListStockCount = new ObservableCollection<PartnersGrouping<string, StockCountModel>>();

            Navigation = navigation;

            RefreshCommand = new Command(async () => LoadData());

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "ReloadGapCheckPage");

            MessagingCenter.Subscribe<App>((App)Application.Current, "ReloadGapCheckPage", (sender) =>
            {
                LoadData();
            });
        }

        private async void EditGapCheckAsync(StockCountModel obj)
        {
            await Navigation.PushAsync(new EditGapCheckPage(obj));
        }

        public partial class LSRetail_StockCountMasterModel
        {
            public string ID;
            public string Description;
            public string DocumentNo;
            public string StockType;
            public string RetailStaff;
            public string LocationCode;
            public string Status;
            public string Management;
            public int IsSend;
            public int Release;
            public int IsDelete;
            public int QtyPack;
            public DateTime DateCreate;
            public string UserCreate;
        }

        public partial class LSRetail_StockCountMaster_ResuftModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public LSRetail_StockCountMasterModel ListData { get; set; }
        }

        private async void DeleteGapCheckAsync(StockCountModel obj)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
            if (obj.RetailStaff.ToUpper() != usernameStore.ToUpper())
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You do not have permission for this function. With " + obj.DocumentNo + "." };
                await PopupNavigation.Instance.PushAsync(dialog);
                return;
            }

            try
            {
                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("Do you want to delete the current Gap Check?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            try
                            {
                                //Đóng popup detail trước
                                await PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }


                            HttpClient client = new HttpClient();
                            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                            client.DefaultRequestHeaders.Authorization = authHeader;

                            var model = new LSRetail_StockCountMasterModel();
                            model.DocumentNo = obj.DocumentNo;
                            model.IsDelete = 1;

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
                                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = dataList.Content };
                                        PopupNavigation.Instance.PushAsync(dialog);

                                        LoadData();
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

        private void SearchItemsAsync(string obj)
        {

        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetStockCount?StoreUser=" + userstore + "&StockType=GapCheck");

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountMastertListModel>(content);

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
                            ListStockCount.Clear();
                            var sortedPartners = dataList.ListData.OrderByDescending(x => x.DateCreate).GroupBy(y => y.DocumentRoot).ToList();
                            foreach (var item in sortedPartners)
                            {
                                ListStockCount.Add(new PartnersGrouping<string, StockCountModel>(item.Key, dataList.ListData.Where(x => x.DocumentRoot == item.Key).OrderBy(x => x.DateCreate)));
                            }
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
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public partial class LSRetail_StockCountMastertListModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public List<StockCountModel> ListData { get; set; }
        }

        protected async void LoadData()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetStockCount?StoreUser=" + userstore + "&StockType=GapCheck");

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountMastertListModel>(content);

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
                            ListStockCount.Clear();
                            var sortedPartners = dataList.ListData.OrderByDescending(x => x.DateCreate).GroupBy(y => y.DocumentRoot).ToList();
                            foreach (var item in sortedPartners)
                            {
                                ListStockCount.Add(new PartnersGrouping<string, StockCountModel>(item.Key, dataList.ListData.Where(x => x.DocumentRoot == item.Key).OrderBy(x => x.DateCreate)));
                            }
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
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task NavigateAddGapCheckAsync()
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                var checkTaoPhieuGapCheck = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "TaoPhieuGapCheck"
           && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                if (checkTaoPhieuGapCheck == true)
                {
                    await Navigation.PushAsync(new AddGapCheckPage());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                ShowLoading = false;
            }
        }
    }
}
