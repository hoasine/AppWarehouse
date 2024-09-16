using AppName.Model;
using AppName.Model.Pickup;
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
    public class PickupLineViewModel : BaseViewModel
    {
        /// <summary>
        /// field giữ toàn bộ dữ liệu sẽ cập nhật
        /// </summary>

        private bool _IsScan;
        public bool IsScan
        {
            get { return _IsScan; }
            set { SetProperty(ref _IsScan, value); }
        }

        protected INavigation Navigation { get; private set; }

        private ObservableCollection<PickUpProductDetail> _PickupLine;
        public ObservableCollection<PickUpProductDetail> PickupLine
        {
            get { return _PickupLine; }
            set { SetProperty(ref _PickupLine, value); }
        }

        private PickUpProductMaster _PickUpProductMaster;
        public PickUpProductMaster PickupHeader
        {
            get { return _PickUpProductMaster; }
            set { SetProperty(ref _PickUpProductMaster, value); }
        }


        private PickUpProductCount _pickUpProductCount;
        public PickUpProductCount PickUpProductCountModel
        {
            get { return _pickUpProductCount; }
            set { SetProperty(ref _pickUpProductCount, value); }
        }

        public ICommand DeletePickupLineCommand { get; set; }
        public ICommand EditPickupLineCommand { get; set; }
        public ICommand SearchItemsCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public Command LoadItemsCommand { get; set; }

        public PickupLineViewModel(INavigation navigation, PickUpProductMaster itemModel)
        {
            IsScan = false;
            PickUpProductCountModel = new PickUpProductCount();
            Navigation = navigation;
            PickupHeader = itemModel;
            PickupLine = new ObservableCollection<PickUpProductDetail>();
            EditPickupLineCommand = new Command<PickUpProductDetail>(EditEditPickupLineAsync);
            DeletePickupLineCommand = new Command<PickUpProductDetail>(DeletePickupLineAsync);
            SearchItemsCommand = new Command<string>(ExecuteSearchItemsCommand);
            CloseCommand = new Command(CloseAsync);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public async void ExecuteSearchItemsCommand(string keyvalue)
        {
            if (keyvalue == "")
            {
                var dialog3 = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Input BarcodeCode to search." };
                PopupNavigation.Instance.PushAsync(dialog3);
            }
            else
            {
                CheckStockLine(keyvalue);
            }
        }

        public async void EditEditPickupLineAsync(PickUpProductDetail obj)
        {
            try
            {
                var dialog = new PickUPLineDetail();
                var viewModel = new PickUPLineDetailViewModel(obj);
                viewModel.IsUpdate = true;
                viewModel.IsCamera = false;
                viewModel.IsSource = true;
                viewModel.ClosePopup += async (send, data) =>
                {
                    try
                    {
                        try
                        {
                            await Navigation.PopModalAsync();
                        }
                        catch (Exception)
                        {
                        }

                        viewModel.ShowLoading = true;

                        await Task.Delay(1);

                        var currentData = new PickUpProductDetail()
                        {
                            MasterID = obj.MasterID,
                            BarcodeNo = obj.BarcodeNo,
                            ID = obj.ID,
                            ItemNo = obj.ItemNo,
                            DateCreate = obj.DateCreate,
                            ItemName = obj.ItemName,
                            Quantity = obj.Quantity,
                            QuantityScan = obj.QuantityScan,
                            IsDelete = 0,
                            ImageFile = data.ImageFile,
                            Note = data.Note,
                            Reason = data.Reason
                        };

                        var check = await UpsertOrDeleteTOLine(currentData);
                        if (check == true)
                        {
                            var temp = new List<PickUpProductDetail>(PickupLine);
                            temp.Remove(obj);
                            temp.Insert(0, data);

                            PickupLine.Clear();
                            PickupLine = new ObservableCollection<PickUpProductDetail>(temp);
                        }
                    }
                    catch (Exception ex)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                        PopupNavigation.Instance.PushAsync(dialog, false);

                        return;
                    }
                    finally
                    {
                        viewModel.ShowLoading = false;
                    }
                };

                dialog.BindingContext = viewModel;


                await Navigation.PushModalAsync(dialog);
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
        }


        async Task ExecuteLoadItemsCommand()
        {
            LoadPickupLine(PickupHeader);
        }

        public async void LoadCount()
        {
            if (PickupLine != null)
            {
                PickUpProductCountModel.TotalItem = PickupLine.GroupBy(s => s.ItemNo).Count();
                PickUpProductCountModel.TotalScan = PickupLine.Sum(s => s.QuantityScan);
            }
        }

        private async Task LoadPickupLine(PickUpProductMaster itemModel)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/GetPickupDetail?MasterID=" + itemModel.ID);

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listPickup = JsonConvert.DeserializeObject<List<PickUpProductDetail>>(content);

                    if (listPickup.Count() > 0)
                    {
                        PickupLine = new ObservableCollection<PickUpProductDetail>(listPickup);
                    }
                    else
                    {
                        PickupLine = new ObservableCollection<PickUpProductDetail>();
                    }

                    LoadCount();
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoadPickupLine Error: " + ex.Message.ToString() + ".", "Error");

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

        private async void DeletePickupLineAsync(PickUpProductDetail obj)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                if (PickupHeader.Status == "Release")
                {
                    Application.Current.MainPage.DisplayAlert("Notification !", "Status must be equal to 'Open' in Transfer Header.", "OK");
                }
                else
                {
                    obj.IsDelete = 1;

                    var check = UpsertOrDeleteTOLine(obj);

                    if (check.Result == true)
                    {
                        PickupLine.Remove(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DeletePickupLine Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog);
            }
        }

        public async Task<SanPhamViewModel> LoadProduct(string KeyValue)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            var listModel = new SanPhamViewModel();

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

                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftMasterFileModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return listModel;
                    }

                    if (dataList.ListData.Count() == 0)
                    {
                        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Không tìm thấy sản phẩm." };
                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                    else
                    {
                        listModel = dataList.ListData.FirstOrDefault();
                    }
                }

                return listModel;
            }
            catch (Exception ex)
            {
                logger.Error("LoadProduct Pickup Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog);

                return listModel;
            }
        }

        public async void CheckStockLine(string barcodeNo)
        {
            try
            {
                ShowLoading = true;

                await Task.Delay(1);

                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception) { }


                var tmpCurrentItem = new PickUpProductDetail();

                if (barcodeNo.Length == 6)
                {
                    tmpCurrentItem = PickupLine.Where(q => q.ItemNo == barcodeNo).FirstOrDefault();
                }
                else
                {
                    var getItemNo = PickupLine.Where(q => q.BarcodeNo == barcodeNo).FirstOrDefault();

                    if (getItemNo != null)
                    {
                        tmpCurrentItem = PickupLine.Where(q => q.ItemNo == getItemNo.ItemNo).FirstOrDefault();
                    }
                    else
                    {
                        tmpCurrentItem = null;
                    }
                }

                if (tmpCurrentItem != null)
                {
                    var quantityBanDau = tmpCurrentItem.QuantityScan;

                    var dialog = new PickUPLineDetail();
                    var viewModel = new PickUPLineDetailViewModel(tmpCurrentItem);
                    viewModel.IsUpdate = true;
                    viewModel.IsCamera = false;
                    viewModel.IsSource = true;
                    viewModel.ClosePopup += async (send, data) =>
                    {
                        try
                        {
                            try
                            {
                                await Navigation.PopModalAsync();
                            }
                            catch (Exception)
                            {
                            }

                            viewModel.ShowLoading = true;
                            viewModel.IsEnabledButton = false;

                            await Task.Delay(100);

                            var currentData = new PickUpProductDetail()
                            {
                                ID = Guid.NewGuid(),
                                MasterID = PickupHeader.ID,
                                BarcodeNo = tmpCurrentItem.BarcodeNo,
                                IsDelete = 0,
                                DateCreate = DateTime.Now,
                                ItemName = data.ItemName,
                                ItemNo = data.ItemNo,
                                QuantityScan = tmpCurrentItem.QuantityScan,
                                ImageFile = data.ImageFile,
                                Quantity = 0,
                                Note = data.Note,
                                Reason = data.Reason
                            };

                            var check = UpsertOrDeleteTOLine(currentData);
                            if (check.Result == true)
                            {
                                var temp = new List<PickUpProductDetail>(PickupLine);
                                temp.Remove(tmpCurrentItem);
                                temp.Insert(0, data);

                                PickupLine.Clear();
                                PickupLine = new ObservableCollection<PickUpProductDetail>(temp);
                            }
                            else
                            {
                                tmpCurrentItem.QuantityScan = quantityBanDau;
                            }
                        }
                        catch (Exception ex)
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                            PopupNavigation.Instance.PushAsync(dialog, false);

                            return;
                        }
                        finally
                        {
                            viewModel.IsEnabledButton = true;
                            viewModel.ShowLoading = false;
                        }
                    };

                    dialog.BindingContext = viewModel;

                    await Navigation.PushModalAsync(dialog);
                }
                else
                {
                    var userstore = Application.Current.Properties["UserStore"].ToString();
                    var producyModel = await LoadProduct(barcodeNo);

                    var CheckItemNoAgain = PickupLine.Where(q => q.ItemNo == producyModel.ItemNo).FirstOrDefault();

                    if (CheckItemNoAgain != null)
                    {
                        var quantityBanDau = CheckItemNoAgain.QuantityScan;

                        var dialog = new PickUPLineDetail();
                        var viewModel = new PickUPLineDetailViewModel(CheckItemNoAgain);
                        viewModel.IsUpdate = true;
                        viewModel.IsCamera = false;
                        viewModel.IsSource = true;
                        viewModel.ClosePopup += async (send, data) =>
                        {
                            try
                            {
                                try
                                {
                                    await Navigation.PopModalAsync();
                                }
                                catch (Exception)
                                {
                                }

                                viewModel.ShowLoading = true;
                                viewModel.IsEnabledButton = false;

                                await Task.Delay(100);

                                var currentData = new PickUpProductDetail()
                                {
                                    ID = Guid.NewGuid(),
                                    MasterID = PickupHeader.ID,
                                    BarcodeNo = tmpCurrentItem.BarcodeNo,
                                    IsDelete = 0,
                                    DateCreate = DateTime.Now,
                                    ItemName = data.ItemName,
                                    ItemNo = data.ItemNo,
                                    QuantityScan = tmpCurrentItem.QuantityScan,
                                    ImageFile = data.ImageFile,
                                    Quantity = 0,
                                    Note = data.Note,
                                    Reason = data.Reason
                                };

                                var check = UpsertOrDeleteTOLine(currentData);
                                if (check.Result == true)
                                {
                                    var temp = new List<PickUpProductDetail>(PickupLine);
                                    temp.Remove(CheckItemNoAgain);
                                    temp.Insert(0, data);

                                    PickupLine.Clear();
                                    PickupLine = new ObservableCollection<PickUpProductDetail>(temp);
                                }
                                else
                                {
                                    tmpCurrentItem.QuantityScan = quantityBanDau;
                                }
                            }
                            catch (Exception ex)
                            {

                                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                                PopupNavigation.Instance.PushAsync(dialog, false);

                                return;
                            }
                            finally
                            {
                                viewModel.IsEnabledButton = true;
                                viewModel.ShowLoading = false;
                            }
                        };

                        dialog.BindingContext = viewModel;

                        await Navigation.PushModalAsync(dialog);
                    }
                    else
                    {
                        try
                        {
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                            await PopupNavigation.Instance.PopAsync();
                        }
                        catch (Exception) { }

                        if (string.IsNullOrEmpty(producyModel.Barcode_No_))
                        {
                            var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Barcode:=" + barcodeNo + " not found product." };
                            PopupNavigation.Instance.PushAsync(dialog, false);
                        }
                        else
                        {
                            var tmpDataModel = new PickUpProductDetail()
                            {
                                ID = Guid.NewGuid(),
                                MasterID = PickupHeader.ID,
                                BarcodeNo = producyModel.Barcode_No_,
                                IsDelete = 0,
                                DateCreate = DateTime.Now,
                                ItemName = producyModel.ItemName,
                                ItemNo = producyModel.ItemNo,
                                QuantityScan = 0,
                                Quantity = 0
                            };

                            var dialog = new PickUPLineDetail();
                            var viewModel = new PickUPLineDetailViewModel(tmpDataModel);
                            viewModel.IsUpdate = true;
                            viewModel.IsCamera = true;
                            viewModel.IsSource = false;
                            viewModel.ClosePopup += async (send, data) =>
                            {
                                try
                                {
                                    await Navigation.PopModalAsync();
                                }
                                catch (Exception)
                                {
                                }

                                try
                                {
                                    viewModel.IsEnabledButton = false;
                                    viewModel.ShowLoading = true;

                                    await Task.Delay(100);

                                    var check = UpsertOrDeleteTOLine(data);

                                    if (check.Result == true)
                                    {
                                        var temp = new List<PickUpProductDetail>(PickupLine);
                                        temp.Insert(0, tmpDataModel);

                                        PickupLine.Clear();
                                        PickupLine = new ObservableCollection<PickUpProductDetail>(temp);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                                    PopupNavigation.Instance.PushAsync(dialog, false);

                                    return;
                                }
                                finally
                                {
                                    viewModel.IsEnabledButton = true;
                                    viewModel.ShowLoading = false;
                                }
                            };

                            dialog.BindingContext = viewModel;

                            await Navigation.PushModalAsync(dialog);
                        }
                    }
                }

                IsScan = false;
            }
            catch (Exception ex)
            {
                IsScan = false;

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);

                return;
            }
            finally
            {
                ShowLoading = false;
                IsScan = false;
            }
        }


        private async void CloseAsync()
        {
            MessagingCenter.Send<App>((App)Application.Current, "RefreshPagePickup");

            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception)
            {
            }
        }

        private async Task<bool> UpsertOrDeleteTOLine(PickUpProductDetail obj)
        {
            ILogger logger = DependencyService.Get<ILogManager>().GetLog();

            try
            {
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return false;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/UpsertDataPickUpDetail", string.Empty));

                var requestJson = new StringContent(JsonConvert.SerializeObject(obj));
                requestJson.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(uri, requestJson).Result;

                logger.Info("Upsert pickup Document:=" + obj.MasterID + " ItemNo:=" + obj.ItemNo + " Quantity:=" + obj.QuantityScan, "Info");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<ResuftPickUpModel>(content);

                    if (dataList.Active == false)
                    {
                        Application.Current.Properties["IsLogin"] = false;

                        Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        Application.Current.MainPage.DisplayAlert("Notification !", "The device has not been activated, Please contact the administrator to activate.", "OK");

                        return false;
                    }

                    if (dataList.code == 200)
                    {
                        return true;

                        LoadCount();
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Notification !", dataList.message, "OK");
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
                logger.Error("Upsert pickup Error: " + ex.Message.ToString() + ".", "Error");

                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                await PopupNavigation.Instance.PushAsync(dialog);

                return false;
            }
        }
    }
}
