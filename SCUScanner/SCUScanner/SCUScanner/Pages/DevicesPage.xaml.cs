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
        SfTabItem NearbyDevicesTabItem;
        SfTabItem ConnectedDeviceTabItem;
        BluetoothSettingInfoView InfoView;
        
       // ListView listView;
        public DevicesPage ()
		{
			InitializeComponent ();
            var content = this.Content as SfTabView;

            sfTabView = content ??  new SfTabView();
            sfTabView.VerticalOptions = LayoutOptions.FillAndExpand;

            InfoView = new BluetoothSettingInfoView();
            BindingContext = devicesViewModel= new DevicesViewModel();
            this.WhenAnyValue(vm => vm.devicesViewModel.IsVisibleLayout).Subscribe(val =>
            {
                var content1 = this.Content as SfTabView;
                if (val) {
                    if (content1 == null)
                        this.Content = sfTabView;
                }
                else
                    this.Content = InfoView;
            });
            this.WhenAnyValue(vm => vm.devicesViewModel.Resources).Subscribe(val =>
            {
                NearbyDevicesTabItem.Title= val["NearbyDevicesCaptionText"];
            });


            NearbyDevicesTabItem = CreateTabItem("NearbyDevicesCaptionText", new ScanBluetoothView1());
            tabItems.Add(NearbyDevicesTabItem);

            ConnectedDeviceTabItem = CreateTabItem("ConnectedDeviceCaptionText",new CharacteristicView());
            tabItems.Add(ConnectedDeviceTabItem);

            sfTabView.Items = tabItems;
             


        }
       
        protected SfTabItem CreateTabItem(string captionCode,View content)
        {
             var TabItem = new SfTabItem();
            TabItem.Title = devicesViewModel.Resources[captionCode];
            TabItem.Content = content;// new ScanBluetoothView1();
            return TabItem;
        }
        
        protected override void OnAppearing()
        {

            base.OnAppearing();
            if (!tabItems.Contains(NearbyDevicesTabItem))
                tabItems.Add(NearbyDevicesTabItem);

            if (!tabItems.Contains(ConnectedDeviceTabItem))
                tabItems.Add(ConnectedDeviceTabItem);
            //    tabView.SelectedIndex = 0;

        }
        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    if (devicesViewModel.IsVisibleLayout)
        //    {
        //        sfTabView.HeightRequest = height- sfTabView.TabHeight;
        //    }
        //    base.OnSizeAllocated(width, height);
        //}
    }
}