﻿
using Plugin.BluetoothLE;
using SCUScanner.Helpers;
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
	public partial class ScanBluetoothPage : ContentPage
    {
        bool IsBluetoothEnabled = false;
        

		public ScanBluetoothPage ()
		{
			InitializeComponent ();
            //MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, (sender, agr) =>
            //{

            //    var arg = agr;
            //    if (arg is CultureChangedMessage)
            //    {
            //        BindingContext = null;
            //        SCUScanner.Models.Settings settings = sender as SCUScanner.Models.Settings;

            //    }
            //});
            BindingContext = new ScanBluetoothViewModel();
            //            Init();
        }

//        private void Init()
//        {
//#if DEBUG
//            IsBluetoothEnabled = true;
//#endif
//            if (IsBluetoothEnabled)
//            {
//                var toolScan = new ToolbarItem();
//                var bindingText = new Binding(path: "Resources[ScanText]");

//        }
    }
}