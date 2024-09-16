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
    public partial class ReceivePOPreviewPage : ContentPage
    {
        public ReceivePOPreviewPage(POHeaderPreviewModel itemModel)
        {
            InitializeComponent();

            BindingContext = new ReceivePOPreviewViewModel(Navigation, itemModel);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}