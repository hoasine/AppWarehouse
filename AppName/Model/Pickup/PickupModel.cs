using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppName.Model.Pickup
{

    public class PickUpProductDetail : ObservableObject
    {
        public Guid ID { get; set; }
        public Guid MasterID { get; set; }
        public int? IsDelete { get; set; }
        public string ItemNo { get; set; }
        public string BarcodeNo { get; set; }
        public string ItemName { get; set; }
        public string ImageFile { get; set; }
        public string Note { get; set; }
        public string Reason { get; set; }
        public int Quantity { get; set; }
        private int _QuantityScan;
        public int QuantityScan
        {
            get { return _QuantityScan; }
            set { SetProperty(ref _QuantityScan, value); }
        }
        public DateTime DateCreate { get; set; }
    }

    public class PickUpProductCount : ObservableObject
    {
        private int _TotalScan;
        public int TotalScan
        {
            get { return _TotalScan; }
            set { SetProperty(ref _TotalScan, value); }
        }

        private int _TotalItem;
        public int TotalItem
        {
            get { return _TotalItem; }
            set
            {
                SetProperty(ref _TotalItem, value);
            }
        }
    }


    public partial class PickUpProductMaster
    {
        public Guid ID { get; set; }
        public DateTime DateCreate { get; set; }
        public string UserCreate { get; set; }
        public string PickUpName { get; set; }
        public string Status { get; set; }
        public int IsDelete { get; set; }
        public string StoreNo { get; set; }
    }

    public partial class ResuftPickUpModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool Active { get; set; }
    }
}
