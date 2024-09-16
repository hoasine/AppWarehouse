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

namespace AppName
{
    public class CheckItemDayDetailViewModel : ObservableObject
    {
        private readonly string _barcodeID;
        private CheckSaleModel _barcode;
        private string _itemno;
        private bool _isHasData;
        private bool _isMess;

        public string ItemNo
        {
            get => _itemno;

            set => SetProperty(ref _itemno, value);
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

        public CheckItemDayDetailViewModel(CheckSaleModel model)
        {
            _barcodeID = model.BarcodeNo;

            if (_barcodeID != null)
            {
                _itemno = "Product: " + model.BarcodeNo;


                if (!string.IsNullOrEmpty(model.ItemNo))
                {
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

        public CheckSaleModel BarCode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }
    }
}
