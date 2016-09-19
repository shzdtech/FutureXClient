using Micro.Future.Message;
using Micro.Future.UI.OptionControls;
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
        }

        public VolatilityLinesVM VolatilityLinesVM
        {
            get;
        } = new VolatilityLinesVM();


        public void Initialize()
        {
            using (var clientCache = new ClientDbContext())
            {
                _contractList = clientCache.GetContractsByProductType((int)ProductType.PRODUCT_OPTIONS);
            }

            //PlotVolatility.Model = traderExHandler.OptionOxyVM.PlotModel;
            volPlot.DataContext = VolatilityLinesVM;
            VegaPosition.Model = _otcHandler.OptionOxyVM.PlotModelBar;
            theoBidPutSC.MarkerOutline = CustomOxyMarkers.LDTriangle;
            theoBidCallSC.MarkerOutline = CustomOxyMarkers.RDTriangle;

            var internalSS = theoBidCallSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalSS.MouseDown += InternalSS_MouseDown;

            _otcHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;

        }

        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM tdOptionVM)
        {
            if(CallPutTDOptionVMCollection.Update(tdOptionVM))
            {
                var list = CallPutTDOptionVMCollection.ToList();
                int idx = list.FindIndex((pb) => string.Compare(pb.PutOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                if(idx >= 0) // Update PutOption
                {
                    var datapt = VolatilityLinesVM.PutAskVolLine[idx];
                    VolatilityLinesVM.PutAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.AskVol);
                    VolatilityLinesVM.PutBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.BidVol);
                    VolatilityLinesVM.PutMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.MidVol);
                    VolatilityLinesVM.TheoPutAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.AskVol);
                    VolatilityLinesVM.TheoPutBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.BidVol);
                    VolatilityLinesVM.TheoPutMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol);

                }
                else
                {
                    idx = list.FindIndex((pb) => string.Compare(pb.CallOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                    if (idx >= 0) // Update CallOption
                    {
                        var datapt = VolatilityLinesVM.CallAskVolLine[idx];
                        VolatilityLinesVM.CallAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.AskVol);
                        VolatilityLinesVM.CallBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.BidVol);
                        VolatilityLinesVM.CallMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.MidVol);
                        VolatilityLinesVM.TheoCallAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.AskVol);
                        VolatilityLinesVM.TheoCallBidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.BidVol);
                        VolatilityLinesVM.TheoCallMidVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.TheoDataVM.MidVol);

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
                            where o.ContractType == 2
                            orderby o.StrikePrice
                            select o.Contract).Distinct().ToList();

            var putList = (from o in optionList
                           where o.ContractType == 3
                           orderby o.StrikePrice
                           select o.Contract).Distinct().ToList();

            ClearPlot();
            CallPutTDOptionVMCollection.Clear();
            var retList = _otcHandler.SubCallPutTDOptionData(strikeList, callList, putList);
            foreach (var vm in retList)
            {
                CallPutTDOptionVMCollection.Add(vm);
                VolatilityLinesVM.CallAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.AskVol));
                VolatilityLinesVM.CallBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.BidVol));
                VolatilityLinesVM.CallMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.MidVol));
                VolatilityLinesVM.PutAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.AskVol));
                VolatilityLinesVM.PutBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.BidVol));
                VolatilityLinesVM.PutMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.MarketDataVM.MidVol));
                VolatilityLinesVM.TheoPutAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.AskVol));
                VolatilityLinesVM.TheoPutBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.BidVol));
                VolatilityLinesVM.TheoPutMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.MidVol));
                VolatilityLinesVM.TheoCallAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.AskVol));
                VolatilityLinesVM.TheoCallBidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.BidVol));
                VolatilityLinesVM.TheoCallMidVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.MidVol));
                VolatilityLinesVM.TheoBidPutVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.PutOptionVM.TheoDataVM.BidVol, double.NaN, 0, vm.CallStrategyVM));
                VolatilityLinesVM.TheoBidCallVolScatter.Add(new ScatterPoint(vm.StrikePrice, vm.CallOptionVM.TheoDataVM.BidVol, double.NaN, 0, vm.CallStrategyVM));
            }
        }

        private void ClearPlot()
        {
            VolatilityLinesVM.ClearAll();
        }


        private void InternalSS_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var bpSC = sender as OxyPlot.Series.ScatterSeries;
            if (bpSC != null)
            {
                TrackerHitResult nearestPoint = bpSC.GetNearestPoint(e.Position, false);
                if (nearestPoint != null)
                {
                    var point = nearestPoint.Item as ScatterPoint;
                    if (point != null)
                    {
                        var strategyVM = (StrategyVM)point.Tag;
                        strategyVM.Enable = !strategyVM.Enable;
                        point.Value = strategyVM.Enable ? 1 : 0;
                        bpSC.PlotModel.InvalidatePlot(false);

                    }
                }
            }
        }


        private void theoBidPutSC_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void theoBidCallSC_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }



}
