using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

public class CTKM
{
    public string Name
    {
        get;
        set;
    }
    
    public string ItemNo
    {
        get;
        set;
    }
    
    public int Total
    {
        get;
        set;
    }
    public List<CTKMItem> CTKMs
    {
        get;
        set;
    }
    public bool IsVisible
    {
        get;
        set;
    } = true;

    public string ImageURL { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Store { get; set; }
    public string PromotionType { get; set; }
    public Color BackgroundColor { get; set; }

    public CTKM() { }
    public CTKM(string name, List<CTKMItem> ctkm)
    {
        Name = name;
        Total = ctkm.Count;
        CTKMs = ctkm;
    }


}
