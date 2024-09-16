using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

public partial class ChecksaleJsonModel
{
    public List<ChecksaleDetailJsonModel> data { get; set; }
    public int code { get; set; }
    public bool success { get; set; }
    public string message { get; set; }
}

public partial class ChecksaleDetailJsonModel
{
    public string ItemNo { get; set; }
    public string ProductDescriptionEN { get; set; }
    public string ProductDescriptionVN { get; set; }
    public string BarcodeNo { get; set; }
    public string LocationCode { get; set; }
    public string StoreName { get; set; }
    public string Category { get; set; }
    public string SubCategoryName { get; set; }
    public string Image { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Stock { get; set; }
    public decimal? Nhap { get; set; }
    public decimal? Xuat { get; set; }
    public decimal? UnPosted { get; set; }
    public decimal? HHPOG_TOTALUNIT { get; set; }
    public string FIXID_SHELF { get; set; }
    public decimal? Sold_Qty_With_Days { get; set; }
}


public partial class CheckSaleModel
{
    public int STT { get; set; }
    public string LocationCode { get; set; }
    public string StoreName { get; set; }
    public string ItemName { get; set; }
    public string ItemNameVN { get; set; }
    public string ItemNo { get; set; }
    public string BarcodeNo { get; set; }
    public string Category { get; set; }
    public string Image { get; set; }
    public string SubCategoryName { get; set; }
    public string UnitPrice { get; set; }
    public int Inventory { get; set; }
    public int? Nhap { get; set; }
    public int? Xuat { get; set; }
    public int? UnPosted { get; set; }
    public int? HHPOG_TOTALUNIT { get; set; }
    public string FIXID_SHELF { get; set; }
    public decimal? Sold_Qty_With_Days { get; set; }
    public ImageSource ImageSource { get; set; }
}


