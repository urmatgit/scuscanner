
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCUScanner.Models
{

    public class MasterDetailPageMenuItem :BaseViewModel
    {
        public MasterDetailPageMenuItem(Type type)
        {
            TargetType = type; // typeof(MasterDetailPageDetail);
        }
        public int Id { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        
        
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }
}