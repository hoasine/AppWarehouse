using System.Collections.ObjectModel;

using System.Globalization;
using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using System.Windows.Input;
using AppName.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace AppName
{
    public class NhapXuatViewModel : ObservableObject
    {
        public ICommand POCommand { get; set; }
        public ICommand TOShipmentCommand { get; set; }
        public ICommand TOReceivingCommand { get; set; }


        private bool _visibleCreateTO;
        public bool VisibleCreateTO
        {
            get { return _visibleCreateTO; }
            set { SetProperty(ref _visibleCreateTO, value); }
        }

        private bool _visibleShipmentTO;
        public bool VisibleShipmentTO
        {
            get { return _visibleShipmentTO; }
            set { SetProperty(ref _visibleShipmentTO, value); }
        }

        private bool _visibleShipmentPageTO;
        public bool ShipmentPageTO
        {
            get { return _visibleShipmentPageTO; }
            set { SetProperty(ref _visibleShipmentPageTO, value); }
        }

        private bool _visibleReceivePageTO;
        public bool ReceivePageTO
        {
            get { return _visibleReceivePageTO; }
            set { SetProperty(ref _visibleReceivePageTO, value); }
        }

        private bool _visibleReceivePagePO;
        public bool ReceivePagePO
        {
            get { return _visibleReceivePagePO; }
            set { SetProperty(ref _visibleReceivePagePO, value); }
        }

        private NavigationCategoryData _category;
        private NavigationItemData _selectedItem;
        protected INavigation Navigation { get; private set; }
        //public NhapXuatViewModel(string variantPageName = null)
        //    : base(listenCultureChanges: true)
        //{
        //    LoadData();
        //}

        public NhapXuatViewModel(INavigation navigation)
             : base(listenCultureChanges: true)
        {
            NavigateCommand = new Command<string>(NavigateAsync);

            POCommand = new Command(OpenPOCommandPage);
            TOShipmentCommand = new Command(OpenTOShipmentCommandPage);
            TOReceivingCommand = new Command(OpenTOReceivingCommandPage);

            Navigation = navigation;

            var listPermission = RealmHelper.Instance.All<LocalPermissionModel>().ToArray();

            VisibleCreateTO = listPermission.Any(q => q.KeyPermission == "CreateTO"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            VisibleShipmentTO = listPermission.Any(q => q.KeyPermission == "ShipmentPageTO"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            ReceivePageTO = listPermission.Any(q => q.KeyPermission == "ReceivePageTO"
              && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            ReceivePagePO = listPermission.Any(q => q.KeyPermission == "ReceivePagePO"
              && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            LoadTotal();

            LoadData();
        }

        private async void OpenPOCommandPage()
        {
            if (ReceivePagePO == true)
            {
                await Navigation.PushAsync(new ReceivePOPage());
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
            }
        }

        private async void OpenTOShipmentCommandPage()
        {
            if (VisibleShipmentTO == true)
            {
                await Navigation.PushAsync(new ShipmentTOPage());
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
            }
        }

        private CountTOPOModel _CountTOPO;
        public CountTOPOModel CountTOPO
        {
            get { return _CountTOPO; }
            set { SetProperty(ref _CountTOPO, value); }
        }

        private async void OpenTOReceivingCommandPage()
        {
            if (ReceivePageTO == true)
            {
                await Navigation.PushAsync(new ReceiveTOPage());
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
            }
        }

        private ObservableCollection<NavigationItemData> items;
        public ObservableCollection<NavigationItemData> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public ICommand NavigateCommand { get; set; }

        public NavigationCategoryData Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public NavigationItemData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
                    {
                        if (value.MenuID == "NhapKho")
                        {

                            Navigation.PushAsync(new KiemKeViewPage());
                        }
                        else if (value.MenuID == "XuatKho")
                        {

                            Navigation.PushAsync(new KiemKeViewPage());
                        }
                    }

                    SetProperty(ref _selectedItem, null);
                }
            }
        }
    
        protected override void OnCultureChanged(CultureInfo culture)
        {
            LoadData();
        }

        public class CountTOPOModel
        {
            public int ReceiveTO { get; set; }
            public int ShipmentTO { get; set; }
            public int ReceivePO { get; set; }
        }

        private async Task LoadTotal()
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

                var userName = Application.Current.Properties["UserName"]?.ToString();
                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/CountPOTO?location=" + userName);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var countopo = JsonConvert.DeserializeObject<CountTOPOModel>(content);

                    if (VisibleShipmentTO == false)
                    {
                        countopo.ShipmentTO = 0;
                    }

                    if (ReceivePageTO == false)
                    {
                        countopo.ReceiveTO = 0;
                    }

                    if (ReceivePagePO == false)
                    {
                        countopo.ReceivePO = 0;
                    }

                    CountTOPO = countopo;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }


        private void LoadData()
        {
            Category = null;

            Items = new ObservableCollection<NavigationItemData>()
            {
               new NavigationItemData()
                {
                    MenuID = "NhapKho",
                      Name = "Nhập kho",
                     BackgroundColor = "#FF7C4ECD",
                     BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                     Icon = "",
                     ItemCount = 19,
                     Badge = 9,
                     Tittle = "Danh sách nhập kho"
                },
                new NavigationItemData()
                {
                    MenuID = "XuatKho",
                        Name = "Xuất kho",
                        BackgroundColor = "#FF7C4ECD",
                        BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                        Icon = "",
                        ItemCount = 19,
                        Badge = 9,
                        Tittle = "Danh sách nhập kho"
                }
            };

            //new NavigationItemData()
            //{
            //    MenuID = "KiemKe",
            //    Name = "Kiểm kê",
            //    BackgroundColor = "#FF5F7DD4",
            //    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_09.jpg",
            //    Icon = "",
            //    ItemCount = 19,
            //    Badge = 9,
            //    Tittle = "Tạo phiếu kiểm theo khu vực"
            //},

        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }


        bool isExecuting = false;

        private async void NavigateAsync(string pageName)
        {
            if (isExecuting)
                return;

            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                isExecuting = true;

                switch (pageName)
                {
                    case "TaoPhieuNhapPage":
                        var checkTOView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "CreateTO"
                      && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                        if (checkTOView == false)
                        {
                            Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                        }
                        else
                        {
                            await Navigation.PushAsync(new TaoPhieuNhapPage());
                        }

                        break;
                    case "TaoPhieuXuatPage":
                        await Navigation.PushAsync(new TaoPhieuXuatPage());
                        break;
                    case "ShipmentTOPage":
                        var checkShipmentPage = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ShipmentPageTO"
                        && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                        if (checkShipmentPage == false)
                        {
                            Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                        }
                        else
                        {
                            await Navigation.PushAsync(new ShipmentTOPage());
                        }

                        break;
                    case "ReceiveTOPage":
                        var checkReceivePageTO = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ReceivePageTO"
                       && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                        if (checkReceivePageTO == false)
                        {
                            Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                        }
                        else
                        {
                            await Navigation.PushAsync(new ReceiveTOPage());
                        }

                        break;
                    case "ReceivePOPage":
                        var CheckReceivePagePO = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "ReceivePagePO"
                      && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

                        if (CheckReceivePagePO == false)
                        {
                            Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
                        }
                        else
                        {
                            await Navigation.PushAsync(new ReceivePOPage());
                        }
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

                ShowLoading = false;
            }
        }
    }
}