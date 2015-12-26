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
    public class SeaParser
    {
        public SeaParser()
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
                        JToken items = result["items"];
                        if (items != null && items.HasValues)
                        {
                            JToken item = items.First;
                            while (item != null && item.HasValues)
                            {
                                RichListItem newItem = new RichListItem();
                                newItem.StartTime = item["date"].ToString(); // 2012-03-10
                                newItem.Wave = item["wave"].ToString(); // 小浪轉大浪
                                newItem.Description = item["description"].ToString(); // 陰時多雲局部雨
                                newItem.Wind = item["wind"].ToString(); // 偏北風
                                newItem.WindScale = item["windScale"].ToString(); // 4至5陣風7級轉5至6陣風8級
                                newItem.ItemType = WeatherItemType.WI_TYPE_NON;
                                newItem.ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_SEA;
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
