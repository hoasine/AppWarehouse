using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace AppName.Model.CycleCount
{
    public partial class UpsertOrDeletePOLineModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
    }

    public class POLineModel
    {
        public string DocumentNo { get; set; }
        public bool? IsDelete { get; set; }
        public string ItemNo { get; set; }
        public string ColorText { get; set; }
        public string ItemName { get; set; }
        public string Image { get; set; }
        public int Quatity { get; set; }
        public int QtytoReceive { get; set; }
        public int Quantity_Scan { get; set; }
    }

    public partial class LSRetail_StockCountDetailListModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public List<StockCountLineModel> ListData { get; set; }
    }

    public partial class RealseDocumentScanModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
    }

    public partial class LSRetail_StockCountDetailModel
    {
        public string ID;
        public string FixelID;
        public string POG;
        public int Zone;
        public string DocumentNo;
        public string BarcodeNo;
        public string Image;
        public string ItemName;
        public string ItemNo;
        public DateTime DateCreate;
        public int Quantity;
        public int Quantity_Scan;
        public int IsDelete;
        public string UserCreate;
    }

}

