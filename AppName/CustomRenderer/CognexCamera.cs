using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppName.CustomRenderer
{
    public class CognexCamera : View
    {
        public Action StartScanning;
        public Action StopScanning;
        public Action CreateScanning;

        public bool AllowCoutinueScan { get; set; }
        public bool ShowFocus { get; set; }

        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
          typeof(Command<List<string>>),
          typeof(CognexCamera));

        public Command<List<string>> DidScannedCommand
        {
            get { return (Command<List<string>>)GetValue(DidScannedCommandProperty); }
            set { base.SetValue(DidScannedCommandProperty, value); }
        }
    }
}
