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
	public partial class DeviceSettingPage : BaseTabPage
    {
        DeviceSettingViewModel deviceSettingViewModel;
        public DeviceSettingPage (ScanResultViewModel selectedDevice)
		{
			InitializeComponent ();
            BindingContext = deviceSettingViewModel= new DeviceSettingViewModel(selectedDevice);

        }
        protected override void OnAppearing()
        {
            deviceSettingViewModel.ParentTabbed = this.Tabbed;
            base.OnAppearing();
        }
        public override void Dispose()
        {
            deviceSettingViewModel = null;
            base.Dispose();
        }
    }
}