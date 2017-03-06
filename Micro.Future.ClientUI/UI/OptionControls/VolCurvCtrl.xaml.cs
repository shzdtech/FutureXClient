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
            if (CallPutTDOptionVMCollection.Update(tdOptionVM) != null)
            {
                var list = CallPutTDOptionVMCollection.ToList();
                int idx = list.FindIndex((pb) => string.Compare(pb.PutOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                double x = 0;
                if (idx >= 0) // Update PutOption
                {
                    x = VolatilityModelVM.PutAskVolLine[idx].X;
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.AskVol)) VolatilityModelVM.PutAskVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.BidVol)) VolatilityModelVM.PutBidVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.BidVol);
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.MidVol)) VolatilityModelVM.PutMidVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.MidVol);
                }
                else
                {
                    idx = list.FindIndex((pb) => string.Compare(pb.CallOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                    if (idx >= 0) // Update CallOption
                    {
                        x = VolatilityModelVM.CallAskVolLine[idx].X;
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.AskVol)) VolatilityModelVM.CallAskVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.AskVol);
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.BidVol)) VolatilityModelVM.CallBidVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.BidVol);
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.MidVol)) VolatilityModelVM.CallMidVolLine[idx] = new DataPoint(x, tdOptionVM.MarketDataVM.MidVol);
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.AskVol)) VolatilityModelVM.TheoAskVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.AskVol);
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.BidVol)) VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.BidVol);
                        if (tdOptionVM.TempTheoDataVM != null)
                        {
                            if (!double.IsNaN(tdOptionVM.TempTheoDataVM.AskVol)) VolatilityModelVM.TheoAskVolLine1[idx] = new DataPoint(x, tdOptionVM.TempTheoDataVM.AskVol);
                            if (!double.IsNaN(tdOptionVM.TempTheoDataVM.BidVol)) VolatilityModelVM.TheoBidVolLine1[idx] = new DataPoint(x, tdOptionVM.TempTheoDataVM.BidVol);
                        }
                    }
                    if (idx >= 0)
                    {
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.AskVol))
                        {
                            VolatilityModelVM.TheoAskVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.AskVol);

                            var scatterPt = VolatilityModelVM.TheoPutAskVolScatter[idx];
                            VolatilityModelVM.TheoPutAskVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.AskVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                            scatterPt = VolatilityModelVM.TheoCallAskVolScatter[idx];
                            VolatilityModelVM.TheoCallAskVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.AskVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                        }
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.BidVol)) VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.BidVol);
                        {
                            VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(x, tdOptionVM.TheoDataVM.BidVol);

                            var scatterPt = VolatilityModelVM.TheoPutBidVolScatter[idx];
                            VolatilityModelVM.TheoPutBidVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.BidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                            scatterPt = VolatilityModelVM.TheoCallBidVolScatter[idx];
                            VolatilityModelVM.TheoCallBidVolScatter[idx] = new ScatterPoint(x, tdOptionVM.TheoDataVM.BidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                        }
                        VolatilityModelVM.TheoMidVolLine[idx] = new DataPoint(x, (tdOptionVM.TheoDataVM.AskVol + tdOptionVM.TheoDataVM.BidVol) / 2);
                        if (tdOptionVM.TempTheoDataVM != null)
                            VolatilityModelVM.TheoMidVolLine1[idx] = new DataPoint(x, (tdOptionVM.TempTheoDataVM.AskVol + tdOptionVM.TempTheoDataVM.BidVol) / 2);
                    }
                }
            }
        }


        public void SelectOption(string contract, string expiredate, string exchange)
        {
            var optionList = (from c in _contractList
                              where c.UnderlyingContract == contract && c.ExpireDate == expiredate && c.Exchange == exchange
                              select c).ToList();

            var strikeList = (from o in optionList
                              orderby o.StrikePrice
                              select o.StrikePrice).Distinct().ToList();

            var callList = (from o in optionList
                            where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                            orderby o.StrikePrice
                            select o.Contract).Distinct().ToList();

            var putList = (from o in optionList
                           where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                           orderby o.StrikePrice
                           select o.Contract).Distinct().ToList();

            ClearPlot();
            CallPutTDOptionVMCollection.Clear();
            var retList = _otcHandler.SubCallPutTDOptionData(strikeList, callList, putList, exchange);
            foreach (var vm in retList)
            {
                CallPutTDOptionVMCollection.Add(vm);
                VolatilityModelVM.CallAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.CallBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.CallMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.MidVol));
                VolatilityModelVM.PutAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.PutBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.PutMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
                VolatilityModelVM.TheoAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.TheoBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.TheoMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
                VolatilityModelVM.TheoAskVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.TheoBidVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.TheoMidVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));


                double value = (vm.PutStrategyVM != null && vm.PutStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoPutAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol, 10, value, vm.PutStrategyVM));
                VolatilityModelVM.TheoPutBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol, 10, value, vm.PutStrategyVM));

                value = (vm.CallStrategyVM != null && vm.CallStrategyVM.AskEnabled) ? 1 : 0;
                VolatilityModelVM.TheoCallAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.AskVol, 10, value, vm.CallStrategyVM));
                VolatilityModelVM.TheoCallBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.BidVol, 10, value, vm.CallStrategyVM));
            }
        }
        //public void SelectOption1(string contract)
        //{
        //    var optionList = (from c in _contractList
        //                      where c.UnderlyingContract == contract
        //                      select c).ToList();

        //    var strikeList = (from o in optionList
        //                      orderby o.StrikePrice
        //                      select o.StrikePrice).Distinct().ToList();

        //    var callList = (from o in optionList
        //                    where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
        //                    orderby o.StrikePrice
        //                    select o.Contract).Distinct().ToList();

        //    var putList = (from o in optionList
        //                   where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
        //                   orderby o.StrikePrice
        //                   select o.Contract).Distinct().ToList();

        //    ClearPlot1();
        //    CallPutTDOptionVMCollection1.Clear();
        //    var retList = _otcHandler.SubCallPutTDOptionData(strikeList, callList, putList);
        //    foreach (var vm in retList)
        //    {
        //        CallPutTDOptionVMCollection1.Add(vm);
        //        VolatilityModelVM1.TheoAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
        //        VolatilityModelVM1.TheoBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
        //        VolatilityModelVM1.TheoMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
        //        VolatilityModelVM1.TheoAskVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
        //        VolatilityModelVM1.TheoBidVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
        //        VolatilityModelVM1.TheoMidVolLine1.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
        //    }
        //}

        private void ClearPlot()
        {
            VolatilityModelVM.ClearAll();
        }
        private void ClearPlot1()
        {
            VolatilityModelVM1.ClearAll();
        }

        public void ClearTempVolLine()
        {
            VolatilityModelVM.TheoMidVolLine1.Clear();
            VolatilityModelVM.TheoAskVolLine1.Clear();
            VolatilityModelVM.TheoBidVolLine1.Clear();
        }
    }
}
