using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class OTCMarketDataHandler : BaseMarketDataHandler
    {
        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            //MessageWrapper.RegisterAction<PBPricingData, ExceptionMessage>
            //                ((uint)BusinessMessageID.MSG_ID_RTN_PRICING, OnReturningPricing, OnErrorAction);
        }
    }
}
