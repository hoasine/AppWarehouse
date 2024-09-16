using AppName.Model;
using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class ReceiveTOLineViewModel : BaseViewModel
    {
        /// <summary>
        /// field giữ toàn bộ dữ liệu sẽ cập nhật
        /// </summary>

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        private TOHeaderPreviewModel tmpItemModel;
        protected INavigation Navigation { get; private set; }

        private ObservableCollection<TOLineModel> _listTOLine;
        public ObservableCollection<TOLineModel> ListTOLine
        {
            get { return _listTOLine; }
            set { SetProperty(ref _listTOLine, value); }
        }

        private TOHeaderModel _listTOHeader;
        public TOHeaderModel ListHeader
        {
            get { return _listTOHeader; }
            set { SetProperty(ref _listTOHeader, value); }
        }

        private bool _IsScan;
        public bool IsScan
        {
            get { return _IsScan; }
            set { SetProperty(ref _IsScan, value); }
        }
        public ICommand OpenShipmentTODetailCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand EditTOLineCommand { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ReceiveTOLineViewModel(INavigation navigation, TOHeaderModel itemModel)
        {
            IsScan = false;

            Navigation = navigation;
            ListHeader = itemModel;

            OpenShipmentTODetailCommand = new Command<string>(OpenShipmentTODetailAsync);
            NextCommand = new Command(NextAsync);
            EditTOLineCommand = new Command<TOLineModel>(EditReceiveTOLineAsync);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Unsubscribe<App>((App)Application.Current, "ClosePageTOPreview");

            MessagingCenter.Subscribe<App>((App)Application.Current, "ClosePageTOPreview", (sender) =>
            {
                ClosePage();
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            LoadShipmentTOLine(ListHeader);
        }

        private async void ClosePage()
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception)
            {
            }
        }


        async Task ExecuteLoadItemsCommand(TOHeaderModel itemModel)
        {
            LoadShipmentTOLine(itemModel);
        }

        //Shipment TO
        private async void LoadShipmentTOLine(TOHeaderModel itemModel)
        {
            try
            {
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                if (IsBusy)
                    return;
                IsBusy = true;

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                var store = Application.Current.Properties["UserName"].ToString();

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/GetTOTOLine?document=" + itemModel.DocumentNo);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listTOHeader = JsonConvert.DeserializeObject<ResuftTOLineModel>(content);

                    if (listTOHeader.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    foreach (var item in listTOHeader.ListData)
                    {
                        if (item.Quatity < item.Quantity_Scan)
                        {
                            item.ColorText = "#F70C0C";
                        }
                        else
                        {
                            item.ColorText = "#17AE34";
                        }

                        //if (string.IsNullOrEmpty(item.Image))
                        //{
                        //    item.Image = "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";
                        //}
                    }

                    ListTOLine = new ObservableCollection<TOLineModel>(listTOHeader.ListData);

                    var modelNhan = new TOHeaderPreviewModel()
                    {
                        DocumentNo = itemModel.DocumentNo,
                        VietnameseDescription = itemModel.VietnameseDescription,
                        Status = itemModel.Status,
                        TransferfromCode = itemModel.TransferfromCode,
                        TransfertoCode = itemModel.TransfertoCode,
                        BuyerID = itemModel.BuyerID,
                        Quantity = itemModel.Quantity,
                        PostingDate = itemModel.PostingDate,
                        TOLine = listTOHeader.ListData
                    };

                    tmpItemModel = modelNhan;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public partial class UpsertOrDeleteTOLineModel
        {
            public bool Active { get; set; }
            public int Code { get; set; }
            public string Content { get; set; }
        }

        public async Task<bool> UpsertQuantityScan(TOLineModel obj)
        {
            try
            {
                ILogger logger = DependencyService.Get<ILogManager>().GetLog();

                logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=Upsert Json:=" + obj.DocumentNo + " | " + obj.ItemNo + " | " + obj.Quantity_Scan + ".", "Info");

                var model = new TOLineModel();
                model.DocumentNo = obj.DocumentNo;
                model.IsDelete = obj.IsDelete;
                model.ItemNo = obj.ItemNo;
                model.ItemName = obj.ItemName;
                model.ColorText = obj.ColorText;
                model.Image = "";
                model.Quatity = obj.Quatity;
                model.QtytoShip = obj.QtytoShip;
                model.Quantity_Scan = obj.Quantity_Scan;

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return false;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/topos/CheckScanReceive", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<UpsertOrDeleteTOLineModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return false;
                    }

                    if (dataList.Code == 200)
                    {
                        logger.Info("Class:=" + this.GetType().Name.Replace("ViewModel", "") + " Action:=" + CheckConnectInternet.GetmethodName() + "Content:=Successfully.", "Info");

                        return true;
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = dataList.Content };
                        PopupNavigation.Instance.PushAsync(dialog);

                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return false;
            }
        }

        public async void OpenShipmentTODetailAsync(string obj)
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                var currentData = ListTOLine.FirstOrDefault(q => q.ItemNo == obj);

                if (currentData == null)
                {
                    var model = LoadProduct(obj);

                    if (model.ItemNo != null)
                    {
                        var checkAgain = ListTOLine.FirstOrDefault(q => q.ItemNo == model.ItemNo);

                        if (checkAgain != null)
                        {
                            currentData = checkAgain;
                        }
                        else
                        {
                            Application.Current.MainPage.DisplayAlert("Notification !", "Not found Item:" + obj + " in TO:=" + ListHeader.DocumentNo, "OK");
                        }
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Notification !", "Not found item:" + obj + " in TO:=" + ListHeader.DocumentNo, "OK");
                    }
                }

                if (currentData != null)
                {
                    currentData.Quantity_Scan++;

                    var statusCheck = UpsertQuantityScan(currentData);

                    if (statusCheck.Result == true)
                    {
                        try
                        {
                            PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception)
                        {

                        }

                        var dialog = new NotificationPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) {  Message = "ItemNo:=" + currentData.ItemNo + " added." };
                        PopupNavigation.Instance.PushAsync(dialog, false);

                        if (currentData.Quatity < currentData.Quantity_Scan)
                        {
                            currentData.ColorText = "#F70C0C";
                        }
                        else
                        {
                            currentData.ColorText = "#17AE34";
                        }

                        //Upsert ListLine
                        var temp = new List<TOLineModel>(ListTOLine);
                        temp.Remove(currentData);
                        temp.Insert(0, currentData);

                        ListTOLine.Clear();
                        ListTOLine = new ObservableCollection<TOLineModel>(temp);
                    }
                }

                IsScan = false;
            }
            catch (Exception ex)
            {
                IsScan = false;

                IsBusy = false;
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
            finally
            {
                IsScan = false;

                IsBusy = false;
            }
        }

        public TOLineModel LoadProduct(string KeyValue)
        {
            var listModel = new TOLineModel();

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return listModel;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoNoImage?KeyValue=" + KeyValue, string.Empty));

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var dataList = JsonConvert.DeserializeObject<ResuftMasterFileModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return listModel;
                    }

                    foreach (var data in dataList.ListData)
                    {
                        var modelData = new TOLineModel()
                        {
                            ItemName = data.ItemName + " " + data.ItemNo,
                            ItemNo = data.ItemNo,
                            Image = data.Image,
                            QtytoShip = 0,
                            Quatity = 0
                        };

                        //if (string.IsNullOrEmpty(modelData.Image))
                        //{
                        //    modelData.Image = "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";
                        //}

                        listModel = modelData;
                    }
                }

                return listModel;
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return listModel;
            }
        }

        private async void NextAsync(object obj)
        {
            tmpItemModel.TOLine = ListTOLine.ToList();

            await Navigation.PushAsync(new ReceiveTOPreviewPage(tmpItemModel));
        }

        private void EditReceiveTOLineAsync(TOLineModel obj)
        {
            try
            {
                var dialog = new ReceiveTOLineDetail();
                var viewModel = new ReceiveTOLineDetailViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        viewModel.ShowLoading = true;

                        await Task.Delay(100);

                        obj.IsDelete = false;
                        var check = UpsertQuantityScan(obj);
                        if (check.Result == true)
                        {
                            if (data.Quatity < data.Quantity_Scan)
                            {
                                data.ColorText = "#F70C0C";
                            }
                            else
                            {
                                data.ColorText = "#17AE34";
                            }

                            ListTOLine.Remove(obj);
                            ListTOLine.Insert(0, data);
                        }

                    }
                    catch (Exception exx)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
                    }
                    finally
                    {
                        viewModel.ShowLoading = false;
                    }
                };

                dialog.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialog);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }

        }
    }
}
