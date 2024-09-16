using AppName.Model;
using AppName.Model.Pickup;
using AppName.Model.XuatNhap;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using Scandit.BarcodePicker.Unified;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppName
{
    public class PickUPLineDetailViewModel : BaseViewModel
    {
        private bool _isUpdate;
        /// <summary>
        /// 0: tạo mới
        /// 1: cập nhật
        /// </summary>
        public bool IsUpdate
        {
            get { return _isUpdate; }
            set { SetProperty(ref _isUpdate, value); }
        }

        private bool _showLoading;
        public bool ShowLoading
        {
            get { return _showLoading; }
            set
            {
                _showLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _IsCamera;
        public bool IsCamera
        {
            get { return _IsCamera; }
            set
            {
                _IsCamera = value;
                OnPropertyChanged();
            }
        }

        private bool _IsSource;
        public bool IsSource
        {
            get { return _IsSource; }
            set
            {
                _IsSource = value;
                OnPropertyChanged();
            }
        }

        private bool _IsEnabledButton;
        public bool IsEnabledButton
        {
            get { return _IsEnabledButton; }
            set
            {
                _IsEnabledButton = value;
                OnPropertyChanged();
            }
        }

        private string _PathImage;
        public string PathImage
        {
            get { return _PathImage; }
            set { SetProperty(ref _PathImage, value); }
        }

        private ObservableCollection<NoteModel> _noteList;
        public ObservableCollection<NoteModel> NoteList
        {
            get { return _noteList; }
            set { SetProperty(ref _noteList, value); }
        }

        private NoteModel _selectedNote;
        public NoteModel SelectedNote
        {
            get { return _selectedNote; }
            set { SetProperty(ref _selectedNote, value); }
        }

        private ObservableCollection<ReasonModel> _reasonList;
        public ObservableCollection<ReasonModel> ReasonList
        {
            get { return _reasonList; }
            set { SetProperty(ref _reasonList, value); }
        }

        private ReasonModel _selectedReason;
        public ReasonModel SelectedReason
        {
            get { return _selectedReason; }
            set { SetProperty(ref _selectedReason, value); }
        }

        private PickUpProductDetail _dataModel;
        public PickUpProductDetail DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public ICommand UpdatePickUPLineDetailCommand { get; set; }

        public EventHandler<PickUpProductDetail> ClosePopup;

        public PickUPLineDetailViewModel(PickUpProductDetail itemModel)
        {
            UpdatePickUPLineDetailCommand = new Command(UpdatePickUPLineDetailCommandAsync);

            DataModel = itemModel;

            LoadReason();
            LoadNote();
        }

        private async void UpdatePickUPLineDetailCommandAsync(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(PathImage) && string.IsNullOrEmpty(DataModel.ImageFile))
                {

                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Vui lòng nhập hình ảnh sản phẩm." };
                    PopupNavigation.Instance.PushAsync(dialog, false);

                    return;
                }

                if (DataModel.QuantityScan == 0)
                {
                    var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = "Vui lòng nhập số lượng." };
                    PopupNavigation.Instance.PushAsync(dialog, false);

                    return;
                }

                if (SelectedNote != null)
                {
                    if (!string.IsNullOrEmpty(SelectedNote.NoteName))
                    {
                        DataModel.Note = SelectedNote.NoteName;
                    }
                }

                if (SelectedReason != null)
                {
                    if (!string.IsNullOrEmpty(SelectedReason.ReasonName))
                    {
                        DataModel.Reason = SelectedReason.ReasonName;
                    }
                }

                if (!string.IsNullOrEmpty(PathImage))
                {
                    byte[] imgdata = System.IO.File.ReadAllBytes(PathImage);

                    string base64String = Convert.ToBase64String(imgdata);
                    DataModel.ImageFile = base64String;
                }

                ClosePopup?.Invoke(this, DataModel);

            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadReason()
        {
            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/GetReason");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var listType = JsonConvert.DeserializeObject<ResuftReasonModel>(content);

                    if (listType.ListData != null)
                    {
                        var listReasonForTransfer = listType.ListData.Where(s => s.ReasonCode.Contains("RTV_")).ToList();

                        ReasonList = new ObservableCollection<ReasonModel>(listReasonForTransfer);
                    }

                    if (DataModel != null)
                    {
                        if (!string.IsNullOrEmpty(DataModel.Reason))
                        {
                            SelectedReason = ReasonList.Where(s => s.ReasonName == DataModel.Reason).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }

        private async Task LoadNote()
        {

            if (CheckConnectInternet.IsConnectedNotClearCookie() == false)
            {
                return;
            }

            try
            {
                var authHeader = new AuthenticationHeaderValue("bearer", Application.Current.Properties["Token"].ToString().Replace("Bearer", ""));

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = authHeader;

                Uri uri = new Uri(RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault().URLApi + "/api/pickup/GetNote");

                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var resuft = JsonConvert.DeserializeObject<ResuftNoteModel>(content);

                    if (resuft.ListData.Count() > 0)
                    {
                        var listReasonForTransfer = resuft.ListData.Where(s => s.NoteCode.Contains("RTV_")).ToList();

                        NoteList = new ObservableCollection<NoteModel>(resuft.ListData);
                    }
                    else
                    {
                        NoteList = new ObservableCollection<NoteModel>();
                    }

                    if (DataModel != null)
                    {
                        if (!string.IsNullOrEmpty(DataModel.Note))
                        {
                            SelectedNote = NoteList.Where(s => s.NoteName == DataModel.Note).FirstOrDefault();
                        }
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                var dialog = new NotificationWarrningPopup(this.GetType().Name, CheckConnectInternet.GetmethodName()) { Message = ex.Message };
                PopupNavigation.Instance.PushAsync(dialog, false);
            }
        }


        public partial class ResuftNoteModel
        {
            public bool Active { get; set; }
            public List<NoteModel> ListData { get; set; }
        }

        public class NoteModel
        {
            public string NoteCode { get; set; }
            public string NoteName { get; set; }
        }

        public partial class ResuftReasonModel
        {
            public bool Active { get; set; }
            public List<ReasonModel> ListData { get; set; }
        }

        public class ReasonModel
        {
            public string ReasonCode { get; set; }
            public string ReasonName { get; set; }
        }
    }
}
