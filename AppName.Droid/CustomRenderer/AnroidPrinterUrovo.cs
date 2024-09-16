using Android.Bluetooth;
using Android.Device;
using Android.OS;
using Android.Widget;
using AppName.Droid.CustomRenderer;
using AppName.Model;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using tsclib;

[assembly: Xamarin.Forms.Dependency(typeof(AnroidPrinterUrovo))]
namespace AppName.Droid.CustomRenderer
{
    public class AnroidPrinterUrovo : IPrinterUrovo
    {
        private int PRINT_TEXT = 0;   //Printed text
        private int PRINT_BITMAP = 1;   //print pictures
        private int PRINT_BARCOD = 2;   //Print bar code
        private int PRINT_FORWARD = 3;   //Forward (paper feed)

        private String[] mBarTypeTable = {
            "3", "20", "25",
            "29", "34", "55", "58",
            "71", "84", "92",
        };

        private int mBarcodeTypeValue;   //Barcode Type

        ////Printer gray value 0-4
        //private int DEF_PRINTER_HUE_VALUE = 0;
        //private int MIN_PRINTER_HUE_VALUE = 0;
        //private int MAX_PRINTER_HUE_VALUE = 4;

        ////Print speed value 0-9
        //private int DEF_PRINTER_SPEED_VALUE = 9;
        //private int MIN_PRINTER_SPEED_VALUE = 0;
        //private int MAX_PRINTER_SPEED_VALUE = 9;

        // Printer status
        private int PRNSTS_OK = 0;                //OK
        //private int PRNSTS_OUT_OF_PAPER = -1;    //Out of paper
        //private int PRNSTS_OVER_HEAT = -2;        //Over heat
        //private int PRNSTS_UNDER_VOLTAGE = -3;    //under voltage
        //private int PRNSTS_BUSY = -4;            //Device is busy
        //private int PRNSTS_ERR = -256;            //Common error
        //private int PRNSTS_ERR_DRIVER = -257;    //Printer Driver error

        //private PrinterManager mPrinterManager;

        //private PrinterManager getPrinterManager()
        //{
        //    {
        //        if (mPrinterManager == null)
        //        {
        //            mPrinterManager = new PrinterManager();
        //            mPrinterManager.Open();
        //        }
        //        return mPrinterManager;
        //    }
        //}

        //public void DisconnectPrinter()
        //{
        //    if (mPrinterManager != null)
        //    {
        //        mPrinterManager.Close();
        //    }
        //}

        //private  void doPrint(PrinterManager printerManager, int type, Object content)
        public void ExecuteStartPrintingUrovo(int type, Object content)
        {
            try
            {
                PrinterManager printerManager = new PrinterManager();
                //var status = printerManager.Open();

                int ret = printerManager.Prn_getStatus();   //Get printer status
                if (ret == PRNSTS_OK)
                {
                    printerManager.SetupPage(384, -1);   //Set paper size
                    switch (type)
                    {
                        case 0:
                            int fontSize = 24;
                            int fontStyle = 0x0000;
                            String fontName = "simsun";

                            int height = 0;
                            String[] texts = ((String)content).Split("\n");   //Split print content into multiple lines
                            for (int i = 0; i < texts.Length; i++)
                            {
                                height += printerManager.DrawText(texts[i].ToString(), 0, height, fontName, fontSize, false, false, 0);   //Printed text
                            }

                            for (int i = 0; i < texts.Length; i++)
                            {
                                height += printerManager.DrawTextEx(texts[i].ToString(), 5, height, 384, -1, fontName, fontSize, 0, fontStyle, 0);   ////Printed text
                            }

                            //for (String text : texts)
                            //{
                            //    height += printerManager.DrawText(text, 0, height, fontName, fontSize, false, false, 0);   //Printed text
                            //}
                            //for (String text : texts)
                            //{
                            //    height += printerManager.DrawTextEx(text, 5, height, 384, -1, fontName, fontSize, 0, fontStyle, 0);   ////Printed text
                            //}

                            height = 0;
                            break;
                        case 1:
                            String text = (String)content;
                            //According to the printed content and barcode type
                            switch (mBarcodeTypeValue)
                            {
                                case 20:  // CODE128, alphabet + no.
                                case 25:  // CODE93, alphabet + no.
                                    if (Regex.IsMatch(text, @"^[A-Za-z0-9]+$"))
                                    {
                                        printerManager.DrawBarcode(text, 196, 300, mBarcodeTypeValue, 2, 70, 0);   //Print bar code
                                    }
                                    else
                                    {
                                        //Toast.makeText(
                                        //        this.getApplicationContext(),
                                        //        "Not support for Chinese code!!!",
                                        //        Toast.LENGTH_SHORT).show();
                                        //printInfo.requestFocus();
                                        //updatePrintStatus(ret);
                                        return;
                                    }
                                    break;
                                case 34:  // UPCA, no., UPCA needs short length of No.
                                          //case 2:// Chinese25MATRIX, no.
                                    if (isNumeric(text))
                                    {
                                        printerManager.DrawBarcode(text, 196, 300, mBarcodeTypeValue, 2, 70, 0);   //Print bar code
                                    }
                                    else
                                    {
                                        var a = "Not support for non-numeric!!!";

                                        return;
                                    }
                                    break;

                                case 3:  // Chinese25INTER, no.
                                case 55:  // PDF417, setHue: 3
                                    printerManager.DrawBarcode(text, 25, 5, mBarcodeTypeValue, 3, 60, 0);   //Print bar code
                                    break;
                                case 58:  // QRCODE
                                case 71:  // DATAMATRIX
                                    printerManager.DrawBarcode(text, 50, 10, mBarcodeTypeValue, 8, 120, 0);   //Print bar code
                                    break;
                                case 84:  // uPDF417
                                    printerManager.DrawBarcode(text, 25, 5, mBarcodeTypeValue, 4, 60, 0);   //Print bar code
                                    break;
                                case 92:  // AZTEC
                                    printerManager.DrawBarcode(text, 50, 10, mBarcodeTypeValue, 8, 120, 0);   //Print bar code
                                    break;
                            }
                            break;

                            //case PRINT_BITMAP:
                            //    Bitmap bitmap = (Bitmap)content;
                            //    if (bitmap != null)
                            //    {
                            //        printerManager.drawBitmap(bitmap, 30, 0);  //print pictures
                            //    }
                            //    else
                            //    {
                            //        Toast.makeText(this, "Picture is null", Toast.LENGTH_SHORT).show();
                            //    }
                            //    break;
                    }

                    ret = printerManager.PrintPage(0);  //Execution printing
                    printerManager.PaperFeed(16);  //paper feed1

                    printerManager.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool isNumeric(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");

        }
    }
}