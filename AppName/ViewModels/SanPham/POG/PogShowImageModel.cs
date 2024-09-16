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
using static AppName.PogWithItemsViewModel;

namespace AppName
{
    public class PogShowImageModel : ObservableObject
    {
        private POGWithItemModel dataModel;
        public POGWithItemModel DataModel
        {
            get { return dataModel; }
            set { SetProperty(ref dataModel, value); }
        }

        public EventHandler<POGWithItemModel> ClosePopup;

        public PogShowImageModel(POGWithItemModel model)
        {
            DataModel = model;
        }
    }
}
