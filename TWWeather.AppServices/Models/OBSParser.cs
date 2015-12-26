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
    public class OBSParser
    {
        public OBSParser()
        {
        }

        //{
        //  "result": {
        //    "description": "X",
        //    "gustWindScale": "-",
        //    "rain": "0.0",
        //    "id": "46692",
        //    "locationName": "台北",
        //    "time": "2012-03-08 00:15:00",
        //    "windDirection": "靜風",
        //    "windScale": "0",
        //    "temperature": 16.0
        //  }
        //}

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
                        String description = "", gustWindScale = "",
                            rain = "", id = "", locationName = "", time = "",
                            windDirection = "", windScale = "", temperature = "";

                        JToken result = jsonObj["result"];
                        description = result["description"].ToString();
                        gustWindScale = result["gustWindScale"].ToString();
                        rain = result["rain"].ToString();
                        id = result["id"].ToString();
                        locationName = result["locationName"].ToString();
                        time = result["time"].ToString();
                        windDirection = result["windDirection"].ToString();
                        windScale = result["windScale"].ToString();
                        temperature = result["temperature"].ToString();

                        RichListItem itemOBS = new RichListItem();
                        itemOBS.Description = description;
                        itemOBS.GustWindScale = gustWindScale;
                        itemOBS.RainScale = rain;
                        itemOBS.Area = locationName;
                        itemOBS.StartTime = time;
                        itemOBS.WindDirection = windDirection;
                        itemOBS.WindScale = windScale;
                        itemOBS.Temperature = temperature;
                        itemOBS.ItemType = WeatherItemType.WI_TYPE_NON;
                        itemOBS.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_OBS;

                        list.Add(itemOBS);
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
