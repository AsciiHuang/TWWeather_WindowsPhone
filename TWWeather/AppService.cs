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
using System.Threading;
using System.Collections.Generic;
using TWWeather.AppServices.Models;
using System.Diagnostics;
using System.Windows.Resources;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace TWWeather
{
    public class AppService
    {
        public List<String> mAreaList = new List<String>();
        public Dictionary<String, List<RichListItem>> mAreaWeatherList = null;
        public List<RichListItem> mCacheList = new List<RichListItem>();

        protected List<List<String>> mClothes = null;
        protected List<List<String>> mAdditionals = null;
        protected List<String> mPants = null;
        protected List<String> mThings = null;

        public Mutex mSettingMutex = null;
        public Mutex mFileMutex = null;
        public AppSettingManager Settings = null; // Settings 管理
        public AppFileManager Files = null; //  File 管理

        #region SingleInstance
        public AppService()
        {
            mSettingMutex = new Mutex(false, "TWWeather_Setting_Mutex");
            Settings = new AppSettingManager();

            mFileMutex = new Mutex(false, "TWWeather_File_Mutex");
            Files = new AppFileManager();
        }

        static AppService()
        {
        }

        public static AppService Instance
        {
            get
            {
                if (_sAppService == null)
                {
                    _sAppService = new AppService();
                }
                return _sAppService;
            }
        }

        private static AppService _sAppService;
        #endregion

        public Boolean NoticeTile
        {
            get
            {
                Boolean isNotice = Settings.Load(Constants.SETTING_KEY_NOTICE_TILE, false);
                if (!isNotice)
                {
                    Settings.Store(Constants.SETTING_KEY_NOTICE_TILE, true);
                }
                return isNotice;
            }
        }

        public Boolean FirstLaunch
        {
            get
            {
                Boolean isFirst = Settings.Load(Constants.SETTING_KEY_FIRST_LAUNCH, true);
                if (isFirst)
                {
                    Settings.Store(Constants.SETTING_KEY_FIRST_LAUNCH, false);
                    return true;
                }
                return false;
            }
        }

        public Boolean FirstLaunchTile
        {
            get
            {
                Boolean isFirst = Settings.Load(Constants.SETTING_KEY_FIRST_LAUNCH_TILE, true);
                if (isFirst)
                {
                    Settings.Store(Constants.SETTING_KEY_FIRST_LAUNCH_TILE, false);
                    return true;
                }
                return false;
            }
        }

        public void UpdateKnownsAreaList(List<SimpleListItem> listArea)
        {
            mAreaList.Clear();
            if(listArea != null)
            {
                foreach (SimpleListItem s in listArea)
                {
                    if (s.ItemType != WeatherItemType.WI_TYPE_NON)
                    {
                        mAreaList.Add(s.Title);
                    }
                }
            }
        }

        public List<String> GetLocalAttentionAreaList()
        {
            List<String> resList = new List<string>();

            //String strList = Settings.Load(Constants.SETTING_KEY_ATTENTION_AREA_LIST, "");
            String strList = Files.Load(Constants.FILE_ATTENTION_AREA_LIST);
            String[] aStrList = strList.Split(',');
            foreach(String s in aStrList)
            {
                if (!"".Equals(s) && mAreaList.IndexOf(s) >= 0)
                {
                    resList.Add(s);
                }
            }

            return resList;
        }

        public void SetAttentionAreaList(List<String> list)
        {
            String strList = "";
            foreach (String s in list)
            {
                strList += String.Format("{0},", s);
            }
            strList = strList.TrimEnd(',');
            //Settings.Store(Constants.SETTING_KEY_ATTENTION_AREA_LIST, strList);
            Files.Save(Constants.FILE_ATTENTION_AREA_LIST, strList);
        }

        public void UpdateAttentionArea(String attentionAreaList)
        {
            if (attentionAreaList == null)
            {
                //Settings.Store(Constants.SETTING_KEY_ATTENTION_AREA_LIST, "");
                Files.Save(Constants.FILE_ATTENTION_AREA_LIST, "");
            }
            else
            {
                //Settings.Store(Constants.SETTING_KEY_ATTENTION_AREA_LIST, attentionAreaList);
                Files.Save(Constants.FILE_ATTENTION_AREA_LIST, attentionAreaList);
            }
        }

        public void AppendDefaultArea()
        {
            String strList = String.Format("{0},{1}", Constants.DEFAULT_AREA_TAIPEI, Constants.DEFAULT_AREA_KAOHSIUNG);
            Settings.Store(Constants.SETTING_KEY_ATTENTION_AREA_LIST, strList);
        }

        public void AppendAttentionArea(String areaName)
        {
            if (mAreaList.IndexOf(areaName) >= 0)
            {
                // 地區總表中有這個值
                List<String> list = GetLocalAttentionAreaList();
                if (list.IndexOf(areaName) < 0)
                {
                    // 沒有加這個值
                    list.Add(areaName);
                    SetAttentionAreaList(list);
                }
            }
        }

        public void RemoveAttentionArea(String areaName)
        {
            if (mAreaList.IndexOf(areaName) >= 0)
            {
                // 地區總表中有這個值
                List<String> list = GetLocalAttentionAreaList();
                if (list.IndexOf(areaName) >= 0)
                {
                    // Setting 中有存這個值
                    list.Remove(areaName);
                    SetAttentionAreaList(list);
                }
            }
        }

        public void CacheForecastData(String areaName)
        {
            foreach (KeyValuePair<String, List<RichListItem>> s in mAreaWeatherList)
            {
                if(s.Key.Equals(areaName))
                {
                    mCacheList = s.Value;
                    break;
                }
            }
        }

        public Boolean GetIsRemember()
        {
            return Settings.Load(Constants.SETTING_KEY_IS_REMEMBER_ATTENTIONLIST, true);
        }

        public void SetIsRemember(Boolean remember)
        {
            Settings.Store(Constants.SETTING_KEY_IS_REMEMBER_ATTENTIONLIST, remember);
        }

        public void SaveBackUpAllForecast(String forecast)
        {
            Files.Save(Constants.FILE_BACKUP_ALL_FORECAST, forecast);
        }

        public String GetBackUpAllForecast()
        {
            return Files.Load(Constants.FILE_BACKUP_ALL_FORECAST);
        }

        public void MoveAttentionAreaFromSettingToFile()
        {
            String setting = Settings.Load(Constants.SETTING_KEY_ATTENTION_AREA_LIST, "");
            Files.Save(Constants.FILE_ATTENTION_AREA_LIST, setting);
        }
    }
}
