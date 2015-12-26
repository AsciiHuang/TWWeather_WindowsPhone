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
using Phone.Controls;
using TWWeather.AppServices.Models;

namespace TWWeather
{
    public partial class ExplorePage : PhoneApplicationPage
    {
        private ProgressIndicator m_Progress;

        private ExplorePageModel viewModel = null;
        public ExplorePageModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new ExplorePageModel();
                }

                return viewModel;
            }
        }

        public ExplorePage()
        {
            InitializeComponent();
            if (m_Progress == null)
            {
                m_Progress = new Phone.Controls.ProgressIndicator();
                m_Progress.ProgressType = ProgressTypes.WaitCursor;
                m_Progress.Text = "讀取中…請稍候…";
            }

            DataContext = ViewModel;

            ViewModel.LoadDataStarted += OnLoadDataStarted;
            ViewModel.LoadDataCompleted += OnLoadDataCompleted;
        }

        private void OnLoadDataStarted()
        {
            m_Progress.Show();
        }

        private void OnLoadDataCompleted()
        {
            m_Progress.Hide();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            IDictionary<String, String> queryString = this.NavigationContext.QueryString;

            if (queryString.ContainsKey("title"))
            {
                ViewModel.PageTitle = queryString["title"];
            }
            if (queryString.ContainsKey("url"))
            {
                ViewModel.ContentURL = queryString["url"];
            }
            if (queryString.ContainsKey("type"))
            {
                String strType = queryString["type"];
                ViewModel.ActionType = (WeatherItemType)int.Parse(strType);
            }
            if (queryString.ContainsKey("template"))
            {
                String strTemplate = queryString["template"];
                ViewModel.Template = (WeatherItemTemplate)int.Parse(strTemplate);
            }
            if (!ViewModel.IsDataLoaded)
            {
                ViewModel.LoadData();
            }
        }

        private void ExploreList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0 && idx < ViewModel.ListItemSource.Count)
            {
                SimpleListItem item = ViewModel.GetOverviewItem(idx);
                HandleItemSelected(item);
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void HandleItemSelected(SimpleListItem item)
        {
            if (item.ItemType == WeatherItemType.WI_TYPE_BLANKWEB)
            {
                if(!"".Equals(item.URL))
                {
                    UtilityHelper.ShowWebBrowser(item.URL);
                }
            }
            else if (item.ItemType != WeatherItemType.WI_TYPE_NON)
            {
                Uri uriExplore = UtilityHelper.GetExplorePageURL(item.Title, item.URL, item.ItemType, item.SubItemTemplate);
                NavigationService.Navigate(uriExplore);
            }
        }
    }
}