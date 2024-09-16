using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using AppName.Model;

namespace AppName
{
    public class TaoPhieuNhapViewModel : ObservableObject
    {
        #region Properties

        protected INavigation Navigation { get; private set; }
        protected Page Page { get; private set; }

        private bool _checkedStep1 = true;
        /// <summary>
        /// Status Step 1
        /// </summary>
        public bool CheckedStep1
        {
            get { return _checkedStep1; }
            set { SetProperty(ref _checkedStep1, value); }
        }

        private bool _checkedStep2;
        /// <summary>
        /// Status Step 2
        /// </summary>
        public bool CheckedStep2
        {
            get { return _checkedStep2; }
            set { SetProperty(ref _checkedStep2, value); }
        }

        private bool _checkedStep3;
        /// <summary>
        /// Status Step 3
        /// </summary>
        public bool CheckedStep3
        {
            get { return _checkedStep3; }
            set { SetProperty(ref _checkedStep3, value); }
        }

        private bool _visibleStep1 = true;
        /// <summary>
        /// View Step 1
        /// </summary>
        public bool VisibleStep1
        {
            get { return _visibleStep1; }
            set { SetProperty(ref _visibleStep1, value); }
        }

        private bool _visibleStep2;
        /// <summary>
        /// View Step 2
        /// </summary>
        public bool VisibleStep2
        {
            get { return _visibleStep2; }
            set { SetProperty(ref _visibleStep2, value); }
        }

        private bool _visibleStep3;
        /// <summary>
        /// View Step 3
        /// </summary>
        public bool VisibleStep3
        {
            get { return _visibleStep3; }
            set { SetProperty(ref _visibleStep3, value); }
        }

        private bool _visibleNotification;
        /// <summary>
        /// Ẩn/Hiện View thông báo sau khi xác nhận
        /// </summary>
        public bool VisibleNotification
        {
            get { return _visibleNotification; }
            set { SetProperty(ref _visibleNotification, value); }
        }

        private ObservableCollection<SanPhamModel> _items;
        public ObservableCollection<SanPhamModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private ObservableCollection<LocationModel> _listLocation;
        public ObservableCollection<LocationModel> ListLocation
        {
            get { return _listLocation; }
            set { SetProperty(ref _listLocation, value); }
        }

        public ICommand NextStep2Command { get; set; }
        public ICommand NextStep3Command { get; set; }
        public ICommand TapStepCommand { get; set; }
        public ICommand OpenSanPhamDetailCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ICommand EditSanPhamCommand { get; set; }
        public ICommand DeleteSanPhamCommand { get; set; }

        #endregion

        #region Constructors

        public TaoPhieuNhapViewModel(INavigation navigation, Page page)
        {
            Navigation = navigation;
            Page = page;

            Items = new ObservableCollection<SanPhamModel>();

            NextStep2Command = new Command(() =>
            {
                VisibleStep1 = VisibleStep3 = false;
                VisibleStep2 = CheckedStep2 = true;
            });

            NextStep3Command = new Command(() =>
            {
                VisibleStep1 = VisibleStep2 = false;
                VisibleStep3 = CheckedStep3 = true;
            });

            // stepIndex chỉ bằng 1 hoặc 2 hoặc 3
            TapStepCommand = new Command<object>(stepIndex =>
            {
                if (stepIndex.Equals("1"))
                {
                    VisibleStep1 = CheckedStep1 = true;

                    CheckedStep2 = CheckedStep3 = VisibleStep2 = VisibleStep3 = false;
                }
                else if (stepIndex.Equals("2"))
                {
                    CheckedStep1 = VisibleStep2 = CheckedStep2 = true;

                    VisibleStep1 = CheckedStep3 = VisibleStep3 = false;
                }
                else
                {
                    CheckedStep1 = CheckedStep2 = VisibleStep3 = CheckedStep3 = true;

                    VisibleStep1 = VisibleStep2 = false;
                }
            });

            OpenSanPhamDetailCommand = new Command<string>(OpenSanPhamDetailAsync);

            ConfirmCommand = new Command(ConfirmAsync);
            CloseCommand = new Command(CloseAsync);

            EditSanPhamCommand = new Command<SanPhamModel>(EditSanPhamAsync);
            DeleteSanPhamCommand = new Command<SanPhamModel>(DeleteSanPhamAsync);

            LoadLocationData();
        }

