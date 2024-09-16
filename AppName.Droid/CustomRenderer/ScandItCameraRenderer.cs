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

[assembly: ExportRenderer(typeof(ScandItCamera), typeof(ScandItCameraRenderer))]
namespace AppName.Droid.CustomRenderer
{
    public class ScandItCameraRenderer : ViewRenderer<ScandItCamera, BarcodePicker>, IOnScanListener
    {
        #region private variables

        private static string ScanditAppKey = "Ade/nh4MPpEdBX2sLCkXcmAvgC3RENFITFqDgYtWmeazMuPbcVM+8RRaTApyXn5LqVY8SRAzS0flfqtztzzCu3hLk6nVIPIx/ADZYwR8Au6jb57oIkKAKL42d9LYFAhkzjlOrXkb2UcuadWd8YdG9Bq3TInZpHfBdOX3NMTXs025SPzwGRw9ntat3NJQdOD+5mqOORp8fM9/CKpIQwG+KxvGEc8Wlg2ZwaepCej+dQOUvpMvfMquqdk6SOMJdrQOhSdLxOgBwo8GDQV5pp6mYNiM5KBmyvzu6sTqhy8pvh5aC8TIZnk2uc5LNkGjUG/ZUlEY0K9FiIYjdtR1cPIOojd/kNnuZ50x+1tgHkdKnVwsWq5ONDUO20ZK04on5dTAnzNK04AHTfbO0WkNwv7V0hieiG7zJIqJ8oG+zgANkAlrjAk3W0e57gKsWLbfXMrhhukgEefAHiSWZ+MYBrkUX6ZJrCOk86v1Q7vRHCyFF9/OdLtX2ubg4kO4KDV6SjFbmij9TR0b6vIg5grOfiQYcUR+2Yh85/3oR/6SJwJYLjKh1cKAtgQ/hLm4HhiD+rwoNDXovwX9dfOVh9gdPU5q2KiNIRJJvd7QHwV8B4dj99kOi3SN26pq/vJvd4ifg2ER7JlMvYfjlxSLCWGQgYbTGLQQRdZJr9JOEr6/4SWfcnOs6aI0uDjf2Z8X1xi6lqJQP6pa9cVvzXmX95ZNcAoeSmVeTqWYQR71/YSsg2KLkO67ne6wTNGjf8IR5PXx4OoeeL6rNwVpH9LpPGZryzTvL72TXUTxJPohSoDzsDTmXVz6w8l4bgREkQ==";
        private List<string> _scannedBarcodes = new List<string>();
        private ScandItCamera _scanedItCamera;
        private BarcodePicker _picker;
        private Context _context;

        #endregion private variables

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ScandItCameraRenderer(Context context) : base(context)
        {
            _context = context;
        }

        //Android 4.3 - to avoid this crash : Unable to activate instance of type Xamarin.Forms.Platform.Android
        public ScandItCameraRenderer(System.IntPtr i, Android.Runtime.JniHandleOwnership j) : base(Forms.Context)
        {
        }

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
                ScanditLicense.AppKey = ScanditAppKey;
                var scanSettings = ScanSettings.Create();
                //set code duplication filter
                if (_scanedItCamera.AllowDuplicate)
                    scanSettings.CodeDuplicateFilter = 1500; //1.5 sec delay for duplication scanning
                else
                    scanSettings.CodeDuplicateFilter = -1;

                //Bar code symbologies
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyEan13, true);
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyEan8, true);
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyQr, true);
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyUpca, true);
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyCode128, true);
                scanSettings.SetSymbologyEnabled(Barcode.SymbologyCode39, true);
                //set code length for code 128
                //var code128Settings = scanSettings.GetSymbologySettings(Barcode.SymbologyCode128);
                //code128Settings.SetActiveSymbolCounts(
                //    new short[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,22,23,24,25,26,27,
                //    28,29,30,31,32,33,34,35,36,37,38,39,40}
                //);
                scanSettings.SetActiveScanningArea(ScanSettings.OrientationPortrait, new RectF(0.0f, 0.45f, 1.0f, 0.55f));
                _picker = new BarcodePicker(_context, scanSettings);
                //set picker properties
                _picker.OverlayView.SetBeepEnabled(true);
                _picker.OverlayView.SetVibrateEnabled(true);
                _picker.OverlayView.SetCameraSwitchVisibility(0);
                //to set picker gui style
                if (_scanedItCamera.ShowFocus)
                    _picker.OverlayView.SetGuiStyle(0);
                else
                    _picker.OverlayView.SetGuiStyle(3);
                //_picker.StartScanning();
                _picker.SetOnScanListener(this);
                //apply picker scan sesstings
                _picker.ApplyScanSettings(scanSettings);
                SetNativeControl(_picker);

                //enable action for start and stop scanning
                if (_scanedItCamera != null)
                {
                    _scanedItCamera.StartScanning = StartScanning;
                    _scanedItCamera.StopScanning = StopScanning;
                }
            }
        }

        /// <summary>
        /// Picker did scan event
        /// </summary>
        /// <param name="scannedSession"></param>
        public void DidScan(IScanSession scannedSession)
        {
            //Adding bar code to barcode list
            var barcodeList = scannedSession?.AllRecognizedCodes;
            var barcode = barcodeList?.LastOrDefault()?.Data;
            //barcode = FormatBarcode(barcodeList?.LastOrDefault());
            if (!string.IsNullOrWhiteSpace(barcode))
                _scannedBarcodes.Add(barcode);
            //invoke command
            _scanedItCamera?.DidScannedCommand?.Execute(_scannedBarcodes);
        }

        /// <summary>
        /// Formats UPC-A barcodes with 12 digits to append 0 to the beginning
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private string FormatBarcode(Barcode barcode)
        {
            var formattedBarcode = barcode?.Data;
            if (barcode?.Symbology == Barcode.SymbologyUpca && barcode?.Data?.Length == 12)
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

        private void StartScanning()
        {
            _picker.StartScanning();
        }

        #endregion scanning control methods

        #region button onclick listener

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            _picker.StopScanning();
            _scanedItCamera.EditClicked?.Invoke();
        }


        #endregion button onclick listener
    }
}