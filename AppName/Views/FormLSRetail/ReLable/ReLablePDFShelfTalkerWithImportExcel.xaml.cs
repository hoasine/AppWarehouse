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

namespace AppName
{
    public partial class ReLablePDFShelfTalkerWithImportExcel : ContentPage
    {
        #region Bindable property
        public static BindableProperty DidScannedCommandProperty = BindableProperty.Create("DidScannedCommand",
        typeof(Command<List<string>>),
        typeof(ReLablePDFShelfTalkerWithImportExcel),
        null);

        ReLablePDFShelfTalkerWithImportExcekViewModel viewModel;

        #endregion

        public ReLablePDFShelfTalkerWithImportExcel()
        {
            InitializeComponent();

            BindingContext = viewModel = new ReLablePDFShelfTalkerWithImportExcekViewModel(Navigation);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (SearchBar)sender;

            try
            {
                if (entry.Text == "")
                {
                    viewModel.SearchItemsCommand.Execute("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: {0}", ex);
            }
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}