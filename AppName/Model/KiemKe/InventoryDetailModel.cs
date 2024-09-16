using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public partial class InventoryDetailModel
{
    public Guid ID { get; set; }
    public string MasterInventoryID { get; set; }
    public string ItemNo { get; set; }
    public string Descriptions { get; set; }
    public string VariantCode { get; set; }
    public string ActualQuatity { get; set; }
    public string ScanQuatity { get; set; }
    public string UnitPrice { get; set; }
}

public partial class TotalInventoryModel
{
    public string LocationCode { get; set; }
    public string TotalInventory { get; set; }
}

