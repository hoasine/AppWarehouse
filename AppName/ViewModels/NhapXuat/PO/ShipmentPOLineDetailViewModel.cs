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
    public class ShipmentPOLineDetailViewModel : ObservableObject
    {
        private bool _isUpdate;
        /// <summary>
        /// 0: tạo mới
        /// 1: cập nhật
        /// </summary>
        ///   private bool _showLoading;
        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public bool IsUpdate
        {
            get { return _isUpdate; }
            set { SetProperty(ref _isUpdate, value); }
        }

        private POLineModel _dataModel;
        public POLineModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ICommand UpdateShipmentPOLineDetailCommand { get; set; }

        public EventHandler<POLineModel> ClosePopup;

        public ShipmentPOLineDetailViewModel(POLineModel itemModel)
        {
            UpdateShipmentPOLineDetailCommand = new Command(UpdateShipmentPOLineDetailAsync);

            DataModel = itemModel;
        }

        private async void UpdateShipmentPOLineDetailAsync(object obj)
        {
            ClosePopup?.Invoke(this, DataModel);

            try
            {
                await PopupNavigation.Instance.PopAsync();

            }
            catch (Exception) { }
        }
    }
}
