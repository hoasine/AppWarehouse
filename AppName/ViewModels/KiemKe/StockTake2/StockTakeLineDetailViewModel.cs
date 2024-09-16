using AppName.Model;
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
    public class StockTakeLineDetailViewModel : BaseViewModel
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

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set
            {
                _showLoading = value;
                OnPropertyChanged();
            }
        }
        
        private bool _IsEnabledButton;
        public bool IsEnabledButton
        {
            get { return _IsEnabledButton; }
            set
            {
                _IsEnabledButton = value;
                OnPropertyChanged();
            }
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
            try
            {
                ClosePopup?.Invoke(this, DataModel);

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }
    }
}
