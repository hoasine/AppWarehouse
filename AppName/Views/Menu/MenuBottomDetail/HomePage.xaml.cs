using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;

namespace AppName
{
    public partial class HomePage : ContentView
    {
        public string Message
        {
            get
            {
                return textLabel.Text;
            }
            set
            {
                textLabel.Text = value;
            }
        }

        public HomePage()
        {
            InitializeComponent();

            var userName = "";

            if (Application.Current.Properties.ContainsKey("UserName"))
            {
                userName = "Welcome to " + Application.Current.Properties["UserName"].ToString();
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

        void MenuRightClick(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingOnePage());
        }

        void StoreClick(object sender, EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
            {
                Navigation.PushAsync(new StoresPage());
            }
        }

        void StaffClick(object sender, EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
            {
                Navigation.PushAsync(new StaffPage());
            }
        }

        void KPIClick(object sender, EventArgs e)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
            {
                //Navigation.PushAsync(new EmployeePerformanceDashboardPage());
            }
        }

    }
}
