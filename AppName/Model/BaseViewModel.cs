using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppName.Model
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string busyText = string.Empty;
        string title = string.Empty;



        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string BusyText
        {
            get => busyText;
            set => SetProperty(ref busyText, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class LicenseModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public bool IsActive { get; set; }
    }

    public class LocalPermissionModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string KeyPermission { get; set; }
        public string Role { get; set; }
    }

    public class UserLSRetailConfig : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string UserName { get; set; }
        public string Password { get; set; }
        public string URLApi { get; set; }
        public string StoreNo { get; set; }
        public string UserStore { get; set; }
        public string LisenceKey { get; set; }
    }

    public class ItemPromotionModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeDiscount { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }


    public class ItemPromotionMultiModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeDiscount { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }

    public class ItemPromotionPDFMSC3TagModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string DatetimeDiscount { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }

    public class ItemPromotionPDFSmallModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string DatetimeDiscount { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }
    public class ItemPromotionPDFShelfTalkerModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string DatetimeDiscount { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }

    public class ItemPromotionPDFShelfTalkerMultiModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string POG { get; set; }
        public string ReturnType { get; set; }
        public string DatetimeDiscount { get; set; }
        public string SchemeDescriptionMixMatch { get; set; }
        public string DatetimeMixMatch { get; set; }
        public string SchemeDescriptionMultiBuy { get; set; }
        public string DatetimeMultil { get; set; }
        public decimal? Unit_Price { get; set; }
        public decimal? Discout { get; set; }
        public string PromotionNow { get; set; }
        public string PromotionUpdate { get; set; }
        public decimal? AfterDiscMember { get; set; }
        public string DateDiscountMember { get; set; }
    }

    public class TemplatePrinterModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string TypeTemplate { get; set; }
        public string Left { get; set; }
        public string Top { get; set; }
        public string Right { get; set; }
        public string Bottom { get; set; }
    }

    public class CreateTOModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
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
        public string URLImage { get; set; }
        public string ExpireDate { get; set; }
        public string UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class CreateTOHeaderModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string Store_to { get; set; }
        public string Store_to_name { get; set; }
        public string Store_to_address { get; set; }
        public string Store_from { get; set; }
        public string Store_from_name { get; set; }
        public string Store_from_address { get; set; }
        public string Vietnamese_Description { get; set; }
        public string Posting_Date { get; set; }
    }

    public class StockTakeLocalModel : RealmObject
    {
        [PrimaryKey]
        public ObjectId ID { get; set; } = ObjectId.GenerateNewId();
        public string IDItem { get; set; }
        public string Zone { get; set; }
        public int QuantityPOG { get; set; }
        public string FixelID { get; set; }
        public string POG { get; set; }
        public string DocumentRoot { get; set; }
        public string DocumentNo { get; set; }
        public string BarcodeNo { get; set; }
        public string Image { get; set; }
        public string ItemName { get; set; }
        public string ItemNo { get; set; }
        public string DateCreate { get; set; }
        public int Quantity { get; set; }
        public int Quantity_Scan { get; set; }
        public int IsDelete { get; set; }
        public string UserCreate { get; set; }
        public bool IsSync { get; set; }
    }
}



