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
using Android.Content;

namespace SCUScanner.Droid
{
    [Activity( Theme = "@style/MainTheme",   ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Context context;
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
            try
            {
                LoadApplication(new App());
            }catch (Exception er)
            {
                var mess = er.Message;
            }
           
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnResume()
        {
            context = this;
            base.OnResume();
        }
        public void openPDF()
        {
            var intent = new Android.Content.Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=com.adobe.reader"));
            // we need to add this, because the activity is in a new context.
            // Otherwise the runtime will block the execution and throw an exception
            intent.AddFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);

            //this.RunOnUiThread(() =>
            //{

            //    AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.context);
            //    AlertDialog alert = builder.Create();

            //    builder.SetTitle("No Application Found");
            //    builder.SetMessage("Download one from Android Market?");
            //    alert.Window.SetType(Android.Views.WindowManagerTypes.Toast);
            //    alert.SetButton("Yes, Please", (sender, args) =>
            //    {
            //        Intent marketIntent = new Intent(Intent.ActionView);
            //        marketIntent.SetData(Android.Net.Uri.Parse("market://details?id=com.adobe.reader"));
            //        Application.Context.StartActivity(marketIntent);
            //    });
            //    alert.SetButton("No, Thanks", (sender, args) =>
            //    {
            //    });
            //    alert.Show();
            //});
        }
    }
}

