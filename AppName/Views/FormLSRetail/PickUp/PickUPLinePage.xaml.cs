﻿using AppName.Model.Pickup;
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
    public partial class PickUPLinePage : ContentPage
    {
        PickupLineViewModel vewModel;
        public PickUPLinePage(PickUpProductMaster itemModel)
        {
            InitializeComponent();

            if (BindingContext == null)
            {
                BindingContext = vewModel = new PickupLineViewModel(Navigation, itemModel);
            }
        }

        protected override void OnAppearing()
        {
            if (vewModel.PickupLine.Count == 0)
            {
                vewModel.LoadItemsCommand.Execute(null);
            }

            MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
            {
                if (vewModel.IsScan == false)
                {
                    vewModel.IsScan = true;

                    vewModel.CheckStockLine(thebarcode.Substring(0, thebarcode.Length - 1));
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