using System;
using Xamarin.Forms;
using AppName.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using AppName.Model;
using AppName.Services;
using System.Diagnostics;
using System.Windows.Input;
using System.Linq;

namespace AppName
{
    public class ReLableViewModel : BaseViewModel
    {
        public ObservableCollection<SanPhamModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command<string> SearchItemsCommand { get; set; }

        private readonly string _barcodeID;
        private SanPhamModel _barcode;
        private string _itemno;
        private bool _isHasData;
        private bool _isMess;

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

        public ReLableViewModel()
        {
            Items = new ObservableCollection<SanPhamModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                LoadData(thebarcode.Substring(0, thebarcode.Length - 1));
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();

                var model = new SanPhamModel();
                model.Barcode_No_ = "Barcode Code";
                model.URLImage = @"https://developers.google.com/ml-kit/vision/barcode-scanning/images/barcode_scanning2x.png";
                model.ItemName = "Item Name";

                Items.Add(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public SanPhamModel BarCode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }

        public async void ExecuteSearchItemsCommand(string keyvalue)
        {
            LoadData(keyvalue);
        }


        public partial class ResuftMasterFileModel
        {
            public bool Active { get; set; }
            public List<SanPhamViewModel> ListData { get; set; }
        }

        protected async void LoadData(string KeyValue)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoNoImage?KeyValue=" + KeyValue, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftMasterFileModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    var listModel = new List<SanPhamModel>();

                    //BarCode = keyvalue;

                    string[] numbers = new[]
                  {
                         ""
                    };

                    Items.Clear();

                    foreach (var data in dataList.ListData)
                    {
                        var modelData = new SanPhamModel()
                        {
                            Barcode_No_ = data.Barcode_No_,
                            ItemName = data.ItemName + " " + data.ItemNo,
                            ItemNo = data.ItemNo,
                            UnitPrice = data.Unit_Price.Value.ToString("#,##"),
                            URLImage = ""
                        };

                        Random rand = new Random();
                        int index = rand.Next(numbers.Length);

                        modelData.URLImage = numbers[index];

                        if (string.IsNullOrEmpty(modelData.URLImage))
                        {
                            modelData.URLImage = "imageempty.png";
                        }

                        SanPhamModel i = modelData;
                        Items.Add(i);
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
    }
}
