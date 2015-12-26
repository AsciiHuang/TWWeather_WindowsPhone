using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TWWeather.AppServices.Models
{
    public enum WeatherItemType
    {
        WI_TYPE_NON, // 無動作 - 0
        WI_TYPE_JSON, // JSON - 1
        WI_TYPE_XML, // XML  - 2
        WI_TYPE_BLANKWEB, // 開啟 Browser - 3
        WI_TYPE_CALL,  // 播電話 - 4
        WI_TYPE_USE_APPSERVICE_DATA, // 把 AppService.Instance.mCacheList 拿來用 - 5
        WI_TYPE_MAILTO, // 寄信 - 6
        WI_TYPE_ABOUT, // 關於我 - 7
        WI_TYPE_SETTING, // 設定 - 8
        WI_TYPE_OTHER, // 其他 - 9
    }

    public enum WeatherItemTemplate
    {
        WI_TEMPLATE_ATTENTION, // 一般的天氣完整面板加上地點名稱 - 0 ForecastParser
        WI_TEMPLATE_AREA, // 預報面板 - 1 ForecastParser
        WI_TEMPLATE_WEEK, // 週、旅遊面板 - 2 WeekParser
        WI_TEMPLATE_SEA, // 漁業 - 3 SeaParser
        WI_TEMPLATE_NEARSEA, // 近海 - 4 NearSeaParser
        WI_TEMPLATE_TIDE, // 潮汐 - 5 TideParser
        WI_TEMPLATE_OBS, // 觀測站(目前天氣) - 6 ObsParser
        WI_TEMPLATE_SMALL_ICON, // 有圖示的列 - 7
        WI_TEMPLATE_LARGE_ICON, // 有圖示的列 - 8
        WI_TEMPLATE_SUMMARY, // 標題與描述 - 9
        WI_TEMPLATE_SIMPLE, // 單列 - 10
        WI_TEMPLATE_GLOBAL, // 全球 - 11 // GlobalParser
        WI_TEMPLATE_SPACE, // 空白列 - 12
        WI_TEMPLATE_SEPARATOR, // 分格線 - 13
    }

    public class SimpleListItem : INotifyPropertyChanged
    {
        public SimpleListItem()
        {
            _title = _description = _url = "";
            ItemType = WeatherItemType.WI_TYPE_NON;
            ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SPACE;
            SubItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SPACE;
        }

        private String _title;
        public String Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private String _description;
        public String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private String _url;
        public String URL
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    NotifyPropertyChanged("URL");
                }
            }
        }

        public WeatherItemType ItemType;
        public WeatherItemTemplate ItemTemplate;
        public WeatherItemTemplate SubItemTemplate;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}