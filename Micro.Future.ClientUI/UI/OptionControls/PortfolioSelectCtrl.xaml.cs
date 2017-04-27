﻿using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
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
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class PortfolioSelectCtrl : UserControl
    {
        public LayoutContent LayoutContent { get; set; }
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        public PortfolioSelectCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
        }


        public string PersistanceId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private void strategyListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, strategyListView.Items);
            }
        }

        private void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue.ToString();
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategySymbolList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                    .Select(c => new { StrategyName = c.StrategySym }).Distinct().ToList();
                strategyListView.ItemsSource = strategySymbolList;
                var portfolioDataContext = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection
                    .Where(c => c.Name == portfolio).Distinct();
                DelayTxt.DataContext = portfolioDataContext;
                Threshold.DataContext = portfolioDataContext;
                var basecontractsList = strategyVMCollection.Select(c => c.BaseContract).Distinct().ToList();
                foreach ( var sVM in strategyVMCollection )
                {
                    var _pricingcontractList = sVM.PricingContractParams.Select(c => c.Contract).Distinct().ToList();
                }
                //var mixcontractList = basecontractsList.Union(_pricingcontractList).ToList();
            }

        }

        private void Delay_KeyUp(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                var portfolioVM = DelayTxt.DataContext as PortfolioVM;
                if (e.Key == Key.Enter)
                {
                    //portfolioVM.Delay = DelayTxt.;
                    portfolioVM.UpdatePortfolio();
                }
            }
        }
        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        private void DelayValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    int delay = (int)e.NewValue;
                    portfolioVM.Delay = delay;
                    portfolioVM.UpdatePortfolio();
                }
            }
        }
        private void ThresholdValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    double threshold = (double)e.NewValue;
                    portfolioVM.Threshold = threshold;
                    portfolioVM.UpdatePortfolio();
                }
            }
        }
    }
}