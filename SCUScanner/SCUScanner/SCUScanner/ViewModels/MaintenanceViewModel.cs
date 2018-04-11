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
        private string GetFileNameFromSerialNo(string serial)
        {
            string result = "";
            int index_ = serial.LastIndexOf('-');
            if (index_ == 0)
                index_ = serial.Length;
            result = serial.Substring(0, index_).ToLower() ;
            result = $"{result}({SettingsBase.SelectedLang.ToLower()}).pdf";
            return result;
        }
        public MaintenanceViewModel(INavigation navigation)
        {
            SerialNumber = "";
            Navigation = navigation;
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 //if (Path.GetExtension(SerialNumber) != "pdf")
                 //    SerialNumber += ".pdf";
                 string filename = GetFileNameFromSerialNo(SerialNumber);
                 
                 var filenamelocal = Path.Combine(WorkDir, filename);
                 FtpClient client = new FtpClient(FtpHost);
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
                 SerialNumber = App.mainTabbed?.CurrentConnectDeviceSN;
             });
        }
        private void DownLoadManual()
        {

        }
    }
}
