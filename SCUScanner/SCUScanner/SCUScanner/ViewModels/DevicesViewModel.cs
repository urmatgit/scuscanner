using Plugin.BluetoothLE;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Timers;
using System.Windows.Input;
using System.ComponentModel;
namespace SCUScanner.ViewModels
{
    public class TimeString: INotifyPropertyChanged
    {
        private string _time;
        public string time
        {
            get => _time;
            set {
                _time = value;
                RaisePropertyChanged("time");
               }
        }
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
    public class DevicesViewModel : BaseViewModel, INotifyPropertyChanged
    {
       
        #region for scanning View
        const int ScanningDuration = 3; //sec
        IDisposable scan;
        IDisposable connect;
        System.Timers.Timer StopScanning = new System.Timers.Timer();
        /// <summary>
        /// 
        /// List Of scanning devices
        /// </summary>
        private ReactiveList<ScanResultViewModel> devices;
        public ReactiveList<ScanResultViewModel> Devices {
            get => devices;
            set => this.RaiseAndSetIfChanged(ref devices, value);
        }
        public ObservableCollection<TimeString> times { get; set; }
        public ICommand ScanToggleCommand { get; }
        public ICommand ConnectCommand { get; }
        bool scanning;
        public bool IsScanning
        {
            get => this.scanning;
            private set => this.RaiseAndSetIfChanged(ref this.scanning, value);
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
        public void ScanTextChange(bool scaning)
        {
            if (scaning)
                ScanText = Resources["StopScanText"];
            else
                ScanText = Resources["ScanText"];
        }
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
        }
        private void StopScanning_Elapsed(object sender, ElapsedEventArgs e)
        {
            times.Add(new TimeString() { time = DateTime.Now.ToString() });
         //   StopScanBle();
            //time = DateTime.Now.ToString();
            
        }

        private void StopScanBle()
        {


            StopScanning.Stop();
            this.scan?.Dispose();
            this.IsScanning = false;

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
        private string _time;
        public string time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }
        #endregion
        public DevicesViewModel()
        {
            times = new ObservableCollection<TimeString>();
            Devices = new ReactiveList<ScanResultViewModel>();
            StopScanning.Interval = 1000 * ScanningDuration;
            StopScanning.Elapsed += StopScanning_Elapsed;
            App.BleAdapter
              .WhenStatusChanged()
              .ObserveOn(RxApp.MainThreadScheduler)
              .Subscribe(x =>
              {
                  CheckStatus(x);
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
                       StopScanBle();
                   }
                   else
                   {

                        //this.Devices.Clear();
                        this.IsScanning = true;

                        //this.ScanText = Resources["ScanText"];
                        if (Models.Settings.Current.ManualScan)
                           StopScanning.Start();
                       if (!App.BleAdapter.IsScanning)
                           this.scan = App.BleAdapter
                                .Scan()
                                    //                            .Where(r=>r.AdvertisementData.ServiceUuids!=null && r.AdvertisementData.ServiceUuids?.Length>0) //filter where service >0
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
            StopScanning.Start();
        }

    }
}
