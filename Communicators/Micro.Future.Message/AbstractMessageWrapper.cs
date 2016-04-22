using System;
using System.Timers;

namespace Micro.Future.Message
{
    public abstract class AbstractMessageWrapper
    {
        public event Action<object> OnUnsolvedErr;

        public static readonly byte[] HEART_BEAT = { 0 };

        private Timer _timer = new Timer();

        private IMessageClient _msgClient;
        public virtual IMessageClient MessageClient
        {
            get
            {
                if (_msgClient == null)
                {
                    _msgClient = AbstractMessageClient.NewInstance;
                    _msgClient.OnMessageRecv += OnMessageRecv;
                }
                return _msgClient;
            }
            set
            {
                _msgClient = value;
                _msgClient.OnMessageRecv += OnMessageRecv;
            }
        }

        protected void RaiseOnUnsolvedErr(Object obj)
        {
            if (OnUnsolvedErr != null)
                OnUnsolvedErr(obj);
        }

        public bool HasSignIn { get; set; }
        public UserInfo User { get; set; }
        public abstract void RegisterAction<TS, TE>(uint msgId,
            Action<TS> action, Action<TE> errAction);

        public abstract long SendMessage(uint msgId, object message);

        protected abstract void OnMessageRecv(uint msgId, byte[] data);

        public bool AutoSendingHeartBeat
        {
            get
            {
                return _timer.Enabled;
            }
        }
        public void StartSendHeartBeat(double interval = 30000)
        {
            _timer.Interval = interval;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            SendHeartBeat();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendHeartBeat();
        }

        public void StopSendHeartBeat()
        {
            _timer.Enabled = false;
        }

        public void SendHeartBeat()
        {
            try
            {
                if (MessageClient.IsConnected)
                {
                    MessageClient.WriteMessage((uint)SystemMessageID.MSG_ID_ECHO, HEART_BEAT);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class RawBuffer
    {
        private byte[] _buffer;
        public RawBuffer(byte[] buffer)
        {
            _buffer = buffer;
        }

        public static RawBuffer ParseFrom(byte[] buffer)
        {
            return new RawBuffer(buffer);
        }

        public static implicit operator byte[] (RawBuffer rb)
        {
            return rb._buffer;
        }
    }
}
