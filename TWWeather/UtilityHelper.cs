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
using TWWeather.AppServices.Models;
using System.Collections.Generic;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Scheduler;

namespace TWWeather
{
    public enum TIMEBLOCK
    {
        TB_TODAY, // 今晨
        TB_TONIGHT, // 今夜
        TB_MORROW, // 明晨
        TB_NON, // 不指定
    }

    public class UtilityHelper
    {
        private static DateTime ToDateTime(String time)
        {
            int year = int.Parse(time.Substring(0, 4));
            int month = int.Parse(time.Substring(5, 2));
            int day = int.Parse(time.Substring(8, 2));
            int hour = int.Parse(time.Substring(11, 2));
            int min = int.Parse(time.Substring(14, 2));
            return new DateTime(year, month, day, hour, min, 0);
        }

        public static TIMEBLOCK NowTargetTimeBlock(List<RichListItem> list)
        {
            TIMEBLOCK res = TIMEBLOCK.TB_TODAY;

            for (int i = 0; i < list.Count; ++i)
            {
                // 愈後面的日期時間會愈大…所以從頭開始比結束時間…
                DateTime endDate = ToDateTime(list[i].EndTime);
                DateTime Now = DateTime.Now;
                if (Now < endDate)
                {
                    res = (TIMEBLOCK)i;
                    break;
                }
            }

            return res;
        }

        public static String GetImageByWeatherDescription(String description)
        {
            if (description == null || "".Equals(description))
            {
                return "Image/NON.png";
            }
            else
            {
                String strRes = "";

                int nHour = DateTime.Now.Hour;
                if (nHour >= 0 && nHour < 5)
                {
                    // 清晨 0 - 3 點
                    strRes = "Image/NIGHT_";
                }
                else if (nHour >= 5 && nHour < 19)
                {
                    // 清晨 4 點到 18 點
                    strRes = "Image/DAY_";
                }
                else
                {
                    // 晚上
                    strRes = "Image/NIGHT_";
                }

                return GetImageByWeatherDescriptionKeyWord(description, strRes);
            }
        }

        public static String GetImageByWeatherDescription(String description, String title)
        {
            if (description == null || "".Equals(description))
            {
                return "Image/NON.png";
            }
            else
            {
                String strRes = "";

                if (title.IndexOf("白") < 0)
                {
                    // 找不到白，代表不是白天 :P
                    strRes = "Image/NIGHT_";
                }
                else
                {
                    strRes = "Image/DAY_";
                }

                return GetImageByWeatherDescriptionKeyWord(description, strRes);
            }
        }

        private static String GetImageByWeatherDescriptionKeyWord(String description, String preFix)
        {
            String strRes = preFix;

            #region Find KeyWord
            String strSub = "";
            if (description.IndexOf("晴") >= 0)
            {
                strSub += "S";
            }

            if (description.IndexOf("雲") >= 0)
            {
                strSub += "C";
            }
            else if (description.IndexOf("陰") >= 0)
            {
                // 雲和陰都是 C 所以在確沒有雲的情況下才檢查陰
                strSub += "C";
            }

            if (description.IndexOf("雨") >= 0)
            {
                strSub += "R";
            }

            if (description.IndexOf("雷") >= 0)
            {
                strSub += "T";
            }
            #endregion

            if ("".Equals(strSub))
            {
                // 找不到對應的天氣
                strRes = "Image/NON.png";
            }
            else
            {
                strRes += String.Format("{0}.png", strSub);
            }

            return strRes;
        }

        public static int GetAvgTemperature(String temp)
        {
            int res = 20;

            String[] aTemp = temp.Split('~');
            if (aTemp.Length == 2)
            {
                try
                {
                    Double low = Double.Parse(aTemp[0].Trim());
                    Double height = Double.Parse(aTemp[1].Trim());
                    res = (int)(low + height) / 2;
                }
                catch (Exception)
                {
                }
            }

            return res;
        }
        
        public static void ShowWebBrowser(String url)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(url, UriKind.Absolute);
            webBrowserTask.Show();
        }

        public static String GetShareToTileAlbumCoverPath(String fileName)
        {
            return "Shared/ShellContent/" + fileName;
        }

        public static String[] GetTemperatureUnit(String temperature)
        {
            String[] unitTemp = new String[2];
            unitTemp.Initialize();

            String[] aTemp = temperature.Split('~');
            if (aTemp.Length == 2)
            {
                try
                {
                    unitTemp[0] = String.Format("{0}{1}", aTemp[0].Trim(), Constants.SUFFIX_SIMPLE_TEMPERATURE);
                    unitTemp[1] = String.Format("{0}{1}", aTemp[1].Trim(), Constants.SUFFIX_SIMPLE_TEMPERATURE);
                }
                catch (Exception)
                {
                }
            }

            return unitTemp;
        }

        #region Get Navigate URL
        public static Uri GetEditAttentionItemPageUri()
        {
            return new Uri("/EditAttentionItemPage.xaml", UriKind.Relative);
        }

        public static Uri GetExplorePageURL(String title, String url, WeatherItemType type, WeatherItemTemplate template)
        {
            String strPath = String.Format("/ExplorePage.xaml?title={0}&url={1}&type={2}&template={3}", title, url, (int)type, (int)template);
            return new Uri(strPath, UriKind.Relative);
        }
        #endregion

        public static Boolean CheckScheduledAgent()
        {
            Boolean IsBackgroundAgentValid = false;
            PeriodicTask taskUpdateTile = null;
            taskUpdateTile = (PeriodicTask)ScheduledActionService.Find(Constants.TWWEATHER_BACKGROUND_SCHEDULER);
            if (taskUpdateTile == null)
            {
                try
                {
                    taskUpdateTile = new PeriodicTask(Constants.TWWEATHER_BACKGROUND_SCHEDULER);
                    taskUpdateTile.Description = "TW Weather Periodic Task for Live Tile.\n注意：關閉此背景作業將會使動態磚失效";
                    ScheduledActionService.Add(taskUpdateTile);
                    IsBackgroundAgentValid = true;
                }
                catch (InvalidOperationException e)
                {
                    // 啟動不了就算了
                }
                catch (Exception e)
                {
                    // 啟動不了就算了
                }
            }
            else
            {
                // 就算存在也要看看有沒有打開
                if (taskUpdateTile.IsEnabled && taskUpdateTile.IsScheduled)
                {
                    IsBackgroundAgentValid = true;
                }
                else
                {
                    // 被關掉了，重新啟動
                    ScheduledActionService.Remove(Constants.TWWEATHER_BACKGROUND_SCHEDULER);
                    try
                    {
                        taskUpdateTile = new PeriodicTask(Constants.TWWEATHER_BACKGROUND_SCHEDULER);
                        taskUpdateTile.Description = "TW Weather Periodic Task for Live Tile.\n注意：關閉此背景作業將會使動態磚失效";
                        ScheduledActionService.Add(taskUpdateTile);
                        IsBackgroundAgentValid = true;
                    }
                    catch (InvalidOperationException e)
                    {
                        // 啟動不了就算了
                    }
                    catch (Exception e)
                    {
                        // 啟動不了就算了
                    }
                }
            }

            //if (IsBackgroundAgentValid)
            //{
            //    ScheduledActionService.LaunchForTest(Constants.TWWEATHER_BACKGROUND_SCHEDULER, TimeSpan.FromMilliseconds(15000));
            //}
            return IsBackgroundAgentValid;
        }
    }
}
