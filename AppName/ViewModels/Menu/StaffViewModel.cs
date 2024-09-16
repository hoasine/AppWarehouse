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
    public class StaffViewModel : ObservableObject
    {
        private Command _searchCommand;

        private ObservableCollection<StaffModel> _staff;
        public ObservableCollection<StaffModel> ListStaff 
        {
            get => _staff;

            set => SetProperty(ref _staff, value);
        }

        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new Command<string>((text) =>
        {
            if (text.Length >= 1)
            {
                ListStaff .Clear();
                var suggestions = ListStaff .Where(c => c.Name.ToLower().StartsWith(text.ToLower())).ToList();
                ListStaff .Clear();
                foreach (var recipe in suggestions)
                    ListStaff .Add(recipe);

            }
        }));

        public StaffViewModel()
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

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/getstaff?storename=" + Application.Current.Properties["UserName"].ToString(), string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    ListStaff  = JsonConvert.DeserializeObject<ObservableCollection<StaffModel>>(content);

                    int number = 1;

                    foreach (var item in ListStaff )
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
