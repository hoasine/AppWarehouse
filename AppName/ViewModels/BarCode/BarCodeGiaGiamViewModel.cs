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
    public class BarCodeGiaGiamViewModel : ObservableObject
    {
        private ObservableCollection<PeriodicDiscountModel> itemsPeriodicDiscount;
        public ObservableCollection<PeriodicDiscountModel> PeriodicDiscountList
        {
            get => itemsPeriodicDiscount;

            set => SetProperty(ref itemsPeriodicDiscount, value);
        }

        private ObservableCollection<string> itemsPeriodicDiscountLine;
        public ObservableCollection<string> PeriodicDiscountLineList
        {
            get => itemsPeriodicDiscountLine;

            set => SetProperty(ref itemsPeriodicDiscountLine, value);
        }

        private bool _isMess;
        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        private bool _isHasData;
        public bool IsHasData
        {
            get => _isHasData;

            set => SetProperty(ref _isHasData, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private PeriodicDiscountModel _selectedPeriodDiscount;
        public PeriodicDiscountModel SelectedPeriodDiscount
        {
            get { return _selectedPeriodDiscount; }

            set
            {
                _selectedPeriodDiscount = value;

                SetProperty(ref _selectedPeriodDiscount, value);

                if (!string.IsNullOrEmpty(value.No))
                {
                    GetpecialLine(value.No);
                }
            }
        }

        private string _selectedPeriodDiscountLine;
        public string SelectedPeriodDiscountLine
        {
            get => _selectedPeriodDiscountLine;

            set => SetProperty(ref _selectedPeriodDiscountLine, value);
        }

        public BarCodeGiaGiamViewModel()
        {
            GetSpecial();
        }

        public async Task GetSpecial()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var store = Application.Current.Properties["UserName"].ToString();

            if (!string.IsNullOrEmpty(store))
            {
                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getlistPerioDis?store=" + store, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    PeriodicDiscountList = JsonConvert.DeserializeObject<ObservableCollection<PeriodicDiscountModel>>(content);
                }
            }
        }

        public async Task GetpecialLine(string no_po)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var store = Application.Current.Properties["UserName"].ToString();

            if (!string.IsNullOrEmpty(store))
            {
                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getlistPerioDisLine?store=" + store + "&no_Po=" + no_po, string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<ObservableCollection<PeriodicDiscountLineModel>>(content).Select(s => s.CodeSpecial);

                    PeriodicDiscountLineList = new ObservableCollection<string>(data);
                }
            }
        }
    }
}
