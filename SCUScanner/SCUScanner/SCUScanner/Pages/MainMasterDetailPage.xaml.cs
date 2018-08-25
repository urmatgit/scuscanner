using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCUScanner.Models;
using SCUScanner.Services;
using Acr.UserDialogs;


namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMasterDetailPage : MasterDetailPage
    {
        MasterDetailPageMenuItem selectedPage = null;
        Page CurrentPage = null;
        SCUScanner.Models.Settings settings= SCUScanner.Models.Settings.Current;
        public MainMasterDetailPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {

                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                //    BindingContext = null;
                      settings = sender as SCUScanner.Models.Settings;
                    CurrentPage.Title = settings.Resources[selectedPage.PageCode];
                }
            });

            ListView_ItemSelected(null, new SelectedItemChangedEventArgs(MasterPage.viewModel.MenuItems.FirstOrDefault(m => m.Id == 0)));

        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            var item = e.SelectedItem as MasterDetailPageMenuItem;
            if (item == null)
                return;
            selectedPage = item;
            var navigation = ((Application.Current.MainPage as Xamarin.Forms.MasterDetailPage)?.Detail as Xamarin.Forms.NavigationPage);
            var CurrentDetailPage=navigation?.CurrentPage ;
            if (CurrentDetailPage == null || CurrentDetailPage.GetType().Name!=item.TargetType.Name || CurrentDetailPage.GetType().Name==typeof(MaintenancePage).Name)
            {
                if (item.TargetType.Name==typeof(DeviceListPage).Name)
                    if(App.mainTabbed ==null)
                    {
                        CurrentPage = (Page)Activator.CreateInstance(item.TargetType);
                        CurrentPage.Title = item.Title;
                        App.mainTabbed = CurrentPage as DeviceListPage;
                    }
                    else
                    {
                        CurrentPage = App.mainTabbed;
                    }
                else {
                    var type = item.TargetType;
                    //if (item.TargetType.Name == typeof(MaintenancePage).Name)
                    //{
                        
                    //    var action=await DisplayActionSheet(null, null, null, settings.Resources["EnterSerialNumberText"], settings.Resources["ListOfManualsText"]);
                    //    if (action== settings.Resources["ListOfManualsText"])
                    //    {
                    //        type = typeof(ListOfManualPage);
                    //        //type = typeof(TestListViewPage);
                    //    }
                    //}else if (item.PageCode== "SparesText")
                    //{


                    //    if (string.IsNullOrEmpty(App.SerialNumber))
                    //    {
                    //        type = typeof(InputSerialForSparesPage);
                    //    }
                    //    else
                    //    {
                    //        await SparePage.InitSparePage();
                    //        type = typeof(SparePage);
                    //    }
                    //}


                    CurrentPage = (Page)Activator.CreateInstance(type);
                    CurrentPage.Title = item.Title;
                }
                Detail = new NavigationPage(CurrentPage);
            }            
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
         
    }
}