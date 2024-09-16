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
    public class ReceiveTODetailViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        public ICommand ConfirmCommand { get; set; }

        private TOHeaderModel _dataModel;
        public TOHeaderModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ReceiveTODetailViewModel(INavigation navigation, TOHeaderModel itemModel)
             : base(listenCultureChanges: true)
        {
            Navigation = navigation;
            ConfirmCommand = new Command(ConfirmAsync);
            DataModel = itemModel;
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
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("You are sure the data is correct. Transfer order will be sent to LS Retail system?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            viewModel.ShowLoading = true;

                            await Task.Delay(100);

                            var checkPostReceiveTO = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "PostReceiveTO"
                             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                            if (checkPostReceiveTO == false)
                            {
                                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                                return;
                            }

                            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Authorization = authHeader;

                            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/PostReceiptTO", string.Empty));

                            var modelInput = new TransferReceiveHeader();
                            modelInput.documentNoField = _dataModel.DocumentNo;
                            modelInput.transfertoCodeField = _dataModel.TransfertoCode;
                            modelInput.transferReceiveLineField = new List<TransferReceiveLine>();

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
                                    MessagingCenter.Send<App>((App)Application.Current, "RefreshPageReceivingTO");

                                    try
                                    {
                                        PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception) { }

                                    await Task.Delay(100);

                                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                                    PopupNavigation.Instance.PushAsync(dialog, false);
                                }
                                else
                                {
                                    try
                                    {
                                        PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception) { }

                                    Application.Current.MainPage.DisplayAlert("Notification !", dataList.Content, "OK");
                                }
                            }
                        }
                    }
                    catch (Exception exx)
                    {
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
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public partial class PostShipment_Model
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public TransferReceiveHeader ListData { get; set; }
        }


        public partial class TransferReceiveHeader
        {

            public string documentNoField;

            public string transfertoCodeField;

            public System.DateTime dateField;

            public string statusField;

            public List<TransferReceiveLine> transferReceiveLineField;


        }

        public partial class TransferReceiveLine
        {

            public string documentNoField;

            public int lineNoField;

            public string itemNoField;

            public string unitofMeasureCodeField;

            public decimal qtytoReceiveField;
        }
    }
}
