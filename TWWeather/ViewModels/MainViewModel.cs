using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using TWWeather.AppServices;
using TWWeather.AppServices.Models;
using System.Net;
using System.Globalization;
using TWWeather.TileServices;
using Microsoft.Phone.Shell;
using System.Linq;

namespace TWWeather
{
    public class MainViewModel : ViewModel
    {
        private Boolean IsDarkTheme = true;

        public delegate void ViewActionHandler();
        public event ViewActionHandler LoadDataStarted;
        public event ViewActionHandler LoadDataCompleted;
        public event ViewActionHandler LoadDataError;

        public MainViewModel()
        {
            AttentionItems = new ObservableCollection<AreaWeatherListItem>();
            ExploreItems = new ObservableCollection<SimpleListItem>();
            OthersItems = new ObservableCollection<IconListItem>();

            if ((double)Application.Current.Resources["PhoneDarkThemeOpacity"] == 1)
            {
                IsDarkTheme = true;
            }
            else
            {
                IsDarkTheme = false;
            }

            RefreshTime();
            InitData();
        }

        public ObservableCollection<AreaWeatherListItem> AttentionItems { get; private set; }
        public ObservableCollection<SimpleListItem> ExploreItems { get; private set; }
        public ObservableCollection<IconListItem> OthersItems { get; private set; }

        #region Properties
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public String AttentionPageTitle
        {
            get
            {
                return "喜好項目";
            }
        }

        public String WeatherExplorePageTitle
        {
            get
            {
                return "氣象總覽";
            }
        }

        private Visibility _showWarningPage = Visibility.Collapsed;
        public Visibility ShowWarningPage
        {
            get
            {
                return _showWarningPage;
            }
            set
            {
                _showWarningPage = value;
                NotifyPropertyChanged("ShowWarningPage");
            }
        }

        public String WarningPageTitle
        {
            get
            {
                return "天氣警報";
            }
        }

        public String NewsPageTitle
        {
            get
            {
                return "氣象報告";
            }
        }

        public String OthersPageTitle
        {
            get
            {
                return "更多";
            }
        }

