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
#if DEBUG
            eSerialNumber.Text = "MP60002-0100100";
#endif
        }
        private async void bSpareClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(eSerialNumber.Text)) return;
            App.SerialNumber = eSerialNumber.Text;

            await SparePage.InitSparePage();
            await Navigation.PushAsync(new SparePage());
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
                    App.SerialNumber= result.Text;
                    //  DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private async void bListOfManualsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListOfManualPage());
           
        }
    }
}