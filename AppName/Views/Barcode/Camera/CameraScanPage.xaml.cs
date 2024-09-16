using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using AppName.Core;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using AppName.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Permissions;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Scandit.BarcodePicker.Unified;

namespace AppName
{
    public partial class CameraScanPage : PopupPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(BarocdeScanSanpPham),
        null);

        public Command<List<string>> DidScannedCommand
        {
            get { return (Command<List<string>>)GetValue(DidScannedCommandProperty); }
            set { SetValue(DidScannedCommandProperty, value); }
        }

        #endregion

        public CameraScanPage()
        {
            InitializeComponent();

            DidScannedCommand = new Command<List<string>>(OnDidScanned);
            scanedCamera.DidScannedCommand = DidScannedCommand;

            if (Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await CrossMedia.Current.Initialize();

                        var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                        if (cameraStatus != PermissionStatus.Granted)
                        {
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                            if (results != null)
                                cameraStatus = results[Permission.Camera];
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
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


        protected override void OnAppearing()
        {
            base.OnAppearing();

            scanedCamera.CreateScanning?.Invoke();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            scanedCamera.StopScanning?.Invoke();
        }

        void OnDidScanned(List<string> scannedCodes)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var dialog = new BarCodeSanPhamDetail();
                //dialog.BindingContext = new BarCodeSanPhamViewModel(scannedCodes?.LastOrDefault());

                dialog.BindingContext = new BarCodeSanPhamViewModel("1000000");

                Navigation.PopModalAsync();

                PopupNavigation.Instance.PushAsync(dialog);
            });
        }
    }
}
