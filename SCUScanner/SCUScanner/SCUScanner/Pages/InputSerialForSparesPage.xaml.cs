using Acr.UserDialogs;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputSerialForSparesPage : ContentPage
    {
        public InputSerialForSparesPage()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel();
            bScanQR.Clicked += BScanQR_Clicked;
            eSerialNumber.Text = "MP60002-0100100";
        }
        private async void BScanQR_Clicked(object sender, EventArgs e)
        {
            var scanPage = new ZXingScannerPage();

            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    eSerialNumber.Text = result.Text;
                    App.SerialNumber = result.Text;
                    //  DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private async void SparesStart_Clicked(object sender, EventArgs e)
        {
            App.SerialNumber = eSerialNumber.Text;
            await SparePage.InitSparePage();
            await Navigation.PushAsync(new SparePage());
        }
    }
}