using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using AppName.Core;
using Xamarin.Forms;

namespace AppName
{
    public partial class BarCodeTonKhoDetailPage : PopupPage
    {
        public BarCodeTonKhoDetailPage()
        {
            InitializeComponent();
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
