using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CentriClean.Controls;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(ContentPage), typeof(NoBackSwipeRenderer))]
namespace CentriClean.Controls
{
    
    public class NoBackSwipeRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (ViewController != null && ViewController.NavigationController != null)
                ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = false;
        }
    }
}