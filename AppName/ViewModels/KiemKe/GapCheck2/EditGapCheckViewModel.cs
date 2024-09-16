using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using AppName.Model;
using System.Threading.Tasks;

namespace AppName
{
    public class EditGapCheckViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        public ICommand ConfirmCommand { get; set; }

        public EditGapCheckViewModel(INavigation navigation, StockCountModel model)
        {
            ConfirmCommand = new Command(ConfirmAsync);

            Navigation = navigation;

            DataModel = model;

            IsReadOnly = DataModel.Release == 1;
        }

        public partial class ResuftCountLineModel
        {
            public int code { get; set; }
            public string message { get; set; }

            public List<StockCountLineModel> data;
        }


        public partial class LSRetail_StockCountDetailListModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public List<StockCountLineModel> ListData { get; set; }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        //private async void LoadData(string documentNo)
        //{
        //    if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
        //    {
        //        return;
        //    }

        //    try
        //    {


        //        HttpClient client = new HttpClient();
        //        var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
        //        client.DefaultRequestHeaders.Authorization = authHeader;

        //        var userstore = Application.Current.Properties["UserStore"].ToString();

        //        Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/GetGapCheckLine?masterID=" + documentNo);

        //        HttpResponseMessage response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string content = await response.Content.ReadAsStringAsync();

        //            var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountDetailListModel>(content);

        //            if (dataList.Active == false)
        //            {
        //                Application.Current.Properties["IsLogin"] = false;

        //                Application.Current.MainPage = new NavigationPage(new LoginFrm());

        //                Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
        //            }

        //            if (dataList != null)
        //            {
        //                if (dataList.Code == 200)
        //                {
        //                    foreach (var item in dataList.ListData)
        //                    {
        //                        if (item.Quantity_Scan == item.Quantity)
        //                        {
        //                            item.ComparePCS = false;
        //                        }
        //                        else if (item.Quantity_Scan > item.Quantity)
        //                        {
        //                            item.ComparePCS = true;
        //                        }
        //                    };

        //                    ListStockLine = new ObservableCollection<StockCountLineModel>(dataList.ListData.OrderBy(s => s.ItemNo));
        //                }
        //                else
        //                {
        //                    var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
        //                    PopupNavigation.Instance.PushAsync(dialog);
        //                }
        //            }
        //            else
        //            {
        //                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is empty." };
        //                PopupNavigation.Instance.PushAsync(dialog);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
        //        PopupNavigation.Instance.PushAsync(dialog, false);
        //    }
        //}

        public partial class LSRetail_StockCountMasterModel
        {
            public string ID;
            public string Description;
            public string DocumentNo;
            public string StockType;
            public string LocationCode;
            public string Status;
            public string Management;
            public int IsSend;
            public int Release;
            public int IsDelete;
            public int QtyPack;
            public DateTime DateCreate;
            public string UserCreate;
        }

        public partial class LSRetail_StockCountMaster_ResuftModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public LSRetail_StockCountMasterModel ListData { get; set; }
        }

        private async void ConfirmAsync(object obj)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var model = new LSRetail_StockCountMasterModel();
                model.DocumentNo = DataModel.DocumentNo;
                model.Description = DataModel.Desciption;
                model.IsDelete = 2;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/stock/UpsertStockCount", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();


                    var dataList = JsonConvert.DeserializeObject<LSRetail_StockCountMaster_ResuftModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
                    }

                    if (dataList != null)
                    {
                        if (dataList.Code == 200)
                        {
                            var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = dataList.Content };
                            await PopupNavigation.Instance.PushAsync(dialog);

                            try
                            {
                                await Navigation.PopAsync();
                            }
                            catch (Exception)
                            {
                            }

                            MessagingCenter.Send<App>((App)Application.Current, "ReloadGapCheckPage");
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Data is empty." };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);
            }
            finally
            {
                ShowLoading = false;
            }
        }
    }
}
