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
using NLog;

public class NLogLogger : ILogger
{
    private Logger log;

    public NLogLogger(Logger log)
    {
        this.log = log;
    }

    public void Debug(string text, params object[] args)
    {
        log.Debug(text, args);
    }

    public void Error(string text, params object[] args)
    {
        log.Error(text, args);
    }

    public void Fatal(string text, params object[] args)
    {
        log.Fatal(text, args);
    }

    public void Info(string text, params object[] args)
    {
        log.Info(text, args);
    }

    public void Trace(string text, params object[] args)
    {
        log.Trace(text, args);
    }

    public void Warn(string text, params object[] args)
    {
        log.Warn(text, args);
    }

}