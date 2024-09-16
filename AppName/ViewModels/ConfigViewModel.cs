using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using AppName.Core;
using AppName.Model;
using MvvmHelpers.Commands;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;

namespace AppName
{
    public class ConfigViewModel : ObservableObject
    {
        public EventHandler<UserLSRetailConfig> ClosePopup;
        public EventHandler<UserLSRetailConfig> UpdatePopup;

        public ICommand CheckLanCommand { get; set; }

        private UserLSRetailConfig _UserConfig;
        public UserLSRetailConfig UserConfig
        {
            get => _UserConfig;

            set => SetProperty(ref _UserConfig, value);
        }

        public ICommand CloseCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearData { get; set; }

        public ConfigViewModel() : base(listenCultureChanges: true)
        {
            CloseCommand = new Command(CloseAsync);
            UpdateCommand = new Command(UpdateAsync);
            ClearData = new Command(ClearDataAsync);

            CheckLanCommand = new Command(CheckLanCommandAsync);

            LoadData();
        }

        private void LoadData()
        {
            var reaml = RealmHelper.Instance;

            var model = reaml.All<UserLSRetailConfig>().FirstOrDefault();

            if (model != null)
            {
                UserConfig = model;
            }
            else
            {
                UserConfig = new UserLSRetailConfig();
            }
        }

        private async void CheckLanCommandAsync()
        {
            var url = UserConfig.URLApi;

            try
            {
                PopupNavigation.Instance.PopAsync();
                PopupNavigation.Instance.PopAsync();
                PopupNavigation.Instance.PopAsync();
                PopupNavigation.Instance.PopAsync();
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception) { }

            if (!string.IsNullOrEmpty(url))
            {
                Uri uriResult;
                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && uriResult.Scheme == Uri.UriSchemeHttp;

                if (result == false)
                {
                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "The API address is malformed. Please check again." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                bool canReach = await CrossConnectivity.Current.IsRemoteReachable(url, 500);

                canReach = true;

                if (canReach == false)
                {
                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Internet or VPN connection failed." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "VPN connection successful." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }
            }
            else
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input URLApi config in LSRetail." };
                PopupNavigation.Instance.PushAsync(dialog, false);
                return;
            }
        }

        private async void ClearDataAsync()
        {
            try
            {
                try
                {
                    PopupNavigation.Instance.PopAsync();
                    PopupNavigation.Instance.PopAsync();
                    PopupNavigation.Instance.PopAsync();
                    PopupNavigation.Instance.PopAsync();
                    PopupNavigation.Instance.PopAsync();
                }
                catch (Exception) { }

                var dialogCheck = new YesNoPage();
                var viewModel = new YesNoViewModel("Temporary data on HH will be deleted. Free up space and speed up your device. Did you confirm the deletion?");
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        if (data == true)
                        {
                            //Đóng popup detail trước
                            await PopupNavigation.Instance.PopAsync();

                            LoadingPopup page1 = new LoadingPopup();
                            await PopupNavigation.Instance.PushAsync(page1);

                            RealmHelper.RemoveAll<ItemPromotionModel>();
                            RealmHelper.RemoveAll<ItemPromotionMultiModel>();
                            RealmHelper.RemoveAll<ItemPromotionPDFMSC3TagModel>();
                            RealmHelper.RemoveAll<ItemPromotionPDFShelfTalkerModel>();
                            RealmHelper.RemoveAll<ItemPromotionPDFShelfTalkerMultiModel>();
                            RealmHelper.RemoveAll<ItemPromotionPDFSmallModel>();
                            RealmHelper.RemoveAll<StockTakeLocalModel>();
                            RealmHelper.RemoveAll<CreateTOHeaderModel>();
                            RealmHelper.RemoveAll<CreateTOModel>();

                            try
                            {
                                PopupNavigation.Instance.PopAsync();
                                PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }

                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "The data has been deleted successfully." };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }

                    }
                    catch (Exception exx)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                };

                dialogCheck.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialogCheck);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog);
            }
        }

        private async void CloseAsync(object obj)
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception) { }
        }

        private async void UpdateAsync(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(UserConfig.UserName))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input user LSRetail." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                if (string.IsNullOrEmpty(UserConfig.Password))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input password LSRetail." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                if (string.IsNullOrEmpty(UserConfig.URLApi))
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input connect to url api." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                var url = UserConfig.URLApi;

                Uri uriResult;
                bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && uriResult.Scheme == Uri.UriSchemeHttp;

                if (result == false)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "The API address is malformed. Please check again." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                    return;
                }

                var reaml = RealmHelper.Instance;

                using (var transaction = reaml.BeginWrite())
                {
                    var getdData = reaml.All<UserLSRetailConfig>().FirstOrDefault();

                    if (getdData != null)
                    {
                        getdData.UserName = UserConfig.UserName;
                        getdData.Password = UserConfig.Password;
                        getdData.StoreNo = "";
                        getdData.UserStore = "";
                        getdData.LisenceKey = UserConfig.LisenceKey;
                        getdData.URLApi = UserConfig.URLApi;

                        reaml.Add(getdData, false);
                        transaction.Commit();

                        UpdatePopup?.Invoke(this, getdData);

                        try
                        {
                            PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        var upModel = new UserLSRetailConfig()
                        {
                            UserName = UserConfig.UserName,
                            Password = UserConfig.Password,
                            LisenceKey = UserConfig.LisenceKey,
                            StoreNo = "",
                            UserStore = "",
                            URLApi = UserConfig.URLApi
                        };

                        reaml.Add(upModel, true);
                        transaction.Commit();

                        UpdatePopup?.Invoke(this, upModel);

                        try
                        {
                            PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }
    }
}
