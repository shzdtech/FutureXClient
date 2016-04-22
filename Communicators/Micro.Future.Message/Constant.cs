using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public static class FieldName
    {
        public const string BROKER_ID = "BrokerID";
        public const string USER_ID = "UserID";
        public const string PASSWORD = "Password";
        public const string NAME = "Name";

        public const string INSTRUMENT_ID = "InstrumentID";
        public const string EXCHANGE_ID = "ExchangeID";
        public const string EXCHANGE_INSTRUMENT_ID = "ExchangeInstID";
        public const string PRODUCT_ID = "ProductID";
        public const string ORDER_ID = "OrderID";
        public const string TRADE_ID = "TradeID";

        public const string DATE = "Date";
        public const string TIME = "Time";
        public const string TIME_START = "TimeStart";
        public const string TIME_END = "TimeEnd";
        public const string EMAIL = "Email";
        public const string COMPANY = "Company";
        public const string CONTACT_NUM = "ContactNum";
    }


    public static class DefautFutureXOption
    {
        public const string AUTHNTICATION_TYPE = "FutureXCookie";
    }
}
