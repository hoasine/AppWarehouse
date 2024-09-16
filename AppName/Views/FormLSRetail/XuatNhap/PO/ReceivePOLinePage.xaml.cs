using AppName.Model.XuatNhap;
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
    public partial class ReceivePOLinePage : ContentPage
    {
        ReceivePOLineViewModel viewmodel;
        public ReceivePOLinePage(POHeaderModel itemModel)
        {
            InitializeComponent();

            if (BindingContext == null)
            {
                BindingContext = viewmodel = new ReceivePOLineViewModel(Navigation, itemModel);
            }
        }

        protected override void OnAppearing()
        {
            viewmodel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                viewmodel.OpenShipmentPODetailAsync(thebarcode.Substring(0, thebarcode.Length - 1));
            });

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

            base.OnDisappearing();
        }
    }
}