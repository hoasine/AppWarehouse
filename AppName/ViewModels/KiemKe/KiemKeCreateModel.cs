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
    public class KiemKeCreateModel : ObservableObject
    {
        private ObservableCollection<LocationModel> itemsLocation;
        public ObservableCollection<LocationModel> LocationList
        {
            get => itemsLocation;

            set => SetProperty(ref itemsLocation, value);
        }

        private ObservableCollection<string> itemsTypes;
        public ObservableCollection<string> TypesList
        {
            get => itemsTypes;

            set => SetProperty(ref itemsTypes, value);
        }

        private string _selectedLocation;
        public string SelectedLocation
        {
            get { return _selectedLocation; }

            set
            {
                _selectedLocation = value;

                SetProperty(ref _selectedLocation, value);
            }
        }

        private LocationModel _selectedTypes;
        public LocationModel SelectedTypes
        {
            get { return _selectedTypes; }

            set
            {
                _selectedTypes = value;

                SetProperty(ref _selectedTypes, value);
            }
        }

        public KiemKeCreateModel()
        {
            GetLocation();

            GetTypes();
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

        public void GetTypes()
        {

            string[] listType = { "None serial", "Serial" };

            TypesList = new ObservableCollection<string>(listType);
        }
    }
}
