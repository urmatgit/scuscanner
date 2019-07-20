using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SCUScanner.Droid.Services;
using SCUScanner.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(OpenPDF))]
namespace SCUScanner.Droid.Services
{
    // using Uri = Android.Net.Uri;

    // Intent intent = new Intent(Intent.ActionView);
    // intent.SetDataAndType(Uri.Parse("file:///" + PathToFile(filename)), "application/pdf");
    // intent.SetFlags(ActivityFlags.ClearTop);
    // Forms.Context.StartActivity(intent);
    // return null;
    public class OpenPDF : IOpenPDF
    {
        public void OpenPdf4(string filePath)
        {

            // var bytes = File.ReadAllBytes(filePath);

            //Copy the private file's data to the EXTERNAL PUBLIC location
            string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
            string application = "";

            string extension = System.IO.Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    application = "application/msword";
                    break;
                case ".pdf":
                    application = "application/pdf";
                    break;
                case ".xls":
                case ".xlsx":
                    application = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    application = "image/jpeg";
                    break;
                default:
                    application = "*/*";
                    break;
            }
            // var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/report" + extension;
            // System.IO.File.WriteAllBytes(externalPath, bytes);

            Java.IO.File file = new Java.IO.File(filePath);
            file.SetReadable(true);
            //Android.Net.Uri uri = Android.Net.Uri.Parse("file://" + filePath);
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, application);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                Application.Context.StartActivity(intent);
                //Application.Context.StartActivity(Intent.CreateChooser(intent, "Open pdf file"));
            }
            catch (Exception)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "No Application Available to View PDF", ToastLength.Short).Show();
                var activity = (MainActivity)Xamarin.Forms.Forms.Context;
                activity.openPDF();
            }
        }

        public void OpenPdf(string filename, string needPermission = "To view files this application needs permissions to access storage.",
            string notPermisson = "Can not access storage on this device, try again.", string noApplication = "No Application Available to View PDF")
        {
           
            try
            {
                var status = CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage).Result;
                if (status != PermissionStatus.Granted)
                {
                    if (CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage).Result)
                    {
                        Toast.MakeText(Xamarin.Forms.Forms.Context, needPermission, ToastLength.Short).Show();
                    }

                    //var results = CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage).Result;
                    ////Best practice to always check that the key exists
                    //if (results.ContainsKey(Permission.Storage))
                    //    status = results[Permission.Storage];
                }
                var CurrentContext = Xamarin.Forms.Forms.Context;
                if (status == PermissionStatus.Granted)
                {
                    var file = new Java.IO.File(Path.Combine(CurrentContext.FilesDir.Path, filename));
                    if (file.Exists())
                    {
                        System.Diagnostics.Debug.WriteLine(file.Path);
                    }
                    //var file = new Java.IO.File( filename);
                    Android.Net.Uri uri = FileProvider.GetUriForFile(CurrentContext, CurrentContext.PackageName + ".fileprovider", file);
                    if (System.IO.File.Exists(uri.Path))
                    {
                        System.Diagnostics.Debug.WriteLine(uri.Path);
                    }
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetDataAndType(uri, "application/pdf");
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.NoHistory);

                    // Trying to allow writing to the external app ...
                    var resInfoList = CurrentContext.PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
                    foreach (var resolveInfo in resInfoList)
                    {
                        var packageName = resolveInfo.ActivityInfo.PackageName;
                        CurrentContext.GrantUriPermission(packageName, uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantPrefixUriPermission | ActivityFlags.GrantReadUriPermission);

                    }
                    CurrentContext.StartActivity(intent);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Toast.MakeText(Xamarin.Forms.Forms.Context, notPermisson, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, noApplication, ToastLength.Short).Show();
            }
        }
        public void OpenPdf2(string filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("file://" + filePath);

            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "application/pdf");
            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
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
        public void OpenPdf1(string filename)
        {
            Java.IO.File f = new Java.IO.File(filename);
            if (f.Exists())
            {
                try
                {
                    Intent openFileIntent = new Intent(Intent.ActionView);
                    openFileIntent.SetDataAndType(Android.Net.Uri.FromFile(f), "application/pdf");
                    openFileIntent.SetFlags(ActivityFlags.NoHistory);
                    openFileIntent.SetFlags(ActivityFlags.GrantReadUriPermission);

                    Application.Context.StartActivity(Intent.CreateChooser(openFileIntent, "Open pdf file"));
                }
                catch (ActivityNotFoundException)
                {
                    //handle when no available apps
                }
            }
        }
    }
}