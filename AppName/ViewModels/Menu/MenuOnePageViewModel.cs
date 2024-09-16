using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using AppName.Core;
using AppName.ViewModels.BarCode.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using AppName.Model;
using System.Linq;
using System.Threading.Tasks;

namespace AppName
{
    public class MenuOnePageViewModel : BaseViewModel
    {
        public Command OpenMenu { get; private set; }
        public Command RightClick { get; private set; }

        public ObservableCollection<ArticleData> _slide;

        public ObservableCollection<ArticleData> Slide
        {
            get { return _slide; }
            set { SetProperty(ref _slide, value); }
        }

        public CompanyInfoModel _company;

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set { SetProperty(ref _showLoading, value); }
        }

        public CompanyInfoModel CompanyInfo
        {
            get { return _company; }
            set { SetProperty(ref _company, value); }
        }

        bool isExecuting = false;

        public MenuOnePageViewModel()
        {
            ShowLoading = false;

            //LoadData();

            RightClick = new Command(async (sender) =>
            {
                if (isExecuting)
                {
                    return;
                }
                else
                {
                    try
                    {
                        isExecuting = true;

                        await Application.Current.MainPage.Navigation.PushAsync(new SettingOnePage());
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        isExecuting = false;
                    }
                }
            });


            OpenMenu = new Command(async (sender) =>
            {
                try
                {
                    ShowLoading = true;

                    await Task.Delay(1);

                    if (CheckConnectInternet.IsConnectedNotClearCookie() == true)
                    {
                        if (isExecuting)
                        {
                            return;
                        }
                        else
                        {
                            try
                            {
                                isExecuting = true;

                                if (sender.ToString() == "SanPham")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new SanPhamPage(), false);
                                }
                                else if (sender.ToString() == "KhuyenMai")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new BarCodeKhuyenMai(), false);
                                }
                                else if (sender.ToString() == "GiamGia")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new BarcodeMarkdown(), false);
                                }
                                else if (sender.ToString() == "TonKho")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new BarCodeTonKho(), false);
                                }
                                else if (sender.ToString() == "XuatNhap")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new XuatNhapPage(), false);
                                }
                                else if (sender.ToString() == "CheckSize")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new BarCodeCheckSize(), false);
                                }
                                else if (sender.ToString() == "KiemKe")
                                {
                                    await Application.Current.MainPage.Navigation.PushAsync(new StockCountPage(), false);
                                }
                                else if (sender.ToString() == "Setting")
                                {
                                    var dialog = new NotificationErrorPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "You are not authorized for this function." };
                                    await PopupNavigation.Instance.PushAsync(dialog);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                isExecuting = false;
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }
                finally
                {
                    ShowLoading = false;
                }
            });
        }


        //private async void LoadData()
        //{
        //    if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        #region

        //        var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

        //        HttpClient client = new HttpClient();
        //        client.DefaultRequestHeaders.Authorization = authHeader;

        //        Uri uri = new Uri(string.Format(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/home/GetCompanyInfo", string.Empty));

        //        HttpResponseMessage response = await client.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string content = await response.Content.ReadAsStringAsync();

        //            CompanyInfo = JsonConvert.DeserializeObject<CompanyInfoModel>(content);
        //        }

        //        #endregion
        //    }
        //    catch (Exception)
        //    {
        //        var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Please check the server. Contact supplier for support." };
        //        await PopupNavigation.Instance.PushAsync(dialog);
        //        await Application.Current.MainPage.Navigation.PushAsync(new LoginFrm());
        //    }
        //}
    }
}
