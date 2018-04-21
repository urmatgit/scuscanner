using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Commands;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{

    public class DeviceListViewModel : BaseViewModel
    {
        DeviceListPage _deviceListPage;
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
        bool isStateOn ;

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
        private CancellationTokenSource _cancellationTokenSource;
        public ICommand ScanToggleCommand { get; }
        public ICommand StopScanCommand { get; }
        public ICommand ConnectCommand { get; }
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        readonly IPermissions _permissions;

        public DeviceListViewModel(DeviceListPage deviceListPage, IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) 
        {
            _deviceListPage = deviceListPage;
            
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
         
            

            IsRefreshing = Adapter.IsScanning;
            IsStateOn = _bluetoothLe.IsOn;
            ScanToggleCommand = ReactiveCommand.Create( () => TryStartScanning(true));
            StopScanCommand = ReactiveCommand.Create(() => StopScan());
            ConnectCommand = ReactiveCommand.CreateFromTask<DeviceListItemViewModel>(async (o) =>
             {
                 if (!IsStateOn) return;
                 //                 var selecte = o;
                 if (o.IsConnected)
                 {
                     await DisconnectDevice(o);
                     _deviceListPage.CurrentPage = _deviceListPage.DeviceListTab;
                     IsConnected = false;
                 }
                 else
                 {
                   IsConnected =  await ConnectDeviceAsync(o, false);
                     if (IsConnected)
                     {
                         _deviceListPage.CurrentPage = _deviceListPage.ConnectDeviceTab;
                         await LoadServices(o);
                     }
                 }
             });
                _bluetoothLe.StateChanged += OnStateChanged;
            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            UpdateStateText();
            IsConnected = false;
            //Adapter.DeviceDisconnected += OnDeviceDisconnected;
            //Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            // quick and dirty :>

            //Adapter.DeviceConnected += (sender, e) => Adapter.DisconnectDeviceAsync(e.Device);

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

        public override  async  void OnActivate(string kod)
        {
            base.OnActivate();
            UpdateIsScanning();
            UpdateScanText(IsRefreshing);
            UpdateStateText();
            if (IsStateOn && !string.IsNullOrEmpty(kod) && kod== "MainTabPage" &&  !IsRefreshing && SettingsBase.AutoScan)
            {
                await  TryStartScanning(true);
            }
        }
        public override void OnDeactivate(string kod)
        {
            base.OnDeactivate();
            if (!string.IsNullOrEmpty(kod) && kod == "MainTabPage")
            {
                Adapter.StopScanningForDevicesAsync();
                UpdateIsScanning();
            }
            
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
                       await  _userDialogs.AlertAsync(Resources["InfoPermissionLocationText"]);
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
            Devices.Clear();

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

                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();

                    await Adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: false, forceBleTransport: false), tokenSource.Token);
                }

                _userDialogs.Toast( $"{SettingsBase.Resources["ConnectStatusText"]}  {device.Device.Name}.");

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
        private async Task DisconnectDevice(DeviceListItemViewModel device)
        {
            try
            {
                if (!device.IsConnected)
                    return;

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
                device.Update();
                _userDialogs.HideLoading();
            }
        }

        #endregion
        #region Find service
        private async Task LoadServices(DeviceListItemViewModel device)
        {
            try
            {
                _userDialogs.ShowLoading("Discovering services...");

                var Services = await device.Device.GetServicesAsync();
                var SCUServices=Services.Where(s=> s.Id.ToString() == GlobalConstants.UUID_MLDP_PRIVATE_SERVICE || s.Id.ToString() == GlobalConstants.UUID_TANSPARENT_PRIVATE_SERVICE);
                //mldpDataCharacteristic, transparentTxDataCharacteristic, transparentRxDataCharacteristic;
                foreach (var service in SCUServices)
                {
                    Debug.WriteLine(service.Id.ToString());
                    transparentTxDataCharacteristic= await  service.GetCharacteristicAsync(Guid.Parse(GlobalConstants.UUID_TRANSPARENT_TX_PRIVATE_CHAR));
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
                            StartUpdates(mldpDataCharacteristic);
                        }
                    }
                    mldpDataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(GlobalConstants.UUID_MLDP_DATA_PRIVATE_CHAR));
                    if (mldpDataCharacteristic != null)
                    {
                        if (mldpDataCharacteristic.CanUpdate)
                        {
                            // mldpDataCharacteristic.ValueUpdated
                            StartUpdates(mldpDataCharacteristic);
                        }
                    }
                }

                var finded = SCUServices.Count();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Error while discovering services");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _userDialogs.HideLoading();
            }
        }
        private async void StartUpdates(ICharacteristic characteristic)
        {
            try
            {
                _updatesStarted = true;
                characteristic.ValueUpdated -= Characteristic_ValueUpdated;
                characteristic.ValueUpdated += Characteristic_ValueUpdated;
            await characteristic.StartUpdatesAsync();
            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
        }

        private void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            string value= Encoding.UTF8.GetString(e.Characteristic.Value, 0, e.Characteristic.Value.Length);
            Debug.WriteLine(value);
        }

        private async void StopUpdates(ICharacteristic characteristic)
        {
            try
            {
                _updatesStarted = false;

                await characteristic.StopUpdatesAsync();
                characteristic.ValueUpdated -= Characteristic_ValueUpdated;

             //   Messages.Insert(0, $"Stop updates");

                

            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
        }
        #endregion

    }

}
 
