using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AppName.CustomRenderer
{
    public interface IDevice
    {
        string GetIdentifier();
        void ShowAlert(string content, int time = 5000);
    }
}
