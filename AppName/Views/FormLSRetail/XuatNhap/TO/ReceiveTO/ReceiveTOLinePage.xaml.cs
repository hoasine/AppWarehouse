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
    public partial class ReceiveTOLinePage : ContentPage
    {
        ReceiveTOLineViewModel vewModel;
        public ReceiveTOLinePage(TOHeaderModel itemModel)
        {
            InitializeComponent();

            if (BindingContext == null)
            {
                BindingContext = vewModel = new ReceiveTOLineViewModel(Navigation, itemModel);
            }
        }

        protected override void OnAppearing()
        {
            vewModel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                if (vewModel.IsScan == false)
                {
                    vewModel.IsScan = true;

                    vewModel.OpenShipmentTODetailAsync(thebarcode.Substring(0, thebarcode.Length - 1));
                }
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