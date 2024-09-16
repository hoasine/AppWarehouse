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
    public class StockTakeViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private ObservableCollection<StockCountModel> _listStockCount;
        public ObservableCollection<StockCountModel> ListStockCount
        {
            get { return _listStockCount; }
            set { SetProperty(ref _listStockCount, value); }
        }

        public ICommand NavigateAddStockTakeCommand { get; set; }
        public ICommand SearchItemsCommand { get; set; }
        public ICommand EditStockTakeCommand { get; set; }
        public ICommand DeleteStockTakeCommand { get; set; }

        public StockTakeViewModel(INavigation navigation)
        {
            NavigateAddStockTakeCommand = new Command(NavigateAddStockTakeAsync);
            SearchItemsCommand = new Command<string>(SearchItemsAsync);
            EditStockTakeCommand = new Command<StockCountModel>(EditStockTakeAsync);
            DeleteStockTakeCommand = new Command<StockCountModel>(DeleteStockTakeAsync);

            Navigation = navigation;

            LoadData();

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "ReloadStockTakePage");

            MessagingCenter.Subscribe<App>((App)Application.Current, "ReloadStockTakePage", (sender) =>
            {
                LoadData();
            });
        }

        private async void EditStockTakeAsync(StockCountModel obj)
        {
            await Navigation.PushAsync(new EditStockTakePage(obj));
        }

        public partial class DAFC_StockCountMasterModel
        {
            public string ID;
            public string Description;
            public string DocumentNo;
            public string StockType;
            public string LocationCode;
            public string Status;
            public string Management;
            public string RetailStaff;
            public int IsSend;
            public int Release;
            public int IsDelete;
            public int QtyPack;
            public DateTime DateCreate;
            public string UserCreate;
        }

        public partial class DAFC_StockCountMaster_ResuftModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public DAFC_StockCountMasterModel ListData { get; set; }
        }

        private async void DeleteStockTakeAsync(StockCountModel obj)
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var model = new DAFC_StockCountMasterModel();
                model.DocumentNo = obj.DocumentNo;
                model.IsDelete = 1;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/UpsertStockCount", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<DAFC_StockCountMaster_ResuftModel>(content);

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
                            var dialog = new NotificationPopup { Message = dataList.Content };
                            PopupNavigation.Instance.PushAsync(dialog);

                            LoadData();
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup { Message = dataList.Content };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationErrorPopup { Message = "Data is empty." };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private void SearchItemsAsync(string obj)
        {

        }
        public partial class DAFC_StockCountMastertListModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public List<StockCountModel> ListData { get; set; }
        }

        public async void LoadData()
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri("http://10.46.12.133:2525/api/stock/GetStockCount?StoreUser=" + userstore + "&StockType=StockTake");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<DAFC_StockCountMastertListModel>(content);

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
                            ListStockCount = new ObservableCollection<StockCountModel>(dataList.ListData.OrderBy(s => s.DocumentNo));
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup { Message = dataList.Content };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationErrorPopup { Message = "Data is empty." };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async void NavigateAddStockTakeAsync(object obj)
        {
            await Navigation.PushAsync(new AddStockTakePage());
        }
    }
}
