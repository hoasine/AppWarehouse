using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using AppName.Core;

namespace AppName
{
    public partial class NotificationLicensePopup : PopupPage
    {
        public NotificationLicensePopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Khi bấm back button trên điện thoại sẽ ko ẩn popup
        /// </summary>
        /// <returns></returns>
        //protected override bool OnBackButtonPressed()
        //{
        //    return false;
        //}
    }
}