        public Visibility IsWarning
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        public BitmapImage SmallIconImage
        {
            get
            {
                if (IsDarkTheme)
                {
                    return new BitmapImage(new Uri("/Image/s_icon_dark.png", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("/Image/s_icon_light.png", UriKind.Relative));
                }
            }
        }

        private String _warningText;
        public String WarningText
        {
            get
            {
                return _warningText;
            }
            set
            {
                _warningText = value;
                NotifyPropertyChanged("WarningText");
            }
        }

        private String _weatherNewsText;
        public String WeatherNewsText
        {
            get
            {
                return _weatherNewsText;
            }
            set
            {
                _weatherNewsText = value;
                NotifyPropertyChanged("WeatherNewsText");
            }
        }

        private String _nowTime;
        public String NowTime
        {
            get
            {
                return _nowTime;
            }
            set
            {
                _nowTime = value;
                NotifyPropertyChanged("NowTime");
            }
        }
        #endregion

        public void InitData()
        {
            #region 將地區列表讀出來塞給 AppService
            List<SimpleListItem> listArea = XMLListDataReader.ReadListData(new Uri("Data/ForecastList.xml", UriKind.Relative));
            AppService.Instance.UpdateKnownsAreaList(listArea);

            if (AppService.Instance.FirstLaunch)
            {
                // 第一次啟動時將台北市、高雄市加入喜好項目
                AppService.Instance.AppendDefaultArea();
            }

            if (AppService.Instance.FirstLaunchTile)
            {
                // 1.2 版加入動態磚…與 Agent 共用的資訊要存成檔案
                // 故將放在 setting 中的關注項目搬到檔案中
                AppService.Instance.MoveAttentionAreaFromSettingToFile();
            }
            #endregion

            #region 總覽
            List<SimpleListItem> listOverView = XMLListDataReader.ReadListData(new Uri("Data/MainExplore.xml", UriKind.Relative));
            foreach (SimpleListItem item in listOverView)
            {
                ExploreItems.Add(item);
            }
            #endregion

            #region 更多
            OthersItems.Add(new IconListItem()
                {
                    Icon = "Image/goto.png", 
                    Title = "中央氣象局網頁",
                    URL = "http://www.cwb.gov.tw", 
                    ItemType = WeatherItemType.WI_TYPE_BLANKWEB, 
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SMALL_ICON
                });
            OthersItems.Add(new IconListItem()
                {
                    Icon = "Image/goto.png",
                    Title = "中央氣象局網頁 PDA 版",
                    URL = "http://www.cwb.gov.tw/pda/",
                    ItemType = WeatherItemType.WI_TYPE_BLANKWEB,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SMALL_ICON
                });
            OthersItems.Add(new IconListItem()
                {
                    Icon = "Image/tel.png",
                    Title = "氣象查詢：886-2-23491234",
                    URL = "+886223491234",
                    ItemType = WeatherItemType.WI_TYPE_CALL,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SMALL_ICON
                });
            OthersItems.Add(new IconListItem()
                {
                    Icon = "Image/tel.png",
                    Title = "氣象查詢：886-2-23491168",
                    URL = "+886223491168",
                    ItemType = WeatherItemType.WI_TYPE_CALL,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SMALL_ICON
                });
            OthersItems.Add(new IconListItem()
                {
                    Title = "",
                    ItemType = WeatherItemType.WI_TYPE_NON,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SPACE
                });
            //OthersItems.Add(new IconListItem()
            //    {
            //        Title = "設定",
            //        URL = "SettingPage.xaml",
            //        ItemType = WeatherItemType.WI_TYPE_SETTING,
            //        ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SIMPLE
            //    });
            OthersItems.Add(new IconListItem()
                {
                    Title = "關於",
                    URL = "AboutPage.xaml",
                    ItemType = WeatherItemType.WI_TYPE_ABOUT,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SIMPLE
                });
            OthersItems.Add(new IconListItem()
                {
                    Title = "意見反應",
                    URL = "asciiss@gmail.com",
                    ItemType = WeatherItemType.WI_TYPE_MAILTO,
                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SIMPLE
                });
            #endregion
        }

        public void LoadData()
        {
            // 取喜好項目、氣象報告

            if (LoadDataStarted != null)
            {
                LoadDataStarted();
            }

            BaseAPI apiWarning = new BaseAPI();
            apiWarning.APILoadCompleted += OnWarningAPILoadCompleted;
            apiWarning.GetStringResponse(Constants.WEATHER_WARNING_URL);

            BaseAPI apiOverView = new BaseAPI();
            apiOverView.APILoadCompleted += OnOverViewAPILoadCompleted;
            apiOverView.GetStringResponse(Constants.WEATHER_OVERVIEW_URL);

            BaseAPI apiForecast = new BaseAPI();
            apiForecast.APILoadCompleted += OnForecastAPILoadCompleted;
            apiForecast.GetStringResponse(Constants.WEATHER_FORECAST_URL);

            IsDataLoaded = true;
        }

        public void RefreshTime()
        {
            NowTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm", CultureInfo.InvariantCulture);
        }

        public AreaWeatherListItem GetAttentionItem(int idx)
        {
            return AttentionItems[idx];
        }

        public SimpleListItem GetExploreItem(int idx)
        {
            return ExploreItems[idx];
        }

        public SimpleListItem GetOthersItem(int idx)
        {
            return OthersItems[idx];
        }

        public void UpdateMyWeatherPage()
        {
            AttentionItems.Clear();
            
            Dictionary<String, List<RichListItem>> allForecast = AppService.Instance.mAreaWeatherList;
            List<String> listAttentionArea = AppService.Instance.GetLocalAttentionAreaList();

            if (allForecast == null || allForecast.Count <= 0)
            {
                if (LoadDataError != null)
                {
                    LoadDataError();
                }
            }
            else
            {
                foreach (String area in listAttentionArea)
                {
                    if (allForecast.ContainsKey(area))
                    {
                        // 找得到這個地區
                        List<RichListItem> items = allForecast[area];
                        TIMEBLOCK timeBlock = 0;
                        if (items.Count > 0)
                        {
                            timeBlock = UtilityHelper.NowTargetTimeBlock(items);
                        }

                        if (items.Count > (int)timeBlock)
                        {
                            AreaWeatherListItem areaItem = new AreaWeatherListItem();
                            areaItem.Title = items[(int)timeBlock].Title;
                            areaItem.Description = items[(int)timeBlock].Description;
                            areaItem.Area = items[(int)timeBlock].Area;
                            areaItem.Temperature = String.Format("{0}{1}", items[(int)timeBlock].Temperature, Constants.SUFFIX_TEMPERATURE);
                            areaItem.ChanceOfRain = String.Format("{0}{1}{2}", Constants.PREFIX_CHANCE_OF_RAIN, items[(int)timeBlock].ChanceOfRain, Constants.SUFFIX_CHANCE_OF_RAIN);
                            areaItem.StartTime = items[(int)timeBlock].StartTime;
                            areaItem.EndTime = items[(int)timeBlock].EndTime;
                            areaItem.ItemType = WeatherItemType.WI_TYPE_USE_APPSERVICE_DATA;
                            areaItem.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_ATTENTION; // 這裡白寫了…因為這個 Template 只用一次
                            areaItem.SubItemTemplate = WeatherItemTemplate.WI_TEMPLATE_AREA; // 若點下項目，下一層要要用 AreaItem 來秀
                            String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(areaItem.Description, areaItem.Title);
                            areaItem.Icon = strWeatherImage;
                            AttentionItems.Add(areaItem);
                        }
                    }
                }
            }
            NotifyPropertyChanged("MyWeatherItems");
        }

        public void OnWarningAPILoadCompleted(String result)
        {
            if (result == null || "".Equals(result))
            {
                ShowWarningPage = Visibility.Collapsed;
            }
            else
            {
                // 有重要資訊
                String fullWarningText = WarningParser.Parse(result);
                if (!"".Equals(fullWarningText))
                {
                    WarningText = fullWarningText;
                    ShowWarningPage = Visibility.Visible;
                }
            }
        }

        public void OnOverViewAPILoadCompleted(String result)
        {
            WeatherNewsText = result.Trim();
            NotifyPropertyChanged("WeatherNewsText");
        }

        public void OnForecastAPILoadCompleted(String result)
        {
            Boolean bIsRemember = AppService.Instance.GetIsRemember();
            Boolean bResultOK = true;
            Boolean bLocalData = false;

            if ((result == null || "".Equals(result)) && bIsRemember)
            {
                String strLocalData = AppService.Instance.GetBackUpAllForecast();

                if (strLocalData == null || "".Equals(strLocalData))
                {
                    bResultOK = false;
                    if (LoadDataError != null)
                    {
                        LoadDataError();
                    }
                }
                else
                {
                    bLocalData = true;
                    result = strLocalData;
                }
            }
            else if (result == null || "".Equals(result))
            {
                bResultOK = false;
                if (LoadDataError != null)
                {
                    LoadDataError();
                }
            }

            if (bResultOK)
            {
                Dictionary<String, List<RichListItem>> allForecast = ForecastParser.ParseAllForecast(result);
                AppService.Instance.mAreaWeatherList = allForecast;
                TileService.Instance.mAreaWeatherList = allForecast;

                if (allForecast != null && allForecast.Count > 0 && !bLocalData)
                {
                    AppService.Instance.SaveBackUpAllForecast(result);
                }

                UpdateMyWeatherPage();
            }

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted();
            }
        }

        public void RemoveAttentionItem(String area)
        {
            AppService.Instance.RemoveAttentionArea(area);
            UpdateMyWeatherPage();
        }

        public void RemoveTile(String area)
        {
            String strTileKeyWord = String.Format(Constants.TILE_NAVIGATION_KEYWORD, area);
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(strTileKeyWord));
            if (tile != null)
            {
                try
                {
                    tile.Delete();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}