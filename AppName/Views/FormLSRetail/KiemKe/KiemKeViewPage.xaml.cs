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

namespace AppName
{
    public partial class KiemKeViewPage : ContentPage
    {

        public KiemKeViewPage()
        {
            InitializeComponent();

            BindingContext = new KiemKeViewModel();
        }

        void KiemKe_click(object sender, EventArgs e)
        {
            var content = "";
            var requei = 0;

            if (LocationPicker.SelectedItem == null)
            {
                content = "Please select the store.";
                requei = 1;
            }

            if (InventoryMaster.SelectedItem == null)
            {
                content = "Please select the check sheet.";
                requei = 1;
            }

            if (requei == 1)
            {
                Application.Current.MainPage.DisplayAlert("Notification!", content, "OK");
            }
            else
            {
                var store = Application.Current.Properties["UserName"].ToString();
                var locationmodel = LocationPicker.ItemsSource[LocationPicker.SelectedIndex] as LocationModel;
                var masterModel = InventoryMaster.ItemsSource[InventoryMaster.SelectedIndex] as InventoryMasterModel;

                var model = new InventoryMasterModel()
                {
                    ID = masterModel.ID,
                    Descriptions = masterModel.Descriptions,
                    StoreName = locationmodel.LocationName,
                    StoreID = locationmodel.LocationCode,
                    UserName = store,
                    CheckType = masterModel.CheckType
                };

                var dialog = new KiemKeDetail(model);
                dialog.BindingContext = new BarcodeKiemKeDetailViewModel(model);

                Navigation.PushAsync(dialog);
            }
        }
    }
}