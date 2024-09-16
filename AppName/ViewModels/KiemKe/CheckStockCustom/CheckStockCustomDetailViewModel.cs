﻿using AppName.Model;
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
    public class CheckStockCustomDetailViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public ICommand ConfirmCommand { get; set; }
        public ICommand SendFileCommand { get; set; }
        public Command CloneCommand { get; set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _IsSendStock;
        public bool IsSendStock
        {
            get { return _IsSendStock; }
            set { SetProperty(ref _IsSendStock, value); }
        }

        public bool _IsSendReport;
        public bool IsSendReport
        {
            get { return _IsSendReport; }
            set { SetProperty(ref _IsSendReport, value); }
        }

        public bool _IsSendClose;

        public bool IsSendClose
        {
            get { return _IsSendClose; }
            set { SetProperty(ref _IsSendClose, value); }
        }

        public bool _IsDuplicate;
        public bool IsDuplicate
        {
            get { return _IsDuplicate; }
            set { SetProperty(ref _IsDuplicate, value); }
        }

        public CheckStockCustomDetailViewModel(INavigation navigation, StockCountModel itemModel)
        {
            Navigation = navigation;
            ConfirmCommand = new Command(ConfirmAsync);
            SendFileCommand = new Command(SendFileCommandASync);
            CloneCommand = new Command(CloneAsync);
            DataModel = itemModel;

            if (DataModel.Release != 2)
            {
                IsSendStock = true;
            }
            else
            {
                IsSendStock = false;
            }

            IsDuplicate = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "DuplicateStockCustom"
           && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            IsSendClose = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "SendCloseStockCustom"
           && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            IsSendReport = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "SendReportStockCustom"
               && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));
        }

        public partial class SendCheckStockCustomModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public StockCountModel ListData { get; set; }
        }

        public class CloneStockCountModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
        }

        private async void CloneAsync(object obj)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (IsDuplicate == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission function for this function.", "OK");
                    return;
                }

                if (string.IsNullOrEmpty(DataModel?.DocumentNo))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "DocumentNo is null" };
                    PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                if (DataModel.Release == 0)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Release' in " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                if (DataModel.Release == 2)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status 'closed' cannot be duplicated. in " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You do not have permission for this function(Staff). With " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var username = Application.Current.Properties["UserStore"]?.ToString();

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/CloneStockCount?DocumentNo=" + DataModel.DocumentNo + "&UserCreate=" + username, string.Empty));

                var response = await client.GetAsync(uri);

                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var dataList = JsonConvert.DeserializeObject<CloneStockCountModel>(content);

                    if (dataList.Code == 200)
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "ReloadStockCustomPage");

                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = content };
                    await PopupNavigation.Instance.PushAsync(dialog);
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

        private async void ConfirmAsync(object obj)
        {
            try
            {
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (IsSendClose == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission function for this function.", "OK");
                    return;
                }

                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("You are sure the data is correct. Counting slip will be sent to LS Retail system?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            viewModel.ShowLoading = true;

                            await Task.Delay(1);

                            if (string.IsNullOrEmpty(DataModel.DocumentNo))
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "DocumentNo is null" };
                                await PopupNavigation.Instance.PushAsync(dialog);
                                return;
                            }

                            if (DataModel.Release != 1)
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Release' in " + DataModel.DocumentNo + "." };
                                await PopupNavigation.Instance.PushAsync(dialog);
                                return;
                            }

                            var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                            if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You do not have permission Staff for this function. With " + DataModel.DocumentNo + "." };
                                await PopupNavigation.Instance.PushAsync(dialog);
                                return;
                            }

                            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Authorization = authHeader;

                            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/SendCheckStockCustomReport", string.Empty));

                            DataModel.IsSend = 1;

                            var requestJson = new StringContent(JsonConvert.SerializeObject(DataModel));
                            requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var response = await client.PostAsync(uri, requestJson);

                            string content = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                var dataList = JsonConvert.DeserializeObject<SendCheckStockCustomModel>(content);

                                if (dataList.Active == false)
                                {
                                    Application.Current.Properties["IsLogin"] = false;

                                    Application.Current.MainPage = new NavigationPage(new LoginFrm());

                                    await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                                    return;
                                }

                                if (dataList.Code == 200)
                                {
                                    MessagingCenter.Send<App>((App)Application.Current, "ReloadStockCustomPage");

                                    try
                                    {
                                        await PopupNavigation.Instance.PopAsync();
                                        await PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                                    await PopupNavigation.Instance.PushAsync(dialog);
                                }
                                else
                                {
                                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                                    await PopupNavigation.Instance.PushAsync(dialog);
                                }
                            }
                            else
                            {
                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = content };
                                await PopupNavigation.Instance.PushAsync(dialog);
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
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async void SendFileCommandASync(object obj)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (IsSendReport == false)
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission function for this function.", "OK");
                    return;
                }

                if (string.IsNullOrEmpty(DataModel.DocumentNo))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "DocumentNo is null" };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                if (DataModel.Release != 1)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Status must be equal to 'Release' in " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You do not have permission STAFF for this function. With " + DataModel.DocumentNo + "." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/SendCheckStockCustomReport", string.Empty));

                DataModel.IsSend = 0;

                var requestJson = new StringContent(JsonConvert.SerializeObject(DataModel));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var dataList = JsonConvert.DeserializeObject<SendCheckStockCustomModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "ReloadStockCustomPage");

                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception)
                        {

                        }

                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = content };
                    await PopupNavigation.Instance.PushAsync(dialog);
                }
            }
            catch (Exception ex)
            {
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
