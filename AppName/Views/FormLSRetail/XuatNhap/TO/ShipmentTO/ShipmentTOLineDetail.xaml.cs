using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShipmentTOLineDetail : PopupPage
    {
        public ShipmentTOLineDetail()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();

            }
            catch (Exception)
            {
            }
        }
    }
}