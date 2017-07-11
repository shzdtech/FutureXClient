using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.UI;
using Micro.Future.LocalStorage;
using Micro.Future;
using System;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.Message;
using System.Security.Cryptography;

namespace Micro.Future.Message
{
    public class AccountHandler : AbstractMessageHandler
    {
        private HashEncoder<HashEncoderOption> _hashEncoder =
            new HashEncoder<HashEncoderOption>(MD5.Create(),
               (md5, byteArray) =>
               {
                   return ((MD5)md5).ComputeHash(byteArray);
               }
               );

        public uint MD5Round
        {
            get;
            set;
        } = 2;

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            
        }


        Task<bool> ResetPassword(string newPassword, int timeout = 10000)
        {
            if (string.IsNullOrEmpty(newPassword))
                return Task.FromResult(false);

            var msgId = (uint)AccountManageMessageID.MSG_ID_RESET_PASSWORD;

            var tcs = new TaskCompletionSource<bool>(new CancellationTokenSource(timeout));

            var serialId = NextSerialId;

            var sst = new StringMap();

            _hashEncoder.Option.Iteration = MD5Round;
            sst.Entry[FieldName.PASSWORD] = _hashEncoder.Encode(newPassword);


            #region callback
            MessageWrapper.RegisterAction<Result, ExceptionMessage>
            (msgId,
            (resp) =>
            {
                if (resp?.SerialId == serialId)
                {
                    tcs.TrySetResult(true);
                }
            },
            (ExceptionMessage bizErr) =>
            {
                tcs.SetResult(false);
            }
            );
            #endregion

            MessageWrapper.SendMessage(msgId, sst);

            return tcs.Task;
        }
    }

}
