﻿using Acr.UserDialogs;
using MvvmCross;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using Plugin.Settings;
using SCUScanner.Services;
using SCUScanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeviceListPage : BaseTabbedPage
    {
        DeviceListViewModel deviceListViewModel;
        public DeviceListPage ()
		{
			InitializeComponent ();
            var adapter = CrossBluetoothLE.Current.Adapter;
           var  _bluetoothLe = CrossBluetoothLE.Current; //  Mvx.Resolve<IBluetoothLE>();
            var _userDialogs = UserDialogs.Instance;//  Mvx.Resolve<IUserDialogs>();
           var  _settings = CrossSettings.Current;
            var _permission = CrossPermissions.Current;
            BindingContext = deviceListViewModel = new DeviceListViewModel(this, _bluetoothLe, adapter,_userDialogs,_settings, _permission);
            
            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            {

                var arg = agr;
                if (arg is CultureChangedMessage)
                {
                    // BindingContext = null;
                    SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;
                    this.Title = settings.Resources["MainText"];
                    if (App.mainTabbed != null)
                    {
                        App.mainTabbed.Title = settings.Resources["MainText"];
                        //   scanBluetoothViewModel.ScanTextChange(scanBluetoothViewModel.IsScanning);
                    }
                }
            });

        }
        public BasePage DeviceListTab
        {
            get => MainTabPage;
        }
        public BasePage ConnectDeviceTab
        {
            get => ConnectedTabPage;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            (this.BindingContext as IViewModel)?.OnActivate();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (this.BindingContext as IViewModel)?.OnDeactivate();
        }
    }
}