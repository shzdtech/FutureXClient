using System;
using System.Windows;
using Micro.Future.Util;
using Micro.Future.Message;
using System.Collections;
using System.Security.Cryptography;

namespace Micro.Future.UI
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private PBSignInManager _signInMgr;
        private HashEncoder<HashEncoderOption>
            _hashEncoder = new HashEncoder<HashEncoderOption>(new MD5CryptoServiceProvider());

        public uint MD5Round
        {
            get;
            set;
        }
        public bool Succeeded
        {
            get;
            private set;
        }

        public IEnumerable AddressCollection
        {
            set
            {
                LoginCombo.ItemsSource = value;
            }
        }

        public LoginWindow(PBSignInManager signInMgr)
        {
            _signInMgr = signInMgr;
            _signInMgr.OnConnected += _signInMgr_OnConnected;
            _signInMgr.OnSessionCreated += _signInMgr_OnSessionCreated;
            InitializeComponent();

            var userInfo = signInMgr.SignInOptions;

            LoginCombo.Text = userInfo.FrontServer;
            brokerTxt.Text = userInfo.BrokerID;
            userTxt.Text = userInfo.UserID;
            passwordTxt.Password = userInfo.Password;
        }

        private void _signInMgr_OnSessionCreated()
        {
            Dispatcher.Invoke(
                    () =>
                    {
                        Close();
                    });
        }

        void _signInMgr_OnConnected(Exception ex)
        {
            if (ex != null)
            {
                Dispatcher.Invoke(
                     () =>
                     {
                         Title = ex.Message;
                     });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string frontserver = LoginCombo.Text;
            string brokerid = brokerTxt.Text;
            string uid = userTxt.Text;
            string password = passwordTxt.Password;

            if (!_signInMgr.MessageWrapper.HasSignIn ||
                _signInMgr.SignInOptions.FrontServer != frontserver ||
                _signInMgr.SignInOptions.BrokerID != brokerid ||
                _signInMgr.SignInOptions.UserID != uid ||
                _signInMgr.SignInOptions.Password != password)
            {
                SignInOptions loginInfo = _signInMgr.SignInOptions;
                loginInfo.FrontServer = frontserver;
                loginInfo.BrokerID = brokerid;
                loginInfo.UserID = uid;
                if (MD5Round > 0)
                {
                    _hashEncoder.Option.Iteration = MD5Round;
                    _hashEncoder.Encode(password);
                }
                else
                    loginInfo.Password = password;
            }

            _signInMgr.SignIn();

            if (_signInMgr.IsSessionCreated)
                Close();
        }
    }
}
