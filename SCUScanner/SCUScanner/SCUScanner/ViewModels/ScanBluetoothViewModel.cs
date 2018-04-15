using System;
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
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace SCUScanner.ViewModels
{
    public class ScanBluetoothViewModel : BaseViewModel
    {
        const int ScanningDuration = 30; //sec
        IDisposable scan;
        IDisposable connect;
        bool IsClickScan = false;
        bool IsBroadcastNameChanged = false;
        System.Timers.Timer StopScanning = new System.Timers.Timer();
        CharacterPage characterPage { get; set; }
        DeviceSettingPage deviceSettingPage { get; set; }
        private ScanResultViewModel LastConnectedItem { get; set; }
        TabbedPage parentTabbed;
        
        public TabbedPage ParentTabbed
        {
            get => parentTabbed;
            set => parentTabbed = value;
        }
        public ObservableCollection<ScanResultViewModel> Devices { get; }


        public ICommand ScanToggleCommand { get; }
       
        public ICommand ConnectCommand { get; }

        public ScanBluetoothViewModel(TabbedPage page) : base()
        {

            parentTabbed = page;
            StopScanning.Interval = 1000 * ScanningDuration;
            StopScanning.Elapsed += StopScanning_Elapsed;
            Devices = new ObservableCollection<ScanResultViewModel>();
            this.WhenAnyValue(vm => vm.IsVisibleLayout).ToProperty(this, x => x.IsVisibleBlueToothTornOff);
            this.WhenAnyValue(vm => vm.IsVisibleLayout).Subscribe(s =>
            {
                if (s)
                    ResourcesEx = Resources;
                else
                    ResourcesEx = null;
            });
            this.WhenAnyValue(vm => vm.SettingsBase.Resources).Subscribe(val =>
            {

                ScanTextChange(App.BleAdapter.IsScanning);

            });
            this.WhenAnyValue(vm => vm.IsScanning).Subscribe(val =>
            {
                ScanTextChange(val);
            });
            if (App.BleAdapter.Status == AdapterStatus.Unsupported || App.BleAdapter.Status == AdapterStatus.Unknown)
            {
                IsVisibleLayout = false;
                // return;
            }


            IsVisibleLayout = App.BleAdapter.Status != AdapterStatus.PoweredOn;
            this.connect = App.BleAdapter
                .WhenDeviceStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    var vm = this.Devices.FirstOrDefault(dev => dev.Uuid.Equals(x.Uuid));
                    if (vm != null)
                    {
                        vm.IsConnected = x.Status == ConnectionStatus.Connected;
                        if (!vm.IsConnected)
                        {
                            App.mainTabbed.CurrentConnectDeviceSN = "";
                            if(!IsBroadcastNameChanged)
                                LastConnectedItem = null;
                        }
                    }
                });

            App.BleAdapter.WhenStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(st =>
            {
                CheckStatus(st);

            });


            
            this.ConnectCommand = ReactiveCommand.CreateFromTask<ScanResultViewModel>(async (o) =>
           {
               if (Models.Settings.Current.ManualScan)
                   StopScanBle();

               LastConnectedItem = o;
               IDevice device = o.Device;
               try
               {
                    // don't cleanup connection - force user to d/c
                    if (device.Status == ConnectionStatus.Disconnected)
                   {
                       Debug.WriteLine("connection");


                       using (var cancelSrc = new CancellationTokenSource())
                       {
                           using (App.Dialogs.Loading(Resources["ConnectingText"], cancelSrc.Cancel, Resources["CancelText"]))
                           {
                               try
                               {
                                   await device.Connect(
                                       new GattConnectionConfig() { AutoConnect = false }
                                       ).ToTask(cancelSrc.Token);
                               }catch(Exception er) 
                               {
                                   if (er.Message.StartsWith("133"))
                                   {
                                       Thread.Sleep(3000);
                                       await device.Connect(
                                           new GattConnectionConfig() { AutoConnect = false }
                                           ).ToTask(cancelSrc.Token);
                                   }
                                   else
                                       throw er;
                               }
                              // var actual = await device.RequestMtu(512); //Read write size (default 20byte)
                                                                          //  App.Dialogs.Alert("MTU Changed to " + actual);
                                var title = Resources["ConnectedDeviceCaptionText"];

                               characterPage = new CharacterPage(o) { Title = title };// ConnectedDevicePage(o) { Title = o.Name };
                               
                               characterPage.Kod = o.Name;
                               characterPage.Tabbed = this.ParentTabbed;
                               title = Resources["DeviceSettingsCaptionText"];
                               App.mainTabbed.characterPage = characterPage;
                               deviceSettingPage = new DeviceSettingPage(o) { Title = title };
                               App.mainTabbed.deviceSettingPage = deviceSettingPage;
                               deviceSettingPage.Kod = $"{o.Name}_setting";
                               deviceSettingPage.Tabbed = this.ParentTabbed;
                               {
                                 //  CleanTabPages();
                               }
                               parentTabbed.Children.Add(characterPage);
                               parentTabbed.Children.Add(deviceSettingPage);
                               parentTabbed.CurrentPage = characterPage;

                           }
                       }
                       App.Dialogs.Toast("Connected");
                   }
                   else
                   {

                       device.CancelConnection();
                       LastConnectedItem = null;
                       o.IsConnected = false;
                       App.mainTabbed.CurrentConnectDeviceSN = "";
                       parentTabbed.CurrentPage = CleanTabPages();
                       App.Dialogs.Toast("Disconnected");

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
                    IsClickScan = true;
                if (this.IsScanning)
                {
                    StopScanBle();
                }
                else
                {

                      //  this.Devices.Clear();
                        while (this.Devices.Any())
                        {
                            this.Devices.RemoveAt(0);
                        }
                        this.IsScanning = true;

                    //this.ScanText = Resources["ScanText"];
                    if (Models.Settings.Current.ManualScan)
                        StopScanning.Start();
                        if (!App.BleAdapter.IsScanning)
                        {
                            this.scan = App.BleAdapter
                                .Scan()
                                    //.Where(r=>r.AdvertisementData.ServiceUuids!=null && r.AdvertisementData.ServiceUuids?.Length>0) //filter where service >0
                                    .Buffer(TimeSpan.FromSeconds(1))
                                    .ObserveOn(RxApp.MainThreadScheduler)
                                    .Subscribe(results =>
                                    {
                                        foreach (var result in results)
                                            this.OnScanResult(result);
                                    });
                            App.Dialogs.Toast("Scanning start");
                        }
                        Debug.WriteLine("End scanning");
                    }
                }
                //,
                //this.WhenAny(
                //    x => x.IsSupported,
                //    x => x.Value
                //)
            );

            //if (Models.Settings.Current.ScanMode)
            //    this.ScanToggleCommand.Execute(null);

        }
        public BaseTabPage CleanTabPages(bool restorelastconnect = false)
        {
             
                var ListOfRemovePages = new List<BaseTabPage>();
            BaseTabPage result = null;
            foreach (BaseTabPage bPage in parentTabbed.Children)
                if (bPage.Kod != SCUScanner.Helpers.GlobalConstants.MAIN_TAB_PAGE)
                    ListOfRemovePages.Add(bPage);
                else
                    result = bPage;
            if (ListOfRemovePages.Count > 0)
                ListOfRemovePages.ForEach(x => {
                    x.Dispose();
                    parentTabbed.Children.Remove(x);
                    });
            App.mainTabbed.characterPage = null;
            App.mainTabbed.deviceSettingPage = null;
            if(restorelastconnect && this.LastConnectedItem != null)
            {
                IsBroadcastNameChanged = true;
                //this.ConnectCommand.Execute(this.LastConnectedItem);

            }
            else
            this.LastConnectedItem = null;
            return result;
        }
        private void StopScanning_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopScanBle();
        }

        private void StopScanBle()
        {
            
                
            StopScanning.Stop();
            this.scan?.Dispose();
            this.IsScanning = false;
            App.Dialogs.Toast("Scanning stop");
        }
        public override void OnDeactivate()
        {
            //StopScanning.Stop();
            if (this.IsScanning)
                StopScanBle();
        }
        public override void OnActivate()
        {
            base.OnActivate();
            if (characterPage!=null)
                characterPage.Title = Resources["ConnectedDeviceCaptionText"];
            if (deviceSettingPage!=null)
                deviceSettingPage.Title= Resources["DeviceSettingsCaptionText"];
            if (!this.IsScanning && Models.Settings.Current.AutoScan && IsClickScan)
            {
                this.ScanToggleCommand.Execute(null);
            }
            //App.BleAdapter
            //    .WhenStatusChanged()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(x =>
            //    {
            //        CheckStatus(x);
            //    });

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
                if (LastConnectedItem != null && dev.Name==LastConnectedItem.Name && !IsBroadcastNameChanged)
                {
                    dev.IsConnected = true;
                }

                //  if (dev.ServiceCount>0)
                this.Devices.Add(dev);
                if (IsBroadcastNameChanged && LastConnectedItem != null)
                {
                    try
                    {
                        ConnectCommand.Execute(dev);
                    }
                    finally
                    {
                        IsBroadcastNameChanged = false;
                    }
                }
            }
            //   UpdateButtonText(dev);
        }
        public void ScanTextChange(bool scaning)
        {
            if (scaning)
                ScanText = Resources["StopScanText"];
            else
                ScanText = Resources["ScanText"];
            if (Devices.Count() > 0)
            {
                foreach(var dev in Devices)
                {
                    dev.UpdateButtonText();
                }
            }
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
                    StopScanBle();
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
            get { return $"{Resources["BlueToothTornOffText"]} {Resources["ScanText"]}"; }
        }
        /// <summary>
        /// Layout show when Bluetooth disabled
        /// </summary>
        public bool IsVisibleBlueToothTornOff
        {
            get
            {
                return !isVisibleLayout;
            }
        }

        /// <summary>
        /// Layout for scanning 
        /// </summary>
        private bool isVisibleLayout = false;
        public bool IsVisibleLayout
        {
            get => isVisibleLayout;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isVisibleLayout, value);


            }
        }
        LocalizedResources resourcesex;
        public LocalizedResources ResourcesEx
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

