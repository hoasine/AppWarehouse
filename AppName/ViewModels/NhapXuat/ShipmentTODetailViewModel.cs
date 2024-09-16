using AppName.Model;
using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class ShipmentTODetailViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private TOHeaderModel _dataModel;
        public TOHeaderModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }
        public ICommand ConfirmCommand { get; set; }

        private string _IsRelease;
        public string IsRelease
        {
            get { return _IsRelease; }
            set { SetProperty(ref _IsRelease, value); }

        }
        public ShipmentTODetailViewModel(INavigation navigation, TOHeaderModel itemModel)
             : base(listenCultureChanges: true)
        {
            Navigation = navigation;
            ConfirmCommand = new Command(ConfirmAsync);
            DataModel = itemModel;

            if (DataModel.Status == "Open")
            {
                IsRelease = "RELEASE";
            }
            else if (DataModel.Status == "Release")
            {
                IsRelease = "REOPEN";
            }
            else
            {
                IsRelease = "UNKNONW";
            }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
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

                if (string.IsNullOrEmpty(IsRelease) || IsRelease == "UNKNONW")
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Reload Transfer Order TO:=" + DataModel.DocumentNo };
                    await PopupNavigation.Instance.PushAsync(dialog);
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/RelaseTO?TransferNO=" + _dataModel.DocumentNo, string.Empty));

                var modelInput = new object();

                var requestJson = new StringContent(JsonConvert.SerializeObject(modelInput));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<PostShipment_Model>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {
                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");

                        IsRelease = "RELEASE";

                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else if (dataList.Code == 201)
                    {
                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");

                        IsRelease = "REOPEN";

                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

            }
            finally
            {
                ShowLoading = false;
            }
        }

        public partial class PostShipment_Model
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public TransferShipmentHeader ListData { get; set; }
        }

        public partial class TransferShipmentHeader
        {
            public string documentNoField;

            public string transferfromCodeField;

            public System.DateTime dateField;

            public string statusField;

            public string toteIdField;

            public List<TransferShipmentLine> transferShipmentLineField;
        }

        public partial class TransferShipmentLine
        {

            public string documentNoField;

            public int lineNoField;

            public string itemNoField;

            public string unitofMeasureCodeField;

            public decimal qtytoShipField;
        }

    }
}
