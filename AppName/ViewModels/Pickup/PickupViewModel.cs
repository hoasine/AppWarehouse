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
using AppName.Model.Pickup;
using System.Windows.Input;

namespace AppName
{
    public class PickupViewModel : BaseViewModel
    {
        #region Properties

        protected INavigation Navigation { get; private set; }

        private ObservableCollection<PickUpProductMaster> _ListPickup;
        public Command RefreshCommand { get; set; }
        public ICommand NavigateAddPickUpCommand { get; set; }

        public ICommand EditPickupCommand { get; set; }
        public ICommand DeletePickupCommand { get; set; }

        public ObservableCollection<PickUpProductMaster> ListPickup
        {
            get { return _ListPickup; }
            set { SetProperty(ref _ListPickup, value); }
        }

        public Command LoadItemsCommand { get; set; }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        #endregion

        #region Constructors

        public PickupViewModel(INavigation navigation)
        {
            Navigation = navigation;

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            NavigateAddPickUpCommand = new Command(NavigateAddPickUpAsync);

            RefreshCommand = new Command(async () => LoadPickup());

            EditPickupCommand = new Command<PickUpProductMaster>(EditPickupAsync);
            DeletePickupCommand = new Command<PickUpProductMaster>(DeletePickupAsync);

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "RefreshPagePickup");

            MessagingCenter.Subscribe<App>((App)Application.Current, "RefreshPagePickup", (sender) =>
            {
                LoadPickup();
            });
        }

        private async void EditPickupAsync(PickUpProductMaster obj)
        {
            await Navigation.PushAsync(new EditPickUPPage(obj));
        }

        private async void DeletePickupAsync(PickUpProductMaster obj)
        {
            if (obj == null)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is null!" };
                await PopupNavigation.Instance.PushAsync(dialog);

                return;
            }

            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Delete pickup Name:=" + obj.PickUpName, "Info");

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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/CreatePickUpMaster", string.Empty));

                obj.IsDelete = 1;

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));

                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);

                MessagingCenter.Send<App>((App)Application.Current, "Loading");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();


                    var dataList = JsonConvert.DeserializeObject<ResuftPickUpModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
                    }

                    if (dataList != null)
                    {
                        if (dataList.code == 200)
                        {
                            MessagingCenter.Send<App>((App)Application.Current, "RefreshPagePickup");

                            try
                            {
                                await PopupNavigation.Instance.PopAsync();
                                await PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception)
                            {

                            }

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = dataList.message };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.message };
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
                logger.Error("Delete pickup error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                ShowLoading = false;
            }
        }

        private async void NavigateAddPickUpAsync()
        {
            await Navigation.PushAsync(new AddPickUPPage());
        }

        async Task ExecuteLoadItemsCommand()
        {
            LoadPickup();
        }

        #endregion

        #region Operators

        public partial class ResuftPickupModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
            public List<PickUpProductMaster> ListData { get; set; }
        }

        async Task LoadPickup()
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Load pickup.", "Info");

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

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/GetPickupMaster");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listPickup = JsonConvert.DeserializeObject<ResuftPickupModel>(content);

                    if (listPickup.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    ListPickup = new ObservableCollection<PickUpProductMaster>(listPickup.ListData);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Load pickup Error: " + ex.Message.ToString() + ".", "Error");

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
