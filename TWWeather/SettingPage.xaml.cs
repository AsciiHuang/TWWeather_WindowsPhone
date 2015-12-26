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

namespace TWWeather
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Boolean isRemember = AppService.Instance.GetIsRemember();
            Boolean isRecommendation = AppService.Instance.GetIsRecommendation();

            switchRemember.IsChecked = isRemember;
            switchRecommendation.IsChecked = isRecommendation;

            if (isRemember)
            {
                IsRemember.Text = "On";
            }
            else
            {
                IsRemember.Text = "Off";
            }

            if (isRecommendation)
            {
                IsRecommendation.Text = "On";
            }
            else
            {
                IsRecommendation.Text = "Off";
            }
        }

        private void OnRememberToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            AppService.Instance.SetIsRemember(true);
            IsRemember.Text = "On";
        }

        private void OnRememberToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            AppService.Instance.SetIsRemember(false);
            IsRemember.Text = "Off";
        }

        private void OnRecommendationToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            AppService.Instance.SetIsRecommendation(true);
            IsRecommendation.Text = "On";
        }

        private void OnRecommendationToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            AppService.Instance.SetIsRecommendation(false);
            IsRecommendation.Text = "Off";
        }
    }
}