using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TWWeather.AppServices.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace TWWeather
{
    public class ExplorePageModel : ViewModel
    {
        public delegate void ViewActionHandler();
        public event ViewActionHandler LoadDataStarted;
        public event ViewActionHandler LoadDataCompleted;

        public String ContentURL;
        public WeatherItemType ActionType;
        public WeatherItemTemplate Template;

        public ExplorePageModel()
        {
            InitData();
            ContentURL = "";
            IsDataLoaded = false;
            ActionType = WeatherItemType.WI_TYPE_NON;
            ListItemSource = new ObservableCollection<RichListItem>();
        }
        
        public ObservableCollection<RichListItem> ListItemSource { get; private set; }

        #region Properties
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        private String _pageTitle;
        public String PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged("PageTitle");
            }
        }

        private String _emptyMessage = "目前沒有資料，或網路發生錯誤，請稍侯再試。";
        public String EmptyMessage
        {
            get
            {
                return _emptyMessage;
            }
            set
            {
                _emptyMessage = value;
                NotifyPropertyChanged("EmptyMessage");
            }
        }

        private Visibility _showEmpty = Visibility.Collapsed;
        public Visibility ShowEmpty
        {
            get
            {
                return _showEmpty;
            }
            set
            {
                _showEmpty = value;
                NotifyPropertyChanged("ShowEmpty");
            }
        }
        #endregion

        public void InitData()
        {
        }

        public void LoadData()
        {
            if (ActionType == WeatherItemType.WI_TYPE_NON)
            {
                return;
            }
            else
            {
                switch (ActionType)
                {
                    case WeatherItemType.WI_TYPE_USE_APPSERVICE_DATA:
                        ReflashByAppCache();
                        break;
                    case WeatherItemType.WI_TYPE_XML:
                        ReflashByLocalData();
                        break;
                    case WeatherItemType.WI_TYPE_JSON:
                        ReflashByURL();
                        break;
                }
            }
            IsDataLoaded = true;
        }

        public SimpleListItem GetOverviewItem(int idx)
        {
            return ListItemSource[idx];
        }

        private void HandleNullData(Boolean isNull)
        {
            if (isNull)
            {
                ShowEmpty = Visibility.Visible;
            }
            else
            {
                ShowEmpty = Visibility.Collapsed;
            }
        }

        #region 三種填入資料的方式
        private void ReflashByAppCache()
        {
            List<RichListItem> items = AppService.Instance.mCacheList;
            if (items == null || items.Count <= 0)
            {
                HandleNullData(true);
            }
            else
            {
                switch (Template)
                {
                    case WeatherItemTemplate.WI_TEMPLATE_AREA:
                        AddForecastItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_OBS:
                        AddOBSItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_WEEK:
                        AddWeekItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SEA:
                        AddSeaItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_NEARSEA:
                        AddNearSeaItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_TIDE:
                        AddTideItems(items);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_GLOBAL:
                        AddGlobalItems(items);
                        break;
                }
            }
        }

        private void ReflashByLocalData()
        {
            ListItemSource.Clear();
            if (LoadDataStarted != null)
            {
                LoadDataStarted();
            }

            List<SimpleListItem> list = XMLListDataReader.ReadListData(new Uri(ContentURL, UriKind.Relative));
            foreach (SimpleListItem item in list)
            {
                ListItemSource.Add(new RichListItem()
                    {
                        Title = item.Title,
                        ItemType = item.ItemType,
                        URL = item.URL,
                        ItemTemplate = item.ItemTemplate,
                        SubItemTemplate = item.SubItemTemplate
                    });
            }
            NotifyPropertyChanged("ListItemSource");

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted();
            }
        }

        public void ReflashByURL()
        {
            if (ContentURL == null || "".Equals(ContentURL))
            {
                HandleNullData(true);
            }
            else
            {
                if (LoadDataStarted != null)
                {
                    LoadDataStarted();
                }

                BaseAPI api = new BaseAPI();
                api.APILoadCompleted += OnAPILoadCompleted;
                api.GetStringResponse(HttpUtility.UrlDecode(ContentURL));
            }
        }
        #endregion

        public void OnAPILoadCompleted(String result)
        {
            ListItemSource.Clear();
            if (result == null || "".Equals(result))
            {
                HandleNullData(true);
            }
            else
            {
                List<RichListItem> listResult = new List<RichListItem>();
                switch (Template)
                {
                    case WeatherItemTemplate.WI_TEMPLATE_AREA:
                        listResult = ForecastParser.Parse(result);
                        AddForecastItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_OBS:
                        listResult = OBSParser.Parse(result);
                        AddOBSItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_WEEK:
                        listResult = WeekParser.Parse(result);
                        AddWeekItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SEA:
                        listResult = SeaParser.Parse(result);
                        AddSeaItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_NEARSEA:
                        listResult = NearSeaParser.Parse(result);
                        AddNearSeaItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_TIDE:
                        listResult = TideParser.Parse(result);
                        AddTideItems(listResult);
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_GLOBAL:
                        listResult = GlobalParser.Parse(result);
                        AddGlobalItems(listResult);
                        break;
                }
                listResult = null;
            }

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted();
            }
        }

        public void AddForecastItems(List<RichListItem> listForecast)
        {
            if (listForecast == null || listForecast.Count <= 0)
            {
                HandleNullData(true);
            }
            else
            {
                for (int i = 0; i < listForecast.Count; ++i)
                {
                    RichListItem item = listForecast[i];
                    RichListItem newItem = new RichListItem();
                    newItem.Title = item.Title;
                    newItem.Description = item.Description;
                    newItem.Area = item.Area;
                    newItem.Temperature = String.Format("{0}{1}", item.Temperature, Constants.SUFFIX_TEMPERATURE);
                    newItem.ChanceOfRain = String.Format("{0}{1}{2}", Constants.PREFIX_CHANCE_OF_RAIN, item.ChanceOfRain, Constants.SUFFIX_CHANCE_OF_RAIN);
                    newItem.StartTime = item.StartTime;
                    newItem.EndTime = item.EndTime;
                    newItem.ItemType = item.ItemType;
                    newItem.ItemTemplate = item.ItemTemplate;
                    String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(item.Description, newItem.Title);
                    newItem.Icon = strWeatherImage;

                    ListItemSource.Add(newItem);
                }
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddOBSItems(List<RichListItem> listOBS)
        {
            for (int i = 0; i < listOBS.Count; ++i)
            {
                RichListItem item = listOBS[i];
                RichListItem newItem = new RichListItem();
                newItem.Description = item.Description;
                newItem.GustWindScale = String.Format("{0}{1}{2}", Constants.PREFIX_GUSTWINDSCALE, item.GustWindScale, Constants.SUFFIX_GUSTWINDSCALE);
                newItem.RainScale = String.Format("{0}{1}{2}", Constants.PREFIX_RAINSCALE, item.RainScale, Constants.SUFFIX_RAINSCALE);
                newItem.Area = item.Area;
                newItem.StartTime = String.Format("{0}{1}", Constants.PREFIX_PUBLISH_TIME, item.StartTime);
                newItem.WindDirection = String.Format("{0}{1}", Constants.PREFIX_WINDDIRECTION, item.WindDirection);
                newItem.WindScale = String.Format("{0}{1}{2}", Constants.PREFIX_WINDSCALE, item.WindScale, Constants.SUFFIX_WINDSCALE);
                newItem.Temperature = String.Format("{0}{1}{2}", Constants.PREFIX_TEMPERATURE, item.Temperature, Constants.SUFFIX_TEMPERATURE);
                String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(item.Description);
                newItem.Icon = strWeatherImage;
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddWeekItems(List<RichListItem> listWeek)
        {
            for (int i = 0; i < listWeek.Count; ++i)
            {
                RichListItem item = listWeek[i];
                RichListItem newItem = new RichListItem();

                newItem.Description = item.Description;
                String strTime = "";
                if (item.Day == null || "".Equals(item.Day))
                {
                    strTime = item.StartTime;
                }
                else
                {
                    strTime = String.Format("{0} - {1}", item.StartTime, item.Day);
                }
                newItem.StartTime = strTime;
                newItem.Temperature = String.Format("{0}{1}", item.Temperature, Constants.SUFFIX_TEMPERATURE);
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddSeaItems(List<RichListItem> listSea)
        {
            for (int i = 0; i < listSea.Count; ++i)
            {
                RichListItem item = listSea[i];
                RichListItem newItem = new RichListItem();

                newItem.StartTime = item.StartTime;
                newItem.Wave = item.Wave;
                newItem.Description = item.Description;
                newItem.Wind = item.Wind;
                newItem.WindScale = item.WindScale;
                String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(item.Description, Constants.DEFAULT_TIMEBLOCK_DESCRIPTION);
                newItem.Icon = strWeatherImage;
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddNearSeaItems(List<RichListItem> listNearSea)
        {
            for (int i = 0; i < listNearSea.Count; ++i)
            {
                RichListItem item = listNearSea[i];
                RichListItem newItem = new RichListItem();

                newItem.Title = Constants.PREFIX_VALIDTIME;
                newItem.Description = item.Description;
                newItem.StartTime = item.StartTime;
                newItem.EndTime = item.EndTime;
                newItem.ValidTime = String.Format("{0} - {1}", item.StartTime, item.EndTime);
                newItem.Wave = item.Wave;
                newItem.WaveLevel = item.WaveLevel;
                newItem.WindScale = item.WindScale;
                newItem.Wind = item.Wind;
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;
                String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(item.Description, Constants.DEFAULT_TIMEBLOCK_DESCRIPTION);
                newItem.Icon = strWeatherImage;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddTideItems(List<RichListItem> listTide)
        {
            for (int i = 0; i < listTide.Count; ++i)
            {
                RichListItem item = listTide[i];
                RichListItem newItem = new RichListItem();

                newItem.Description = item.Description;
                newItem.StartTime = item.StartTime;
                newItem.LunarDate = item.LunarDate;
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }

        public void AddGlobalItems(List<RichListItem> listGlobal)
        {
            for (int i = 0; i < listGlobal.Count; ++i)
            {
                RichListItem item = listGlobal[i];
                RichListItem newItem = new RichListItem();

                newItem.Description = item.Description;
                //newItem.AvgRain = String.Format("{0}{1}{2}", Constants.PREFIX_AVGRAINSCALE, item.AvgRain, Constants.SUFFIX_RAINSCALE);
                //newItem.Temperature = String.Format("{0}{1}{2}", Constants.PREFIX_TEMPERATURE, item.Temperature, Constants.SUFFIX_TEMPERATURE);
                //newItem.AvgTemperature = String.Format("{0}{1}{2}", Constants.PREFIX_AVGTEMPERATURE, item.AvgTemperature, Constants.SUFFIX_TEMPERATURE);
                newItem.AvgRain = item.AvgRain;
                newItem.Temperature = String.Format("{0}{1}", item.Temperature, Constants.SUFFIX_TEMPERATURE);
                newItem.AvgTemperature = String.Format("{0}{1}", item.AvgTemperature, Constants.SUFFIX_TEMPERATURE);
                String strWeatherImage = UtilityHelper.GetImageByWeatherDescription(item.Description, Constants.DEFAULT_TIMEBLOCK_DESCRIPTION);
                newItem.Icon = strWeatherImage;
                newItem.ItemType = item.ItemType;
                newItem.ItemTemplate = item.ItemTemplate;

                ListItemSource.Add(newItem);
            }
            NotifyPropertyChanged("ListItemSource");
        }
    }
}
