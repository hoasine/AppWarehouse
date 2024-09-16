using AppName.Model.XuatNhap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class ReceivePOPreviewViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private POHeaderPreviewModel _dataModel;
        public POHeaderPreviewModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _visibleNotification;
        public bool VisibleNotification
        {
            get { return _visibleNotification; }
            set { SetProperty(ref _visibleNotification, value); }
        }

        public ICommand ConfirmCommand { get; set; }

        public ReceivePOPreviewViewModel(INavigation navigation, POHeaderPreviewModel itemModel)
        : base(listenCultureChanges: true)
        {
            ConfirmCommand = new Command(ConfirmAsync);
            Navigation = navigation;
            DataModel = itemModel;
        }

        private async void ConfirmAsync(object obj)
        {
            MessagingCenter.Send<App>((App)Application.Current, "ClosePagePOPreview");

            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception) { }
        }
    }
}
