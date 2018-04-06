using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;
using SCUScanner.ViewModels;

namespace SCUScanner.Pages
{
    public partial class SCUItemsListPage : ContentPage
    {
        SCUItemsViewModel viewModel;
        public SCUItemsListPage(string unitName)
        {
            InitializeComponent();
            pullToRefresh.Refreshing += PullToRefresh_Refreshing;
            
            BindingContext = viewModel = new SCUItemsViewModel(unitName);
        }

        private async void PullToRefresh_Refreshing(object sender, EventArgs e)
        {
            pullToRefresh.IsRefreshing = true;
            await Task.Delay(1000);
            try
            {
                await viewModel.GetNextItems();
            }
            finally
            {
                pullToRefresh.IsRefreshing = false;
            }
        }
    }
}
