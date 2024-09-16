using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using AppName.Model;
using AppName.Model.Pickup;
using System.Threading.Tasks;

namespace AppName
{
    public class EditPickupViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private PickUpProductMaster _dataModel;
        public PickUpProductMaster DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ICommand ConfirmCommand { get; set; }


        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public EditPickupViewModel(INavigation navigation, PickUpProductMaster model)
        {
            ConfirmCommand = new Command(ConfirmAsync);

            Navigation = navigation;

            DataModel = model;
        }

        private async void ConfirmAsync()
        {
            if (DataModel == null)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is null!" };
                await PopupNavigation.Instance.PushAsync(dialog);

                return;
            }

            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Edit pickup Name:=" + DataModel.PickUpName, "Info");

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

                var requestJson = new StringContent(JsonConvert.SerializeObject(DataModel));

                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);

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

                            try
                            {
                                await Navigation.PopAsync();
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
                logger.Error("Edit pickup Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                ShowLoading = false;
            }
        }
    }
}
