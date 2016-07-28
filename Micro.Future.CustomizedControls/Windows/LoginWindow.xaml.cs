using System;
using System.Windows;
using Micro.Future.Utility;
using Micro.Future.Message;
using System.Collections;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Micro.Future.UI
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public AbstractSignInManager SignInManager
        {
            get; protected set;
        }

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

        public LoginWindow(AbstractSignInManager signInMgr)
        {
            SignInManager = signInMgr;
            SignInManager.OnConnected += _signInMgr_OnConnected;
            SignInManager.OnSessionCreated += _signInMgr_OnSessionCreated;
            InitializeComponent();

            var userInfo = signInMgr.SignInOptions;

            LoginCombo.Text = userInfo.FrontServer;
            brokerTxt.Text = userInfo.BrokerID;
            userTxt.Text = userInfo.UserName;
            passwordTxt.Password = userInfo.Password;
        }

        private void _signInMgr_OnSessionCreated()
        {
            Close();
        }

        void _signInMgr_OnConnected(Exception ex)
        {
            if (ex != null)
            {
                Title = ex.Message;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string frontserver = LoginCombo.Text;
            string brokerid = brokerTxt.Text;
            string uid = userTxt.Text;
            string password = passwordTxt.Password;

            if (SignInManager.SignInOptions.FrontServer != frontserver ||
                SignInManager.SignInOptions.BrokerID != brokerid ||
                SignInManager.SignInOptions.UserName != uid ||
                SignInManager.SignInOptions.Password != password)
            {
                SignInManager.SignInOptions.FrontServer = frontserver;
                SignInManager.SignInOptions.BrokerID = brokerid;
                SignInManager.SignInOptions.UserName = uid;
                if (MD5Round > 0)
                {
                    _hashEncoder.Option.Iteration = MD5Round;
                    password = _hashEncoder.Encode(password);
                }

                SignInManager.SignInOptions.Password = password;
            }

            SignInManager.SignIn();
        }
    }
}
