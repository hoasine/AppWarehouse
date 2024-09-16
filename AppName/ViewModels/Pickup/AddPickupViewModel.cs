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
using AppName.Model.Pickup;

namespace AppName
{
    public class AddPickupViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }


        #region field page

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private DateTime _DateCreate = DateTime.Today;
        public DateTime DateCreate
        {
            get { return _DateCreate; }
            set { SetProperty(ref _DateCreate, value); }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        #endregion

        public ICommand ConfirmCommand { get; set; }

        public AddPickupViewModel(INavigation navigation)
        {
            ConfirmCommand = new Command(ConfirmAsync);

            Navigation = navigation;
        }
        private async void ConfirmAsync(object obj)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

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

                var data = new PickUpProductMaster();
                data.ID = Guid.NewGuid();
                data.DateCreate = DateTime.Now;
                data.PickUpName = Description;
                data.Status = "New";
                data.IsDelete = 0;

                logger.Info("Create pickup Name:=" + Description, "Info");

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/CreatePickUpMaster");

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var requestJson = new StringContent(JsonConvert.SerializeObject(data));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(uri, requestJson);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftPickUpModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
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

                            try
                            {
                                await Navigation.PopAsync();
                            }
                            catch (Exception)
                            {
                            }

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "Create a successfuly pickup sheet." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.message };
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
                logger.Error("Create pickup Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog);
            }
            finally
            {
                ShowLoading = false;
            }
        }
    }
}
