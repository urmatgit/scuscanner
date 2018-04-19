
using SCUScanner.Models;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {

        public string CurrentConnectDeviceSN { get; set; }
        //private IGattCharacteristic _SelectedCharacteristic;
        //public IGattCharacteristic SelectedCharacteristic { 
        //        get { return _SelectedCharacteristic; }
        //    set {
        //        _SelectedCharacteristic = value;
        //    }
        //    }
        private CurrentDeviceInfo currentDeviceInfo;
        public CurrentDeviceInfo CurrentDeviceInfo
        {
            get => currentDeviceInfo;
            set => currentDeviceInfo = value;
        }
        public MainTabbedPage ()
        {
            CurrentDeviceInfo = new CurrentDeviceInfo();
            InitializeComponent();
            BindingContext = new BaseViewModel();
        }
        //public CharacterPage characterPage
        //{
        //    get; set;
        //}
        //public DeviceSettingPage deviceSettingPage
        //{
        //    get;set;
        //}
        //public ScanBluetoothPage ScanPage
        //{
        //    get { return ScanningPage; }
        //}
        //public void CloseConnection()
        //{
        //    ScanningPage.scanBluetoothViewModel.CleanTabPages();
        //}
        protected override void OnAppearing()
        {
            base.OnAppearing();
//            ScanningPage.DoAppearing();

        }
        protected override void OnPagesChanged(NotifyCollectionChangedEventArgs e)
        {
            
            base.OnPagesChanged(e);
        }
        protected override void OnCurrentPageChanged()
        {
            
            base.OnCurrentPageChanged();
        }
    }
}