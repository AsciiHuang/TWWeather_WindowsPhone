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
using System.IO.IsolatedStorage;

namespace TWWeather
{
    public class AppSettingManager
    {
        public AppSettingManager()
        {
        }

        public valueType Load<valueType>(String key, valueType defaultValue)
        {
            valueType local;
            try
            {
                AppService.Instance.mSettingMutex.WaitOne();
                try
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        local = (valueType)IsolatedStorageSettings.ApplicationSettings[key];
                    }
                    else
                    {
                        local = defaultValue;
                        Store(key, defaultValue);
                    }
                }
                catch (Exception)
                {
                    IsolatedStorageSettings.ApplicationSettings.Clear();
                    local = defaultValue;
                }
            }
            finally
            {
                AppService.Instance.mSettingMutex.ReleaseMutex();
            }
            return local;
        }

        public void Store(String key, Object o)
        {
            try
            {
                AppService.Instance.mSettingMutex.WaitOne();
                IsolatedStorageSettings.ApplicationSettings[key] = o;
                try
                {
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
                catch (Exception)
                {
                }
            }
            finally
            {
                AppService.Instance.mSettingMutex.ReleaseMutex();
            }
        }
    }
}
