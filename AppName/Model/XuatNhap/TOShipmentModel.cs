using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppName.Model.XuatNhap
{
    public partial class Retail_Transfer_OrderModel
    {
        public string Vietnamese_Description { get; set; }
        public string Store_to { get; set; }
        public string Store_from { get; set; }
        public string Transaction_Type { get; set; }
        public DateTime Posting_Date { get; set; }
        public List<TransferLinesModel> TOLines { get; set; }
    }

    public partial class TransferLinesModel
    {
        public string Item_No { get; set; }
        public string ExpireDate { get; set; }
        public int Quantity { get; set; }
    }

    public partial class Retail_Transfer_Order_API_Model
    {
        public bool Active { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public Retail_Transfer_Order_ResuftModel ListData { get; set; }
    }

    public partial class Retail_Transfer_Order_ResuftModel
    {
        public string No { get; set; }
        public string Vietnamese_Description { get; set; }
        public string Store_to { get; set; }
        public string Store_from { get; set; }
        public string Transfer_from_Address { get; set; }
        public string Transfer_to_Address { get; set; }
        public string Buyer_ID { get; set; }
        public DateTime Posting_Date { get; set; }
        public DateTime Receipt_Date { get; set; }
        public DateTime Shipment_Date { get; set; }
        public List<TransferLinesResuftModel> TOLines { get; set; }
    }

    public partial class TransferLinesResuftModel
    {
        public string Item_No { get; set; }
        public string Description { get; set; }
        public string Vietnamese_Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal InventoryByLocation { get; set; }
        public decimal QtyToShip { get; set; }
        public decimal Quantity_Received { get; set; }
    }

}
