using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using System.Windows.Input;
using AppName.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace AppName
{
    public class PrintPDFExpireDateViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }
        public ICommand NavigateCommand { get; set; }

        public PrintPDFExpireDateViewModel(INavigation navigation)
        {
            NavigateCommand = new Command<string>(NavigateAsync);

            Navigation = navigation;

            var listPermission = RealmHelper.Instance.All<LocalPermissionModel>().ToArray();
        }

        private async void NavigateAsync(string pageName)
        {
            switch (pageName)
            {

                case "QRExpireDateWithPromotion":
                    await Navigation.PushAsync(new PrintPDFExpireDateWithPromotionPage());
                    break;
                case "QRExpireDateWithImportExcel":
                    await Navigation.PushAsync(new ReLablePDFDiscounrWithImportExcel());
                    break;
               
                default:
                    break;
            }
        }
    }
}