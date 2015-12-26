using System.Windows;
using Microsoft.Phone.Scheduler;
using TWWeather.AppServices.Models;
using System;
using System.Collections.Generic;
using TWWeather.TileServices;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.Phone.Shell;

namespace ScheduledAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent 建構函式，會初始化 UnhandledException 處理常式
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // 訂閱 Managed 例外狀況處理常式
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// 發生未處理的例外狀況時要執行的程式碼
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 發生未處理的例外狀況; 切換到偵錯工具
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// 執行排程工作的代理程式
        /// </summary>
        /// <param name="task">
        /// 叫用的工作
        /// </param>
        /// <remarks>
        /// 這個方法的呼叫時機為叫用週期性或耗用大量資料的工作時
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            Boolean bFindTile = TileService.Instance.FindAreaLiveTile();
            if (bFindTile)
            {
                BaseAPI apiForecast = new BaseAPI();
                apiForecast.APILoadCompleted += OnForecastAPILoadCompleted;
                apiForecast.GetStringResponse(Constants.WEATHER_FORECAST_URL);
                Debug.WriteLine(">>>>> ScheduledAgent OnInvoke");
            }
        }

        public void OnForecastAPILoadCompleted(String result)
        {
            Debug.WriteLine(">>>>> ScheduledAgent OnInvoke :: OnForecastAPILoadCompleted :: " + result.Length);
            if (String.IsNullOrEmpty(result))
            {
                // 取資料錯誤，不更新了
                NotifyComplete();
            }
            else
            {
                TileService.Instance.mAreaWeatherList.Clear();
                Dictionary<String, List<RichListItem>> allForecast = ForecastParser.ParseAllForecast(result);
                if (allForecast != null && allForecast.Count > 0)
                {
                    TileService.Instance.mAreaWeatherList = allForecast;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            TileService.Instance.RefreshAllTile();
                        }
                    );
                }
                NotifyComplete();
                //ScheduledActionService.LaunchForTest(Constants.TWWEATHER_BACKGROUND_SCHEDULER, TimeSpan.FromMilliseconds(15000));
            }
        }
    }
}