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
            BindingContext = viewModel = new SCUItemsViewModel(unitName);
        }
    }
}
