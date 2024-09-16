﻿using AppName.Model.XuatNhap;
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
    public partial class ShipmentTOPage : ContentPage
    {
        public ShipmentTOPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ShipmentTOViewModel(Navigation);
        }

        ShipmentTOViewModel viewModel;

        protected override void OnAppearing()
        {
            viewModel.LoadItemsCommand.Execute(null);

            base.OnAppearing();
        }


        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var dialog = new ShipmentTODetail((TOHeaderModel)e.SelectedItem);

            PopupNavigation.Instance.PushAsync(dialog);

            listView.SelectedItem = null;
        }
    }
}