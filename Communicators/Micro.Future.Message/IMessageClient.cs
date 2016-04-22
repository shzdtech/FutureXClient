using System;

namespace Micro.Future.Message
{
    public interface IMessageClient
    {
        void Connect();

        void StartListen();

        void StopListen();

        void Close();
        long WriteMessage(uint msgId, byte[] message);
        bool IsConnected { get; }
        bool IsListening { get; }
        string ConnectionString { get; set; }

        event Action<uint, byte[]> OnMessageRecv;
        event Action<Exception> OnConnected;
        event Action<Exception> OnDisconnected;
        event Action OnClosed;
    }
}

