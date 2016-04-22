using System;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace Micro.Future.Message
{
    public class TCPMessageClient : AbstractMessageClient
    {
        public const byte STH = 0x01;
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte ETB = 0x17;
        public const uint HEADER_SIZE = 6;
        public const uint EXINFO_SIZE = 6;
        public static readonly char[] SPLIT_CHAR = { ':', '/' };
        private byte[] header = new byte[HEADER_SIZE];
        private byte[] tail = new byte[EXINFO_SIZE];
        private NetworkStream _currentStream;

        private class InnerState
        {
            public InnerState() { }
            public InnerState(NetworkStream netStream)
            {
                NetStream = netStream;
            }

            public byte[] Buffer;
            public NetworkStream NetStream;
        }

        #region Operations
        public override void Connect()
        {
            string[] address = ConnectionString.Split(SPLIT_CHAR);
            long len = address.Length;
            if (len >= 2)
            {
                string host = address[len - 2];
                int port = int.Parse(address[len - 1]);
                Connect(host, port);
            }
        }

        public void Connect(string hostname, int port)
        {
            lock (this)
            {
                if (!IsConnected)
                {
                    TcpClient tcpclient = new TcpClient();
                    tcpclient.BeginConnect(hostname, port, AsyncConnectedCallback, tcpclient);
                }
            }
        }


        public override void StartListen()
        {
            lock (this)
            {
                if (IsConnected && !IsListening)
                {
                    IsListening = true;
                    AsyncRecvMsgHeader(new InnerState(_currentStream));
                }
            }
        }

        public override void StopListen()
        {
            IsListening = false;
        }

        public override void Close()
        {
            lock (this)
            {
                IsListening = false;
                if (IsConnected)
                {
                    IsConnected = false;

                    _currentStream.Close();
                    _currentStream.Dispose();
                    _currentStream = null;

                    RaiseOnClosed();
                }
            }
        }

        public override long WriteMessage(uint msgId, byte[] data)
        {
            long packetLen = 0;
            if (IsConnected)
            {
                packetLen = HEADER_SIZE + data.Length + EXINFO_SIZE;
                byte[] buffer = new byte[packetLen];
                //Header
                long contentLen = data.Length + EXINFO_SIZE;
                buffer[0] = STH;
                buffer[1] = (byte)contentLen;
                buffer[2] = (byte)(contentLen >> 8);
                buffer[3] = (byte)(contentLen >> 16);
                buffer[4] = (byte)(contentLen >> 24);
                buffer[HEADER_SIZE - 1] = STX;

                data.CopyTo(buffer, HEADER_SIZE);

                //Extra Info
                long bias = HEADER_SIZE + data.Length;
                buffer[bias] = ETX;
                buffer[bias + 1] = (byte)msgId;
                buffer[bias + 2] = (byte)(msgId >> 8);
                buffer[bias + 3] = (byte)(msgId >> 16);
                buffer[bias + 4] = (byte)(msgId >> 24);
                buffer[bias + EXINFO_SIZE - 1] = ETB;

                if (_currentStream != null)
                    BeginWrite(_currentStream, buffer, 0, buffer.Length,
                        delegate (IAsyncResult ar)
                        {
                            EndWrite(ar);
                        }, _currentStream);
            }

            return packetLen;
        }

        #endregion

        #region Async Operations
        private void AsyncConnectedCallback(IAsyncResult ar)
        {
            Exception ex = null;
            try
            {
                TcpClient tcpclient = (TcpClient)ar.AsyncState;
                tcpclient.EndConnect(ar);
                _currentStream = tcpclient.GetStream();
                IsConnected = true;
            }
            catch (Exception sex)
            {
                ex = sex;
            }

            RaiseOnConnected(ex);
        }
        private void AsyncRecvMsgHeader(InnerState innerState)
        {
            if (innerState.NetStream != null)
                BeginRead(innerState.NetStream, header, 0, header.Length,
                    AsyncRecvMsgHeaderCallback, innerState);
        }

        private void AsyncRecvMsgHeaderCallback(IAsyncResult ar)
        {
            if (EndRead(ar) == HEADER_SIZE)
            {
                if (header[0] == STH && header[HEADER_SIZE - 1] == STX)
                {
                    uint contentLen = (uint)(header[1] | header[2] << 8 | header[3] << 16 | header[4] << 24);

                    var innerState = (InnerState)ar.AsyncState;
                    innerState.Buffer = new byte[contentLen - EXINFO_SIZE];

                    if (innerState.NetStream != null)
                        BeginRead(innerState.NetStream, innerState.Buffer, 0, innerState.Buffer.Length,
                            AsyncRecvMsgContentCallback, innerState);
                }
            }
        }

        private void AsyncRecvMsgContentCallback(IAsyncResult ar)
        {
            if (EndRead(ar) >= 0)
            {
                var innerState = (InnerState)ar.AsyncState;
                if (innerState.NetStream != null)
                    BeginRead(innerState.NetStream, tail, 0, tail.Length,
                        AsyncRecvMsgTailCallBack, innerState);
            }
        }

        private void AsyncRecvMsgTailCallBack(IAsyncResult ar)
        {
            if (EndRead(ar) == EXINFO_SIZE)
            {
                if (tail[0] == ETX && tail[EXINFO_SIZE - 1] == ETB)
                {
                    uint msgId = (uint)(tail[1] | tail[2] << 8 | tail[3] << 16 | tail[4] << 24);

                    var innerState = (InnerState)ar.AsyncState;

                    if (innerState.NetStream != null)
                    {
                        var newState = new InnerState(innerState.NetStream);
                        AsyncRecvMsgHeader(newState);

                        RaiseOnMessageRecv(msgId, innerState.Buffer);
                    }
                }
            }
        }
        #endregion

        #region Wrapped Functions
        private IAsyncResult BeginRead(NetworkStream netStream, byte[] buffer, int offset, int size,
          AsyncCallback callback, object state)
        {
            IAsyncResult ar = null;
            try
            {
                ar = netStream.BeginRead(buffer, offset, size,
                    callback, state);
            }
            catch (IOException ex)
            {
                RaiseOnDisconnected(ex);
            }
            catch (Exception)
            {

            }

            return ar;
        }

        private int EndRead(IAsyncResult ar)
        {
            int nb = 0;
            try
            {
                var innerState = (InnerState)ar.AsyncState;
                if (innerState.NetStream != null)
                {
                    nb = innerState.NetStream.EndRead(ar);
                    if (nb == 0 && innerState.Buffer != null && nb != innerState.Buffer.Length)
                    {
                        RaiseOnDisconnected(new IOException("Zero byte recieved."));
                    }
                }
            }
            catch (IOException ex)
            {
                RaiseOnDisconnected(ex);
            }
            catch (Exception)
            {

            }

            return nb;
        }


        private IAsyncResult BeginWrite(NetworkStream netStream, byte[] buffer, int offset, int size,
            AsyncCallback callback, object state)
        {
            IAsyncResult ar = null;
            try
            {
                ar = netStream.BeginWrite(buffer, offset, size,
                    callback, state);
            }
            catch (IOException ex)
            {
                RaiseOnDisconnected(ex);
            }
            catch (Exception)
            {

            }

            return ar;
        }

        private void EndWrite(IAsyncResult ar)
        {
            try
            {
                var netStream = (NetworkStream)ar.AsyncState;
                if (netStream != null)
                    netStream.EndWrite(ar);
            }
            catch (IOException ex)
            {
                RaiseOnDisconnected(ex);
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}

