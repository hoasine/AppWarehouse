using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public class CTKMItemViewModel
{
    private CTKMItem _ctkm;
    public CTKMItemViewModel(CTKMItem ctkm)
    {
        _ctkm = ctkm;
    }
    public string ItemNo
    {
        get
        {
            return _ctkm.ItemNo;
        }
    }
    public string Scheme_Description
    {
        get
        {
            return _ctkm.Scheme_Description;
        }
    }

    public string Disc_Percent
    {
        get
        {
            return _ctkm.Disc_Percent;
        }
    }

    public string Division
    {
        get
        {
            return _ctkm.Division;
        }
    }

    public string Item_Category
    {
        get
        {
            return _ctkm.Item_Category;
        }
    }

    public string Item_Cate_Desc
    {
        get
        {
            return _ctkm.Item_Cate_Desc;
        }
    }

    public string Product_Group_Code
    {
        get
        {
            return _ctkm.Product_Group_Code;
        }
    }

    public string Product_Group_Name
    {
        get
        {
            return _ctkm.Product_Group_Name;
        }
    }


    public string Type
    {
        get
        {
            return _ctkm.Type;
        }
    }


    public string Disc_Amount
    {
        get
        {
            return _ctkm.Disc_Amount;
        }
    }

    public string Standard_Price_Including_VAT
    {
        get
        {
            return _ctkm.Standard_Price_Including_VAT;
        }
    }

    public string Disc_Amount_Include_VAT
    {
        get
        {
            return _ctkm.Disc_Amount_Include_VAT;
        }
    }

    public string Offer_Price_Including_VAT
    {
        get
        {
            return _ctkm.Offer_Price_Including_VAT;
        }
    }

    public bool IsCheckViewDiscoutOffer
    {
        get
        {
            return _ctkm.IsCheckViewDiscoutOffer;
        }
    }
    
    
    public bool IsCheckViewMultyMixMatch
    {
        get
        {
            return _ctkm.IsCheckViewMultyMixMatch;
        }
    }
    
    public CTKMItem CTKMItem
    {
        get => _ctkm;
    }
}
