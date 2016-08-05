using Micro.Future.Business.Handler.Enums;
using Micro.Future.Utility;

namespace Micro.Future.Message
{
    public enum DirectionType
    {
        [LocalizableDescription(@"DirectionType_Sell", typeof(Enums))]
        SELL = 0,

        [LocalizableDescription(@"DirectionType_Buy", typeof(Enums))]
        BUY = 1
    };

    public enum OrderStatus
    {
        [LocalizableDescription(@"OrderStatus_UNDEFINED", typeof(Enums))]
        UNDEFINED = 0,
        [LocalizableDescription(@"OrderStatus_ALL_TRADED", typeof(Enums))]
        ALL_TRADED = 1,
        [LocalizableDescription(@"OrderStatus_PARTIAL_TRADED", typeof(Enums))]
        PARTIAL_TRADED = 2,
        [LocalizableDescription(@"OrderStatus_CANCELED", typeof(Enums))]
        CANCELED = 3,
        [LocalizableDescription(@"OrderStatus_OPEN_REJECTED", typeof(Enums))]
        OPEN_REJECTED = 4,
        [LocalizableDescription(@"OrderStatus_REJECTED", typeof(Enums))]
        REJECTED = 5,
        [LocalizableDescription(@"OrderStatus_OPENED", typeof(Enums))]
        OPENED = 10,
        [LocalizableDescription(@"OrderStatus_PARTIAL_TRADING", typeof(Enums))]
        PARTIAL_TRADING = 11,
        [LocalizableDescription(@"OrderStatus_CANCELING", typeof(Enums))]
        CANCELING = 12,
        [LocalizableDescription(@"OrderStatus_CANCEL_REJECTED", typeof(Enums))]
        CANCEL_REJECTED = 13,
        [LocalizableDescription(@"OrderStatus_SUBMITTING", typeof(Enums))]
        SUBMITTING = 14,
    };

    public enum OrderExecType
    {
        [LocalizableDescription(@"EXECTYPE_LIMIT", typeof(Enums))]
        LIMIT = 0,
        [LocalizableDescription(@"EXECTYPE_MARKET", typeof(Enums))]
        MARKET = 1,
    };

    public enum OrderTIFType
    {
        [LocalizableDescription(@"OrderTIFType_GFD", typeof(Enums))]
        GFD = 0,
        [LocalizableDescription(@"OrderTIFType_IOC", typeof(Enums))]
        IOC = 1,
    };

    public enum TradingType
    {
        [LocalizableDescription(@"TRADINGTYPE_MANUAL", typeof(Enums))]
        TRADINGTYPE_MANUAL = 0,
        [LocalizableDescription(@"TRADINGTYPE_QUOTE", typeof(Enums))]
        TRADINGTYPE_QUOTE = 1,
        [LocalizableDescription(@"TRADINGTYPE_AUTO", typeof(Enums))]
        TRADINGTYPE_AUTO = 2,
        [LocalizableDescription(@"TRADINGTYPE_HEDGE", typeof(Enums))]
        TRADINGTYPE_HEDGE = 3,
    };

    public enum OrderOffsetType
    {
        ///开仓
        [LocalizableDescription(@"OpenClose_OPEN", typeof(Enums))]
        OPEN = 0,
        ///平仓
        [LocalizableDescription(@"OpenClose_CLOSE", typeof(Enums))]
        CLOSE = 1,
        ///强平
        [LocalizableDescription(@"OpenClose_FORCECLOSE", typeof(Enums))]
        FORCECLOSE = 2,
        ///平今
        [LocalizableDescription(@"OpenClose_CLOSETODAY", typeof(Enums))]
        CLOSETODAY = 3,
        ///平昨
        [LocalizableDescription(@"OpenClose_CLOSEYESTERDAY", typeof(Enums))]
        CLOSEYESTERDAY = 4,
        ///强减
        [LocalizableDescription(@"OpenClose_FORCEOFF", typeof(Enums))]
        FORCEOFF = 5,
        ///本地强平
        [LocalizableDescription(@"OpenClose_LOCALFORCECLOSE", typeof(Enums))]
        LOCALFORCECLOSE = 6,
    };

    public enum HedgeType
    {
        ///投机
        [LocalizableDescription(@"HEDGETYPE_SPECULATION", typeof(Enums))]
        HEDGETYPE_SPECULATION = 0,
        ///套利
        [LocalizableDescription(@"HEDGETYPE_ARBITRAGE", typeof(Enums))]
        HEDGETYPE_ARBITRAGE = 1,
        ///套保
        [LocalizableDescription(@"HEDGETYPE_HEDGE", typeof(Enums))]
        HEDGETYPE_HEDGE = 2,
    };

    public enum PositionDirectionType
    {
        [LocalizableDescription(@"DirectionType_NET", typeof(Enums))]
        PD_NET = 0,

        [LocalizableDescription(@"PositionDirectionType_LONG", typeof(Enums))]
        PD_LONG = 1,

        [LocalizableDescription(@"PositionDirectionType_SHORT", typeof(Enums))]
        PD_SHORT = 2,
    };

    public enum Type1
    {

        Future = 0,
       
        Option = 1,

    };

    public enum Style
    {

        Vanilla = 0,

        Geometric = 1,

        Arethmetic = 1,


    };

}
