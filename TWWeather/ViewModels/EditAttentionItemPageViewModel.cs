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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TWWeather.AppServices.Models;

namespace TWWeather
{
    public class EditAttentionItemPageViewModel : ViewModel
    {
        public class AddItem : ViewModel
        {
            private String _title;
            public String Title
            {
                get
                {
                    return _title;
                }
                set
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }

            private Visibility _showCheck;
            public Visibility ShowCheck
            {
                get
                {
                    return _showCheck;
                }
                set
                {
                    _showCheck = value;
                    NotifyPropertyChanged("ShowCheck");
                }
            }
        }

        public Boolean IsDataLoaded
        {
            get;
            private set;
        }

        public ObservableCollection<AddItem> ListItemSource { get; private set; }

        public EditAttentionItemPageViewModel()
        {
            IsDataLoaded = false;
            ListItemSource = new ObservableCollection<AddItem>();
            InitData();
            IsDataLoaded = true;
        }

        public void InitData()
        {
            Dictionary<String, List<RichListItem>> allForecast = AppService.Instance.mAreaWeatherList;
            List<String> listAttentionArea = AppService.Instance.GetLocalAttentionAreaList();

            ListItemSource.Clear();
            foreach (KeyValuePair<String, List<RichListItem>> s in allForecast)
            {
                if (listAttentionArea.IndexOf(s.Key) < 0)
                {
                    // 總表中的這個值不是 "已加入" 地區
                    ListItemSource.Add(new AddItem()
                    {
                        Title = s.Key,
                        ShowCheck = Visibility.Collapsed
                    });
                }
                else
                {
                    ListItemSource.Add(new AddItem()
                    {
                        Title = s.Key,
                        ShowCheck = Visibility.Visible
                    });
                }
            }
        }

        public void ReFreshCheckStatus(int idx)
        {
            if (idx < 0 || idx >= ListItemSource.Count)
            {
                return;
            }

            if (ListItemSource[idx].ShowCheck == Visibility.Collapsed)
            {
                // 目前看不到，設為看得到
                ListItemSource[idx].ShowCheck = Visibility.Visible;
            }
            else
            {
                // 看得到，設為看不到
                ListItemSource[idx].ShowCheck = Visibility.Collapsed;
            }

            NotifyPropertyChanged("ListItemSource");
        }

        public void UpdateAttentionArea()
        {
            // 將打勾的加入
            //foreach(AddItem item in ListItemSource)
            //{
            //    if (item.ShowCheck == Visibility.Visible)
            //    {
            //        AppService.Instance.AppendAttentionArea(item.Title);
            //    }
            //}

            // 因為改成可以從此頁取消加入，所以要全部掃過一遍
            String strAttentionAreaList = "";
            foreach (AddItem item in ListItemSource)
            {
                if (item.ShowCheck == Visibility.Visible)
                {
                    strAttentionAreaList += String.Format("{0},", item.Title);
                }
            }
            strAttentionAreaList = strAttentionAreaList.TrimEnd(',');
            AppService.Instance.UpdateAttentionArea(strAttentionAreaList);
        }
    }
}
