using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MaintenancePage : ContentPage
	{
        MaintenanceViewModel maintenanceViewModel;
        
        public MaintenancePage ()
		{
			InitializeComponent ();
            BindingContext = maintenanceViewModel = new MaintenanceViewModel(Navigation);
            bScanQR.Clicked += BScanQR_Clicked;

        }

        private async void BScanQR_Clicked(object sender, EventArgs e)
        {
            var scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    maintenanceViewModel.SerialNumber = result.Text;
                   //  DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }
    }
}