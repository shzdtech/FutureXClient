using Micro.Future.Message;
using Micro.Future.Message.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class OTCMDTradingDeskHandler: AbstractOTCMarketDataHandler
    {
        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);

            MessageWrapper.RegisterAction<PBPricingDataList, BizErrorMsg>
                            ((uint)BusinessMessageID.MSG_ID_OTC_RET_MARKETDATA, OnReturningPricing, OnErrorAction);
        }
        protected override void OnReturningPricing(Message.Business.PBPricingDataList PB)
        {
            foreach (var p in PB.PricingList)
            {
                var ps = from s in StrategyVMCollection
                                  where (s.Exchange == p.Exchange && s.Contract == p.Contract)
                                  select s;
                foreach(var s in ps)
                {
                    s.BidPrice = p.BidPrice;
                    s.AskPrice = p.AskPrice;
                }
            }
        }
    }
}
