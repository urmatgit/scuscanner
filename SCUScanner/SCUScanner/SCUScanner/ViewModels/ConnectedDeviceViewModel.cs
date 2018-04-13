using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SCUScanner.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using SCUScanner.Models;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SCUScanner.ViewModels
{
    public class ConnectedDeviceViewModel : BaseViewModel
    {


        IDisposable watcher;
       // IGattCharacteristic gattCharacteristic;
        Color PreviewColor = Color.White;
        public ScanResultViewModel DeviceViewModel { get; set; }
        SCUSendData ScuData { get; set; }
        System.Timers.Timer TimerAlarm;

        //    public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = new ObservableCollection<Group<GattCharacteristicViewModel>>();
        //public ObservableCollection<GattDescriptorViewModel> GattDescriptors { get; } = new ObservableCollection<GattDescriptorViewModel>();
        public TabbedPage ParentTabbed { get; set; }
        readonly IList<IDisposable> cleanup = new List<IDisposable>();
        IDevice device;
        public ICommand DisconnectCommand { get; }
        public ICommand SelectCharacteristic { get; }
        public ICommand SaveCommand { get; }
        public ICommand ValueShareCommand { get; }
        public void Disconnect()
        {

        }
        public ConnectedDeviceViewModel(ScanResultViewModel selectedDevice)
        {
           // HRS = 250;
            AlarmHours = 700;
            TimerAlarm = new System.Timers.Timer();
            TimerAlarm.Interval = 500;
            TimerAlarm.Elapsed += TimerAlarm_Elapsed;
            device = selectedDevice.Device;
            Name = device?.Name;
            OperatorName = SettingsBase.OperatorName;
            DeviceViewModel = selectedDevice;
            IsVisibleLayout = true;
            this.DisconnectCommand = ReactiveCommand.Create(() =>
               {
                   if (App.mainTabbed != null)
                   {
                       App.mainTabbed.ScanPage?.scanBluetoothViewModel.CleanTabPages();
                   }
       
               });
            this.SelectCharacteristic = ReactiveCommand.CreateFromTask<GattCharacteristicViewModel>(async x =>
                                            await x.SelectedGattCharacteristic()
                                            );
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
              {
                  //   return;// пока отключаем
                  if (ScuData == null) return;
                  SCUItem scuitem = null;

                  scuitem = new SCUItem()
                  {
                      UnitName = ScuData.ID,
                      SerialNo = ScuData.SN,

                      BroadCastId = Address,
                      DateWithTime = this.LastValue,
                      Speed = ScuData.S,
                      HoursElapsed = HRS, //Потом из уст.
                      AlarmHours = AlarmHours,//Потом из уст.
                      AlarmSpeed = ScuData.A,
                      Location = LocationName,
                      Notes = Note,
                      Operator = OperatorName
                  };



                  using (App.Dialogs.Loading(Resources["SavingText"]))
                  {
                      var id = await App.Database.SaveItemAsync(scuitem);
                  }



              });
            ValueShareCommand = ReactiveCommand.CreateFromTask(async () =>
             {

                 if (!CrossShare.IsSupported)
                     return;


                 await CrossShare.Current.Share(new ShareMessage
                 {
                     Title = "Reception text",
                     Text = SourceText

                 });

             });
            this.WhenAnyValue(vm => vm.StatusColor).Subscribe(c =>
            {
                if (c == Color.Red || c == Color.Yellow)
                {
                    if (!TimerChangeColor)
                        TimerAlarm.Start();
                }
                else
                    TimerAlarm.Stop();
            });
            this.WhenAnyValue(vm => vm.SN).Subscribe(s => {
                App.mainTabbed.CurrentConnectDeviceSN = s;
                });
            this.WhenAnyValue(vm => vm.Name).Subscribe(val =>
            {
                try
                {
                    DeviceViewModel.Name = val;
                }
                catch (Exception er) { }
            });
            //  StatusColor = Color.Green;
            OnActivateOnLoad();
        }
        Color oldColor = Color.White;
        bool TimerChangeColor = false;
        private void TimerAlarm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerAlarm.Stop();
            if (StatusColor == Color.Red || StatusColor == Color.Yellow || StatusColor == Color.White)
            {
                TimerChangeColor = true;
                Color tmpColor = StatusColor;
                if (tmpColor != Color.White)
                {
                    oldColor = tmpColor;
                    tmpColor = Color.White;
                }
                else
                {
                    tmpColor = oldColor;
                    oldColor = Color.White;
                }
                StatusColor = tmpColor;
                TimerChangeColor = false;
                TimerAlarm.Start();
            }



        }

        string value;
        public string Value
        {
            get => this.value;
            private set => this.RaiseAndSetIfChanged(ref this.value, value);
        }
        DateTime lastValue;
        public DateTime LastValue
        {
            get => this.lastValue;
            private set => this.RaiseAndSetIfChanged(ref this.lastValue, value);
        }
        public string Address
        {
            get => DeviceViewModel.Address;
        }
        string name;
        public string Name
        {
            get => this.name;
            private set => this.RaiseAndSetIfChanged(ref this.name, value);
        }
        Guid uuid;
        public Guid Uuid
        {
            get => this.uuid;
            private set => this.RaiseAndSetIfChanged(ref this.uuid, value);
        }
        int rssi;
        public int Rssi
        {
            get => this.rssi;
            private set => this.RaiseAndSetIfChanged(ref this.rssi, value);
        }
        ConnectionStatus status = ConnectionStatus.Disconnected;
        public ConnectionStatus Status
        {
            get => this.status;
            private set => this.RaiseAndSetIfChanged(ref this.status, value);
        }
        string connectText = "Connected";
        public string ConnectText
        {
            get => this.connectText;
            private set => this.RaiseAndSetIfChanged(ref this.connectText, value);
        }
        private bool isVisibleLayout = false;
        public bool IsVisibleLayout
        {
            get => isVisibleLayout;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isVisibleLayout, value);


            }
        }
        //
        private string deviceID;
        public string DeviceID
        {
            get => deviceID;
            set => this.RaiseAndSetIfChanged(ref deviceID, value);
        }
        private string operatorName;
        public string OperatorName
        {
            get => operatorName;
            set => this.RaiseAndSetIfChanged(ref operatorName, value);
        }
        private string locationName;
        public string LocationName
        {
            get => locationName;
            set => this.RaiseAndSetIfChanged(ref locationName, value);
        }
        private string note;
        public string Note
        {
            get => note;
            set => this.RaiseAndSetIfChanged(ref note, value);
        }
        private int? warning;
        /// <summary>
        /// W – Warning (уровень предупреждения) 
        /// </summary>
        public int? Warning
        {
            get => warning;
            set => this.RaiseAndSetIfChanged(ref warning, value);
        }
        private int rpm;
        /// <summary>
        /// S-Speed текущая скорость вращения мотора, 
        /// </summary>
        public int RPM
        {
            get => rpm;
            set => this.RaiseAndSetIfChanged(ref rpm, value);
        }
        private int? alarmLimit;
        /// <summary>
        /// A -Alarm Level (уровень тревоги), 
        /// </summary>
        public int? AlarmLimit
        {
            get => alarmLimit;
            set => this.RaiseAndSetIfChanged(ref alarmLimit, value);
        }
        private string sn;
        public string SN
        {
            get => sn;
            set => this.RaiseAndSetIfChanged(ref sn, value);
        }
        private int hrs;
        public int HRS
        {
            get => hrs;
            set => this.RaiseAndSetIfChanged(ref hrs, value);
        }

        private int alarmHours;
        public int AlarmHours
        {
            get => alarmHours;
            set => this.RaiseAndSetIfChanged(ref alarmHours, value);
        }
        private Color statusColor;
        public Color StatusColor
        {
            get => statusColor;
            set => this.RaiseAndSetIfChanged(ref statusColor, value);
        }
        private string sourceText;
        public string SourceText
        {
            get => sourceText;
            set => this.RaiseAndSetIfChanged(ref sourceText, value);
        }
        public void OnActivateOnLoad()
        {
            this.cleanup.Clear();
            this.cleanup.Add(this.device
               .WhenStatusChanged()
               .ObserveOn(RxApp.MainThreadScheduler)
               .Subscribe(x =>
               {
                   this.Status = x;

                   switch (x)
                   {
                       case ConnectionStatus.Disconnecting:
                           this.ConnectText = "Disconnecting";
                           break;
                       case ConnectionStatus.Connecting:
                           this.ConnectText = "Connecting";
                           break;

                       case ConnectionStatus.Disconnected:
                           this.ConnectText = Resources["DisconnectStatusText"];
                           using (var dialog = App.Dialogs.Loading(Resources["DisconnectStatusText"]))
                           {
                               App.mainTabbed.CloseConnection();
                           }
                           //  this.GattCharacteristics.Clear();
                           //   this.GattDescriptors.Clear();
                           this.Rssi = 0;
                           break;

                       case ConnectionStatus.Connected:
                           this.ConnectText = Resources["ConnectStatusText"]; // "Disconnect";
                           break;
                   }
               })
           );

            this.cleanup.Add(this.device
                .WhenNameUpdated()
                .Subscribe(x => this.Name = this.device.Name)
            );

            this.cleanup.Add(this.device
                .WhenRssiUpdated()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(rssi => this.Rssi = rssi)
            );
            this.cleanup.Add(this.device
               .WhenServiceDiscovered()
               .Where(c => c.Uuid.ToString() == GlobalConstants.UUID_MLDP_PRIVATE_SERVICE || c.Uuid.ToString() == GlobalConstants.UUID_TANSPARENT_PRIVATE_SERVICE)
               .Subscribe(service =>
               {
                   if (string.IsNullOrEmpty(service.Uuid.ToString())) return;

                   ///TODO filter 
                   //  var group = new Group<GattCharacteristicViewModel>(service.Uuid.ToString());
                   service
                       .WhenCharacteristicDiscovered()
                       // .Where(c=> InListCharacters(c.Uuid.ToString()))
                       .ObserveOn(RxApp.MainThreadScheduler)
                       .Subscribe(character =>
                       {
                           var strUuid = character.Uuid.ToString();
                           if (InListCharacters(character.Uuid.ToString()))
                           {
                               App.mainTabbed.SelectedCharacteristic = character;
                               //gattCharacteristic = character;
                               NotifyEnable(character);

                           }

                       });
               })
               );
        }
        private async void NotifyDisable(IGattCharacteristic character) {
            await App.mainTabbed.SelectedCharacteristic?.DisableNotifications();
        }
        private void NotifyEnable(IGattCharacteristic character)
        {
            Device.BeginInvokeOnMainThread(() =>
            {

                if (character.CanNotify())
                {


                    if (this.watcher != null)
                    {
                        this.watcher.Dispose();
                        this.watcher = null;
                        App.mainTabbed.SelectedCharacteristic = null;
                    }
                    character.EnableNotifications().Subscribe();
                    this.watcher = character.WhenNotificationReceived().Subscribe(
                        x => GetValue(x)
                        );

                }




            });
        }
        private bool InListCharacters(string uuid)
        {

            if (uuid.Equals(GlobalConstants.UUID_MLDP_DATA_PRIVATE_CHAR) || uuid.Equals(GlobalConstants.UUID_TRANSPARENT_RX_PRIVATE_CHAR) || uuid.Equals(GlobalConstants.UUID_TRANSPARENT_TX_PRIVATE_CHAR))
                return true;
            return false;

        }
        private bool IsStartedJson = false;
        private string StrJson = "";
        private void GetValue(CharacteristicGattResult readresult)
        {
            this.LastValue = DateTime.Now;
            if (App.mainTabbed.SelectedCharacteristic == null)
                App.mainTabbed.SelectedCharacteristic = readresult.Characteristic;
            ScuData = null;
            if (!readresult.Success)
                this.SourceText = "ERROR - " + readresult.ErrorMessage;

            else if (readresult.Data == null)
                this.SourceText = "EMPTY";

            else
            {
                this.Value = Encoding.UTF8.GetString(readresult.Data, 0, readresult.Data.Length);

                if (Value.StartsWith("{"))
                {
                    this.SourceText = this.Value;
                    IsStartedJson = true;
                    StrJson = this.Value;
                }
                else if (IsStartedJson)
                {
                    this.SourceText += this.Value;
                    StrJson += this.Value;
                }
                else
                    StrJson = "";
                Debug.Write(this.SourceText);
                //RPM = null;
                //AlarmLimit = null;
                StrJson =  StrJson.Trim();
                if (IsStartedJson && StrJson.EndsWith("}"))
                {
                    try
                    {
                        StrJson = Regex.Replace(StrJson, "\"ID\":(.[^,]+)", "\"ID\":\"$1\"");
                        StrJson = Regex.Replace(StrJson, "\"S:(.[^,]+)", "\"S\":$1");
                        StrJson = Regex.Replace(StrJson, "\"SN\":(.[^,]+)", "\"SN\":\"$1\"").Replace("%", "pc");
                        //StrJson = StrJson
                        //    .Replace("\"ID\":", "\"ID\":\"")
                        //    .Replace(",\"SN\":", "\",\"SN\":\"")
                        //    .Replace(",\"C\":", "\",\"C\":")
                        //    .Replace("%", "pc");

                        ScuData = JsonConvert.DeserializeObject<SCUSendData>(StrJson);
                        Debug.WriteLine($"\nScuData-{StrJson}");

                    }
                    catch (Exception er)
                    {
                        //App.Dialogs.Alert("Deserialize data error- \n" + er.Message);
                        

                        ScuData = null;
                    }
                    finally
                    {
                        StrJson = "";
                        IsStartedJson = false;
                    }
                    if (ScuData != null)
                    {
                        RPM = ScuData.S;
                        AlarmLimit = ScuData.A;
                        SN = ScuData.SN;
                        HRS = ScuData.H;
                        Warning = ScuData.W;

                        var tmpNewColor = ChangeStatusColor(RPM, Warning, AlarmLimit);
                        if (PreviewColor != tmpNewColor)
                        {
                            try
                            {
                                StatusColor = tmpNewColor;
                                PreviewColor = tmpNewColor;
                            }
                            catch (Exception er)
                            {
                                Debug.WriteLine($"Status color chage error-{er.Message}");
                            }

                        }
                    }

                }


            }
        }
        private Color ChangeStatusColor(int s, int? w, int? a)
        {
            if (s > w) return Color.Green;
            if (a < s && s <= w) return Color.Yellow;
            if (s <= a) return Color.Red;
            return Color.Red;
        }

        public void Dispose()
        {
            if (App.mainTabbed.SelectedCharacteristic != null)
            {
                App.mainTabbed.SelectedCharacteristic.DisableNotifications();
                App.mainTabbed.SelectedCharacteristic = null;
            }
            device.CancelConnection();
            DeviceViewModel.IsConnected = false;
            TimerAlarm.Stop();
            TimerAlarm.Dispose();
            
            this.device = null;
            foreach (var item in this.cleanup)
                item.Dispose();
            this.watcher = null;
           // App.mainTabbed.SelectedCharacteristic = null;
        }
        public override void OnDeactivate()
        {
            SettingsBase.OperatorName=OperatorName;
            base.OnDeactivate();
            
            //foreach (var item in this.cleanup)
            //    item.Dispose();

        }
    }
}
