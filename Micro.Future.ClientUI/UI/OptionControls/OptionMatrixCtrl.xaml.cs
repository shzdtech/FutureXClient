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
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionMatrixCtrl : UserControl

    {
        private IDictionary<string, int> _riskDict = new Dictionary<string, int>();


        private HashSet<string> _riskSet = new HashSet<string>();

        private Timer _timer;
        private const int UpdateInterval = 2147483647;

        public QueryValuation Queryvaluation
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

        //private void ReloadDataCallback(object state)
        //{
        //    Dispatcher.Invoke(async () =>
        //    {
        //        var portfolio = portfolioCB.SelectedValue?.ToString();
        //        var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
        //        lock (BarItemCollection)
        //        {
        //            foreach (var baritem in BarItemCollection)
        //            {
        //                baritem.Value = 0;
        //            }
        //            foreach (var vm in riskVMlist)
        //            {

        //                if (_riskSet.Contains(vm.Contract))
        //                {
        //                    var contractinfo = ClientDbContext.FindContract(vm.Contract);

        //                    if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
        //                || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION))

        //                    {
        //                        int index;

        //                        if (_riskDict.TryGetValue(vm.Contract, out index))
        //                        {

        //                            var barItem = BarItemCollection[index];
        //                            if (deltaCheckBox.IsChecked.Value)
        //                                barItem.Value += vm.Delta;
        //                            else if (gammaCheckBox.IsChecked.Value)
        //                                barItem.Value += vm.Gamma;
        //                            else if (vegaCheckBox.IsChecked.Value)
        //                                barItem.Value += vm.Vega100;
        //                            else if (thetaCheckBox.IsChecked.Value)
        //                                barItem.Value += vm.Theta365;
        //                            else if (rhoCheckBox.IsChecked.Value)
        //                                barItem.Value += vm.Rho100;
        //                        }
        //                    }
        //                }

        //                plotModel.InvalidatePlot(true);
        //            }
        //        }
        //    });
        //}
        private void ReloadDataCallback()
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
            if (expIUP.Value != null && interestUP.Value != null)
            {
                queryvaluation.Interest = interestUP.Value;
                queryvaluation.DaysRemain = expIUP.Value;
            }
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
                    var tableValuation = price - priceCntIUP.Value * priceSizeIUP.Value + (y - 1) * priceSizeIUP.Value;
                    var tableVol = 0 + volCntIUP.Value * volSizeIUP.Value - (x - 1) * volSizeIUP.Value;
                    TableValuation = (double)tableValuation;
                    TableVol = (double)tableVol;
                    //if (variateRadioButton.IsChecked.Value)
                    //{
                    //    queryvaluationzero.ContractParams[strategyvm.Contract] = new ValuationParam { Price = price, Volatitly = 0 };
                    //}
                    queryvaluation.ContractParams[strategyvm.Contract] = new ValuationParam { Price = (double)tableValuation, Volatitly = (double)tableVol };

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
                        }
                        if (_riskSet.Contains(basecontract))
                        {
                            if ((callCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION)
                         || (putCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION)
                         || (futureCheckBox.IsChecked.Value && contractinfo.ContractType == (int)ContractType.CONTRACTTYPE_FUTURE))
                            {
                                var basecontractPosition = positions.Where(p => p.Contract == basecontract).FirstOrDefault();
                                if (basecontractPosition != null)
                                {
                                    if (pnlCheckBox.IsChecked.Value)
                                    {
                                        if (basecontractPosition.Direction == PositionDirectionType.PD_LONG)
                                        {
                                            basecontractPosition.Profit = ((double)tableValuation - price) * basecontractPosition.Position * basecontractPosition.Multiplier;
                                        }
                                        else if (basecontractPosition.Direction == PositionDirectionType.PD_SHORT)
                                        {
                                            basecontractPosition.Profit = (price - (double)tableValuation) * basecontractPosition.Position * basecontractPosition.Multiplier;
                                        }
                                        riskset.PnL += basecontractPosition.Profit;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var riskVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluation, portfolio);
            var riskzeroVMlist = await _otcOptionTradeHandler.QueryValuationRiskAsync(queryvaluationzero, portfolio);
            //riskVMlist.Add(new RiskVM { Contract = "m1709", Delta = 0.3, Gamma = 0.2 });

            //if (riskMatrixTable.RowGroups.Count != 0)
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
                }

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
                        //else if (pnlRadioButton.IsChecked.Value)
                        //    barItem.Value += basecontractPosition.Profit;
                    }
                }
            }
            return riskset;
        }
        private async void RiskIndex(string portfolio)
        {
            if (string.IsNullOrEmpty(portfolio))
                return;
            var positions = _tradeExHandler.PositionVMCollection.Where(p => p.Portfolio == portfolio);
            var queryvaluation = new QueryValuation();
            var queryvaluationzero = new QueryValuation();
            if (expIUP.Value != null && interestUP.Value != null)
            {
                queryvaluation.Interest = interestUP.Value;
                queryvaluation.DaysRemain = expIUP.Value;
            }
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
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;

        }

        private async void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                deltaCheckBox.IsChecked = true;
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
                        vm.MktVM = await _marketdataHandler.SubMarketDataAsync(vm.Contract);
                    }
                }
                var mktList = await _marketdataHandler.SubMarketDataAsync(strategyVMList);
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

        private async void exCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var basecontract = strategyBaseVM.Contract;
                _riskSet.Add(basecontract);
                MarketData = await _marketdataHandler.SubMarketDataAsync(basecontract);

                //var lastprice = mktVM.LastPrice;

                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo.ExpireDate == strategyBaseVM.Expiration)
                        _riskSet.Add(vm.Contract);
                }
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
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == basecontract);
                foreach (var vm in strategyVMList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo.ExpireDate == strategyBaseVM.Expiration)
                        _riskSet.Remove(vm.Contract);
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

                    StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;

                    if (strategyBaseVM != null)
                    {

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
            }
        }
        private void expirationValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                Expiration = (int)e.NewValue;
            }
        }
        private void interestValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                Interest = (int)e.NewValue;
            }
        }
        private void makeTable(int row, double rowsize, int column, double columnsize)
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

        }

        private void settlementRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void valuationRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
            ReloadDataCallback();
        }
    }
}
