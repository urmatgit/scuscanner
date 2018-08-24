using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfDataGrid.XForms.iOS;
using Syncfusion.SfGauge.XForms.iOS;

using UIKit;

namespace SCUScanner.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            MR.Gestures.iOS.Settings.LicenseKey = "CB2F-LQLC-HAY5-7DMG-DSZZ-FAEX-RF5D-3RYN-FE74-4RN3-NVVD-34LH-DEMV";
            global::Xamarin.Forms.Forms.Init();
            MR.Gestures.iOS.Settings.LicenseKey = "CB2F-LQLC-HAY5-7DMG-DSZZ-FAEX-RF5D-3RYN-FE74-4RN3-NVVD-34LH-DEMV";
            KeyboardOverlapRenderer.Init();
            new SfDigitalGaugeRenderer();
            SfDataGridRenderer.Init();
            SfListViewRenderer.Init();
          
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            LoadApplication(new App());
            
            return base.FinishedLaunching(app, options);
        }
        // Details: https://github.com/wcoder/Xamarin.Plugin.DeviceOrientation#xamarinforms-ios
        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, IntPtr forWindow)
        {
            return Plugin.DeviceOrientation.DeviceOrientationImplementation.SupportedInterfaceOrientations;
        }
    }
}
