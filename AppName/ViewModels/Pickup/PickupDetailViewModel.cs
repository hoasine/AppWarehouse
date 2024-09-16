using AppName.Model;
using AppName.Model.Pickup;
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
    public class PickupDetailViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private PickUpProductMaster _dataModel;
        public PickUpProductMaster DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }
        public ICommand ConfirmCommand { get; set; }

        public PickupDetailViewModel(INavigation navigation, PickUpProductMaster itemModel)
             : base(listenCultureChanges: true)
        {
            Navigation = navigation;
            ConfirmCommand = new Command(ConfirmAsync);
            DataModel = itemModel;
        }

        public partial class ResuftPickUpModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public bool Active { get; set; }
        }

        private async void ConfirmAsync(object obj)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Send pickup Json:=" + JsonConvert.SerializeObject(obj) + ".", "Info");

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("You are sure the data is correct. Pickup slip will be sent to LS Retail system?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            viewModel.ShowLoading = true;

                            await Task.Delay(1);

                            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Authorization = authHeader;

                            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/SendPickupReport", string.Empty));

                            var requestJson = new StringContent(JsonConvert.SerializeObject(DataModel));
                            requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var response = await client.PostAsync(uri, requestJson);
                            MessagingCenter.Send<App>((App)Application.Current, "Loading");
                            string content = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var dataList = JsonConvert.DeserializeObject<ResuftPickUpModel>(content);

                                if (dataList.Active == false)
                                {
                                    Application.Current.Properties["IsLogin"] = false;

                                    Application.Current.MainPage = new NavigationPage(new LoginFrm());

                                    await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                                    return;
                                }

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
                                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.message };
                                    await PopupNavigation.Instance.PushAsync(dialog);
                                }


                                logger.Info("Send pickup: " + dataList.message, "Info");
                            }
                            else
                            {
                                logger.Error("Send pickup: " + content, "Error");

                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = content };
                                await PopupNavigation.Instance.PushAsync(dialog);
                            }
                        }
                    }
                    catch (Exception exx)
                    {
                        logger.Error("Send pickup Error: " + exx.Message.ToString() + ".", "Error");

                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                    finally
                    {
                        viewModel.ShowLoading = false;
                    }
                };

                dialogCheck.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialogCheck);
            }
            catch (Exception ex)
            {
                logger.Error("Send pickup Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
                MessagingCenter.Send<App>((App)Application.Current, "Loading");
            }
            finally
            {
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
