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
using System.Diagnostics;
using TWWeather.AppServices.Models;
using Microsoft.Phone.Tasks;
using System.Windows.Threading;
using Microsoft.Phone.Scheduler;
using TWWeather.TileServices;
using Microsoft.Phone.Shell;

namespace TWWeather
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Phone.Controls.ProgressIndicator m_Progress;

        private DispatcherTimer timerShowTime = null;
        private static MainViewModel viewModel = null;
        public static MainViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = new MainViewModel();
                }

                return viewModel;
            }
        }

        // 建構函式
        public MainPage()
        {
            InitializeComponent();

            if (m_Progress == null)
            {
                m_Progress = new Phone.Controls.ProgressIndicator();
                m_Progress.ProgressType = ProgressTypes.WaitCursor;
                m_Progress.Text = "讀取中…請稍候…";
            }

            DataContext = ViewModel;

            timerShowTime = new DispatcherTimer();
            timerShowTime.Interval = TimeSpan.FromSeconds(20.0);
            timerShowTime.Tick += new EventHandler(OnShowTimerTick);
            timerShowTime.Start();

            ViewModel.LoadDataStarted += OnLoadDataStarted;
            ViewModel.LoadDataCompleted += OnLoadDataCompleted;
            viewModel.LoadDataError += OnLoadDataError;
        }

        // 載入 ViewModel 項目的資料
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsDataLoaded)
            {
                ViewModel.LoadData();
                CheckTaskAgent();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (ViewModel.IsDataLoaded)
            {
                ViewModel.UpdateMyWeatherPage();
            }

            base.OnNavigatedTo(e);
        }

        private void OnShowTimerTick(object sender, EventArgs e)
        {
            ViewModel.RefreshTime();
        }

        private void OnLoadDataStarted()
        {
            m_Progress.Show();
        }

        private void OnLoadDataCompleted()
        {
            m_Progress.Hide();
            if (!AppService.Instance.NoticeTile)
            {
                MessageBox.Show("提醒您，在喜好項目上長按可將該項目釘選至桌面(需支援背景執行)。");
            }
            TileService.Instance.RefreshAllTile();
        }

        public void OnLoadDataError()
        {
            m_Progress.Hide();
            MessageBox.Show("目前沒有資料，或網路發生錯誤，請稍侯再試。");
        }

        private void CheckTaskAgent()
        {
            // 看看用戶有沒有釘選關注地區
            Boolean bFindTile = TileService.Instance.FindAreaLiveTile();
            if (bFindTile)
            {
                // 若有釘選就檢查 TaskAgent 有沒有活著
                if (!UtilityHelper.CheckScheduledAgent())
                {
                    // Task Agent 喚不起來，通知用戶檢查設定
                    MessageBox.Show("您的裝置拒絕 TW Weather 啟動背景作業。\n請先至系統設定頁開啟 TW Weather 的背景作業權限，否則動態磚將會更新失敗。");
                }
            }
        }

        #region 列表動作處理
        private void OnAttentionListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0 && idx < ViewModel.AttentionItems.Count)
            {
                HandleAttentionItemSelected(idx);
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void OnExploreListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0 && idx < ViewModel.ExploreItems.Count)
            {
                SimpleListItem item = ViewModel.GetExploreItem(idx);
                HandleExploreItemSelected(item);
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void OnOthersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0 && idx < ViewModel.OthersItems.Count)
            {
                SimpleListItem item = ViewModel.GetOthersItem(idx);
                HandleOtherItemSelected(item);
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void HandleAttentionItemSelected(int idx)
        {
            AreaWeatherListItem item = ViewModel.GetAttentionItem(idx);
            String areaName = item.Area;
            AppService.Instance.CacheForecastData(areaName);

            Uri uriExplore = UtilityHelper.GetExplorePageURL(item.Area, item.URL, item.ItemType, item.SubItemTemplate);
            NavigationService.Navigate(uriExplore);
        }

        private void HandleExploreItemSelected(SimpleListItem item)
        {
            Uri uriExplore = UtilityHelper.GetExplorePageURL(item.Title, item.URL, item.ItemType, item.SubItemTemplate);
            NavigationService.Navigate(uriExplore);
        }

        private void HandleOtherItemSelected(SimpleListItem item)
        {
            switch (item.ItemType)
            {
                case WeatherItemType.WI_TYPE_BLANKWEB:
                    UtilityHelper.ShowWebBrowser(item.URL);
                    break;
                case WeatherItemType.WI_TYPE_CALL:
                    CallPhoneNumber(item.URL);
                    break;
                case WeatherItemType.WI_TYPE_ABOUT:
                    ShowAbout();
                    break;
                case WeatherItemType.WI_TYPE_MAILTO:
                    MailTo(item.URL);
                    break;
                case WeatherItemType.WI_TYPE_SETTING:
                    ShowSetting();
                    break;
                case WeatherItemType.WI_TYPE_OTHER:
                    break;
            }
        }

        private void CallPhoneNumber(String number)
        {
            String phoneNumber = number.Replace("-", "");

            PhoneCallTask phoneCallTask = new PhoneCallTask();
            phoneCallTask.PhoneNumber = phoneNumber;
            phoneCallTask.Show();
        }

        private void ShowAbout()
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ShowSetting()
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void MailTo(String mailAddress)
        {
            EmailComposeTask mailTask = new EmailComposeTask();
            mailTask.To = mailAddress;
            mailTask.Subject = "TWWeather Freeback";
            mailTask.Show();
        }
        #endregion

        private void menuItem_Reflash_Click(object sender, EventArgs e)
        {
            ViewModel.LoadData();
        }

        private void menuItem_Edit_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(UtilityHelper.GetEditAttentionItemPageUri());
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((Panorama)sender).SelectedIndex;
            if (idx == 0)
            {
                ApplicationBar.IsVisible = true;
            }
            else
            {
                ApplicationBar.IsVisible = false;
            }
        }

        private void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem temp = (MenuItem)sender;
            AreaWeatherListItem item = (AreaWeatherListItem)temp.DataContext;

            if (item != null)
            {
                if (!"".Equals(item.Area))
                {
                    ViewModel.RemoveAttentionItem(item.Area);
                    viewModel.RemoveTile(item.Area);
                }
            }
        }

        private void ContextMenuSetTile_Click(object sender, RoutedEventArgs e)
        {
            MenuItem temp = (MenuItem)sender;
            AreaWeatherListItem item = (AreaWeatherListItem)temp.DataContext;

            // 檢查 scheduledAgent 是否活著…若是死的就試著喚醒…
            if (UtilityHelper.CheckScheduledAgent())
            {
                if (item != null)
                {
                    if (!"".Equals(item.Area))
                    {
                        #region 若沒有此動態磚就加入，過程發生錯誤也沒關係，Agent 會幫忙把資訊補回來
                        String strTileKeyWord = String.Format(Constants.TILE_NAVIGATION_KEYWORD, item.Area);
                        ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(strTileKeyWord));
                        if (tile == null)
                        {
                            String strAreaMD5 = MD5Core.GetHashString(item.Area);
                            String strBackgroundImageFileName = String.Format("Fore_{0}.jpg", strAreaMD5);
                            String strBackBackgroundImageFileName = String.Format("Back_{0}.jpg", strAreaMD5);
                            String strBackgroundImageFilePath = UtilityHelper.GetShareToTileAlbumCoverPath(strBackgroundImageFileName);
                            String strBackBackgroundImageFilePath = UtilityHelper.GetShareToTileAlbumCoverPath(strBackBackgroundImageFileName);
                            String strNavigationURL = String.Format(Constants.TILE_NAVIGATION_URL, item.Area);

                            try
                            {
                                StandardTileData NewTileData = new StandardTileData
                                {
                                    BackgroundImage = new Uri("isostore:/" + strBackgroundImageFilePath, UriKind.Absolute),
                                    BackBackgroundImage = new Uri("isostore:/" + strBackBackgroundImageFilePath, UriKind.Absolute)
                                };
                                ShellTile.Create(new Uri(strNavigationURL, UriKind.Relative), NewTileData);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        #endregion

                        // 加入此地區的動態磚
                        TileService.Instance.UpdateTile(item.Area);
                    }
                }
            }
            else
            {
                // 開不了…提示
                MessageBox.Show("您的裝置拒絕 TW Weather 啟動背景作業。\n請先至系統設定頁開啟 TW Weather 的背景作業權限，否則動態磚將會更新失敗。");
            }
        }
    }
}