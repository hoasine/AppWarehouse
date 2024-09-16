using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace AppName.Model.StockTake
{

    public partial class UpsertOrDeleteTOLineModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
    }

    public partial class RealseDocumentScanModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
    }

    public class TOLineModel
    {
        public string DocumentNo { get; set; }
        public bool? IsDelete { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ColorText { get; set; }
        public string Image { get; set; }
        public int Quatity { get; set; }
        public int QtytoShip { get; set; }
        public int Quantity_Scan { get; set; }
    }


    public partial class LSRetail_StockCountDetailListModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public List<StockCountLineModel> ListData { get; set; }
    }

    public partial class LSRetail_StockCountMaster_ResuftModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public LSRetail_StockCountMasterModel ListData { get; set; }
    }

    public partial class LSRetail_StockCountMasterModel
    {
        public string ID;
        public string Description;
        public string DocumentNo;
        public string StockType;
        public string LocationCode;
        public string Status;
        public string Management;
        public int IsSend;
        public int Release;
        public int IsDelete;
        public int QtyPack;
        public DateTime DateCreate;
        public string UserCreate;
        public List<LSRetail_StockCountDetailModel> Lines;
    }

    public partial class LSRetail_StockCountDetailModel
    {
        public string ID;
        public int Zone;
        public string POG;
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

    public partial class UpsertOrDeletePOLineModel
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
    }

    public class POLineModel
    {
        public int Zone { get; set; }
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

    public partial class ProductInfoStockResutModel
    {
        public bool Active { get; set; }
        public List<ProductInfoStockModel> ListData { get; set; }
    }

    public class ProductInfoStockModel
    {
        public string LocationCode;
        public string BarcodeNo;
        public string ItemNo;
        public string ItemName;
        public string POG;
        public string FixelID;
        public int Stock;
        public int QuantityPOG;
        public decimal UnitPrice;
    }
}

