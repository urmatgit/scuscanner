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

namespace SCUScanner.ViewModels
{
    public class ConnectedDeviceViewModel : BaseViewModel
    {

        IDisposable watcher;
        public ScanResultViewModel DeviceViewModel {get;set;}
        SCUSendData ScuData { get; set; }
        System.Timers.Timer TimerAlarm;
        object forLock;
        //    public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = new ObservableCollection<Group<GattCharacteristicViewModel>>();
        //public ObservableCollection<GattDescriptorViewModel> GattDescriptors { get; } = new ObservableCollection<GattDescriptorViewModel>();
        public TabbedPage ParentTabbed { get; set; }
        readonly IList<IDisposable> cleanup = new List<IDisposable>();
        IDevice device;
        public ICommand DisconnectCommand { get; }
        public ICommand SelectCharacteristic { get; }
        public ICommand SaveCommand { get; }
        public ConnectedDeviceViewModel(ScanResultViewModel selectedDevice)
        {
            HRS = 250;
            AlarmHours = 700;
            TimerAlarm = new System.Timers.Timer();
            TimerAlarm.Interval = 500;
            TimerAlarm.Elapsed += TimerAlarm_Elapsed;
            device = selectedDevice.Device;
            Name = device?.Name;
            DeviceViewModel = selectedDevice;
            IsVisibleLayout = true;
            this.DisconnectCommand = ReactiveCommand.Create(() =>
               {
                   try
                   {
                       // don't cleanup connection - force user to d/c
                       if (this.device.Status != ConnectionStatus.Disconnected)
                       {
                           this.device.CancelConnection();
                           selectedDevice.IsConnected = false;
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
               });
            this.SelectCharacteristic = ReactiveCommand.CreateFromTask <GattCharacteristicViewModel>(async x =>
                                             await x.SelectedGattCharacteristic()
                                            );
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
              {
                  return;// пока отключаем
                  if (ScuData == null) return;
                  SCUItem scuitem = null;
                  
                           scuitem = new SCUItem()
                          {
                              ID = ScuData.ID,
                              MacAddress=Address,
                              DateWithTime= this.LastValue,
                              Speed=ScuData.S,
                              Location=LocationName,
                              Comment=Note,
                              Operator=""
                           };



                  using (App.Dialogs.Loading(Resources["SavingText"]))
                  {
                      var id = await App.Database.SaveItemAsync(scuitem);
                  }
                      


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
          //  StatusColor = Color.Green;
        }
        Color oldColor=Color.White;
        bool TimerChangeColor = false;
        private void TimerAlarm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerAlarm.Stop();
            if (StatusColor==Color.Red || StatusColor == Color.Yellow || StatusColor == Color.White)
            {
                TimerChangeColor = true;
                if (StatusColor != Color.White)
                {
                    oldColor = StatusColor;
                    StatusColor = Color.White;
                }
                else
                {
                    StatusColor = oldColor;
                    oldColor = Color.White;
                }
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
        private int warning;
        /// <summary>
        /// W – Warning (уровень предупреждения) 
        /// </summary>
        public int Warning
        {
            get => warning;
            set => this.RaiseAndSetIfChanged(ref warning, value);
        }
        private int? rpm;
        /// <summary>
        /// S-Speed текущая скорость вращения мотора, 
        /// </summary>
        public int? RPM 
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
        private string errorMsg;
        public string ErrorMsg
        {
            get => errorMsg;
            set => this.RaiseAndSetIfChanged(ref errorMsg, value);
        }
        public override void OnActivate()
        {
            base.OnActivate();
            var count = cleanup.Count;
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
               .Where(c=>c.Uuid.ToString()==GlobalConstants.UUID_MLDP_PRIVATE_SERVICE || c.Uuid.ToString()==GlobalConstants.UUID_TANSPARENT_PRIVATE_SERVICE)
               .Subscribe(service =>
               {
                   if (string.IsNullOrEmpty(service.Uuid.ToString())) return; 
                   ///TODO filter 
                 //  var group = new Group<GattCharacteristicViewModel>(service.Uuid.ToString());
                   service
                       .WhenCharacteristicDiscovered()
                       .Where(c=> InListCharacters(c.Uuid.ToString()))
                       .ObserveOn(RxApp.MainThreadScheduler)
                       .Subscribe(character =>
                       {
                            
                           
                           Device.BeginInvokeOnMainThread(() =>
                           {

                               if (character.CanRead())
                               {
                                   //Task.Run(async () =>
                                   //{
                                   var result = character.Read(). Subscribe(x=>// ReadUntil(Encoding.UTF8.GetBytes("}")).Subscribe(x =>
                                   {
                                       GetValue(x);
                                   });
                                   //});

                               }
                               if (character.CanNotify())
                               {
                                   
                                   
                                   this.watcher = character
                                    .RegisterAndNotify()
                                    .Subscribe(x =>
                                        {
                                            GetValue(x);
                                        });
                               }

                               //var vm = new GattCharacteristicViewModel(character,device);

                               //if (vm.CanRead || vm.CanNotify)
                               //{
                               //    Task.Run( async () =>
                               //    {
                               //        await vm.SelectedGattCharacteristic(true);
                               //    });
                               //}
                               //MDLCharacteristicViewModel = vm;
                             //  group.Add(vm);
                               ////if (group.Count == 1)
                               //var gr = this.GattCharacteristics.FirstOrDefault(g => g.Name == group.Name);
                               //if (gr == null)
                               //    this.GattCharacteristics.Add(group);
                               //else
                               //    this.GattCharacteristics[this.GattCharacteristics.IndexOf(gr)] = group;


                           });

                           //if (group.Count == 1 && this.GattCharacteristics.FirstOrDefault(g=>g.Name== group.Name)==null)
                           //    this.GattCharacteristics.Add(group);

                           //character
                           //    .WhenDescriptorDiscovered()
                           //    .Subscribe(desc => Device.BeginInvokeOnMainThread(() =>
                           //    {
                           //        this.GattCharacteristics.add
                           //        var dvm = new GattDescriptorViewModel(this.Dialogs, desc);
                           //        this.GattDescriptors.Add(dvm);
                           //    }));
                       });
               })
               );
        }
        private bool InListCharacters(string uuid)
        {
            
            if (uuid.Equals(GlobalConstants.UUID_MLDP_DATA_PRIVATE_CHAR) || uuid.Equals(GlobalConstants.UUID_TRANSPARENT_RX_PRIVATE_CHAR) || uuid.Equals(GlobalConstants.UUID_TRANSPARENT_TX_PRIVATE_CHAR))
                return true;
            return false;
                
        }
         private void GetValue(CharacteristicGattResult readresult)
        {
            this.LastValue = DateTime.Now;
             
                ScuData = null;
            if (!readresult.Success)
                  //App.Dialogs.AlertAsync("ERROR - " + readresult.ErrorMessage);
                    this.ErrorMsg = "ERROR - " + readresult.ErrorMessage;

                else if (readresult.Data == null)
                    this.ErrorMsg = "EMPTY";

                else
                {
                    this.Value = Encoding.UTF8.GetString(readresult.Data, 0, readresult.Data.Length);
                    //RPM = null;
                    //AlarmLimit = null;
                //if (!string.IsNullOrEmpty(this.Value))
                //{
                //    try
                //    {
                //        //
                //        string val = this.Value;
                //        if (!string.IsNullOrEmpty(val))
                //        {
                //            val = val
                //                .Replace("\"ID\":", "\"ID\":\"")
                //                .Replace(",\"SN\":", "\",\"SN\":\"")
                //                .Replace(",\"C\":", "\",\"C\":");
                //            ScuData = JsonConvert.DeserializeObject<SCUSendData>(val);
                //        }

                //    }
                //    catch (Exception er)
                //    {
                //        App.Dialogs.Alert("Deserialize datat error- \n" + er.Message);
                //    }
                //    RPM = ScuData.S;
                //    AlarmLimit = ScuData.A;
                //    SN = ScuData.SN;
                //    Warning = ScuData.W;
                //    StatusColor = ChangeStatusColor(RPM, Warning, AlarmLimit);
                  
                //}

               
            }
        }
        private Color ChangeStatusColor (int? s, int w, int? a) 
         {
            if (s > w) return Color.Green;
            if (a < s && s <= w) return Color.Yellow;
            if (s <= a) return Color.Red;
            return Color.Red;
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            foreach (var item in this.cleanup)
                item.Dispose();
            
        }
    }
}
