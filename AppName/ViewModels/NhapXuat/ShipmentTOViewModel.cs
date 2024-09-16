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
    public class ShipmentTOViewModel : BaseViewModel
    {
        #region Properties

        protected INavigation Navigation { get; private set; }

        private ObservableCollection<TOHeaderModel> _listTOHeader;
        public Command RefreshCommand { get; set; }
        public ObservableCollection<TOHeaderModel> ListTOHeader
        {
            get { return _listTOHeader; }
            set { SetProperty(ref _listTOHeader, value); }
        }

        public Command LoadItemsCommand { get; set; }

        #endregion

        #region Constructors

        public ShipmentTOViewModel(INavigation navigation)
        {
            Navigation = navigation;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            RefreshCommand = new Command(async () => LoadShipmentTO());

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "RefreshPageTO");

            MessagingCenter.Subscribe<App>((App)Application.Current, "RefreshPageTO", (sender) =>
            {
                LoadShipmentTO();
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            LoadShipmentTO();
        }

        #endregion

        #region Operators

        public partial class ResuftTOModel
        {
            public bool Active { get; set; }
            public List<TOHeaderModel> ListData { get; set; }
        }

        //Shipment TO
        async Task LoadShipmentTO()
        {
            try
            {
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (IsBusy)
                    return;
                IsBusy = true;

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/GetFromTOHeader?location=" + store);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listTOHeader = JsonConvert.DeserializeObject<ResuftTOModel>(content);

                    if (listTOHeader.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    ListTOHeader = new ObservableCollection<TOHeaderModel>(listTOHeader.ListData);
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
