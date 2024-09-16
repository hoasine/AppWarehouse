using System;
using Xamarin.Forms;
using AppName.Core;

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
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;

namespace AppName
{
    public partial class BarCodeKiemKe : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(BarCodeTonKho),
        null);

        public Command<List<string>> DidScannedCommand
        {
            get { return (Command<List<string>>)GetValue(DidScannedCommandProperty); }
            set { SetValue(DidScannedCommandProperty, value); }
        }
        #endregion

        public BarCodeKiemKe()
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
                var dialog = new BarCodeTonKhoDetail();
                dialog.BindingContext = new BarCodeKiemKeViewModel(scannedCodes?.LastOrDefault());

                Navigation.PopModalAsync();

                PopupNavigation.Instance.PushAsync(dialog);
            });
        }
    }
}