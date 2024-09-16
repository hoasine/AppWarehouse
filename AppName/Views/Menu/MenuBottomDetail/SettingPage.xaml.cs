using Xamarin.Forms;
using AppName.Core;
using System;
using System.Threading.Tasks;

namespace AppName
{
    public partial class SettingPage : ContentView
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private async void DangXuatClick(object sender, EventArgs args)
        {
            await Application.Current.SavePropertiesAsync();
            Application.Current.MainPage = new NavigationPage(new LoginFrm());

            Application.Current.Properties["IsLogin"] = false;
            Application.Current.Properties["UserName"] = "";
            Application.Current.Properties["RoleID"] = "";
            Application.Current.Properties["Token"] = "";

            //sdasdsad
        }

        void GioiThieu_Click(object sender, EventArgs args)
        {
            //Application.Current.MainPage = new NavigationPage(new RootMasterDetailPage());

            //Navigation.PushAsync(new AboutPage());
        }

        void Template_clik(object sender, EventArgs args)
        {
            Template();
        }

        public async Task Template()
        {
            //await Application.Current.SavePropertiesAsync();
            //Application.Current.MainPage = new NavigationPage(new RootMasterDetailPage());
        }
    }
}
