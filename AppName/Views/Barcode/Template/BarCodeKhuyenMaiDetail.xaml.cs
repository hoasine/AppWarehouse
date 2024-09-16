using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using AppName.Core;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace AppName
{
    public partial class BarCodeKhuyenMaiDetail : PopupPage
    {
        public BarCodeKhuyenMaiDetail(CTKMsGroupViewModel viewModel)
        {
            InitializeComponent();
        }

        private CTKMsGroupViewModel ViewModel
        {
            get { return (CTKMsGroupViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (ViewModel.ListCTKM.Count == 0)
                {
                    ViewModel.LoadCTKMItemCommand.Execute(null);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
