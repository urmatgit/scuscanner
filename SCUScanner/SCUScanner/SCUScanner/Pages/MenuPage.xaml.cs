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
using SCUScanner.Services;
using System.Resources;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;
         public MenuPageViewModel viewModel;
        public MenuPage()
        {
            InitializeComponent();

             viewModel = new MenuPageViewModel();

            viewModel.MenuItems = CreateMenuItems();
                //new ObservableCollection<MasterDetailPageMenuItem>(
                //    new[]
                //        {
                //            new MasterDetailPageMenuItem(typeof(Settings)) { Id = 0, Title =SCUScanner.Resources.AppResource.SettingsText},
                //            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 1, Title = "Page 2" },
                //            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 2, Title = "Page 3" }
                //        });

            BindingContext = viewModel;
            ListView = MenuItemsListView;
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {
                
                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                    BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    viewModel.MenuItems = CreateMenuItems(settings);
                    BindingContext = viewModel;
                }
            });
        }
        
        private ObservableCollection<MasterDetailPageMenuItem> CreateMenuItems(Models.Settings settings =null)
        {
            
             return new ObservableCollection<MasterDetailPageMenuItem>(
                   new[]
                       {
                       
                            new MasterDetailPageMenuItem(typeof(MainTabbedPage)) { Id = 0, IconSource="technology.png", PageCode="MainText",  Title =settings==null?SCUScanner.Resources.AppResource.MainText : settings.Resources["MainText"] },
                            //new MasterDetailPageMenuItem(typeof(SCUItemsPage)) { Id = 1, IconSource="list.png", PageCode="DataText",  Title =settings==null?SCUScanner.Resources.AppResource.DataText : settings.Resources["DataText"] },
                            new MasterDetailPageMenuItem(typeof(DataDevicesListPage)) { Id = 1, IconSource="list.png", PageCode="DataText",  Title =settings==null?SCUScanner.Resources.AppResource.DataText : settings.Resources["DataText"] },
                            new MasterDetailPageMenuItem(typeof(Settings)) { Id = 2,IconSource="cogwheel.png",PageCode="SettingsText",Title =settings==null?SCUScanner.Resources.AppResource.SettingsText : settings.Resources["SettingsText"]},
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 3,IconSource="graduation.png", PageCode="HelpText", Title = settings==null?SCUScanner.Resources.AppResource.HelpText : settings.Resources["HelpText"]  },
                            new MasterDetailPageMenuItem(typeof(MaintenancePage)) { Id = 4,IconSource="settings.png", PageCode="MaintenanceText", Title = settings==null?SCUScanner.Resources.AppResource.MaintenanceText : settings.Resources["MaintenanceText"]  },
                            new MasterDetailPageMenuItem(typeof(MasterDetailPageDetail)) { Id = 5,IconSource="Spares.png", PageCode ="SparesText", Title = settings==null?SCUScanner.Resources.AppResource.SparesText : settings.Resources["SparesText"]  }
                       });
        }

    }
}