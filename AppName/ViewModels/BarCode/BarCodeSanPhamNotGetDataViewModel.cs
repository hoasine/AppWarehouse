using System;
using Xamarin.Forms;
using AppName.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using System.Windows.Input;
using AppName.CustomRenderer;

namespace AppName
{
    public class BarCodeSanPhamNotGetDataViewModel : ObservableObject
    {
        private readonly string _barcodeID;
        private SanPhamModel _barcode;
        private string _itemno;
        private bool _isHasData;
        private bool _isMess;

        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        public bool IsHasData
        {
            get => _isHasData;

            set => SetProperty(ref _isHasData, value);
        }

        private bool _isEditData;
        public bool IsEditData
        {
            get { return _isEditData; }
            set { SetProperty(ref _isEditData, value); }
        }

        public ICommand AddCommand { get; set; }
        /// <summary>
        /// Tạo EventHandler để khi Close Popup truyền được BarCode mới sửa xuống Grid
        /// </summary>
        public EventHandler<SanPhamModel> ClosePopup;

        public BarCodeSanPhamNotGetDataViewModel(SanPhamModel model)
        {
            AddCommand = new Command(AddAsync);

            _barcodeID = model.Barcode_No_;

            if (_barcodeID != null)
            {
                _itemno = "Product: " + model.Barcode_No_;


                if (!string.IsNullOrEmpty(model.ItemNo))
                {
                    model.ImageSource = ConvertImageBase64.ConvertImage(model.URLImage);

                    BarCode = model;

                    IsHasData = true;
                    IsMess = false;
                }
                else
                {
                    IsHasData = false;
                    IsMess = true;
                }
            }
        }

        private async void AddAsync(object obj)
        {
            ClosePopup?.Invoke(this, BarCode);

            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }

        public SanPhamModel BarCode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }
    }
}
