using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppName.ViewModels
{
    public abstract class APIBaseViewModel
    {
        #region Fields

        public class ApiOutput
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
        }

        /// <summary>
        /// để tên vậy cho dễ. Nghĩa là class này chuyên nhận list.
        /// Ví dụ e trả về List SanPham thì e chỉ cần để là Listoutput<SanPhameModel>.
        /// Mục đích mình tạo class này để kế thừa các field trong class ApiOutput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public partial class ListOutput<T> : ApiOutput
        {
            public List<T> ListData { get; set; }
        }

        #endregion

        #region Methods

        public async static Task<T> GETAPI<T>(Uri uri) where T : ApiOutput
        {
            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<T>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return null;
                    }

                    return dataList;
                }

                return (T)Convert.ChangeType(null, typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }
        
        
        public async static Task<T> PostAPI<T>(Uri uri, object obj) where T : ApiOutput
        {
            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                //var stringContent = new StringContent(obj.ToString(), UnicodeEncoding.UTF8, "application/json");

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(uri, requestJson);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<T>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return (T)Convert.ChangeType(null, typeof(T));
                    }

                    return dataList;
                }

                return (T)Convert.ChangeType(null, typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(null, typeof(T));
            }
        }


      

        #endregion
    }
}
