using System.Collections.ObjectModel;

using System.Globalization;
using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using AppName.Model;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppName
{
    public class SanPhamListViewModel : BaseViewModel
    {
        private NavigationCategoryData _category;
        private NavigationItemData _selectedItem;
        protected INavigation Navigation { get; private set; }


        public SanPhamListViewModel(INavigation navigation)
        {
            Navigation = navigation;
            LoadData();
        }


        private ObservableCollection<NavigationItemData> items;
        public ObservableCollection<NavigationItemData> Items
        {
            get => items;

            set => SetProperty(ref items, value);
        }


        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }


        public NavigationCategoryData Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }


        private bool _isExecuting;
        public bool isExecuting
        {
            get { return _isExecuting; }
            set { SetProperty(ref _isExecuting, value); }
        }

        public NavigationItemData SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    if (isExecuting)
                        return;

                    isExecuting = true;

                    try
                    {
                        if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
                        {
                            if (value.MenuID == "SanPham")
                            {
                                Navigation.PushAsync(new BarocdeScanSanpPhamSearch());
                            }
                            else if (value.MenuID == "TonKho")
                            {
                                Navigation.PushAsync(new TonKhoPage());
                            }
                            else if (value.MenuID == "Relable")
                            {
                                Navigation.PushAsync(new ReLablePage());
                            }
                            else if (value.MenuID == "RelablePDF")
                            {
                                Navigation.PushAsync(new PrintPDFPage());
                            }
                            else if (value.MenuID == "ChangePricePromotion")
                            {
                                Navigation.PushAsync(new BarcodeMarkdown());
                            }
                            else if (value.MenuID == "PickUP")
                            {
                                Navigation.PushAsync(new PickUPPage());
                            }
                            else if (value.MenuID == "CheckItemDay")
                            {
                                Navigation.PushAsync(new CheckItemDayPage());
                            }
                            else if (value.MenuID == "ViewPOG")
                            {
                                Navigation.PushAsync(new POGWithItemPage());
                            }
                            else if (value.MenuID == "PrintPDFExpireDate")
                            {
                                Navigation.PushAsync(new PrintPDFExpireDatePage());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                    }
                    finally
                    {
                        isExecuting = false;
                    }

                    SetProperty(ref _selectedItem, null);
                }
            }
        }

        public async void ClickMenu(NavigationItemData value)
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
            {
                if (value.MenuID == "SanPham")
                {
                    Navigation.PushAsync(new BarocdeScanSanpPhamSearch());
                }
                else if (value.MenuID == "TonKho")
                {
                    Navigation.PushAsync(new TonKhoPage());
                }
                else if (value.MenuID == "Relable")
                {
                    Navigation.PushAsync(new ReLablePage());
                }
                else if (value.MenuID == "RelablePDF")
                {
                    Navigation.PushAsync(new PrintPDFPage());
                }
                else if (value.MenuID == "ChangePricePromotion")
                {
                    Navigation.PushAsync(new BarcodeMarkdown());
                }
                else if (value.MenuID == "PickUP")
                {
                    Navigation.PushAsync(new PickUPPage());
                }
                else if (value.MenuID == "CheckItemDay")
                {
                    Navigation.PushAsync(new CheckItemDayPage());
                }
                else if (value.MenuID == "ViewPOG")
                {
                    Navigation.PushAsync(new POGWithItemPage());
                }
                else if (value.MenuID == "PrintPDFExpireDate")
                {
                    Navigation.PushAsync(new PrintPDFExpireDatePage());
                }
            }
        }

        private void LoadData()
        {
            Category = null;

            var checkKiemTraSanPhamView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "KiemTraSanPham"
                && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkKiemTraTonView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "KiemTraTon"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkInTemGiaBluetoothView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "InTemGiaBluetooth"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkInTemGiaPDFView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "InTemGiaPDF"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkPickUPView = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "PickUP"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkCheckItemDay = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "CheckItemDay"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var checkPrintExpireDate = RealmHelper.Instance.All<LocalPermissionModel>().ToArray().Any(q => q.KeyPermission == "PrintPDFExpireDate"
          && !string.IsNullOrWhiteSpace(q.Role) && q.Role.Contains("VIEW"));

            var list = new List<NavigationItemData>();

            if (checkKiemTraSanPhamView == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkKiemTraSanPhamView,
                    MenuID = "SanPham",
                    Name = "Product Management",
                    BackgroundColor = "#FF7C4ECD",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.ShoppingBag,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Look up product information"
                });
            }

            if (checkKiemTraTonView == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkKiemTraTonView,
                    MenuID = "TonKho",
                    Name = "Inventory management",
                    BackgroundColor = "#D8B41F",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.Database,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Inventory by product at stores"
                });
            }

            if (checkInTemGiaBluetoothView == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkInTemGiaBluetoothView,
                    MenuID = "Relable",
                    Name = "Print price tag",
                    BackgroundColor = "#2EAFBC",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.Bluetooth,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Reprint new price stamps"
                });
            }

            if (checkInTemGiaPDFView == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkInTemGiaPDFView,
                    MenuID = "RelablePDF",
                    Name = "Print term PDF",
                    BackgroundColor = "#BE1919",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.Printer,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Print term with file template PDF"
                });
            }

            if (checkPickUPView == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkPickUPView,
                    MenuID = "PickUP",
                    Name = "Pickup Product",
                    BackgroundColor = "#2E8FBC",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.ShoppingCart,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Scan the list of goods to export the report"
                });
            }

            if (checkPrintExpireDate == true)
            {
                list.Add(new NavigationItemData()
                {
                    IsVisible = checkPrintExpireDate,
                    MenuID = "PrintPDFExpireDate",
                    Name = "Print QRCode expire date",
                    BackgroundColor = "#3D934E",
                    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                    Icon = GrialIconsFont.LogoGrial,
                    ItemCount = 19,
                    Badge = 9,
                    Tittle = "Print product expiration QRCode stamp"
                });
            }
         
            Items = new ObservableCollection<NavigationItemData>(list);

            //{

            //,   new NavigationItemData()
            //    {
            //     IsVisible =checkCheckItemDay,
            //        MenuID = "CheckItemDay",
            //            Name = "Check sales in 133 days",
            //            BackgroundColor = "#46BC2E",
            //            BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
            //            Icon = GrialIconsFont.Check,
            //            ItemCount = 19,
            //            Badge = 9,
            //            Tittle = "Check the number of sales for the specified day"
            //    }
            //,   new NavigationItemData()
            //    {
            //     IsVisible = true,
            //        MenuID = "ViewPOG",
            //            Name = "POG with items",
            //            BackgroundColor = "#2EBC5B",
            //            BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
            //            Icon = GrialIconsFont.List,
            //            ItemCount = 19,
            //            Badge = 9,
            //            Tittle = "View pog with items"
            //    }
            //};
        }
    }
}