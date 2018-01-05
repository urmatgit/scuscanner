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
using SCUScanner.ViewModels;
namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;

        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MenuPageViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPageMenuItem>(
                    new[]
                        {
                            new MasterDetailPageMenuItem(typeof(Settings)) { Id = 0, Title = SCUScanner.Resources.AppResource.SettingsText },
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 1, Title = "Page 2" },
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 2, Title = "Page 3" }
                        })
            };

            ListView = MenuItemsListView;
        }


    }
}