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

namespace TWWeather
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class WeatherListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SpaceItem
        {
            get;
            set;
        }

        public DataTemplate SeparatorItem
        {
            get;
            set;
        }

        public DataTemplate SimpleItem
        {
            get;
            set;
        }

        public DataTemplate SmallIconItem
        {
            get;
            set;
        }

        public DataTemplate LargeIconItem
        {
            get;
            set;
        }

        public DataTemplate AreaItem
        {
            get;
            set;
        }

        public DataTemplate OBSItem
        {
            get;
            set;
        }

        public DataTemplate WeekItem
        {
            get;
            set;
        }

        public DataTemplate SeaItem
        {
            get;
            set;
        }

        public DataTemplate NearSeaItem
        {
            get;
            set;
        }

        public DataTemplate TideItem
        {
            get;
            set;
        }

        public DataTemplate GlobalItem
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = null;
            if (item != null)
            {
                SimpleListItem richItem = (SimpleListItem)item;

                switch (richItem.ItemTemplate)
                {
                    case WeatherItemTemplate.WI_TEMPLATE_SPACE:
                        template = SpaceItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SEPARATOR:
                        template = SeparatorItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SIMPLE:
                        template = SimpleItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SMALL_ICON:
                        template = SmallIconItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_LARGE_ICON:
                        template = LargeIconItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_AREA:
                        template = AreaItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_OBS:
                        template = OBSItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_WEEK:
                        template = WeekItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_SEA:
                        template = SeaItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_NEARSEA:
                        template = NearSeaItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_TIDE:
                        template = TideItem;
                        break;
                    case WeatherItemTemplate.WI_TEMPLATE_GLOBAL:
                        template = GlobalItem;
                        break;
                }

                return template;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
