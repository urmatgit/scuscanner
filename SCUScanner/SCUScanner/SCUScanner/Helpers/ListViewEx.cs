using SCUScanner.Services;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.Helpers
{
   public class ListViewEx : Xamarin.Forms.ListView
    {
        public static readonly BindableProperty ItemClickCommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ListView)
        );

        public ListViewEx()
        {
            this.ItemTapped += this.OnItemTapped;
            this.ItemAppearing += this.OnItemAppearing;
            this.ItemDisappearing += this.OnItemDisappearing;
        }


        protected virtual void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
            => (e.Item as IViewModel)?.OnActivate();


        protected virtual void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
            => (e.Item as IViewModel)?.OnDeactivate();


        public bool DisableRowSelection { get; set; } = true;


        public ICommand ItemClickCommand
        {
            get => (ICommand)this.GetValue(ItemClickCommandProperty);
            set => this.SetValue(ItemClickCommandProperty, value);
        }


        void OnItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (this.DisableRowSelection)
                this.SelectedItem = null;

            if (e.Item != null && this.ItemClickCommand != null && this.ItemClickCommand.CanExecute(e))
            {
                this.ItemClickCommand.Execute(e.Item);
                this.SelectedItem = null;
            }
        }
    }
    public class sfListViewEx : SfListView
    {
        public static readonly BindableProperty ItemClickCommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ListView)
        );

        public sfListViewEx()
        {
            this.ItemTapped += SfListViewEx_ItemTapped;
          //  this.ItemTapped += this.OnItemTapped;
        //    this.ItemAppearing += this.OnItemAppearing;
            this.ItemAppearing += SfListViewEx_ItemAppearing;
            //this.ItemDisappearing += this.OnItemDisappearing;
            this.ItemDisappearing += SfListViewEx_ItemDisappearing;
        }

        private void SfListViewEx_ItemDisappearing(object sender, ItemDisappearingEventArgs e)
            => (e.ItemData as IViewModel)?.OnDeactivate();

        private void SfListViewEx_ItemAppearing(object sender, ItemAppearingEventArgs e)
        
             => (e.ItemData as IViewModel)?.OnActivate();
        

        private void SfListViewEx_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            if (this.DisableRowSelection)
                this.SelectedItem = null;

            if (e.ItemData != null && this.ItemClickCommand != null && this.ItemClickCommand.CanExecute(e))
            {
                this.ItemClickCommand.Execute(e.ItemData);
                this.SelectedItem = null;
            }
        }

        //protected virtual void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        //    => (e.Item as IViewModel)?.OnActivate();


        protected virtual void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
            => (e.Item as IViewModel)?.OnDeactivate();


        public bool DisableRowSelection { get; set; } = true;


        public ICommand ItemClickCommand
        {
            get => (ICommand)this.GetValue(ItemClickCommandProperty);
            set => this.SetValue(ItemClickCommandProperty, value);
        }

        
        
    }
}
