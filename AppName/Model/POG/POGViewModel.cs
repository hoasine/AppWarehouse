using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

public class POGViewModel : ObservableRangeCollection<POGItemViewModel>, INotifyPropertyChanged
{
    // It's a backup variable for storing CountryViewModel objects  
    private ObservableRangeCollection<POGItemViewModel> pogPOGItems = new ObservableRangeCollection<POGItemViewModel>();

    public POGViewModel(POG pog, bool expanded = false)
    {
        this.POG = pog;
        this._expanded = expanded;
        foreach (POGItem pogitem in pog.POGs)
        {
            pogPOGItems.Add(new POGItemViewModel(pogitem));
        }
        if (expanded) AddRange(pogPOGItems);
    }

    public POGViewModel() { }

    private bool _expanded;

    public bool Expanded
    {
        get
        {
            return _expanded;
        }
        set
        {
            if (_expanded != value)
            {
                _expanded = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                if (_expanded)
                {
                    this.AddRange(pogPOGItems);
                }
                else
                {
                    this.Clear();
                }
            }
        }
    }
    public string StateIcon
    {
        get
        {
            if (Expanded)
            {
                return "icon_down.png";
            }
            else
            {
                return "icon_up.png";
            }
        }
    }
    public string Name
    {
        get
        {
            return POG.Name;
        }
    }
    
    public string ItemNo
    {
        get
        {
            return POG.ItemNo;
        }
    }

    public POG POG
    {
        get;
        set;
    }
}
