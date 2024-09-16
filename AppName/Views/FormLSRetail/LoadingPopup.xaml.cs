using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopup : PopupPage
    {
        public LoadingPopup()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<App>((App)Xamarin.Forms.Application.Current, "Loading");

            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "Loading", async (sender) =>
            {
               await clospage();
            });

            animatepage1();
            animatepage2();
            animatepage3();
        }
        async void animatepage1()
        {
            await Image1.RelRotateTo(360, 50000);
        }
        async void animatepage2()
        {
            await Image2.RelRotateTo(360, 50000);
        }
        async void animatepage3()
        {
            await Image3.RelRotateTo(360, 50000);
        }
        async Task clospage()
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }
        protected override Task OnAppearingAnimationEndAsync()
        {
            return Content.FadeTo(0.5);
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return Content.FadeTo(1);
        }
    }
}