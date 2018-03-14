using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
    public class HideableToolbarItem : ToolbarItem
    {
        public HideableToolbarItem() : base()
        {
            this.InitVisibility();
        }

        private async void InitVisibility()
        {
            await Task.Delay(100);
            OnIsVisibleChanged1(this, false, IsVisible);
        }

        public ContentPage Parent { set; get; }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static BindableProperty IsVisibleProperty =
             BindableProperty.Create(nameof(IsVisible),
                                           typeof(bool),
                                           typeof(HideableToolbarItem),
                                           true,propertyChanged: OnIsVisibleChanged1);

        private static void OnIsVisibleChanged1(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as HideableToolbarItem;

            if (item.Parent == null)
                return;

            var items = item.Parent.ToolbarItems;

            if ( (bool)newValue && !items.Contains(item))
            {
                items.Add(item);
            }
            else if (!(bool)newValue && items.Contains(item))
            {
                items.Remove(item);
            }
        }

        

      
        }
     
    //public static readonly BindableProperty IsVisibleProperty =
    //       BindableProperty.Create(nameof(IsVisible),
    //                               typeof(bool),
    //                               typeof(HideableToolbarItem),
    //                               true);
    //public bool IsVisible
    //{
    //    get { return (bool)GetValue(IsVisibleProperty); }
    //    set { SetValue(IsVisibleProperty, value); }
    //}
    ////public bool IsVisible
    ////{
    ////    get { return (bool)GetValue(IsVisibleProperty); }
    ////    set { SetValue(IsVisibleProperty, value); }
    ////}

    ////public static readonly BindableProperty IsVisibleProperty =
    ////  BindableProperty.Create(nameof(IsVisible),
    ////    typeof(bool),
    ////    typeof(HideableToolbarItem),
    ////    true,
    ////    propertyChanged: OnIsVisibleChanged);

    ////private string oldText = "";
    ////private System.Windows.Input.ICommand oldCommand = null;

    ////private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    ////{
    ////    var item = bindable as HideableToolbarItem;

    ////    var newValueBool = (bool)newValue;
    ////    var oldValueBool = (bool)oldValue;

    ////    if (!newValueBool && oldValueBool)
    ////    {
    ////        item.oldText = item.Text;
    ////        item.oldCommand = item.Command;
    ////        item.Text = "";
    ////        item.Command = null;
    ////    }

    ////    if (newValueBool && !oldValueBool)
    ////    {
    ////        item.Text = item.oldText;
    ////        item.Command = item.oldCommand;
    ////    }
    ////}

}
