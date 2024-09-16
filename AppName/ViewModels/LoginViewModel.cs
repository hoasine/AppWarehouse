using System.Collections.ObjectModel;

using System.Globalization;
using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using AppName.Model;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Plugin.Connectivity;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using NLog;
using NLog.Config;

namespace AppName
{
    public class LoginViewModel : BaseViewModel
    {

        protected INavigation Navigation { get; private set; }

        public Command LoginClickCommand { get; set; }
        public ICommand ForgotPassword { get; set; }
        public ICommand LoadUserStoreExcute { get; set; }
        public ICommand RefreshUserStoreCommand { get; set; }
        public ICommand ConfigCommand { get; set; }

        private UserLSRetailConfigModel _UserConfig;
        public UserLSRetailConfigModel UserConfig
        {
            get => _UserConfig;

            set => SetProperty(ref _UserConfig, value);
        }

        private string _Password;
        public string Password
        {
            get => _Password;

            set => SetProperty(ref _Password, value);
        }

        public class UserLSRetailConfigModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string URLApi { get; set; }
            public string LisenceKey { get; set; }
            public string StoreNo { get; set; }
            public string UserStore { get; set; }
        }

        private ObservableCollection<UserStoreModel> _UserStore;
        public ObservableCollection<UserStoreModel> UserStore
        {
            get { return _UserStore; }
            set { SetProperty(ref _UserStore, value); }
        }

        private string _lbMess;
        public string LBMess
        {
            get => _lbMess;

            set => SetProperty(ref _lbMess, value);
        }

        private bool _isMess;
        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        private UserStoreModel _SelectedUserStore;
        public UserStoreModel SelectedUserStore
        {
            get { return _SelectedUserStore; }
            set { SetProperty(ref _SelectedUserStore, value); }
        }

        public Command LoadItemsCommand { get; set; }

        public LoginViewModel(INavigation navigation)
        {
            Navigation = navigation;
            LoginClickCommand = new Command(LoginClickAsync);
            ForgotPassword = new Command(ForgotPasswordCommand);
            ConfigCommand = new Command(ConfigAsync);
            RefreshUserStoreCommand = new Command(RefreshUserStore);
            LoadItemsCommand = new Command(async () => await checkUser(false));

            //Password = "watson2024";
        }

