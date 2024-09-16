using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

public class POGItemViewModel
{
    private POGItem _pog;
    public POGItemViewModel(POGItem POG)
    {
        _pog = POG;
    }
    public string Description
    {
        get
        {
            return _pog.Description;
        }
    }
    public string FixelID
    {
        get
        {
            return _pog.FixelID;
        }
    }

    public string Image
    {
        get
        {
            return _pog.Image;
        }
    }

    public string POG
    {
        get
        {
            return _pog.POG;
        }
    }

    public decimal TotalFacing
    {
        get
        {
            return _pog.TotalFacing;
        }
    }

    public decimal TotalUnit
    {
        get
        {
            return _pog.TotalUnit;
        }
    }

    public POGItem POGItem
    {
        get => _pog;
    }
}
