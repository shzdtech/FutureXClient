using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class SignInOptions
    {
        public SignInOptions() { }
        public SignInOptions(SignInOptions signInOpt)
        {
            NeedSignIn = signInOpt.NeedSignIn;
            MainAccount = signInOpt.MainAccount;
            BrokerID = signInOpt.BrokerID;
            UserID = signInOpt.UserID;
            Password = signInOpt.Password;
            FrontServer = signInOpt.FrontServer;
            FrontID = signInOpt.FrontID;
            SessionID = signInOpt.SessionID;
            Heartbeat = signInOpt.Heartbeat;
        }

        public bool MainAccount { get; set; }
        public bool NeedSignIn { get; set; } = true;
        public string BrokerID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string FrontServer { get; set; }
        public long Heartbeat { get; set; } = 30000;
        public int? FrontID { get; set; }
        public int? SessionID { get; set; }
    }

    public interface ISignInManagerFactory
    {
        AbstractSignInManager CreateSignInManage(SignInOptions signInOptions);
    }

    public abstract class AbstractSignInManager
    {
        public event Action<Exception> OnConnected;
        public event Action OnSessionCreated;
        public event Action<UserInfo> OnLogged;
        public event Action<MessageException> OnLoginError;

        public AbstractMessageWrapper MessageWrapper { get; protected set; }

        public SignInOptions SignInOptions { get; protected set; }
        public AbstractSignInManager()
        {
            SignInOptions = new SignInOptions();
            Reset();
        }
        public AbstractSignInManager(SignInOptions signInOptions)
        {
            SignInOptions = new SignInOptions(signInOptions);
            Reset();
        }

        protected virtual void Reset()
        {
            IsSessionCreated = false;
            MessageWrapper = CreateMessageWrapper();
        }

        protected abstract AbstractMessageWrapper CreateMessageWrapper();

        public bool ConnectWithSignIn
        {
            get;
            set;
        }

        public void RaiseOnConnected(Exception ex)
        {
            if (OnConnected != null)
                OnConnected(ex);
        }

        public void RaiseOnSessionCreated()
        {
            if (OnSessionCreated != null)
                OnSessionCreated();
        }

        public void RaiseOnLogged(UserInfo userInfo)
        {
            if (OnLogged != null)
                OnLogged(userInfo);
        }

        public void RaiseOnLoginError(MessageException ex)
        {
            if (OnLoginError != null)
                OnLoginError(ex);
        }
        public bool IsSessionCreated
        {
            get;
            protected set;
        }

        abstract public void ConnectServer();

        public virtual Task<Exception> ConnectServerAsync()
        {
            lock (this)
            {
                TaskCompletionSource<Exception> tcs = new TaskCompletionSource<Exception>();

                #region callback
                OnConnected += (ex) =>
                {
                    if (ex != null)
                    {
                        tcs.TrySetResult(ex);
                    }
                };

                OnSessionCreated += () =>
                {
                    IsSessionCreated = true;
                    tcs.TrySetResult(null);
                };
                #endregion

                ConnectServer();

                return tcs.Task;
            }
        }

        abstract public void SignIn();


        public virtual async Task<TaskResult<UserInfo, Exception>> SignInAsync()
        {
            Exception ex = null;

            if (!IsSessionCreated)
                ex = await ConnectServerAsync();

            if (ex != null)
                return new TaskResult<UserInfo, Exception>(ex);

            TaskCompletionSource<TaskResult<UserInfo, Exception>> tcs =
                new TaskCompletionSource<TaskResult<UserInfo, Exception>>();

            #region callback
            OnLogged += (userInfo) =>
            {
                tcs.TrySetResult(new TaskResult<UserInfo, Exception>(userInfo));
            };

            OnLoginError += (exeption) =>
            {
                tcs.TrySetResult(new TaskResult<UserInfo, Exception>(exeption));
            };
            #endregion

            SignIn();

            return await tcs.Task;
        }
    }
}
