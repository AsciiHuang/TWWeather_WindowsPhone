using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using TWWeather.AppServices.Models;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TWWeather.TileServices
{
    public class TileService
    {
        public Dictionary<String, List<RichListItem>> mAreaWeatherList = null;
        
        #region SingleInstance
        public TileService()
        {
            mAreaWeatherList = new Dictionary<string, List<RichListItem>>();
        }

        static TileService()
        {
        }

        public static TileService Instance
        {
            get
            {
                if (_sTileService == null)
                {
                    _sTileService = new TileService();
                }
                return _sTileService;
            }
        }

        private static TileService _sTileService;
        #endregion

        public void RefreshAllTile()
        {
            if (mAreaWeatherList == null)
            {
                return;
            }

            foreach (KeyValuePair<String, List<RichListItem>> areaData in mAreaWeatherList)
            {
                String strAreaName = areaData.Key;
                String strTileKeyWord = String.Format(Constants.TILE_NAVIGATION_KEYWORD, strAreaName);
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(strTileKeyWord));
                if (tile == null)
                {
                    Debug.WriteLine(">>>>> non find Tile :: " + strTileKeyWord);
                }
                else
                {
                    Debug.WriteLine(">>>>> find Tile :: " + strTileKeyWord);
                    UpdateTile(strAreaName);
                }
            }
        }

        public void UpdateTile(String area)
        {
            if (mAreaWeatherList == null || mAreaWeatherList.Count <= 0)
            {
                return;
            }

            if (mAreaWeatherList.ContainsKey(area))
            {
                if (mAreaWeatherList.ContainsKey(area))
                {
                    List<RichListItem> items = mAreaWeatherList[area];
                    if (items.Count == 3)
                    {
                        String strAreaMD5 = MD5Core.GetHashString(area);
                        String strBackgroundImageFileName = String.Format("Fore_{0}.jpg", strAreaMD5);
                        String strBackBackgroundImageFileName = String.Format("Back_{0}.jpg", strAreaMD5);
                        String strBackgroundImageFilePath = GetShareToTileAlbumCoverPath(strBackgroundImageFileName);
                        String strBackBackgroundImageFilePath = GetShareToTileAlbumCoverPath(strBackBackgroundImageFileName);

                        Boolean IsRefreshOK = false;
                        try
                        {
                            CreateTileBackgroundFile(items[0], strBackgroundImageFilePath);
                            CreateTileBackBackgroundFile(items[1], items[2], strBackBackgroundImageFilePath);
                            IsRefreshOK = true;
                        }
                        catch (Exception)
                        {
                        }

                        if (IsRefreshOK)
                        {
                            String strTileKeyWord = String.Format(Constants.TILE_NAVIGATION_KEYWORD, area);
                            String strNavigationURL = String.Format(Constants.TILE_NAVIGATION_URL, area);
                            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(strTileKeyWord));

                            if (tile != null)
                            {
                                // 更新圖片
                                StandardTileData NewTileData = new StandardTileData
                                {
                                    BackgroundImage = new Uri("isostore:/" + strBackgroundImageFilePath, UriKind.Absolute),
                                    BackBackgroundImage = new Uri("isostore:/" + strBackBackgroundImageFilePath, UriKind.Absolute)
                                };
                                tile.Update(NewTileData);
                            }
                        }
                    }
                }
            }
        }

        private void CreateTileBackgroundFile(RichListItem richItem, String filePath)
        {
            SolidColorBrush brushWhite = new SolidColorBrush(Colors.White);
            WriteableBitmap bmpTile = new WriteableBitmap(173, 173);

            #region 底色
            Rectangle rect = new Rectangle();
            rect.Width = 173;
            rect.Height = 173;
            rect.Fill = new SolidColorBrush(Color.FromArgb(255, 185, 183, 172));
            bmpTile.Render(rect, null);

            Rectangle rectSub = new Rectangle();
            rectSub.Width = 163;
            rectSub.Height = 163;
            rectSub.RadiusX = 6;
            rectSub.RadiusY = 6;
            rectSub.Fill = new SolidColorBrush(Color.FromArgb(255, 168, 165, 152));
            bmpTile.Render(rectSub, new TranslateTransform() { X = 5, Y = 5 });
            #endregion

            #region 雨的圖片
            StreamResourceInfo resourceRain = Application.GetResourceStream(new Uri("Image/rain.png", UriKind.Relative));
            BitmapImage bmpRain;
            using (resourceRain.Stream)
            {
                bmpRain = new BitmapImage();
                bmpRain.SetSource(resourceRain.Stream);
                resourceRain.Stream.Close();
            }
            Image imgRain = new Image()
            {
                Width = 18,
                Height = 16,
                Source = bmpRain,
            };
            bmpTile.Render(imgRain, new TranslateTransform() { X = 10, Y = 49 });
            #endregion

            #region 天氣圖片
            String strWeatherImage = GetImageByWeatherDescription(richItem.Description, richItem.Title);
            StreamResourceInfo resourceWeather = Application.GetResourceStream(new Uri(strWeatherImage, UriKind.Relative));
            BitmapImage bmpWeather;
            using (resourceWeather.Stream)
            {
                bmpWeather = new BitmapImage();
                bmpWeather.SetSource(resourceWeather.Stream);
                resourceWeather.Stream.Close();
            }
            Image imgWeahter = new Image()
            {
                Width = 175,
                Height = 120,
                Source = bmpWeather,
            };
            bmpTile.Render(imgWeahter, new TranslateTransform() { X = 30, Y = 38 });
            #endregion

            #region 地名
            TextBlock lblAddress = new TextBlock();
            lblAddress.Text = richItem.Area;
            lblAddress.FontSize = 24;
            lblAddress.FontWeight = FontWeights.Bold;
            lblAddress.Foreground = brushWhite;
            bmpTile.Render(lblAddress, new TranslateTransform() { X = 10, Y = 10 });
            #endregion

            #region 降雨
            TextBlock lblRain = new TextBlock();
            lblRain.Text = String.Format("{0}{1}", richItem.ChanceOfRain, Constants.SUFFIX_CHANCE_OF_RAIN);
            lblRain.FontSize = 18;
            lblRain.FontWeight = FontWeights.Bold;
            lblRain.Foreground = brushWhite;
            bmpTile.Render(lblRain, new TranslateTransform() { X = 32, Y = 47 });
            #endregion

            #region 溫度範圍
            TextBlock lblRange = new TextBlock();
            lblRange.Text = String.Format("{0}{1}", richItem.Temperature, Constants.SUFFIX_TEMPERATURE);
            lblRange.FontSize = 32;
            lblRange.FontWeight = FontWeights.ExtraBold;
            lblRange.Foreground = brushWhite;
            bmpTile.Render(lblRange, new TranslateTransform() { X = 10, Y = 135 });
            #endregion

            // 存檔
            bmpTile.Invalidate();
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(filePath))
            {
                isoStore.DeleteFile(filePath);
            }
            IsolatedStorageFileStream file = isoStore.CreateFile(filePath);
            bmpTile.SaveJpeg(file, 173, 173, 0, 100);
            file.Close();
            file.Dispose();
        }

        private void CreateTileBackBackgroundFile(RichListItem item1, RichListItem item2, String filePath)
        {
            SolidColorBrush brushWhite = new SolidColorBrush(Colors.White);
            WriteableBitmap bmpTile = new WriteableBitmap(173, 173);

            #region 底色
            Rectangle rect = new Rectangle();
            rect.Width = 173;
            rect.Height = 173;
            rect.Fill = new SolidColorBrush(Color.FromArgb(255, 185, 183, 172));
            bmpTile.Render(rect, null);

            Rectangle rectSub = new Rectangle();
            rectSub.Width = 162;
            rectSub.Height = 78;
            rectSub.RadiusX = 6;
            rectSub.RadiusY = 6;
            rectSub.Fill = new SolidColorBrush(Color.FromArgb(255, 168, 165, 152));
            bmpTile.Render(rectSub, new TranslateTransform() { X = 5, Y = 5 });
            bmpTile.Render(rectSub, new TranslateTransform() { X = 5, Y = 89 });
            #endregion

            #region 雨的圖片
            StreamResourceInfo resourceRain = Application.GetResourceStream(new Uri("Image/rain.png", UriKind.Relative));
            BitmapImage bmpRain;
            using (resourceRain.Stream)
            {
                bmpRain = new BitmapImage();
                bmpRain.SetSource(resourceRain.Stream);
                resourceRain.Stream.Close();
            }
            Image imgRain = new Image()
            {
                Width = 18,
                Height = 16,
                Source = bmpRain,
            };
            bmpTile.Render(imgRain, new TranslateTransform() { X = 15, Y = 62 });
            bmpTile.Render(imgRain, new TranslateTransform() { X = 15, Y = 146 });
            #endregion

            #region 天氣圖片
            String strWeatherImageA = GetImageByWeatherDescription(item1.Description, item1.Title);
            StreamResourceInfo resourceWeatherA = Application.GetResourceStream(new Uri(strWeatherImageA, UriKind.Relative));
            BitmapImage bmpWeatherA;
            using (resourceWeatherA.Stream)
            {
                bmpWeatherA = new BitmapImage();
                bmpWeatherA.SetSource(resourceWeatherA.Stream);
                resourceWeatherA.Stream.Close();
            }
            Image imgWeahterA = new Image()
            {
                Width = 73,
                Height = 50,
                Source = bmpWeatherA,
            };
            bmpTile.Render(imgWeahterA, new TranslateTransform() { X = 8, Y = 8 });

            String strWeatherImageB = GetImageByWeatherDescription(item2.Description, item2.Title);
            StreamResourceInfo resourceWeatherB = Application.GetResourceStream(new Uri(strWeatherImageB, UriKind.Relative));
            BitmapImage bmpWeatherB;
            using (resourceWeatherB.Stream)
            {
                bmpWeatherB = new BitmapImage();
                bmpWeatherB.SetSource(resourceWeatherB.Stream);
                resourceWeatherB.Stream.Close();
            }
            Image imgWeahterB = new Image()
            {
                Width = 73,
                Height = 50,
                Source = bmpWeatherB,
            };
            bmpTile.Render(imgWeahterB, new TranslateTransform() { X = 8, Y = 92 });
            #endregion

            #region 降雨
            TextBlock lblRainA = new TextBlock();
            lblRainA.Text = String.Format("{0}{1}", item1.ChanceOfRain, Constants.SUFFIX_CHANCE_OF_RAIN);
            lblRainA.FontSize = 18;
            lblRainA.FontWeight = FontWeights.Bold;
            lblRainA.Foreground = brushWhite;
            bmpTile.Render(lblRainA, new TranslateTransform() { X = 36, Y = 60 });

            TextBlock lblRainB = new TextBlock();
            lblRainB.Text = String.Format("{0}{1}", item2.ChanceOfRain, Constants.SUFFIX_CHANCE_OF_RAIN);
            lblRainB.FontSize = 18;
            lblRainB.FontWeight = FontWeights.Bold;
            lblRainB.Foreground = brushWhite;
            bmpTile.Render(lblRainB, new TranslateTransform() { X = 36, Y = 144 });
            #endregion

            #region 溫度範圍
            String[] unitTempA = GetTemperatureUnit(item1.Temperature);
            TextBlock lblRangeA = new TextBlock();
            lblRangeA.FontSize = 26;
            lblRangeA.FontWeight = FontWeights.ExtraBold;
            lblRangeA.Foreground = brushWhite;
            lblRangeA.Text = unitTempA[0];
            bmpTile.Render(lblRangeA, new TranslateTransform() { X = 92, Y = 20 });
            lblRangeA.Text = unitTempA[1];
            bmpTile.Render(lblRangeA, new TranslateTransform() { X = 120, Y = 54 });

            String[] unitTempB = GetTemperatureUnit(item2.Temperature);
            TextBlock lblRangeB = new TextBlock();
            lblRangeB.FontSize = 26;
            lblRangeB.FontWeight = FontWeights.ExtraBold;
            lblRangeB.Foreground = brushWhite;
            lblRangeB.Text = unitTempB[0];
            bmpTile.Render(lblRangeB, new TranslateTransform() { X = 92, Y = 104 });
            lblRangeB.Text = unitTempB[1];
            bmpTile.Render(lblRangeB, new TranslateTransform() { X = 120, Y = 138 });
            #endregion

            #region 畫溫度中間的對角線
            Line line = new Line();
            line.Fill = brushWhite;
            line.Width = 55;
            line.Height = 55;
            line.X1 = 0;
            line.Y1 = 54;
            line.X2 = 54;
            line.Y2 = 0;
            line.Stretch = Stretch.Fill;
            line.Stroke = brushWhite;
            line.StrokeThickness = 2;
            bmpTile.Render(line, new TranslateTransform() { X = 95, Y = 22 });
            bmpTile.Render(line, new TranslateTransform() { X = 95, Y = 106 });
            #endregion

            // 存檔
            bmpTile.Invalidate();
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(filePath))
            {
                isoStore.DeleteFile(filePath);
            }
            IsolatedStorageFileStream file = isoStore.CreateFile(filePath);
            bmpTile.SaveJpeg(file, 173, 173, 0, 100);
            file.Close();
            file.Dispose();
        }

        public String GetShareToTileAlbumCoverPath(String fileName)
        {
            return "Shared/ShellContent/" + fileName;
        }

        public String[] GetTemperatureUnit(String temperature)
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

        public String GetImageByWeatherDescription(String description, String title)
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

        private String GetImageByWeatherDescriptionKeyWord(String description, String preFix)
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

        public Boolean FindAreaLiveTile()
        {
            Boolean bFind = false;

            List<String> listArea = TileService.Instance.LoadAttentionList();

            if (listArea != null && listArea.Count > 0)
            {
                foreach (String area in listArea)
                {
                    String strTileKeyWord = String.Format(Constants.TILE_NAVIGATION_KEYWORD, area);
                    ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(strTileKeyWord));
                    if (tile != null)
                    {
                        bFind = true;
                        break;
                    }
                }
            }

            return bFind;
        }

        public List<String> LoadAttentionList()
        {
            List<String> listRes = new List<string>();
            String strList = "";
            try
            {
                #region 讀出字串到 strList
                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoFile.FileExists(Constants.FILE_ATTENTION_AREA_LIST))
                {
                    IsolatedStorageFileStream fStream = new IsolatedStorageFileStream(Constants.FILE_ATTENTION_AREA_LIST, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, isoFile);
                    if (fStream != null && fStream.Length > 0)
                    {
                        Byte[] btReadBuf = new Byte[(int)fStream.Length];
                        btReadBuf.Initialize();
                        int nCurrentRead = fStream.Read(btReadBuf, 0, btReadBuf.Length);
                        if (nCurrentRead == (int)fStream.Length)
                        {
                            // 正常讀丸
                            strList = Encoding.UTF8.GetString(btReadBuf, 0, btReadBuf.Length);
                        }
                    }
                    fStream.Close();
                    fStream.Dispose();
                }
                isoFile.Dispose();
                #endregion

                if (!String.IsNullOrEmpty(strList))
                {
                    String[] aStrList = strList.Split(',');
                    foreach (String s in aStrList)
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            listRes.Add(s);
                        }
                    }
                }
            }
            catch (Exception)
            {
                listRes.Clear();
            }

            return listRes;
        }
    }
}
