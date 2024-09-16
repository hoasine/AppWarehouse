using Xamarin.Forms;
using AppName.Core;

using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AppName.CustomRenderer;
using Rg.Plugins.Popup.Services;
using AppName.Model;

namespace AppName
{
    public partial class MenuOnePage : ContentPage
    {
        public MenuOnePage()
        {
            InitializeComponent();

            var userName = "";

            if (Application.Current.Properties.ContainsKey("RetailName"))
            {
                userName = "Welcome to " + Application.Current.Properties["RetailName"].ToString();
                lbUserName.Text = userName;
            }

            if (string.IsNullOrEmpty(Application.Current.Properties["Token"].ToString()))
            {
                Application.Current.SavePropertiesAsync();
                Application.Current.MainPage = new NavigationPage(new LoginFrm());
            }
            else
            {
                BindingContext = new MenuOnePageViewModel();
            }
        }

        //void MenuRightClick(object sender, EventArgs e)
        //{
        //    Navigation.PushAsync(new SettingOnePage());
        //}

        private async void DangXuatClick(object sender, EventArgs args)
        {
            RealmHelper.RemoveAll<LocalPermissionModel>();

            await Application.Current.SavePropertiesAsync();
            Application.Current.MainPage = new NavigationPage(new LoginFrm());

            Application.Current.Properties["IsLogin"] = false;
            Application.Current.Properties["UserName"] = "";
            Application.Current.Properties["RoleID"] = "";
            Application.Current.Properties["Token"] = "";
        }
    }
}