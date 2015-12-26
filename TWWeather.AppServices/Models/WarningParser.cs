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
using Newtonsoft.Json.Linq;

namespace TWWeather.AppServices.Models
{
    public class WarningParser
    {
        public WarningParser()
        {
        }

        public static String Parse(String jsonString)
        {
            String strRes = "";

            //{
            //  "result": [
            //    {
            //      "text": "發震時間：101年3月24日23時3分1秒\n震央位置：北緯24.39°東經121.50°\n震源深度：7.1公里\n芮氏規模：4.1\n相對位置：花蓮縣政府北偏西方46.2公里(位於宜蘭縣南澳鄉)\n各地震度級宜蘭縣地區最大震度4級南山4南澳3牛鬥2羅東1蘇澳1花蓮縣地區最大震度2級太魯閣2吉安1銅門1花蓮市1壽豐1和平1西林1南投縣地區最大震度2級合歡山2台中市地區最大震度2級德基2桃園縣地區最大震度1級三光1\n",
            //      "id": "semi",
            //      "name": "第051號地震報告"
            //    },
            //    {
            //      "text": ".white:link{text-decoration:none;?color:#ffffcc}.white:visited{text-decoration:none;color:#ffffcc}.white:hover{text-decoration:none;?color:#ffffcc}-->\n\n\n",
            //      "id": "PW28.wml\">低溫特報</a><br /><a href=\"warning/PW25",
            //      "name": "陸上強風特報"
            //    }
            //  ]
            //}

            if (jsonString != null && !"".Equals(jsonString))
            {
                try
                {
                    JObject jsonObj = JObject.Parse(jsonString);
                    if (jsonObj != null && jsonObj.HasValues)
                    {
                        JToken result = jsonObj["result"];
                        if (result != null && result.HasValues)
                        {
                            String strText = "";//, strName = "";
                            JToken item = result.First;
                            // 看起來除了地震特報之外似乎有 bug，先不 parse
                            while (item != null && item.HasValues)
                            {
                                //strName = item["name"].ToString();
                                strText = item["text"].ToString();
                                strText = strText.Replace("\n", "");
                                if (!(String.IsNullOrEmpty(strText)))
                                {
                                    // 若不為空，串上去
                                    strRes = String.Format("{0}\n\n{1}", strRes, strText);
                                }
                                item = item.Next;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    strRes = "";
                }
            }
            strRes = strRes.Trim();
            return strRes;
        }
    }
}
