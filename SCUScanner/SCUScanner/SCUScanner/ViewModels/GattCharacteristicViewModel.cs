using Acr;
using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class GattCharacteristicViewModel : BaseViewModel
    {
        IDevice BleDevice;
        IDisposable watcher;
        public IGattCharacteristic Characteristic { get; }
        public ICommand SendCommand { get; }
        public ICommand ReadCommand { get; }
        string valueToWrite;
        bool? IsDisplayUtf8 = null;
        public string ValueToWrite
        {
            get => this.valueToWrite;
            set => this.RaiseAndSetIfChanged(ref this.valueToWrite, value);
        }
        public async  Task<GattCharacteristicViewModel> SelectedGattCharacteristic(bool? isDisplayUtf8=null)
        {
            IsDisplayUtf8 = isDisplayUtf8;
            if (CanRead)
            {
                
                var value = await Characteristic
                      .Read()
                      //.Timeout(TimeSpan.FromSeconds(3))
                      .ToTask();

                if (IsDisplayUtf8==null)
                    IsDisplayUtf8 = await App.Dialogs.ConfirmAsync("Display Value as UTF8 or HEX?", okText: "UTF8", cancelText: "HEX");
                //if (BleDevice.Features.HasFlag(DeviceFeatures.MtuRequests))
                //{
                //    var actual = await BleDevice.RequestMtu(512);
                //}
                this.SetReadValue(this, value, IsDisplayUtf8.Value);

            }
            if (CanNotify)
            {

                if (IsDisplayUtf8 == null)
                    IsDisplayUtf8 = await App.Dialogs.ConfirmAsync(
                           "Display Value as UTF8 or HEX?",
                           okText: "UTF8",
                           cancelText: "HEX"
                       );
                this.watcher = Characteristic
                    .RegisterAndNotify()
                    .Subscribe(x =>
                    {
                        
                        this.SetReadValue(this, x, IsDisplayUtf8 ?? true);
                    });


            }
            return this;
        }
        public GattCharacteristicViewModel(IGattCharacteristic characteristic,IDevice device)
        {
            BleDevice = device;
            Characteristic = characteristic;
            ReadCommand = ReactiveCommand.CreateFromTask(async () => {
                await SelectedGattCharacteristic();
            });
            this.SendCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                string Uuid = Characteristic.Uuid.ToString();
                if (!String.IsNullOrWhiteSpace(ValueToWrite))
                {
                    var utf8 = await App.Dialogs.ConfirmAsync("Write value from UTF8 or HEX?", okText: "UTF8", cancelText: "HEX");
                    try
                    {
                        using (App.Dialogs.Loading("Writing Value..."))
                        {
                            var value = ValueToWrite.Trim();
                            var bytes = utf8 ? Encoding.UTF8.GetBytes(value) : new byte[] { byte.Parse(value) };
                            if (Characteristic.CanWriteWithResponse())
                            {
                                await Characteristic
                                    .Write(bytes)
                                    .Timeout(TimeSpan.FromSeconds(5))
                                    .ToTask();
                            }
                            else
                            {
                                Characteristic.WriteWithoutResponse(bytes);
                            }

                            this.Value = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        App.Dialogs.Alert($"Error Writing {Uuid} - {ex}");
                    }
                }
            });
        }
        public Guid Uuid => this.Characteristic.Uuid;
        public Guid ServiceUuid => this.Characteristic.Service.Uuid;
        public string Description => this.Characteristic.Description;
        public string Properties => this.Characteristic.Properties.ToString();
        public bool CanNotify => this.Characteristic.CanNotify();
        string value;
        public string Value
        {
            get => this.value;
             set => this.RaiseAndSetIfChanged(ref this.value, value);
        }
       
        public bool CanRead
        {
            get => this.Characteristic.CanRead();
        }
        public bool CanWrite {
            get => this.Characteristic.CanWrite();
        }
       
        bool notifying;
        public bool IsNotifying
        {
            get => this.notifying;
            private set => this.RaiseAndSetIfChanged(ref this.notifying, value);
        }


        bool valueAvailable;
        public bool IsValueAvailable
        {
            get => this.valueAvailable;
            private set => this.RaiseAndSetIfChanged(ref this.valueAvailable, value);
        }


        DateTime lastValue;
        public DateTime LastValue
        {
            get => this.lastValue;
             set => this.RaiseAndSetIfChanged(ref this.lastValue, value);
        }

        void SetReadValue(GattCharacteristicViewModel selectedGatt, CharacteristicGattResult result, bool fromUtf8) => Device.BeginInvokeOnMainThread(() =>
        {


            this.LastValue = DateTime.Now;
            selectedGatt.LastValue = DateTime.Now;
            
            if (!result.Success)
                this.Value = "ERROR - " + result.ErrorMessage;

            else if (result.Data == null)
                this.Value = "EMPTY";

            else
                this.Value = fromUtf8
                    ? Encoding.UTF8.GetString(result.Data, 0, result.Data.Length)
                    : ByteToString(result.Data);
            //BitConverter.ToString(result.Data);
            selectedGatt.Value = this.Value;
        });
        private string ByteToString(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
                stringBuilder.AppendFormat("{0} ", b.ToString());
            return stringBuilder.ToString();
        }
    }
}
