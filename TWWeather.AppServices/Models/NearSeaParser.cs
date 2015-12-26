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
    public class NearSeaParser
    {
        public NearSeaParser()
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
                        JToken result = jsonObj["result"];
                        RichListItem newItem = new RichListItem();

                        newItem.Description = result["description"].ToString();
                        newItem.StartTime = result["validBeginTime"].ToString();
                        newItem.EndTime = result["validEndTime"].ToString();
                        newItem.Wave = result["wave"].ToString();
                        newItem.WaveLevel = result["waveLevel"].ToString();
                        newItem.WindScale = result["windScale"].ToString();
                        newItem.Wind = result["wind"].ToString();
                        newItem.ItemType = WeatherItemType.WI_TYPE_NON;
                        newItem.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_NEARSEA;

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
