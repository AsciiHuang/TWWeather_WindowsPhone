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

namespace TWWeather.AppServices.Models
{
    public class RichListItem : AreaWeatherListItem
    {
        public RichListItem() : base()
        {
            _gustWindScale = _windDirection = _windScale = _rainScale = _wave = _wind = _waveLevel = _validTime = _lunarDate = _avgRain = _avgTemperature = "";
        }

        #region Week
        private String _day; // 陣風
        public String Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                NotifyPropertyChanged("Day");
            }
        }
        #endregion

        #region OBS
        private String _gustWindScale; // 陣風
        public String GustWindScale
        {
            get
            {
                return _gustWindScale;
            }
            set
            {
                _gustWindScale = value;
                NotifyPropertyChanged("GustWindScale");
            }
        }

        private String _windDirection; // 風向
        public String WindDirection
        {
            get
            {
                return _windDirection;
            }
            set
            {
                _windDirection = value;
                NotifyPropertyChanged("WindDirection");
            }
        }

        private String _windScale; // 風力
        public String WindScale
        {
            get
            {
                return _windScale;
            }
            set
            {
                _windScale = value;
                NotifyPropertyChanged("WindScale");
            }
        }

        private String _rainScale; // 雨量
        public String RainScale
        {
            get
            {
                return _rainScale;
            }
            set
            {
                _rainScale = value;
                NotifyPropertyChanged("RainScale");
            }
        }
        #endregion

        #region Sea、NearSea
        private String _wave; // 浪
        public String Wave
        {
            get
            {
                return _wave;
            }
            set
            {
                _wave = value;
                NotifyPropertyChanged("Wave");
            }
        }

        private String _wind; // 風
        public String Wind
        {
            get
            {
                return _wind;
            }
            set
            {
                _wind = value;
                NotifyPropertyChanged("Wind");
            }
        }

        private String _waveLevel; // 浪高
        public String WaveLevel
        {
            get
            {
                return _waveLevel;
            }
            set
            {
                _waveLevel = value;
                NotifyPropertyChanged("WaveLevel");
            }
        }

        private String _validTime; // 有效時間
        public String ValidTime
        {
            get
            {
                return _validTime;
            }
            set
            {
                _validTime = value;
                NotifyPropertyChanged("ValidTime");
            }
        }
        #endregion

        #region Tide
        private String _lunarDate; // 農曆日期
        public String LunarDate
        {
            get
            {
                return _lunarDate;
            }
            set
            {
                _lunarDate = value;
                NotifyPropertyChanged("LunarDate");
            }
        }

        // 賴得再包一層 List 了…直接在 Parser 裡面串成 Description 好了 :P
        //private String _tideTime; // 潮汐時間
        //public String TideTime
        //{
        //    get
        //    {
        //        return _tideTime;
        //    }
        //    set
        //    {
        //        _tideTime = value;
        //        NotifyPropertyChanged("TideTime");
        //    }
        //}

        //private String _tideName; // 潮汐狀態
        //public String TideName
        //{
        //    get
        //    {
        //        return _tideName;
        //    }
        //    set
        //    {
        //        _tideName = value;
        //        NotifyPropertyChanged("TideName");
        //    }
        //}

        //private String _tideHeight; // 高潮………哦…是潮高…
        //public String TideHeight
        //{
        //    get
        //    {
        //        return _tideHeight;
        //    }
        //    set
        //    {
        //        _tideHeight = value;
        //        NotifyPropertyChanged("TideHeight");
        //    }
        //}
        #endregion

        #region Global
        private String _avgRain;
        public String AvgRain
        {
            get
            {
                return _avgRain;
            }
            set
            {
                _avgRain = value;
                NotifyPropertyChanged("AvgRain");
            }
        }

        private String _avgTemperature;
        public String AvgTemperature
        {
            get
            {
                return _avgTemperature;
            }
            set
            {
                _avgTemperature = value;
                NotifyPropertyChanged("AvgTemperature");
            }
        }
        #endregion
    }
}
