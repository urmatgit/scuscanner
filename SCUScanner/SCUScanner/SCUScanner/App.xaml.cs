using Acr.UserDialogs;
using Plugin.BluetoothLE;
using SCUScanner.Models;
using SCUScanner.Resources;
using SCUScanner.Services;
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
        public static IAdapterScanner BleAdapterScanner;
        
        public App ()
		{
			InitializeComponent();
            Dialogs = UserDialogs.Instance;
            BleAdapter = CrossBleAdapter.Current;
            BleAdapterScanner = CrossBleAdapter.AdapterScanner;
            if (Device.RuntimePlatform != Device.WinPhone)
            {
                if (!string.IsNullOrEmpty(Settings.Current.SelectedLang))
                {
                    AppResource.Culture = DependencyService.Get<ILocalizeService>().SetLocale(Settings.Current.SelectedLang);
                    CurrentLanguage = AppResource.Culture.Name;
                }
                else
                {
                    
                    AppResource.Culture = DependencyService.Get<ILocalizeService>().GetCurrentCultureInfo();
                    Settings.Current.SelectedLang = CurrentLanguage = AppResource.Culture.Name;
                }
                
            }
            MainPage = new SCUScanner.Pages.MainMasterDetailPage();
		}
        
		protected override void OnStart ()
		{
			// Handle when your app starts
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
