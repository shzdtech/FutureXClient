﻿using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionContractRiskGraphCtrl : UserControl

    {
        private IDictionary<string, int> _riskDict = new Dictionary<string, int>();
        public List<ColumnItem> BarItemCollection
        {
            get;
        } = new List<ColumnItem>();

        private HashSet<string> _riskSet = new HashSet<string>();

        private Timer _timer;
        private const int UpdateInterval = 1000;
        public MarketDataVM MarketDataVM
        {
            get;
            set;
        }
        public class StrategyBaseVM
        {
            public string OptionContract
            {
                get;
                set;
            }
            public string Contract
            {
                get;
                set;
            }
            public string Expiration
            {
                get;
                set;
            }
            public bool RiskGraphEnable
            {
                get;
                set;
            }
            public double Valuation
            {
                get;
                set;
            }
            public bool Selected
            {
                get;
                set;
            }
            public double LastPrice
            {
                get;
                set;
            }
            public double SettlePrice
            {
                get;
                set;
            }
            public MarketDataVM MktVM
            {
                get;
                set;
            }

        }

        private TraderExHandler _tradeExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private MarketDataHandler _marketDataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();

        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private async void PnLIndex(string portfolio)
        {
            var positions = _tradeExHandler.PositionVMCollection.Where(p => p.Portfolio == portfolio);

            foreach (var vm in positions)
            {
                var contractinfo = ClientDbContext.FindContract(vm.Contract);
                string basecontract = null;
                if (contractinfo != null)
                {
                    if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                    {
                        basecontract = contractinfo.UnderlyingContract;
                    }
                    else
                        basecontract = contractinfo.Contract;
                    MarketDataVM = await _marketDataHandler.SubMarketDataAsync(basecontract);
                }
                if (_riskSet.Contains(basecontract))
                {
                    if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                 || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION)
                 || (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE))
                    {
                        int index;
                        var basecontractPosition = positions.Where(p => p.Contract == basecontract).FirstOrDefault();
                        if (basecontractPosition.Direction == PositionDirectionType.PD_LONG)
                        {
                            basecontractPosition.Profit = (MarketDataVM.LastPrice - basecontractPosition.MeanCost) * basecontractPosition.Position * basecontractPosition.Multiplier;
                        }
                        else if (basecontractPosition.Direction == PositionDirectionType.PD_SHORT)
                        {
                            basecontractPosition.Profit = (basecontractPosition.MeanCost - MarketDataVM.LastPrice) * basecontractPosition.Position * basecontractPosition.Multiplier;
                        }
                        if (_riskDict.TryGetValue(basecontract, out index))
                        {

                            var barItem = BarItemCollection[index];
                            barItem.Value += basecontractPosition.Profit;
                        }
                    }
                }
            }
            plotModel.InvalidatePlot(true);
        }
        private async void RiskIndex(string portfolio)
        {
            var queryvaluation = new QueryValuation();
            foreach (var item in expirationLV.ItemsSource)
            {
                var strategyvm = item as StrategyBaseVM;

                double price = 0;
                if (strategyvm.Selected)
                {
                    if (marketRadioButton.IsChecked.Value)
                    {
                        price = strategyvm.MktVM.LastPrice;
                    }
                    else if (settlementRadioButton.IsChecked.Value)
                    {
                        price = strategyvm.MktVM.SettlePrice;
                    }
                    else if (valuationRadioButton.IsChecked.Value)
                    {
                        price = strategyvm.Valuation;
                    }

                    queryvaluation.ContractParams[strategyvm.Contract] = new ValuationParam { Price = price, Volatitly = 0 };

                }
            }
            var riskVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
            foreach (var vm in riskVMlist)
            {
                var contractinfo = ClientDbContext.FindContract(vm.Contract);
                string basecontract = null;
                if (contractinfo != null)
                {
                    if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                    {
                        basecontract = contractinfo.UnderlyingContract;
                    }
                    else
                        basecontract = contractinfo.Contract;
                    //MarketDataVM = await _marketDataHandler.SubMarketDataAsync(vm.Contract);

                }

                if (_riskSet.Contains(basecontract))
                {
                    //var contractinfo = ClientDbContext.FindContract(vm.Contract);

                    if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION)
                || (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE))

                    {
                        int index;
                        //var basecontractPosition = positions.Where(p => p.Contract == vm.Contract).FirstOrDefault();
                        //if (basecontractPosition.Direction == PositionDirectionType.PD_LONG)
                        //{
                        //    basecontractPosition.Profit = (MarketDataVM.LastPrice - basecontractPosition.MeanCost) * basecontractPosition.Position * basecontractPosition.Multiplier;
                        //}
                        //else if (basecontractPosition.Direction == PositionDirectionType.PD_SHORT)
                        //{
                        //    basecontractPosition.Profit = (basecontractPosition.MeanCost - MarketDataVM.LastPrice) * basecontractPosition.Position * basecontractPosition.Multiplier;
                        //}

                        if (_riskDict.TryGetValue(basecontract, out index))
                        {

                            var barItem = BarItemCollection[index];
                            if (deltaRadioButton.IsChecked.Value)
                                barItem.Value += vm.Delta;
                            else if (gammaRadioButton.IsChecked.Value)
                                barItem.Value += vm.Gamma;
                            else if (vegaRadioButton.IsChecked.Value)
                                barItem.Value += vm.Vega100;
                            else if (thetaRadioButton.IsChecked.Value)
                                barItem.Value += vm.Theta365;
                            else if (rhoRadioButton.IsChecked.Value)
                                barItem.Value += vm.Rho100;
                            //else if (pnlRadioButton.IsChecked.Value)
                            //    barItem.Value += basecontractPosition.Profit;
                        }
                    }
                }
                //if (_riskSet.Contains(futurecontract))
                //{
                //    if(futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE)
                //    {
                //        int index;

                //        if (_riskDict.TryGetValue(basecontract, out index))
                //        {

                //            var barItem = BarItemCollection[index];
                //            if (deltaRadioButton.IsChecked.Value)
                //                barItem.Value += vm.Delta;
                //            else if (gammaRadioButton.IsChecked.Value)
                //                barItem.Value += vm.Gamma;
                //            else if (vegaRadioButton.IsChecked.Value)
                //                barItem.Value += vm.Vega100;
                //            else if (thetaRadioButton.IsChecked.Value)
                //                barItem.Value += vm.Theta365;
                //        }
                //    }
                //}

                plotModel.InvalidatePlot(true);
            }

        }
        private void ReloadDataCallback(object state)
        {
            Dispatcher.Invoke( () =>
            {
                _tradeExHandler.QueryPosition();
                var positionCollection = _tradeExHandler.PositionVMCollection;
                var portfolio = portfolioCB.SelectedValue?.ToString();
                var positions = _tradeExHandler.PositionVMCollection.Where(p => p.Portfolio == portfolio);
                //var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                lock (BarItemCollection)
                {
                    foreach (var baritem in BarItemCollection)
                    {
                        baritem.Value = 0;
                    }
                }
                //RiskIndex(portfolio);
                if (pnlRadioButton.IsChecked.Value)
                    PnLIndex(portfolio);
                else if (!pnlRadioButton.IsChecked.Value)
                    RiskIndex(portfolio);
            });
        }

        public OptionContractRiskGraphCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
        }

        private async void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                deltaRadioButton.IsChecked = true;
                marketRadioButton.IsChecked = true;
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract))
                    .GroupBy(s => s.BaseContract).Select(c => new StrategyBaseVM { Contract = c.First().BaseContract, OptionContract = c.First().Contract }).ToList();
                var strategyVMList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract)).ToList();
                foreach (var vm in strategyContractList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.OptionContract);
                    if (contractinfo != null)
                    {
                        vm.Expiration = contractinfo.ExpireDate;
                        vm.MktVM = await _marketDataHandler.SubMarketDataAsync(vm.Contract);

                    }
                }

                expirationLV.ItemsSource = strategyContractList;


                var baseContractSet = new SortedSet<string>();
                foreach (var vm in strategyVMList)
                {
                    if (vm.BaseContract != null)
                        baseContractSet.Add(vm.BaseContract);
                }

                var baseContractList = baseContractSet.ToList();
                baseContractAxis.ItemsSource = baseContractList;
                foreach (var vm in strategyVMList)
                {
                    if (vm.BaseContract != null)
                    {
                        _riskDict[vm.BaseContract] = baseContractList.FindIndex(s => s == vm.BaseContract);
                    }
                }
                // set x-axis using strikeList;
                lock (BarItemCollection)
                {
                    BarItemCollection.Clear();

                    for (int i = 0; i < baseContractList.Count; i++)
                    {
                        BarItemCollection.Add(new ColumnItem { CategoryIndex = i, Value = 0 });
                    }
                }

                columnSeries.ItemsSource = BarItemCollection;

                _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
            }
        }

        private void exCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
                foreach (var vm in strategyVMList)
                {
                    if (vm.BaseContract != null)
                        _riskSet.Add(vm.BaseContract);
                }
            }
        }

        private void exCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
                foreach (var vm in strategyVMList)
                {
                    if (vm.BaseContract != null)
                        _riskSet.Remove(vm.BaseContract);
                }
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Enter)
                {

                    //StrategyVM strategyVM = ctrl.Tag as StrategyVM;
                    //if (strategyVM != null)
                    //{
                    //    if (e.Key == Key.Enter)
                    //        strategyVM.UpdateStrategy();
                    //    else
                    //    {
                    //        ctrl.DataContext = null;
                    //        ctrl.DataContext = strategyVM;
                    //    }
                    //}
                    //ctrl.Background = Brushes.White;
                }
                else
                {
                    //ctrl.Background = Brushes.MistyRose;
                }
            }
        }
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            plotModel.ResetAllAxes();
        }

        private void marketRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void settlementRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void valuationRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
