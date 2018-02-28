using Micro.Future.Business.Handler.Enums;
using Micro.Future.Utility;

namespace Micro.Future.Message
{
    public enum ParamActionType
    {
        [LocalizableDescription(@"ACTIONTYPE_NONE", typeof(Enums))]
        None = 0,

        [LocalizableDescription(@"ACTIONTYPE_WARNING", typeof(Enums))]
        WARNING = 1,

        [LocalizableDescription(@"ACTIONTYPE_STOPOPENORDER", typeof(Enums))]
        StopOpenOrder = 1
    };
    public enum ParamEnableType
    {
        [LocalizableDescription(@"ENABLED_FALSE", typeof(Enums))]
        False = 0,

        [LocalizableDescription(@"ENABLED_True", typeof(Enums))]
        True = 1,
    };

    public enum ParamMatchType
    {
        [LocalizableDescription(@"MATCHTYPE_ANY", typeof(Enums))]
        Any = 0,

        [LocalizableDescription(@"MATCHTYPE_ALL", typeof(Enums))]
        All = 1,
    };

    public enum ParamRiskControlType
    {
        [LocalizableDescription(@"TYPE_PRETRADE", typeof(Enums))]
        PreTrade = 0,

        [LocalizableDescription(@"TYPE_POSTTRADE", typeof(Enums))]
        PostTrade = 1,
    };
}
