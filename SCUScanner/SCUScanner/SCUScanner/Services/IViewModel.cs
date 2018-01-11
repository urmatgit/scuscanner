using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SCUScanner.Services
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void Init(object args = null);

        void OnActivate();
        void OnDeactivate();
    }
}
