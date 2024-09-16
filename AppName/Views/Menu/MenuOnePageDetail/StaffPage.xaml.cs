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
    public partial class StaffPage : ContentPage
    {
        public StaffPage()
        {
            InitializeComponent();
            
            BindingContext = new StaffViewModel();
        }

        private void Handel_SeachChange(object sender, TextChangedEventArgs e)
        {
            var content = BindingContext as StaffViewModel;

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                StaffListView.ItemsSource = content.ListStaff;
            }
            else
            {
                StaffListView.ItemsSource = content.ListStaff.Where(s =>
                s.Name.Contains(e.NewTextValue)
                || s.Name.Contains(e.NewTextValue.ToLower())
                || s.Code.Contains(e.NewTextValue)
                || s.Code.Contains(e.NewTextValue.ToLower()));
            }
        }

    }
}
