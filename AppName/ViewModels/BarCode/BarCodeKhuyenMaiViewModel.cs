using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using AppName.ViewModels.BarCode.Model;
using System.Linq;

namespace AppName
{
    public class BarCodeKhuyenMaiViewModel : ObservableObject
    {
        private readonly string _barcodeID;

        public List<ObservableGroupCollection<string, CTKMModel>> _ctkm;

        public List<ObservableGroupCollection<string, CTKMModel>> GroupedData
        {
            get { return _ctkm; }
            set { SetProperty(ref _ctkm, value); }
        }

        public Command<CTKMModel> RefreshItemsCommand
        {
            get;
            set;
        }

        public BarCodeKhuyenMaiViewModel(string barcodeID = null)
        {
            _barcodeID = barcodeID;

            RefreshItemsCommand = new Command<CTKMModel>((item) => ExecuteRefreshItemsCommand(item));

            LoadData();
        }

        private CTKMModel _oldHotel;


        public bool isExpanded = false;
        private void ExecuteRefreshItemsCommand(CTKMModel item)
        {
            if (_oldHotel == item)
            {
                // click twice on the same item will hide it  
                Expanded = !item.Expanded;
            }
            else
            {
                if (_oldHotel != null)
                {
                    // hide previous selected item  
                    Expanded = false;
                }
                // show selected item  
                Expanded = true;
            }
            _oldHotel = item;

        }
        protected async void LoadData()
        {
            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString());

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format("http://app_dafc.dafc.com.vn:3232/api/home/GetDataPromotion?barCode=" + _barcodeID, string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<ObservableCollection<CTKMModel>>(content);
               
                GroupedData = data.OrderBy(p => p.Type)
                            .GroupBy(p => p.Type.ToString())
                            .Select(p => new ObservableGroupCollection<string, CTKMModel>(p)).ToList();
            }
        }
    }
}
