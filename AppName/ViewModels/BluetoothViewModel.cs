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
    public class BluetoothViewModel : ObservableObject
    {
        public EventHandler<BluetoothConnetedModel> ClosePopup;

        public ICommand BluetoothCommad { get; set; }
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

        public BluetoothViewModel(BluetoothConnetedModel obj) : base(listenCultureChanges: true)
        {
            BluetoothCommad = new Command<BluetoothModel>(BluetoothCommadAsync);
            OnCloseCommand = new Command<string>(OnCloseCommandAsync);

            DataBluetoothConneted = obj;

            LoadData();

            var check = BluetoothList.Where(s => s.MacID == DataBluetoothConneted.MacID).FirstOrDefault();
            if (check != null && !string.IsNullOrEmpty(check.MacID))
            {
                BluetoothList.Remove(check);

                check.IsConnect = DataBluetoothConneted.IsConnect;
                BluetoothList.Insert(0, check);
            }
        }

        private async void BluetoothCommadAsync(BluetoothModel obj)
        {
            var checkConnted = BluetoothList.Where(s => s.IsConnect == true).FirstOrDefault();
            if (checkConnted != null)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please close bluetooth " + checkConnted.BlueName + " connection before connecting new device." };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }

            if (obj.IsConnect == true)
            {
                ConnectBlue(obj);
            }
            else
            {
                DisconnectBlue(obj);
            }
        }

        public void ConnectBlue(BluetoothModel obj)
        {
            var statusDisconnect = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().DisconnectPrinter();

            if (statusDisconnect == true)
            {
                DataBluetoothConneted.MacID = obj.MacID;
                DataBluetoothConneted.BlueName = obj.BlueName;
                DataBluetoothConneted.IsBluetooth = false;
                DataBluetoothConneted.IsConnect = false;
            }
            else
            {
                DataBluetoothConneted.MacID = obj.MacID;
                DataBluetoothConneted.BlueName = obj.BlueName;
                DataBluetoothConneted.IsBluetooth = true;
                DataBluetoothConneted.IsConnect = true;
            }

            var check = BluetoothList.Where(s => s.MacID == DataBluetoothConneted.MacID).FirstOrDefault();
            if (check != null && !string.IsNullOrEmpty(check.MacID))
            {
                BluetoothList.Remove(check);

                check.IsConnect = DataBluetoothConneted.IsConnect;
                BluetoothList.Insert(0, check);
            }
        }

        public void DisconnectBlue(BluetoothModel obj)
        {
            var status = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().ConnectPrinter(obj.MacID);

            if (status == true)
            {
                DataBluetoothConneted.MacID = obj.MacID;
                DataBluetoothConneted.BlueName = obj.BlueName;
                DataBluetoothConneted.IsBluetooth = true;
                DataBluetoothConneted.IsConnect = true;
            }
            else
            {
                DataBluetoothConneted.MacID = obj.MacID;
                DataBluetoothConneted.BlueName = obj.BlueName;
                DataBluetoothConneted.IsBluetooth = false;
                DataBluetoothConneted.IsConnect = false;
            }

            var check = BluetoothList.Where(s => s.MacID == DataBluetoothConneted.MacID).FirstOrDefault();
            if (check != null && !string.IsNullOrEmpty(check.MacID))
            {
                BluetoothList.Remove(check);

                check.IsConnect = DataBluetoothConneted.IsConnect;
                BluetoothList.Insert(0, check);
            }
        }

        private void LoadData()
        {
            var allListsSource = new ObservableCollection<BluetoothModel>();

            allListsSource = Xamarin.Forms.DependencyService.Get<IBarcodeMarkDown>().GetListBluetooth();

            BluetoothList = new ObservableCollection<BluetoothModel>(allListsSource);
        }

        private async void OnCloseCommandAsync(object obj)
        {
            ClosePopup?.Invoke(this, DataBluetoothConneted);

            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
