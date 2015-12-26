using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Resources;
using System.Text;

namespace TWWeather
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            // 把關於我讀出來
            StreamResourceInfo resource = Application.GetResourceStream(new Uri("Data/AboutMe.txt", UriKind.Relative));
            Byte[] btRes = new Byte[resource.Stream.Length];
            resource.Stream.Read(btRes, 0, (int)resource.Stream.Length);
            String aboutText = Encoding.UTF8.GetString(btRes, 0, btRes.Length);
            _aboutText = aboutText;

            DataContext = this;
        }

        private String _aboutText;
        public String AboutText
        {
            get
            {
                return _aboutText;
            }
        }
    }
}