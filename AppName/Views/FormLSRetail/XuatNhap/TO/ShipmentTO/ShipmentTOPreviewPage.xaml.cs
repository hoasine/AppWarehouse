using AppName.Model.XuatNhap;
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
    public partial class ShipmentTOPreviewPage : ContentPage
    {
        public ShipmentTOPreviewPage(TOHeaderPreviewModel itemModel)
        {
            InitializeComponent();

            BindingContext = new ShipmentTOPreviewViewModel(Navigation, itemModel);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}