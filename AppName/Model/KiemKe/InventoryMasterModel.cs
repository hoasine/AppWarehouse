using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public partial class InventoryMasterModel
{
    public Guid ID { get; set; }
    public string Descriptions { get; set; }
    public string StoreName { get; set; }
    public string StoreID { get; set; }
    public string UserName { get; set; }
    public string CheckType { get; set; }
    public DateTime DatePonit { get; set; }
}