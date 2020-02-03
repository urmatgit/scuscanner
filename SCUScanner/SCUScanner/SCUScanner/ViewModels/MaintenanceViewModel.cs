using Acr.UserDialogs;
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
        private bool cameraPermission = true;
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
        public async Task  PermissionCamera()
        {
           

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {

                 if (Device.Android == Device.RuntimePlatform)
                    {
                string infoText = SettingsBase.Resources["InfoPermissionCameraText"];
                //if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await App.Dialogs.AlertAsync(infoText, SettingsBase.Resources["NeedPermissionText"],   SettingsBase.Resources["OkText"]);
                }
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

            SerialNumber = App.SerialNumber;// App.mainTabbed?.deviceListViewModel?.SN;;
            //App.SerialNumber = SerialNumber;
           

            Navigation = navigation;
            WorkDir = DependencyService.Get<ISQLite>().GetWorkManualDir();
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 App.SerialNumber = SerialNumber;
                 if (string.IsNullOrEmpty(SerialNumber)) return;
                 //if (Path.GetExtension(SerialNumber) != "pdf")
                 //    SerialNumber += ".pdf";
                 CancellationTokenSource tokenSource = new CancellationTokenSource();

                 var config = new ProgressDialogConfig()
                 {
                     Title = $"{Resources["DownloadWaitText"] }  ({SerialNumber})",
                     CancelText = Resources["CancelText"],
                     IsDeterministic = false,
                     OnCancel = tokenSource.Cancel
                 };
                 //

                 using (var progress = App.Dialogs.Progress(config))
                 {
                     progress.Show();
                     await Utils.DownloadManual<string>(SerialNumber.ToUpper(), SettingsBase.SelectedLangKod.ToLower(), progress, async (o) =>
                      {
                          DependencyService.Get<IOpenPDF>().OpenPdf(o, needPermission: Resources["needPermission"], notPermisson: Resources["notPermisson"], noApplication: Resources["noApplication"]);

                          //WebViewPageCS webViewPageCS = new WebViewPageCS(o);
                          //await Navigation.PushAsync(webViewPageCS);
                      });
                 }


             });//, this.WhenAnyValue(true));// x.CameraPermission));
            
        }
        private void DownLoadManual()
        {

        }
    }
}
