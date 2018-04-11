using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;

using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SCUScanner.ViewModels
{
    public class DeviceSettingViewModel: BaseViewModel
    {
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
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }


                                     }
                                     if (AlarmLevel != null)
                                     {
                                         res = await WriteToDevice($"^{AlarmLevel}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["AlarmLevelText"]}- {strResult}";
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (CutOff != null)
                                     {
                                         res = await WriteToDevice($"@{CutOff}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["CutOffText"]}- {strResult}";
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (AlarmHours != null)
                                     {
                                         var strAlarmHours = AlarmHours.ToString();
                                         res = await WriteToDevice($"~{strAlarmHours.PadLeft(4, '0')}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["AlarmHoursText"]}- {strResult}";
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                         }
                                     }
                                     if (!string.IsNullOrEmpty(SetSerialNumber))
                                     {
                                         res = await WriteToDevice($"${SetSerialNumber}", cancelSrc);
                                         if (res != null)
                                         {
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["SetSerialNumberText"]}- {strResult}";
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
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
                         if (stringBuilder.Length>0)
                            await App.Dialogs.AlertAsync(stringBuilder.ToString());
                     }
                 }
             });
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
