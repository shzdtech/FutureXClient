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
    public partial class OptionMatrixCtrl : UserControl

    {
        private IDictionary<string, int> _riskDict = new Dictionary<string, int>();
        public List<ColumnItem> BarItemCollection
        {
            get;
        } = new List<ColumnItem>();

        private HashSet<string> _riskSet = new HashSet<string>();

        private Timer _timer;
        private const int UpdateInterval = 1000;

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
        private void ReloadDataCallback(object state)
        {
            var portfolio = portfolioCB.SelectedValue?.ToString();
            if (priceCntIUP != null && priceSizeIUP != null && volCntIUP != null && volSizeIUP != null)
            {

            }

        }
        public OptionMatrixCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
        }

        private void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

                _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
            }
        }


        private async void exCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                StrategyBaseVM strategyBaseVM = ctrl.DataContext as StrategyBaseVM;
                var basecontract = strategyBaseVM.Contract;
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
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyVMList = strategyVMCollection.Where(s => s.BaseContract == strategyBaseVM.Contract);
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
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            plotModel.ResetAllAxes();
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
                PriceCnt = (int)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
            }
        }
        private void priceSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                PriceSize = (double)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
            }
        }
        private void volCntValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                VolCnt = (int)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
            }
        }
        private void volSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                VolSize = (double)e.NewValue;
                makeTable(VolCnt, VolSize, PriceCnt, PriceSize);
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
                double vol = 0 - row * rowsize;
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
                        vol = vol + rowsize;
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
                    }
                    else
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run(price.ToString()))));
                        price = price + columnsize;
                    }
                }
            }
        }
    }
}
