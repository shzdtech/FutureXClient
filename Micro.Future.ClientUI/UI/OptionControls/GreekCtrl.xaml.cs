﻿using Micro.Future.Utility;
using Micro.Future.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class GreekCtrl : UserControl
    {
        public GreekCtrl()
        {
            InitializeComponent();

        }

        public void BindingToSource(ObservableCollection<RiskVM> source)
        {

            var cvs = CollectionViewSource.GetDefaultView(source);
            if (cvs != null)
            {
                cvs.GroupDescriptions.Add(new PropertyGroupDescription("Underlying"));
            }

            GreekListView.ItemsSource = null;
            GreekListView.ItemsSource = cvs;
        }

        private void GreekListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, GreekListView.Items);
            }
        }

    }

    public class DeltaConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return null;

            ReadOnlyObservableCollection<object> items =
              (ReadOnlyObservableCollection<object>)value;

            var delta = items.Sum(c => ((RiskVM)c).Delta);
            var gamma = items.Sum(c => ((RiskVM)c).Gamma);
            var theta = items.Sum(c => ((RiskVM)c).Theta365);
            var vega = items.Sum(c => ((RiskVM)c).Vega100);

            return string.Format("\t {0:N2}\t{1:N4}\t{2:N2}\t{3:N2}", delta, gamma, vega, theta);
        }

        public object ConvertBack(object value, System.Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
    public class GammaConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (null == value)
                return null;

            ReadOnlyObservableCollection<object> items =
              (ReadOnlyObservableCollection<object>)value;

            var gamma = items.Sum(c => ((RiskVM)c).Gamma);

            return gamma;
        }

        public object ConvertBack(object value, System.Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

}
