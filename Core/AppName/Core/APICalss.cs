using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace AppName.Core
{
    public class APICalss
    {

        public partial class SanPhamViewModel
        {
            public string Barcode_No_ { get; set; }
            public string ItemName { get; set; }
            public string LSRetail_Code { get; set; }
            public string Variant { get; set; }
            public string VendorItemNo { get; set; }
            public string ItemNo { get; set; }
            public string No__Series { get; set; }
            public string BrandName { get; set; }
            public decimal? Unit_Price { get; set; }
        }

        public partial class SanPhamModel
        {
            public string Barcode_No_ { get; set; }
            public string ItemName { get; set; }
            public string LSRetail_Code { get; set; }
            public string Variant { get; set; }
            public string VendorItemNo { get; set; }
            public string ItemNo { get; set; }
            public string No__Series { get; set; }
            public string BrandName { get; set; }
            public string URLImage { get; set; }
            public string COLOURS { get; set; }
            public string UnitPrice { get; set; }
            public int? Quantity { get; set; }
            /// <summary>
            /// Thêm field ?? có d? li?u hi?n th? gi?ng design
            /// </summary>
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        private async void OpenSanPhamDetailAsync(string obj)
        {
            //try
            //{
            //    var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

            //    HttpClient client = new HttpClient();
            //    client.DefaultRequestHeaders.Authorization = authHeader;

            //    Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfo?KeyValue=" + obj + "&pageSize=1", string.Empty));

            //    HttpResponseMessage response = await client.GetAsync(uri);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        string content = await response.Content.ReadAsStringAsync();

            //        var dataList = JsonConvert.DeserializeObject<List<SanPhamViewModel>>(content);

            //        var model = dataList.FirstOrDefault();

            //        if (model != null)
            //        {
            //            var modelData = new List<SanPhamModel>().FirstOrDefault(q => q.ItemNo == model.ItemNo);

            //            if (modelData == null)
            //            {
            //                modelData = new SanPhamModel()
            //                {
            //                    BrandName = model.BrandName,
            //                    Variant = model.Variant,
            //                    Barcode_No_ = model.Barcode_No_,
            //                    LSRetail_Code = model.LSRetail_Code,
            //                    ItemName = model.ItemName + " " + model.ItemNo,
            //                    ItemNo = model.ItemNo,
            //                    No__Series = model.No__Series,
            //                    UnitPrice = model.Unit_Price.Value.ToString("#,##"),
            //                    URLImage = "",
            //                    COLOURS = "",
            //                    VendorItemNo = model.VendorItemNo
            //                };

            //                string[] numbers = new[]
            //                {
            //                           
            //                };

            //                Random rand = new Random();
            //                int index = rand.Next(numbers.Length);

            //                modelData.URLImage = numbers[index];

            //                if (string.IsNullOrEmpty(modelData.URLImage))
            //                {
            //                    modelData.URLImage = "imageempty.png";
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

    }
}
