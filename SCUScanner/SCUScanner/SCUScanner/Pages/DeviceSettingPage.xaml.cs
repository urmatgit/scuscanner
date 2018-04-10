﻿using SCUScanner.ViewModels;
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
	public partial class DeviceSettingPage : BaseTabPage
    {
		public DeviceSettingPage (ScanResultViewModel selectedDevice)
		{
			InitializeComponent ();
            BindingContext = new DeviceSettingViewModel(selectedDevice);

        }
	}
}