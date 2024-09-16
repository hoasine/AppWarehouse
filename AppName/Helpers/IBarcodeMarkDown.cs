
using AppName.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface IBarcodeMarkDown
{
    bool ExecuteStartPrintingIStagtamplate(ItemInfoUpdateGiaModel model);
    bool ExecuteStartPrintingISshelfTemplate(ItemInfoUpdateGiaModel model);
    bool ConnectPrinter(string macID);
    bool DisconnectPrinter();
    ObservableCollection<BluetoothModel> GetListBluetooth();
}
