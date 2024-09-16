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
using MWBarcodeScanner;
using CoreGraphics;
using AVFoundation;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(CognexCamera), typeof(CognexCameraRenderer))]
namespace AppName.iOS.CustomRenderers
{
    public class CognexCameraRenderer : ViewRenderer<CognexCamera, UIView>
    {
        #region Khai báo

        private List<string> _scannedBarcodes = new List<string>();
        private CognexCamera _scancognexCamera;
        public MWScannerViewController scannerView;
        public Scanner scanner;

        #endregion private variables

        private void DidScan(ScannerResult result)
        {
            InvokeOnMainThread(() =>
            {
                Thread.Sleep(100);

                if (result != null)
                {
                    if (scanner.closeScannerOnDecode)
                    {
                        if (!string.IsNullOrWhiteSpace(result.code))
                        {
                            _scannedBarcodes.Add(result.code);
                        }

                        _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(result.code))
                        {
                            _scannedBarcodes.Add(result.code);
                        }

                        _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);

                        new System.Threading.Timer(obj => scanner.resumeScanning(), null, 3000, System.Threading.Timeout.Infinite);
                    }
                }
            });
        }

        public void StartCheck()
        {
            scannerView = new MWScannerViewController();
            scannerView.OnResult += DidScan;
            scanner.viewController = scannerView;

            SetNativeControl((UIView)scannerView.View);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CognexCamera> e)
        {
            base.OnElementChanged(e);

            _scancognexCamera = e.NewElement;

            if (Control == null)
            {
                try
                {
                    scanner = new Scanner();
                    scanner.closeScannerOnDecode = _scancognexCamera.AllowCoutinueScan;
                    scanner.useZoom = false;
                    scanner.useFlash = false;
                    scanner.useAutoRect = true;
                    scanner.hidePausebutton = true;
                    scanner.enableShowLocation = true;
                    scanner.enableBeepSound = true;

                    customDecoderInit(false);

                    MWScannerViewController.param_OverlayMode = Scanner.OM_NONE;
                    MWScannerViewController.state = CameraState.DECODE_DISPLAY;
                    MWScannerViewController.closeFrame = new CGRect(0, 0, 0, 0);
                    MWScannerViewController.stopped = false;
                    MWScannerViewController.param_PauseScanner = false;
                    MWScannerViewController.videoZoomSupported = true;
                    MWScannerViewController.param_EnableLocation = true;
                    MWScannerViewController.param_EnableBeepSound = true;
                    //MWScannerViewController.param_EnableClose = false;
                    MWScannerViewController.param_Orientation = UIInterfaceOrientationMask.Portrait;

                    if (_scancognexCamera != null)
                    {
                        _scancognexCamera.CreateScanning = CreateScanning;
                        _scancognexCamera.StopScanning = StopScanning;
                        _scancognexCamera.StartScanning = StartScanning;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        public static CGRect RECT_LANDSCAPE_1D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_LANDSCAPE_2D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_PORTRAIT_1D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_PORTRAIT_2D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_FULL_1D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_FULL_2D = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_DOTCODE = new CGRect(0, 0, 100, 100);
        public static CGRect RECT_FULL = new CGRect(0, 0, 100, 100);
        public static void customDecoderInit(bool imageScanning)
        {
            int registerResult = BarcodeConfig.MWB_registerSDK("lOuRzo5EovBP9wVLdP4vdkvL01hseRFgSnHxpjXcewU=");

            switch (registerResult)
            {
                case BarcodeConfig.MWB_RTREG_OK:
                    Console.WriteLine("Registration OK");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_KEY:
                    Console.WriteLine("Registration Invalid Key");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_CHECKSUM:
                    Console.WriteLine("Registration Invalid Checksum");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_APPLICATION:
                    Console.WriteLine("Registration Invalid Application");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_SDK_VERSION:
                    Console.WriteLine("Registration Invalid  SDK Version");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_KEY_VERSION:
                    Console.WriteLine("Registration Invalid Key Version");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_PLATFORM:
                    Console.WriteLine("Registration Invalid Platform");
                    break;
                case BarcodeConfig.MWB_RTREG_KEY_EXPIRED:
                    Console.WriteLine("Registration Key Expired");
                    break;
                default:
                    break;
            }

            BarcodeConfig.MWB_setResultType(BarcodeConfig.MWB_RESULT_TYPE_MW);

            BarcodeConfig.MWB_setActiveCodes(
                BarcodeConfig.MWB_CODE_MASK_25 |
                BarcodeConfig.MWB_CODE_MASK_39 |
                BarcodeConfig.MWB_CODE_MASK_93 |
                BarcodeConfig.MWB_CODE_MASK_128 |
                BarcodeConfig.MWB_CODE_MASK_AZTEC |
                BarcodeConfig.MWB_CODE_MASK_DM |
                BarcodeConfig.MWB_CODE_MASK_EANUPC |
                BarcodeConfig.MWB_CODE_MASK_PDF |
                BarcodeConfig.MWB_CODE_MASK_QR |
                BarcodeConfig.MWB_CODE_MASK_CODABAR |
                BarcodeConfig.MWB_CODE_MASK_11 |
                BarcodeConfig.MWB_CODE_MASK_MSI |
                BarcodeConfig.MWB_CODE_MASK_RSS |
                BarcodeConfig.MWB_CODE_MASK_MAXICODE |
                BarcodeConfig.MWB_CODE_MASK_POSTAL
            );

            //QUét 2 chiều
            BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL | BarcodeConfig.MWB_SCANDIRECTION_VERTICAL);

            if (imageScanning)
            {
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL);
            }
            else
            {
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM, RECT_FULL_2D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR, RECT_FULL_2D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI, RECT_FULL_1D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
                BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);
            }

            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_25, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_MSI, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_39, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_CODABAR, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_11, 5);

            if (imageScanning)
                MWImageScanner.PARSER_MASK = BarcodeConfig.MWP_PARSER_MASK_NONE;
            else
                MWScannerViewController.setActiveParserMask(BarcodeConfig.MWP_PARSER_MASK_NONE);

            if (imageScanning)
                BarcodeConfig.MWB_setLevel(5);
            else
                BarcodeConfig.MWB_setLevel(2);

            int ver = BarcodeConfig.MWB_getLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            String libVersion = v1.ToString() + "." + v2.ToString() + "." + v3.ToString();
            Console.WriteLine("Lib version: " + libVersion);
        }

        #region scanning control methods

        private void StopScanning()
        {
            try
            {
                //scanner.closeScanner();
                MWScannerViewController.param_PauseScanner = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateScanning()
        {
            try
            {
                StartCheck();
            }
            catch (Exception ex)
            {

            }
        }

        private void StartScanning()
        {
            try
            {
                MWScannerViewController.param_PauseScanner = false;
            }
            catch (Exception ex)
            {

            }
        }


        #endregion scanning control methods
    }
}