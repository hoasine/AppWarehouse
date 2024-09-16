using AppName.Model;
using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class YesNoViewModel : BaseViewModel
    {
        protected INavigation Navigation { get; private set; }

        public ICommand CloseCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
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

        public EventHandler<bool> ClosePopup;

        public YesNoViewModel(string description)
        {
            Description = description;

            ConfirmCommand = new Command(ConfirmCommandASync);
            CloseCommand = new Command(CloseCommandAsync);
        }

        private async void CloseCommandAsync()
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception)
            {
            }
        }

        private async void ConfirmCommandASync(object obj)
        {
            ClosePopup?.Invoke(this, true);

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
