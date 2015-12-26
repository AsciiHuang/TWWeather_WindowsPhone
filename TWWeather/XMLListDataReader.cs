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
using System.Windows.Resources;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using TWWeather.AppServices;
using TWWeather.AppServices.Models;

namespace TWWeather
{
    public class XMLListDataReader
    {
        public static List<SimpleListItem> ReadListData(Uri uri)
        {
            List<SimpleListItem> resList = new List<SimpleListItem>();

            StreamResourceInfo resource = Application.GetResourceStream(uri);
            Byte[] btRes = new Byte[resource.Stream.Length];
            resource.Stream.Read(btRes, 0, (int)resource.Stream.Length);
            String xmlData = Encoding.UTF8.GetString(btRes, 0, btRes.Length);

            try
            {
                XElement rootElement = XElement.Parse(xmlData);
                IEnumerable<XElement> allItems = from elem in rootElement.Elements("item") select elem;
                foreach (XElement item in allItems)
                {
                    SimpleListItem w = new SimpleListItem();
                    w.ItemType = (WeatherItemType)int.Parse((String)item.Element("type"));
                    w.Title = (String)item.Element("name");
                    //w.URL = HttpUtility.UrlDecode((String)item.Element("url"));
                    // 等到要用的人再自己做 URL Decode
                    w.URL = (String)item.Element("url");
                    w.ItemTemplate = (WeatherItemTemplate)int.Parse((String)item.Element("template"));
                    w.SubItemTemplate = (WeatherItemTemplate)int.Parse((String)item.Element("subtemplate"));
                    resList.Add(w);
                }
            }
            catch (Exception)
            {
                resList.Clear();
            }

            return resList;
        }
    }
}
