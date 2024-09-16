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
using System.Windows.Input;
using System.Linq;
using AppName.Model;

namespace AppName
{
    public class StoreViewModel : ObservableObject
    {
        private Command _searchCommand;

        private ObservableCollection<StoreModel> _store;
        public ObservableCollection<StoreModel> ListStore
        {
            get => _store;

            set => SetProperty(ref _store, value);
        }

        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new Command<string>((text) =>
        {
            if (text.Length >= 1)
            {
                ListStore.Clear();
                var suggestions = ListStore.Where(c => c.Name.ToLower().StartsWith(text.ToLower())).ToList();
                ListStore.Clear();
                foreach (var recipe in suggestions)
                    ListStore.Add(recipe);

            }
        }));

        public StoreViewModel()
        {
            LoadData();
        }

        protected async Task LoadData()
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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getstore", string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    ListStore = JsonConvert.DeserializeObject<ObservableCollection<StoreModel>>(content);

                    int number = 1;

                    foreach (var item in ListStore)
                    {
                        item.STT = number;

                        number++;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
