using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SCUScanner.ViewModels
{
    public class MaintenanceViewModel:BaseViewModel
    {
        public ICommand ScanQRCommand { get; }
        public ICommand DownloadManualCommand { get; }
        private string serialnumber;
        public string SerialNumber
        {
            get => serialnumber;
            set=> this.RaiseAndSetIfChanged(ref this.serialnumber, value);
        }
        public MaintenanceViewModel()
        {
            ScanQRCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            DownloadManualCommand = ReactiveCommand.CreateFromTask(async () =>
             {

             });
            SerialNumber = App.mainTabbed?.CurrentConnectDeviceSN;
        }
            
        private void DownLoadManual()
        {

        }
    }
}
