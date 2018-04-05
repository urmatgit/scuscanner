using FluentFTP;
using ReactiveUI;
using SCUScanner.Helpers;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class MaintenanceViewModel : BaseViewModel
    {
        private INavigation Navigation;
        private const string FtpHost = "ftp://ftp.chester.ru";
        public ICommand ScanQRCommand { get; }
        public ICommand DownloadManualCommand { get; }
        private string serialnumber;
        string WorkDir;
        public string SerialNumber
        {
            get => serialnumber;
            set => this.RaiseAndSetIfChanged(ref this.serialnumber, value);
        }
        public MaintenanceViewModel(INavigation navigation)
        {
            SerialNumber = "readme";
            Navigation = navigation;
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 if (Path.GetExtension(SerialNumber) != "pdf")
                     SerialNumber += ".pdf";
                 string filename = Path.Combine(WorkDir, serialnumber);
                 FtpClient client = new FtpClient(FtpHost);
                 try
                 {

                     client.Credentials = new NetworkCredential("chesterr_urmat", "Scuscanner2018");

                     // begin connecting to the server
                     await client.ConnectAsync();
                     if (client.IsConnected)
                     {
                         

                         {
                             
                             if (client.FileExists($"/manuals/{SerialNumber}"))
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
                                             string tmpFileName = filename + DateTime.Now.Second.ToString();
                                             dowloaded = await client.DownloadFileAsync(tmpFileName, $"/manuals/{SerialNumber}", true);
                                             File.Copy(tmpFileName, filename,true);
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
                     if ( File.Exists(filename))
                     {
                         WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                         await Navigation.PushAsync(webViewPageCS);
                     }
                 }
                 SerialNumber = App.mainTabbed?.CurrentConnectDeviceSN;
             });
        }
        private void DownLoadManual()
        {

        }
    }
}
