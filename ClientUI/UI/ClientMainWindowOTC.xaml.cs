using System;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Micro.Future.Message;
using Micro.Future.Util;
using Micro.Future.Properties;
using System.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientMainWindowOTC : RibbonWindow
    {
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _otcClientSignIner = new PBSignInManager();
        private PBSignInManager _ctpMdSignIner = new PBSignInManager();
        private PBSignInManager _ctpTradeSignIner = new PBSignInManager();
        public ClientMainWindowOTC()
        {
            InitializeComponent();
            Initialize();
            Login();
        }

        public void Initialize()
        {
            // UI initialization
            otcMarketDataLV.OnQuoteSelected += FastOrderCtl.OnQuoteSelected;
            positionsWindow.OnPositionSelected += FastOrderCtl.OnPositionSelected;

            //
            var msgWrapper = _otcClientSignIner.MessageWrapper;

            msgWrapper.MessageClient.OnDisconnected += OTCClient_OnDisconnected;

            _otcClientSignIner.OnLoginError += OnErrorMessageRecv;
            _otcClientSignIner.OnLogged += _otcClientSignIner_OnLogged;

            msgWrapper.MessageClient.OnDisconnected += loginStatus.OnDisconnected;
            _otcClientSignIner.OnLogged += loginStatus.OnLogged;
            _otcClientSignIner.OnLoginError += loginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().OnError += OnErrorMessageRecv;

            // Initialize Market Data
            msgWrapper = _ctpMdSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += MD_OnDisconnected;

            _ctpMdSignIner.OnLoginError += OnErrorMessageRecv;
            _ctpMdSignIner.OnLogged += _ctpMdSignIner_OnLogged;

            _ctpMdSignIner.OnLogged += ctpLoginStatus.OnLogged;
            _ctpMdSignIner.OnLoginError += ctpLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().OnError += OnErrorMessageRecv;

            // Initialize Trading Server
            msgWrapper = _ctpTradeSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += TD_OnDisconnected;

            _ctpTradeSignIner.OnLoginError += OnErrorMessageRecv;

            _ctpTradeSignIner.OnLogged += ctpTradeLoginStatus.OnLogged;
            _ctpTradeSignIner.OnLoginError += ctpTradeLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpTradeLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OnError += OnErrorMessageRecv;
        }

        private void _ctpMdSignIner_OnLogged(IUserInfo obj)
        {
            clientFundLV.ReloadData();
            Thread.Sleep(1500);
            positionsWindow.ReloadData();
            Thread.Sleep(1500);
            tradeWindow.ReloadData();
            Thread.Sleep(1500);
            executionWindow.ReloadData();
        }

        void _otcClientSignIner_OnLogged(IUserInfo obj)
        {
            RightDownStatus.Content = "欢迎" + obj.LastName + obj.FirstName;
        }

        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(this, errRsult.Message, "发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OTCClient_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, "请尝试重新登陆", "服务器连接已断开", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private bool Login()
        {
            LoginWindow loginWindow = new LoginWindow(_otcClientSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["OTCSERVER.ADDRESS"].Values
            };
            loginWindow.ShowDialog();

            MDServerLogin();

            TradingServerLogin();

            return true;
        }

        void MD_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, "请点击状态栏中的连接按钮尝试重新连接", "行情服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void TD_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, "请点击状态栏中的连接按钮尝试重新连接", "Trading服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MDServerLogin()
        {
            var mdCfg = _config.Content["MDSERVER"];
            _ctpMdSignIner.SignInOptions.FrontServer = mdCfg["ADDRESS"];
            _ctpMdSignIner.SignInOptions.BrokerID = mdCfg["BROKERID"];
            _ctpMdSignIner.SignInOptions.UserID = mdCfg["USERID"];
            _ctpMdSignIner.SignInOptions.Password = mdCfg["PASSWORD"];
            _ctpMdSignIner.SignIn();

            ctpLoginStatus.Prompt = "正在连接CTP Market服务器...";
        }

        private void TradingServerLogin()
        {
            var mdCfg = _config.Content["TRADESERVER"];
            _ctpTradeSignIner.SignInOptions.FrontServer = mdCfg["ADDRESS"];
            _ctpTradeSignIner.SignInOptions.BrokerID = mdCfg["BROKERID"];
            _ctpTradeSignIner.SignInOptions.UserID = mdCfg["USERID"];
            _ctpTradeSignIner.SignInOptions.Password = mdCfg["PASSWORD"];
            _ctpTradeSignIner.SignIn();

            ctpLoginStatus.Prompt = "正在连接CTP Trading服务器...";
        }

        private void RibbonLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void loginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            Login();
        }

        private void ctpLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MDServerLogin();
        }

        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
        }

        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientQuoteGroupView();
            ancable.Title = "自选合约";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView();
            quoteGrpVw.FilterByExchange(null);
            ancable.Content = quoteGrpVw;
            ancable.Title = "中金期货";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientQuoteGroupView();
            ancable.Title = "上海期货";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientQuoteGroupView();
            ancable.Title = "大连期货";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientQuoteGroupView();
            ancable.Title = "郑州期货";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientExecutionWindow();
            ancable.Title = "所有委托单";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Opening(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientExecutionWindow();
            ancable.Title = "挂单";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientExecutionWindow();
            ancable.Title = "已成交";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Trade(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientTradeWindow();
            ancable.Title = "所有成交记录";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientTradeWindow();
            ancable.Title = "开仓记录";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientTradeWindow();
            ancable.Title = "平仓记录";
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            ancable.Content = new ClientPositionWindow();
            ancable.Title = "持仓";
            positionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Exchange(string exchange, string title)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView();
            //quoteGrpVw.filter
            ancable.Content = quoteGrpVw;
            ancable.Title = "中金期货";
            tradePane.Children.Add(ancable);
        }
    }
}
