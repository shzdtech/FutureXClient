using System;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Micro.Future.Message;
using Micro.Future.Util;
using Micro.Future.Properties;
using System.Threading;
using Xceed.Wpf.AvalonDock.Layout;
using System.Collections.Generic;
using WPFLocalizeExtension;
using Micro.Future.UI;

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
            MessageBox.Show(this, errRsult.Message, WPFUtility.GetLocalizedString("Error", "Resource"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OTCClient_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, WPFUtility.GetLocalizedString("ReConnect", "Resource"), "服务器连接已断开", MessageBoxButton.OK, MessageBoxImage.Information);
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
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("Optional", "Resource");
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter( "CFFEX","","" );
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CFFEX", "Resource");
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("SHFE","","");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("SHFE", "Resource");
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("DCE","","");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("DCE", "Resource");
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("CZCE","","");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CZCE", "Resource");
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllExecution", "Resource");
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Opening(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENNING });
            ancable.Title = WPFUtility.GetLocalizedString("Opening", "Resource");
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED});
            ancable.Title = WPFUtility.GetLocalizedString("Traded", "Resource");
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Trade(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content =tradeWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllTraded", "Resource");
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
            ancable.Title = WPFUtility.GetLocalizedString("Open", "Resource");
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.CLOSE });
            ancable.Title = WPFUtility.GetLocalizedString("Close", "Resource");
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var positionWin = new ClientPositionWindow() { LayoutContent = ancable };
            ancable.Content = positionWin;
            ancable.Title = WPFUtility.GetLocalizedString("Position", "Resource");
            positionPane.Children.Add(ancable);
        }


        private void FastOrder_Click(object sender, RoutedEventArgs e)
        {
            FastOrderWin fastOrderWindow = new FastOrderWin();
            fastOrderWindow.Show();
            otcMarketDataLV.OnQuoteSelected += fastOrderWindow.OnQuoteSelected;
            positionsWindow.OnPositionSelected += fastOrderWindow.OnPositionSelected;
        }
    }
}
