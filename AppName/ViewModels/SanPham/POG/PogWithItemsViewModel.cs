using System;
using Xamarin.Forms;
using AppName.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using AppName.ViewModels.BarCode.Model;
using AppName.Model;
using AppName.Services;
using System.Diagnostics;
using System.Windows.Input;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;

namespace AppName
{
    public class PogWithItemsViewModel : BaseViewModel
    {
        private ObservableCollection<POGWithItemModel> _items;
        public ICommand CommandShowImage { get; set; }

        public ObservableCollection<POGWithItemModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public Command RefreshCommand { get; set; }
        private bool _isHasData;
        private bool _isMess;

        public bool IsMess
        {
            get => _isMess;

            set => SetProperty(ref _isMess, value);
        }

        public bool IsHasData
        {
            get => _isHasData;

            set => SetProperty(ref _isHasData, value);
        }

        protected INavigation Navigation { get; private set; }


        public PogWithItemsViewModel(INavigation navigation)
        {
            Navigation = navigation;

            RefreshCommand = new Command(() => LoadData());
            CommandShowImage = new Command<POGWithItemModel>(ShowImage);

            Items = new ObservableCollection<POGWithItemModel>();

            LoadData();
        }

        private async void ShowImage(POGWithItemModel obj)
        {
            if (obj == null)
            {
                return;
            }

            var dialog = new PogShowImage();
            dialog.BindingContext = new PogShowImageModel(obj);

            PopupNavigation.Instance.PushAsync(dialog);
        }

        protected async void LoadData()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = authHeader;

                var model = new
                {
                    LocationCode = ""
                };

                var stringContent = new StringContent(model.ToString(), UnicodeEncoding.UTF8, "application/json");

                var url = "";
                var userName = Application.Current.Properties["UserName"]?.ToString();
                url = RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/GetPOGWithItem?LocationCode=" + userName;

                Uri uri = new Uri(string.Format(url, string.Empty));

                HttpResponseMessage response = await client.PostAsync(uri, stringContent);

                var mess = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftPOGWithModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    var listModel = new List<POGWithItemModel>();

                    Items.Clear();

                    if (dataList.ListData.data.Count() > 0)
                    {
                        foreach (var item in dataList.ListData.data)
                        {
                            if (item.POG == "MASK1")
                            {
                                item.Image = "POG01.jpg";
                            }
                            else if (item.POG == "MASK2")
                            {
                                item.Image = "POG02.jpg";
                            }
                            else
                            {
                                item.Image = "POG03.jpg";
                            }
                        }

                        var group = dataList.ListData.data.ToList();

                        Items = new ObservableCollection<POGWithItemModel>(group);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "There is nothing to show in this" };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public partial class ResuftPOGWithModel
        {
            public bool Active { get; set; }
            public ResuftGetPOGWithModel ListData { get; set; }
        }


        public partial class ResuftGetPOGWithModel
        {
            public int code { get; set; }
            public bool success { get; set; }
            public string message { get; set; }

            public List<POGWithItemModel> data;
        }

        public partial class POGWithItemModel
        {
            public string ItemNo { get; set; }
            public string FixelID { get; set; }
            public string POG { get; set; }
            public int TotalFacing { get; set; }
            public int TotalUnit { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }
        }


        //public class POGWithItemGroupModel : List<POGWithItemModel>
        //{
        //    public string FixelIDGroup { get; set; }
        //    public List<POGWithItemModel> Data => this;
        //}
    }
}
