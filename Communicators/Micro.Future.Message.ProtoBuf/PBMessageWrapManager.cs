using System;
using System.Collections.Generic;

namespace Micro.Future.Message.PBMessageHandler
{
    public class PBMessageWrapManager : Dictionary<string, AbstractMessageWrapper>
    {
        private static readonly PBMessageWrapManager _instance = new PBMessageWrapManager();
        public static PBMessageWrapManager Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public AbstractMessageWrapper CreateMessageWrapper(string name)
        {
            lock (this)
            {
                AbstractMessageWrapper msgwrp;
                if (!TryGetValue(name, out msgwrp))
                {
                    msgwrp = new PBMessageWrapper();
                    this[name] = msgwrp;
                }

                return msgwrp;
            }
        }

        public void AttachMessageWrapper(string name, AbstractMessageWrapper msgWrapper)
        {
            this[name] = msgWrapper;
        }
        public AbstractMessageWrapper DetachMessageWrapper(string name)
        {
            var msgwrp = this[name];
            this.Remove(name);
            return msgwrp;
        }
    }
}
