using AppName.Model;
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
    public class StockCountViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        private CountStockModel _CountStock;
        public CountStockModel CountStock
        {
            get { return _CountStock; }
            set { SetProperty(ref _CountStock, value); }
        }
        public ICommand StockTakeCommand { get; set; }
        public ICommand GapCheckCommand { get; set; }
        public ICommand CycleCountCommand { get; set; }

        public ICommand NavigatePageCommand { get; set; }

        private bool _visibleStockCountPage;
        public bool VisibleStockCountPage
        {
            get { return _visibleStockCountPage; }
            set { SetProperty(ref _visibleStockCountPage, value); }
        }

        private bool _visibleStockTakePage;
        public bool VisibleStockTakePage
        {
            get { return _visibleStockTakePage; }
            set { SetProperty(ref _visibleStockTakePage, value); }
        }

        private bool _visibleGapCheckPage;
        public bool VisibleGapCheckPage
        {
            get { return _visibleGapCheckPage; }
            set { SetProperty(ref _visibleGapCheckPage, value); }
        }

        private bool _visibleStockCountCustomPage;
        public bool VisibleStockCountCustomPage
        {
            get { return _visibleStockCountCustomPage; }
            set { SetProperty(ref _visibleStockCountCustomPage, value); }
        }
        
        private bool _visiblePickupPage;
        public bool VisiblePickupPage
        {
            get { return _visiblePickupPage; }
            set { SetProperty(ref _visiblePickupPage, value); }
        }

        public StockCountViewModel(INavigation navigation)
        {
            NavigatePageCommand = new Command<string>(NavigatePageAsync);

            Navigation = navigation;

            StockTakeCommand = new Command(StockTakeCommandPage);
            GapCheckCommand = new Command(GapCheckCommandPage);
            CycleCountCommand = new Command(CycleCountCommandPage);

            MessagingCenter.Subscribe<App>((App)Application.Current, "RefreshCountStock", (sender) =>
            {
                LoadTotal();
            });

            var listPermission = RealmHelper.Instance.All<LocalPermissionModel>().ToArray();

            VisibleStockCountPage = listPermission.Any(q => q.KeyPermission == "StockCountPage"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            VisibleStockTakePage = listPermission.Any(q => q.KeyPermission == "StockTakePage"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            VisibleGapCheckPage = listPermission.Any(q => q.KeyPermission == "GapCheckPage"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            VisibleStockCountCustomPage = listPermission.Any(q => q.KeyPermission == "StockCountCustomPage"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            VisiblePickupPage = listPermission.Any(q => q.KeyPermission == "PickupPage"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            LoadTotal();
        }

        bool isExecuting = false;

        private async void StockTakeCommandPage()
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;

                if (VisibleStockTakePage == true)
                {
                    await Navigation.PushAsync(new StockTakePage());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                isExecuting = false;
            }
        }

        private async void GapCheckCommandPage()
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;

                if (VisibleStockCountCustomPage == true)
                {
                    await Navigation.PushAsync(new CheckStockCustomPage());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                isExecuting = false;
            }

        }

        private async void CycleCountCommandPage()
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;

                if (VisibleStockCountPage == true)
                {
                    await Navigation.PushAsync(new CycleCountPage());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                }

            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                isExecuting = false;
            }
        }

        private async void NavigatePageAsync(string pageName)
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;


                switch (pageName)
                {
                    case nameof(StockTakePage):
                        await Navigation.PushAsync(new StockTakePage());
                        break;

                    case nameof(GapCheckPage):
                        await Navigation.PushAsync(new GapCheckPage());
                        break;

                    case nameof(CycleCountPage):
                        await Navigation.PushAsync(new CycleCountPage());
                        break;
                    
                    case nameof(CheckStockCustomPage):
                        await Navigation.PushAsync(new CheckStockCustomPage());
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                isExecuting = false;
            }
        }

        public class CountStockModel
        {
            public int StockTake { get; set; }
            public int CycleCount { get; set; }
            public int GapCheck { get; set; }
        }

        private async void LoadTotal()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userName = Application.Current.Properties["UserStore"]?.ToString();
                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/CountStock?userStore=" + userName);

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var countStock = JsonConvert.DeserializeObject<CountStockModel>(content);

                    if (VisibleStockCountPage == false)
                    {
                        countStock.CycleCount = 0;
                    }

                    if (VisibleStockTakePage == false)
                    {
                        countStock.StockTake = 0;
                    }

                    if (VisibleGapCheckPage == false)
                    {
                        countStock.GapCheck = 0;
                    }

                    CountStock = countStock;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

    }
}
