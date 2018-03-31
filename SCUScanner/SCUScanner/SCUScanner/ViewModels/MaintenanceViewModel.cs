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
    public class MaintenanceViewModel:BaseViewModel
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
            set=> this.RaiseAndSetIfChanged(ref this.serialnumber, value);
        }
        public MaintenanceViewModel(INavigation navigation)
        {
            Navigation = navigation;
            WorkDir = DependencyService.Get<ISQLite>().GetWorkDir();
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 FtpClient client = new FtpClient(FtpHost);
                 client.Credentials = new NetworkCredential("chesterr_urmat", "Scuscanner2018");

                 // begin connecting to the server
                 await client.ConnectAsync();
                 if (Path.GetExtension(SerialNumber) != "pdf")
                     SerialNumber += ".pdf";
                 using (var cancelSrc = new CancellationTokenSource())
                 {
                     using (App.Dialogs.Loading("Downloading manual", cancelSrc.Cancel, Resources["CancelText"]))
                     {
                         string filename = Path.Combine(WorkDir, serialnumber);
                         if (client.DownloadFile(filename, $"/manuals/{SerialNumber}", true))
                         {
                             if (File.Exists(filename))
                             {
                                 WebViewPageCS webViewPageCS = new WebViewPageCS(filename);
                                 await Navigation.PushAsync(webViewPageCS);
                             }
                         }
                     }

                 };
                 SerialNumber = App.mainTabbed?.CurrentConnectDeviceSN;
             });
            }
        private void DownLoadManual()
        {

        }
    }
}
