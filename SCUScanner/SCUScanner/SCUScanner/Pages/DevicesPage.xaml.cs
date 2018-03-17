using SCUScanner.ViewModels;
using Syncfusion.XForms.TabView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SCUScanner.Pages.Views;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DevicesPage : ContentPage
	{
        TabItemCollection tabItems = new TabItemCollection();
        DevicesViewModel devicesViewModel;
        SfTabView sfTabView;
        BluetoothSettingInfoView InfoView;
        SfTabItem ScanBluetoothItem = new SfTabItem();
       // ListView listView;
        public DevicesPage ()
		{
			InitializeComponent ();
            sfTabView = new SfTabView();
            sfTabView.VerticalOptions = LayoutOptions.FillAndExpand;
            InfoView = new BluetoothSettingInfoView();
            BindingContext = devicesViewModel= new DevicesViewModel();
            this.WhenAnyValue(vm => vm.devicesViewModel.IsVisibleLayout).Subscribe(val =>
            {
                if (val)
                     this.Content = sfTabView;
                else
                    this.Content = InfoView;
            });
            

            ScanBluetoothItem = CreateScanBluetoothItem();

            tabItems.Add(ScanBluetoothItem);

            sfTabView.Items = tabItems;
             


        }

        protected SfTabItem CreateScanBluetoothItem()
        {
            SfTabItem sfTabItem = new SfTabItem();
            sfTabItem.Title = devicesViewModel.Resources["MainText"];
            sfTabItem.Content = new ScanBluetoothView1();
            return sfTabItem;
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
            if (!tabItems.Contains(ScanBluetoothItem))
                tabItems.Add(ScanBluetoothItem);
          
            //    tabView.SelectedIndex = 0;

        }
        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    if (devicesViewModel.IsVisibleLayout)
        //        sfTabView.HeightRequest = height;
        //    base.OnSizeAllocated(width, height);
        //}
    }
}