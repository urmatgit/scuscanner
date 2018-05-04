using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SCUScanner.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]
namespace SCUScanner.Droid.Controls
{
   public class ExtendedTabbedPageRenderer : TabbedPageRenderer
    {
        private Android.Views.View formViewPager = null;
        private TabLayout tabLayout = null;

        public ExtendedTabbedPageRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            this.tabLayout = (TabLayout)this.GetChildAt(1);

            changeTabsFont();
        }

        private void changeTabsFont()
        {
            //Typeface font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "fonts/" + Constants.FontStyle);
            ViewGroup vg = (ViewGroup)tabLayout.GetChildAt(0);
            int tabsCount = vg.ChildCount;
            for (int j = 0; j < tabsCount; j++)
            {
                ViewGroup vgTab = (ViewGroup)vg.GetChildAt(j);
                int tabChildsCount = vgTab.ChildCount;
                for (int i = 0; i < tabChildsCount; i++)
                {
                    Android.Views.View tabViewChild = vgTab.GetChildAt(i);
                    if (tabViewChild is TextView)
                    {
                        //((TextView)tabViewChild).Typeface = font;
                        ((TextView)tabViewChild).TextSize = 10f;

                    }
                }
            }
        }
    }
}