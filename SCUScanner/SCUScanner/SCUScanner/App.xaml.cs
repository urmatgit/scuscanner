using Acr.UserDialogs;

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
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace SCUScanner
{
    public partial class App : Application
    {
        public static string CurrentLanguage = "EN";
        public static IUserDialogs Dialogs;
        //public static IAdapter BleAdapter;
        public const string DATABASE_NAME = "SCUData.db";
        public static bool IsAccessToBle=false;
        public static SCUDataRepository database;
        public static SCUDataRepository Database
        {
            get
            {
                if (database == null)
                {
                    database = new SCUDataRepository(DATABASE_NAME);
                }
                return database;
            }
        }
        public static SCUScanner.Pages.DeviceListPage mainTabbed { get;  set; }
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU2MTFAMzEzNjJlMzIyZTMwR09JdHZ0cmFVakkzKzI4cVRwNS81Y1E1aUNPY21Na3JRd1ZvV2UvVkFSVT0=");
            InitializeComponent();
            Dialogs = UserDialogs.Instance;
            //BleAdapter = CrossBleAdapter.Current;

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


        private async Task CheckLocationPermission()
        {
             
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                
                string infoText = Settings.Current.Resources["InfoPermissionLocationText"];
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                       await App.Dialogs.AlertAsync(infoText, "Info", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location))
                    status = results[Permission.Location];
                
            }
            IsAccessToBle = true;
        }

        protected override  async void OnStart()
        {

            if (Device.Android == Device.RuntimePlatform)
            {

                await CheckLocationPermission();

            }
            else
                App.IsAccessToBle = true;
           await   Database.CreateTable();
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

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
