using AppName.Model.Pickup;
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
    public partial class PickUPDetail : PopupPage
    {
        private PickUpProductMaster tmpItemModel;

        public PickUPDetail(PickUpProductMaster itemModel)
        {
            InitializeComponent();

            tmpItemModel = itemModel;

            BindingContext = new PickupDetailViewModel(Navigation, itemModel);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
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

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PickUPLinePage(tmpItemModel));

            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}