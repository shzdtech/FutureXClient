using Micro.Future.Message;
using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
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
        public static void Update(this IEnumerable<CallPutTDOptionVM> collection, TradingDeskOptionVM newVM)
        {
            TradingDeskOptionVM quote = null;
            var cp = collection.FirstOrDefault((pb) => string.Compare(pb.PutOptionVM.Contract, newVM.Contract, true) == 0);
            if (cp != null)
            {
                quote = cp.PutOptionVM;
            }
            else
            {
                cp = collection.FirstOrDefault((pb) => string.Compare(pb.CallOptionVM.Contract, newVM.Contract, true) == 0);
                if (cp != null)
                {
                    quote = cp.CallOptionVM;
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

                quote.TheoDataVM.AskPrice = newVM.TheoDataVM.AskPrice;
                quote.TheoDataVM.AskSize = newVM.TheoDataVM.AskSize;
                quote.TheoDataVM.AskVol = newVM.TheoDataVM.AskVol;
                quote.TheoDataVM.AskDelta = newVM.TheoDataVM.AskDelta;
                quote.TheoDataVM.AskGamma = newVM.TheoDataVM.AskGamma;
                quote.TheoDataVM.AskTheta = newVM.TheoDataVM.AskTheta;
                quote.TheoDataVM.AskVega = newVM.TheoDataVM.AskVega;

                quote.TheoDataVM.BidPrice = newVM.TheoDataVM.BidPrice;
                quote.TheoDataVM.BidSize = newVM.TheoDataVM.BidSize;
                quote.TheoDataVM.BidVol = newVM.TheoDataVM.BidVol;
                quote.TheoDataVM.BidDelta = newVM.TheoDataVM.BidDelta;
                quote.TheoDataVM.BidGamma = newVM.TheoDataVM.BidGamma;
                quote.TheoDataVM.BidTheta = newVM.TheoDataVM.BidTheta;
                quote.TheoDataVM.BidVega = newVM.TheoDataVM.BidVega;
            }
        }
    }
}
