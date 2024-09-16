using AppName.Model.XuatNhap;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class ShipmentTOLineDetailViewModel : ObservableObject
    {
        private bool _isUpdate;
        /// <summary>
        /// 0: tạo mới
        /// 1: cập nhật
        /// </summary>
        public bool IsUpdate
        {
            get { return _isUpdate; }
            set { SetProperty(ref _isUpdate, value); }
        }
        
        private bool _isOK;
        /// <summary>
        /// 0: tạo mới
        /// 1: cập nhật
        /// </summary>
        public bool IsOK
        {
            get { return _isOK; }
            set { SetProperty(ref _isOK, value); }
        }

        private TOLineModel _dataModel;
        public TOLineModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private string _lbThongBao;
        public string LBThongBao
        {
            get { return _lbThongBao; }
            set { SetProperty(ref _lbThongBao, value); }
        }


        private bool _visibleThongBao;
        public bool visibleThongBao
        {
            get { return _visibleThongBao; }
            set { SetProperty(ref _visibleThongBao, value); }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }


        public Command ChangeQuantity { get; set; }


        public ICommand UpdateShipmentTOLineDetailCommand { get; set; }

        public EventHandler<TOLineModel> ClosePopup;

        public ShipmentTOLineDetailViewModel(TOLineModel itemModel)
        {
            UpdateShipmentTOLineDetailCommand = new Command(UpdateShipmentTOLineDetailAsync);

            visibleThongBao = false;
            IsOK = false;

            DataModel = itemModel;
            DataModel.Quantity_Scan = DataModel.Quatity;

            ChangeQuantity = new Command(ChangeQuantityAsync);
        }

        public void ChangeQuantityAsync()
        {
            try
            {
                if (DataModel.Stock < DataModel.Quatity)
                {
                    DataModel.Quatity = DataModel.Quantity_Scan;
                    visibleThongBao = true;
                    LBThongBao = "Item:=" + DataModel.ItemNo + " currently has " + DataModel.Stock + " pcs in stock. The system inventory is lower than the actual quantity.";

                    IsOK = false;
                }
                else
                {
                    visibleThongBao = false;
                    LBThongBao = "";

                    IsOK = true;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async void UpdateShipmentTOLineDetailAsync(object obj)
        {
            if (IsOK == false)
            {
                visibleThongBao = true;
                LBThongBao = "Item:=" + DataModel.ItemNo + " currently has " + DataModel.Stock + " pcs in stock. The system inventory is lower than the actual quantity.";
                return;
            }

            ClosePopup?.Invoke(this, DataModel);

            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception) { }
        }
    }
}
