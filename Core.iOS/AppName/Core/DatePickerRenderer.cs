using Foundation;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace AppName.Core
{
    public class DatePickerRenderer : Xamarin.Forms.Platform.iOS.DatePickerRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var langStr = NSLocale.CurrentLocale.LanguageCode;

                UITextField entry = Control;
                UIDatePicker picker = (UIDatePicker)entry.InputView;
                picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
                picker.Locale = new NSLocale("vi");
            }
        }
    }
}
