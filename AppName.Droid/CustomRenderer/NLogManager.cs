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
using NLog.Config;
using NLog.Targets;
using System.Runtime.CompilerServices;
using NLog;

[assembly: Xamarin.Forms.Dependency(typeof(NLogManager))]
namespace AppName.Droid.CustomRenderer
{
    public class NLogManager : ILogManager
    {
        public NLogManager()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var consoleRule = new LoggingRule("*", LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            string rootPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments);
            var filePathDir = System.IO.Path.Combine(rootPath, "Logs");
            if (!File.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }

            string fileNameWithDate = "Log_LSRetail_" + DateTime.Today.ToString("ddMMyyyy") + ".txt";

            var fileTarget = new FileTarget();

            fileTarget.FileName = System.IO.Path.Combine(filePathDir, fileNameWithDate);
            fileTarget.Layout = "${longdate:universalTime=true} | ${level:upperCase=true} | ${message}";

            config.AddTarget("file", fileTarget);

            var fileRule = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(fileRule);

            LogManager.Configuration = config;
        }

        public ILogger GetLog([CallerFilePath] string callerFilePath = "")
        {
            string fileName = callerFilePath;

            if (fileName.Contains("/"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf("/", StringComparison.CurrentCultureIgnoreCase) + 1);
            }

            var logger = LogManager.GetLogger(fileName);
            return new NLogLogger(logger);
        }
    }


}