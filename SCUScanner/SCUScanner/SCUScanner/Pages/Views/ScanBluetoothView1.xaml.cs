using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanBluetoothView1 : ContentView
	{
       public ScanBluetoothViewModel ScanViewModel { get; set; }
        public ScanBluetoothView1 ()
		{
			InitializeComponent ();
              BindingContext = ScanViewModel = new ScanBluetoothViewModel();
            //scanBluetoothViewModel = this.BindingContext as ScanBluetoothViewModel;
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {

                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                    // BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    //this.Title = settings.Resources["MainText"];
                    if (App.mainTabbed != null)
                    {
                        App.mainTabbed.Title = settings.Resources["MainText"];
                        ScanViewModel.ScanTextChange(ScanViewModel.IsScanning);
                    }
                }
            });

        }
    }
}