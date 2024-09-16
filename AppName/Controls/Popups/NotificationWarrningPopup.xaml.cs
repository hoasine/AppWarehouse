using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using AppName.Core;

namespace AppName
{
    public partial class NotificationWarrningPopup : PopupPage
    {
        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(
                nameof(Message),
                typeof(string),
                typeof(NotificationPopup),
                defaultValue: string.Empty);

        public string ClassName { get; set; }
        public string ActionName { get; set; }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set
            {
                SetValue(MessageProperty, value);

                if(!string.IsNullOrEmpty(value))
                {
                    ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                    logger.Error("Class:=" + (!string.IsNullOrEmpty(ClassName) ? ClassName.Replace("ViewModel", "") : "") + " Action:=" + ActionName + " Content:= " + value, "Error");
                }
            }
        }

        public NotificationWarrningPopup(string className, string actionName)
        {
            InitializeComponent();

            ClassName = className;
            ActionName = actionName;
        }

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception) { }
        }
    }
}
