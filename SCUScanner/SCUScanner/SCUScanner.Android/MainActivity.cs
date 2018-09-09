using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace SCUScanner.Droid
{
    [Activity( Theme = "@style/MainTheme",   ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU2MTFAMzEzNjJlMzIyZTMwR09JdHZ0cmFVakkzKzI4cVRwNS81Y1E1aUNPY21Na3JRd1ZvV2UvVkFSVT0=");
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
                CrossCurrentActivity.Current.Activity = this;
            
            base.OnCreate(bundle);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
                UserDialogs.Init(this);
                ZXing.Net.Mobile.Forms.Android.Platform.Init();
                MR.Gestures.Android.Settings.LicenseKey = "CB2F-LQLC-HAY5-7DMG-DSZZ-FAEX-RF5D-3RYN-FE74-4RN3-NVVD-34LH-DEMV";

                LoadApplication(new App());
           
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

