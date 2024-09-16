using AppName.Model.StockTake;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
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
using static AppName.ViewModels.APIBaseViewModel;

namespace AppName
{
    public class StockTakePreviewViewModel : ObservableObject
    {
        protected INavigation Navigation { get; private set; }

        private StockCountModel _dataModel;
        public StockCountModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        private ObservableCollection<StockCountLineModel> _listStockLine;
        public ObservableCollection<StockCountLineModel> ListStockLine
        {
            get { return _listStockLine; }
            set { SetProperty(ref _listStockLine, value); }
        }

        private string _Release;
        public string Release
        {
            get { return _Release; }
            set { SetProperty(ref _Release, value); }
        }

        private bool _IsStaff;
        public bool IsStaff
        {
            get { return _IsStaff; }
            set { SetProperty(ref _IsStaff, value); }
        }

        public ICommand ReleaseCommand { get; set; }
        public ICommand EditStockTakeLineCommand { get; set; }
        public ICommand DeleteStockTakeLineCommand { get; set; }
        public ICommand ChangeZoneCommand { get; set; }

        private ObservableCollection<ZoneModel> _listZone;
        public ObservableCollection<ZoneModel> ListZone
        {
            get { return _listZone; }
            set { SetProperty(ref _listZone, value); }
        }

        private ZoneModel _selectedZone;
        public ZoneModel SelectedZone
        {
            get { return _selectedZone; }
            set { SetProperty(ref _selectedZone, value); }
        }

        public partial class ZoneModel
        {
            public string ZoneName { get; set; }
            public int Number { get; set; }
        }