        public async Task checkUser(bool checkInternet)
        {
            //Nếu điền biến name trong config vào getlog thì se lấy theo file confign android
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                ShowLoading = true;

                await Task.Delay(100);

                logger.Info("Load user store in login", "Info");

                if (checkInternet == true)
                {
                    if (CheckConnectInternet.IsConnectedNotClearCookieNotLogin() == false)
                    {
                        return;
                    }
                }

                var realm = RealmHelper.Instance;

                var checkData = realm.All<UserLSRetailConfig>().FirstOrDefault();

                if (checkData != null)
                {
                    UserConfig = new UserLSRetailConfigModel()
                    {
                        Password = checkData.Password,
                        StoreNo = checkData.StoreNo,
                        UserStore = checkData.UserStore,
                        URLApi = checkData.URLApi,
                        LisenceKey = checkData.LisenceKey,
                        UserName = checkData.UserName
                    };

                    var url = RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi;

                    Uri uriResult;
                    bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                        && uriResult.Scheme == Uri.UriSchemeHttp;

                    if (result == false)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(checkData.LisenceKey))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input LisenceKey config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(checkData.URLApi))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(checkData.UserName))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input UserName config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(checkData.Password))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Password config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (!string.IsNullOrEmpty(checkData.UserName))
                    {
                        LoadUserStoreCommand(checkData.UserName);
                    }
                }
                else
                {
                    UserConfig = new UserLSRetailConfigModel();

                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please configure connection with LSRetail" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message.ToString(), "Error");

                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog);
            }
            finally
            {
                ShowLoading = false;
            }
        }

        private async void ForgotPasswordCommand(object obj)
        {
            await Navigation.PushAsync(new PasswordRecoveryPage());
        }

        public class ResuftLSRetailUserModel
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public LSRetailUserModel Data { get; set; }
        }

        public class LSRetailUserModel
        {
            public string UserAdmin { get; set; }
            public string PasswordAdmin { get; set; }
            public string StoreNo { get; set; }
        }


        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set
            {
                _showLoading = value;
                OnPropertyChanged();
            }
        }

        private void RefreshUserStore()
        {
            checkUser(true);
        }

        private async void ConfigAsync()
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Config LS Retail info.", "Info");

            var dialog = new ConfigPage();
            var viewModel = new ConfigViewModel();

            viewModel.UpdatePopup += async (send, data) =>
            {
                try
                {
                    ShowLoading = true;

                    await Task.Delay(100);

                    if (CheckConnectInternet.IsConnectedNotClearCookieNotLogin() == false)
                    {
                        return;
                    }

                    if (data != null)
                    {
                        UserConfig.UserName = data.UserName;
                        UserConfig.UserStore = data.UserStore;
                        UserConfig.URLApi = data.URLApi;
                        UserConfig.Password = data.Password;
                        UserConfig.StoreNo = data.StoreNo;
                        UserConfig.LisenceKey = data.LisenceKey;
                    }

                    if (string.IsNullOrEmpty(data.LisenceKey))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input LisenceKey config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(data.URLApi))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(data.UserName))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input UserName config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    if (string.IsNullOrEmpty(data.Password))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Password config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    var url = data.URLApi;

                    Uri uriResult;
                    bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                        && uriResult.Scheme == Uri.UriSchemeHttp;

                    if (result == false)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                        return;
                    }

                    try
                    {
                        PopupNavigation.Instance.PopAsync();
                        PopupNavigation.Instance.PopAsync();
                        PopupNavigation.Instance.PopAsync();
                        PopupNavigation.Instance.PopAsync();
                        PopupNavigation.Instance.PopAsync();
                    }
                    catch (Exception) { }

                    HttpClient client = new HttpClient();

                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/user/GetLSRetailUser?UserNamesLSRetail=" + UserConfig.UserName + "&PasswordLSRetail=" + UserConfig.Password, string.Empty));

                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var dataList = JsonConvert.DeserializeObject<ResuftLSRetailUserModel>(content);
                        if (dataList != null)
                        {
                            if (dataList.Data != null)
                            {
                                UserConfig = new UserLSRetailConfigModel()
                                {
                                    Password = data.Password,
                                    StoreNo = data.StoreNo,
                                    UserStore = data.UserStore,
                                    URLApi = data.URLApi,
                                    LisenceKey = data.LisenceKey,
                                    UserName = data.UserName
                                };

                                UserConfig.StoreNo = dataList.Data?.StoreNo;
                                LoadUserStoreCommand(UserConfig.UserName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                    await PopupNavigation.Instance.PushAsync(dialog, false);
                }
                finally
                {
                    ShowLoading = false;
                }
            };

            dialog.BindingContext = viewModel;

            await PopupNavigation.Instance.PushAsync(dialog);
        }

        private async void LoginClickAsync()
        {
            string userName = UserConfig.UserName;
            string uRLApi = UserConfig.URLApi;
            string lisenceKey = UserConfig.LisenceKey;
            string passwordLS = UserConfig.Password;
            string userStore = SelectedUserStore?.RetailID;

            if (string.IsNullOrEmpty(lisenceKey))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input LicenseKey config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            if (string.IsNullOrEmpty(uRLApi))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            if (string.IsNullOrEmpty(userName))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input UserName config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            if (string.IsNullOrEmpty(passwordLS))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input Password config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            if (string.IsNullOrEmpty(userStore))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input UserStore." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input password user store." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }
            var url = UserConfig.URLApi;

            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttp;

            if (result == false)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }

            CheckLogin(userName, passwordLS);
        }

        protected async void CheckLogin(string username, string password)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                ShowLoading = true;

                await Task.Delay(100);

                logger.Info("User " + username + " Logined.", "Info");

                if (CheckConnectInternet.IsConnectedNotClearCookieNotLogin() == false)
                {
                    return;
                }

                if (string.IsNullOrEmpty(SelectedUserStore.RetailID))
                {
                    LBMess = "Please select store user information.";
                    IsMess = true;

                    return;
                }

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("UserNames", username);
                client.DefaultRequestHeaders.Add("Password", password);

                var data = new
                {
                    UserNames = username,
                    Password = password
                };

                // lấy playerID sau khi đăng ký push notification OneSignal
                //var oneSignalResult = await OneSignal.Current.IdsAvailableAsync();

                IDevice device = DependencyService.Get<IDevice>();
                string deviceIdentifier = device.GetIdentifier();


                var stringContent = new StringContent(data.ToString(), UnicodeEncoding.UTF8, "application/json");


                //System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                //System.Net.NetworkInformation.PingReply rep = await p.SendPingAsync("10.46.12.133");
                //if (rep.Status != System.Net.NetworkInformation.IPStatus.Success)
                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/user/loginInput?UserNames={0}&Password={1}&ImelDeveice={2}&UserRetail={3}&passwordUser={4}&Lisence={5}",
                    username, password, deviceIdentifier, SelectedUserStore.RetailID.ToUpper(), Password, Uri.EscapeDataString(UserConfig.LisenceKey.ToString())));

                var response = await client.PostAsync(uri, stringContent);

                var mess = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var datalogin = JsonConvert.DeserializeObject<ViewModels.BarCode.Model.LoginModel>(content);

                    if (datalogin.Status == "200")
                    {
                        Application.Current.Properties["IsLogin"] = true;

                        Application.Current.Properties["UserStore"] = SelectedUserStore.RetailID.ToUpper();
                        Application.Current.Properties["RetailName"] = SelectedUserStore.RetailName.ToUpper();
                        Application.Current.Properties["Permission"] = SelectedUserStore.Permission.ToUpper();

                        Application.Current.Properties["LSRetailName"] = username;
                        Application.Current.Properties["PasswordLSRetail"] = password;

                        //StoreNo
                        Application.Current.Properties["UserName"] = datalogin.UserName.ToUpper();
                        Application.Current.Properties["RoleID"] = datalogin.RoleID;
                        Application.Current.Properties["Token"] = datalogin.Token;
                        Application.Current.Properties["DateExpires"] = datalogin.DateExpires;

                        var listPermission = datalogin.PermissionList.Select(q => new LocalPermissionModel
                        {
                            KeyPermission = q.KeyPermission,
                            Role = q.Role
                        });

                        RealmHelper.UpdateModel(listPermission);

                        await Application.Current.SavePropertiesAsync();
                        Application.Current.MainPage = new NavigationPage(new MenuOnePage());
                        //Application.Current.MainPage = new MenuBottom();
                        //Application.Current.MainPage = new NavigationPage(new RootMasterDetailPage());
                    }
                    else
                    {
                        Application.Current.Properties["IsLogin"] = false;
                        Application.Current.Properties["UserStore"] = "";
                        Application.Current.Properties["RetailName"] = "";
                        Application.Current.Properties["Permission"] = "";

                        Application.Current.Properties["LSRetailName"] = "";
                        Application.Current.Properties["PasswordLSRetail"] = "";

                        await Application.Current.MainPage.DisplayAlert("Notification !", datalogin.Mess, "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Notification !", mess.ToString(), "OK");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

                await Application.Current.MainPage.DisplayAlert("Notification !", "Error connecting to server. Contact IT for more information.", "OK");
            }
            finally
            {
                ShowLoading = false;
            }
        }

        public async void LoadUserStoreCommand(string obj)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                HttpClient client = new HttpClient();
                //var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                //client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/user/GetUserStore?storecode=" + obj);

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var countopo = JsonConvert.DeserializeObject<UserStoreModel[]>(content).OrderBy(s => s.RetailID);

                    if (countopo.Count() == 0)
                    {
                        UserStore = new ObservableCollection<UserStoreModel>();

                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Not found users with account " + obj + "." };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        UserStore = new ObservableCollection<UserStoreModel>(countopo);
                    }
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "The database is not accessible. Please check url: " + RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi };
                    await PopupNavigation.Instance.PushAsync(dialog);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }
    }
}