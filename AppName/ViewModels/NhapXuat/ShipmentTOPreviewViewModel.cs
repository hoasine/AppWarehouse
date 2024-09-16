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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class ShipmentTOPreviewViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private TOHeaderPreviewModel _dataModel;
        public TOHeaderPreviewModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _visibleNotification;
        public bool VisibleNotification
        {
            get { return _visibleNotification; }
            set { SetProperty(ref _visibleNotification, value); }
        }

        public ICommand ConfirmCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ShipmentTOPreviewViewModel(INavigation navigation, TOHeaderPreviewModel itemModel)
        {
            Navigation = navigation;

            ConfirmCommand = new Command(ConfirmAsync);
            CloseCommand = new Command(CloseAsync);

            DataModel = itemModel;
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

        //release POST /api/topos/RelaseTO
        private async void ConfirmAsync()
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

                            await Task.Delay(1);

                            var checkPostshipTO = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "PostshipTO"
                              && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                            if (checkPostshipTO == false)
                            {
                                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                                return;
                            }

                            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Authorization = authHeader;

                            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/PostShipment", string.Empty));

                            var modelInput = new TransferShipmentHeader();
                            modelInput.documentNoField = _dataModel.DocumentNo;
                            modelInput.statusField = _dataModel.Status;
                            modelInput.transferfromCodeField = _dataModel.TransferfromCode;
                            modelInput.dateField = Convert.ToDateTime(_dataModel.PostingDate.Substring(6, 4) + "-" + _dataModel.PostingDate.Substring(3, 2) + "-" + _dataModel.PostingDate.Substring(0, 2));
                            modelInput.transferShipmentLineField = new List<TransferShipmentLine>();

                            var requestJson = new StringContent(JsonConvert.SerializeObject(modelInput));
                            requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var response = await client.PostAsync(uri, requestJson);

                            if (response.IsSuccessStatusCode)
                            {
                                string content = await response.Content.ReadAsStringAsync();

                                var dataList = JsonConvert.DeserializeObject<PostShipment_Model>(content);

                                if (dataList.Active == false)
                                {
                                    try
                                    {
                                        await PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception) { }

                                    Application.Current.Properties["IsLogin"] = false;

                                    Application.Current.MainPage = new NavigationPage(new LoginFrm());

                                    Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                                    return;
                                }

                                if (dataList.Code == 200)
                                {
                                    Uri uriCheckStatus = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/SendTOShipment", string.Empty));

                                    var modelCheckStatus = new TOLineModel();
                                    modelCheckStatus.DocumentNo = _dataModel.DocumentNo;

                                    var requestJsonCheckStatus = new StringContent(JsonConvert.SerializeObject(modelCheckStatus));
                                    requestJsonCheckStatus.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                    var responseCheckStatus = await client.PostAsync(uriCheckStatus, requestJsonCheckStatus);

                                    if (responseCheckStatus.IsSuccessStatusCode)
                                    {
                                        string contentAgain = await responseCheckStatus.Content.ReadAsStringAsync();

                                        var dataListAgain = JsonConvert.DeserializeObject<PostShipment_Model>(contentAgain);

                                        try
                                        {
                                            await PopupNavigation.Instance.PopAsync();
                                            await PopupNavigation.Instance.PopAsync();
                                        }
                                        catch (Exception) { }

                                        if (dataListAgain.Code == 200)
                                        {
                                            MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");
                                            MessagingCenter.Send<App>((App)Application.Current, "ClosePageTOFromShipmentPreview");

                                            try
                                            {
                                                await Navigation.PopAsync();
                                            }
                                            catch (Exception) { }

                                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Posted shipment TO:=" + _dataModel.DocumentNo + " successfully" };
                                            await PopupNavigation.Instance.PushAsync(dialog);
                                        }
                                        else
                                        {
                                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataListAgain.Content };
                                            PopupNavigation.Instance.PushAsync(dialog, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (dataList.Content == "Sorry, we just updated this page. Reopen it, and try again.")
                                    {
                                        var responseAgain = await client.PostAsync(uri, requestJson);

                                        if (responseAgain.IsSuccessStatusCode)
                                        {
                                            try
                                            {
                                                await PopupNavigation.Instance.PopAsync();
                                            }
                                            catch (Exception) { }

                                            string contentAgain = await responseAgain.Content.ReadAsStringAsync();

                                            var dataListAgain = JsonConvert.DeserializeObject<PostShipment_Model>(contentAgain);

                                            if (dataListAgain.Code == 200)
                                            {
                                                Thread.Sleep(5000);

                                                Uri uriCheckStatus = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/SendTOShipment", string.Empty));

                                                var modelCheckStatus = new TOLineModel();
                                                modelCheckStatus.DocumentNo = _dataModel.DocumentNo;

                                                var requestJsonCheckStatus = new StringContent(JsonConvert.SerializeObject(modelCheckStatus));
                                                requestJsonCheckStatus.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                                var responseCheckStatus = await client.PostAsync(uriCheckStatus, requestJsonCheckStatus);

                                                if (responseCheckStatus.IsSuccessStatusCode)
                                                {
                                                    string contentCheckStatus = await responseCheckStatus.Content.ReadAsStringAsync();

                                                    var dataCheckStatus = JsonConvert.DeserializeObject<PostShipment_Model>(contentCheckStatus);

                                                    if (dataCheckStatus.Code == 200)
                                                    {
                                                        MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");

                                                        try
                                                        {
                                                            await PopupNavigation.Instance.PopAsync();
                                                            await PopupNavigation.Instance.PopAsync();
                                                        }
                                                        catch (Exception) { }

                                                        MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");
                                                        MessagingCenter.Send<App>((App)Application.Current, "ClosePageTOFromShipmentPreview");

                                                        try
                                                        {
                                                            await Navigation.PopAsync();
                                                        }
                                                        catch (Exception) { }

                                                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Posted shipment TO:=" + _dataModel.DocumentNo + " successfully" };
                                                        await PopupNavigation.Instance.PushAsync(dialog);
                                                    }
                                                    else
                                                    {
                                                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataCheckStatus.Content };
                                                        PopupNavigation.Instance.PushAsync(dialog, false);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataListAgain.Content };
                                                PopupNavigation.Instance.PushAsync(dialog, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            await PopupNavigation.Instance.PopAsync();
                                        }
                                        catch (Exception) { }

                                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                                        PopupNavigation.Instance.PushAsync(dialog, false);
                                    }
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

                return;
            }
            finally
            {

            }
        }

        private async void CloseAsync()
        {
            MessagingCenter.Send<App>((App)Application.Current, "RefreshPageTO");
            MessagingCenter.Send<App>((App)Application.Current, "ClosePageTOFromShipmentPreview");

            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception) { }
        }
    }
}
