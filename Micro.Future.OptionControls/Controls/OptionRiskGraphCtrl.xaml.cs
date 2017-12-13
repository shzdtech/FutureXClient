﻿using Micro.Future.Business.Handler.Router;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Utility;
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
using Xceed.Wpf.Toolkit;


namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionRiskGraphCtrl : UserControl

    {
        private IList<ContractInfo> _futurecontractList;
        private IDictionary<string, int> _riskDict = new Dictionary<string, int>();
        //private Timer _timer;
        //public bool PositionUpdated
        //{
        //    get;
        //    set;
        //}
        public List<ContractInfo> OptionList
        {
            get;
        } = new List<ContractInfo>();
        public string SelectedContract
        {
            get;
            set;
        }
        public string SelectedOptionContract
        {
            get;
            set;
        }
        public ObservableCollection<PortfolioVM> PortfolioVMCollection
        {
            get;
            set;
        }
        public List<ColumnItem> BarItemCollection
        {
            get;
        } = new List<ColumnItem>();

        private HashSet<string> _riskSet = new HashSet<string>();

        //private int UpdateInterval = 1000;

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
            public string FutureExpiration
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

        private TraderExHandler _ctpoptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private CTPETFTraderHandler _ctpetfTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
        private CTPSTOCKTraderHandler _ctpstockTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCETFTradeHandler _otcETFHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradeHandler>();
        private OTCStockTradeHandler _otcStockHandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private MarketDataHandler _marketDataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
        private OTCETFTradingDeskHandler _otcETFTradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();


        //private void ReloadDataCallback(object state)
        public void ReloadDataCallback()
        {
            Dispatcher.Invoke(async () =>
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                var queryvaluation = new QueryValuation();
                selectedWrapPanel.Children.Clear();
                foreach (var item in expirationLV.ItemsSource)
                {
                    var strategyvm = item as StrategyBaseVM;

                    double price = 0;
                    if (strategyvm.Selected)
                    {
                        if (marketRadioButton.IsChecked.Value)
                        {
                            price = (strategyvm.MktVM.AskPrice + strategyvm.MktVM.BidPrice) / 2;
                        }
                        else if (settlementRadioButton.IsChecked.Value)
                        {
                            price = strategyvm.MktVM.PreSettlePrice;
                        }
                        else if (valuationRadioButton.IsChecked.Value)
                        {
                            price = strategyvm.Valuation;
                        }
                        AddSelectContractMsg(strategyvm.Contract, price);
                        queryvaluation.ContractParams[strategyvm.Contract] = new ValuationParam { Price = price, Volatitly = 0 };

                    }
                }
                var hedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolio).Select(c => c.HedgeContractParams).FirstOrDefault();
                SelectedContract = hedgeVM.Select(c => c.Contract).FirstOrDefault();
                //var _handler = OTCTradeHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                //var riskVMlist = await _handler.QueryValuationRiskAsync(queryvaluation, portfolio);
                var riskVMlist = new ObservableCollection<RiskVM>();
                if (_otcOptionTradeHandler.MessageWrapper.HasSignIn)
                {
                    riskVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
                }
                else if (_otcETFHandler.MessageWrapper.HasSignIn)
                {
                    riskVMlist = await _otcETFHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
                }
                else if (_otcStockHandler.MessageWrapper.HasSignIn)
                {
                    riskVMlist = await _otcStockHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
                }
                lock (BarItemCollection)
                {
                    foreach (var baritem in BarItemCollection)
                    {
                        baritem.Value = 0;
                    }
                    foreach (var vm in riskVMlist)
                    {

                        if (_riskSet.Contains(vm.Contract))
                        {
                            var contractinfo = ClientDbContext.FindContract(vm.Contract);

                            if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                        || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION))

                            {
                                int index;

                                if (_riskDict.TryGetValue(vm.Contract, out index))
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
                                }
                            }
                        }

                        plotModel.InvalidatePlot(true);
                    }
                }
            });
        }

        public OptionRiskGraphCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection;
            var portfolioList = portfolioVMCollection.Where(c => !string.IsNullOrEmpty(c.Name)).Select(c => c.Name).Distinct().ToList();
            portfolioCB.ItemsSource = portfolioList;
            portfolioVMCollection.Union(MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>()?.PortfolioVMCollection);
            portfolioVMCollection.Union(MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>()?.PortfolioVMCollection);
            PortfolioVMCollection = portfolioVMCollection;
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            OptionList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS));
            OptionList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_ETFOPTION));
            //_timer = new Timer(PositionUpdateCallback, null, UpdateInterval, UpdateInterval);
            //_tradeExHandler.OnPositionUpdated += OnPositionUpdated;
            //refreshsSizeIUP.Value = 0;
        }
        //private void PositionUpdateCallback(object state)
        //{

        //    if (PositionUpdated)
        //    {
        //        PositionUpdated = false;
        //        if (portfolioCB.SelectedValue != null)
        //        {
        //            ReloadDataCallback();
        //        }
        //    }


        //}
        //private void OnPositionUpdated(PositionVM vm)
        //{
        //    PositionUpdated = true;
        //}

        private async void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BarItemCollection.Clear();
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                deltaRadioButton.IsChecked = true;
                marketRadioButton.IsChecked = true;
                var hedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                SelectedContract = hedgeVM.Select(c => c.Contract).FirstOrDefault();
                SelectedOptionContract = OptionList.Where(c => c.UnderlyingContract == SelectedContract).Select(c => c.Contract).FirstOrDefault();
                var _tradingdeskhandler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                if (MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>().MessageWrapper.HasSignIn)
                    _tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                else if (MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>().MessageWrapper.HasSignIn)
                    _tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();
                else if (MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>().MessageWrapper.HasSignIn)
                    _tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>();
                var _handler = TradeExHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                var _marketdatashandler = MarketDataHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                var positionList = _handler?.PositionVMCollection.Where(s => s.Portfolio == portfolio).Select(s => s.Contract).Distinct();
                var strategyVMCollection = _tradingdeskhandler?.StrategyVMCollection;
                //var strategyVMCollection = _otcETFTradingDeskHandler?.StrategyVMCollection;
                //var _otctradehandler = OTCTradeHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                //var riskVMCollection = await _otctradehandler.QueryRiskAsync(portfolio);
                var riskVMCollection = new ObservableCollection<RiskVM>();
                if (_otcOptionTradeHandler.MessageWrapper.HasSignIn)
                {
                    riskVMCollection = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                }
                else if (_otcETFHandler.MessageWrapper.HasSignIn)
                {
                    riskVMCollection = await _otcETFHandler.QueryRiskAsync(portfolio);
                }
                else if (_otcStockHandler.MessageWrapper.HasSignIn)
                {
                    riskVMCollection = await _otcStockHandler.QueryRiskAsync(portfolio);
                }

                var riskVMList = riskVMCollection.Select(s => s.Contract).Distinct();
                positionList = positionList.Intersect(_futurecontractList.Select(c => c.Contract));
                //var portfolioVM = _otcETFTradingDeskHandler?.PortfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                var portfolioVM = _tradingdeskhandler?.PortfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                var hedgeContractList = portfolioVM.HedgeContractParams.Select(c => c.Contract).Distinct().ToList();
                var strategyContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract))
                                    .GroupBy(s => s.BaseContract).Select(c => new StrategyBaseVM { Contract = c.First().BaseContract, OptionContract = c.First().Contract }).ToList();
                var strategyPricingContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio)
                    .SelectMany(c => c.PricingContractParams).Select(c => c.Contract).Distinct().ToList();
                List<string> strategyUnderlyingList = new List<string>();
                List<string> strategyUnderlyingContractList = new List<string>();
                if (strategyPricingContractList != null)
                {
                    foreach (var contract in strategyPricingContractList)
                    {
                        strategyUnderlyingList.AddRange(_futurecontractList.Where(c => c.Contract == contract).Select(c => c.ProductID));
                    }
                }
                var UnderlyingList = strategyUnderlyingList.Distinct();
                if (UnderlyingList != null)
                {
                    foreach (var underlying in UnderlyingList)
                    {
                        strategyUnderlyingContractList.AddRange(_futurecontractList.Where(c => c.ProductID == underlying).Select(c => c.Contract));
                    }
                }
                var strategyVMList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract)).ToList();

                var unshowContracts = positionList.Except(strategyContractList.Select(s => s.Contract));
                var unshowRiskContracts = hedgeContractList.Except(strategyContractList.Select(s => s.Contract));
                strategyContractList.AddRange(unshowContracts.Select(c => new StrategyBaseVM { Contract = c }));

                var contractList = strategyContractList.Union(strategyUnderlyingContractList.Select(c => new StrategyBaseVM { Contract = c }));
                contractList = contractList.GroupBy(c => c.Contract).Select(c => c.FirstOrDefault()).ToList();

                foreach (var vm in contractList)
                {
                    if (vm.OptionContract != null)
                    {
                        var contractinfo = ClientDbContext.FindContract(vm.OptionContract);
                        if (contractinfo != null)
                        {
                            vm.Expiration = contractinfo.ExpireDate;
                            vm.MktVM = await _marketdatashandler.SubMarketDataAsync(vm.Contract);
                        }
                    }
                    if (vm.Contract != null)
                    {
                        var futurecontractinfo = ClientDbContext.FindContract(vm.Contract);
                        if (futurecontractinfo != null)
                        {
                            vm.FutureExpiration = futurecontractinfo.ExpireDate;
                            vm.MktVM = await _marketdatashandler.SubMarketDataAsync(vm.Contract);
                        }
                    }
                }

                expirationLV.ItemsSource = contractList;


                var strikeSet = new SortedSet<double>();
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo != null)
                    {
                        strikeSet.Add(contractinfo.StrikePrice);
                    }
                }

                var strikeList = strikeSet.ToList();
                strikeAxis.ItemsSource = strikeList;
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo != null)
                    {
                        _riskDict[contractinfo.Contract] = strikeList.FindIndex(s => s == contractinfo.StrikePrice);
                    }
                }
                // set x-axis using strikeList;
                lock (BarItemCollection)
                {
                    BarItemCollection.Clear();

                    for (int i = 0; i < strikeList.Count; i++)
                    {
                        BarItemCollection.Add(new ColumnItem { CategoryIndex = i, Value = 0 });
                    }
                }

                columnSeries.ItemsSource = BarItemCollection;
                ReloadDataCallback();
                //_timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
            }
        }
        private void AddSelectContractMsg(string basecontract, double price)
        {

            string msg = string.Format("  Contract: {0}  Price: {1:N}  ", basecontract, price);
            selectedWrapPanel.Children.Add(new Label { Content = msg });

        }
        private void exCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var hedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                SelectedContract = hedgeVM.Select(c => c.Contract).FirstOrDefault();
                var _tradingdeskhandler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                var strategyVMCollection = _tradingdeskhandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo.ExpireDate == strategyBaseVM.Expiration)
                        _riskSet.Add(vm.Contract);
                }
                ReloadDataCallback();
            }
        }

        private void exCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var hedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                SelectedContract = hedgeVM.Select(c => c.Contract).FirstOrDefault();
                var _tradingdeskhandler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                var strategyVMCollection = _tradingdeskhandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo.ExpireDate == strategyBaseVM.Expiration)
                        _riskSet.Remove(vm.Contract);
                }
                ReloadDataCallback();
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

        public void resetGraph()
        {
            plotModel.ResetAllAxes();
        }

        public void resetRiskButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
            plotModel.ResetAllAxes();
        }

        private void marketRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void settlementRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void valuationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void deltaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void gammaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void vegaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void thetaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void rhoRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void pnlRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void callCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();

        }

        private void callCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void putCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void putCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }
        private void quoteListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, expirationLV.Items);
            }
        }
        //private void IntSpinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        //{
        //    var updownctrl = sender as IntegerUpDown;
        //    if (updownctrl != null)
        //    {
        //        Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
        //    }
        //}
        //private void refreshsSizeIUP_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    var updownctrl = sender as IntegerUpDown;
        //    if (updownctrl != null && e.OldValue != null && e.NewValue != null)
        //    {
        //        UpdateInterval = (int)e.NewValue * 1000;
        //    }
        //}
    }
}
