using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Commands;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Plugin.Share;
using Plugin.Share.Abstractions;
using ReactiveUI;
using SCUScanner.Helpers;
using SCUScanner.Models;
using SCUScanner.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{

    public class DeviceListViewModel : BaseViewModel
    {
        DeviceListPage _deviceListPage;
        DeviceListItemViewModel SelectedDevice;
        private readonly IBluetoothLE _bluetoothLe;
        private readonly IUserDialogs _userDialogs;
        private readonly ISettings _settings;
        private readonly IAdapter Adapter;
        ICharacteristic mldpDataCharacteristic, transparentTxDataCharacteristic, transparentRxDataCharacteristic;
        private bool isRefreshing; //= Adapter.IsScanning;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => this.RaiseAndSetIfChanged(ref isRefreshing, value);

        }
        bool isStateOn;

        public bool IsStateOn
        {
            get => isStateOn;
            set => this.RaiseAndSetIfChanged(ref isStateOn, value);
        }
        bool isConnected;

        public bool IsConnected
        {
            get => isConnected;
            set => this.RaiseAndSetIfChanged(ref isConnected, value);
        }
        private bool _flgWaitDataUpate = false;
        public bool flgWaitDataUpate
        {
            get=> _flgWaitDataUpate;
            set => this.RaiseAndSetIfChanged(ref _flgWaitDataUpate, value);
        }
        private CancellationTokenSource _cancellationTokenSource;
        public ICommand ScanToggleCommand { get; }
        public ICommand StopScanCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand ValueShareCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand WriteCommnad { get; }
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        readonly IPermissions _permissions;

        public DeviceListViewModel(DeviceListPage deviceListPage, IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions)
        {

            TimerAlarm = new System.Timers.Timer();
            TimerAlarm.Interval = 500;
            TimerAlarm.Elapsed += TimerAlarm_Elapsed;
            TimerWaitDataUpdate = new System.Timers.Timer();
            TimerWaitDataUpdate.Interval = GlobalConstants.WaitingForReconnecting;
            TimerWaitDataUpdate.Elapsed += TimerWaitDataUpdate_Elapsed;
            this.WhenAnyValue(vm => vm.flgWaitDataUpate).Subscribe(flg =>
              {
                  TimerWaitDataUpdate.Enabled = flg;
                  //if (flg)
                  //{
                  //    TimerWaitDataUpdate.Start();
                  //}else
                  //{
                  //    TimerWaitDataUpdate.Stop();
                  //}
                  
              });
            _deviceListPage = deviceListPage;
            OperatorName = SettingsBase.OperatorName;
            UpdateScanText(false);
            Adapter = adapter;
            _permissions = permissions;
            _bluetoothLe = bluetoothLe;
            _userDialogs = userDialogs;
            _settings = settings;
            this.WhenAnyValue(vm => vm.IsRefreshing).Subscribe(r =>
            {
                UpdateScanText(r);
                string ONOF = r ? "On" : "Off";
                _userDialogs.Toast($"Scanning  {ONOF}");
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


            IsRefreshing = Adapter.IsScanning;
            IsStateOn = _bluetoothLe.IsOn;
            ScanToggleCommand = ReactiveCommand.Create(() => TryStartScanning(true));
            StopScanCommand = ReactiveCommand.Create(() => StopScan());
            DisconnectCommand = ReactiveCommand.CreateFromTask(async () =>
            {

                await DisconnectDevice(SelectedDevice);

            });
            ConnectCommand = ReactiveCommand.CreateFromTask<DeviceListItemViewModel>(async (o) =>
            {
                if (!IsStateOn) return;
                //                 var selecte = o;
                SelectedDevice = o;
                if (SelectedDevice.IsConnected)
                {

                    await DisconnectDevice(SelectedDevice);

                }
                else
                {
                    IsConnected = await ConnectDeviceAsync(SelectedDevice, false);
                    if (IsConnected)
                    {
                        Name = SelectedDevice.Name;
                        _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;

                        await LoadServices(SelectedDevice);
                    }
                }
            });
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                //   return;// пока отключаем
                if (ScuData == null) return;
              


                SCUItem scuitem = new SCUItem()
                {
                    UnitName = ScuData.ID,
                    SerialNo = ScuData.SN,

                    BroadCastId = ScuData.ID,
                    DateWithTime = this.LastValue,
                    Speed = ScuData.S,
                    HoursElapsed = HRS, //Потом из уст.
                    AlarmHours = AlarmHours,//Потом из уст.
                    AlarmSpeed = ScuData.A,
                    Location = LocationName,
                    Notes = Note,
                    Operator = OperatorName
                };


                var id = await App.Database.SaveItemAsync(scuitem);
                if (id > 0)
                {
                    _userDialogs.Toast("Saved");
                }
                else
                {
                    Debug.WriteLine($"Not saved {scuitem.Id}");
                }

                ScuData = null;
            });
            WriteCommnad = ReactiveCommand.CreateFromTask<string>(async (kod) =>
            {
                await WriteToDevice(kod);
            });
            ValueShareCommand = ReactiveCommand.CreateFromTask(async () =>
            {


                if (!CrossShare.IsSupported && string.IsNullOrEmpty(LastJsonForShare))
                
                    return;


                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = "Reception text",
                    Text = SourceText

                });

            });
            _bluetoothLe.StateChanged += OnStateChanged;

            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;

            UpdateStateText();
            IsConnected = false;
            Adapter.DeviceDisconnected += OnDeviceDisconnected;
            Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            // quick and dirty :>

            //Adapter.DeviceConnected += (sender, e) => Adapter.DisconnectDeviceAsync(e.Device);

        }

        private async void TimerWaitDataUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimerWaitDataUpdate.Stop();
            if (flgWaitDataUpate)
            {
                flgWaitDataUpate = false;
                await DisconnectDevice(SelectedDevice, true);
            }
        }

        private async void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();

            await StopUpdates();
            await DisconnectDevice(SelectedDevice);
            _userDialogs.HideLoading();
            _userDialogs.Toast($"Connection LOST {e.Device.Name}", TimeSpan.FromMilliseconds(6000));

        }
        private async void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            await StopUpdates();
            _userDialogs.HideLoading();
            _userDialogs.Toast($"Disconnected {e.Device.Name}");
        }
        #region Device list page
        private void UpdateStateText()
        {
            StateText = GetStateText();
        }

        private void UpdateScanText(bool r)
        {
            if (r)
            {
                ScanText = Resources["StopScanText"];
            }
            else
                ScanText = Resources["ScanText"];
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {

            UpdateIsScanning();
            CleanupCancellationToken();
        }
        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            AddOrUpdateDevice(args.Device);
        }
        private string statetext;
        public string StateText
        {
            get => statetext;
            set => this.RaiseAndSetIfChanged(ref statetext, value);
        }
        private string GetStateText()
        {
            switch (_bluetoothLe.State)
            {
                case BluetoothState.Unknown:
                    return "Unknown BLE state.";
                case BluetoothState.Unavailable:
                    return "BLE is not available on this device.";
                case BluetoothState.Unauthorized:
                    return "You are not allowed to use BLE.";
                case BluetoothState.TurningOn:
                    return "BLE is warming up, please wait.";
                case BluetoothState.On:
                    return "BLE is on.";
                case BluetoothState.TurningOff:
                    return "BLE is turning off. That's sad!";
                case BluetoothState.Off:
                    if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.Android)
                        return Models.Settings.Current.Resources["SetHintBluetoothAndroidText"];
                    else
                        return Models.Settings.Current.Resources["SetHintBluetoothIOSText"];
                default:
                    return "Unknown BLE state.";
            }
        }
        private void UpdateStateOn()
        {
            IsStateOn = _bluetoothLe.IsOn;
            UpdateStateText();
        }
        private void UpdateIsScanning()
        {
            IsRefreshing = Adapter.IsScanning;

        }
        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {

            UpdateStateOn();
            UpdateStateText();

            //RaisePropertyChanged(nameof(StateText));
            //TryStartScanning();
        }
        private void StopScan()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                CleanupCancellationToken();
                IsRefreshing = Adapter.IsScanning;
            }

        }

        string scantext;
        private bool _updatesStarted;

        public string ScanText
        {
            get => scantext;
            set => this.RaiseAndSetIfChanged(ref this.scantext, value);
        }

        public override async void OnActivate(string kod)
        {
            base.OnActivate();
            UpdateIsScanning();
            UpdateScanText(IsRefreshing);
            UpdateStateText();
            if (IsStateOn && !string.IsNullOrEmpty(kod))
            {
                if (kod == "MainTabPage" && !IsRefreshing && SettingsBase.AutoScan)
                {
                    await TryStartScanning(true);
                }
                else if (kod == "ConnectedTabPage")
                {
                    if (LastCharForUpdate != null)
                    {
                        StartUpdates(LastCharForUpdate);
                          TimerAlarm.Enabled = true;
                    }
                }
            }
        }
        public override async void OnDeactivate(string kod)
        {
            if (!string.IsNullOrEmpty(kod))
            {
                if (kod == "MainTabPage")
                {
                   await  Adapter.StopScanningForDevicesAsync();
                    UpdateIsScanning();
                }
                else if (kod == "ConnectedTabPage")
                {
                   await  StopUpdates();
                    SettingsBase.OperatorName = OperatorName;
                    
                }
            }
            base.OnDeactivate();


        }


        private async Task TryStartScanning(bool refresh = false)
        {

            if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.Android)
            {
                var status = await _permissions.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var permissionResult = await _permissions.RequestPermissionsAsync(Permission.Location);

                    if (permissionResult.First().Value != PermissionStatus.Granted)
                    {
                        await _userDialogs.AlertAsync(Resources["InfoPermissionLocationText"]);
                        return;
                    }
                }
            }

            if (IsStateOn && (refresh || !Devices.Any()))
            {
                if (!IsRefreshing)
                {
                    //     ScanText = Resources["StopScanText"];
                    IsRefreshing = true;
                    ScanForDevices();
                }
                else
                {
                    StopScan();
                }
            }
        }

        private async void ScanForDevices()
        {
          
            while (this.Devices.Any())
            {
                this.Devices.RemoveAt(0);
            }

            foreach (var connectedDevice in Adapter.ConnectedDevices)
            {
                //update rssi for already connected evices (so tha 0 is not shown in the list)
                try
                {
                    await connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    //Mvx.Trace(ex.Message);
                    await _userDialogs.AlertAsync($"Failed to update RSSI for {connectedDevice.Name}");
                }

                AddOrUpdateDevice(connectedDevice);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            //RaisePropertyChanged(() => StopScanCommand);

            //RaisePropertyChanged(() => IsRefreshing);
            Adapter.ScanMode = ScanMode.LowLatency;
            Adapter.ScanTimeout = 30000;
            await Adapter.StartScanningForDevicesAsync(cancellationToken: _cancellationTokenSource.Token);
        }
        private void AddOrUpdateDevice(IDevice device)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var vm = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
                if (vm != null)
                {
                    vm.Update();
                }
                else
                {
                    Devices.Add(new DeviceListItemViewModel(device));
                }
            });
        }
        private void CleanupCancellationToken()
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            UpdateIsScanning();
            //if (!isRefreshing)
            //    ScanText = Resources["ScanText"];
        }
        #endregion
        #region Connect to device

        private async Task<bool> ConnectDeviceAsync(DeviceListItemViewModel device, bool showPrompt = true)
        {

            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                var config = new ProgressDialogConfig()
                {
                    Title = $"{Resources["ConnectingText"] }  ({device.Name})",
                    CancelText = Resources["CancelText"],
                    IsDeterministic = false,
                    OnCancel = tokenSource.Cancel
                };
                //
                StopScan();
                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();

                    await Adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: false, forceBleTransport: false), tokenSource.Token);
                }

                _userDialogs.Toast($"{SettingsBase.Resources["ConnectStatusText"]}  {device.Name}.");

                //PreviousGuid = device.Device.Id;
                return true;

            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Connection error");

                return false;
            }
            finally
            {
                _userDialogs.HideLoading();
                device.Update();
            }
        }
        private async Task DisconnectDevice(DeviceListItemViewModel device, bool autoconnect = false)
        {
            await StopAndClearCharacters();
            if (device.IsConnected)
            {
                try
                {



                    _userDialogs.ShowLoading($"{SettingsBase.Resources["DisConnectButtonText"]} {device.Name}...");

                    await Adapter.DisconnectDeviceAsync(device.Device);
                    _userDialogs.Toast($"{SettingsBase.Resources["DisconnectedStatusText"]} {device.Name}");

                }
                catch (Exception ex)
                {
                    _userDialogs.Alert(ex.Message, "Disconnect error");
                }
                finally
                {
                    Debug.WriteLine($"New device name -{device.Name}");
                    device.Update();

                    _userDialogs.HideLoading();
                }
            }
           

            IsConnected = false;

            if (autoconnect)
            {
                Thread.Sleep(5000);
                //await TryStartScanning(true);
                if (SelectedDevice != null)
                {
                    ConnectCommand.Execute(SelectedDevice);
                }
            }
            else if (_deviceListPage.CurrentPage != _deviceListPage.DeviceListTab)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _deviceListPage.CurrentPage = _deviceListPage.DeviceListTab;
                });

             
            }
        
        }

        #endregion
        #region Find service
        private async Task LoadServices(DeviceListItemViewModel device)
        {
            try
            {
              // _userDialogs.ShowLoading("Discovering services...");

                var Services = await device.Device.GetServicesAsync();
                var SCUServices = Services.Where(s => s.Id.ToString() == GlobalConstants.UUID_MLDP_PRIVATE_SERVICE || s.Id.ToString() == GlobalConstants.UUID_TANSPARENT_PRIVATE_SERVICE);
                //mldpDataCharacteristic, transparentTxDataCharacteristic, transparentRxDataCharacteristic;
                foreach (var service in SCUServices)
                {
                    Debug.WriteLine(service.Id.ToString());
                    transparentTxDataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(GlobalConstants.UUID_TRANSPARENT_TX_PRIVATE_CHAR));
                    if (transparentRxDataCharacteristic != null)
                    {
                        if (transparentRxDataCharacteristic.CanUpdate)
                        {

                        }
                    }
                    transparentTxDataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(GlobalConstants.UUID_TRANSPARENT_TX_PRIVATE_CHAR));
                    if (transparentTxDataCharacteristic != null)
                    {
                        if (transparentTxDataCharacteristic.CanUpdate)
                        {
                            //    StartUpdates(mldpDataCharacteristic);
                        }
                    }
                    var _mldpDataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(GlobalConstants.UUID_MLDP_DATA_PRIVATE_CHAR));
                    if (_mldpDataCharacteristic != null && mldpDataCharacteristic == null)
                    {
                        if (_mldpDataCharacteristic.CanUpdate)
                        {
                            // mldpDataCharacteristic.ValueUpdated
                            mldpDataCharacteristic = _mldpDataCharacteristic;
                            StartUpdates(mldpDataCharacteristic);

                        }
                    }
                }
             //   _userDialogs.HideLoading();
                var finded = SCUServices.Count();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Error while discovering services");
                Debug.WriteLine(ex.Message);
            }
           
        }
        ICharacteristic LastCharForUpdate;
        private async void StartUpdates(ICharacteristic characteristic)
        {
            try
            {
                _updatesStarted = true;
                if (LastCharForUpdate != null)
                {
                    try
                    {
                        LastCharForUpdate.ValueUpdated -= Characteristic_ValueUpdated;
                        await LastCharForUpdate.StopUpdatesAsync();
                        Thread.Sleep(100);
                    }
                    catch (Exception er)
                    {
                        LastCharForUpdate = null;
                    }
                }

                characteristic.ValueUpdated -= Characteristic_ValueUpdated;
                characteristic.ValueUpdated += Characteristic_ValueUpdated;
                await characteristic.StartUpdatesAsync();
                _userDialogs.Toast($"Start updates");
                LastCharForUpdate = characteristic;
                flgWaitDataUpate = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{characteristic.Id}-{ex.Message}");
                //await _userDialogs.AlertAsync(ex.Message);
            }
        }
        SCUSendData ScuData { get; set; }
        private string StrJson = "";
        private bool IsStartedJson = false;
        private void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            string value = Encoding.UTF8.GetString(e.Characteristic.Value, 0, e.Characteristic.Value.Length);
            flgWaitDataUpate = false;
            //SourceText = value;
            //    Debug.WriteLine(value);
            if (value.StartsWith("{"))
            {
                this.SourceText = value;
                IsStartedJson = true;
                StrJson = value;
            }
            else if (IsStartedJson)
            {
                this.SourceText += value;
                StrJson += value;
            }
            else
            {
                StrJson = "";
            }
            Debug.Write(this.SourceText);
            StrJson = StrJson.Trim();
            if (IsStartedJson && StrJson.EndsWith("}"))
            {
                ScuData = null;
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
                    LastJsonForShare = StrJson;
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
                    try
                    {
                        this.LastValue = DateTime.Now;
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
                    catch (Exception er)
                    {
                        Debug.WriteLine(er.Message);
                    }
                }

            }


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
        private Color ChangeStatusColor(int s, int? w, int? a)
        {
            if (s > w) return Color.Green;
            if (a < s && s <= w) return Color.Yellow;
            if (s <= a) return Color.Red;
            return Color.Red;
        }
        private async Task StopAndClearCharacters()
        {
           await StopUpdates();
            mldpDataCharacteristic = transparentTxDataCharacteristic = transparentRxDataCharacteristic = null;
        }
        private async Task StopUpdate(ICharacteristic characteristic = null)
        {
            try
            {

                if (characteristic != null)
                {
                    characteristic.ValueUpdated -= Characteristic_ValueUpdated;
                    await characteristic.StopUpdatesAsync();
                    Debug.WriteLine("Stop update");
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Stop update  er-{ex.Message}");

            }

        }
        private async Task StopUpdates(ICharacteristic characteristic = null)
        {
            ////mldpDataCharacteristic, transparentTxDataCharacteristic, transparentRxDataCharacteristic;
            try
            {
                flgWaitDataUpate = false;
                await StopUpdate(LastCharForUpdate);
                await StopUpdate(mldpDataCharacteristic);

                await StopUpdate(transparentTxDataCharacteristic);
                await StopUpdate(transparentRxDataCharacteristic);

                //   Messages.Insert(0, $"Stop updates");


                _userDialogs.Toast($"Stop updates");
            }
            catch (Exception ex)
            {

                await _userDialogs.AlertAsync(ex.Message);
            }
            finally
            {
                _updatesStarted = false;
            }
            LastStateColorTimer = TimerAlarm.Enabled;
            TimerAlarm.Stop();

        }
        private int alarmHours = 700;
        public int AlarmHours
        {
            get => alarmHours;
            set => this.RaiseAndSetIfChanged(ref alarmHours, value);
        }
        DateTime lastValue;
        public DateTime LastValue
        {
            get => this.lastValue;
            private set => this.RaiseAndSetIfChanged(ref this.lastValue, value);
        }
        bool LastStateColorTimer = false;
        /// <summary>
        /// для мигания 
        /// </summary>
        /// 
        System.Timers.Timer TimerAlarm;

        System.Timers.Timer TimerWaitDataUpdate;
        Color PreviewColor = Color.White;
        private int? warning;
        /// <summary>
        /// W – Warning (уровень предупреждения) 
        /// </summary>
        public int? Warning
        {
            get => warning;
            set => this.RaiseAndSetIfChanged(ref warning, value);
        }
        string name;
        public string Name
        {
            get => this.name;
            private set => this.RaiseAndSetIfChanged(ref this.name, value);
        }
        private string sn;
        public string SN
        {
            get => sn;
            set => this.RaiseAndSetIfChanged(ref sn, value);
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
        private Color statusColor;
        public Color StatusColor
        {
            get => statusColor;
            set => this.RaiseAndSetIfChanged(ref statusColor, value);
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
        private int hrs;
        public int HRS
        {
            get => hrs;
            set => this.RaiseAndSetIfChanged(ref hrs, value);
        }
        private string note;
        public string Note
        {
            get => note;
            set => this.RaiseAndSetIfChanged(ref note, value);
        }
        private string locationName;
        public string LocationName
        {
            get => locationName;
            set => this.RaiseAndSetIfChanged(ref locationName, value);
        }
        private string operatorName;
        public string OperatorName
        {
            get => operatorName;
            set => this.RaiseAndSetIfChanged(ref operatorName, value);
        }
        private string sourceText;
        public string SourceText
        {
            get => sourceText;
            set => this.RaiseAndSetIfChanged(ref sourceText, value);
        }
        #endregion


        #region Write to device
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
        private string alarmHoursToWrite;
        public string AlarmHoursToWrite
        {
            get => alarmHoursToWrite;
            set => this.RaiseAndSetIfChanged(ref alarmHoursToWrite, value);
        }
        private string setSerialNumber;
        public string SetSerialNumber
        {
            get => setSerialNumber;
            set => this.RaiseAndSetIfChanged(ref setSerialNumber, value);
        }
        public string LastJsonForShare { get; private set; }

        private async Task WriteToDevice(string kod)
        {
            switch (kod)
            {
                case "BI":

                    if (!string.IsNullOrEmpty(BroadcastIdentity))
                    {
                       if(await WriteValueAsync($"!{BroadcastIdentity}"))
                        {
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
                            SelectedDevice.Name = BroadcastIdentity;
                            SelectedDevice.flgManualChangeName = true;
                           await DisconnectDevice(SelectedDevice,true);
                        }
                    }
                    break;//BroadcastIdentity ID
                case "AL":
                    if (!string.IsNullOrEmpty(AlarmLevel))
                    {
                       if (await WriteValueAsync($"^{AlarmLevel}"))
                        _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;
                    }
                    break;//larmLevel
                case "CF":
                    if (!string.IsNullOrEmpty(CutOff))
                    {
                       if (await WriteValueAsync($"@{CutOff}"))
                        _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;
                    }
                        break;//CutOff
                case "AH":
                    if (!string.IsNullOrEmpty(AlarmHoursToWrite))
                    {
                        if (await WriteValueAsync($"~{AlarmHoursToWrite.PadLeft(4, '0')}"))
                        _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;
                    }
                        break;//AlarmHours
                case "SN":
                    if (!string.IsNullOrEmpty(SetSerialNumber))
                    {
                        var serial = SetSerialNumber;
                        if (serial.Length < 20)
                        {
                            serial += ">";
                        }
                        serial += $"${serial}";
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        _userDialogs.Loading(Resources["SendingValueText"],tokenSource.Cancel);
                        bool res = true;
                        try
                        {
                            foreach (char ch in serial)
                            {
                                res=res && await WriteValueAsync(ch.ToString(),false);
                                System.Diagnostics.Debug.WriteLine(ch.ToString());
                                Thread.Sleep(10);

                            }
                        }
                        finally
                        {
                            _userDialogs.HideLoading();
                            tokenSource.Dispose();
                            tokenSource = null;
                            _userDialogs.Toast($"{Resources["SendingValueText"]} {serial}");
                        }
                        if (res)
                            _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;
                    }
                    break;//SerialNumber
            }
        }
        private async Task<bool> WriteValueAsync(string value, bool showloading = true)
        {
            bool result = false;
         
            try
            {
                
                
                

                
                var data = GetBytes(value);
                ICharacteristic writer = null;
                if (mldpDataCharacteristic != null)
                    writer = mldpDataCharacteristic;
                else
                    writer = transparentRxDataCharacteristic;
                //if (showloading)
                // _userDialogs.Loading(Resources["SendingValueText"],tokenSource.Cancel, Resources["CancelText"]);
                 result=   await writer.WriteAsync(data);
                //if (showloading)
                //{
                //    _userDialogs.HideLoading();
                //    tokenSource.Dispose();
                //    tokenSource = null;
                //}

                if (showloading)
                    _userDialogs.Toast($"{Resources["SendingValueText"]} {value}");
            }
            catch (Exception ex)
            {
                if (showloading)
                {
//                    _userDialogs.HideLoading();
                    await _userDialogs.AlertAsync(ex.Message);
                }
                else
                    Debug.WriteLine(ex.Message);
                result = false;
            }
            //finally
            //{
            //    _userDialogs.HideLoading();
            //    tokenSource?.Dispose();
            //    tokenSource = null;
            //}
            return result;
        }
        private static byte[] GetBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
        #endregion

    }

}

