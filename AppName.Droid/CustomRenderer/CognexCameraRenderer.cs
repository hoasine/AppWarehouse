using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MWBarcodeScanner;
using System.Drawing;
using Android.Content.PM;
using AppName.CustomRenderer;
using AppName.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using ScanditBarcodePicker.Android;
using System.Threading;
using Android.Util;
using System.IO;
using Android.Graphics;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CognexCamera), typeof(CognexCameraRenderer))]
namespace AppName.Droid.CustomRenderer
{
    public class CognexCameraRenderer : ViewRenderer<CognexCamera, RelativeLayout>, ISurfaceHolderCallback, IScanSuccessCallback
    {
        #region private variables

        private List<string> _scannedBarcodes = new List<string>();
        private CognexCamera _scancognexCamera;
        private ScannerActivity scannerView;
        private Scanner scanner;
        private Context _context;
        static SurfaceView surfaceView;
        static RelativeLayout rlSurfaceContainer;
        public bool useAutoRect = true;

        #endregion private variables

        public CognexCameraRenderer(Context context) : base(context)
        {
            _context = context;

            surfaceView = new SurfaceView(_context);
            AddView(surfaceView);
        }

        public CognexCameraRenderer(System.IntPtr i, Android.Runtime.JniHandleOwnership j) : base(Xamarin.Forms.Forms.Context)
        {

        }

        //private void DidScan(ScannerResult result)
        //{
        //    if (result != null)
        //    {
        //        if (scanner.closeScannerOnDecode)
        //        {
        //            if (!string.IsNullOrWhiteSpace(result.code))
        //            {
        //                _scannedBarcodes.Add(result.code);
        //            }

        //            _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(result.code))
        //            {
        //                _scannedBarcodes.Add(result.code);
        //            }

        //            _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);

        //            new System.Threading.Timer(obj => scanner.resumeScanning(), null, 3000, System.Threading.Timeout.Infinite);
        //        }
        //    }
        //}

