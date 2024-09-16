using Foundation;
using UIKit;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using AppName.Core;

namespace AppName.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            UIApplication.SharedApplication.IdleTimerDisabled = true;

            var ignore = typeof(SvgCachedImage);
            CachedImageRenderer.Init(); 

            CarouselView.FormsPlugin.iOS.CarouselViewRenderer.Init(); 

            Rg.Plugins.Popup.Popup.Init();

            GrialKit.Init(new ThemeColors());

            // Xamarin Test Cloud Agent
            #if ENABLE_TEST_CLOUD
                Xamarin.Calabash.Start();
            #endif

            FormsHelper.ForceLoadingAssemblyContainingType<FFImageLoading.Transformations.BlurredTransformation>();

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
