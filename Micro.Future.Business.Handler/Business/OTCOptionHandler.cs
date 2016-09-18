using Micro.Future.Message;
using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class OTCOptionHandler : AbstractOTCHandler
    {
        public VolatilityLinesVM VolatilityLinesVM
        {
            get;
        } = new VolatilityLinesVM();
        public OptionOxyVM OptionOxyVM
        {
            get;
        } = new OptionOxyVM();
        public void OnUpdateOption()
        {
            var columnSeries = new ColumnSeries();
            for (double i = 0.5; i < 10; i++)
            {
                VolatilityLinesVM.CallAskVolLine.Add(new DataPoint(i, 10 - 0.5 * i));
                VolatilityLinesVM.CallBidVolLine.Add(new DataPoint(i, i));
                VolatilityLinesVM.TheoBidVolLine.Add(new DataPoint(i, i));
                VolatilityLinesVM.TheoBidPutVolScatter.Add(new ScatterPoint(i, i, double.NaN, i / 10, true));
                VolatilityLinesVM.TheoBidCallVolScatter.Add(new ScatterPoint(i, i, double.NaN, i / 10, true));
                columnSeries.Items.Add(new ColumnItem { Value = i });
            }
            OptionOxyVM.PlotModelBar.Series.Add(columnSeries);
        }

        public void OnUpdateTest()
        {
            for (int i = 0; i < 10; i++)
            {
                RiskVMCollection.Add(new RiskVM { Delta = i, DisplayName = i.ToString(), PositionDelta = i, PositionVega = i, Value = i, Vega = i });
                PositionVMCollection.Add(new PositionVM { Selected = true, StrikePrice = i, ProductType = 0, Style = 0, Position = i });
                PriceGreekVMCollection.Add(new PriceGreekVM { cAsk = i, cBid = i, cDelta = i, cMid = i, cVega = i, DisplayName = i.ToString(), pAsk = i, pBid = i, pDelta = i, pMid = i, pVega = i, Strike = i });
                VolatilityVMCollection.Add(new VolatilityVM { DisplayName = i.ToString(), Strike = i, VolAsk = i, volBid = i, VolMid = i });
            }
        }
        public ObservableCollection<RiskVM> RiskVMCollection
        {
            get;
        } = new ObservableCollection<RiskVM>();
        public ObservableCollection<PriceGreekVM> PriceGreekVMCollection
        {
            get;
        } = new ObservableCollection<PriceGreekVM>();

        public ObservableCollection<VolatilityVM> VolatilityVMCollection
        {
            get;
        } = new ObservableCollection<VolatilityVM>();
        public ObservableCollection<PositionVM> PositionVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);
            MessageWrapper.RegisterAction<PBTradingDeskOptionParams, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_RTN_TRADINGDESK_PRICING, OnTradingDeskOptionParams, OnErrorAction);
        }

        private void OnTradingDeskOptionParams(PBTradingDeskOptionParams tradingDeskOption)
        {
            TradingDeskOptionVM quote = new TradingDeskOptionVM();

            quote.MarketDataVM.AskPrice = tradingDeskOption.MarketData.AskPrice;
            quote.MarketDataVM.AskSize = tradingDeskOption.MarketData.AskSize;
            quote.MarketDataVM.AskVol = tradingDeskOption.MarketData.AskVolatility;
            quote.MarketDataVM.BidPrice = tradingDeskOption.MarketData.BidPrice;
            quote.MarketDataVM.BidSize = tradingDeskOption.MarketData.BidSize;
            quote.MarketDataVM.BidVol = tradingDeskOption.MarketData.BidVolatility;
            quote.MarketDataVM.MidVol = (tradingDeskOption.MarketData.BidVolatility + tradingDeskOption.MarketData.AskVolatility)/2;
            quote.MarketDataVM.MidPrice = (tradingDeskOption.MarketData.BidPrice + tradingDeskOption.MarketData.AskPrice) / 2;

            quote.TheoDataVM.AskPrice = tradingDeskOption.TheoData.AskPrice;
            quote.TheoDataVM.AskSize = tradingDeskOption.TheoData.AskSize;
            quote.TheoDataVM.AskVol = tradingDeskOption.TheoData.AskVolatility;
            quote.TheoDataVM.AskDelta = tradingDeskOption.TheoData.AskDelta;
            quote.TheoDataVM.AskGamma = tradingDeskOption.TheoData.AskGamma;
            quote.TheoDataVM.AskTheta = tradingDeskOption.TheoData.AskTheta;
            quote.TheoDataVM.AskVega = tradingDeskOption.TheoData.AskVega;

            quote.TheoDataVM.BidPrice = tradingDeskOption.TheoData.BidPrice;
            quote.TheoDataVM.BidSize = tradingDeskOption.TheoData.BidSize;
            quote.TheoDataVM.BidVol = tradingDeskOption.TheoData.BidVolatility;
            quote.TheoDataVM.BidDelta = tradingDeskOption.TheoData.BidDelta;
            quote.TheoDataVM.BidGamma = tradingDeskOption.TheoData.BidGamma;
            quote.TheoDataVM.BidTheta = tradingDeskOption.TheoData.BidTheta;
            quote.TheoDataVM.BidVega = tradingDeskOption.TheoData.BidVega;
            quote.TheoDataVM.MidVol = (tradingDeskOption.TheoData.BidVolatility + tradingDeskOption.TheoData.AskVolatility)/2;
            quote.TheoDataVM.MidPrice = (tradingDeskOption.TheoData.BidPrice + tradingDeskOption.TheoData.AskPrice) / 2;
            OnTradingDeskOptionParamsReceived?.Invoke(quote);
        }


        public event Action<TradingDeskOptionVM> OnTradingDeskOptionParamsReceived;


        public IList<CallPutTDOptionVM> SubCallPutTDOptionData(IList<double> strikeList, IList<string> callList, IList<string> putList)
        {
            var retList = new List<CallPutTDOptionVM>();

            for (int i = 0; i < callList.Count; i++)
            {
                var callOption = new TradingDeskOptionVM { Contract = callList[i] };
                var putOption = new TradingDeskOptionVM { Contract = putList[i] };
                retList.Add(new CallPutTDOptionVM()
                {
                    StrikePrice = strikeList[i],
                    CallOptionVM = callOption,
                    PutOptionVM = putOption
                });
            }

            SubMarketData(callList);
            SubMarketData(putList);

            return retList;
        }

    }

    public static class OptionVMExtensions
    {
        public static bool Update(this IEnumerable<CallPutTDOptionVM> collection, TradingDeskOptionVM newVM)
        {
            bool ret;
            TradingDeskOptionVM quote = null;
            var cp = collection.FirstOrDefault((pb) => string.Compare(pb.PutOptionVM.Contract, newVM.Contract, true) == 0);
            if (cp != null)
            {
                quote = cp.PutOptionVM;
                ret = true;
            }
            else
            {
                cp = collection.FirstOrDefault((pb) => string.Compare(pb.CallOptionVM.Contract, newVM.Contract, true) == 0);
                if (cp != null)
                {
                    quote = cp.CallOptionVM;
                    ret = true;
                }
            }

            if (quote != null)
            {
                quote.MarketDataVM.AskPrice = newVM.MarketDataVM.AskPrice;
                quote.MarketDataVM.AskSize = newVM.MarketDataVM.AskSize;
                quote.MarketDataVM.AskVol = newVM.MarketDataVM.AskVol;
                quote.MarketDataVM.BidPrice = newVM.MarketDataVM.BidPrice;
                quote.MarketDataVM.BidSize = newVM.MarketDataVM.BidSize;
                quote.MarketDataVM.BidVol = newVM.MarketDataVM.BidVol;
                quote.MarketDataVM.MidVol = newVM.MarketDataVM.MidVol;
                quote.MarketDataVM.MidPrice = newVM.MarketDataVM.MidPrice;

                quote.TheoDataVM.AskPrice = newVM.TheoDataVM.AskPrice;
                quote.TheoDataVM.AskSize = newVM.TheoDataVM.AskSize;
                quote.TheoDataVM.AskVol = newVM.TheoDataVM.AskVol;
                quote.TheoDataVM.AskDelta = newVM.TheoDataVM.AskDelta;
                quote.TheoDataVM.AskGamma = newVM.TheoDataVM.AskGamma;
                quote.TheoDataVM.AskTheta = newVM.TheoDataVM.AskTheta;
                quote.TheoDataVM.AskVega = newVM.TheoDataVM.AskVega;
                quote.TheoDataVM.MidVol = newVM.TheoDataVM.MidVol;
                quote.TheoDataVM.MidPrice = newVM.TheoDataVM.MidPrice;

                quote.TheoDataVM.BidPrice = newVM.TheoDataVM.BidPrice;
                quote.TheoDataVM.BidSize = newVM.TheoDataVM.BidSize;
                quote.TheoDataVM.BidVol = newVM.TheoDataVM.BidVol;
                quote.TheoDataVM.BidDelta = newVM.TheoDataVM.BidDelta;
                quote.TheoDataVM.BidGamma = newVM.TheoDataVM.BidGamma;
                quote.TheoDataVM.BidTheta = newVM.TheoDataVM.BidTheta;
                quote.TheoDataVM.BidVega = newVM.TheoDataVM.BidVega;
            }

            return true;
        }
    }
}