        public StockTakePreviewViewModel(INavigation navigation, StockCountModel model)
        {
            try
            {
                ReleaseCommand = new Command(ReleaseAsync);

                Navigation = navigation;

                DataModel = model;

                if (DataModel.Release == 0)
                {
                    Release = "Release";
                }
                else if (DataModel.Release == 1)
                {
                    Release = "ReOpen";
                }
                else if (DataModel.Release == 3)
                {
                    Release = "UNKNOWN";
                }

                var userName = Application.Current.Properties["UserStore"]?.ToString();
                if (model.RetailStaff.ToUpper() == userName.ToUpper())
                {
                    IsStaff = true;
                }
                else
                {
                    IsStaff = false;
                }

                EditStockTakeLineCommand = new Command<StockCountLineModel>(EditShipmentTOLineAsync);
                DeleteStockTakeLineCommand = new Command<StockCountLineModel>(DeleteShipmentTOLineAsync);
                ChangeZoneCommand = new Command(ChangeZoneAsync);
                SelectedZone = new ZoneModel();

                MessagingCenter.Unsubscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic");

                MessagingCenter.Subscribe<App, string>((App)Xamarin.Forms.Application.Current, "BarcodeDatalogic", async (sender, thebarcode) =>
                {
                    CheckStockLine(thebarcode.Substring(0, thebarcode.Length - 1));
                });


                if (DataModel.QtyPack > 0)
                {
                    var listZone = new List<ZoneModel>();

                    for (int i = 1; i < DataModel.QtyPack; i++)
                    {
                        listZone.Add(new ZoneModel()
                        {
                            ZoneName = "Zone" + i,
                            Number = i
                        });
                    }

                    if (listZone.Count() > 0)
                    {
                        ListZone = new ObservableCollection<ZoneModel>(listZone);

                        SelectedZone = ListZone.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public void ChangeZoneAsync()
        {
            LoadDataLines(DataModel.DocumentNo, SelectedZone.Number);
        }

        public void EditShipmentTOLineAsync(StockCountLineModel obj)
        {
            if (DataModel.Release != 0)
            {
                var dialog3 = new NotificationWarrningPopup { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                PopupNavigation.Instance.PushAsync(dialog3);
                return;
            }

            try
            {
                var dialog = new StockTakeLineDetail();
                var viewModel = new StockTakeLineDetailViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.ClosePopup += (send, data) =>
                {
                    var currentData = new DAFC_StockCountDetailModel()
                    {
                        DocumentNo = obj.DocumentNo,
                        Zone = SelectedZone.Number,
                        BarcodeNo = obj.BarcodeNo,
                        ID = obj.ID,
                        DateCreate = obj.DateCreate,
                        ItemNo = obj.ItemNo,
                        Quantity = obj.Quantity,
                        Quantity_Scan = obj.Quantity_Scan,
                        UserCreate = obj.UserCreate,
                        IsDelete = 0
                    };

                    var check = UpsertQuantityScan(currentData);
                    if (check.Result == true)
                    {
                        var temp = new List<StockCountLineModel>(ListStockLine);
                        temp.Remove(obj);
                        temp.Insert(0, data);

                        ListStockLine.Clear();
                        ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                        DataModel.CountItem = ListStockLine.Count();
                        DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                        DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                        DataModel.ListData = new List<StockCountLineModel>(ListStockLine);
                    }
                };

                dialog.BindingContext = viewModel;

                PopupNavigation.Instance.PushAsync(dialog);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public void DeleteShipmentTOLineAsync(StockCountLineModel obj)
        {
            if (DataModel.Release != 0)
            {
                var dialog3 = new NotificationWarrningPopup { Message = "Status must be equal to 'Open' in " + obj.DocumentNo + "." };
                PopupNavigation.Instance.PushAsync(dialog3);
                return;
            }

            try
            {
                var currentData = new DAFC_StockCountDetailModel()
                {
                    DocumentNo = obj.DocumentNo,
                    Zone = SelectedZone.Number,
                    BarcodeNo = obj.BarcodeNo,
                    ID = obj.ID,
                    DateCreate = obj.DateCreate,
                    ItemNo = obj.ItemNo,
                    Quantity = obj.Quantity,
                    Quantity_Scan = obj.Quantity_Scan,
                    UserCreate = obj.UserCreate,
                    IsDelete = 1
                };

                var check = UpsertQuantityScan(currentData);
                if (check.Result == true)
                {
                    var temp = new List<StockCountLineModel>(ListStockLine);
                    temp.Remove(obj);

                    ListStockLine.Clear();
                    ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                    DataModel.CountItem = ListStockLine.Count();
                    DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                    DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                    DataModel.ListData = new List<StockCountLineModel>(ListStockLine);

                    try
                    {
                        PopupNavigation.Instance.PopAsync();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public async Task<bool> UpsertQuantityScan(DAFC_StockCountDetailModel obj)
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return false;
            }

            try
            {

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/UpsertOrDeleteStockLine", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<UpsertOrDeletePOLineModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return false;
                    }

                    if (dataList.Code == 200)
                    {
                        return true;
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup { Message = dataList.Content };
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
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return false;
            }
        }

        private async void ReleaseAsync(object obj)
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/RealseDocumentScan?DocumentNo=" + DataModel.DocumentNo, string.Empty));

                var response = client.GetAsync(uri).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<RealseDocumentScanModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return;
                    }

                    if (dataList.Code == 200)
                    {

                        if (dataList.Status == "Open")
                        {
                            Release = "Realse";
                            DataModel.Release = 0;

                            var dialog = new NotificationPopup { Message = "Changed status Open successfully." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "Release")
                        {
                            Release = "ReOpen";
                            DataModel.Release = 1;

                            var dialog = new NotificationPopup { Message = "Changed status Release successfully." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "Sent")
                        {
                            var dialog = new NotificationWarrningPopup { Message = DataModel.DocumentNo + " has been sent so can't reopen." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else if (dataList.Status == "NoLines")
                        {
                            var dialog = new NotificationWarrningPopup { Message = "Stock Take lines no data to report " + DataModel.DocumentNo + "." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                        else
                        {
                            Release = "UNKNOWN";

                            var dialog = new NotificationWarrningPopup { Message = "Changed status UNKNOWN." };
                            PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup { Message = dataList.Content };
                        PopupNavigation.Instance.PushAsync(dialog);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async void LoadDataLines(string documentNo, int Zone)
        {
            try
            {
                if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                LoadingPopup page1 = new LoadingPopup();
                await PopupNavigation.Instance.PushAsync(page1);

                HttpClient client = new HttpClient();
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                client.DefaultRequestHeaders.Authorization = authHeader;

                var userstore = Application.Current.Properties["UserStore"].ToString();

                Uri uri = new Uri("http://10.46.12.133:2525/api/stock/GetStockTakeLine?masterID=" + documentNo + "&zone=" + Zone);

                HttpResponseMessage response = await client.GetAsync(uri);

                MessagingCenter.Send<App>((App)Application.Current, "Loading");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<DAFC_StockCountDetailListModel>(content);

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
                            if (dataList.ListData.Count() > 0)
                            {
                                foreach (var item in dataList.ListData)
                                {
                                    if (item.Quantity_Scan == item.Quantity)
                                    {
                                        item.ComparePCS = false;
                                    }
                                    else if (item.Quantity_Scan > item.Quantity)
                                    {
                                        item.ComparePCS = true;
                                    }
                                };

                                DataModel.CountItem = dataList.ListData.Count();
                                DataModel.SumQuantityLine = dataList.ListData.Sum(q => q.Quantity);
                                DataModel.SumQuantity_Scan = dataList.ListData.Sum(q => q.Quantity_Scan);

                                ListStockLine = new ObservableCollection<StockCountLineModel>(dataList.ListData.OrderBy(s => s.ItemNo));
                            }
                            else
                            {
                                DataModel.CountItem = 0;
                                DataModel.SumQuantityLine = 0;
                                DataModel.SumQuantity_Scan = 0;

                                ListStockLine = new ObservableCollection<StockCountLineModel>();
                            }
                        }
                        else
                        {
                            var dialog = new NotificationErrorPopup { Message = dataList.Content };
                            await PopupNavigation.Instance.PushAsync(dialog);
                        }
                    }
                    else
                    {
                        var dialog = new NotificationErrorPopup { Message = "Data is empty." };
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

        private async void CheckStockLine(string barcodeNo)
        {
            if (DataModel.Release != 0)
            {
                var dialog3 = new NotificationWarrningPopup { Message = "Status must be equal to 'Open' in " + DataModel.DocumentNo + "." };
                PopupNavigation.Instance.PushAsync(dialog3);
                return;
            }

            try
            {
                if (DataModel.Release == 0)
                {
                    var tmpCurrentItem = ListStockLine.FirstOrDefault(q => q.BarcodeNo == barcodeNo);
                    if (tmpCurrentItem != null)
                    {
                        tmpCurrentItem.Quantity_Scan = tmpCurrentItem.Quantity_Scan + 1;

                        if (tmpCurrentItem.Quantity_Scan == tmpCurrentItem.Quantity)
                        {
                            tmpCurrentItem.ComparePCS = false;
                        }
                        else if (tmpCurrentItem.Quantity_Scan > tmpCurrentItem.Quantity)
                        {
                            tmpCurrentItem.ComparePCS = true;
                        }

                        var currentData = new DAFC_StockCountDetailModel()
                        {
                            DocumentNo = tmpCurrentItem.DocumentNo,
                            Zone = SelectedZone.Number,
                            POG = "",
                            BarcodeNo = tmpCurrentItem.BarcodeNo,
                            ID = tmpCurrentItem.ID,
                            DateCreate = tmpCurrentItem.DateCreate,
                            ItemNo = tmpCurrentItem.ItemNo,
                            Quantity = tmpCurrentItem.Quantity,
                            Quantity_Scan = tmpCurrentItem.Quantity_Scan,
                            UserCreate = tmpCurrentItem.UserCreate,
                            IsDelete = 0
                        };

                        var statusCheck = UpsertQuantityScan(currentData);

                        if (statusCheck.Result == true)
                        {
                            try
                            {
                                PopupNavigation.Instance.PopAsync();
                            }
                            catch (Exception) { }

                            var temp = new List<StockCountLineModel>(ListStockLine);
                            temp.Remove(tmpCurrentItem);
                            temp.Insert(0, tmpCurrentItem);

                            ListStockLine.Clear();
                            ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                            DataModel.CountItem = ListStockLine.Count();
                            DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                            DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);

                            var dialog = new NotificationPopup { Message = "Barcode:=" + barcodeNo + " added." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                    }
                    else
                    {
                        var userstore = Application.Current.Properties["UserStore"].ToString();
                        var producyModel = LoadProduct(barcodeNo);

                        var tmpDataModel = new StockCountLineModel
                        {
                            DocumentNo = DataModel.DocumentNo,
                            ItemName = producyModel.ItemName,
                            Zone = SelectedZone.Number,
                            BarcodeNo = producyModel.BarcodeNo,
                            ID = Guid.NewGuid().ToString(),
                            DateCreate = DateTime.Now,
                            POG = "",
                            IsDelete = 0,
                            ItemNo = producyModel.ItemNo,
                            Quantity = producyModel.Stock,
                            Quantity_Scan = 1,
                            UserCreate = userstore
                        };

                        var temp = new List<StockCountLineModel>(ListStockLine);
                        temp.Insert(0, tmpDataModel);

                        ListStockLine.Clear();
                        ListStockLine = new ObservableCollection<StockCountLineModel>(temp);


                        HttpClient client = new HttpClient();
                        var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));
                        client.DefaultRequestHeaders.Authorization = authHeader;

                        var model = new DAFC_StockCountMasterModel();
                        model.DocumentNo = DataModel.DocumentNo;
                        model.Description = DataModel.Desciption;
                        model.IsDelete = 3;
                        model.Lines = new List<DAFC_StockCountDetailModel>()
                {
                    new DAFC_StockCountDetailModel()
                    {
                        DocumentNo = tmpDataModel.DocumentNo,
                        Zone = SelectedZone.Number,
                        POG = "",
                        ItemName = tmpDataModel.ItemName,
                        BarcodeNo = tmpDataModel.BarcodeNo,
                        ID = tmpDataModel.ID,
                        DateCreate = tmpDataModel.DateCreate,
                        IsDelete = tmpDataModel.IsDelete,
                        ItemNo = tmpDataModel.ItemNo,
                        Quantity = tmpDataModel.Quantity,
                        Quantity_Scan = tmpDataModel.Quantity_Scan,
                        UserCreate = tmpDataModel.UserCreate
                    }
                };

                        Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/UpsertStockCount", string.Empty));

                        var requestJson = new StringContent(JsonConvert.SerializeObject(model));
                        requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = client.PostAsync(uri, requestJson).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            var dataList = JsonConvert.DeserializeObject<DAFC_StockCountMaster_ResuftModel>(content);

                            if (dataList.Active == false)
                            {
                                Application.Current.Properties["IsLogin"] = false;

                                Application.Current.MainPage = new NavigationPage(new LoginFrm());

                                Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");
                            }

                            if (dataList != null)
                            {
                                if (dataList.Code == 200)
                                {
                                    DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                                    DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);

                                    try
                                    {
                                        PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception) { }

                                    var dialog = new NotificationPopup { Message = "Barcode:=" + barcodeNo + " added." };
                                    PopupNavigation.Instance.PushAsync(dialog, false);
                                }
                                else
                                {
                                    try
                                    {
                                        PopupNavigation.Instance.PopAsync();
                                    }
                                    catch (Exception) { }

                                    var dialog = new NotificationWarrningPopup { Message = dataList.Content };
                                    PopupNavigation.Instance.PushAsync(dialog);
                                }
                            }
                            else
                            {
                                var dialog = new NotificationErrorPopup { Message = "Data is empty." };
                                PopupNavigation.Instance.PushAsync(dialog);
                            }
                        }
                    }
                }
                else
                {
                    var tmpCurrentItem = ListStockLine.FirstOrDefault(q => q.BarcodeNo == barcodeNo);
                    if (tmpCurrentItem != null)
                    {
                        var temp = new List<StockCountLineModel>(ListStockLine);
                        temp.Remove(tmpCurrentItem);
                        temp.Insert(0, tmpCurrentItem);

                        ListStockLine.Clear();
                        ListStockLine = new ObservableCollection<StockCountLineModel>(temp);

                        DataModel.CountItem = ListStockLine.Count();
                        DataModel.SumQuantityLine = ListStockLine.Sum(q => q.Quantity);
                        DataModel.SumQuantity_Scan = ListStockLine.Sum(q => q.Quantity_Scan);
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup { Message = "Not found :=" + barcodeNo + "." };
                        PopupNavigation.Instance.PushAsync(dialog, false);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        public ProductInfoStockModel LoadProduct(string KeyValue)
        {
            var listModel = new ProductInfoStockModel();

            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return listModel;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/ProductInfoStock?KeyValue=" + KeyValue + "&store=" + DataModel.LocationCode, string.Empty));

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var dataList = JsonConvert.DeserializeObject<ProductInfoStockResutModel>(content);
                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return listModel;
                    }

                    return dataList.ListData.First();
                }

                return listModel;
            }
            catch (Exception ex)
            {
                return listModel;
            }
        }

        public async Task<bool> UpsertQuantityScan(POLineModel obj)
        {
            if (CustomRenderer.CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return false;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format("http://10.46.12.133:2525/api/stock/UpsertOrDeleteStockLine", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<UpsertOrDeletePOLineModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return false;
                    }

                    if (dataList.Code == 200)
                    {
                        return true;
                    }
                    else
                    {
                        var dialog = new NotificationWarrningPopup { Message = dataList.Content };
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
                var dialog = new NotificationWarrningPopup { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return false;
            }
        }
    }
}
