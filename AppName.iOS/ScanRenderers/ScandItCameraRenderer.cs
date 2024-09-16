using Foundation;
using Scandit;
using ScanditBarcodeScanner.iOS;
using AppName.CustomRenderer;
using AppName.iOS.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Threading;

[assembly: ExportRenderer(typeof(ScandItCamera), typeof(ScandItCameraRenderer))]
namespace AppName.iOS.CustomRenderers
{
    public class ScandItCameraRenderer : ViewRenderer<ScandItCamera, UIView>
    {
        #region private variables

        private static string ScanditAppKey = "Ade/nh4MPpEdBX2sLCkXcmAvgC3RENFITFqDgYtWmeazMuPbcVM+8RRaTApyXn5LqVY8SRAzS0flfqtztzzCu3hLk6nVIPIx/ADZYwR8Au6jb57oIkKAKL42d9LYFAhkzjlOrXkb2UcuadWd8YdG9Bq3TInZpHfBdOX3NMTXs025SPzwGRw9ntat3NJQdOD+5mqOORp8fM9/CKpIQwG+KxvGEc8Wlg2ZwaepCej+dQOUvpMvfMquqdk6SOMJdrQOhSdLxOgBwo8GDQV5pp6mYNiM5KBmyvzu6sTqhy8pvh5aC8TIZnk2uc5LNkGjUG/ZUlEY0K9FiIYjdtR1cPIOojd/kNnuZ50x+1tgHkdKnVwsWq5ONDUO20ZK04on5dTAnzNK04AHTfbO0WkNwv7V0hieiG7zJIqJ8oG+zgANkAlrjAk3W0e57gKsWLbfXMrhhukgEefAHiSWZ+MYBrkUX6ZJrCOk86v1Q7vRHCyFF9/OdLtX2ubg4kO4KDV6SjFbmij9TR0b6vIg5grOfiQYcUR+2Yh85/3oR/6SJwJYLjKh1cKAtgQ/hLm4HhiD+rwoNDXovwX9dfOVh9gdPU5q2KiNIRJJvd7QHwV8B4dj99kOi3SN26pq/vJvd4ifg2ER7JlMvYfjlxSLCWGQgYbTGLQQRdZJr9JOEr6/4SWfcnOs6aI0uDjf2Z8X1xi6lqJQP6pa9cVvzXmX95ZNcAoeSmVeTqWYQR71/YSsg2KLkO67ne6wTNGjf8IR5PXx4OoeeL6rNwVpH9LpPGZryzTvL72TXUTxJPohSoDzsDTmXVz6w8l4bgREkQ==";
        private List<string> _scannedBarcodes = new List<string>();
        private ScandItCamera _scanedItCamera;
        private BarcodePicker _picker;

        #endregion private variables

        /// <summary>
        /// On element changed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<ScandItCamera> e)
        {
            base.OnElementChanged(e);
            //assign new elemnt value to scanned it camera
            _scanedItCamera = e.NewElement;

            if (Control == null)
            {
                try
                {
                    var scanLicense = new ScanditLicense();
                    scanLicense.AppKey = ScanditAppKey;
                    ScanSettings scanSettings = ScanSettings.DefaultSettings();
                    //set code duplication filter
                    //if (_scanedItCamera.AllowDuplicate)
                    //    scanSettings.CodeDuplicateFilter = 1500; //1.5 sec delay for duplication scanning
                    //else
                    //    scanSettings.CodeDuplicateFilter = -1;

                    //Bar code symbologies
                    //scanSettings.SetSymbologyEnabled(Symbology.EAN13, true);
                    //scanSettings.SetSymbologyEnabled(Symbology.QR, true);
                    scanSettings.SetSymbologyEnabled(ScanditBarcodeScanner.iOS.Symbology.UPC12, true);
                    scanSettings.SetSymbologyEnabled(ScanditBarcodeScanner.iOS.Symbology.Code128, true);
                    scanSettings.SetSymbologyEnabled(ScanditBarcodeScanner.iOS.Symbology.Code39, true);
                    //scanSettings.SetSymbologyEnabled(Symbology.EAN8, true);
                    //update code 128 settings
                    //var code128Settings = scanSettings.SettingsForSymbology(Symbology.Code128);
                    //var countArray = NSArray.FromObjects(4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,
                    //28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40);
                    //code128Settings.ActiveSymbolCounts = new NSSet(countArray);
                    //apply scan settings
                    _picker = new BarcodePicker(scanSettings);
                    _picker.OverlayView.SetBeepEnabled(true);
                    _picker.OverlayView.SetVibrateEnabled(true);
                    _picker.OverlayView.SetCameraSwitchVisibility(CameraSwitchVisibility.Never);
                    _picker.OverlayView.GuiStyle = GuiStyle.None;
                    //to set picker gui style
                    if (_scanedItCamera.ShowFocus)
                        _picker.OverlayView.GuiStyle = GuiStyle.Default;
                    else
                        _picker.OverlayView.GuiStyle = GuiStyle.LocationsOnly;

                    _picker.DidScan += DidScan;
                    //set native control as picker view
                    SetNativeControl(_picker.View);

                    //enable action for start and stop scanning
                    if (_scanedItCamera != null)
                    {
                        _scanedItCamera.StartScanning = StartScanning;
                        _scanedItCamera.StopScanning = StopScanning;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
        
        /// <summary>
        /// Searchs the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs</param>
        private void SearchClicked(object sender, EventArgs e)
        {
            _picker.StopScanning();
            _scanedItCamera.EditClicked?.Invoke();
        }

        /// <summary>
        /// Did scan delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DidScan(object sender, BarcodePickerDidScanEventArgs e)
        {
            //Adding bar code to barcode list
            var barcodeList = e.Session.AllRecognizedCodes;
            var barcode = barcodeList.LastOrDefault()?.Data;
            //barcode = FormatBarcode(barcodeList?.LastOrDefault());
            if (!string.IsNullOrWhiteSpace(barcode))
                _scannedBarcodes.Add(barcode);
            //invoke command
            _scanedItCamera?.DidScannedCommand?.Execute(_scannedBarcodes);
        }

        /// <summary>
        /// Formats UPC-A barcodes with 12 digits to append 0 to the begninning
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private string FormatBarcode(Barcode barcode)
        {
            var formattedBarcode = barcode?.Data;
            if (barcode?.Symbology == ScanditBarcodeScanner.iOS.Symbology.UPC12 && barcode?.Data?.Length == 12)
            {
                formattedBarcode = barcode?.Data?.Insert(0, "0");
            }
            return formattedBarcode;
        }

        #region scanning control methods

        private void StopScanning()
        {
            _picker.StopScanning();
        }
        
        
        private void PrinerClick()
        {

        }

        private void StartScanning()
        {
            _picker.StartScanning();
        }

        #endregion scanning control methods
    }
}