using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AppName;
using AppName.Droid.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContextMenuButton), typeof(ContextMenuButtonRenderer))]
namespace AppName.Droid.Renderers
{
    public class ContextMenuButtonRenderer : ButtonRenderer
    {
        public ContextMenuButtonRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Element is ContextMenuButton contextButton)
            {
                contextButton.GetCoordinates = GetCoordinatesNative;
            }
        }

        private (int x, int y) GetCoordinatesNative()
        {
            var displayMetrics = Resources.DisplayMetrics;
            var density = displayMetrics.Density;

            var screenHeight = displayMetrics.HeightPixels;
            var appHeight = Xamarin.Forms.Application.Current.MainPage.Height;
            var heightOffset = screenHeight / density - appHeight;

            var coords = new int[2];
            GetLocationOnScreen(coords);
            return ((int)(coords[0] / density), (int)(coords[1] / density) - (int)heightOffset);
        }
    }
}