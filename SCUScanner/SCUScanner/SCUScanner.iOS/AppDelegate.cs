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

            UINavigationBar.Appearance.BarTintColor = FromHexString("#211e1e");//  UIColor.Blue;
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
            UINavigationBar.Appearance.Translucent = false;
            LoadApplication(new App());
            
            return base.FinishedLaunching(app, options);
        }
        // Details: https://github.com/wcoder/Xamarin.Plugin.DeviceOrientation#xamarinforms-ios
        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, IntPtr forWindow)
        {
            return Plugin.DeviceOrientation.DeviceOrientationImplementation.SupportedInterfaceOrientations;
        }
        public   UIColor FromHexString(string hexValue, float alpha = 1.0f)
        {
            try
            {
                string colorString = hexValue.Replace("#", "");

                float red, green, blue;

                switch (colorString.Length)
                {
                    case 3: // #RGB
                        {
                            red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                            green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                            blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                            return UIColor.FromRGBA(red, green, blue, alpha);
                        }
                    case 6: // #RRGGBB
                        {
                            red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                            green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                            blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                            return UIColor.FromRGBA(red, green, blue, alpha);
                        }
                    case 8: // #RRGGBBAA
                        {
                            red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                            green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                            blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                            alpha = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;

                            return UIColor.FromRGBA(red, green, blue, alpha);
                        }

                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

                }
            }
            catch (Exception genEx)
            {
                return null;
            }
        }
    }
}
