using Xamarin.Forms;

namespace AppName.Core
{
	public class SwitchProperties
	{
		public static readonly BindableProperty TintColorProperty = BindableProperty.CreateAttached("TintColor", typeof(Color), typeof(SwitchProperties), Color.Default);

		public static Color GetTintColor(BindableObject bo)
		{
			return (Color)bo.GetValue(TintColorProperty);
		}

		public static void SetTintColor(BindableObject bo, Color value)
		{
			bo.SetValue(TintColorProperty, value);
		}
	}
}
