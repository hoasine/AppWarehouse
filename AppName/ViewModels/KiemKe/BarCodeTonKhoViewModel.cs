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
using System.Linq;
using AppName.ViewModels.BarCode.Model;
using AppName.CustomRenderer;
using AppName.Model;

namespace AppName
{
    public class BarCodeTonKhoViewModel : ObservableObject
    {
        private readonly string _barcodeID;
        private TonKhoModel _tonkho;
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

        public BarCodeTonKhoViewModel(string barcodeID)
        {
            _barcodeID = barcodeID;

            if (_barcodeID != null)
            {
                _itemno = "Tồn Kho: " + _barcodeID;

                LoadData();
            }
        }

        public TonKhoModel TonKho
        {
            get { return _tonkho; }
            set { SetProperty(ref _tonkho, value); }
        }

        protected async void LoadData()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getdatainventory?barCode=" + _barcodeID + "&type=1", string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                TonKho = JsonConvert.DeserializeObject<List<TonKhoModel>>(content).FirstOrDefault();

                if (TonKho != null && !string.IsNullOrEmpty(TonKho.BarcodeNo))
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
