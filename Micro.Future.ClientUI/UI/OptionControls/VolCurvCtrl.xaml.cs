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
            volPlot.DataContext = _otcHandler.VolatilityLinesVM;
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

                }
                else
                {
                    idx = list.FindIndex((pb) => string.Compare(pb.CallOptionVM.Contract, tdOptionVM.Contract, true) == 0);
                    if (idx >= 0) // Update CallOption
                    {
                        var datapt = VolatilityLinesVM.CallAskVolLine[idx];
                        VolatilityLinesVM.CallAskVolLine[idx] = new DataPoint(datapt.X, tdOptionVM.MarketDataVM.AskPrice);
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
                VolatilityLinesVM.CallAskVolLine.Add(new DataPoint(vm.StrikePrice, vm.CallOptionVM.MarketDataVM.AskPrice));
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
                        // int index = (int)nearestPoint.Index;
                        bool isOn = (bool)point.Tag;
                        point.Tag = !isOn;
                        point.Value = isOn ? 1 : 0;
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
