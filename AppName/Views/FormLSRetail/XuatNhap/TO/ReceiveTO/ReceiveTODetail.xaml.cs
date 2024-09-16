﻿using AppName.Model.XuatNhap;
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
    public partial class ReceiveTODetail : PopupPage
    {
        private TOHeaderModel tmpItemModel;

        public ReceiveTODetail(TOHeaderModel itemModel)
        {
            InitializeComponent();

            tmpItemModel = itemModel;

            BindingContext = new ReceiveTODetailViewModel(Navigation, itemModel);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReceiveTOLinePage(tmpItemModel));

            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}