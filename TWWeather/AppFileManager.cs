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
using System.IO;
using System.Text;

namespace TWWeather
{
    public class AppFileManager
    {
        public AppFileManager()
        {
        }

        public String Load(String filePath)
        {
            String strRes = "";

            try
            {
                AppService.Instance.mFileMutex.WaitOne();
                try
                {
                    IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                    if (isoFile.FileExists(filePath))
                    {
                        IsolatedStorageFileStream fStream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, isoFile);
                        if (fStream != null && fStream.Length > 0)
                        {
                            Byte[] btReadBuf = new Byte[(int)fStream.Length];
                            btReadBuf.Initialize();
                            int nCurrentRead = fStream.Read(btReadBuf, 0, btReadBuf.Length);
                            if (nCurrentRead == (int)fStream.Length)
                            {
                                // 正常讀丸
                                strRes = Encoding.UTF8.GetString(btReadBuf, 0, btReadBuf.Length);
                            }
                        }
                        fStream.Close();
                        fStream.Dispose();
                    }
                    isoFile.Dispose();
                }
                catch (Exception)
                {
                    strRes = "";
                }
            }
            finally
            {
                AppService.Instance.mFileMutex.ReleaseMutex();
            }

            return strRes;
        }

        public void Save(String filePath, String data)
        {
            try
            {
                AppService.Instance.mFileMutex.WaitOne();
                try
                {
                    IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                    if (isoFile.FileExists(filePath))
                    {
                        isoFile.DeleteFile(filePath);
                    }
                    IsolatedStorageFileStream file = isoFile.OpenFile(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    if (!"".Equals(data))
                    {
                        Byte[] byteArray = Encoding.UTF8.GetBytes(data);
                        file.Write(byteArray, 0, byteArray.Length);
                    }
                    file.Close();
                    file.Dispose();
                    isoFile.Dispose();
                }
                catch (Exception)
                {
                }
            }
            finally
            {
                AppService.Instance.mFileMutex.ReleaseMutex();
            }
        }
    }
}
