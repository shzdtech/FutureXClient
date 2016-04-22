using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Google.ProtocolBuffers;
using Micro.Future.Message.Business;

namespace Micro.Future.Message.PBMessageHandler
{
    public class PBMessageWrapper : AbstractMessageWrapper
    {
        class ActionInfo
        {
            public ActionInfo(Action<object> action, MethodInfo parseFrom, Action<object> errAction)
            { this.Action = action; this.ParseFrom = parseFrom; this.ErrAction = errAction; }
            public Action<object> Action;
            public MethodInfo ParseFrom;
            public Action<object> ErrAction;
        }

        private IDictionary<uint, ActionInfo> _mapDecoder = new Dictionary<uint, ActionInfo>();


        public override void RegisterAction<TS, TE>(uint msgId,
            Action<TS> action, Action<TE> errAction)
        {
            MethodInfo parseFrom = typeof(TS).
                GetMethod("ParseFrom", BindingFlags.Public | BindingFlags.Static,
                null, new Type[] { typeof(byte[]) }, null);
            Action<object> sAction = s => action((TS)s);
            Action<object> eAction = null;
            if (errAction != null)
                eAction = e => errAction((TE)e);
            _mapDecoder[msgId] = new ActionInfo(sAction, parseFrom, eAction);
        }

        public override long SendMessage(uint msgId, object message)
        {
            IMessageLite pbmsg = message as IMessageLite;
            byte[] buff = message != null ? pbmsg.ToByteArray() : (byte[])message;
            return MessageClient.WriteMessage(msgId, buff);
        }

        protected override void OnMessageRecv(uint msgId, byte[] data)
        {
            ActionInfo sa;
            // Error Occured
            if (msgId == (uint)SystemMessageID.MSG_ID_ERROR)
            {
                BizErrorMsg bizErr = BizErrorMsg.ParseFrom(data);
                if (_mapDecoder.ContainsKey(bizErr.MessageId))
                {
                    var eAction = _mapDecoder[bizErr.MessageId].ErrAction;
                    if (eAction != null)
                        eAction(bizErr);
                    else
                        RaiseOnUnsolvedErr(bizErr);
                }
            }
            else if (_mapDecoder.TryGetValue(msgId, out sa))
            {
                Action<object> action = sa.Action;
                var pbObj = sa.ParseFrom.Invoke(null, new object[] { data });
                action(pbObj);
            }
        }
    }
}
