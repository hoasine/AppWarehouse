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
    public class ReceiveTOLineDetailViewModel : ObservableObject
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

        private TOLineModel _dataModel;
        public TOLineModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }


        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public ICommand UpdateReceiveTOLineDetailCommand { get; set; }

        public EventHandler<TOLineModel> ClosePopup;

        public ReceiveTOLineDetailViewModel(TOLineModel itemModel)
        {
            UpdateReceiveTOLineDetailCommand = new Command(UpdateReceiveTOLineDetailAsync);

            DataModel = itemModel;
        }

        private async void UpdateReceiveTOLineDetailAsync(object obj)
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
    }
}
