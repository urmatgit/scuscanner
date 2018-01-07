using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
   public class BaseDetailPage : ContentPage
    {
        public IList<HideableToolbarItem> CustomToolbar { get; private set; }

        public BaseDetailPage()
        {
            var items = new ObservableCollection<HideableToolbarItem>();
            items.CollectionChanged += ToolbarItemsChanged;

            CustomToolbar = items;
        }

        private void ToolbarItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ToolbarItems.Clear();

            foreach (var item in CustomToolbar)
            {
                item.PropertyChanged += OnToolbarItemPropertyChanged;
                if (item.IsVisible)
                {
                    ToolbarItems.Add(item);
                }
            }
        }

        private void OnToolbarItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == HideableToolbarItem.IsVisibleProperty.PropertyName)
            {
                UpdateToolbar();
            }
        }

        private void UpdateToolbar()
        {
            foreach (var item in CustomToolbar)
            {
                if (item.IsVisible)
                {
                    ToolbarItems.Add(item);
                }
                else
                {
                    ToolbarItems.Remove(item);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ToolbarItems.Clear();
            CustomToolbar.Clear();
            foreach (var item in CustomToolbar)
            {
                item.PropertyChanged -= OnToolbarItemPropertyChanged;
            }
        }
    }
}
