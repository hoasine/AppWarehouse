using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Services;
using AppName.Model;
using System.Linq;

namespace AppName
{
    public class ReceivePOViewModel : BaseViewModel
    {
        #region Properties

        protected INavigation Navigation { get; private set; }

        private bool isRefreshing = false;
        public Command RefreshCommand { get; set; }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public Command LoadItemsCommand { get; set; }

        private ObservableCollection<POHeaderModel> _listPOHeader;
        public ObservableCollection<POHeaderModel> ListPOHeader
        {
            get { return _listPOHeader; }
            set { SetProperty(ref _listPOHeader, value); }
        }

        #endregion

        #region Constructors

        public ReceivePOViewModel(INavigation navigation)
        {
            Navigation = navigation;

            LoadItemsCommand = new Command(async () => await ExLoadShipmentPOJOb());

            RefreshCommand = new Command(async () => await ExLoadShipmentPOJOb());

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "RefreshPagePO");

            MessagingCenter.Subscribe<App>((App)Application.Current, "RefreshPagePO", (sender) =>
            {
                LoadReceivePO();
            });
        }

        async Task ExLoadShipmentPOJOb()
        {
            LoadReceivePO();
        }

        #endregion

        #region Operators

        public partial class ResuftPOHeaderModel
        {
            public bool Active { get; set; }
            public List<POHeaderModel> ListData { get; set; }
        }

        private async Task LoadReceivePO()
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
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/GetPOHeader?location=" + store);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listPOHeader = JsonConvert.DeserializeObject<ResuftPOHeaderModel>(content);
                    if (listPOHeader.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    ListPOHeader = new ObservableCollection<POHeaderModel>(listPOHeader.ListData);
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

        #endregion
    }
}
