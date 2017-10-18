using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.Router
{
    public class OTCTradeHandlerRouter : MessageHandlerRouter<BaseTraderHandler>
    {
        public static OTCTradeHandlerRouter DefaultInstance { get; } = new OTCTradeHandlerRouter();
    }
}
