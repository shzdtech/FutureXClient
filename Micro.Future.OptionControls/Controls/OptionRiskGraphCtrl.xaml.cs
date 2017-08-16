using Micro.Future.CustomizedControls.Controls;
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

        private TraderExHandler _tradeExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private MarketDataHandler _marketDataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();


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
                var riskVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
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
            portfolioCB.ItemsSource = portfolioVMCollection;
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
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
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                deltaRadioButton.IsChecked = true;
                marketRadioButton.IsChecked = true;
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var riskVMCollection = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                var riskVMList = riskVMCollection.Select(s => s.Contract).Distinct();
                var positionList = _tradeExHandler?.PositionVMCollection.Where(s => s.Portfolio == portfolio).Select(s => s.Contract).Distinct();
                positionList = positionList.Intersect(_futurecontractList.Select(c => c.Contract));
                var portfolioVM = _otcOptionHandler?.PortfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                var hedgeContractList = portfolioVM.HedgeContractParams.Select(c => c.Contract).Distinct().ToList();
                var strategyContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract))
                                    .GroupBy(s => s.BaseContract).Select(c => new StrategyBaseVM { Contract = c.First().BaseContract, OptionContract = c.First().Contract }).ToList();

                var strategyVMList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract)).ToList();

                var unshowContracts = positionList.Except(strategyContractList.Select(s => s.Contract));
                var unshowRiskContracts = hedgeContractList.Except(strategyContractList.Select(s => s.Contract));

                strategyContractList.AddRange(unshowContracts.Select(c => new StrategyBaseVM { Contract = c }));

                foreach (var vm in strategyContractList)
                {
                    if (vm.OptionContract != null)
                    {
                        var contractinfo = ClientDbContext.FindContract(vm.OptionContract);
                        if (contractinfo != null)
                        {
                            vm.Expiration = contractinfo.ExpireDate;
                            vm.MktVM = await _marketDataHandler.SubMarketDataAsync(vm.Contract);
                        }
                    }
                    if (vm.Contract != null)
                    {
                        var futurecontractinfo = ClientDbContext.FindContract(vm.Contract);
                        if (futurecontractinfo != null)
                        {
                            vm.FutureExpiration = futurecontractinfo.ExpireDate;
                        }
                    }
                }

                expirationLV.ItemsSource = strategyContractList;


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
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
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
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
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
