using Micro.Future.Message;
using Micro.Future.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using System.Threading;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VolCurvCtrl : UserControl
    {
        private IList<ContractInfo> _contractList;
        private AbstractOTCHandler _abstractOTCHandler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
        private OTCOptionHandler _otcHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

        private List<KeyValuePair<ContractKeyVM, double>> _optionCallVMList = new List<KeyValuePair<ContractKeyVM, double>>();
        private List<KeyValuePair<ContractKeyVM, double>> _optionPutVMList = new List<KeyValuePair<ContractKeyVM, double>>();
        private List<KeyValuePair<ContractKeyVM, double>> _optionTheoCallVMList = new List<KeyValuePair<ContractKeyVM, double>>();
        private List<KeyValuePair<ContractKeyVM, double>> _optionTheoPutVMList = new List<KeyValuePair<ContractKeyVM, double>>();
        private const int ScatterSize = 10;
        public VolCurvCtrl()
        {
            InitializeComponent();
            Initialize();
        }

        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();
        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection1
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();


        public VolatilityLinesVM VolatilityModelVM { get; } = new VolatilityLinesVM();
        public VolatilityLinesVM VolatilityModelVM1 { get; } = new VolatilityLinesVM();


        public void Initialize()
        {
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();            //VegaPosition.Model = _otcHandler.OptionOxyVM.PlotModelBar;
            volPlot.DataContext = VolatilityModelVM;
            //theoAskLS1.ItemsSource = VolatilityModelVM1.TheoAskVolLine;
            //theoBidLS1.ItemsSource = VolatilityModelVM1.TheoBidVolLine;
            //theoMidLS1.ItemsSource = VolatilityModelVM1.TheoMidVolLine;
            theoPutAskSC.MarkerOutline = CustomOxyMarkers.LUTriangle;
            theoCallAskSC.MarkerOutline = CustomOxyMarkers.RUTriangle;
            theoPutBidSC.MarkerOutline = CustomOxyMarkers.LDTriangle;
            theoCallBidSC.MarkerOutline = CustomOxyMarkers.RDTriangle;

            var internalBidSS = theoPutBidSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalBidSS.MouseDown += PutBidScatter_MouseDown;
            var internalAskSS = theoPutAskSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalAskSS.MouseDown += PutAskScatter_MouseDown;

            internalBidSS = theoCallBidSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalBidSS.MouseDown += CallBidScatter_MouseDown;
            internalAskSS = theoCallAskSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalAskSS.MouseDown += CallAskScatter_MouseDown;

            _otcHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;
            _otcHandler.OnStrategyUpdated += _otcHandler_OnStrategyUpdated;
        }
        private void _otcHandler_OnStrategyUpdated(StrategyVM strategyVM)
        {
            var contract = strategyVM.Contract;
            var exchange = strategyVM.Exchange;
            var pointPA = VolatilityModelVM.TheoPutAskVolScatter.
                FirstOrDefault(c => ((StrategyVM)c.Tag).Contract == contract && ((StrategyVM)c.Tag).Exchange == exchange);
            var pointPB = VolatilityModelVM.TheoPutBidVolScatter.
                FirstOrDefault(c => ((StrategyVM)c.Tag).Contract == contract && ((StrategyVM)c.Tag).Exchange == exchange);
            var pointCA = VolatilityModelVM.TheoCallAskVolScatter.
                FirstOrDefault(c => ((StrategyVM)c.Tag).Contract == contract && ((StrategyVM)c.Tag).Exchange == exchange);
            var pointCB = VolatilityModelVM.TheoCallBidVolScatter.
                FirstOrDefault(c => ((StrategyVM)c.Tag).Contract == contract && ((StrategyVM)c.Tag).Exchange == exchange);
            if (pointPA != null)
                pointPA.Value = strategyVM.AskEnabled ? 1 : 0;
            if (pointPB != null)
                pointPB.Value = strategyVM.BidEnabled ? 1 : 0;
            if (pointCA != null)
                pointCA.Value = strategyVM.AskEnabled ? 1 : 0;
            if (pointCB != null)
                pointCB.Value = strategyVM.BidEnabled ? 1 : 0;
        }
        //private void OnPutBidStatus(string contract, bool status)
        //{
        //    var point = contract as ScatterPoint;
        //    if (point != null)
        //    {
        //        point.Value = status ? 1 : 0;
        //    }
        //}
        private void PutBidScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var point = e.HitTestResult.Item as ScatterPoint;
            if (point != null)
            {
                if (e.Position.X < e.HitTestResult.NearestHitPoint.X && e.Position.DistanceTo(e.HitTestResult.NearestHitPoint) < point.Size)
                {
                    var plot = e.HitTestResult.Element as PlotElement;
                    if (plot != null)
                    {
                        var strategyVM = point.Tag as StrategyVM;
                        if (strategyVM != null)
                        {
                            strategyVM.BidEnabled = !strategyVM.BidEnabled;
                            point.Value = strategyVM.BidEnabled ? 1 : 0;
                            plot.PlotModel.InvalidatePlot(true);
                        }
                    }
                }
            }
        }

        private void PutAskScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var point = e.HitTestResult.Item as ScatterPoint;
            if (point != null)
            {
                if (e.Position.X < e.HitTestResult.NearestHitPoint.X && e.Position.DistanceTo(e.HitTestResult.NearestHitPoint) < point.Size)
                {
                    var plot = e.HitTestResult.Element as PlotElement;
                    if (plot != null)
                    {
                        var strategyVM = point.Tag as StrategyVM;
                        if (strategyVM != null)
                        {
                            strategyVM.AskEnabled = !strategyVM.AskEnabled;
                            point.Value = strategyVM.AskEnabled ? 1 : 0;
                            plot.PlotModel.InvalidatePlot(true);
                        }
                    }
                }
            }
        }

        private void CallBidScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var point = e.HitTestResult.Item as ScatterPoint;
            if (point != null)
            {
                if (e.Position.X > e.HitTestResult.NearestHitPoint.X && e.Position.DistanceTo(e.HitTestResult.NearestHitPoint) < point.Size)
                {
                    var plot = e.HitTestResult.Element as PlotElement;
                    if (plot != null)
                    {
                        var strategyVM = point.Tag as StrategyVM;
                        if (strategyVM != null)
                        {
                            strategyVM.BidEnabled = !strategyVM.BidEnabled;
                            point.Value = strategyVM.BidEnabled ? 1 : 0;
                            plot.PlotModel.InvalidatePlot(true);
                        }
                    }
                }
            }
        }

        private void CallAskScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var point = e.HitTestResult.Item as ScatterPoint;
            if (point != null)
            {
                if (e.Position.X > e.HitTestResult.NearestHitPoint.X && e.Position.DistanceTo(e.HitTestResult.NearestHitPoint) < point.Size)
                {
                    var plot = e.HitTestResult.Element as PlotElement;
                    if (plot != null)
                    {
                        var strategyVM = point.Tag as StrategyVM;
                        if (strategyVM != null)
                        {
                            strategyVM.AskEnabled = !strategyVM.AskEnabled;
                            point.Value = strategyVM.AskEnabled ? 1 : 0;
                            plot.PlotModel.InvalidatePlot(true);
                        }
                    }
                }
            }
        }

        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM tdOptionVM)
        {
            int idx;
            if (tdOptionVM.ImpliedVolVM != null)
            {
                idx = _optionPutVMList.FindIndex(c => c.Key.EqualContract(tdOptionVM));
                if (idx >= 0) // Update PutOption
                {
                    double x = _optionPutVMList[idx].Value;
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.AskVol)) VolatilityModelVM.PutAskVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.BidVol)) VolatilityModelVM.PutBidVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.BidVol);
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.MidVol)) VolatilityModelVM.PutMidVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.MidVol);
                }

                idx = _optionCallVMList.FindIndex(c => c.Key.EqualContract(tdOptionVM));
                if (idx >= 0) // Update CallOption
                {
                    double x = _optionCallVMList[idx].Value;
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.AskVol)) VolatilityModelVM.CallAskVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.BidVol)) VolatilityModelVM.CallBidVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.BidVol);
                    if (!double.IsNaN(tdOptionVM.ImpliedVolVM.MidVol)) VolatilityModelVM.CallMidVolLine[idx] = new DataPoint(x, tdOptionVM.ImpliedVolVM.MidVol);
                }
            }

            idx = _optionTheoCallVMList.FindIndex(c => c.Key.EqualContract(tdOptionVM));
            if (idx < 0)
                idx = _optionTheoPutVMList.FindIndex(c => c.Key.EqualContract(tdOptionVM));
            if (idx >= 0)
            {
                double x = _optionTheoCallVMList[idx].Value;
                if(tdOptionVM.TheoDataVM != null)
                { 
                    if (!double.IsNaN(tdOptionVM.TheoDataVM.AskVol))
                    {
                        VolatilityModelVM.TheoAskVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.AskVol);
                        var scatterPt = VolatilityModelVM.TheoPutAskVolScatter[idx];
                        VolatilityModelVM.TheoPutAskVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.AskVol, ScatterSize, scatterPt.Value, scatterPt.Tag);
                        scatterPt = VolatilityModelVM.TheoCallAskVolScatter[idx];
                        VolatilityModelVM.TheoCallAskVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.AskVol, ScatterSize, scatterPt.Value, scatterPt.Tag);
                    }

                    if (!double.IsNaN(tdOptionVM.TheoDataVM.BidVol))
                    {
                        VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.BidVol);
                        var scatterPt = VolatilityModelVM.TheoPutBidVolScatter[idx];
                        VolatilityModelVM.TheoPutBidVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.BidVol, ScatterSize, scatterPt.Value, scatterPt.Tag);
                        scatterPt = VolatilityModelVM.TheoCallBidVolScatter[idx];
                        VolatilityModelVM.TheoCallBidVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.BidVol, ScatterSize, scatterPt.Value, scatterPt.Tag);
                        VolatilityModelVM.TheoMidVolLine[idx] = new DataPoint(x, (tdOptionVM.TheoDataVM.AskVol + tdOptionVM.TheoDataVM.BidVol) / 2);
                    }
                }

                if (tdOptionVM.TempTheoDataVM != null)
                {
                    if (!double.IsNaN(tdOptionVM.TempTheoDataVM.AskVol)) VolatilityModelVM.TheoAskVolLine1[idx] = new DataPoint(x, tdOptionVM.TempTheoDataVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.TempTheoDataVM.BidVol))
                    {
                        VolatilityModelVM.TheoBidVolLine1[idx] = new DataPoint(x, tdOptionVM.TempTheoDataVM.BidVol);
                        VolatilityModelVM.TheoMidVolLine1[idx] = new DataPoint(x, (tdOptionVM.TempTheoDataVM.AskVol + tdOptionVM.TempTheoDataVM.BidVol) / 2);
                    }
                }

                if (tdOptionVM.WingsReturnVM != null)
                {
                    VolatilityModelVM.ATMLine = tdOptionVM.WingsReturnVM.ATMFPrice;
                    VolatilityModelVM.ReferencePriceLine = tdOptionVM.WingsReturnVM.RefPrice;
                    VolatilityModelVM.SynFLine = tdOptionVM.WingsReturnVM.SyncFPrice;
                    VolatilityModelVM.X0Line = tdOptionVM.WingsReturnVM.X0;
                    VolatilityModelVM.X1Line = tdOptionVM.WingsReturnVM.X1;
                    VolatilityModelVM.X2Line = tdOptionVM.WingsReturnVM.X2;
                    VolatilityModelVM.X3Line = tdOptionVM.WingsReturnVM.X3;
                }
            }
        }

        public void SelectOptionImpl(string exchange, string contract, string expiredate)
        {
            var optionList = (from c in _contractList
                              where c.Exchange == exchange && c.UnderlyingContract == contract && c.ExpireDate == expiredate
                              select c).ToList();

            var strikeList = (from o in optionList
                              orderby o.StrikePrice
                              select o.StrikePrice).Distinct().ToList();

            var callList = (from o in optionList
                            where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                            orderby o.StrikePrice
                            select new ContractKeyVM(exchange, o.Contract)).ToList();

            var putList = (from o in optionList
                           where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                           orderby o.StrikePrice
                           select new ContractKeyVM(exchange, o.Contract)).ToList();

            ClearImplPlot();
            CallPutTDOptionVMCollection1.Clear();
            _optionCallVMList.Clear();
            _optionPutVMList.Clear();
            var retList = _otcHandler.MakeCallPutTDOptionData(strikeList, callList, putList);
            foreach (var vm in retList)
            {
                CallPutTDOptionVMCollection1.Add(vm);
                _optionCallVMList.Add(new KeyValuePair<ContractKeyVM, double>(vm.CallOptionVM, vm.StrikePrice));
                _optionPutVMList.Add(new KeyValuePair<ContractKeyVM, double>(vm.PutOptionVM, vm.StrikePrice));
                VolatilityModelVM.CallAskVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.CallBidVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.CallMidVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.PutAskVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.PutBidVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.PutMidVolLine.Add(DataPoint.Undefined);
            }
        }

        public void SelectOption(string exchange, string contract, string expiredate)
        {
            var optionList = (from c in _contractList
                              where c.Exchange == exchange && c.UnderlyingContract == contract && c.ExpireDate == expiredate
                              select c).ToList();

            var strikeList = (from o in optionList
                              orderby o.StrikePrice
                              select o.StrikePrice).Distinct().ToList();

            var callList = (from o in optionList
                            where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                            orderby o.StrikePrice
                            select new ContractKeyVM(exchange, o.Contract)).ToList();

            var putList = (from o in optionList
                           where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                           orderby o.StrikePrice
                           select new ContractKeyVM(exchange, o.Contract)).ToList();

            ClearTheoPlot();
            CallPutTDOptionVMCollection.Clear();
            _optionTheoCallVMList.Clear();
            _optionTheoPutVMList.Clear();
            var retList = _otcHandler.MakeCallPutTDOptionData(strikeList, callList, putList);
            foreach (var vm in retList)
            {
                CallPutTDOptionVMCollection.Add(vm);
                _optionTheoCallVMList.Add(new KeyValuePair<ContractKeyVM, double>(vm.CallOptionVM, vm.StrikePrice));
                _optionTheoPutVMList.Add(new KeyValuePair<ContractKeyVM, double>(vm.PutOptionVM, vm.StrikePrice));
                VolatilityModelVM.TheoAskVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.TheoBidVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.TheoMidVolLine.Add(DataPoint.Undefined);
                VolatilityModelVM.TheoAskVolLine1.Add(DataPoint.Undefined);
                VolatilityModelVM.TheoBidVolLine1.Add(DataPoint.Undefined);
                VolatilityModelVM.TheoMidVolLine1.Add(DataPoint.Undefined);

                double value = (vm.PutStrategyVM != null && vm.PutStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoPutAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, 0, 0, value, vm.PutStrategyVM));
                VolatilityModelVM.TheoPutBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, 0, 0, value, vm.PutStrategyVM));

                value = (vm.CallStrategyVM != null && vm.CallStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoCallAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, 0, 0, value, vm.CallStrategyVM));
                VolatilityModelVM.TheoCallBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, 0, 0, value, vm.CallStrategyVM));
            }
        }

        public void TempCurveReset(string exchange, string contract, string expiredate)
        {
            VolatilityModelVM.TheoAskVolLine1.Clear();
            VolatilityModelVM.TheoBidVolLine1.Clear();
            VolatilityModelVM.TheoMidVolLine1.Clear();
        }

        public void ScatterReset()
        {
            foreach (var vm in CallPutTDOptionVMCollection)
            {
                double value = (vm.PutStrategyVM != null && vm.PutStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoPutAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.PutOptionVM.ImpliedVolVM.AskVol, 10, value, vm.PutStrategyVM));
                VolatilityModelVM.TheoPutBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.PutOptionVM.ImpliedVolVM.BidVol, 10, value, vm.PutStrategyVM));

                value = (vm.CallStrategyVM != null && vm.CallStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoCallAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.ImpliedVolVM.AskVol, 10, value, vm.CallStrategyVM));
                VolatilityModelVM.TheoCallBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.ImpliedVolVM.BidVol, 10, value, vm.CallStrategyVM));
            }
        }

        private void ClearPlot()
        {
            VolatilityModelVM.ClearAll();
        }

        private void ClearImplPlot()
        {
            VolatilityModelVM.CallAskVolLine.Clear();
            VolatilityModelVM.CallBidVolLine.Clear();
            VolatilityModelVM.CallMidVolLine.Clear();
            VolatilityModelVM.PutAskVolLine.Clear();
            VolatilityModelVM.PutBidVolLine.Clear();
            VolatilityModelVM.PutMidVolLine.Clear();
        }

        private void ClearTheoPlot()
        {
            VolatilityModelVM.TheoAskVolLine.Clear();
            VolatilityModelVM.TheoAskVolLine1.Clear();
            VolatilityModelVM.TheoBidVolLine.Clear();
            VolatilityModelVM.TheoBidVolLine1.Clear();
            VolatilityModelVM.TheoMidVolLine.Clear();
            VolatilityModelVM.TheoMidVolLine1.Clear();
            VolatilityModelVM.TheoPutAskVolScatter.Clear();
            VolatilityModelVM.TheoPutBidVolScatter.Clear();
            VolatilityModelVM.TheoCallAskVolScatter.Clear();
            VolatilityModelVM.TheoCallBidVolScatter.Clear();
        }

        public void ClearTempVolLine()
        {
            VolatilityModelVM.TheoMidVolLine1.Clear();
            VolatilityModelVM.TheoAskVolLine1.Clear();
            VolatilityModelVM.TheoBidVolLine1.Clear();
        }
    }
}
