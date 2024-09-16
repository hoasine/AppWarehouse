using System;
using Xamarin.Forms;
using AppName.Core;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Permissions;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System.IO;

namespace AppName
{
    public partial class PDFViewPage : ContentPage
    {
        private string fileNameUrl = "";

        public PDFViewPage(string pdfString, string fileName)
        {
            InitializeComponent();

            fileNameUrl = fileName;

            byte[] bytes = Convert.FromBase64String(pdfString);

            Stream documenStream = new MemoryStream(bytes);
            pdfViewer.LoadDocument(documenStream);
            printButton.Clicked += PrintButton_Clicked;
        }
        private void PrintButton_Clicked(object sender, EventArgs e)
        {
            Stream printStream = pdfViewer.SaveDocument();
            DependencyService.Get<IPrintService>().Print(printStream, fileNameUrl + ".pdf");
        }

    }
}