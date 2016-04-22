using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class BaseException : Exception
    {
        public BaseException(int code, string message, int syscode)
            : base(message)
        {
            Code = code;
            SysCode = syscode;
        }

        public int Code
        {
            get;
            private set;
        }

        public int SysCode
        {
            get;
            private set;
        }
    }

    public class MessageException : BaseException
    {
        public MessageException(uint messageId, int code, string message, int syscode)
            : base(code, message, syscode)
        {
            MessageId = messageId;
        }
        public uint MessageId
        {
            get;
            private set;
        }
    }
}
