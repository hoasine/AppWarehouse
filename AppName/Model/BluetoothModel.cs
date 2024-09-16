using System;
using System.Collections.Generic;
using System.Text;

namespace AppName.Model
{
    public class BluetoothModel : ObservableObject
    {
        public string BlueName { get; set; }
        public string MacID { get; set; }
        public string Status { get; set; }
        public bool IsConnect { get; set; }
        //private bool _IsConnect;
        //public bool IsConnect
        //{
        //    get => _IsConnect;

        //    set => SetProperty(ref _IsConnect, value);
        //}
    }


    public class BluetoothConnetedModel : ObservableObject
    {
        public bool IsBluetooth { get; set; }
        public string Template { get; set; }
        private string _BlueName;
        public string BlueName
        {
            get => _BlueName;

            set => SetProperty(ref _BlueName, value);
        }

        private string _MacID;
        public string MacID
        {
            get => _MacID;

            set => SetProperty(ref _MacID, value);
        }

        public string Status { get; set; }
        private bool _IsConnect;
        public bool IsConnect
        {
            get => _IsConnect;

            set => SetProperty(ref _IsConnect, value);
        }
    }
}
