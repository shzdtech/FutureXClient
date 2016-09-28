﻿using Micro.Future.Message;
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

        public VolatilityLinesVM VolatilityModelVM { get; } = new VolatilityLinesVM();

        public void Initialize()
        {
            _contractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);

            //VegaPosition.Model = _otcHandler.OptionOxyVM.PlotModelBar;
            volPlot.DataContext = VolatilityModelVM;


            theoBidSC.MarkerOutline = CustomOxyMarkers.LDTriangle;
            theoAskSC.MarkerOutline = CustomOxyMarkers.RDTriangle;

            var internalBidSS = theoBidSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalBidSS.MouseDown += BidScatter_MouseDown;
            var internalAskSS = theoAskSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalAskSS.MouseDown += AskScatter_MouseDown;

            _otcHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;

        }

        private void BidScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.Position.X < e.HitTestResult.NearestHitPoint.X)
            {
                var plot = e.HitTestResult.Element as PlotElement;
                if (plot != null)
                {
                    var point = e.HitTestResult.Item as ScatterPoint;
                    if (point != null)
                    {
                        var strategyVM = (StrategyVM)point.Tag;
                        strategyVM.BidEnabled = !strategyVM.BidEnabled;
                        point.Value = strategyVM.BidEnabled ? 1 : 0;
                        plot.PlotModel.InvalidatePlot(false);
                    }
                }
            }
        }
        private void AskScatter_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.Position.X > e.HitTestResult.NearestHitPoint.X)
            {
                var plot = e.HitTestResult.Element as PlotElement;
                if (plot != null)
                {
                    var point = e.HitTestResult.Item as ScatterPoint;
                    if (point != null)
                    {
                        var strategyVM = (StrategyVM)point.Tag;
                        strategyVM.AskEnabled = !strategyVM.AskEnabled;
                        point.Value = strategyVM.AskEnabled ? 1 : 0;
                        plot.PlotModel.InvalidatePlot(false);
                    }
                }
            }
        }

        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM tdOptionVM)
        {
            if (CallPutTDOptionVMCollection.Update(tdOptionVM))
            {
                var list = CallPutTDOptionVMCollection.ToList();
                int idx = list.FindIndex((pb) => string.Compare(pb.PutOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                if (idx >= 0) // Update PutOption
                {
                    var datapt = VolatilityModelVM.PutAskVolLine[idx];
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.AskVol)) VolatilityModelVM.PutAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.BidVol)) VolatilityModelVM.PutBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.BidVol);
                    if (!double.IsNaN(tdOptionVM.MarketDataVM.MidVol)) VolatilityModelVM.PutMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.MidVol);
                    if (!double.IsNaN(tdOptionVM.TheoDataVM.AskVol)) VolatilityModelVM.TheoAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.AskVol);
                    if (!double.IsNaN(tdOptionVM.TheoDataVM.BidVol)) VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.BidVol);
                    if (!double.IsNaN(tdOptionVM.TheoDataVM.MidVol))
                    {
                        var scatterPt = VolatilityModelVM.TheoAskVolScatter[idx];
                        VolatilityModelVM.TheoAskVolScatter[idx] = new ScatterPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                        scatterPt = VolatilityModelVM.TheoBidVolScatter[idx];
                        VolatilityModelVM.TheoBidVolScatter[idx] = new ScatterPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                        VolatilityModelVM.TheoMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol);
                    }
                }
                else
                {
                    idx = list.FindIndex((pb) => string.Compare(pb.CallOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                    if (idx >= 0) // Update CallOption
                    {
                        var datapt = VolatilityModelVM.CallAskVolLine[idx];
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.AskVol)) VolatilityModelVM.CallAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.AskVol);
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.BidVol)) VolatilityModelVM.CallBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.BidVol);
                        if (!double.IsNaN(tdOptionVM.MarketDataVM.MidVol)) VolatilityModelVM.CallMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.MidVol);
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.AskVol)) VolatilityModelVM.TheoAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.AskVol);
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.BidVol)) VolatilityModelVM.TheoBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.BidVol);
                        if (!double.IsNaN(tdOptionVM.TheoDataVM.MidVol))
                        {
                            var scatterPt = VolatilityModelVM.TheoAskVolScatter[idx];
                            VolatilityModelVM.TheoAskVolScatter[idx] = new ScatterPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                            scatterPt = VolatilityModelVM.TheoBidVolScatter[idx];
                            VolatilityModelVM.TheoBidVolScatter[idx] = new ScatterPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol, scatterPt.Size, scatterPt.Value, scatterPt.Tag);
                            VolatilityModelVM.TheoMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol);
                        }
                    }
                }
            }
        }


        public void SelectOption(string contract)
        {
            var optionList = (from c in _contractList
                              where c.UnderlyingContract == contract
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
            var retList = _otcHandler.SubCallPutTDOptionData(strikeList, callList, putList);
            foreach (var vm in retList)
            {
                CallPutTDOptionVMCollection.Add(vm);
                VolatilityModelVM.CallAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.CallBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.CallMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.MidVol));
                VolatilityModelVM.PutAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
                VolatilityModelVM.PutBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
                VolatilityModelVM.PutMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
                VolatilityModelVM.TheoAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.AskVol));
                VolatilityModelVM.TheoBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.BidVol));
                VolatilityModelVM.TheoMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.MidVol));
                VolatilityModelVM.TheoAskVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.AskVol, double.NaN, 0, vm.CallStrategyVM));
                VolatilityModelVM.TheoBidVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.BidVol, double.NaN, 0, vm.CallStrategyVM));
            }
        }

        private void ClearPlot()
        {
            VolatilityModelVM.ClearAll();
        }
    }
}
