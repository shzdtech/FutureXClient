using System;
using System.Windows;
using Micro.Future.Utility;
using Micro.Future.Message;
using System.Collections;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Micro.Future.CustomizedControls.Windows;

namespace Micro.Future.CustomizedControls
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindowTest: Window
    {
        public event Action<LoginWindow, IUserInfo> OnLogged;
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

        public LoginWindowTest(AbstractSignInManager signInMgr)
        {
            SignInManager = signInMgr;
            SignInManager.OnLogged += OnLogSuccess;
            SignInManager.OnLoginError += OnLoginError;

            InitializeComponent();

            var userInfo = signInMgr.SignInOptions;

            LoginCombo.Text = userInfo.FrontServer;
            userTxt.Text = userInfo.UserName;
            passwordTxt.Password = userInfo.Password;
            //LanguageCombo. = 
        }

        private void OnLoginError(MessageException ex)
        {
            if (ex != null)
            {
                loginBtn.IsEnabled = true;
                MessageBox.Show(this, ex.Message);
            }
        }

        private void OnLogSuccess(IUserInfo userinfo)
        {
            //OnLogged?.Invoke(this, userinfo);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string frontserver = LoginCombo.Text;
            string brokerId = userBroker.Text;
            string uid = userTxt.Text;
            string password = passwordTxt.Password;

            if (SignInManager.SignInOptions.FrontServer != frontserver ||
                SignInManager.SignInOptions.BrokerID != brokerId ||
                SignInManager.SignInOptions.UserName != uid ||
                SignInManager.SignInOptions.Password != password)
            {
                SignInManager.SignInOptions.FrontServer = frontserver;
                SignInManager.SignInOptions.BrokerID = brokerId;
                SignInManager.SignInOptions.UserName = uid;
                if (SignInManager.SignInOptions.EncryptPassword)
                {
                    _hashEncoder.Option.Iteration = MD5Round;
                    password = _hashEncoder.Encode(password);
                }

                SignInManager.SignInOptions.Password = password;
            }

            SignInManager.SignIn();

            loginBtn.IsEnabled = false;
        }

        public void ReportStatus(string statusMsg)
        {
            Dispatcher.Invoke(() => DataLoadingStatus.Text = statusMsg);
        }
    }
}
