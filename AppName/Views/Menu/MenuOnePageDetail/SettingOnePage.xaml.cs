using Xamarin.Forms;
using AppName.Core;
using System;
using System.Threading.Tasks;

namespace AppName
{
    public partial class SettingOnePage : ContentPage
    {
        public SettingOnePage()
        {
            InitializeComponent();

            if ((bool)Application.Current.Properties["IsLogin"] == true)
            {

            }
            else
            {
                lblDangXuat.IsVisible = false;
            }
        }

        private async void DangXuatClick(object sender, EventArgs args)
        {
            await Application.Current.SavePropertiesAsync();
            Application.Current.MainPage = new NavigationPage(new LoginFrm());

            Application.Current.Properties["IsLogin"] = false;
            Application.Current.Properties["UserName"] = "";
            Application.Current.Properties["RoleID"] = "";
            Application.Current.Properties["Token"] = "";
        }
        
        private async void DangNhapClick(object sender, EventArgs args)
        {
            await Application.Current.SavePropertiesAsync();
            Application.Current.MainPage = new NavigationPage(new LoginFrm());
        }

        void GioiThieu_Click(object sender, EventArgs args)
        {
            Navigation.PushAsync(new AboutPage());
        }

        void Template_clik(object sender, EventArgs args)
        {
            Template();
        }

        public async Task Template()
        {
            await Application.Current.SavePropertiesAsync();
            //Application.Current.MainPage = new NavigationPage(new RootMasterDetailPage());
        }
    }
}
