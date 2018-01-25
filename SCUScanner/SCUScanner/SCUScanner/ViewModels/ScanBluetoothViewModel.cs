﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using ReactiveUI;
using SCUScanner.Services;
using Xamarin.Forms;
using System.Timers;
using SCUScanner.Pages;
using System.Threading;
using System.Reactive.Threading.Tasks;

namespace SCUScanner.ViewModels
{
    public class ScanBluetoothViewModel : BaseViewModel
    {
        const int ScanningDuration = 30; //sec
        IDisposable scan;
        IDisposable connect;
       
        System.Timers.Timer StopScanning = new System.Timers.Timer();

        TabbedPage parentTabbed;
        public TabbedPage ParentTabbed
        {
            get=> parentTabbed;
            set => parentTabbed=value;
        }
        public ObservableCollection<ScanResultViewModel> Devices { get; }


        public ICommand ScanToggleCommand { get; }
        public ICommand SelectDeviceCommand { get; }
        public ICommand  ConnectCommand { get;  }
     
        public ScanBluetoothViewModel(TabbedPage page):base()
        {
            
            parentTabbed = page;
            StopScanning.Interval = 1000 * ScanningDuration;
            StopScanning.Elapsed += StopScanning_Elapsed;
            Devices=new ObservableCollection<ScanResultViewModel>();
            if (App.BleAdapter.Status == AdapterStatus.Unsupported)
            {
                IsVisibleLayout = false;
                return;
            } 
            IsVisibleLayout =  App.BleAdapter.Status != AdapterStatus.PoweredOn;
            this.connect = App.BleAdapter
                .WhenDeviceStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    var vm = this.Devices.FirstOrDefault(dev => dev.Uuid.Equals(x.Uuid));
                    if (vm != null)
                    {
                        vm.IsConnected = x.Status == ConnectionStatus.Connected;

                    }
                });
            this.WhenAnyValue(vm => vm.IsVisibleLayout).ToProperty(this, x => x.IsVisibleBlueToothTornOff);
            this.WhenAnyValue(vm => vm.IsVisibleLayout).Subscribe(s =>
            {
                if (s)
                    ResourcesEx = Resources;
                else
                    ResourcesEx = null;
            });
            this.WhenAnyValue(vm => vm.Resources).Subscribe(val =>
            {
                
                    ScanTextChange(App.BleAdapter.IsScanning);
                
            });
            this.WhenAnyValue(vm => vm.IsScanning).Subscribe(val =>
            {
                ScanTextChange(val);
            });
            App.BleAdapter.WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(st =>
            {
                CheckStatus(st);

            });
            
            
            App.BleAdapter.WhenScanningStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(on =>
                {
                    this.IsScanning = on;
                    ScanTextChange(on);
                });
            this.SelectDeviceCommand = ReactiveCommand.Create<ScanResultViewModel>(x =>
            {
                StopScan();
                App.Dialogs.Alert($"Selected {x.Name}");
                //services.VmManager.Push<DeviceViewModel>(x.Device);
            });
            this.ConnectCommand = ReactiveCommand.CreateFromTask<ScanResultViewModel> (async  (o) =>
            {
                StopScan();
                IDevice device = o.Device;
                try
                {
                    // don't cleanup connection - force user to d/c
                    if ( device.Status == ConnectionStatus.Disconnected)
                    {
                        //Only for LE Simulator
                        if (!device.Features.HasFlag(DeviceFeatures.PairingRequests))
                        {
                            App.Dialogs.Alert("Pairing is not supported on this platform");
                        }
                        else if (device.PairingStatus == PairingStatus.Paired)
                        {
                            //App.Dialogs.Alert("Device is already paired");
                        }
                        else
                        {
                            await device.PairingRequest();
                        }

                        using (var cancelSrc = new CancellationTokenSource())
                        {
                            using (App.Dialogs.Loading(Resources["ConnectingText"], cancelSrc.Cancel,Resources["CancelText"]))
                            {

                                await device.Connect().ToTask(cancelSrc.Token);
                                
                                var devPage = new ConnectedDevicePage(o) { Title = o.Name };
                                devPage.Tabbed = this.ParentTabbed;
                                parentTabbed.Children.Add(devPage);
                                parentTabbed.CurrentPage = devPage;

                            }
                        }
                    }
                    else
                    {
                        device.CancelConnection();
                        o.IsConnected = false;
                        var devicePage = parentTabbed.Children.FirstOrDefault(p => p.Title == o.Name);
                        if (devicePage != null)
                        {
                            parentTabbed.CurrentPage = parentTabbed.Children[0];
                            parentTabbed.Children.Remove(devicePage);
                            
                        }
                    }
                //   UpdateButtonText(o);
                }
                catch (Exception ex)
                {
                    App.Dialogs.Alert(ex.ToString());
                }
            });
            
