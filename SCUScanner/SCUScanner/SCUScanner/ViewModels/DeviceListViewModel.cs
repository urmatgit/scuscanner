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
using SCUScanner.Models;
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
        private readonly IBluetoothLE _bluetoothLe;
        private readonly IUserDialogs _userDialogs;
        private readonly ISettings _settings;
        private readonly IAdapter Adapter;
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
        private CancellationTokenSource _cancellationTokenSource;
        public ICommand ScanToggleCommand { get; }
        public ICommand StopScanCommand { get; }
        
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        readonly IPermissions _permissions;

        public DeviceListViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) 
        {
            ScanText = Resources["ScanText"];
            Adapter = adapter;
         
         
            _permissions = permissions;
            _bluetoothLe = bluetoothLe;
            _userDialogs = userDialogs;
            _settings = settings;

            IsRefreshing = Adapter.IsScanning;
            IsStateOn = _bluetoothLe.IsOn;
            ScanToggleCommand = ReactiveCommand.Create(() => TryStartScanning(true));
            StopScanCommand = ReactiveCommand.Create(() => StopScan());
            _bluetoothLe.StateChanged += OnStateChanged;
            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            //Adapter.DeviceDisconnected += OnDeviceDisconnected;
            //Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            // quick and dirty :>

            //Adapter.DeviceConnected += (sender, e) => Adapter.DisconnectDeviceAsync(e.Device);

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
                        return Settings.Current.Resources["SetHintBluetoothAndroidText"];
                    else
                        return Settings.Current.Resources["SetHintBluetoothIOSText"];
                default:
                    return "Unknown BLE state.";
            }
        }
        private void UpdateStateOn()
        {
            IsStateOn = _bluetoothLe.IsOn;
        }
        private void UpdateIsScanning()
        {
            IsRefreshing = Adapter.IsScanning;
        }
        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {

            UpdateStateOn();
            var stattext = GetStateText();
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
        public string ScanText
        {
            get => scantext;
            set => this.RaiseAndSetIfChanged(ref this.scantext, value);
        }

        public override void OnActivate()
        {
            base.OnActivate();
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            Adapter.StopScanningForDevicesAsync();
            IsRefreshing=Adapter.IsScanning;
        }


        private async void TryStartScanning(bool refresh = false)
        {
            
            if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.Android)
            {
                var status = await _permissions.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var permissionResult = await _permissions.RequestPermissionsAsync(Permission.Location);

                    if (permissionResult.First().Value != PermissionStatus.Granted)
                    {
                       await  _userDialogs.AlertAsync("Permission denied. Not scanning.");
                        return;
                    }
                }
            }

            if (IsStateOn && (refresh || !Devices.Any()))
            {
                if (!IsRefreshing)
                {
                    ScanText = Resources["StopScanText"];
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
            StopScanCommand.Execute(null);
            if (!isRefreshing)
                ScanText = Resources["ScanText"];
        }

       

        


       
    }

}
 
