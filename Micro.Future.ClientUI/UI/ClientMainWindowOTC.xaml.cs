﻿using System;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using System.Threading;
using Xceed.Wpf.AvalonDock.Layout;
using System.Collections.Generic;
using Micro.Future.Controls;
using System.Windows.Controls;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientMainWindowOTC : RibbonWindow
    {
        private const string CST_CONTROL_ASSEMBLY = "Micro.Future.Resources.Localization";
        private const string RESOURCE_FILE = "Resources";
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _otcClientSignIner = new PBSignInManager();
        private PBSignInManager _ctpTradeSignIner = new PBSignInManager();
        private PBSignInManager _ctpMdSignIner = new PBSignInManager();

        



        

        public ClientMainWindowOTC()
        {
            InitializeComponent();
            ribbonMenu.Title += " (" + MFUtilities.ClientVersion + ")";
            Initialize();
            Login();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
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

        

        void _otcClientSignIner_OnLogged(IUserInfo obj)
        {
            RightDownStatus.Content = "欢迎" + obj.LastName + obj.FirstName;

            _ctpMdSignIner.SignInOptions.UserID =
                    _ctpTradeSignIner.SignInOptions.UserID =
                    _otcClientSignIner.SignInOptions.UserID;

            _ctpMdSignIner.SignInOptions.Password =
                _ctpTradeSignIner.SignInOptions.Password =
                _otcClientSignIner.SignInOptions.Password;
            
            MDServerLogin();
            TradingServerLogin();
        }

        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(this, errRsult.Message, WPFUtility.GetLocalizedString("Error", RESOURCE_FILE), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void OTCClient_OnDisconnected(Exception ex)
        {
            MessageBox.Show(this, WPFUtility.GetLocalizedString("ReConnect", RESOURCE_FILE), "服务器连接已断开", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private bool Login()
        {
            LoginWindow loginWindow = new LoginWindow(_otcClientSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["OTCSERVER.ADDRESS"].Values
            };

            loginWindow.ShowDialog();

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
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                var mdCfg = _config.Content["MDSERVER"];
                _ctpMdSignIner.SignInOptions.FrontServer = mdCfg["ADDRESS"];
                _ctpMdSignIner.SignInOptions.BrokerID = mdCfg["BROKERID"];
                if (!string.IsNullOrWhiteSpace(mdCfg["USERID"]))
                    _ctpMdSignIner.SignInOptions.UserID = mdCfg["USERID"];
                if (!string.IsNullOrWhiteSpace(mdCfg["PASSWORD"]))
                    _ctpMdSignIner.SignInOptions.Password = mdCfg["PASSWORD"];

                ctpLoginStatus.Prompt = "正在连接CTP行情服务器...";
                _ctpMdSignIner.SignIn();
            }
        }

        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                var tdCfg = _config.Content["TRADESERVER"];
                _ctpTradeSignIner.SignInOptions.FrontServer = tdCfg["ADDRESS"];
                _ctpTradeSignIner.SignInOptions.BrokerID = tdCfg["BROKERID"];
                if (!string.IsNullOrWhiteSpace(tdCfg["USERID"]))
                    _ctpTradeSignIner.SignInOptions.UserID = tdCfg["USERID"];
                if (!string.IsNullOrWhiteSpace(tdCfg["PASSWORD"]))
                    _ctpTradeSignIner.SignInOptions.Password = tdCfg["PASSWORD"];

                ctpTradeLoginStatus.Prompt = "正在连接CTP交易服务器...";
                _ctpTradeSignIner.SignIn();
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

        private void ctpLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MDServerLogin();
        }

        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
        }


        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            
            Thread.Sleep(2000);
            tradeFrame.clientFundLV.ReloadData();
            Thread.Sleep(2000);
            tradeFrame.positionsWindow.ReloadData();
            Thread.Sleep(2000);
            tradeFrame.tradeWindow.ReloadData();
            Thread.Sleep(2000);
            tradeFrame.executionWindow.ReloadData();

        }

        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("Optional", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("CFFEX", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CFFEX", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("SHFE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("SHFE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("DCE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("DCE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("CZCE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CZCE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllExecution", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Opening(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENNING, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            ancable.Title = WPFUtility.GetLocalizedString("Opening", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED });
            ancable.Title = WPFUtility.GetLocalizedString("Traded", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Trade(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllTraded", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
            ancable.Title = WPFUtility.GetLocalizedString("Open", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.CLOSE });
            ancable.Title = WPFUtility.GetLocalizedString("Close", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var positionWin = new ClientPositionWindow() { LayoutContent = ancable };
            ancable.Content = positionWin;
            ancable.Title = WPFUtility.GetLocalizedString("Position", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradeFrame.positionPane.Children.Add(ancable);
        }


        private void FastOrder_Click(object sender, RoutedEventArgs e)
        {
            FastOrderWin fastOrderWindow = new FastOrderWin();
            fastOrderWindow.Show();
            tradeFrame.otcMarketDataLV.OnQuoteSelected += fastOrderWindow.OnQuoteSelected;
            tradeFrame.positionsWindow.OnPositionSelected += fastOrderWindow.OnPositionSelected;
        }

        private void RibbonTabHeader_Clicked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            mainPanel.SelectedContentIndex = int.Parse(ctrl.Tag.ToString());
        }
    }
}