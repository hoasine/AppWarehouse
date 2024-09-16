using System;
using Com.Datalogic.Device;
using Com.Datalogic.Decode;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Threading.Tasks;
using Java.Util;
using AppName.Core;
using Android.Util;
using Android;
using Plugin.Connectivity;
using Plugin.Media;

namespace AppName.Droid
{
    [Activity(
        Label = "@string/app_name",
        Icon = "@drawable/icon_dafc",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        LaunchMode = LaunchMode.Multiple,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Locale | ConfigChanges.LayoutDirection
        )
    ]
    public class MainActivity : FormsAppCompatActivity, IReadListener
    {
        private Locale _locale;

        private readonly string LOGTAG = typeof(MainActivity).Name;

        BarcodeManager decoder = null;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            await TryToGetPermissions();

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.AppTheme);

            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(savedInstanceState);

            // Inicializando FFImageLoading
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: false);

            Forms.SetFlags("SwipeView_Experimental");

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTQwNUAzMTM4MmUzNDJlMzBGT29sdENza2kyME1jUHpPNVd5enVXY1AvNVZ1SVdPQlVMNUE4R1c1M0FvPQ==");

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Inicializando Xamarin.Essentials
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            await CrossMedia.Current.Initialize();

            // Inicializando Popups
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            GrialKit.Init(this);

            _locale = Resources.Configuration.Locale;

            ReferenceCalendars();

            LoadApplication(new App());
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();

            // If the decoder instance is null, create it.
            if (decoder == null)
            {
                // Remember an onPause call will set it to null.
                decoder = new BarcodeManager();
            }

            // From here on, we want to be notified with exceptions in case of errors.
            ErrorManager.EnableExceptions(true);



            try
            {
                // add our class as a listener
                decoder.AddReadListener(this);
            }
            catch (DecodeException e)
            {
                Log.Error(LOGTAG, "Error while trying to bind a listener to BarcodeManager", e);
            }

            //var isconnected = CrossConnectivity.Current.IsConnected;
            //if (isconnected == false)
            //{
            //    Toast.MakeText(this, "Please check the device has 3G/LTE or WIFI connection enabled.", ToastLength.Short).Show();
            //}
        }

        protected override void OnPause()
        {
            base.OnPause();

            Log.Info(LOGTAG, "onPause");

            // If we have an instance of BarcodeManager.
            if (decoder != null)
            {
                try
                {
                    // Unregister our listener from it and free resources
                    decoder.RemoveReadListener(this);
                }
                catch (Exception e)
                {
                    Log.Error(LOGTAG, "Error while trying to remove a listener from BarcodeManager", e);
                }
            }
        }

        void IReadListener.OnRead(IDecodeResult decodeResult)
        {
            //// Change the displayed text to the current received result.
            //mBarcodeText.Text = decodeResult.Text;
            //mSymbology.Text = decodeResult.BarcodeID.ToString();
            MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", decodeResult.Text);
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            GrialKit.NotifyConfigurationChanged(newConfig);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();

                        }
                        else
                        {
                            //Permission Denied :(
                            Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

                        }
                    }
                    break;
            }

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ReferenceCalendars()
        {
            // When compiling in release, you may need to instantiate the specific
            // calendar so it doesn't get stripped out by the linker. Just uncomment
            // the lines you need according to the localization needs of the app.
            // For instance, in 'ar' cultures UmAlQuraCalendar is required.
            // https://bugzilla.xamarin.com/show_bug.cgi?id=59077

            //new System.Globalization.UmAlQuraCalendar();
            // new System.Globalization.ChineseLunisolarCalendar();
            // new System.Globalization.ChineseLunisolarCalendar();
            // new System.Globalization.HebrewCalendar();
            // new System.Globalization.HijriCalendar();
            // new System.Globalization.IdnMapping();
            // new System.Globalization.JapaneseCalendar();
            // new System.Globalization.JapaneseLunisolarCalendar();
            // new System.Globalization.JulianCalendar();
            // new System.Globalization.KoreanCalendar();
            // new System.Globalization.KoreanLunisolarCalendar();
            // new System.Globalization.PersianCalendar();
            // new System.Globalization.TaiwanCalendar();
            // new System.Globalization.TaiwanLunisolarCalendar();
            // new System.Globalization.ThaiBuddhistCalendar();
        }


        #region RuntimePermissions

        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }


        }
        const int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
            {
                            //TODO add more permissions
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadPhoneState,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.ReadExternalStorage
             };
        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.WriteExternalStorage;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                //set alert for executing the task
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Permissions Needed");
                alert.SetMessage("The application need special permissions to continue");
                alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                {
                    RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();


                return;
            }

            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }

        #endregion
    }
}

