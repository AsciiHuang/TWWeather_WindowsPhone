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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TWWeather.AppServices.Models
{
    public class GlobalParser
    {
        public GlobalParser()
        {
        }

        public static List<RichListItem> Parse(String jsonString)
        {
            List<RichListItem> list = new List<RichListItem>();

            if (jsonString != null && !"".Equals(jsonString))
            {
                try
                {
                    JObject jsonObj = JObject.Parse(jsonString);
                    if (jsonObj != null && jsonObj.HasValues)
                    {
                        RichListItem newItem = new RichListItem();

                        String forecast = "", avgRain = "", temperature = "", avgTemperature = "";
                        JToken result = jsonObj["result"];
                        newItem.AvgRain = avgRain = result["avgRain"].ToString();
                        newItem.Description = forecast = result["forecast"].ToString();
                        newItem.Temperature = temperature = result["temperature"].ToString();
                        newItem.AvgTemperature = avgTemperature = result["avgTemperature"].ToString();
                        newItem.ItemType = WeatherItemType.WI_TYPE_NON;
                        newItem.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_GLOBAL;

                        list.Add(newItem);
                    }
                }
                catch (Exception)
                {
                    list = new List<RichListItem>();
                }
            }

            return list;
        }
    }
}
