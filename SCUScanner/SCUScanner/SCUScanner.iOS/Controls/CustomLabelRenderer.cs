using SCUScanner.iOS.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(handler: typeof(Xamarin.Forms.Label), target: typeof(CustomLabelRenderer))]
namespace SCUScanner.iOS.Controls
{
   public class CustomLabelRenderer : LabelRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                base.OnElementPropertyChanged(sender, e);
            }
        }
         
    }
}
