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
    public class Constants
    {
        public const String WEATHER_WARNING_URL = "http://twweatherapi.herokuapp.com/warning?output=json"; // 重要天氣警告
        public const String WEATHER_OVERVIEW_URL = "http://twweatherapi.herokuapp.com/overview"; // 氣象局新聞
        public const String WEATHER_FORECAST_URL = "http://twweatherapi.herokuapp.com/forecast?output=json&location=all"; // 48 小時 => location=2

        public const String DEFAULT_AREA_TAIPEI = "台北市";
        public const String DEFAULT_AREA_KAOHSIUNG = "高雄市";
        public const String DEFAULT_TIMEBLOCK_DESCRIPTION = "白天";
        public const String TWWEATHER_BACKGROUND_SCHEDULER = "TWWeatherBackgroundScheduler";

        public const String TILE_NAVIGATION_KEYWORD = "TileId={0}";
        public const String TILE_NAVIGATION_URL = "/MainPage.xaml?TileId={0}";

        public const String SETTING_KEY_FIRST_LAUNCH = "TWWeatherFirstLaunch";
        public const String SETTING_KEY_NOTICE_TILE = "TWWeatherNoticeTile";
        public const String SETTING_KEY_FIRST_LAUNCH_TILE = "TWWeatherFirstLaunchTile";
        public const String SETTING_KEY_ATTENTION_AREA_LIST = "TWWeatherAttentionList";
        public const String SETTING_KEY_IS_REMEMBER_ATTENTIONLIST = "TWWeatherIsRememberAttentionList";
        public const String FILE_BACKUP_ALL_FORECAST = "TWWeatherBackupAllForecast.tww";

        public const String FILE_ATTENTION_AREA_LIST = "TWWeatherAttentionList.ini";

        #region Prefix
        public const String PREFIX_CHANCE_OF_RAIN = "降雨機率 ";
        public const String PREFIX_PUBLISH_TIME = "發佈時間：";
        public const String PREFIX_TEMPERATURE = "溫度：";
        public const String PREFIX_RAINSCALE = "累積雨量：";
        public const String PREFIX_WINDDIRECTION = "風向：";
        public const String PREFIX_WINDSCALE = "風力：";
        public const String PREFIX_GUSTWINDSCALE = "陣風：";
        public const String PREFIX_VALIDTIME = "有效時間";
        #endregion

        #region Suffix
        public const String SUFFIX_SIMPLE_TEMPERATURE = "°";
        public const String SUFFIX_TEMPERATURE = " °C";
        public const String SUFFIX_CHANCE_OF_RAIN = " %";
        public const String SUFFIX_RAINSCALE = " mm";
        public const String SUFFIX_WINDSCALE = " 級";
        public const String SUFFIX_GUSTWINDSCALE = " 級";
        #endregion
    }
}
