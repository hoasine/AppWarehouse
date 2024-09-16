using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

public class POG
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
    public List<POGItem> POGs
    {
        get;
        set;
    }
    public bool IsVisible
    {
        get;
        set;
    } = true;

    public string Description { get; set; }
    public Color BackgroundColor { get; set; }

    public POG() { }
    public POG(string name, List<POGItem> pog)
    {
        Name = name;
        Total = pog.Count;
        POGs = pog;
    }


}
