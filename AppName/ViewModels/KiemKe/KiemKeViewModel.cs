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
using System.ComponentModel;
using AppName.CustomRenderer;
using AppName.Model;

namespace AppName
{
    public class KiemKeViewModel : ObservableObject
    {
        private ObservableCollection<LocationModel> itemsLocation;
        public ObservableCollection<LocationModel> LocationList
        {
            get => itemsLocation;

            set => SetProperty(ref itemsLocation, value);
        }

        private ObservableCollection<InventoryMasterModel> itemsInventoryMaster;
        public ObservableCollection<InventoryMasterModel> InventoryMasterList
        {
            get => itemsInventoryMaster;

            set => SetProperty(ref itemsInventoryMaster, value);
        }

        private LocationModel _selectedLocation;
        public LocationModel SelectedLocation
        {
            get { return _selectedLocation; }

            set
            {
                _selectedLocation = value;

                SetProperty(ref _selectedLocation, value);

                if (!string.IsNullOrEmpty(value.LocationCode))
                {
                    Get_InventoryMaster(value.LocationCode);
                }
            }
        }

        private string _selectedInventoryMaster;
        public string SelectedInventoryMaster
        {
            get => _selectedInventoryMaster;

            set => SetProperty(ref _selectedInventoryMaster, value);
        }

        public KiemKeViewModel()
        {
            GetLocation();
        }

        public async Task GetLocation()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/getlocation", string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                LocationList = JsonConvert.DeserializeObject<ObservableCollection<LocationModel>>(content);
            }
        }

        public async Task Get_InventoryMaster(string locationCode)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/get_inventorymaster?locationCode=" + locationCode, string.Empty));

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                InventoryMasterList = JsonConvert.DeserializeObject<ObservableCollection<InventoryMasterModel>>(content);
            }
        }
    }
}
