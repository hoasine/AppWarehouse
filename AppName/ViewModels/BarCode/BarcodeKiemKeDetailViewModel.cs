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
using AppName.CustomRenderer;
using System.Linq;

namespace AppName
{
    public class BarcodeKiemKeDetailViewModel : BaseViewModel
    {

        private ObservableCollection<SanPhamModel> items;
        public ObservableCollection<SanPhamModel> ItemBarodes
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public Command LoadItemsCommand { get; set; }

        private SanPhamModel _data;

        private Guid _masterID;

        public Guid MasterID
        {
            get => _masterID;

            set => SetProperty(ref _masterID, value);
        }


        private InventoryMasterModel _inventoryMaster;

        public InventoryMasterModel InventoryMaster
        {
            get => _inventoryMaster;

            set => SetProperty(ref _inventoryMaster, value);
        }

        private TonKhoTotalModel _tonKhoTotalModel;

        public TonKhoTotalModel TonKhoTotal
        {
            get { return _tonKhoTotalModel; }
            set { SetProperty(ref _tonKhoTotalModel, value); }
        }

        private ObservableCollection<InventoryDetailModel> _inventoryDetail;

        public ObservableCollection<InventoryDetailModel> InventoryDetail
        {
            get => _inventoryDetail;

            set => SetProperty(ref _inventoryDetail, value);
        }

        public BarcodeKiemKeDetailViewModel(InventoryMasterModel model = null)
        {
            _masterID = model.ID;
            InventoryMaster = model;
            ItemBarodes = new ObservableCollection<SanPhamModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            LoadPhieu(InventoryMaster.StoreID);

            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                var resuft = await LoadData(thebarcode.Substring(0, thebarcode.Length - 1));
                if (resuft == false)
                {
                    var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "No products found in the system." };
                    await PopupNavigation.Instance.PushAsync(dialog);
                }

                await LoadPhieu(InventoryMaster.StoreID);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ItemBarodes.Clear();

                var model = new SanPhamModel();
                model.Barcode_No_ = "Barcode Code";
                model.URLImage = @"https://developers.google.com/ml-kit/vision/barcode-scanning/images/barcode_scanning2x.png";
                model.ItemName = "Item Name";

                ItemBarodes.Add(model);
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

        public SanPhamModel Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        protected async Task<bool> LoadData(string barcode)
        {
            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/internalapp/getdatamasterfile?barCode=" + barcode + "&masterID=" + _masterID, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    Data = JsonConvert.DeserializeObject<SanPhamModel>(content);

                    string[] numbers = new[]
                    {
                        ""
                    };

                    if (Data != null)
                    {
                        Random rand = new Random();
                        int index = rand.Next(numbers.Length);

                        Data.URLImage = numbers[index];

                        if (string.IsNullOrEmpty(Data.URLImage))
                        {
                            Data.URLImage = "imageempty.png";
                        }

                        SanPhamModel i = Data;
                        ItemBarodes.Add(i);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected async Task LoadPhieu(string LocationCode)
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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/GetTotalInventoryMobileApp?LocationCode=" + LocationCode + "&MasterID=" + _masterID, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    TonKhoTotal = JsonConvert.DeserializeObject<TonKhoTotalModel>(content);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
