using System;
using Xamarin.Forms;
using AppName.Core;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Permissions;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using AppName.Model;

namespace AppName
{
    public partial class KiemKeCreatePage : ContentPage
    {

        public KiemKeCreatePage()
        {
            InitializeComponent();

            BindingContext = new KiemKeCreateModel();
        }

        async void CreateInventoryMaster_Click(object sender, EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            var content = "";
            var requei = 0;

            if (LocationPicker.SelectedItem == null)
            {
                content = "Please select a store.";
                requei = 1;
            }

            if (string.IsNullOrEmpty(txtDes.Text))
            {
                content = "Please enter content.";
                requei = 1;
            }

            if (requei == 1)
            {
                Application.Current.MainPage.DisplayAlert("Notification!", content, "OK");
            }
            else
            {
                try
                {
                    var store = Application.Current.Properties["UserName"].ToString();
                    var locationmodel = LocationPicker.ItemsSource[LocationPicker.SelectedIndex] as LocationModel;
                    var model = new InventoryMasterModel()
                    {
                        ID = Guid.NewGuid(),
                        Descriptions = txtDes.Text,
                        StoreName = locationmodel.LocationName,
                        StoreID = locationmodel.LocationCode,
                        CheckType = TypeKiemKes.SelectedIndex.ToString()
                    };

                                   var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/kiemke/create_inventorymaster", string.Empty));

                    var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                    requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(uri, requestJson).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        if (TypeKiemKes.SelectedIndex == 1)
                        {
                            var dialog = new KiemKeDetailSerial(new KiemKeDetailViewModel());
                            dialog.BindingContext = new KiemKeDetailViewModel(model);

                            try
                            {
                                Navigation.PopAsync();
                            }
                            catch (Exception)
                            {
                            }

                            Navigation.PushAsync(dialog);
                        }
                        else
                        {
                            var dialog = new KiemKeDetail(new InventoryMasterModel());
                            dialog.BindingContext = new KiemKeDetailViewModel(model);

                            try
                            {
                                Navigation.PopAsync();
                            }
                            catch (Exception)
                            {
                            }

                            Navigation.PushAsync(dialog);
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
}