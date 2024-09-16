using Android.Content;
using Android.Graphics;
using ScanditBarcodePicker.Android;
using AppName.CustomRenderer;
using ScanditBarcodePicker.Android.Recognition;
using AppName.Droid.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using tsclib;
using System.Threading;
using Android.Bluetooth;
using AppName.Model;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(BarcodeMarkdown))]
namespace AppName.Droid.CustomRenderer
{
    public class BarcodeMarkdown : Java.Lang.Object, IBarcodeMarkDown
    {
        bluetooth bt = new bluetooth();

        public bool ConnectPrinter(string macID)
        {
            try
            {

                if (bt.IsConnected == false)
                {
                    bt.openport(macID);

                    return true;
                }
                else
                {
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                bt.IsConnected = false;


                return false;
            }
        }

        public bool DisconnectPrinter()
        {
            try
            {
                if (bt.IsConnected == true)
                {
                    bt.closeport();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public ObservableCollection<BluetoothModel> GetListBluetooth()
        {
            var listModel = new ObservableCollection<BluetoothModel>();

            try
            {
                var listBlu1e = BluetoothAdapter.DefaultAdapter;

                var BluetoothDeviceList = new ObservableCollection<object>();

                var listBlue = BluetoothAdapter.DefaultAdapter.BondedDevices;

                foreach (var item in listBlue)
                {
                    var model = new BluetoothModel();

                    model.BlueName = item.Name;
                    model.MacID = item.Address;
                    model.Status = item.BondState.ToString();

                    listModel.Add(model);
                }

                return listModel;

            }
            catch (Exception ex)
            {
                return listModel;
            }
        }

        //Test tự động xuống dòng content
        //public void ExecuteStartPrinting(ItemInfoUpdateGiaModel model)
        //{
        //    var disc = String.Format("{0:0,0}", model.Discout);
        //    var gia = String.Format("{0:0,0}", model.Unit_Price);

        //    Thread.Sleep(100);
        //    bt.sendcommand("SIZE 50 mm,30 mm\r\n");
        //    bt.sendcommand("GAP 5 mm,0mm \n");
        //    bt.sendcommand("DIRECTION 0\r\n");
        //    bt.sendcommand("CLS \r\n");
        //    bt.clearbuffer();


        //    var hepBarcode = 2;
        //    var rongBarcode = 1;
        //    bt.sendcommand("BARCODE 270,8,\"128\",75,2,0, " + hepBarcode + " ," + rongBarcode + ",2,\"" + model.Barcode + "\"\n");

        //    var hepTexiPrice = 15;
        //    var rongTexiPrice = 15;

        //    if (model.Discout != model.Unit_Price)
        //    {
        //        bt.sendcommand("TEXT 10, 15,\"0\",0," + hepTexiPrice + "," + rongTexiPrice + ",1,\"" + disc + "\"\n");
        //        bt.sendcommand("TEXT 10, 65,\"0\",0," + hepTexiPrice + "," + rongTexiPrice + ",1,\"" + gia + "\"\n");
        //    }
        //    else
        //    {
        //        bt.sendcommand("TEXT 10, 45,\"0\",0," + hepTexiPrice + "," + rongTexiPrice + ",1,\"" + gia + "\"\n");
        //    }

        //    bt.sendcommand("BAR 8," + 80 + ",135,4 \r\n");

        //    var content = "";
        //    content = content + model.ItemName + " | POG:" + model.POG + " \n";
        //    content = content + model.SchemeDescriptionMixMatch + " \n";
        //    content = content + "(" + model.DatetimeMixMatch + ")" + " \n";
        //    content = content + model.SchemeDescriptionMultiBuy + " \n";
        //    content = content + "(" + model.DatetimeMultil + ")" + " \n";

        //    var a = "lawmakers in West Virginia's House ofDelegates have approved a bill that wouldallow gun owners to carry concealedhandguns ed a bill that wouldallow gun owners to ed a bill that wouldallow gun owners to  without a permit.";
        //    bt.sendcommand("BOX 10,120,270,120,2 \r\n");
        //    bt.sendcommand("BLOCK 15,15,270,120,\"0\", 0,8,8,20,2,\"We standbehind our products with one of the mostcomprehensive support programs in theAuto - ID industry.\"");
        //    //bt.sendcommand("BLOCK 20,120,270,120,\"3\",0,10,10,0,0,0,\"" + a + "\" \r\n");

        //    //var hepInfo = 7;
        //    //var rongInfo = 7;
        //    //var startContentText = 120;
        //    //bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + model.ItemName + " | POG:" + model.POG + "\"\n");

        //    //if (!string.IsNullOrEmpty(model.SchemeDescriptionMixMatch))
        //    //{
        //    //    startContentText = startContentText + 23;
        //    //    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + model.SchemeDescriptionMixMatch + "\"\n");

        //    //    startContentText = startContentText + 23;
        //    //    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + "(" + model.DatetimeMixMatch + ")" + "\"\n");
        //    //}

        //    //if (!string.IsNullOrEmpty(model.SchemeDescriptionMultiBuy))
        //    //{
        //    //    startContentText = startContentText + 23;
        //    //    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + model.SchemeDescriptionMultiBuy + "\"\n");

        //    //    startContentText = startContentText + 23;
        //    //    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + "(" + model.DatetimeMultil + ")" + "\"\n");
        //    //}

        //    bt.sendcommand("TEXT 288," + 195 + ",\"3\",0,1,1,\"SALE\" \r\n");
        //    bt.sendcommand("REVERSE 260," + 185 + ",120,40 \r\n");

        //    bt.printlabel(1, 1);
        //}

        public bool ExecuteStartPrintingIStagtamplate(ItemInfoUpdateGiaModel model)
        {
            try
            {
                var disc = String.Format("{0:0,0}", model.Discout);
                var gia = String.Format("{0:0,0}", model.Unit_Price);

                Thread.Sleep(200);
                bt.sendcommand("SIZE 50 mm,30 mm\r\n");
                bt.sendcommand("GAP 5 mm,0mm \n");
                bt.sendcommand("DIRECTION 0\r\n");
                bt.sendcommand("CLS \r\n");
                bt.clearbuffer();


                var hepBarcode = 2;
                var rongBarcode = 1;
                bt.sendcommand("BARCODE 270,8,\"128\",62,2,0, " + hepBarcode + " ," + rongBarcode + ",2,\"" + model.Barcode + "\"\n");

                var hepTexiPrice = 15;
                var rongTexiPrice = 15;

                if (model.Discout != 0)
                {
                    bt.sendcommand("TEXT 10, 15,\"0\",0," + 10 + "," + 10 + ",1,\"" + gia + "\"\n");
                    bt.sendcommand("TEXT 10, 55,\"0\",0," + hepTexiPrice + "," + rongTexiPrice + ",1,\"" + disc + "\"\n");
                    bt.sendcommand("BAR 8," + 26 + ",100,3 \r\n");
                }
                else
                {
                    bt.sendcommand("TEXT 10, 45,\"0\",0," + hepTexiPrice + "," + rongTexiPrice + ",1,\"" + gia + "\"\n");
                }


                var hepInfo = 7;
                var rongInfo = 7;
                var startContentText = 97;

                bt.sendcommand("TEXT 15, " + startContentText + ", \"ROMAN.TTF\" ,0," + hepInfo + "," + rongInfo + ",1,\""
                    + (!string.IsNullOrEmpty(model.ItemNo) ? (model.ItemNo + "|") : "")
                    + (!string.IsNullOrEmpty(model.ReturnType) ? (model.ReturnType + "|") : "")
                    + (!string.IsNullOrEmpty(model.POG) ? (model.POG + "|") : "")
                    + (model.ItemName.Length >= 30 ? RemoveUnicode(model.ItemName.Substring(0, 30)) + "..." : RemoveUnicode(model.ItemName)) + "\"\n");

                if (model.Discout != 0)
                {
                    startContentText = startContentText + 23;

                    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + model.DatetimeDiscount + "\"\n");
                }

                if (!string.IsNullOrEmpty(model.SchemeDescriptionMixMatch))
                {
                    startContentText = startContentText + 23;
                    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + RemoveUnicode(model.SchemeDescriptionMixMatch) + "\"\n");

                    startContentText = startContentText + 23;
                    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + "(" + model.DatetimeMixMatch + ")" + "\"\n");
                }

                if (!string.IsNullOrEmpty(model.SchemeDescriptionMultiBuy))
                {
                    startContentText = startContentText + 23;
                    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + RemoveUnicode(model.SchemeDescriptionMultiBuy) + "\"\n");

                    startContentText = startContentText + 23;
                    bt.sendcommand("TEXT 15, " + startContentText + ",\"0\",0," + hepInfo + "," + rongInfo + ",1,\"" + "(" + model.DatetimeMultil + ")" + "\"\n");
                }

                bt.sendcommand("TEXT 298," + 195 + ",\"3\",0,1,1,\"SALE\" \r\n");
                bt.sendcommand("REVERSE 270," + 185 + ",120,40 \r\n");

                bt.printlabel(1, 1);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public bool ExecuteStartPrintingISshelfTemplate(ItemInfoUpdateGiaModel model)
        {
            try
            {
                var disc = String.Format("{0:0,0}", model.Discout);
                var gia = String.Format("{0:0,0}", model.Unit_Price);

                Thread.Sleep(200);
                bt.sendcommand("SIZE 50 mm,30 mm\r\n");
                bt.sendcommand("GAP 5 mm,0mm \n");
                bt.sendcommand("DIRECTION 0\r\n");
                bt.sendcommand("CLS \r\n");
                bt.clearbuffer();

                //var hepBarcode = 2;
                //var rongBarcode = 1;
                //bt.sendcommand("BARCODE 270,8,\"128\",75,2,0, " + hepBarcode + " ," + rongBarcode + ",2,\"" + model.Barcode + "\"\n");

                var hepTexiPrice = 15;
                var rongTexiPrice = 15;

                if (model.Discout != model.Unit_Price)
                {
                    bt.sendcommand("TEXT 290, 10,\"0\",0,12,12,1,\"" + gia + "\"\n");
                    bt.sendcommand("BAR 270,25,120,2 \r\n");

                    bt.sendcommand("TEXT 200, 35,\"0\",0,23,23,2,\"" + disc + "\"\n");
                }
                else
                {
                    bt.sendcommand("TEXT 200, 35,\"0\",0,23,23,2,\"" + gia + "\"\n");
                }

                bt.sendcommand("TEXT 5, 95,\"0\",0,7,7,1,\"" + model.ItemName + " | POG:" + "\"\n");

                bt.sendcommand("TEXT 190," + 150 + ",\"ROMAN.TTF\",0,27,27,2,\"BIG SAVES\" \r\n");
                bt.sendcommand("REVERSE 0," + 120 + ",480,120 \r\n");

                bt.printlabel(1, 1);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}