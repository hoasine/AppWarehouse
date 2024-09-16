using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public partial class CTKMItem
{
    public string ItemNo { get; set; }
    public string Scheme_Description { get; set; }
    public string Division { get; set; }
    public string Item_Category { get; set; }
    public string Item_Cate_Desc { get; set; }
    public string Product_Group_Code { get; set; }
    public string Product_Group_Name { get; set; }
    public string Disc_Percent { get; set; }
    public string Disc_Amount { get; set; }
    public string Type { get; set; }
    public string Standard_Price_Including_VAT { get; set; }
    public string Disc_Amount_Include_VAT { get; set; }
    public string Offer_Price_Including_VAT { get; set; }
    public bool IsCheckViewDiscoutOffer { get; set; } = false;
    public bool IsCheckViewMultyMixMatch { get; set; } = false;
    public bool IsVisible { get; set; } = false;
}


