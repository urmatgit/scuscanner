using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
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
                         using (App.Dialogs.Loading(Resources["SendingValueText"]))
                         {
                             CharacteristicGattResult res = null;
                             string strResult = "";

                             System.Diagnostics.Debug.WriteLine("Start sending value");
                             if (!string.IsNullOrEmpty(BroadcastIdentity)){
                                 res = await WriteToDevice($"!{BroadcastIdentity}");
                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                     strResult = $"{Resources["BroadcastIdentityText"]}- {strResult}";
                                     stringBuilder.AppendLine(strResult);
                                     System.Diagnostics.Debug.WriteLine(strResult);
                              
                             }
                             if (AlarmLevel!=null)
                             {
                                 res = await WriteToDevice($"^{AlarmLevel}");
                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                 strResult = $"{Resources["AlarmLevelText"]}- {strResult}";
                                 stringBuilder.AppendLine(strResult);
                                 System.Diagnostics.Debug.WriteLine(strResult);
                             }
                             if (CutOff != null)
                             {
                                 res = await WriteToDevice($"@{CutOff}");
                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                 strResult = $"{Resources["CutOffText"]}- {strResult}";
                                 stringBuilder.AppendLine(strResult);
                                 System.Diagnostics.Debug.WriteLine(strResult);
                             }
                             if (AlarmHours != null)
                             {
                                 res = await WriteToDevice($"~{AlarmHours}");
                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                 strResult = $"{Resources["AlarmHoursText"]}- {strResult}";
                                 stringBuilder.AppendLine(strResult);
                                 System.Diagnostics.Debug.WriteLine(strResult);
                             }
                             if (!string.IsNullOrEmpty(SetSerialNumber))
                             {
                                 res = await WriteToDevice($"${SetSerialNumber}");
                                 strResult = res.Success ? "OK" : res.ErrorMessage;
                                 strResult = $"{Resources["SetSerialNumberText"]}- {strResult}";
                                 stringBuilder.AppendLine(strResult);
                                 System.Diagnostics.Debug.WriteLine(strResult);
                             }
                         }
                         await App.Dialogs.AlertAsync(stringBuilder.ToString());
                     }
                 }
             });
            }
        private async Task<CharacteristicGattResult> WriteToDevice(string str)
        {
            CharacteristicGattResult result = null;
            
                byte[] bytes = Encoding.UTF8.GetBytes(str);

                  result =  await App.mainTabbed.SelectedCharacteristic.Write(bytes)
                                    .Timeout(TimeSpan.FromSeconds(5))
                                    .ToTask();
                
            
            return result;
        }
        private string broadcastIdentity;
        public string BroadcastIdentity
        {
            get => broadcastIdentity;
             set => this.RaiseAndSetIfChanged(ref broadcastIdentity, value);
        }
        private int? alarmLevel;
        public int? AlarmLevel
        {
            get => alarmLevel;
            set => this.RaiseAndSetIfChanged(ref alarmLevel, value);
        }
        private int? cutOff;
        public int? CutOff
        {
            get => cutOff;
            set => this.RaiseAndSetIfChanged(ref cutOff, value);
        }
        private int? alarmHours;
        public int? AlarmHours
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
