using Xamarin.Forms;
using AppName.Core;
using Realms;
using AppName.Model;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;

namespace AppName
{
    public static class ResourceHelper
    {
        public static bool PingWithResponse(int timeout, string iP, out string response)
        {
            //try
            //{
            //    PopupNavigation.Instance.PopAsync();
            //    PopupNavigation.Instance.PopAsync();
            //}
            //catch (Exception) { }

            bool result = false;

            using (var p = new Ping())
            {
                var r = p.Send(iP, timeout);

                //response = $"Ping to {iP} [{r.Address}]";
                response = $"Ping to {iP}.";

                if (r.Status == IPStatus.Success)
                {
                    response += $" Successful Response delay = {r.RoundtripTime} ms";
                    result = true;
                }
                else
                {
                    response += $" Failed Status: {r.Status}";
                }
            }

            return result;
        }


        public static T FindResource<T>(string resourceKey)
        {
            if (Application.Current?.Resources != null &&
                Application.Current.Resources.TryGetValue(resourceKey, out var result))
            {
                if (result is T)
                {
                    return (T)result;
                }
                else if (result is OnPlatform<T>)
                {
                    return (OnPlatform<T>)result;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Kiểm tra license.
        /// false: hiển thị popup thông báo license hết hạn.
        /// true: tiếp tục sử dụng app
        /// </summary>
        /// <returns></returns>
        public static bool CheckLicenseAvailable()
        {
            var license = RealmHelper.GetLicense();

            return license != null && license.IsActive;
        }


    }

    public static class RealmHelper
    {
        public static Realm Instance
        {
            get
            {
                var config = new RealmConfiguration
                {
                    SchemaVersion = 22,
                    ShouldDeleteIfMigrationNeeded = false
                };

                return Realm.GetInstance(config);
            }
        }

        /// <summary>
        /// lấy dữ liệu cấu hình License tại localDB của app.
        /// Dữ liệu luôn được lưu 1 dòng.
        /// Nếu có thay đổi chỉ cập nhật chứ ko được tạo mới
        /// </summary>
        /// <returns></returns>
        public static LicenseModel GetLicense()
        {
            return Instance.All<LicenseModel>().FirstOrDefault();
        }

        public static CreateTOModel GetItemTO(string ItemNo)
        {
            return Instance.All<CreateTOModel>().Where(s => s.ItemNo == ItemNo).FirstOrDefault();
        }

        public static List<CreateTOModel> GetItemTOAll()
        {
            return Instance.All<CreateTOModel>().ToList();
        }

        public static CreateTOHeaderModel GetItemTOHeader()
        {
            return Instance.All<CreateTOHeaderModel>().FirstOrDefault();
        }

        public static List<LicenseModel> GetLisenceListAll()
        {
            return Instance.All<LicenseModel>().ToList();
        }


        public static List<ItemPromotionModel> GetItemPromotionAll()
        {
            return Instance.All<ItemPromotionModel>().ToList();
        }

        public static List<ItemPromotionMultiModel> GetItemPromotionMultiAll()
        {
            return Instance.All<ItemPromotionMultiModel>().ToList();
        }

        public static List<ItemPromotionPDFSmallModel> GetItemPromotionPDFSmallAll()
        {
            return Instance.All<ItemPromotionPDFSmallModel>().ToList();
        }
        
        public static List<ItemPromotionPDFMSC3TagModel> GetItemPromotionPDFMSC3TagAll()
        {
            return Instance.All<ItemPromotionPDFMSC3TagModel>().ToList();
        }

        public static List<ItemPromotionPDFShelfTalkerModel> GetItemPromotionPDShelfTalkerAll()
        {
            return Instance.All<ItemPromotionPDFShelfTalkerModel>().ToList();
        }


        public static List<ItemPromotionPDFShelfTalkerMultiModel> GetItemPromotionPDShelfTalkerMultiAll()
        {
            return Instance.All<ItemPromotionPDFShelfTalkerMultiModel>().ToList();
        }

        public static StockTakeLocalModel GetItemStockTake(string ID)
        {
            return Instance.All<StockTakeLocalModel>().Where(s => s.IDItem == ID).FirstOrDefault();
        }

        /// <summary>
        /// Update dữ liệu localDB(các class kế thừa RealmObject).
        /// Mặc định Primakey sẽ tự sinh dữ liệu.
        /// Nếu update = false, sẽ tạo dòng mới với giá trị Primakey mới, ngược lại sẽ update dòng cũ
        /// </summary>
        /// <param name="update"></param>
        /// <param name="listModel"></param>
        public static void UpdateModel<T>(T obj, bool update = true) where T : IRealmObject
        {
            using (var transaction = Instance.BeginWrite())
            {
                Instance.Add(obj, update);

                transaction.Commit();
            }
        }

        public static void UpdateModel<T>(IEnumerable<T> obj, bool update = true) where T : IRealmObject
        {
            using (var transaction = Instance.BeginWrite())
            {
                Instance.Add(obj, update);

                transaction.Commit();
            }
        }

        public static void RemoveAll<T>() where T : IRealmObject
        {
            using (var transaction = Instance.BeginWrite())
            {
                Instance.RemoveAll<T>();

                transaction.Commit();
            }
        }
    }
}