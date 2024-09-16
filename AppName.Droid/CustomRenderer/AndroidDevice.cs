using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MWBarcodeScanner;
using System.Drawing;
using Android.Content.PM;
using AppName.CustomRenderer;
using AppName.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using ScanditBarcodePicker.Android;
using System.Threading;
using Android.Util;
using System.IO;
using Android.Graphics;
using Xamarin.Forms;
using static Android.Provider.Settings;
using Android.Support.Design.Widget;
using Android.Graphics.Drawables;

[assembly: Xamarin.Forms.Dependency(typeof(XFUniqueIdentifier.Droid.AndroidDevice))]
namespace XFUniqueIdentifier.Droid
{
    public class AndroidDevice : IDevice
    {
        public string GetIdentifier()
        {
            String deviceId;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                deviceId = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            }
            else
            {
                Android.Telephony.TelephonyManager mTelephonyMgr;
                mTelephonyMgr = (Android.Telephony.TelephonyManager)Forms.Context.GetSystemService(Android.Content.Context.TelephonyService);

                deviceId = mTelephonyMgr.DeviceId;
            }

            return deviceId;
        }

        public void ShowAlert(string content, int time)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Toast toast = Toast.MakeText(Android.App.Application.Context, content, ToastLength.Short);
                Android.Views.View view = toast.View;

                //view.SetBackgroundColor(Android.Graphics.Color.Rgb(236, 80, 80));
                toast.View.Background.SetColorFilter(Android.Graphics.Color.ParseColor("#f24242"), PorterDuff.Mode.SrcIn);
                TextView text = (TextView)view.FindViewById(Android.Resource.Id.Message);
                text.SetTextColor(Android.Graphics.Color.White);
                toast.SetGravity(GravityFlags.NoGravity, 0, -50);
                toast.SetMargin(0, 10);
                toast.Show();



                //GradientDrawable shape = new GradientDrawable();
                //shape.SetCornerRadius(15); //For Corner radius
                //shape.SetColor(Android.Graphics.Color.Rgb(236, 80, 80));
                //var contentView = Xamarin.Essentials.Platform.CurrentActivity?.FindViewById(Android.Resource.Id.Content);
                //Snackbar snackBar = Snackbar.Make(contentView, content, time);
                //Android.Views.View view = snackBar.View;
                //view.SetBackground(shape);
                //FrameLayout.LayoutParams frameLayout = (FrameLayout.LayoutParams)view.LayoutParameters;
                //frameLayout.Gravity = GravityFlags.Bottom;
                //frameLayout.BottomMargin = 20;
                //frameLayout.LeftMargin = 20;
                //view.LayoutParameters = frameLayout;
                //snackBar.SetBehavior(new BaseTransientBottomBar.Behavior());
                //snackBar.Show();


                //GradientDrawable shape = new GradientDrawable();
                //shape.SetCornerRadius(20);
                //shape.SetColor(Android.Graphics.Color.Rgb(236, 80, 80));
                //var contentView = Xamarin.Essentials.Platform.CurrentActivity?.FindViewById(Android.Resource.Id.Content);

                //Snackbar snackBar = Snackbar.Make(contentView, content, time);

                //var a = new BaseTransientBottomBar.Behavior();
                //a.SetSwipeDirection(50);
                //a.SetStartAlphaSwipeDistance(50);
                //snackBar.SetBehavior(a);

                //Android.Views.View view = snackBar.View;
                //view.SetBackground(shape);

                //FrameLayout.LayoutParams frameLayout = (FrameLayout.LayoutParams)view.LayoutParameters;
                //frameLayout.Gravity = GravityFlags.Bottom;
                //frameLayout.SetMargins(200, 200, 200, 200);
                //view.LayoutParameters = frameLayout;
                //snackBar.Show();
            });
        }
    }
}