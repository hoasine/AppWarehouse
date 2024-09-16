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
using AppName.Model;
using Rg.Plugins.Popup.Services;
using System.Linq;

namespace AppName
{
    public class ReceiveTOViewModel : BaseViewModel
    {
        #region Properties

        protected INavigation Navigation { get; private set; }

        public Command RefreshCommand { get; set; }

        private ObservableCollection<TOHeaderModel> _listTOHeader;
        public ObservableCollection<TOHeaderModel> ListTOHeader
        {
            get { return _listTOHeader; }
            set { SetProperty(ref _listTOHeader, value); }
        }

        #endregion

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        #region Constructors
        public Command LoadItemsCommand { get; set; }

        public ReceiveTOViewModel(INavigation navigation)
        {
            Navigation = navigation;

            RefreshCommand = new Command(async () => LoadReceiveTO());

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "RefreshPageReceivingTO");

            MessagingCenter.Subscribe<App>((App)Application.Current, "RefreshPageReceivingTO", (sender) =>
            {
                LoadReceiveTO();
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            LoadReceiveTO();
        }

        #endregion

        #region Operators

        public partial class ResuftTOHeaderModel
        {
            public bool Active { get; set; }
            public List<TOHeaderModel> ListData { get; set; }
        }

        private async void LoadReceiveTO()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/GetToTOHeader?location=" + store);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listTOHeader = JsonConvert.DeserializeObject<ResuftTOHeaderModel>(content);
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
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;

                ShowLoading = false;
            }
        }

        #endregion
    }
}
