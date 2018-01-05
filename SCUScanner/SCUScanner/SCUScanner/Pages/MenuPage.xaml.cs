using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCUScanner.Models;
namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;

        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MenuPageViewModel();
            ListView = MenuItemsListView;
        }

        class MenuPageViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailPageMenuItem> MenuItems { get; set; }
            
            public MenuPageViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPageMenuItem>(new[]
                {
                    new MasterDetailPageMenuItem(typeof(Settings)) { Id = 0, Title = "Settings" },
                    new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 1, Title = "Page 2" },
                    new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 2, Title = "Page 3" } 
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}