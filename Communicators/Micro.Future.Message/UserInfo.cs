using System;

namespace Micro.Future.Message
{
    public enum RoleType
    {
        UserClient = 1,
        TradingDesk = 2,
    }
    public class UserInfo
    {
        public string ID
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ContactNumber
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }

        public string BrokerID
        {
            get;
            set;
        }

        public RoleType Role
        {
            get;
            set;
        }

        public int Permission
        {
            get;
            set;
        }
    }
}
