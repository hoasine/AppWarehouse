using AppName.CustomRenderer;
using AppName.Model;
using AppName.ViewModels.BarCode.Model;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppName
{
    public class CTKMsGroupViewModel : BaseViewModel
    {
        private CTKMViewModel _oldCTKM;
        private readonly string _barcodeID;
        private string _itemno;
        private bool _isHasData;
        private bool _isMess;

        public Command<string> SearchItemsCommand { get; set; }

        private ObservableCollection<CTKMViewModel> items;
        public ObservableCollection<CTKMViewModel> ListCTKM
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        private ObservableCollection<CTKM> _listCTKMs;
        /// <summary>
        /// Field mới dựa vào class CTKM để tạo data cho việc chỉnh sửa giao diện tab Danh sách khuyến mãi
        /// </summary>
        public ObservableCollection<CTKM> ListCTKMs
        {
            get { return _listCTKMs; }
            set { SetProperty(ref _listCTKMs, value); }
        }

        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
        }

        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        public bool IsHasData
        {
            get => _isHasData;

            set => SetProperty(ref _isHasData, value);
        }

        public Command LoadDanhSachCTKMsCommand { get; set; }
        public Command LoadCTKMItemCommand { get; set; }
        public Command<CTKMViewModel> RefreshItemsCommand { get; set; }

        public CTKMsGroupViewModel(string barcodeID = null)
        {
            ListCTKMs = new ObservableCollection<CTKM>();

            items = new ObservableCollection<CTKMViewModel>();
            ListCTKM = new ObservableCollection<CTKMViewModel>();
            LoadDanhSachCTKMsCommand = new Command(async () => await ExecuteLoadDanhSachCommandAsync());
            LoadCTKMItemCommand = new Command(async () => await ExecuteLoadDanhSachCommandAsync());
            RefreshItemsCommand = new Command<CTKMViewModel>((item) => ExecuteRefreshItemsCommand(item));

            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);


        }

        private void ExecuteRefreshItemsCommand(CTKMViewModel item)
        {
            if (_oldCTKM == item)
            {
                // click twice on the same item will hide it
                item.Expanded = !item.Expanded;
            }
            else
            {
                if (_oldCTKM != null)
                {
                    // hide previous selected item
                    _oldCTKM.Expanded = false;
                }
                // show selected item
                item.Expanded = true;
            }

            _oldCTKM = item;
        }

        public async void ExecuteSearchItemsCommand(string keyvalue)
        {
            var checkCTKMView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "CTKM"
             && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            if (checkCTKMView == false)
            {
                Application.Current.MainPage.DisplayAlert("Notification !", "You do not have permission for this function.", "OK");
            }
            else
            {
                if (keyvalue == "")
                {
                    ListCTKM.Clear();
                }
                else
                {
                    LoadData(keyvalue);
                }
            }
        }

        private Color RandomColor()
        {
            List<string> listBackgroundColor = new List<string> { "#7ea5fa", "#efa650", "#e7648a", "#a47afd", "#74e2b9", "#E2DB74", "#74D8E2" };

            var random = new Random();
            var index = random.Next(0, 4);

            return Color.FromHex(listBackgroundColor[index]);
        }

        public partial class CTKMMasterModel
        {
            public string NO { get; set; }
            public string Description { get; set; }
            public int Status { get; set; }
            public string StartingDate { get; set; }
            public string EndingDate { get; set; }
            public string DiscountTypeName { get; set; }
        }

        public partial class ResuftPromotionListModel
        {
            public bool Active { get; set; }
            public List<CTKMMasterModel> ListData { get; set; }
        }

        async Task ExecuteLoadDanhSachCommandAsync()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                ListCTKMs.Clear();

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/promotion/GetPromotionList", string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var store = Application.Current.Properties["UserName"].ToString();

                    string content = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<ResuftPromotionListModel>(content);
                    if (data.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (data.ListData.Count() > 0)
                    {
                        int number = 1;
                        foreach (var item in data.ListData.OrderBy(s => s.DiscountTypeName))
                        {
                            var model = new CTKM();
                            model.ImageURL = "https://banner2.cleanpng.com/20171127/764/sale-tag-png-clip-art-image-5a1bdadb35e560.8734260515117749392208.jpg";
                            model.ItemNo = item.NO;
                            model.Name = number + ". " + item.Description;
                            model.StartTime = item.StartingDate;
                            model.EndTime = item.EndingDate;
                            model.Store = store;
                            model.PromotionType = item.DiscountTypeName;
                            //model.BackgroundColor = RandomColor();

                            if (item.DiscountTypeName == "Discount Offers")
                            {
                                model.BackgroundColor = Color.FromHex("#efa650");
                            }
                            else if (item.DiscountTypeName == "Discount Mix&Match")
                            {
                                model.BackgroundColor = Color.FromHex("#7ea5fa");
                            }
                            else if (item.DiscountTypeName == "Line Discount Offers")
                            {
                                model.BackgroundColor = Color.FromHex("#e7648a");
                            }
                            else if (item.DiscountTypeName == "Total Discount Offers")
                            {
                                model.BackgroundColor = Color.FromHex("#a47afd");
                            }
                            else if (item.DiscountTypeName == "Discount Multibuy")
                            {
                                model.BackgroundColor = Color.FromHex("#74e2b9");
                            }
                            else if (item.DiscountTypeName == "Tender Offers")
                            {
                                model.BackgroundColor = Color.FromHex("#E2DB74");
                            }
                            else
                            {
                                model.BackgroundColor = Color.FromHex("#74D8E2");
                            }

                            ListCTKMs.Add(model);

                            number++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public partial class ResuftPromotionModel
        {
            public bool Active { get; set; }
            public List<CTKM> ListData { get; set; }
        }

        public async void LoadData(string KeyValue)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            logger.Info("Load item CTKM Json:=" + JsonConvert.SerializeObject(KeyValue) + ".", "Info");

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                ListCTKM.Clear();

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/promotion/GetPromotion?ItemNo=" + KeyValue, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<ResuftPromotionModel>(content);
                    if (data.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (data.ListData.Count() > 0)
                    {
                        ItemNo = "Promotions: " + data.ListData.FirstOrDefault().ItemNo;
                        IsHasData = true;
                        IsMess = false;

                        int number = 1;
                        foreach (var item in data.ListData)
                        {
                            item.Name = number + ". " + item.Name + " (" + item.Total + ")";

                            ListCTKM.Add(new CTKMViewModel(item));

                            number++;
                        }
                    }
                    else
                    {
                        ItemNo = "Promotion: Not found";

                        IsHasData = false;
                        IsMess = true;

                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this view :=" + KeyValue };
                        await PopupNavigation.Instance.PushAsync(dialog, false);
                    }
                }
                else
                {
                    logger.Error("Load item CTKM Error: " + "Server connection error !" + ".", "Error");

                    ItemNo = "Server connection error !";

                    IsHasData = false;
                    IsMess = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Load item CTKM Error: " + ex.Message.ToString() + ".", "Error");

                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
