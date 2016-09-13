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
        public ObservableCollection<TradingDeskOptionVM> TradingDeskOptionVMCollection
        {
            get;
        } = new ObservableCollection<TradingDeskOptionVM>();

        private void OnTradingDeskOptionParams(PBTradingDeskOptionParams tradingDeskOption)
        {
            var tdOptionVM = TradingDeskOptionVMCollection.FindContract(tradingDeskOption.Exchange, tradingDeskOption.Contract);
            if (tdOptionVM == null)
            {
                tdOptionVM = new TradingDeskOptionVM
                {
                    Exchange = tradingDeskOption.Exchange,
                    Contract = tradingDeskOption.Contract
                };
                TradingDeskOptionVMCollection.Add(tdOptionVM);
            }

            tdOptionVM.MarketDataVM.AskPrice = tradingDeskOption.MarketData.AskPrice;
            tdOptionVM.MarketDataVM.AskSize = tradingDeskOption.MarketData.AskSize;
            tdOptionVM.MarketDataVM.AskVol = tradingDeskOption.MarketData.AskVolatility;
            tdOptionVM.MarketDataVM.BidPrice = tradingDeskOption.MarketData.BidPrice;
            tdOptionVM.MarketDataVM.BidSize = tradingDeskOption.MarketData.BidSize;
            tdOptionVM.MarketDataVM.BidVol = tradingDeskOption.MarketData.BidVolatility;

            tdOptionVM.TheoDataVM.AskPrice = tradingDeskOption.TheoData.AskPrice;
            tdOptionVM.TheoDataVM.AskSize = tradingDeskOption.TheoData.AskSize;
            tdOptionVM.TheoDataVM.AskVol = tradingDeskOption.TheoData.AskVolatility;
            tdOptionVM.TheoDataVM.AskDelta = tradingDeskOption.TheoData.AskDelta;
            tdOptionVM.TheoDataVM.AskGamma = tradingDeskOption.TheoData.AskGamma;
            tdOptionVM.TheoDataVM.AskTheta = tradingDeskOption.TheoData.AskTheta;
            tdOptionVM.TheoDataVM.AskVega = tradingDeskOption.TheoData.AskVega;

            tdOptionVM.TheoDataVM.BidPrice = tradingDeskOption.TheoData.BidPrice;
            tdOptionVM.TheoDataVM.BidSize = tradingDeskOption.TheoData.BidSize;
            tdOptionVM.TheoDataVM.BidVol = tradingDeskOption.TheoData.BidVolatility;
            tdOptionVM.TheoDataVM.BidDelta = tradingDeskOption.TheoData.BidDelta;
            tdOptionVM.TheoDataVM.BidGamma = tradingDeskOption.TheoData.BidGamma;
            tdOptionVM.TheoDataVM.BidTheta = tradingDeskOption.TheoData.BidTheta;
            tdOptionVM.TheoDataVM.BidVega = tradingDeskOption.TheoData.BidVega;
        }

    }
}
