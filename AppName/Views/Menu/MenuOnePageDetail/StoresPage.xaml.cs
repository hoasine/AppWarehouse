using System;
using System.Collections.Generic;

using Xamarin.Forms;
using AppName.Core;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Rg.Plugins.Popup.Services;
using System.Net.Http.Headers;
using System.Linq;

namespace AppName
{
    public partial class StoresPage : ContentPage
    {
        public StoresPage()
        {
            InitializeComponent();

            BindingContext = new StoreViewModel();
        }

        private void Handel_SeachChange(object sender, TextChangedEventArgs e)
        {
            var content = BindingContext as StoreViewModel;

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                StoreListView.ItemsSource = content.ListStore;
            }
            else
            {
                StoreListView.ItemsSource = content.ListStore.Where(s =>
                s.Name.Contains(e.NewTextValue)
                || s.Name.Contains(e.NewTextValue.ToLower())
                || s.No_.Contains(e.NewTextValue)
                || s.No_.Contains(e.NewTextValue.ToLower()));
            }
        }

    }
}
