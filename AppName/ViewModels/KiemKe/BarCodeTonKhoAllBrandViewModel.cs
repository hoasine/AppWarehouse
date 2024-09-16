using System;
using Xamarin.Forms;
using AppName.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using System.Threading;
using AppName.CustomRenderer;
using AppName.Model;
using System.Linq;

namespace AppName
{
    public class BarCodeTonKhoAllBrandViewModel : ObservableObject
    {
        private readonly string _barcodeID;
        private readonly string _type;

        private ObservableCollection<TonKhoModel> items;
        public ObservableCollection<TonKhoModel> ListTonKho
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        private ObservableCollection<TonKhoModel> item1111s;
        public ObservableCollection<TonKhoModel> ListTonKho1
        {
            get => item1111s;

            set => SetProperty(ref item1111s, value);
        }

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

        public BarCodeTonKhoAllBrandViewModel(string barcodeID, string type)
        {
            _barcodeID = barcodeID;
            _type = type;

            if (_barcodeID != null)
            {
                _itemno = "Inventory: " + _barcodeID;

                LoadData();
            }
        }

        protected async Task LoadData()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getdatainventory?barCode=" + _barcodeID + "&type=" + _type, string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                ListTonKho = JsonConvert.DeserializeObject<ObservableCollection<TonKhoModel>>(content);

                if (ListTonKho.Count > 0)
                {
                    IsHasData = true;
                    IsMess = false;
                }
                else
                {
                    IsHasData = false;
                    IsMess = true;
                }
            }
            else
            {
                IsHasData = false;
                IsMess = true;
            }
        }
    }
}
