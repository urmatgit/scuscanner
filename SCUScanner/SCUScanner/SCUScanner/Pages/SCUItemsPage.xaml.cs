using SCUScanner.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SCUScanner.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SCUItemsPage : ContentPage
    {
        
        bool isSettingsOpen=false;

        public SCUItemsPage()
        {
            InitializeComponent();
            //BindingContext = new SCUDatasViewModel();
            FillOptionList();
        }
        private void FillOptionList()
        {
            foreach (var col in dataGrid.Columns)
            {
                ColumnsList.Items.Add(col.HeaderText);
            }
            SCUDatasViewModel viewModel = this.BindingContext as SCUDatasViewModel;
            if (viewModel!=null)
            ColumnsList.SelectedIndex =  viewModel.SettingsBase.SelectedColumnIndex;// 0;
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            contentGrid.HeightRequest = height;
            base.OnSizeAllocated(width, height);
        }
        public void OptionClick(object sender, EventArgs e)
        {
            if (isSettingsOpen)
            {
                isSettingsOpen = false;
                ClosePropertiesView();
            }
            else
            {
                OpenPropertiesView();
                isSettingsOpen = true;
            }
        }
        private void ClosePropertiesView()
        {
             
                propertyView.TranslateTo(0, propertyView.Height, 400, Easing.Linear);
            contentGrid.Opacity = 1;
           
        }
        public void OpenPropertiesView()
        {
            
                propertyView.TranslateTo(0, -propertyView.Height, 400, Easing.Linear);
            contentGrid.Opacity = 0.5;
            
        }
        public void PropertiesCloseButton_Clicked(object sender, EventArgs e)
        {
            ClosePropertiesView();
            isSettingsOpen = false;
        }

    }
}
