using System;
using Xamarin.Forms;
using AppName.Core;

namespace AppName
{
    public partial class PasswordRecoveryPage : ContentPage
    {
        public PasswordRecoveryPage()
        {
            InitializeComponent();

            grialNavigationBar.HeightRequestBeyondNativeBar = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height;
        }
    }
}