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
    public class TideParser
    {
        public TideParser()
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
                        String description = "";
                        JToken result = jsonObj["result"];
                        JToken items = result["items"];
                        if (items != null && items.HasValues)
                        {
                            JToken item = items.First;
                            while (item != null && item.HasValues)
                            {
                                // 正常來說這裡要跑三次，分別是三個 date
                                RichListItem newItem = new RichListItem();

                                newItem.StartTime = item["date"].ToString();
                                newItem.LunarDate = item["lunarDate"].ToString();

                                #region Tide Detail
                                JToken tides = item["tides"];
                                if (tides != null && tides.HasValues)
                                {
                                    JToken tide = tides.First;
                                    String shortTime = "", name = "", height = "";
                                    while (tide != null && tide.HasValues)
                                    {
                                        // 這裡是列出一天當中滿潮、乾潮的時間，數量不一定
                                        shortTime = tide["shortTime"].ToString();
                                        name = tide["name"].ToString();
                                        height = tide["height"].ToString();
                                        description += String.Format("{0}\t\t\t{1}\t\t\t{2}cm\n", name, shortTime, height.PadLeft(6));
                                        tide = tide.Next;
                                    }
                                    //description = description.TrimEnd('\n');
                                }
                                #endregion

                                newItem.Description = description;
                                newItem.ItemType = WeatherItemType.WI_TYPE_NON;
                                newItem.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_TIDE;

                                list.Add(newItem);
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
    }
}
