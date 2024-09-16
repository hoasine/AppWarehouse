using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using AppName.Core;
using AppName.Model;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using static AppName.RelableTermPDFSmallViewModel;

namespace AppName
{
    public class SettingPrintSaleTakerModel : BaseViewModel
    {
        public EventHandler<TemplatePrinterValueModel> ClosePopup;

        public ICommand CloseCommand { get; set; }

        private TemplatePrinterValueModel _TemplatePrinterValue;
        public TemplatePrinterValueModel TemplatePrinterValue
        {
            get => _TemplatePrinterValue;

            set => SetProperty(ref _TemplatePrinterValue, value);
        }

        public SettingPrintSaleTakerModel(TemplatePrinterValueModel obj)
        {
            TemplatePrinterValue = obj;
            CloseCommand = new Command(CloseAsync);
        }

        private async void CloseAsync(object obj)
        {
            if (string.IsNullOrEmpty(TemplatePrinterValue.Top))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "PLease input Top margin value." };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }

            if (string.IsNullOrEmpty(TemplatePrinterValue.Left))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "PLease input Left margin value." };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }

            if (string.IsNullOrEmpty(TemplatePrinterValue.Right))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "PLease input Right margin value." };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }

            if (string.IsNullOrEmpty(TemplatePrinterValue.Bottom))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "PLease input Bottom margin value." };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }

            ClosePopup?.Invoke(this, TemplatePrinterValue);

            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
