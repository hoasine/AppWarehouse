using AppName.Model.XuatNhap;
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
    public partial class CycleCountDetail : PopupPage
    {
        private StockCountModel tmpItemModel;

        CycleCountDetailViewModel viewModel;

        public CycleCountDetail(StockCountModel itemModel)
        {
            InitializeComponent();

            tmpItemModel = itemModel;

            BindingContext = viewModel = new CycleCountDetailViewModel(Navigation, itemModel);
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
            try
            {
                viewModel.ShowLoading = true;

                await Task.Delay(10);

                PopupNavigation.Instance.PopAllAsync();

                await Navigation.PushAsync(new CycleCountPreviewPage(tmpItemModel));
            }
            catch (Exception ex)
            {
                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog);
            }
            finally
            {
                viewModel.ShowLoading = false;
            }

        }
    }
}