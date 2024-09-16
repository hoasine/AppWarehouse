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
    public class StockTakeLineDetailViewModel : ObservableObject
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

        private StockCountLineModel _dataModel;
        public StockCountLineModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ICommand UpdateStockTakeLineDetailCommand { get; set; }

        public EventHandler<StockCountLineModel> ClosePopup;

        public StockTakeLineDetailViewModel(StockCountLineModel itemModel)
        {
            UpdateStockTakeLineDetailCommand = new Command(UpdateStockTakeLineDetailCommandAsync);

            DataModel = itemModel;
        }

        private async void UpdateStockTakeLineDetailCommandAsync(object obj)
        {
            ClosePopup?.Invoke(this, DataModel);

            await PopupNavigation.Instance.PopAsync();
        }
    }
}
