using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using Micro.Future.Message;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using System.ComponentModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// OTCTradingDesk01.xaml 的交互逻辑
    /// </summary>  

    public partial class OTCTradingStrategyLV : UserControl, IReloadData
    {
        public OTCTradingStrategyLV()
        {
            InitializeComponent();
        }

        public AbstractOTCHandler OTCHandler { get; set; }
        public string PersistanceId
        {
            get;
            set;
        }

        public void ReloadData()
        {
            OTCTradingLV.ItemsSource = OTCHandler?.StrategyVMCollection;
            OTCHandler?.StrategyVMCollection.Clear();
            OTCHandler?.QueryStrategy();
            OTCHandler?.SubMarketData(new List<string>());
        }

        private void OnStrategyChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FrameworkElement ctrl = sender as FrameworkElement;
            StrategyVM strategyVM = ctrl.DataContext as StrategyVM;
            if (strategyVM != null)
                strategyVM.UpdateStrategy();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {
                    StrategyVM strategyVM = OTCTradingLV.SelectedItem as StrategyVM;
                    if (strategyVM != null)
                    {
                        if (e.Key == Key.Enter)
                            strategyVM.UpdateStrategy();
                        else
                        {
                            ctrl.DataContext = null;
                            ctrl.DataContext = strategyVM;
                        }
                    }
                    ctrl.Background = Brushes.White;
                }
                else
                {
                    ctrl.Background = Brushes.MistyRose;
                }
            }
        }

        private async void OTCTradingLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StrategyVM strategyVM = OTCTradingLV.SelectedItem as StrategyVM;
            if (strategyVM != null)
            {
                var paramsVM = await OTCHandler?.QueryModelParamsAsync(strategyVM.PricingModel);
                StrategyParam.ItemsSource = paramsVM.Params;
            }
        }
    }
}
