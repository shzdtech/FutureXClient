using Micro.Future.EnumMap;
using Micro.Future.UI;

namespace Micro.Future.Message
{
    public enum DirectionType
    {
        [LocalizableDescription(@"DirectionType_Sell", typeof(Resource))]
        SELL = 0,

        [LocalizableDescription(@"DirectionType_Buy", typeof(Resource))]
        BUY = 1
    };

    public enum OrderStatus
    {
        [LocalizableDescription(@"OrderStatus_UNDEFINED", typeof(Resource))]
        UNDEFINED = 0,
        [LocalizableDescription(@"OrderStatus_ALL_TRADED", typeof(Resource))]
        ALL_TRADED = 1,
        [LocalizableDescription(@"OrderStatus_PARTIAL_TRADED", typeof(Resource))]
        PARTIAL_TRADED = 2,
        [LocalizableDescription(@"OrderStatus_CANCELED", typeof(Resource))]
        CANCELED = 3,
        [LocalizableDescription(@"OrderStatus_OPEN_REJECTED", typeof(Resource))]
        OPEN_REJECTED = 4,
        [LocalizableDescription(@"OrderStatus_REJECTED", typeof(Resource))]
        REJECTED = 5,
        [LocalizableDescription(@"OrderStatus_OPENNING", typeof(Resource))]
        OPENNING = 10,
        [LocalizableDescription(@"OrderStatus_PARTIAL_TRADING", typeof(Resource))]
        PARTIAL_TRADING = 11,
        [LocalizableDescription(@"OrderStatus_CANCELING", typeof(Resource))]
        CANCELING = 12,
        [LocalizableDescription(@"OrderStatus_CANCEL_REJECTED", typeof(Resource))]
        CANCEL_REJECTED = 13
    };

    public enum OrderExecType
    {
        [LocalizableDescription(@"EXECTYPE_LIMIT", typeof(Resource))]
        LIMIT = 0,
        [LocalizableDescription(@"EXECTYPE_MARKET", typeof(Resource))]
        MARKET = 1,
    };

    public enum OrderTIFType
    {
        [LocalizableDescription(@"OrderTIFType_GFD", typeof(Resource))]
        GFD = 0,
        [LocalizableDescription(@"OrderTIFType_IOC", typeof(Resource))]
        IOC = 1,
    };

    public enum TradingType
    {
        [LocalizableDescription(@"TRADINGTYPE_MANUAL", typeof(Resource))]
        TRADINGTYPE_MANUAL = 0,
        [LocalizableDescription(@"TRADINGTYPE_QUOTE", typeof(Resource))]
        TRADINGTYPE_QUOTE = 1,
        [LocalizableDescription(@"TRADINGTYPE_AUTO", typeof(Resource))]
        TRADINGTYPE_AUTO = 2,
        [LocalizableDescription(@"TRADINGTYPE_HEDGE", typeof(Resource))]
        TRADINGTYPE_HEDGE = 3,
    };

    public enum OrderOffsetType
    {
        ///开仓
        [LocalizableDescription(@"OpenClose_OPEN", typeof(Resource))]
        OPEN = 0,
        ///平仓
        [LocalizableDescription(@"OpenClose_CLOSE", typeof(Resource))]
        CLOSE = 1,
        ///强平
        [LocalizableDescription(@"OpenClose_FORCECLOSE", typeof(Resource))]
        FORCECLOSE = 2,
        ///平今
        [LocalizableDescription(@"OpenClose_CLOSETODAY", typeof(Resource))]
        CLOSETODAY = 3,
        ///平昨
        [LocalizableDescription(@"OpenClose_CLOSEYESTERDAY", typeof(Resource))]
        CLOSEYESTERDAY = 4,
        ///强减
        [LocalizableDescription(@"OpenClose_FORCEOFF", typeof(Resource))]
        FORCEOFF = 5,
        ///本地强平
        [LocalizableDescription(@"OpenClose_LOCALFORCECLOSE", typeof(Resource))]
        LOCALFORCECLOSE = 6,
    };

    public enum HedgeType
    {
        ///投机
        [LocalizableDescription(@"HEDGETYPE_SPECULATION", typeof(Resource))]
        HEDGETYPE_SPECULATION = 0,
        ///套利
        [LocalizableDescription(@"HEDGETYPE_ARBITRAGE", typeof(Resource))]
        HEDGETYPE_ARBITRAGE = 1,
        ///套保
        [LocalizableDescription(@"HEDGETYPE_HEDGE", typeof(Resource))]
        HEDGETYPE_HEDGE = 2,
    };

    public enum PositionDirectionType
    {
        [LocalizableDescription(@"DirectionType_NET", typeof(Resource))]
        PD_NET = 0,

        [LocalizableDescription(@"PositionDirectionType_LONG", typeof(Resource))]
        PD_LONG = 1,

        [LocalizableDescription(@"PositionDirectionType_SHORT", typeof(Resource))]
        PD_SHORT = 2,
    };

}
