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
                      ((uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO, OnTradingDeskOptionParams, OnErrorAction);
        }
        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();


        private void OnTradingDeskOptionParams(PBTradingDeskOptionParams tradingDeskOption)
        {
            TradingDeskOptionVM quote = null;
            var cp = CallPutTDOptionVMCollection.FirstOrDefault((pb) => string.Compare(pb.PutOptionVM.Contract, tradingDeskOption.Contract, true) == 0);
            if (cp != null)
            {
                quote = cp.PutOptionVM;
            }
            else
            {
                cp = CallPutTDOptionVMCollection.FirstOrDefault((pb) => string.Compare(pb.CallOptionVM.Contract, tradingDeskOption.Contract, true) == 0);
                if (cp != null)
                {
                    quote = cp.CallOptionVM;
                }
            }

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
        }


        public void SubCallPutTDOptionData(IList<double> strikeList, IList<string> callList, IList<string> putList)
        {
            for (int i = 0; i < callList.Count; i++)
            {
                var callOption = new TradingDeskOptionVM { Contract = callList[i] };
                var putOption = new TradingDeskOptionVM { Contract = putList[i] };
                CallPutTDOptionVMCollection.Add(new CallPutTDOptionVM()
                {
                    StrikePrice = strikeList[i],
                    CallOptionVM = callOption,
                    PutOptionVM = putOption
                });
            }

            SubMarketData(callList);
            SubMarketData(putList);
        }

    }
}
