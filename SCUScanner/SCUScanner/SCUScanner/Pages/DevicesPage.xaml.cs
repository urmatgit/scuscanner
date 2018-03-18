﻿using SCUScanner.ViewModels;
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
using SCUScanner.Helpers;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DevicesPage : ContentPage
	{
        TabItemCollection tabItems = new TabItemCollection();
        DevicesViewModel devicesViewModel;
        public  SfTabView sfTabView
        {
            get;set;
        }
        SfTabItemEx NearbyDevicesTabItem;
        SfTabItemEx ConnectedDeviceTabItem;
        SfTabItemEx ConnectedDeviceSettingTabItem;
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
          


            NearbyDevicesTabItem = CreateTabItem("NearbyDevicesCaptionText", new ScanBluetoothView1());
            tabItems.Add(NearbyDevicesTabItem);
            NearbyDevicesTabItem.Index = tabItems.Count - 1;
            this.WhenAnyValue(vm => vm.devicesViewModel.Resources).Subscribe(val =>
            {
                var newtitle = val["NearbyDevicesCaptionText"];
                if (NearbyDevicesTabItem.Caption != newtitle)
                NearbyDevicesTabItem.Caption = newtitle;
                if (ConnectedDeviceTabItem != null)
                {
                    newtitle = val["ConnectedDeviceCaptionText"];
                    if (ConnectedDeviceTabItem.Caption != newtitle)
                    ConnectedDeviceTabItem.Caption = newtitle;
                }
                if (ConnectedDeviceSettingTabItem != null)
                {
                    newtitle = val["DeviceSettingsCaptionText"];
                    if (ConnectedDeviceSettingTabItem.Caption != newtitle)
                        ConnectedDeviceSettingTabItem.Caption = newtitle;
                }
            });
            //ConnectedDeviceTabItem = CreateTabItem("ConnectedDeviceCaptionText",new CharacteristicView());
            //tabItems.Add(ConnectedDeviceTabItem);

            sfTabView.Items = tabItems;
             


        }
        public void CreateDeviceSettingTabView(ScanResultViewModel scanResultViewModel)
        {

            ConnectedDeviceSettingTabItem = CreateTabItem("DeviceSettingsCaptionText", new DeviceSettingView(scanResultViewModel));

            tabItems.Add(ConnectedDeviceSettingTabItem);
            ConnectedDeviceSettingTabItem.Index = tabItems.Count - 1;
           // sfTabView.SelectedIndex = ConnectedDeviceSettingTabItem.Index;
        }
        public  void CreateCharacterTabView(ScanResultViewModel scanResultViewModel)
        {
            
            ConnectedDeviceTabItem = CreateTabItem("ConnectedDeviceCaptionText",new CharacteristicView(scanResultViewModel));

            tabItems.Add(ConnectedDeviceTabItem);
            ConnectedDeviceTabItem.Index = tabItems.Count - 1;
            sfTabView.SelectedIndex = ConnectedDeviceTabItem.Index;
            CreateDeviceSettingTabView(scanResultViewModel);
        }
        public void RemoveDeviceSettingTabView()
        {
            if (ConnectedDeviceSettingTabItem != null && tabItems.Contains(ConnectedDeviceSettingTabItem))
                tabItems.Remove(ConnectedDeviceSettingTabItem);
            //sfTabView.SelectedIndex = 0;
        }
        public void RemoveCharacterTabView()
        {
            if (ConnectedDeviceTabItem !=null && tabItems.Contains(ConnectedDeviceTabItem))
                tabItems.Remove(ConnectedDeviceTabItem);
            sfTabView.SelectedIndex = 0;
            RemoveDeviceSettingTabView();
        }
        protected SfTabItemEx CreateTabItem(string captionCode,View content)
        {
             var TabItem = new SfTabItemEx(sfTabView);
            TabItem.Caption = devicesViewModel.Resources[captionCode];
            
            TabItem.Content = content;// new ScanBluetoothView1();
            return TabItem;
        }
        
        protected override void OnAppearing()
        {

            base.OnAppearing();
            if (!tabItems.Contains(NearbyDevicesTabItem))
                tabItems.Add(NearbyDevicesTabItem);

            if (!tabItems.Contains(ConnectedDeviceTabItem) && ConnectedDeviceTabItem!=null)
                tabItems.Add(ConnectedDeviceTabItem);
            if (!tabItems.Contains(ConnectedDeviceSettingTabItem) && ConnectedDeviceSettingTabItem != null) 
                tabItems.Add(ConnectedDeviceSettingTabItem);
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