        #endregion

        #region Operators

        private async void CloseAsync(object obj)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception) { }
        }

        private async void ConfirmAsync(object obj)
        {
            // Delay thời gian tượng trung cho call Api
            await Task.Delay(3000);

            VisibleStep1 = VisibleStep2 = VisibleStep3 = false;

            VisibleNotification = true;
        }

        private async void OpenSanPhamDetailAsync(string obj)
        {
            try
            {
                if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
                {
                    return;
                }

                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/products/ProductInfoNoImage?KeyValue=" + obj + "&pageSize=1", string.Empty));

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var dataList = JsonConvert.DeserializeObject<List<SanPhamViewModel>>(content);

                    var model = dataList.FirstOrDefault();

                    if (model != null)
                    {
                        var modelData = Items.FirstOrDefault(q => q.ItemNo == model.ItemNo);

                        if (modelData == null)
                        {
                            modelData = new SanPhamModel()
                            {
                                Barcode_No_ = model.Barcode_No_,
                                ItemName = model.ItemName + " " + model.ItemNo,
                                ItemNo = model.ItemNo,
                                UnitPrice = model.Unit_Price.Value.ToString("#,##"),
                                URLImage = "",
                            };

                            string[] numbers = new[]
                            {
                                  ""
                            };

                            Random rand = new Random();
                            int index = rand.Next(numbers.Length);

                            modelData.URLImage = numbers[index];

                            if (string.IsNullOrEmpty(modelData.URLImage))
                            {
                                modelData.URLImage = "imageempty.png";
                            }
                        }

                        var dialog = new SanPhamDetail();
                        var viewModel = new BarCodeSanPhamNotGetDataViewModel(modelData);
                        viewModel.ClosePopup += (send, barCode) =>
                        {
                            try
                            {
                                var index = Items.IndexOf(modelData);

                                if (index == -1)
                                {
                                    Items.Add(barCode);
                                }
                                else
                                {
                                    Items.RemoveAt(index);
                                    Items.Insert(index, barCode);
                                }
                            }
                            catch (Exception exx)
                            {
                                var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                                PopupNavigation.Instance.PushAsync(dialog);
                            }
                        };
                        dialog.BindingContext = viewModel;

                        await PopupNavigation.Instance.PushAsync(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private void EditSanPhamAsync(SanPhamModel obj)
        {
            try
            {
                var dialog = new SanPhamDetail();
                var viewModel = new BarCodeSanPhamNotGetDataViewModel(obj);
                viewModel.IsEditData = true;
                viewModel.ClosePopup += (send, barCode) =>
                {
                    try
                    {
                        var index = Items.IndexOf(obj);

                        Items.RemoveAt(index);
                        Items.Insert(index, barCode);

                    }
                    catch (Exception exx)
                    {
                        var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = exx.Message.ToString() };
                        PopupNavigation.Instance.PushAsync(dialog);
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

        private async void DeleteSanPhamAsync(SanPhamModel obj)
        {
            var result = await Page.DisplayAlert("Thông báo", "Bạn có muốn xóa dòng sản phẩm này?", "Có", "Không");

            if (result)
            {
                Items.Remove(obj);
            }
        }

        private async void LoadLocationData()
        {
            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri("https://lsretail_api.dafc.com.vn/api/products/GetLocation");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listLocation = JsonConvert.DeserializeObject<LocationModel[]>(content);

                    ListLocation = new ObservableCollection<LocationModel>(listLocation);
                }
                else
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "response.IsSuccessStatusCode = False" };
                    PopupNavigation.Instance.PushAsync(dialog, false);
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        #endregion
    }
}
