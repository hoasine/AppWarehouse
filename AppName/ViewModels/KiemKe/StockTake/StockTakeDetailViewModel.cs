using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class StockTakeDetailViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        public ICommand ConfirmCommand { get; set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public StockTakeDetailViewModel(INavigation navigation, StockCountModel itemModel)
             : base(listenCultureChanges: true)
        {
            Navigation = navigation;
            ConfirmCommand = new Command(ConfirmAsync);
            DataModel = itemModel;
        }

        public partial class SendStockTakeModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public StockCountModel ListData { get; set; }
        }

        public partial class RealseDocumentScanModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
            public string Status { get; set; }
        }

        private async void ConfirmAsync(object obj)
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(DataModel.DocumentNo))
                {
                    var dialog = new NotificationWarrningPopup { Message = "DocumentNo is null" };
                    PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                if (DataModel.Release != 1)
                {
                    var dialog = new NotificationWarrningPopup { Message = "Status must be equal to 'Release' in " + DataModel.DocumentNo + "." };
                    PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                var usernameStore = Application.Current.Properties["UserStore"]?.ToString();
                if (DataModel.RetailStaff.ToUpper() != usernameStore.ToUpper())
                {
                    var dialog = new NotificationWarrningPopup { Message = "You do not have permission for this function. With " + DataModel.DocumentNo + "." };
                    PopupNavigation.Instance.PushAsync(dialog);
                    return;
                }

                LoadingPopup page1 = new LoadingPopup();
                PopupNavigation.Instance.PushAsync(page1);

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/SendStockTake", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(DataModel));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, requestJson);

                MessagingCenter.Send<App>((App)Application.Current, "Loading");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();


                    var dataList = JsonConvert.DeserializeObject<SendStockTakeModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        await Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "ReloadStockTakePage");

                        //await Application.Current.MainPage.DisplayAlert("Notification !", dataList.Content, "OK");

                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception)
                        {

                        }

                        var dialog = new NotificationPopup { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup { Message = dataList.Content };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog, false);

                MessagingCenter.Send<App>((App)Application.Current, "Loading");
            }
            finally
            {
            }
        }
    }
}
