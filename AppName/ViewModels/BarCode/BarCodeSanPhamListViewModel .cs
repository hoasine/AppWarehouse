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
    public class BarCodeSanPhamListViewModel : BaseViewModel
    {
        public ObservableCollection<SanPhamModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

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

        public BarCodeSanPhamListViewModel()
        {
            Items = new ObservableCollection<SanPhamModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

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

        protected async void LoadData(string thebarcode)
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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getdata?barCode=" + thebarcode, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    BarCode = JsonConvert.DeserializeObject<SanPhamModel>(content);

                    string[] numbers = new[]
                  {
                        ""
                      
                    };

                    if (BarCode != null)
                    {
                        Random rand = new Random();
                        int index = rand.Next(numbers.Length);

                        BarCode.URLImage = numbers[index];

                        if (string.IsNullOrEmpty(BarCode.URLImage))
                        {
                            BarCode.URLImage = "imageempty.png";
                        }

                        SanPhamModel i = BarCode;
                        Items.Add(i);
                    }
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
