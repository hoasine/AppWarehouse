using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

public partial class ResuftMasterFileModel
{
    public bool Active { get; set; }
    public List<SanPhamViewModel> ListData { get; set; }
}

public partial class SanPhamViewModel
{
    public string Barcode_No_ { get; set; }
    public string ItemNo { get; set; }
    public string ItemName { get; set; }
    public string Image { get; set; }
    public string ReturnType { get; set; }
    public string ReturnComment { get; set; }
    public string ReturnDesc { get; set; }
    public string StatusItem { get; set; }
    public string ItemCategory { get; set; }
    public string Division { get; set; }
    public string CrossDocking { get; set; }
    public int ExpireDate { get; set; }
    public int Stock { get; set; }
    public decimal? Unit_Price { get; set; }
}

public partial class SanPhamModel
{
    public string Barcode_No_ { get; set; }
    public string ItemNo { get; set; }
    public string ItemName { get; set; }
    public string ItemCategory { get; set; }
    public string Division { get; set; }
    public string Image { get; set; }
    public string ReturnType { get; set; }
    public string ReturnDesc { get; set; }
    public string CrossDocking { get; set; }
    public decimal? Unit_Price { get; set; }
    public string ReturnComment { get; set; }
    public string URLImage { get; set; }
    public string ExpireDate { get; set; }
    public string StatusItem { get; set; }
    public string UnitPrice { get; set; }
    public ImageSource ImageSource { get; set; }
    public int? Quantity { get; set; }
    /// <summary>
    /// Thêm field để có dữ liệu hiển thị giống design
    /// </summary>
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
