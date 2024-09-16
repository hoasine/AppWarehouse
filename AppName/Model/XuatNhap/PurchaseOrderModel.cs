using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppName.Model.XuatNhap
{
    public class LocationModel
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
    }

    #region TO
    public class TOHeaderModel
    {
        public string DocumentNo { get; set; }
        public string VietnameseDescription { get; set; }
        public string Status { get; set; }
        public string TransferfromCode { get; set; }
        public string TransfertoCode { get; set; }
        public string BuyerID { get; set; }
        public int Quantity { get; set; }
        public int ItemCount { get; set; }
        public string PostingDate { get; set; }
    }

    public class TOHeaderPreviewModel
    {
        public string DocumentNo { get; set; }
        public string VietnameseDescription { get; set; }
        public string Status { get; set; }
        public string TransferfromCode { get; set; }
        public string TransfertoCode { get; set; }
        public string BuyerID { get; set; }
        public int Quantity { get; set; }
        public string PostingDate { get; set; }
        public List<TOLineModel> TOLine { get; set; }
    }

    public partial class ResuftTOLineModel
    {
        public bool Active { get; set; }
        public List<TOLineModel> ListData { get; set; }
    }


    public class TOLineModel : ObservableObject
    {
        public string DocumentNo { get; set; }
        public bool? IsDelete { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ColorText { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }

        private int _Quatity;
        public int Quatity
        {
            get { return _Quatity; }
            set { SetProperty(ref _Quatity, value); }
        }

        private int _Quantity_Scan;
        public int Quantity_Scan
        {
            get { return _Quantity_Scan; }
            set { SetProperty(ref _Quantity_Scan, value); }
        }

        public int QtytoShip { get; set; }
    }

    #endregion

    public class CompanyModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class BrandModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
