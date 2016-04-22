using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public abstract class AbstractMessageHandler
    {
        public event Action<MessageException> OnError;

        public AbstractMessageHandler()
        {

        }

        public AbstractMessageWrapper MessageWrapper { get; protected set; }

        public AbstractMessageHandler(AbstractMessageWrapper messageWrapper)
        {
            RegisterMessageWrapper(messageWrapper);
        }

        public void RegisterMessageWrapper(AbstractMessageWrapper messageWrapper)
        {
            if (messageWrapper == null)
                throw new ArgumentNullException(nameof(messageWrapper));

            MessageWrapper = messageWrapper;
            OnMessageWrapperRegistered(messageWrapper);
        }

        public abstract void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper);

        public void RaiseOnError(MessageException mex)
        {
            if (OnError != null)
            {
                OnError(mex);
            }
        }
    }
    public abstract class MessageHandlerTemplate<T> : AbstractMessageHandler where T : new()
    {
        private readonly static T _instance = new T();

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
