using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.Router
{
    public class OTCMarketDataHandlerRouter : MessageHandlerRouter<BaseMarketDataHandler>
    {
        public static OTCMarketDataHandlerRouter DefaultInstance { get; } = new OTCMarketDataHandlerRouter();
    }
}
