using AppName.CustomRenderer;
using AppName.Model;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using NLog;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using Xamarin.Forms;

namespace AppName
{
    public partial class App : Application
    {
        private MenuOnePage mPage;

        public App()
        {
            InitializeComponent();

            OneSignal.Current.StartInit("597d7a51-76a7-4cfc-8ed9-0f0f03164b02")
                .HandleNotificationReceived(OnHandleNotificationReceived)
                .HandleNotificationOpened(OnHandleNotificationOpened)
                .InFocusDisplaying(OSInFocusDisplayOption.InAppAlert).EndInit();

            Resources["DefaultStringResources"] = new Resx.AppResources();

            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                logger.Info("Open application.", "Info");

                if (ResourceHelper.CheckLicenseAvailable())
                {
                    CheckConnectInternet.IsConnectedNotClearCookieNotLogin();

                    Application.Current.Properties["IsLogin"] = false;
                    Application.Current.Properties["UserName"] = "";
                    Application.Current.Properties["RoleID"] = "";
                    Application.Current.Properties["Token"] = "";
                    Application.Current.Properties["DateExpires"] = "";

                    MainPage = new NavigationPage(new LoginFrm());
                }
                else
                {
                    var licenseModel = RealmHelper.GetLicense();
                    if (licenseModel == null)
                    {
                        licenseModel = new LicenseModel();
                    }
                    licenseModel.IsActive = true; // đã được kích hoạt
                    RealmHelper.UpdateModel(licenseModel, true);

                    MainPage = new NavigationPage(new LoginFrm());

                    ReturnLicsence();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
                PopupNavigation.Instance.PushAsync(dialog);
            }
        }

        /// <summary>
        /// Event khi ấn chạm mở thông báo
        /// </summary>
        /// <param name="result"></param>
        private void OnHandleNotificationOpened(OSNotificationOpenedResult result)
        {

        }

        /// <summary>
        /// Event khi nhận được thông báo
        /// </summary>
        /// <param name="notification"></param>
        private void OnHandleNotificationReceived(OSNotification notification)
        {

        }

        private async void ReturnLicsence()
        {
            var listCurrentPopup = PopupNavigation.Instance.PopupStack?.Select(q => q.ToString().Split('.').LastOrDefault()).ToArray();
            if (!(listCurrentPopup?.Length > 0 && listCurrentPopup.Contains(nameof(NotificationLicensePopup))))
            {
                var dialog = new NotificationLicensePopup();
                await PopupNavigation.Instance.PushAsync(dialog);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            //DependencyService.Get<IDevice>().ShowAlert("Only Retail Staff can release document. You d Only Retail Staff can release document. ");

            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                logger.Info("On resume application.", "Info");

                var check = CheckConnectInternet.IsConnectedNotClearCookie();

                if (check == true)
                {
                    if (!ResourceHelper.CheckLicenseAvailable())
                    {
                        // Khi chưa kích hoạt, request api kiểm tra. Sau đó, update lại LicenseModel localDB
                        ReturnLicsence();
                    }

                    base.OnResume();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

                base.OnResume();
            }
        }
    }
}