            this.ScanToggleCommand = ReactiveCommand.Create(
                () =>
                {
                    if (!isVisibleLayout)
                    {
                        return;
                    }
                    if (this.IsScanning)
                    {
                        StopScan();
                    }
                    else
                    {

                        this.Devices.Clear();
                        this.IsScanning = true ;

                        //this.ScanText = Resources["ScanText"];
                        if (Models.Settings.Current.ManualScan)
                            StopScanning.Start();
                        this.scan = App.BleAdapter
                            .Scan()
                            .Buffer(TimeSpan.FromSeconds(1))
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(results =>
                            {
                                foreach (var result in results)
                                    this.OnScanResult(result);
                            });
                        Debug.WriteLine("End scanning");
                    }
                }
                //,
                //this.WhenAny(
                //    x => x.IsSupported,
                //    x => x.Value
                //)
            );
            if (Models.Settings.Current.ScanMode)
                this.ScanToggleCommand.Execute(null);


        }

        private void StopScanning_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopScan();
        }

        private void StopScan()
        {
            StopScanning.Stop();
            this.scan?.Dispose();
            this.IsScanning = false;

        }
        public override void OnActivate()
        {
            base.OnActivate();
            App.BleAdapter
                .WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                   CheckStatus(x);
                });

        }
        //void UpdateButtonText(ScanResultViewModel dev)
        //{
        //    dev.ConnectButtonText = dev.IsConnected ? Resources["DisConnectButtonText"] : Resources["ConnectButtonText"];
        //}
        void OnScanResult(IScanResult result)
        {
            var dev = this.Devices.FirstOrDefault(x => x.Uuid.Equals(result.Device.Uuid));
            if (dev != null)
            {
                dev.TrySet(result);
            }
            else
            {
                dev = new ScanResultViewModel();
                dev.TrySet(result);
                
                
                
                this.Devices.Add(dev);
            }
         //   UpdateButtonText(dev);
        }
        public void ScanTextChange(bool scaning)
        {
            if (scaning)
                ScanText = Resources["StopScanText"];
            else
                ScanText = Resources["ScanText"];
        }
        
        bool scanning;
        public bool IsScanning
        {
            get => this.scanning;
            private set => this.RaiseAndSetIfChanged(ref this.scanning, value);
        }
        private bool CheckStatus(AdapterStatus status)
        {
            
            if (status == AdapterStatus.PoweredOn)
            {
                IsVisibleLayout = true;
                if (string.IsNullOrEmpty(ScanText))
                    ScanText = Resources["ScanText"];
            }
            else
            {
                IsVisibleLayout = false;
                ScanText = "";
                if (IsScanning)
                    StopScan();
            }
            return isVisibleLayout;
        }
       
        string scantext;
        public string ScanText
        {
            get => scantext;
            set => this.RaiseAndSetIfChanged(ref this.scantext, value);
        }
        public string BlueToothTornOffText
        {
            get { return $"{Resources["BlueToothTornOffText"]} {Resources["ScanText"]}" ; }
        }
        /// <summary>
        /// Layout show when Bluetooth disabled
        /// </summary>
        public bool IsVisibleBlueToothTornOff
        {
            get {
                return !isVisibleLayout;
            }
        }
        
        /// <summary>
        /// Layout for scanning 
        /// </summary>
        private bool isVisibleLayout=false;
        public bool IsVisibleLayout
        {
            get  =>  isVisibleLayout; 
            set 
            {
                this.RaiseAndSetIfChanged(ref this.isVisibleLayout, value);
                
            
            }
        }
        LocalizedResources resourcesex;
        public  LocalizedResources ResourcesEx
        {
            get
            {
            
                return resourcesex;
            }
             set
            {
                //if (!isVisible)
                //    resourcesex = null;
                //else
                //    resourcesex = value;
                this.RaiseAndSetIfChanged(ref this.resourcesex, value);
            }
        }
    }
}

