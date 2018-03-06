
using Plugin.BluetoothLE;
using SCUScanner.Helpers;
using SCUScanner.Services;
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
	public partial class ScanBluetoothPage : BaseTabPage
    {
        bool IsBluetoothEnabled = false;
        ScanBluetoothViewModel scanBluetoothViewModel;

        public ScanBluetoothPage ()
		{
			InitializeComponent ();
            Kod = GlobalConstants.MAIN_TAB_PAGE;   
            BindingContext = scanBluetoothViewModel=new ScanBluetoothViewModel(Tabbed);
            lblHintBluetoothAndroidText.IsVisible = !(lblHintBluetoothIOSText.IsVisible = Device.RuntimePlatform == Device.iOS);
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {

                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                   // BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    this.Title = settings.Resources["MainText"];
                    if (App.mainTabbed != null)
                    {
                        App.mainTabbed.Title= settings.Resources["MainText"];
                        scanBluetoothViewModel.ScanTextChange(scanBluetoothViewModel.IsScanning);
                    }
                }
            });

        }
        public void DoAppearing()
        {
            OnAppearing();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ScanBluetoothViewModel). ParentTabbed = Tabbed;
            (this.BindingContext as IViewModel)?.OnActivate();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (this.BindingContext as IViewModel)?.OnDeactivate();
        }
    }
}