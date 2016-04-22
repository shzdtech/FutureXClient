using System;

namespace Micro.Future.Message
{
    public abstract class AbstractMessageClient : IMessageClient
    {
        public event Action<uint, byte[]> OnMessageRecv;

        public event Action<Exception> OnConnected;

        public event Action<Exception> OnDisconnected;

        public event Action OnClosed;

        ~AbstractMessageClient()
        {
            Close();
        }

        public static IMessageClient NewInstance
        {
            get
            {
                return new TCPMessageClient();
            }
        }

        public abstract void Connect();

        public abstract void Close();

        public abstract long WriteMessage(uint msgId, byte[] message);

        public virtual bool IsListening
        {
            get;
            protected set;
        }
        public virtual bool IsConnected
        {
            get;
            protected set;
        }

        public string ConnectionString { get; set; }

        public abstract void StartListen();

        public abstract void StopListen();

        protected virtual void RaiseOnConnected(Exception ex)
        {
            if (ex == null)
                IsConnected = true;

            if (OnConnected != null)
                OnConnected(ex);
        }
        protected virtual void RaiseOnDisconnected(Exception ex)
        {
            if (IsConnected && OnDisconnected != null)
            {
                IsConnected = false;
                IsListening = false;
                OnDisconnected(ex);
            }
        }

        protected virtual void RaiseOnClosed()
        {
            if (OnClosed != null)
                OnClosed();
        }

        protected virtual void RaiseOnMessageRecv(uint messageId, byte[] data)
        {
            if (OnMessageRecv != null)
                OnMessageRecv(messageId, data);
        }
    }
}
