using Acr.UserDialogs;
using Plugin.BluetoothLE;
using SCUScanner.Models;
using SCUScanner.Resources;
using SCUScanner.Services;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SCUScanner
{
    public partial class App : Application
    {
        public static string CurrentLanguage = "EN";
        public static IUserDialogs Dialogs;
        public static IAdapter BleAdapter;
        
        
        public App ()
		{
			InitializeComponent();
            Dialogs = UserDialogs.Instance;
            BleAdapter = CrossBleAdapter.Current;
            
            if (Device.RuntimePlatform != Device.WinPhone)
            {
                if (!string.IsNullOrEmpty(Settings.Current.SelectedLang))
                {
                    AppResource.Culture = DependencyService.Get<ILocalizeService>().SetLocale(Settings.Current.SelectedLang);
                    CurrentLanguage = AppResource.Culture.Name;
                    Settings.Current.SetResourcesLang(CurrentLanguage);
                }
                else
                {
                    
                    AppResource.Culture = DependencyService.Get<ILocalizeService>().GetCurrentCultureInfo();
                    Settings.Current.SelectedLang = CurrentLanguage = AppResource.Culture.Name;
                }
                
            }

            MainPage = new SCUScanner.Pages.MainMasterDetailPage();
		}
        

		protected override async void OnStart ()
		{
            switch (BleAdapter.Status)
            {
                case AdapterStatus.Unsupported:
                    await Dialogs.AlertAsync(Settings.Current.Resources["BluetoothUnsupportText"]);
                    return;
                case AdapterStatus.PoweredOff:

                    bool res = await Dialogs.ConfirmAsync(Settings.Current.Resources["AskBluetoothSetText"], okText: Settings.Current.Resources["OkText"], cancelText: Settings.Current.Resources["CancelText"]);
                    if (res)
                    {
                        if (BleAdapter.CanControlAdapterState())
                            BleAdapter.SetAdapterState(true);
                        else if (BleAdapter.CanOpenSettings())
                            BleAdapter.OpenSettings();
                    }
                    break;
            }
            // Handle when your app starts
            //if (BleAdapter.Status == AdapterStatus.PoweredOff )
            //{
            //    Task.Run(async () =>
            //    {
            //        bool res= await Dialogs.ConfirmAsync(Settings.Current.Resources["AskBluetoothSetText"], okText: Settings.Current.Resources["OkText"], cancelText: Settings.Current.Resources["CancelText"]);
            //          if(res)
            //        {
            //            if (BleAdapter.CanControlAdapterState())
            //                BleAdapter.SetAdapterState(true);
            //            else if (BleAdapter.CanOpenSettings())
            //                BleAdapter.OpenSettings();
            //        }
            //    });
            //}
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
