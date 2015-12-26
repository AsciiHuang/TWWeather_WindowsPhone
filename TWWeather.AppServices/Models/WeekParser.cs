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
    public class WeekParser
    {
        public WeekParser()
        {
        }

        #region old
        //{
        //  "result": {
        //    "locationName": "台北市",
        //    "items": [
        //      {
        //        "date": "2012-12-06",
        //        "description": "陰短暫雨",
        //        "temperature": "18 ~ 23"
        //      },
        //      {
        //        "date": "2012-12-07",
        //        "description": "陰時多雲短暫雨",
        //        "temperature": "20 ~ 25"
        //      },
        //      {
        //        "date": "2012-12-08",
        //        "description": "陰有雨",
        //        "temperature": "15 ~ 21"
        //      },
        //      {
        //        "date": "2012-12-09",
        //        "description": "陰有雨",
        //        "temperature": "14 ~ 17"
        //      },
        //      {
        //        "date": "2012-12-10",
        //        "description": "陰時多雲短暫雨",
        //        "temperature": "13 ~ 17"
        //      },
        //      {
        //        "date": "2012-12-11",
        //        "description": "多雲時陰短暫雨",
        //        "temperature": "13 ~ 17"
        //      },
        //      {
        //        "date": "2012-12-12",
        //        "description": "多雲時陰短暫雨",
        //        "temperature": "15 ~ 19"
        //      }
        //    ],
        //    "id": "Taipei",
        //    "publishTime": "2012-12-05 16:30:00"
        //  }
        //}

        //public static List<RichListItem> Parse(String jsonString)
        //{
        //    List<RichListItem> list = new List<RichListItem>();

        //    if (jsonString != null && !"".Equals(jsonString))
        //    {
        //        try
        //        {
        //            JObject jsonObj = JObject.Parse(jsonString);
        //            if (jsonObj != null && jsonObj.HasValues)
        //            {
        //                String areaName = "", description = "", date = "", temperature = "";

        //                JToken result = jsonObj["result"];
        //                areaName = result["locationName"].ToString();
        //                JToken items = result["items"];
        //                if (items != null && items.HasValues)
        //                {
        //                    JToken item = items.First;
        //                    while (item != null && item.HasValues)
        //                    {
        //                        date = item["date"].ToString();
        //                        description = item["description"].ToString();
        //                        temperature = item["temperature"].ToString();

        //                        list.Add(new RichListItem()
        //                        {
        //                            Title = areaName,
        //                            Description = description,
        //                            Area = areaName,
        //                            StartTime = date,
        //                            EndTime = date,
        //                            Temperature = temperature,
        //                            ItemType = WeatherItemType.WI_TYPE_NON,
        //                            ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_WEEK
        //                        });

        //                        item = item.Next;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            list = new List<RichListItem>();
        //        }
        //    }

        //    return list;
        //}
        #endregion

        #region new
//{
//  "result": {
//    "locationName": "台北市",
//    "items": [
//      {
//        "date": "2012-03-22",
//        "description": "多雲時晴",
//        "temperature": "23 ~ 27",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-22",
//        "description": "多雲時晴",
//        "temperature": "18 ~ 23",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-23",
//        "description": "多雲時陰短暫陣雨",
//        "temperature": "18 ~ 27",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-23",
//        "description": "陰時多雲短暫陣雨",
//        "temperature": "13 ~ 21",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-24",
//        "description": "陰短暫陣雨",
//        "temperature": "13 ~ 16",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-24",
//        "description": "陰時多雲",
//        "temperature": "13 ~ 15",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-25",
//        "description": "多雲時陰",
//        "temperature": "13 ~ 18",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-25",
//        "description": "多雲",
//        "temperature": "15 ~ 17",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-26",
//        "description": "多雲時晴",
//        "temperature": "15 ~ 21",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-26",
//        "description": "晴時多雲",
//        "temperature": "16 ~ 19",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-27",
//        "description": "多雲時晴",
//        "temperature": "16 ~ 23",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-27",
//        "description": "多雲",
//        "temperature": "17 ~ 20",
//        "day": "晚上"
//      },
//      {
//        "date": "2012-03-28",
//        "description": "多雲",
//        "temperature": "17 ~ 25",
//        "day": "白天"
//      },
//      {
//        "date": "2012-03-28",
//        "description": "晴時多雲",
//        "temperature": "18 ~ 22",
//        "day": "晚上"
//      }
//    ],
//    "id": "1",
//    "publishTime": "2012-03-22 11:00:00"
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
                        String areaName = "", description = "", date = "", temperature = "", day = "";

                        JToken result = jsonObj["result"];
                        areaName = result["locationName"].ToString();
                        JToken items = result["items"];
                        if (items != null && items.HasValues)
                        {
                            JToken item = items.First;
                            int nYear = 0, nMonth = 0, nDay = 0;
                            while (item != null && item.HasValues)
                            {
                                date = item["date"].ToString();
                                if (!String.IsNullOrEmpty(date))
                                {
                                    String[] aStrList = date.Split('-');
                                    if (aStrList.Length == 3)
                                    {
                                        try
                                        {
                                            nYear = int.Parse(aStrList[0]);
                                            nMonth = int.Parse(aStrList[1]);
                                            nDay = int.Parse(aStrList[2]);
                                            DateTime dt = new DateTime(nYear, nMonth, nDay);
                                            DayOfWeek dw = dt.DayOfWeek;
                                            date = String.Format("{0} ({1})", date, dw.ToString());
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                                description = item["description"].ToString();
                                temperature = item["temperature"].ToString();
                                JToken jDay = item["day"];
                                if (jDay != null)
                                {
                                    day = jDay.ToString();
                                }
                                else
                                {
                                    day = "";
                                }

                                list.Add(new RichListItem()
                                {
                                    Title = areaName, 
                                    Description = description, 
                                    Area = areaName, 
                                    StartTime = date, 
                                    EndTime = date, 
                                    Temperature = temperature, 
                                    Day = day, 
                                    ItemType = WeatherItemType.WI_TYPE_NON, 
                                    ItemTemplate = WeatherItemTemplate.WI_TEMPLATE_WEEK
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
        #endregion
    }
}
