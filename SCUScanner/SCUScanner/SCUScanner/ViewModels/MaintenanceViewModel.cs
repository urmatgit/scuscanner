using FluentFTP;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using ReactiveUI;
using SCUScanner.Helpers;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class MaintenanceViewModel : BaseViewModel
    {
        private INavigation Navigation;
        
        public ICommand ScanQRCommand { get; }
        public ICommand DownloadManualCommand { get; }
        private string serialnumber;
        private bool cameraPermission = false;
        public bool CameraPermission
        {
            get => cameraPermission;
            set => this.RaiseAndSetIfChanged(ref cameraPermission, value);
        }
        string WorkDir;
        public string SerialNumber
        {
            get => serialnumber;
            set => this.RaiseAndSetIfChanged(ref this.serialnumber, value);
        }
        private async Task  PermissionCamera()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {

                string infoText = SettingsBase.Resources["InfoPermissionLocationText"];
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await App.Dialogs.AlertAsync(infoText, "Info", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key ca
                if (results.ContainsKey(Permission.Camera))
                    status = results[Permission.Camera];

            }
            CameraPermission =  status == PermissionStatus.Granted;
        }
        public  MaintenanceViewModel(INavigation navigation)
        {

            SerialNumber = App.mainTabbed?.deviceListViewModel?.SN;;
             PermissionCamera();

            Navigation = navigation;
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 //if (Path.GetExtension(SerialNumber) != "pdf")
                 //    SerialNumber += ".pdf";
                 string filename =Utils. GetFileNameFromSerialNo(SerialNumber, SettingsBase.SelectedLangKod.ToLower());
                 
                 var filenamelocal = Path.Combine(WorkDir, filename);
                 FtpClient client = new FtpClient(GlobalConstants.FtpHost);
                 try
                 {

                     client.Credentials = new NetworkCredential("chesterr_urmat", "Scuscanner2018");

                     // begin connecting to the server
                     await client.ConnectAsync();
                     if (client.IsConnected)
                     {
                         

                         {
                             
                             if (client.FileExists($"/manuals/{filename}"))
                             {
                                 //if (File.Exists(filename))
                                 //{
                                 //    File.Delete(filename);
                                 //}
                                 bool dowloaded = false;
                                 try
                                 {
                                     using (var cancelSrc = new CancellationTokenSource())
                                     {
                                         using (App.Dialogs.Loading(Resources["DownloadText"], cancelSrc.Cancel, Resources["CancelText"]))
                                         {
                                             string tmpFileName = filenamelocal + DateTime.Now.Second.ToString();
                                             dowloaded = await client.DownloadFileAsync(tmpFileName, $"/manuals/{filename}", true);
                                             File.Copy(tmpFileName, filenamelocal, true);
                                             try
                                             {
                                                 File.Delete(tmpFileName);
                                             }catch(Exception er)
                                             {

                                             }
                                         }
                                     }
                                 }catch(Exception ex)
                                 {
                                     await App.Dialogs.AlertAsync(ex.ToString());

                                 }
                                     //if (dowloaded || File.Exists(filename))
                                     //{
                                     //    WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                                     //    await Navigation.PushAsync(webViewPageCS);
                                     //}
                                 
                             }
                             else
                             {

                                 await App.Dialogs.AlertAsync(Resources["ManualNotFoundText"]);
                             }



                         };
                     }
                     else
                     {
                         await App.Dialogs.AlertAsync(Resources["NoInternetConOrErrorText"]);
                     }
                 }
                 catch (Exception ex)
                 {
                     await App.Dialogs.AlertAsync(Resources["NoInternetConOrErrorText"]);

                 }
                 finally
                 {
                     await client?.DisconnectAsync();
                     if ( File.Exists(filenamelocal))
                     {
                         WebViewPageCS webViewPageCS = new WebViewPageCS(filenamelocal);
                         await Navigation.PushAsync(webViewPageCS);
                     }
                 }
                //SerialNumber = App.mainTabbed?.CurrentConnectDeviceSN;
             }, this.WhenAnyValue(x=>x.CameraPermission));
            
        }
        private void DownLoadManual()
        {

        }
    }
}
