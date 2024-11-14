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
    public class PrintPDFViewModel : ObservableObject
    {
        public ICommand POCommand { get; set; }
        public ICommand TOShipmentCommand { get; set; }
        public ICommand TOReceivingCommand { get; set; }

        private bool _visibleShipmentTO;
        public bool VisibleShipmentTO
        {
            get { return _visibleShipmentTO; }
            set { SetProperty(ref _visibleShipmentTO, value); }
        }

        private NavigationCategoryData _category;
        private NavigationItemData _selectedItem;
        protected INavigation Navigation { get; private set; }

        public PrintPDFViewModel(INavigation navigation)
        {
            NavigateCommand = new Command<string>(NavigateAsync);

            POCommand = new Command(OpenPOCommandPage);
            TOShipmentCommand = new Command(OpenTOShipmentCommandPage);
            TOReceivingCommand = new Command(OpenTOReceivingCommandPage);

            Navigation = navigation;

            var listPermission = RealmHelper.Instance.All<LocalPermissionModel>().ToArray();

            VisibleShipmentTO = listPermission.Any(q => q.KeyPermission == "Shipment"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            LoadData();
        }

        private async void OpenPOCommandPage()
        {
            await Navigation.PushAsync(new ReceivePOPage());
        }

        private async void OpenTOShipmentCommandPage()
        {
            await Navigation.PushAsync(new ShipmentTOPage());
        }

        private CountTOPOModel _CountTOPO;
        public CountTOPOModel CountTOPO
        {
            get { return _CountTOPO; }
            set { SetProperty(ref _CountTOPO, value); }
        }

        private async void OpenTOReceivingCommandPage()
        {
            await Navigation.PushAsync(new ReceiveTOPage());
        }


        private ObservableCollection<NavigationItemData> items;
        public ObservableCollection<NavigationItemData> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }

        public ICommand NavigateCommand { get; set; }

        public NavigationCategoryData Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public NavigationItemData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
                    {
                        if (value.MenuID == "NhapKho")
                        {

                            Navigation.PushAsync(new KiemKeViewPage());
                        }
                        else if (value.MenuID == "XuatKho")
                        {

                            Navigation.PushAsync(new KiemKeViewPage());
                        }
                    }

                    SetProperty(ref _selectedItem, null);
                }
            }
        }

        protected override void OnCultureChanged(CultureInfo culture)
        {
            LoadData();
        }

        public class CountTOPOModel
        {
            public int ReceiveTO { get; set; }
            public int ShipmentTO { get; set; }
            public int ReceivePO { get; set; }
        }

        private void LoadData()
        {
            Category = null;

            Items = new ObservableCollection<NavigationItemData>()
            {
               new NavigationItemData()
                {
                    MenuID = "NhapKho",
                      Name = "Nhập kho",
                     BackgroundColor = "#FF7C4ECD",
                     BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                     Icon = "",
                     ItemCount = 19,
                     Badge = 9,
                     Tittle = "Danh sách nhập kho"
                },
                new NavigationItemData()
                {
                    MenuID = "XuatKho",
                        Name = "Xuất kho",
                        BackgroundColor = "#FF7C4ECD",
                        BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                        Icon = "",
                        ItemCount = 19,
                        Badge = 9,
                        Tittle = "Danh sách nhập kho"
                }
            };
        }

        private async void NavigateAsync(string pageName)
        {
            switch (pageName)
            {

                #region Scan prices

                case "ScanPriceTag":
                    await Navigation.PushAsync(new ReLablePDFSmallPage());
                    break;
                case "ScanShelftaker":
                    await Navigation.PushAsync(new ReLablePDFShelfTalkerPage());
                    break;
                case "ScanMSC":
                    await Navigation.PushAsync(new ReLablePDFPage());
                    break;
                case "ScanMSCDumpin":
                    await Navigation.PushAsync(new ReLablePDFMSCDumpinPage());
                    break;

                #endregion  

                case "PriceTagPOG":
                    await Navigation.PushAsync(new ReLablePDFSmallWithPOGPage());
                    break;
                case "ShelfTakerWithPOG":
                    await Navigation.PushAsync(new ReLablePDFShelfTalkerWithPOGPage());
                    break;
                case "PriceTagImportExcel":
                    await Navigation.PushAsync(new ReLablePDFSmallWithImportExcel());
                    break;
                case "ShelfTakerWithExcel":
                    await Navigation.PushAsync(new ReLablePDFShelfTalkerWithImportExcel());
                    break;
                case "PrintMSCMulti":
                    await Navigation.PushAsync(new ReLablePDFMultiPage());
                    break;
                case "ShelfTakerMulti":
                    await Navigation.PushAsync(new ReLablePDFShelfTalker_MultiPage());
                    break;
                case "PrintWithPromotion":
                    await Navigation.PushAsync(new ReLablePDFSmallWithPromotionPage());
                    break;
                case "ShelfTakerMDWithExcel":
                    await Navigation.PushAsync(new ReLablePDFDiscounrWithImportExcel());
                    break;
                case "PriceTagMDImportExcel":
                    await Navigation.PushAsync(new ReLablePDFDiscounrWithImportExcel());
                    break;

                default:
                    break;
            }
        }

    }
}