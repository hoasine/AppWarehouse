using System.Collections.ObjectModel;

using System.Globalization;
using Xamarin.Forms;
using AppName.Core;
using System;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using AppName.Model;

namespace AppName
{
    public class MenuKiemKeViewModel : ObservableObject
    {
        private NavigationCategoryData _category;
        private NavigationItemData _selectedItem;
        protected INavigation Navigation { get; private set; }
        //public MenuKiemKeViewModel(string variantPageName = null)
        //    : base(listenCultureChanges: true)
        //{
        //    LoadData();
        //}

        public MenuKiemKeViewModel(INavigation navigation)
             : base(listenCultureChanges: true)
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
                        if (value.MenuID == "KiemKe")
                        {
                            Navigation.PushAsync(new KiemKeCreatePage());
                        }
                        else if (value.MenuID == "ListKiemKe")
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

        private void LoadData()
        {
            Category = null;

            Items = new ObservableCollection<NavigationItemData>()
            {
               new NavigationItemData()
                {
                    MenuID = "ListKiemKe",
                      Name = "Danh sách phiếu kê",
                     BackgroundColor = "#FF7C4ECD",
                     BackgroundImage  = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_02.jpg",
                     Icon = "",
                     ItemCount = 19,
                     Badge = 9,
                     Tittle = "Phiếu kiểm theo khu vực"
                }
            };

            //new NavigationItemData()
            //{
            //    MenuID = "KiemKe",
            //    Name = "Kiểm kê",
            //    BackgroundColor = "#FF5F7DD4",
            //    BackgroundImage = "https://s3-us-west-2.amazonaws.com/grial-images/v3.0/category_09.jpg",
            //    Icon = "",
            //    ItemCount = 19,
            //    Badge = 9,
            //    Tittle = "Tạo phiếu kiểm theo khu vực"
            //},

        }
    }
}