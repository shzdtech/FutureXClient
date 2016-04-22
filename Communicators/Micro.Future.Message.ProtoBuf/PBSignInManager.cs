using Micro.Future.Message;
using Micro.Future.Message.Business;
using Micro.Future.Message.PBMessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class PBSignInManager : AbstractSignInManager
    {
        public PBSignInManager()
        {
            RegisterCallbacks();
        }
        public PBSignInManager(SignInOptions signInOptions) : base(signInOptions)
        {
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            MessageWrapper.MessageClient.OnConnected += OnServerConnected;
            MessageWrapper.MessageClient.OnDisconnected += OnServerDisconnected;

            MessageWrapper.RegisterAction<PBUserInfo, BizErrorMsg>
                ((uint)SystemMessageID.MSG_ID_LOGIN, LoginSuccessAction, LoginErrorMsgAction);
            MessageWrapper.RegisterAction<RawBuffer, RawBuffer>
                ((uint)SystemMessageID.MSG_ID_SESSION_CREATED, SessionCreated, null);
        }

        void OnServerDisconnected(Exception obj)
        {
            IsSessionCreated = false;
            MessageWrapper.HasSignIn = false;
        }

        public override void ConnectServer()
        {
            IsSessionCreated = false;
            MessageWrapper.MessageClient.ConnectionString = SignInOptions.FrontServer;
            MessageWrapper.MessageClient.Connect();
        }

        void OnServerConnected(Exception ex)
        {
            if (ex == null)
                MessageWrapper.MessageClient.StartListen();
            else if (ConnectWithSignIn)
            {
                RaiseOnLoginError(
                    new MessageException((uint)SystemMessageID.MSG_ID_LOGIN,
                    ex.HResult,
                    ex.Message,
                    ex.HResult)
                );
            }

            RaiseOnConnected(ex);
        }

        private void SessionCreated(RawBuffer rb)
        {
            IsSessionCreated = true;

            RaiseOnSessionCreated();

            if (ConnectWithSignIn)
            {
                SignIn();
            }
        }

        public override void SignIn()
        {
            if (!IsSessionCreated)
            {
                ConnectWithSignIn = true;
                ConnectServer();
            }
            else
            {
                var bid = NamedStringVector.CreateBuilder();
                bid.SetName(FieldName.BROKER_ID);
                bid.AddEntry(SignInOptions.BrokerID ?? string.Empty);
                var uid = NamedStringVector.CreateBuilder();
                uid.SetName(FieldName.USER_ID);
                uid.AddEntry(SignInOptions.UserID ?? string.Empty);
                var psw = NamedStringVector.CreateBuilder();
                psw.SetName(FieldName.PASSWORD);
                psw.AddEntry(SignInOptions.Password ?? string.Empty);

                var sst = SimpleStringTable.CreateBuilder();
                sst.AddColumns(bid).AddColumns(uid).AddColumns(psw);

                MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_LOGIN, sst.Build());
            }
        }

        private void LoginSuccessAction(PBUserInfo st)
        {
            MessageWrapper.HasSignIn = true;
            MessageWrapper.User = new UserInfo()
            {
                ID = st.UserId,
                Name = st.Name,
                Email = st.Email,
                Company = st.Company,
                ContactNumber = st.ContactNum,
                Role = (RoleType)st.Role,
                Permission = st.Permission,
            };

            if (SignInOptions.Heartbeat > 0)
                MessageWrapper.StartSendHeartBeat(SignInOptions.Heartbeat);

            RaiseOnLogged(MessageWrapper.User);
        }

        private void LoginErrorMsgAction(BizErrorMsg bizErr)
        {
            RaiseOnLoginError(
                new MessageException(bizErr.MessageId,
                bizErr.Errorcode,
                Encoding.UTF8.GetString(bizErr.Description.ToByteArray()),
                bizErr.Syserrcode)
                );
        }

        protected override AbstractMessageWrapper CreateMessageWrapper()
        {
            return new PBMessageWrapper();
        }
    }

    public class PBSignInManagerFactory : ISignInManagerFactory
    {
        public AbstractSignInManager CreateSignInManage(SignInOptions signInOptions)
        {
            return new PBSignInManager(signInOptions);
        }
    }
}
