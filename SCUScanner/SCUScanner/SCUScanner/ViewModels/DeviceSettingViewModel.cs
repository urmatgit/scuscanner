using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class DeviceSettingViewModel: BaseViewModel
    {
        public TabbedPage ParentTabbed { get; set; }
        public ICommand SendCommand { get; }
        IDevice device;

        public DeviceSettingViewModel(ScanResultViewModel selectedDevice)
        {
            device = selectedDevice.Device;

            SendCommand = ReactiveCommand.CreateFromTask(async () =>
             {
                 if (App.mainTabbed != null && App.mainTabbed.SelectedCharacteristic!=null  )
                 {
                     if (App.mainTabbed.SelectedCharacteristic.CanWrite())
                     {
                         StringBuilder stringBuilder = new StringBuilder();
                         bool DoDisconnect = false;
                         using (var cancelSrc = new CancellationTokenSource())
                         {
                             using (var dialog= App.Dialogs.Loading(Resources["SendingValueText"], cancelSrc.Cancel))
                             {
                                 CharacteristicGattResult res = null;
                                 string strResult = "";
                                 try
                                 {
                                     System.Diagnostics.Debug.WriteLine("Start sending value");
                                     if (!string.IsNullOrEmpty(BroadcastIdentity))
                                     {
                                         //WriteToDeviceWithoutResponse($"!{BroadcastIdentity}");

                                         res = await WriteToDevice($"!{BroadcastIdentity}", cancelSrc);
                                         if (res != null)
                                         {
                                             
                                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                                 strResult = $"{Resources["BroadcastIdentityText"]}- {strResult}";
                                             
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                             if (res.Success)
                                             {
                                                 DoDisconnect = true;
                                                
                                             }else
                                                 stringBuilder.AppendLine(strResult);
                                         }


                                     }
                                     if (!string.IsNullOrEmpty(AlarmLevel))
                                     {
                                         res = await WriteToDevice($"^{AlarmLevel}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["AlarmLevelText"]}- {strResult}";
                                             if (!res.Success)
                                                 stringBuilder.AppendLine(strResult);

                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (!string.IsNullOrEmpty(CutOff))
                                     {
                                         res = await WriteToDevice($"@{CutOff}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "" : res.ErrorMessage;
                                             strResult = $"{Resources["CutOffText"]}- {strResult}";
                                             if (!res.Success)
                                                 stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (!string.IsNullOrEmpty(AlarmHours))
                                     {
                                         var strAlarmHours = AlarmHours.ToString();
                                         res = await WriteToDevice($"~{strAlarmHours.PadLeft(4, '0')}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "" : res.ErrorMessage;
                                             strResult = $"{Resources["AlarmHoursText"]}- {strResult}";
                                             if (!res.Success)
                                                 stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (!string.IsNullOrEmpty(SetSerialNumber))
                                     {
                                         var serial = SetSerialNumber;
                                         if (serial.Length < 21)
                                         {
                                             serial += ">";
                                         }
                                         serial += $"${serial}";
                                         res = null;
                                         foreach (char ch in serial)
                                         {
                                             res = await WriteToDevice(ch.ToString(), cancelSrc);
                                             System.Diagnostics.Debug.WriteLine(ch.ToString());
                                             Thread.Sleep(10);
                                             
                                         }
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "" : res.ErrorMessage;
                                             strResult = $"{Resources["SetSerialNumberText"]}- {strResult}";
                                             
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                             if (res.Success)
                                             {
                                                 DoDisconnect = true;
                                                 //Disconnect();
                                             }else
                                                 stringBuilder.AppendLine(strResult);
                                         }
                                     }
                                 }
                                 catch (Exception er)
                                 {
                                     dialog.Hide();
                                     
                                     await App.Dialogs.AlertAsync(er.Message);

                                 }
                             }
                         }
                         if (DoDisconnect)
                         {
                            
                             //After changing the identity the Bluetooth module will broadcast with a different Id and so the connection must be dropped, wait 7 seconds for its internal reboot  and then re - connect

                             int duration = 8;
                             
                             using (var dialog = App.Dialogs.Progress(string.Format(Resources["WaitForChangeIDText"], duration)))
                             {
                                 int step = 100 / duration;
                                 while (dialog.PercentComplete < 100)
                                 {
                                     await Task.Delay(TimeSpan.FromSeconds(1));
                                     dialog.PercentComplete += step;
                                 }
                             }

                         }
                         if (stringBuilder.Length>0)
                            await App.Dialogs.AlertAsync(stringBuilder.ToString());
                         Disconnect();
                     }
                 }
             });
            }
        private void Disconnect()
        {
            if (App.mainTabbed != null)
            {
                App.mainTabbed.ScanPage.scanBluetoothViewModel.CleanTabPages(true);
            }
        
        }
        private async Task<CharacteristicGattResult> WriteToDevice(string str, CancellationTokenSource cancellationTokenSource)
        {
            CharacteristicGattResult result = null;
            
                byte[] bytes = Encoding.UTF8.GetBytes(str);

                result = await App.mainTabbed.SelectedCharacteristic.Write(bytes)
                                  .Timeout(TimeSpan.FromSeconds(10))
                                  .ToTask();
                

            return result;
        }
        private void WriteToDeviceWithoutResponse(string str )
        {
            

            byte[] bytes = Encoding.UTF8.GetBytes(str);

             App.mainTabbed.SelectedCharacteristic.WriteWithoutResponse(bytes);

        
        }
          
        private string broadcastIdentity;
       
        public string BroadcastIdentity
        {
            get => broadcastIdentity;
             set => this.RaiseAndSetIfChanged(ref broadcastIdentity, value);
        }
        private string alarmLevel;
        public string AlarmLevel
        {
            get => alarmLevel;
            set => this.RaiseAndSetIfChanged(ref alarmLevel, value);
        }
        private string cutOff;
        public string CutOff
        {
            get => cutOff;
            set => this.RaiseAndSetIfChanged(ref cutOff, value);
        }
        private string alarmHours;
        public string AlarmHours
        {
            get => alarmHours;
            set => this.RaiseAndSetIfChanged(ref alarmHours, value);
        }
        private string setSerialNumber;
        public string SetSerialNumber
        {
            get => setSerialNumber;
            set => this.RaiseAndSetIfChanged(ref setSerialNumber, value);
        }
            
            
    }
}
