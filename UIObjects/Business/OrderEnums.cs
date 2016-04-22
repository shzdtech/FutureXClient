using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public enum DirectionType
    {
        SELL = 0,
        BUY = 1
    };

    public enum OrderStatus
    {
        UNDEFINED = 0,
        ALL_TRADED = 1,
        PARTIAL_TRADED = 2,
        CANCELED = 3,
        OPEN_REJECTED = 4,
        REJECTED = 5,
        OPENNING = 10,
        PARTIAL_TRADING = 11,
        CANCELING = 12,
        CANCEL_REJECTED = 13
    };

    public enum OrderExecType
    {
        LIMIT = 0,
        MARKET = 1,
    };

    public enum OrderTIFType
    {
        GFD = 0,
        IOC = 1,
    };

    public enum TradingType
    {
        TRADINGTYPE_MANUAL = 0,
        TRADINGTYPE_QUOTE = 1,
        TRADINGTYPE_AUTO = 2,
        TRADINGTYPE_HEDGE = 3,
    };

    public enum OrderOffsetType
    {
        ///开仓
        OPEN = 0,
        ///平仓
        CLOSE = 1,
        ///强平
        FORCECLOSE = 2,
        ///平今
        CLOSETODAY = 3,
        ///平昨
        CLOSEYESTERDAY = 4,
        ///强减
        FORCEOFF = 5,
        ///本地强平
        LOCALFORCECLOSE = 6,
    };

    public enum HedgeType
    {
        ///投机
        HEDGETYPE_SPECULATION = 0,
        ///套利
        HEDGETYPE_ARBITRAGE = 1,
        ///套保
        HEDGETYPE_HEDGE = 2,
    };
}
