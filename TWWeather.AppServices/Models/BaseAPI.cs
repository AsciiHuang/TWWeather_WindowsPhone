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

namespace TWWeather.AppServices.Models
{
    public class BaseAPI
    {
        public delegate void APILoadHandler(String result);
        public event APILoadHandler APILoadCompleted = null;

        public BaseAPI()
        {
        }

        public void GetStringResponse(String apiURL)
        {
            // WP 的 WebClient、HttpWebRequest 是強制吃 Cache 的…所以要自行串上亂數
            if (apiURL.IndexOf('?') < 0)
            {
                apiURL += String.Format("?rdn={0}", DateTime.Now.Ticks);
            }
            else
            {
                apiURL += String.Format("&rdn={0}", DateTime.Now.Ticks);
            }

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadStringCompleted;
            try
            {
                webClient.DownloadStringAsync(new Uri(apiURL, UriKind.Absolute));
            }
            catch (Exception)
            {
                if (APILoadCompleted != null)
                {
                    APILoadCompleted("");
                }
            }
        }

        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                String strResult = "";
                try
                {
                    strResult = e.Result;
                }
                catch (Exception)
                {
                    strResult = "";
                }

                if (APILoadCompleted != null)
                {
                    APILoadCompleted(strResult);
                }
            }
            else
            {
                if (APILoadCompleted != null)
                {
                    APILoadCompleted("");
                }
            }
        }
    }
}
