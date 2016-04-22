using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class MessageHandlerContainer : IEnumerable<KeyValuePair<Type, AbstractMessageHandler>>
    {    

        protected IDictionary<Type, AbstractMessageHandler> _messageHandlerMap =
            new Dictionary<Type, AbstractMessageHandler>();

        public static MessageHandlerContainer DefaultInstance { get; set; } = new MessageHandlerContainer();

        public static void Register<TMessageHandler, TBusinessHandler>(SignInOptions options = null)
           where TMessageHandler : AbstractMessageHandler
           where TBusinessHandler : TMessageHandler
        {
            Registry[typeof(TMessageHandler)] =
                new Tuple<Type, SignInOptions>(typeof(TBusinessHandler), options);
        }

        public static void Register<TMessageHandler>(Type TBusinessHandler, SignInOptions options = null)
           where TMessageHandler : AbstractMessageHandler
        {
            Registry[typeof(TMessageHandler)] =
                new Tuple<Type, SignInOptions>(TBusinessHandler, options);
        }

        public static void UnRegister<TMessageHandler>()
            where TMessageHandler : AbstractMessageHandler
        {
            Registry.Remove(typeof(TMessageHandler));
        }

        public static void UnRegister(Type TMessageHandler)
        {
            Registry.Remove(TMessageHandler);
        }

        public static SignInOptions GetSignInOptions(Type msgHdlType)
        {
            Tuple<Type, SignInOptions> tuple = null;
            if (Registry.TryGetValue(msgHdlType, out tuple))
                return tuple.Item2;

            return null;
        }

        public static SignInOptions GetSignInOptions<TMessageHandler>() where TMessageHandler : AbstractMessageHandler
        {
            return GetSignInOptions(typeof(TMessageHandler));
        }

        public static IDictionary<Type, Tuple<Type, SignInOptions>> Registry
        {
            get;
            protected set;
        } = new Dictionary<Type, Tuple<Type, SignInOptions>>();

        public virtual MessageHandlerContainer Refresh()
        {
            lock (this)
            {
                foreach (var key in Registry.Keys.Except(_messageHandlerMap.Keys))
                {
                    _messageHandlerMap[key] =
                        Activator.CreateInstance(Registry[key].Item1) as AbstractMessageHandler;
                }
            }

            return this;
        }


        public virtual AbstractMessageHandler Get(Type type)
        {
            AbstractMessageHandler msgHdl;
            if (!_messageHandlerMap.TryGetValue(type, out msgHdl) &&
                Registry.ContainsKey(type))
            {
                lock (this)
                {
                    Tuple<Type, SignInOptions> tuple;
                    if (Registry.TryGetValue(type, out tuple))
                    {
                        msgHdl = Activator.CreateInstance(tuple.Item1) as AbstractMessageHandler;
                        _messageHandlerMap[type] = msgHdl;
                    }
                }
            }

            return msgHdl;
        }

        public virtual TMessageHandler Get<TMessageHandler>() where TMessageHandler : AbstractMessageHandler
        {
            return Get(typeof(TMessageHandler)) as TMessageHandler;
        }

        public IEnumerator<KeyValuePair<Type, AbstractMessageHandler>> GetEnumerator()
        {
            return _messageHandlerMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _messageHandlerMap.GetEnumerator();
        }
    }
}
