using System.Collections.Generic;
using System.Windows;
using Micro.Future.Message;
using System.ComponentModel;
using System.Windows.Controls;
using Micro.Future.CustomizedControls;
using System;
using System.Windows.Controls.Primitives;
using Micro.Future.Resources.Localization;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class DomesticMarketFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _ctpMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<MarketDataHandler>());
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());


        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }


        public void LoginAsync(string usernname, string password)
        {
            _ctpMdSignIner.SignInOptions.UserName = _ctpTradeSignIner.SignInOptions.UserName = usernname;
            _ctpMdSignIner.SignInOptions.Password = _ctpTradeSignIner.SignInOptions.Password = password;
            MarketDataServerLogin();
            TradingServerLogin();
        }

        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public DomesticMarketFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
            }
        }

        public void Initialize()
        {
            // Initialize Market Data
            var msgWrapper = _ctpMdSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += MD_OnDisconnected;

            _ctpMdSignIner.OnLoginError += OnErrorMessageRecv;
            _ctpMdSignIner.OnLogged += ctpLoginStatus.OnLogged;
            _ctpMdSignIner.OnLoginError += ctpLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().OnError += OnErrorMessageRecv;

            // Initialize Trading Server
            msgWrapper = _ctpTradeSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += TD_OnDisconnected;

            _ctpTradeSignIner.OnLoginError += OnErrorMessageRecv;
            _ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;
            _ctpTradeSignIner.OnLogged += ctpTradeLoginStatus.OnLogged;
            _ctpTradeSignIner.OnLoginError += ctpTradeLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpTradeLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OnError += OnErrorMessageRecv;
        }


        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(errRsult.Message, WPFUtility.GetLocalizedString("Error", LocalizationInfo.ResourceFile), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void MD_OnDisconnected(Exception ex)
        {
            MessageBox.Show("请点击状态栏中的连接按钮尝试重新连接", "行情服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void TD_OnDisconnected(Exception ex)
        {
            MessageBox.Show("请点击状态栏中的连接按钮尝试重新连接", "Trading服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MarketDataServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                ctpLoginStatus.Prompt = "正在连接CTP行情服务器...";
                _ctpMdSignIner.SignIn();
            }
        }

        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                ctpTradeLoginStatus.Prompt = "正在连接CTP交易服务器...";
                _ctpTradeSignIner.SignIn();
            }
        }

        private void ctpLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MarketDataServerLogin();
        }

        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
        }


        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            clientFundLV.ReloadData();
            positionsWindow.ReloadData();
            tradeWindow.ReloadData();
            executionWindow.ReloadData();
        }

        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("CFFEX", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("SHFE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("DCE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("CZCE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            executionPane.AddContent(new ExecutionControl()).Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Opening(object sender, RoutedEventArgs e)
        {
            var executionWin = new ExecutionControl();
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENNING, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWin).Title = WPFUtility.GetLocalizedString("Opening", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            var executionWin = new ExecutionControl();
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED });
            executionPane.AddContent(executionWin).Title = WPFUtility.GetLocalizedString("Traded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_AllTraded(object sender, RoutedEventArgs e)
        {
            tradePane.AddContent(new TradeRecordControl()).Title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            var tradeWin = new TradeRecordControl();
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
            tradePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Open", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            var tradeWin = new TradeRecordControl();
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.CLOSE });
            tradePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Close", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            positionPane.AddContent(new PositionControl()).Title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
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
