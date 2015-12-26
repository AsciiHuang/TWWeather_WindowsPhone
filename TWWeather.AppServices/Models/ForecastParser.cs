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
using System.Diagnostics;

namespace TWWeather.AppServices.Models
{
    public class ForecastParser
    {
        public ForecastParser()
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
                        String areaName = "", description = "", title = "", 
                            rain = "", beginTime = "", endTime = "", temperature = "";
                        JToken result = jsonObj["result"];
                        areaName = result["locationName"].ToString();
                        JToken items = result["items"];
                        if (items != null && items.HasValues)
                        {
                            JToken item = items.First;
                            while (item != null && item.HasValues)
                            {
                                description = item["description"].ToString();
                                title = item["title"].ToString();
                                rain = item["rain"].ToString();
                                beginTime = item["beginTime"].ToString();
                                endTime = item["endTime"].ToString();
                                temperature = item["temperature"].ToString();

                                list.Add(new RichListItem()
                                {
                                    Title = title,
                                    Description = description,
                                    Area = areaName,
                                    ChanceOfRain = rain,
                                    StartTime = beginTime,
                                    EndTime = endTime,
                                    Temperature = temperature, 
                                    ItemType = WeatherItemType.WI_TYPE_NON, 
                                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_AREA
                                });

                                item = item.Next;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    list = new List<RichListItem>();
                }
            }

            return list;
        }

        public static Dictionary<String, List<RichListItem>> ParseAllForecast(String jsonString)
        {
            Dictionary<String, List<RichListItem>> allRes = new Dictionary<string, List<RichListItem>>();

            if (jsonString != null && !"".Equals(jsonString))
            {
                try
                {
                    JObject jsonObj = JObject.Parse(jsonString);
                    if (jsonObj != null && jsonObj.HasValues)
                    {
                        String areaName = "", description = "", title = "",
                            rain = "", beginTime = "", endTime = "", temperature = "";
                        JToken result = jsonObj["result"];
                        JToken area = result.First;
                        while (area != null && area.HasValues)
                        {
                            areaName = area["locationName"].ToString();
                            JToken items = area["items"];
                            if (items != null && items.HasValues)
                            {
                                List<RichListItem> list = new List<RichListItem>();
                                JToken item = items.First;
                                while (item != null && item.HasValues)
                                {
                                    description = item["description"].ToString();
                                    title = item["title"].ToString();
                                    rain = item["rain"].ToString();
                                    beginTime = item["beginTime"].ToString();
                                    endTime = item["endTime"].ToString();
                                    temperature = item["temperature"].ToString();

                                    list.Add(new RichListItem()
                                    {
                                        Title = title,
                                        Description = description,
                                        Area = areaName,
                                        ChanceOfRain = rain,
                                        StartTime = beginTime,
                                        EndTime = endTime,
                                        Temperature = temperature,
                                        ItemType = WeatherItemType.WI_TYPE_NON,
                                        ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_AREA
                                    });

                                    item = item.Next;
                                }
                                allRes.Add(areaName, list);
                            }
                            area = area.Next;
                        }
                    }
                }
                catch(Exception)
                {
                    allRes = new Dictionary<string, List<RichListItem>>();
                }
            }

            return allRes;
        }
    }
}