        void IScanSuccessCallback.barcodeDetected(MWBarcodeScanner.MWResult result)
        {
            if (result != null)
            {
                if (scanner.closeScannerOnDecode)
                {
                    if (!string.IsNullOrWhiteSpace(result.text.ToString()))
                    {
                        _scannedBarcodes.Add(result.text.ToString());
                    }

                    _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(result.text.ToString()))
                    {
                        _scannedBarcodes.Add(result.text.ToString());
                    }

                    _scancognexCamera?.DidScannedCommand?.Execute(_scannedBarcodes);

                    new System.Threading.Timer(obj => scanner.resumeScanning(), null, 2000, System.Threading.Timeout.Infinite);
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CognexCamera> e)
        {
            base.OnElementChanged(e);

            _scancognexCamera = e.NewElement;

            if (Control == null)
            {
                scanner = new Scanner(_context);
                scannerView = new ScannerActivity();
                scanner.closeScannerOnDecode = _scancognexCamera.AllowCoutinueScan;
                scanner.useZoom = false;
                scanner.useFlash = false;
                scanner.useAutoRect = true;
                scanner.hidePausebutton = true;
                scanner.enableShowLocation = true;
                scanner.enableBeepSound = true;

                customDecoderInit(false);

                ScannerActivity.param_OverlayMode = ScannerActivity.OverlayMode.OM_NONE;
                ScannerActivity.state = ScannerActivity.State.PREVIEW;
                ScannerActivity.param_PauseScanner = false;
                ScannerActivity.param_EnableLocation = true;
                ScannerActivity.param_EnableBeepSound = true;

                if (_scancognexCamera != null)
                {
                    _scancognexCamera.CreateScanning = CreateScanning;
                    _scancognexCamera.StopScanning = StopScanning;
                    _scancognexCamera.StartScanning = StartScanning;
                }

                //scanner.ScanInView(this, new RectangleF(0, 0, 100, 100));
            }
        }

        public void StartCheck()
        {
            CameraManager.init(_context);

            ScannerActivity.successCallback = this;

            surfaceView = new SurfaceView(_context);
            surfaceView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            rlSurfaceContainer = new RelativeLayout(_context);

            ISurfaceHolder holder = surfaceView.Holder;
            holder.AddCallback(this);
            holder.SetType(SurfaceType.PushBuffers);

            rlSurfaceContainer.AddView(surfaceView);

            SetNativeControl(rlSurfaceContainer);
        }

        private void initCamera(ISurfaceHolder surfaceHolder)
        {
            try
            {
                CameraManager.setDesiredPreviewSize(1280, 720);

                CameraManager.get().openDriver(surfaceHolder, (_context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait));
            }
            catch (IOException ioe)
            {
                return;
            }
            catch (System.Exception e)
            {
                return;
            }

            var cameraInfo = new Android.Hardware.Camera.CameraInfo();
            Android.Hardware.Camera.GetCameraInfo(CameraManager.USE_FRONT_CAMERA ? 1 : 0, cameraInfo);
            if (cameraInfo.Orientation == 270)
            {
                BarcodeConfig.MWB_setFlags(0, BarcodeConfig.MWB_CFG_GLOBAL_ROTATE180);
            }

            CameraManager.get().startPreview();
            ScannerActivity.state = ScannerActivity.State.PREVIEW;

            BarcodeConfig.MWB_setResultType(BarcodeConfig.MWB_RESULT_TYPE_MW);
            CameraManager.get().requestPreviewFrame(scanner);
        }

        public static RectangleF RECT_LANDSCAPE_1D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_LANDSCAPE_2D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_PORTRAIT_1D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_PORTRAIT_2D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_FULL_1D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_FULL_2D = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_DOTCODE = new RectangleF(0, 0, 100, 100);
        public static RectangleF RECT_FULL = new RectangleF(0, 0, 100, 100);

        public static void customDecoderInit(bool imageScanning)
        {
            Console.WriteLine("Decoder initialization");

            int registerResult = BarcodeConfig.MWB_registerSDK("XAaPA3Q/9zyI5MiaL3l2x6sS766Mju9QsGAuzD5fK/A=", Xamarin.Forms.Forms.Context);

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
                    Console.WriteLine("Registration Invalid SDK Version");
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

            // choose code type or types you want to search for

            // Our sample app is configured by default to search all supported barcodes...
            BarcodeConfig.MWB_setActiveCodes(BarcodeConfig.MWB_CODE_MASK_25 |
                BarcodeConfig.MWB_CODE_MASK_39 |
                BarcodeConfig.MWB_CODE_MASK_93 |
                BarcodeConfig.MWB_CODE_MASK_128 |
                BarcodeConfig.MWB_CODE_MASK_AZTEC |
                BarcodeConfig.MWB_CODE_MASK_DM |
                BarcodeConfig.MWB_CODE_MASK_EANUPC |
                BarcodeConfig.MWB_CODE_MASK_PDF |
                BarcodeConfig.MWB_CODE_MASK_QR |
                BarcodeConfig.MWB_CODE_MASK_CODABAR |
                BarcodeConfig.MWB_CODE_MASK_RSS |
                BarcodeConfig.MWB_CODE_MASK_MAXICODE |
                 BarcodeConfig.MWB_CODE_MASK_POSTAL);

            // Our sample app is configured by default to search both directions...
            BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL | BarcodeConfig.MWB_SCANDIRECTION_VERTICAL);

            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height).
            //If you scan from image set RECT_FULL fro all types
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
                ScannerActivity.setParserMask(BarcodeConfig.MWP_PARSER_MASK_NONE);

            if (imageScanning)
                BarcodeConfig.MWB_setLevel(5);
            else
                BarcodeConfig.MWB_setLevel(2);


            //get and print Library version
            int ver = BarcodeConfig.MWB_getLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            String libVersion = v1.ToString() + "." + v2.ToString() + "." + v3.ToString();
            Console.WriteLine("Lib version: " + libVersion);
        }

        private void StopScanning()
        {
            try
            {
                scanner.closeScanner();
                //scanner.closeScanner();
                ScannerActivity.param_PauseScanner = true;
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
                ScannerActivity.param_PauseScanner = false;
            }
            catch (Exception ex)
            {

            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height)
        {
            if (!ScannerActivity.hasSurface)
            {
                ScannerActivity.hasSurface = true;
                initCamera(holder);
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            CameraManager.get().stopPreview();
            CameraManager.get().closeDriver();

            ScannerActivity.hasSurface = false;
        }
    }
}