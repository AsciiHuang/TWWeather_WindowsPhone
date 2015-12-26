using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace TWWeather
{
    public partial class EditAttentionItemPage : PhoneApplicationPage
    {
        private EditAttentionItemPageViewModel viewModel = null;
        public EditAttentionItemPageViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new EditAttentionItemPageViewModel();
                }

                return viewModel;
            }
        }

        public EditAttentionItemPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (ViewModel.IsDataLoaded)
            {
                ViewModel.InitData();
            }
            base.OnNavigatedTo(e);
        }

        private void AreaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0)
            {
                ViewModel.ReFreshCheckStatus(idx);
                ((ListBox)sender).SelectedItem = null;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateAttentionArea();

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}