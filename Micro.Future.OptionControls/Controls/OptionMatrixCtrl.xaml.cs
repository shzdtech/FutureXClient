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
    public partial class OptionMatrixCtrl : UserControl

    {
        //private Timer _timer;
        //public bool PositionUpdated
        //{
        //    get;
        //    set;
        //}

        private IDictionary<string, int> _riskDict = new Dictionary<string, int>();

        private HashSet<string> _riskSet = new HashSet<string>();

        //private int UpdateInterval = 1000;

        public QueryValuation Queryvaluation
        {
            get;
            set;
        }
        public double SelectPrice
        {
            get;
            set;
        }
        public string SelectContract
        {
            get;
            set;
        }
        public double TheoPrice
        {
            get;
            set;
        }
        public class RiskSet
        {
            public double Gamma
            {
                get;
                set;
            }
            public double Delta
            {
                get;
                set;
            }
            public double Theta
            {
                get;
                set;
            }
            public double Vega
            {
                get;
                set;
            }
            public double Rho
            {
                get;
                set;
            }
            public double PnL
            {
                get;
                set;
            }
            public double Position
            {
                get;
                set;
            }
            public double Price
            {
                get;
                set;
            }
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
        public MarketDataVM MarketData
        {
            get;
            set;
        }
        public int PriceCnt
        {
            get;
            set;
        }
        public double PriceSize
        {
            get;
            set;
        }
        public int VolCnt
        {
            get;
            set;
        }
        public double VolSize
        {
            get;
            set;
        }
        public double Interest
        {
            get;
            set;
        }
        public int Expiration
        {
            get;
            set;
        }
        public double Delta
        {
            get;
            set;
        }
        public double Gamma
        {
            get;
            set;
        }
        public double Vega
        {
            get;
            set;
        }
        public double Theta
        {
            get;
            set;
        }
        public double Rho
        {
            get;
            set;
        }
        public double TableValuation
        {
            get;
            set;
        }
        public double TableVol
        {
            get;
            set;
        }
        public double TablePnL
        {
            get;
            set;
        }
        private TraderExHandler _tradeExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private MarketDataHandler _marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
        private AbstractOTCHandler _abstractOTCHandler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();

        public void ReloadDataCallback()
        {
            Dispatcher.Invoke(() =>
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                //if (priceCntIUP.Value != null && priceSizeIUP.Value != null && volCntIUP.Value != null && volSizeIUP.Value != null)
                //{
                //    int volCount = 2 * VolCnt + 2;
                //    int priceCount = 2 * PriceCnt + 2;
                //    if (riskMatrixTable.RowGroups.Count != 0)
                //    {
                //        for (int x = 1; x < riskMatrixTable.RowGroups[0].Rows.Count; x++)
                //        {
                //            TableRow currentRow = riskMatrixTable.RowGroups[0].Rows[x];
                //            for (int y = 1; y < riskMatrixTable.Columns.Count; y++)
                //            {
                //                currentRow.Cells[y].Blocks.Clear();
                //                string msg = string.Format("D:\n G:\n V:\n T:\n R: \nPnL:");
                //                currentRow.Cells[y].Blocks.Add(new Paragraph(new Run(msg)));
                //            }
                //        }
                //    }
                //}
                RiskIndex(portfolio);
            });
        }
        private async Task<RiskSet> MakeRisk(int x, int y)
        {
            var queryvaluation = new QueryValuation();
            var queryvaluationzero = new QueryValuation();
            var portfolio = portfolioCB.SelectedValue?.ToString();
            var positions = _tradeExHandler.PositionVMCollection.Where(p => p.Portfolio == portfolio);
            var riskset = new RiskSet();

            queryvaluation.Interest = interestUP.Value;
            queryvaluation.DaysRemain = expIUP.Value;

            foreach (var item in expirationLV.ItemsSource)
            {
                var strategyvm = item as StrategyBaseVM;
                double price = 0;
                //if (!strategyvm.Selected)
                //{
                //    selectedWrapPanel.Children.Clear();
                //}
                if (strategyvm.Selected)
                {
                    //_contractList.Add(strategyvm.MktVM.Contract);
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
                    var tableValuation = price - priceCntIUP.Value * priceSizeIUP.Value + (y - 1) * priceSizeIUP.Value;
                    var tableVol = 0 + volCntIUP.Value * volSizeIUP.Value - (x - 1) * volSizeIUP.Value;
                    TableValuation = (double)tableValuation;
                    TableVol = (double)tableVol;
                    //if (variateRadioButton.IsChecked.Value)
                    //{
                    //    queryvaluationzero.ContractParams[strategyvm.Contract] = new ValuationParam { Price = price, Volatitly = 0 };
                    //}
                    queryvaluationzero.ContractParams[strategyvm.Contract] = new ValuationParam { Price = price, Volatitly = 0 };
                    queryvaluation.ContractParams[strategyvm.Contract] = new ValuationParam { Price = (double)tableValuation, Volatitly = (double)tableVol };

                    //foreach (var vm in positions)
                    //{
                    //    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    //    string basecontract = null;
                    //    string contract = null;
                    //    if (contractinfo != null)
                    //    {
                    //        if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                    //        {
                    //            basecontract = contractinfo.UnderlyingContract;
                    //            contract = contractinfo.Contract;
                    //        }
                    //        else
                    //            basecontract = contractinfo.Contract;
                    //    }
                    //    if (_riskSet.Contains(basecontract))
                    //    {
                    //        if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                    //     || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION))
                    //        {
                    //            var theoprice = _theoPriceList.Where(c => c.Contract == contract).Select(c => c.MidPrice).FirstOrDefault();
                    //            var theotableValuation = theoprice - priceCntIUP.Value * priceSizeIUP.Value + (y - 1) * priceSizeIUP.Value;
                    //            var theotableVol = 0 + volCntIUP.Value * volSizeIUP.Value - (x - 1) * volSizeIUP.Value;
                    //            var basecontractPosition = positions.Where(p => p.Contract == basecontract).FirstOrDefault();
                    //            if (basecontractPosition != null)
                    //            {
                    //                if (pnlCheckBox.IsChecked.Value)
                    //                {
                    //                    if (basecontractPosition.Direction == PositionDirectionType.PD_LONG)
                    //                    {
                    //                        basecontractPosition.Profit = ((double)theotableValuation - theoprice) * basecontractPosition.Position * basecontractPosition.Multiplier;
                    //                    }
                    //                    else if (basecontractPosition.Direction == PositionDirectionType.PD_SHORT)
                    //                    {
                    //                        basecontractPosition.Profit = (theoprice - (double)theotableValuation) * basecontractPosition.Position * basecontractPosition.Multiplier;
                    //                    }
                    //                    riskset.PnL += basecontractPosition.Profit;
                    //                }
                    //            }
                    //        }
                    //        if (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE)
                    //        {
                    //            var basecontractPosition = positions.Where(p => p.Contract == basecontract).FirstOrDefault();
                    //            if (basecontractPosition != null)
                    //            {
                    //                if (pnlCheckBox.IsChecked.Value)
                    //                {
                    //                    if (basecontractPosition.Direction == PositionDirectionType.PD_LONG)
                    //                    {
                    //                        basecontractPosition.Profit = ((double)tableValuation - price) * basecontractPosition.Position * basecontractPosition.Multiplier;
                    //                    }
                    //                    else if (basecontractPosition.Direction == PositionDirectionType.PD_SHORT)
                    //                    {
                    //                        basecontractPosition.Profit = (price - (double)tableValuation) * basecontractPosition.Position * basecontractPosition.Multiplier;
                    //                    }
                    //                    riskset.PnL += basecontractPosition.Profit;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            var riskVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
            var riskzeroVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluationzero, portfolio);
            //riskVMlist.Add(new RiskVM { Contract = "m1709", Delta = 0.3, Gamma = 0.2 });

            //if (riskMatrixTable.RowGroups.Count != 0)
            double zeropnl = 0;
            foreach (var vm in riskzeroVMlist)
            {
                var contractinfo = ClientDbContext.FindContract(vm.Contract);
                string basecontract = null;
                string contract = null;
                if (contractinfo != null)
                {
                    if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                    {
                        basecontract = contractinfo.UnderlyingContract;
                        contract = contractinfo.Contract;
                    }
                    else
                    {
                        basecontract = contractinfo.Contract;
                        contract = contractinfo.Contract;
                    }
                    var contractPosition = positions.Where(p => p.Contract == contract).FirstOrDefault();


                    if (_riskSet.Contains(basecontract))
                    {
                        //var contractinfo = ClientDbContext.FindContract(vm.Contract);

                        if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                    || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION)
                    || (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE))
                        {
                            if (pnlCheckBox.IsChecked.Value)
                            {
                                if (contractPosition != null)
                                {
                                    if (contractPosition.Direction == PositionDirectionType.PD_LONG)
                                    {
                                        zeropnl += vm.Price * contractPosition.Multiplier * contractPosition.Position;
                                    }
                                    else if (contractPosition.Direction == PositionDirectionType.PD_SHORT)
                                    {
                                        zeropnl -= vm.Price * contractPosition.Multiplier * contractPosition.Position;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var vm in riskVMlist)
            {
                var contractinfo = ClientDbContext.FindContract(vm.Contract);
                string basecontract = null;
                string contract = null;
                if (contractinfo != null)
                {
                    if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                    {
                        basecontract = contractinfo.UnderlyingContract;
                        contract = contractinfo.Contract;
                    }
                    else
                    {
                        basecontract = contractinfo.Contract;
                        contract = contractinfo.Contract;
                    }
                    var contractPosition = positions.Where(p => p.Contract == contract).FirstOrDefault();


                    if (_riskSet.Contains(basecontract))
                    {
                        //var contractinfo = ClientDbContext.FindContract(vm.Contract);

                        if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                    || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION)
                    || (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE))
                        {
                            if (deltaCheckBox.IsChecked.Value)
                                riskset.Delta += vm.Delta;
                            if (gammaCheckBox.IsChecked.Value)
                                riskset.Gamma += vm.Gamma;
                            if (vegaCheckBox.IsChecked.Value)
                                riskset.Vega += vm.Vega100;
                            if (thetaCheckBox.IsChecked.Value)
                                riskset.Theta += vm.Theta365;
                            if (rhoCheckBox.IsChecked.Value)
                                riskset.Rho += vm.Rho100;
                            if (pnlCheckBox.IsChecked.Value)
                            {
                                if (contractPosition != null)
                                {
                                    if (contractPosition.Direction == PositionDirectionType.PD_LONG)
                                    {
                                        riskset.PnL += vm.Price * contractPosition.Multiplier * contractPosition.Position;
                                    }
                                    else if (contractPosition.Direction == PositionDirectionType.PD_SHORT)
                                    {
                                        riskset.PnL -= vm.Price * contractPosition.Multiplier * contractPosition.Position;
                                    }
                                }
                            }
                            //Logger.Debug(vm.Price.ToString());
                        }
                    }
                }
            }

            riskset.PnL -= zeropnl;

            return riskset;
        }
        private async void RiskIndex(string portfolio)
        {
            if (string.IsNullOrEmpty(portfolio))
                return;
            var positions = _tradeExHandler.PositionVMCollection.Where(p => p.Portfolio == portfolio);
            var queryvaluation = new QueryValuation();
            var queryvaluationzero = new QueryValuation();
            selectedWrapPanel.Children.Clear();
            if (expirationLV.ItemsSource != null)
            {
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
                    }
                }
            }

            queryvaluation.Interest = interestUP.Value;
            queryvaluation.DaysRemain = expIUP.Value;

            if (priceCntIUP.Value != null && priceSizeIUP.Value != null && volCntIUP.Value != null && volSizeIUP.Value != null)
            {
                int volCount = 2 * VolCnt + 2;
                int priceCount = 2 * PriceCnt + 2;
                if (riskMatrixTable.RowGroups.Count != 0)
                {
                    for (int x = 1; x < riskMatrixTable.RowGroups[0].Rows.Count; x++)
                    {
                        TableRow currentRow = riskMatrixTable.RowGroups[0].Rows[x];
                        for (int y = 1; y < riskMatrixTable.Columns.Count; y++)
                        {
                            //currentRow.Cells[y].Blocks.Clear();
                            if (variateRadioButton.IsChecked.Value)
                            {
                                var risksetzero = await MakeRisk(1 + VolCnt, 1 + PriceCnt);
                                var riskset = await MakeRisk(x, y);
                                string msg = string.Format("Δ:{0:N2}\n Γ:{1:N4}\n V:{2:N2}\n Θ:{3:N2}\n Ρ:{4:N2}\nPnL:{5:N2}", riskset.Delta - risksetzero.Delta, riskset.Gamma - risksetzero.Gamma, riskset.Vega - risksetzero.Vega, riskset.Theta - risksetzero.Theta, riskset.Rho - risksetzero.Rho, riskset.PnL);
                                var firstblock = currentRow.Cells[y].Blocks.FirstBlock as Paragraph;
                                var firstrun = firstblock.Inlines.FirstInline as Run;
                                firstrun.Text = msg;
                            }
                            else
                            {
                                var riskset = await MakeRisk(x, y);
                                string msg = string.Format("Δ:{0:N2}\n Γ:{1:N4}\n V:{2:N2}\n Θ:{3:N2}\n Ρ:{4:N2}\nPnL:{5:N2}", riskset.Delta, riskset.Gamma, riskset.Vega, riskset.Theta, riskset.Rho, riskset.PnL);
                                //currentRow.Cells[y].Blocks.Add(new Paragraph(new Run(msg)));
                                var firstblock = currentRow.Cells[y].Blocks.FirstBlock as Paragraph;
                                var firstrun = firstblock.Inlines.FirstInline as Run;
                                firstrun.Text = msg;
                            }

                        }
                    }
                }
            }
        }

        public OptionMatrixCtrl()
        {
            InitializeComponent();
            //_tradeExHandler.OnPositionUpdated += OnPositionUpdated;
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
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
                deltaCheckBox.IsChecked = true;
                marketRadioButton.IsChecked = true;
                absoluteRadioButton.IsChecked = true;
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
                        vm.MktVM = await _marketdataHandler.SubMarketDataAsync(vm.Contract);
                    }
                }

                expirationLV.ItemsSource = strategyContractList;


                //var strikeSet = new SortedSet<double>();
                //foreach (var vm in strategyVMList)
                //{
                //    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                //    if (contractinfo != null)
                //    {
                //        strikeSet.Add(contractinfo.StrikePrice);
                //    }
                //}

                //var strikeList = strikeSet.ToList();
                //strikeAxis.ItemsSource = strikeList;
                //foreach (var vm in strategyVMList)
                //{
                //    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                //    if (contractinfo != null)
                //    {
                //        _riskDict[contractinfo.Contract] = strikeList.FindIndex(s => s == contractinfo.StrikePrice);
                //    }
                //}
                //// set x-axis using strikeList;
                //lock (BarItemCollection)
                //{
                //    BarItemCollection.Clear();

                //    for (int i = 0; i < strikeList.Count; i++)
                //    {
                //        BarItemCollection.Add(new ColumnItem { CategoryIndex = i, Value = 0 });
                //    }
                //}

                //columnSeries.ItemsSource = BarItemCollection;
                //_timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
                ReloadDataCallback();

            }
        }
        private void AddSelectContractMsg(string basecontract, double price)
        {

            string msg = string.Format("  Contract: {0}  Price: {1:N}  ", basecontract, price);
            selectedWrapPanel.Children.Add(new Label { Content = msg });

        }
        private async void exCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var basecontract = strategyBaseVM.Contract;
                _riskSet.Add(basecontract);
                //if (marketRadioButton.IsChecked.Value)
                //{
                //    string msg = string.Format("  {0}: {1:N2}", basecontract, (strategyBaseVM.MktVM.AskPrice + strategyBaseVM.MktVM.BidPrice) / 2);
                //    selectedWrapPanel.Children.Add(new Label { Content = msg });
                //}
                //else if (settlementRadioButton.IsChecked.Value)
                //{
                //    string msg = string.Format("  {0}: {1:N2}", basecontract, strategyBaseVM.MktVM.PreSettlePrice);
                //    selectedWrapPanel.Children.Add(new Label { Content = msg });
                //}
                //else if (valuationRadioButton.IsChecked.Value)
                //{
                //    string msg = string.Format("  {0}: {1:N2}", basecontract, strategyBaseVM.Valuation);
                //    selectedWrapPanel.Children.Add(new Label { Content = msg });
                //}
                MarketData = await _marketdataHandler.SubMarketDataAsync(basecontract);
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
                var basecontract = strategyBaseVM.Contract;
                _riskSet.Remove(basecontract);
                if (marketRadioButton.IsChecked.Value)
                {
                    string msg = string.Format("{0}", basecontract);
                    //selectedWrapPanel.Children.Contains(UIElement.);
                }
                else if (settlementRadioButton.IsChecked.Value)
                {
                    //SelectedContractVMCollection.Remove(new SelectedContractVM { Contract = basecontract, Price = strategyBaseVM.MktVM.PreSettlePrice });
                }
                else if (valuationRadioButton.IsChecked.Value)
                {
                    //SelectedContractVMCollection.Remove(new SelectedContractVM { Contract = basecontract, Price = strategyBaseVM.Valuation });
                }
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == basecontract);
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

                    StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;

                    if (strategyBaseVM != null)
                    {
                        ReloadDataCallback();
                    }
                }
            }
        }
        private void IntSpinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        private void priceCntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                //riskMatrixTable.Columns.Clear();
                //int priceCount = 2 * (int)e.NewValue + 1;
                //PriceCnt = priceCount;
                //for (int x = 0; x < priceCount; x++)
                //{
                //    riskMatrixTable.Columns.Add(new TableColumn());

                //}
                PriceCnt = (int)e.OldValue;
                PriceCnt = (int)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
                ReloadDataCallback();
            }
        }
        private void priceSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                PriceSize = (double)e.OldValue;
                PriceSize = (double)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
                ReloadDataCallback();

            }
        }
        private void volCntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                VolCnt = (int)e.OldValue;
                VolCnt = (int)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
                ReloadDataCallback();

            }
        }
        private void volSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                VolSize = (double)e.OldValue;
                VolSize = (double)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
                ReloadDataCallback();

            }
        }
        private void expirationValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                Expiration = (int)e.NewValue;
                ReloadDataCallback();

            }
        }
        private void interestValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                Interest = (int)e.NewValue;
                ReloadDataCallback();

            }
        }
        public void makeTable(int row, double rowsize, int column, double columnsize)
        {
            if (column != 0 && columnsize != 0 && row != 0 && rowsize != 0)
            {
                riskMatrixTable.Columns.Clear();
                riskMatrixTable.RowGroups.Clear();
                int priceCount = 2 * column + 2;
                PriceCnt = column;
                for (int x = 0; x < priceCount; x++)
                {
                    riskMatrixTable.Columns.Add(new TableColumn());
                }
                int volCount = 2 * row + 2;
                double vol = row * rowsize;
                riskMatrixTable.RowGroups.Add(new TableRowGroup());
                for (int x = 0; x < volCount; x++)
                {
                    riskMatrixTable.RowGroups[0].Rows.Add(new TableRow());
                    //TableRow currentRow0 = riskMatrixTable.RowGroups[0].Rows[0];
                    if (x == 0)
                    {
                        addRowHeader(PriceCnt, columnsize);
                        //currentRow0.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                    }
                    else
                    {
                        TableRow currentRow = riskMatrixTable.RowGroups[0].Rows[x];
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(vol.ToString()))));
                        currentRow.Cells[0].BorderThickness = new Thickness(1.0);
                        currentRow.Cells[0].BorderBrush = new SolidColorBrush(Color.FromRgb(192, 192, 192));
                        vol = vol - rowsize;
                        for (int y = 1; y < (2 * column + 2); y++)
                        {
                            currentRow.Cells.Add(new TableCell(new Paragraph(new Run())));
                            currentRow.Cells[y].BorderThickness = new Thickness(1.0);
                            currentRow.Cells[y].BorderBrush = new SolidColorBrush(Color.FromRgb(192, 192, 192));
                        }
                    }
                }
            }
        }
        private void addRowHeader(int columnCnt, double columnsize)
        {
            TableRow currentRow = riskMatrixTable.RowGroups[0].Rows[0];
            if ((2 * columnCnt + 2) != 0)
            {
                double price = 0 - columnCnt * columnsize;
                for (int y = 0; y < (2 * columnCnt + 2); y++)
                {
                    if (y == 0)
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        currentRow.Cells[y].BorderThickness = new Thickness(1.0);
                        currentRow.Cells[y].BorderBrush = new SolidColorBrush(Color.FromRgb(192, 192, 192));
                    }
                    else
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(price.ToString()))));
                        currentRow.Cells[y].BorderThickness = new Thickness(1.0);
                        currentRow.Cells[y].BorderBrush = new SolidColorBrush(Color.FromRgb(192, 192, 192));
                        price = price + columnsize;
                    }
                }
            }
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

        public void resetButton_Click(object sender, RoutedEventArgs e)
        {
            makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
            ReloadDataCallback();
        }

        private void deltaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void deltaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void gammaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void gammaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void vegaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void vegaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void thetaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void thetaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void rhoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void rhoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void pnlCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void pnlCheckBox_Unchecked(object sender, RoutedEventArgs e)
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

        private void futureCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void futureCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void absoluteRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

        private void variateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadDataCallback();
        }

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
