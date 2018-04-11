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
                                             if (res.Success)
                                             {
                                                 Disconnect();
                                             }
                                         }


                                     }
                                     if (!string.IsNullOrEmpty(AlarmLevel))
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
                                     if (!string.IsNullOrEmpty(CutOff))
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
                                     if (!string.IsNullOrEmpty(AlarmHours))
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
                                             strResult = res.Success ? "OK" : res.ErrorMessage;
                                             strResult = $"{Resources["SetSerialNumberText"]}- {strResult}";
                                             stringBuilder.AppendLine(strResult);
                                             System.Diagnostics.Debug.WriteLine(strResult);
                                             if (res.Success)
                                             {
                                                 Disconnect();
                                             }
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
        private void Disconnect()
        {
            try
            {
                // don't cleanup connection - force user to d/c
                if (this.device.Status != ConnectionStatus.Disconnected)
                {
                    this.device.CancelConnection();
                    
                    if (ParentTabbed != null)
                    {
                        //var connectedPage=ParentTabbed.Children.GetEnumerator().
                        var connectedPage = ParentTabbed.Children.FirstOrDefault(p => p.Title == device.Name);
                        if (connectedPage != null)
                        {
                            ParentTabbed.CurrentPage = ParentTabbed.Children[0];
                            ParentTabbed.Children.Remove(connectedPage);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                App.Dialogs.Alert(ex.ToString());
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
