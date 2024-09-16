using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static AppName.ViewModels.APIBaseViewModel;


public partial class StockCountModel : ObservableObject
{
    public string ID { get; set; }
    public string Desciption { get; set; }
    public string NoLine { get; set; }
    public string DocumentNo { get; set; }
    public string LocationCode { get; set; }
    public string Status { get; set; }
    public string Management { get; set; }
    public string AreMask { get; set; }
    public string RetailStaff { get; set; }
    public int IsSend { get; set; }
    public int Release { get; set; }
    public string DocumentRoot { get; set; }
    public string ReferenceNo { get; set; }
    public int IsDelete { get; set; }
    public int QtyPack { get; set; }
    public DateTime? DateCreate { get; set; }
    public string UserCreate { get; set; }

    private int _CountItem;
    public int CountItem
    {
        get { return _CountItem; }
        set { SetProperty(ref _CountItem, value); }
    }

    private int _SumQuantityLine;
    public int SumQuantityLine
    {
        get { return _SumQuantityLine; }
        set
        {
            SetProperty(ref _SumQuantityLine, value);

            Remain = SumQuantityLine - SumQuantity_Scan;
        }
    }

    private int _SumQuantity_Scan;
    public int SumQuantity_Scan
    {
        get { return _SumQuantity_Scan; }
        set
        {
            SetProperty(ref _SumQuantity_Scan, value);

            Remain = SumQuantityLine - SumQuantity_Scan;
        }
    }

    private int _Remain;
    public int Remain
    {
        get { return _Remain; }
        set { SetProperty(ref _Remain, value); }
    }

    //public int Remain => SumQuantityLine - SumQuantity_Scan;
    public List<StockCountLineModel> ListData { get; set; }

}

public partial class StockCountLineModel : ObservableObject
{
    public string ID { get; set; }
    public string DocumentNo { get; set; }
    public string BarcodeNo { get; set; }
    public string IsHasData { get; set; }
    public string ItemName { get; set; }
    public string Image { get; set; }
    public string ReferenceInfo { get; set; }
    public string ItemNo { get; set; }
    public int Quantity { get; set; }

    private string _Zone;
    public string Zone
    {
        get { return _Zone; }
        set { SetProperty(ref _Zone, value); }
    }
    
    private string _ColorIsScan;
    public string ColorIsScan
    {
        get { return _ColorIsScan; }
        set { SetProperty(ref _ColorIsScan, value); }
    }

    private int _Quantity_Scan;
    public int Quantity_Scan
    {
        get { return _Quantity_Scan; }
        set { SetProperty(ref _Quantity_Scan, value); }
    }

    //public int Quantity_Scan { get; set; }
    public int QuantityPOG { get; set; }
    public string FixID { get; set; }
    public string POG { get; set; }
    public DateTime DateCreate { get; set; }
    public string UserCreate { get; set; }
    public int IsDelete { get; set; }
    public int Remain => Quantity - Quantity_Scan;
    public bool? ComparePCS { get; set; }
}



