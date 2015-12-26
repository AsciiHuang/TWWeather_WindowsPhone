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
    public class AreaWeatherListItem : IconListItem
    {
        public AreaWeatherListItem() : base()
        {
            _area = _sTime = _eTime = _temperature = _chanceOfRain = _recommendation = "";
        }

        private String _area;
        public String Area
        {
            get
            {
                return _area;
            }
            set
            {
                if (value != _area)
                {
                    _area = value;
                    NotifyPropertyChanged("Area");
                }
            }
        }

        private String _sTime;
        public String StartTime
        {
            get
            {
                return _sTime;
            }
            set
            {
                if (value != _sTime)
                {
                    _sTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        private String _eTime;
        public String EndTime
        {
            get
            {
                return _eTime;
            }
            set
            {
                if (value != _eTime)
                {
                    _eTime = value;
                    NotifyPropertyChanged("EndTime");
                }
            }
        }

        private String _temperature;
        public String Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    NotifyPropertyChanged("Temperature");
                }
            }
        }

        private String _chanceOfRain;
        public String ChanceOfRain
        {
            get
            {
                return _chanceOfRain;
            }
            set
            {
                if (value != _chanceOfRain)
                {
                    _chanceOfRain = value;
                    NotifyPropertyChanged("ChanceOfRain");
                }
            }
        }

        private String _recommendation;
        public String Recommendation
        {
            get
            {
                return _recommendation;
            }
            set
            {
                _recommendation = value;
                NotifyPropertyChanged("Recommendation");
            }
        }
    }
}
