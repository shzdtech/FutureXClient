using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Util;
using System;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Micro.Future.UI
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class OTCTradingDesk : RibbonWindow
    {
        private PBSignInManager _otcSignIner = new PBSignInManager();
        private PBSignInManager _ctpMdSignIner = new PBSignInManager();
        private Config _config = new Config(Settings.Default.ConfigFile);

        public OTCTradingDesk()
        {
            InitializeComponent();
            Initialize();
            Login();
        }

        private void Initialize()
        {
            var msgWrapper = _otcSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += OTC_OnDisconnected;

            _otcSignIner.OnLoginError += OnErrorMessageRecv;
            _otcSignIner.OnLogged += OTC_OnLogged;

            _otcSignIner.OnLogged += loginStatus.OnLogged;
            _otcSignIner.OnLoginError += loginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += loginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().OnError += OnErrorMessageRecv;

            // Initialize Market Data
            msgWrapper = _ctpMdSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += MD_OnDisconnected;

            _ctpMdSignIner.OnLoginError += OnErrorMessageRecv;

            _ctpMdSignIner.OnLogged += ctpStatus.OnLogged;
            _ctpMdSignIner.OnConnected += ctpStatus.OnDisconnected;
            _ctpMdSignIner.OnLoginError += ctpStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().OnError += OnErrorMessageRecv;
        }

        void OTC_OnLogged(IUserInfo obj)
        {
            RightDownStatus.Content = "欢迎" + obj.LastName + obj.FirstName;
            strategyListView.ReloadData();
            contractParamListView.ReloadData();
            otcMarketDataLV.ReloadData();
        }

        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(this, errRsult.Message, "发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OTC_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, "请尝试重新登陆", "服务器连接已断开", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void MD_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, "请点击状态栏中的连接按钮尝试重新连接", "行情服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool Login()
        {
            LoginWindow loginWindow = new LoginWindow(_otcSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["OTCSERVER.ADDRESS"].Values
            };

            loginWindow.ShowDialog();

            MDServerLogin();

            return true;
        }

        private void MDServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                var mdCfg = _config.Content["MDSERVER"];
                _ctpMdSignIner.SignInOptions.FrontServer = mdCfg["ADDRESS"];
                _ctpMdSignIner.SignIn();

                ctpStatus.Prompt = "正在连接CTP服务器...";
            }
        }

        private void RibbonLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void loginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            Login();
        }

        private void ctpStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MDServerLogin();
        }
    }
}
