
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
        

		public ScanBluetoothPage ()
		{
			InitializeComponent ();
            
            BindingContext = new ScanBluetoothViewModel(Tabbed);
            lblHintBluetoothAndroidText.IsVisible = !(lblHintBluetoothIOSText.IsVisible = Device.RuntimePlatform == Device.iOS);
            
            
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