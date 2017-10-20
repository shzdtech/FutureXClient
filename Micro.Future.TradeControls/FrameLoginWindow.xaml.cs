using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;


namespace Micro.Future.CustomizedControls.Windows
{
    public partial class FrameLoginWindow : Window
    {
        public event Action<FrameLoginWindow, IUserInfo> OnLogged;
        public AbstractSignInManager SignInManager
        {
            get; protected set;
        }
        public AbstractSignInManager TDSignInManager
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
        public FrameLoginWindow(AbstractSignInManager signInMgr, AbstractSignInManager tdSignInMgr)
        {
            SignInManager = signInMgr;
            SignInManager.OnLogged += OnLogSuccess;
            SignInManager.OnLoginError += OnLoginError;

            TDSignInManager = tdSignInMgr;
            TDSignInManager.OnLogged += OnLogSuccess;
            TDSignInManager.OnLoginError += OnLoginError;

            InitializeComponent();

            var userInfo = signInMgr.SignInOptions;
            userTxt.Text = userInfo.UserName;
            passwordTxt.Password = userInfo.Password;
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
            OnLogged?.Invoke(this, userinfo);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string uid = userTxt.Text;
            string password = passwordTxt.Password;
            if (
                    SignInManager.SignInOptions.UserName != uid ||
                    SignInManager.SignInOptions.Password != password)
            {
                SignInManager.SignInOptions.UserName = uid;
                if (SignInManager.SignInOptions.EncryptPassword)
                {
                    _hashEncoder.Option.Iteration = MD5Round;
                    password = _hashEncoder.Encode(password);
                }

                SignInManager.SignInOptions.Password = password;
            }

            SignInManager.SignIn();

            if (
                    TDSignInManager.SignInOptions.UserName != uid ||
                    TDSignInManager.SignInOptions.Password != password)
            {
                TDSignInManager.SignInOptions.UserName = uid;
                if (TDSignInManager.SignInOptions.EncryptPassword)
                {
                    _hashEncoder.Option.Iteration = MD5Round;
                    password = _hashEncoder.Encode(password);
                }

                TDSignInManager.SignInOptions.Password = password;
            }

            TDSignInManager.SignIn();

            loginBtn.IsEnabled = false;
        }
    }
}
