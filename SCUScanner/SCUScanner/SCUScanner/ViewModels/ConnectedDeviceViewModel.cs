﻿using Plugin.BluetoothLE;
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

namespace SCUScanner.ViewModels
{
    public class ConnectedDeviceViewModel : BaseViewModel
    {
        

        public ScanResultViewModel DeviceViewModel {get;set;}
        public ObservableCollection<Group<GattCharacteristicViewModel>> GattCharacteristics { get; } = new ObservableCollection<Group<GattCharacteristicViewModel>>();
        //public ObservableCollection<GattDescriptorViewModel> GattDescriptors { get; } = new ObservableCollection<GattDescriptorViewModel>();
        public TabbedPage ParentTabbed { get; set; }
        readonly IList<IDisposable> cleanup = new List<IDisposable>();
        IDevice device;
        public ICommand DisconnectCommand { get; }
        public ICommand SelectCharacteristic { get; }
        
        public ConnectedDeviceViewModel(ScanResultViewModel selectedDevice)
        {
            device = selectedDevice.Device;
            DeviceViewModel = selectedDevice;

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
        public override void OnActivate()
        {
            base.OnActivate();
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
                           this.GattCharacteristics.Clear();
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
               .Subscribe(service =>
               {
                   if (string.IsNullOrEmpty(service.Uuid.ToString())) return; 
                   var group = new Group<GattCharacteristicViewModel>(service.Uuid.ToString());
                   service
                       .WhenCharacteristicDiscovered()
                       .ObserveOn(RxApp.MainThreadScheduler)
                       .Subscribe(character =>
                       {
                           if (group.FirstOrDefault(f => f.Uuid==character.Uuid) == null)
                           {
                               var vm = new GattCharacteristicViewModel(character);
                               
                               group.Add(vm);
                           }
                           if (group.Count == 1 && this.GattCharacteristics.FirstOrDefault(g=>g.Name== group.Name)==null)
                               this.GattCharacteristics.Add(group);

                           //character
                           //    .WhenDescriptorDiscovered()
                           //    .Subscribe(desc => Device.BeginInvokeOnMainThread(() =>
                           //    {
                           //        var dvm = new GattDescriptorViewModel(this.Dialogs, desc);
                           //        this.GattDescriptors.Add(dvm);
                           //    }));
                       });
               })
               );
        }
      
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            foreach (var item in this.cleanup)
                item.Dispose();
        }
    }
}
