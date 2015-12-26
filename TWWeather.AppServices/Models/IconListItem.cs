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
using System.Windows.Media.Imaging;

namespace TWWeather.AppServices.Models
{
    public class IconListItem: SimpleListItem
    {
        public IconListItem() : base()
        {
            _icon = "";
        }

        private String _icon;
        public String Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (value != _icon)
                {
                    _icon = value;
                    NotifyPropertyChanged("Icon");
                }
            }
        }
    }
}
