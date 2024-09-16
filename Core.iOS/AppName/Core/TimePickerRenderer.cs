using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace AppName.Core
{
    public class TimePickerRenderer : Xamarin.Forms.Platform.iOS.TimePickerRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                UITextField entry = Control;
                UIDatePicker picker = (UIDatePicker)entry.InputView;
                picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
            }
        }
    }
}
