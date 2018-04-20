using MvvmCross;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
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
	public partial class DeviceListPage : BaseTabbedPage
    {
        DeviceListViewModel deviceListViewModel;
        public DeviceListPage ()
		{
			InitializeComponent ();
            var adapter = CrossBluetoothLE.Current.Adapter;
            BindingContext = deviceListViewModel = new DeviceListViewModel(adapter);

		}
	}
}