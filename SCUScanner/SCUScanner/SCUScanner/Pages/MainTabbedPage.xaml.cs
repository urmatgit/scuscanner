using Plugin.BluetoothLE;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
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
        private IGattCharacteristic _SelectedCharacteristic;
        public IGattCharacteristic SelectedCharacteristic { 
                get { return _SelectedCharacteristic; }
            set {
                _SelectedCharacteristic = value;
            }
            }
        public MainTabbedPage ()
        {

            InitializeComponent();
            BindingContext = new BaseViewModel();
        }
        public ScanBluetoothPage ScanPage
        {
            get { return ScanningPage; }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ScanningPage.DoAppearing();

        }
    }
}