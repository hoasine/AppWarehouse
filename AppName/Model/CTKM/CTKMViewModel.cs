using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

public class CTKMViewModel : ObservableRangeCollection<CTKMItemViewModel>, INotifyPropertyChanged
{
    // It's a backup variable for storing CountryViewModel objects  
    private ObservableRangeCollection<CTKMItemViewModel> ctkmCTKMItems = new ObservableRangeCollection<CTKMItemViewModel>();

    public CTKMViewModel(CTKM ctkm, bool expanded = false)
    {
        this.CTKM = ctkm;
        this._expanded = expanded;
        foreach (CTKMItem ctkmitem in ctkm.CTKMs)
        {
            ctkmCTKMItems.Add(new CTKMItemViewModel(ctkmitem));
        }
        if (expanded) AddRange(ctkmCTKMItems);
    }

    public CTKMViewModel() { }

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
                    this.AddRange(ctkmCTKMItems);
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
            return CTKM.Name;
        }
    }
    
    public string ItemNo
    {
        get
        {
            return CTKM.ItemNo;
        }
    }

    public CTKM CTKM
    {
        get;
        set;
    }
}
