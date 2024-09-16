using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public partial class ItemInfoUpdateGiaModel
{
    public string Barcode { get; set; }
    public string ItemName { get; set; }
    public string ItemNo { get; set; }
    public string POG { get; set; }
    public string ReturnType { get; set; }
    public string SchemeDescriptionMixMatch { get; set; }
    public string DatetimeMixMatch { get; set; }
    public string SchemeDescriptionMultiBuy { get; set; }
    public string DatetimeDiscount { get; set; }
    public string DatetimeMultil { get; set; }
    public decimal? Unit_Price { get; set; }
    public decimal? Discout { get; set; }
    public string PromotionNow { get; set; }
    public string PromotionUpdate { get; set; }
}