using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SCUScanner.Droid.Services;
using SCUScanner.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(OpenPDF))]
namespace SCUScanner.Droid.Services
{

    public class OpenPDF : IOpenPDF
    {
        
        public void OpenPdf(string filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + filePath);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "application/pdf");
            
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {

                Application.Context.StartActivity(intent);

            }
            catch (Exception e)
            {
                
                    AlertDialog.Builder builder = new AlertDialog.Builder(Application.Context);
                    AlertDialog alert = builder.Create();

                    alert.SetTitle("No Application Found");
                    alert.SetMessage("Download one from Android Market?");
                    alert.Window.SetType(Android.Views.WindowManagerTypes.Toast);
                    alert.SetButton("Yes, Please", (sender, args) =>
                    {
                        Intent marketIntent = new Intent(Intent.ActionView);
                        marketIntent.SetData(Android.Net.Uri.Parse("market://details?id=com.adobe.reader"));
                        Application.Context.StartActivity(marketIntent);
                    });
                    alert.SetButton("No, Thanks", (sender, args) =>
                     {
                     });
                    alert.Show();
                
                //  Toast.MakeText(Application.Context, "No Application Available to View PDF", ToastLength.Short).Show();
            }
        }
    }
}