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
                var activity = (MainActivity)Xamarin.Forms.Forms.Context;
                activity.openPDF(); 
                //  Toast.MakeText(Application.Context, "No Application Available to View PDF", ToastLength.Short).Show();
            }
        }
    }
}