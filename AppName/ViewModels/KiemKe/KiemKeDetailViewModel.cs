using AppName.Model;
using AppName.ViewModels.BarCode.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppName
{
    public class KiemKeDetailViewModel : BaseViewModel
    {
        private InventoryMasterModel _inventoryMaster;

        public InventoryMasterModel InventoryMaster
        {
            get => _inventoryMaster;

            set => SetProperty(ref _inventoryMaster, value);
        }

        private ObservableCollection<InventoryDetailModel> _inventoryDetail;

        public ObservableCollection<InventoryDetailModel> InventoryDetail
        {
            get => _inventoryDetail;

            set => SetProperty(ref _inventoryDetail, value);
        }

        public KiemKeDetailViewModel(InventoryMasterModel model = null)
        {
            InventoryMaster = model;
        }
    }
}
