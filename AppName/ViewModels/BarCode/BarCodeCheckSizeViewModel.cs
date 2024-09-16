using System;
using Xamarin.Forms;
using AppName.Core;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using AppName.CustomRenderer;
using AppName.Model;

namespace AppName
{
    public class BarCodeCheckSizeViewModel : ObservableObject
    {
        private readonly string _barcodeID;

        private ObservableCollection<CheckSizeModel> items;
        public ObservableCollection<CheckSizeModel> ListSize
        {
            get => items;

            set => SetProperty(ref items, value);
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

        public BarCodeCheckSizeViewModel(string barcodeID, int type)
        {
            _barcodeID = barcodeID;

            if (_barcodeID != null)
            {
                _itemno = "Product Size: " + _barcodeID;

                LoadData(type);
            }
        }

        protected async void LoadData(int type)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getsizeproduct?barCode=" + _barcodeID + "&type=" + type, string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                ListSize = JsonConvert.DeserializeObject<ObservableCollection<CheckSizeModel>>(content);

                if (ListSize.Count > 0)
                {
                    ItemNo = "Product Size: " + ListSize.FirstOrDefault().ItemNo;
                    IsHasData = true;
                    IsMess = false;

                    int number = 1;

                    foreach (var item in ListSize)
                    {
                        item.STT = number;

                        number++;
                    }
                }
                else
                {
                    IsHasData = false;
                    IsMess = true;
                }
            }
            else
            {
                ItemNo = "Server connection error !";

                IsHasData = false;
                IsMess = true;
            }
        }
    }
}
