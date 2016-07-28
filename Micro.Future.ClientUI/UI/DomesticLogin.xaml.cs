using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Micro.Future.UI
{
    /// <summary>
    /// DomesticLogin.xaml 的交互逻辑
    /// </summary>
    public partial class DomesticLogin : Window
    {
        private PBSignInManager _md_connhelper = new PBSignInManager();
        private PBSignInManager _trd_connhelper = new PBSignInManager();

        private const int TIME_OUT = 15000;

        public SignInOptions User
        {
            get;
            private set;
        }

        public bool IsSucceeded
        {
            get;
            private set;
        }

        public DomesticLogin(ICollection<string> mdServers,
            ICollection<string> trdServers)
        {
            InitializeComponent();

            this.MktFrontIDCombo.ItemsSource = mdServers;

            this.TrdFrontIDCombo.ItemsSource = trdServers;

            _md_connhelper.OnLogged += _md_connhelper_OnLogged;
            _md_connhelper.OnLoginError += _md_connhelper_OnError;

            _trd_connhelper.OnLogged += _trd_connhelper_OnLogged;
            _trd_connhelper.OnLoginError += _trd_connhelper_OnError;
        }

        void _trd_connhelper_OnLogged(IUserInfo obj)
        {
            IsSucceeded = true;
            Close();
        }

        void _trd_connhelper_OnError(MessageException mex)
        {
            this.Title = "交易服务器连接失败: " + mex.Message;
        }

        void _md_connhelper_OnError(MessageException mex)
        {
            this.Title = "行情服务器连接失败:" + mex.Message;
        }

        void _md_connhelper_OnLogged(IUserInfo obj)
        {
            this.Title = "正在连接交易服务器...";
            _trd_connhelper.SignInOptions.BrokerID = brokerTxt.Text;
            _trd_connhelper.SignInOptions.UserName = userTxt.Text;
            _trd_connhelper.SignInOptions.Password = passwordTxt.Password;
            _trd_connhelper.SignInOptions.FrontServer = MktFrontIDCombo.Text;
            _trd_connhelper.SignIn();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _md_connhelper.SignInOptions.BrokerID = brokerTxt.Text;
            _md_connhelper.SignInOptions.UserName = userTxt.Text;
            _md_connhelper.SignInOptions.Password = passwordTxt.Password;
            _md_connhelper.SignInOptions.FrontServer = MktFrontIDCombo.Text;

            this.Title = "正在连接行情服务器...";
            _md_connhelper.SignIn();
        }
    }
}
