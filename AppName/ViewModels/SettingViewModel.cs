using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using AppName.Core;
using AppName.Model;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;

namespace AppName
{
    public class SettingViewModel : BaseViewModel
    {
        public EventHandler<BluetoothConnetedModel> ClosePopup;

        private string _template;
        public string Template
        {
            get => _template;

            set => SetProperty(ref _template, value);
        }

        public ICommand CloseCommand { get; set; }

        private bool _IStagtamplate;
        public bool IStagtamplate
        {
            get { return _IStagtamplate; }
            set
            {
                SetProperty(ref _IStagtamplate, value);

                if (value == true)
                {
                    ISshelfTemplate = false;
                    ISISCTemplate = false;
                }
            }
        }

        private bool _ISshelfTemplate;
        public bool ISshelfTemplate
        {
            get { return _ISshelfTemplate; }
            set
            {
                SetProperty(ref _ISshelfTemplate, value);

                if (value == true)
                {
                    IStagtamplate = false;
                    ISISCTemplate = false;
                }
            }
        }

        private bool _IsShowConnected;
        public bool IsShowConnected
        {
            get { return _IsShowConnected; }
            set
            {
                SetProperty(ref _IsShowConnected, value);
            }
        }

        private bool _ISISCTemplate;
        public bool ISISCTemplate
        {
            get { return _ISISCTemplate; }
            set
            {
                SetProperty(ref _ISISCTemplate, value);

                if (value == true)
                {
                    ISshelfTemplate = false;
                    IStagtamplate = false;
                }
            }
        }

        public ICommand BluetoothCommad { get; set; }
        public ICommand DiscoinnectCommand { get; set; }
        public ICommand OnCloseCommand { get; set; }

        private BluetoothConnetedModel _DataBluetoothConneted;
        public BluetoothConnetedModel DataBluetoothConneted
        {
            get => _DataBluetoothConneted;

            set => SetProperty(ref _DataBluetoothConneted, value);
        }

        private ObservableCollection<BluetoothModel> _bluetoothList;
        public ObservableCollection<BluetoothModel> BluetoothList
        {
            get => _bluetoothList;

            set => SetProperty(ref _bluetoothList, value);
        }

        public SettingViewModel(BluetoothConnetedModel obj)
        {
            Template = obj.Template;

            if (Template == "IStagtamplate")
            {
                IStagtamplate = true;
                ISshelfTemplate = false;
                ISISCTemplate = false;
            }
            else if (Template == "ISshelfTemplate")
            {
                IStagtamplate = false;
                ISshelfTemplate = true;
                ISISCTemplate = false;
            }
            else if (Template == "ISISCTemplate")
            {
                IStagtamplate = false;
                ISshelfTemplate = false;
                ISISCTemplate = true;
            }

            DataBluetoothConneted = obj;

            LoadData();

            if (!string.IsNullOrEmpty(DataBluetoothConneted.MacID))
            {
                var check = BluetoothList.Where(s => s.MacID == obj.MacID).FirstOrDefault();

                BluetoothList.Remove(check);
                IsShowConnected = true;
            }
            else
            {
                IsShowConnected = false;
            }

            CloseCommand = new Command(CloseAsync);
            DiscoinnectCommand = new Command(DiscoinnectAsync);
            BluetoothCommad = new Command<BluetoothModel>(BluetoothCommadAsync);

            Xamarin.Forms.MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "DisconnectBLue", (sender) =>
            {
                Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().DisconnectPrinter();
                Xamarin.Forms.MessagingCenter.Unsubscribe<App>((App)Xamarin.Forms.Application.Current, "DisconnectBLue");
            });
        }

        private async void BluetoothCommadAsync(BluetoothModel obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!string.IsNullOrEmpty(DataBluetoothConneted.MacID))
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please disconnect bluetooth " + DataBluetoothConneted.BlueName + " connection before connecting new device." };
                        PopupNavigation.Instance.PushAsync(dialog, false);

                        return;
                    }
                    else
                    {
                        ConnectBlue(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async void DiscoinnectAsync()
        {
            LoadingPopup page1 = new LoadingPopup();
            await PopupNavigation.Instance.PushAsync(page1);

            try
            {
                var statusDisconnect = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().DisconnectPrinter();

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {
                }

                if (statusDisconnect == true)
                {
                    DataBluetoothConneted.MacID = "";
                    DataBluetoothConneted.BlueName = "";
                    DataBluetoothConneted.IsBluetooth = false;
                    DataBluetoothConneted.IsConnect = false;

                    if (!string.IsNullOrEmpty(DataBluetoothConneted.MacID))
                    {
                        IsShowConnected = true;
                    }
                    else
                    {
                        IsShowConnected = false;
                    }

                    LoadData();

                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "Disconnect the print device " + DataBluetoothConneted.BlueName + " successfully." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please check Bluetooth enabled or enabled printing device." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {
                }
            }
        }

        public async void ConnectBlue(BluetoothModel obj)
        {
            LoadingPopup page1 = new LoadingPopup();
            await PopupNavigation.Instance.PushAsync(page1);

            try
            {
                var status = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().ConnectPrinter(obj.MacID);

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {
                }

                if (status == true)
                {
                    DataBluetoothConneted.MacID = obj.MacID;
                    DataBluetoothConneted.BlueName = obj.BlueName;
                    DataBluetoothConneted.IsBluetooth = true;
                    DataBluetoothConneted.IsConnect = true;

                    BluetoothList.Remove(obj);

                    if (!string.IsNullOrEmpty(DataBluetoothConneted.MacID))
                    {
                        IsShowConnected = true;
                    }
                    else
                    {
                        IsShowConnected = false;
                    }

                    var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "Connect the print device " + obj.BlueName + " successfully." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please check Bluetooth enabled or enabled printing device." };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {
                }
            }
        }

        private void LoadData()
        {
            if (IsBusy)
                return;
            IsBusy = true;


            try
            {
                var allListsSource = new ObservableCollection<BluetoothModel>();

                allListsSource = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().GetListBluetooth();

                BluetoothList = new ObservableCollection<BluetoothModel>(allListsSource);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async void CloseAsync(object obj)
        {
            string template = "";

            if (IStagtamplate == true)
            {
                Template = "IStagtamplate";
            }
            else if (ISISCTemplate == true)
            {
                Template = "ISISCTemplate";
            }
            else
            {
                Template = "ISshelfTemplate";
            }

            DataBluetoothConneted.Template = Template;

            ClosePopup?.Invoke(this, DataBluetoothConneted);

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
