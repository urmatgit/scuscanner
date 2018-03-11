using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
   public class TestTimeViewModel: INotifyPropertyChanged
    {
        System.Timers.Timer StopScanning = new System.Timers.Timer();

        private string _time;
        public string time
        {
            get => _time;
            set
            {
                _time = value;
                RaisePropertyChanged("time");
            }
        }
        public TestTimeViewModel()
        {
            times = new ObservableCollection<TimeString>();

            //StopScanning.Interval = 1000 * 3;
            //StopScanning.Elapsed += StopScanning_Elapsed;

            //StopScanning.Start();
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                ///teas dfas df

                times.Add(new TimeString() { time = DateTime.Now.ToString() });

                return true;
            });
        }
        private void StopScanning_Elapsed(object sender, ElapsedEventArgs e)
        {
            times.Add(new TimeString() { time = DateTime.Now.ToString() });
            //   StopScanBle();
            //time = DateTime.Now.ToString();

        }
        public ObservableCollection<TimeString> times { get; set; }
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
