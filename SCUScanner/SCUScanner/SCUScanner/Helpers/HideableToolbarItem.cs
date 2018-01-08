using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public class HideableToolbarItem : ToolbarItem
    {
        public static readonly BindableProperty IsVisibleProperty =
               BindableProperty.Create(nameof(IsVisible),
                                       typeof(bool),
                                       typeof(HideableToolbarItem),
                                       true);
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
        //public bool IsVisible
        //{
        //    get { return (bool)GetValue(IsVisibleProperty); }
        //    set { SetValue(IsVisibleProperty, value); }
        //}

        //public static readonly BindableProperty IsVisibleProperty =
        //  BindableProperty.Create(nameof(IsVisible),
        //    typeof(bool),
        //    typeof(HideableToolbarItem),
        //    true,
        //    propertyChanged: OnIsVisibleChanged);

        //private string oldText = "";
        //private System.Windows.Input.ICommand oldCommand = null;

        //private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var item = bindable as HideableToolbarItem;

        //    var newValueBool = (bool)newValue;
        //    var oldValueBool = (bool)oldValue;

        //    if (!newValueBool && oldValueBool)
        //    {
        //        item.oldText = item.Text;
        //        item.oldCommand = item.Command;
        //        item.Text = "";
        //        item.Command = null;
        //    }

        //    if (newValueBool && !oldValueBool)
        //    {
        //        item.Text = item.oldText;
        //        item.Command = item.oldCommand;
        //    }
        //}
    }
}
