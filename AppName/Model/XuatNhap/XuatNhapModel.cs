using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppName.Model.XuatNhap
{
    public class POHeaderModel
    {
        public string DocumentNo { get; set; }
        public string vendorName { get; set; }
        public string StoreNo { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public int ItemCount { get; set; }
        public string PostingDate { get; set; }
    }

    public class POHeaderPreviewModel
    {
        public string DocumentNo { get; set; }
        public string StoreNo { get; set; }
        public string Status { get; set; }
        public string vendorName { get; set; }
        public int Quantity { get; set; }
        public int ItemCount { get; set; }
        public string PostingDate { get; set; }
        public List<POLineModel> POLine { get; set; }
    }

    public partial class ResuftPOLineModel
    {
        public bool Active { get; set; }
        public List<POLineModel> ListData { get; set; }
    }
    public class POLineModel
    {
        public string DocumentNo { get; set; }
        public bool? IsDelete { get; set; }
        public string ItemNo { get; set; }
        public string ColorText { get; set; }
        public string LineNo { get; set; }
        public string ItemName { get; set; }
        public string Image { get; set; }
        public int Quatity { get; set; }
        public int QtytoReceive { get; set; }
        public int Quantity_Scan { get; set; }
    }
}
