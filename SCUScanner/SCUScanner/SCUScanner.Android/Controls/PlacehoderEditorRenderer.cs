using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SCUScanner.Droid.Controls;
using SCUScanner.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(PlacehoderEditorRenderer))]
namespace SCUScanner.Droid.Controls
{
    public class PlacehoderEditorRenderer : EditorRenderer
    {
        public PlacehoderEditorRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            var element = (PlaceholderEditor)Element;

            Control.Hint = element.Placeholder;
            Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
        }
    }
}