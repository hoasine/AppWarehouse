using AppName.Model.XuatNhap;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static AppName.AddStockTakeViewModel;

namespace AppName
{
    public class AddAreasViewModel : ObservableObject
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

        private AreasModel _dataModel;
        public AreasModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ICommand AddAreaCommand { get; set; }

        public EventHandler<AreasModel> ClosePopup;

        public AddAreasViewModel(AreasModel itemModel)
        {
            AddAreaCommand = new Command(AddAreaCommandAsync);

            if (itemModel == null)
            {
                DataModel = new AreasModel();
            }
            else
            {
                DataModel = itemModel;
            }
        }

        private async void AddAreaCommandAsync(object obj)
        {
            if (string.IsNullOrEmpty(DataModel.MasrkName))
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please input data AreaName and Number." };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            else
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
}
