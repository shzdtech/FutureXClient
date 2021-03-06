﻿using System.Collections.Generic;
using System.Windows;
using Micro.Future.Message;
using System.ComponentModel;
using System.Windows.Controls;
using Micro.Future.CustomizedControls;
using System;
using System.Windows.Controls.Primitives;
using Micro.Future.Resources.Localization;
using Micro.Future.UI;
using Micro.Future.Utility;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.CustomizedControls.Windows;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.LocalStorage;
using Micro.Future.Windows;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.IO;
using System.Text;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class OTCTradeFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _mdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AbstractOTCHandler>());
        private AbstractSignInManager _tdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradeHandler>());


        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _mdSignIner.SignInOptions.UserName = _tdSignIner.SignInOptions.UserName = usernname;
            _mdSignIner.SignInOptions.Password = _tdSignIner.SignInOptions.Password = password;
            var entries = _mdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _mdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            entries = _tdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _tdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            MarketDataServerLogin();
            TradingServerLogin();

            return LoginTaskSource.Task;
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

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public OTCTradeFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
            }
        }

        public void Initialize()
        {
            // Initailize UI events
            MarketDataControl.OnQuoteSelected += FastOrderCtl.OnQuoteSelected;
            PositionControl.OnPositionSelected += FastOrderCtl.OnPositionSelected;
            marketDataLV.AnchorablePane = quotePane;
            executionWindow.AnchorablePane = executionPane;
            tradeWindow.AnchorablePane = tradePane;
            positionsWindow.AnchorablePane = positionPane;
            quotePane.Children[0].Title = WPFUtility.GetLocalizedString("Quote", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            executionPane.Children[0].Title = WPFUtility.GetLocalizedString("ExecutionWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            tradePane.Children[0].Title = WPFUtility.GetLocalizedString("TradeWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            positionPane.Children[0].Title = WPFUtility.GetLocalizedString("PositionWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);

            // Initialize Market Data
            var msgWrapper = _mdSignIner.MessageWrapper;

            _mdSignIner.OnLogged += ctpLoginStatus.OnLogged;
            _mdSignIner.OnLogged += _mdSignIner_OnLogged;
            _mdSignIner.OnLoginError += ctpLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpLoginStatus.OnDisconnected;
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>().RegisterMessageWrapper(msgWrapper);

            // Initialize Trading Server
            msgWrapper = _tdSignIner.MessageWrapper;

            _tdSignIner.OnLogged += _tdSignIner_OnLogged;
            _tdSignIner.OnLoginError += _tdSignIner_OnLoginError;
            _tdSignIner.OnLogged += ctpTradeLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += ctpTradeLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpTradeLoginStatus.OnDisconnected;

            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            tradeHandler.RegisterMessageWrapper(msgWrapper);
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();


            clientFundLV.TradeHandler = tradeHandler;
            marketDataLV.MarketDataHandler = marketdataHandler;
            FastOrderCtl.TradeHandler = tradeHandler;
            FastOrderCtl.MarketDataHandler = marketdataHandler;
            executionWindow.TradeHandler = tradeHandler;
            executionWindow.MarketDataHandler = marketdataHandler;
            tradeWindow.TradeHandler = tradeHandler;
            positionsWindow.TradeHandler = tradeHandler;
            positionsWindow.MarketDataHandler = marketdataHandler;
        }

        private void _mdSignIner_OnLogged(IUserInfo obj)
        {
            marketDataLV.ReloadData();
        }

        private void _tdSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }

        private void MarketDataServerLogin()
        {
            if (!_mdSignIner.MessageWrapper.HasSignIn)
            {
                ctpLoginStatus.Prompt = "正在连接CTP行情服务器...";
                _mdSignIner.SignIn();
            }
        }

        private void TradingServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                ctpTradeLoginStatus.Prompt = "正在连接CTP交易服务器...";
                _tdSignIner.SignIn();
            }
        }

        private void ctpMdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MarketDataServerLogin();
        }

        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
        }


        private void _tdSignIner_OnLogged(IUserInfo obj)
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();

            clientFundLV.ReloadData();
            Thread.Sleep(1000);
            positionsWindow.ReloadData();
            Thread.Sleep(1000);
            tradeWindow.ReloadData();
            Thread.Sleep(1000);
            executionWindow.ReloadData();

            LoginTaskSource.TrySetResult(true);
            //var layoutInfo = ClientDbContext.GetLayout(tradeHandler.MessageWrapper.User.Id, otcTradeDM.Uid);
            //if (layoutInfo != null)
            //{
            //    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(otcTradeDM);

            //    using (var reader = new StringReader(layoutInfo.LayoutCFG))
            //    {
            //        layoutSerializer.Deserialize(reader);
            //    }
            //}
        }

        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            //quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var marketDataWin = new MarketDataControl(marketDataLV.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>());
            marketDataWin.FilterSettingsWin.Title += "(" + title + ")";
            marketDataWin.FilterSettingsWin.FilterTabTitle = title;
            quotePane.AddContent(marketDataWin).Title = title;
            marketDataWin.FilterSettingsWin.Save();
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            //executionPane.AddContent(new ExecutionControl(Guid.NewGuid().ToString())).Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Opened(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENED, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED, OrderStatus.PARTIAL_TRADED });
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_AllTraded(object sender, RoutedEventArgs e)
        {
            //tradePane.AddContent(new TradeRecordControl()).Title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var tradeRecordWin = new TradeRecordControl(tradeWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>());
            tradeRecordWin.FilterSettingsWin.Title += "(" + title + ")";
            tradeRecordWin.FilterSettingsWin.FilterTabTitle = title;
            tradePane.AddContent(tradeRecordWin).Title = title;
            tradeRecordWin.FilterSettingsWin.Save();
        }

        //private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        //{
        //    var tradeWin = new TradeRecordControl(Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
        //    tradeWin.FilterByStatus(new List<OrderOpenCloseType> { OrderOpenCloseType.OPEN });
        //    tradePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Open", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        //}

        //private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        //{
        //    var tradeWin = new TradeRecordControl(Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
        //    tradeWin.FilterByStatus(new List<OrderOpenCloseType> { OrderOpenCloseType.CLOSE });
        //    tradePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Close", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        //}

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            //positionPane.AddContent(new PositionControl()).Title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var positionWin = new PositionControl(positionsWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>(), MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>());
            positionWin.FilterSettingsWin.Title += "(" + title + ")";
            positionWin.FilterSettingsWin.FilterTabTitle = title;
            positionPane.AddContent(positionWin).Title = title;
            positionWin.FilterSettingsWin.Save();
        }

        private void MenuItem_Click_Portfolio(object sender, RoutedEventArgs e)
        {
            PortoforlioWindow win = new PortoforlioWindow();
            win.Show();
        }

        private void MenuItem_Click_FutureAccount(object sender, RoutedEventArgs e)
        {
            FutureAccountInfoWindow win = new FutureAccountInfoWindow();
            win.Show();
        }

        private void ClosingPositionPane(object sender, CancelEventArgs e)
        {
            positionsWindow.DeletePositionDB();
        }

        private async void MenuItem_RefreshContracts_Click(object sender, RoutedEventArgs e)
        {
            var mdHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
            await mdHandler.SyncContractInfoAsync(true);
            MessageBox.Show(Application.Current.MainWindow, "合约已刷新，请重新启动应用！");
        }

        public void SaveLayout()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();

            var layoutInfo = ClientDbContext.GetLayout(tradeHandler.MessageWrapper.User?.Id, otcTradeDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(otcTradeDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            ClientDbContext.SaveLayoutInfo(tradeHandler.MessageWrapper.User.Id, otcTradeDM.Uid, strBuilder.ToString());
        }

        public void OnClosing()
        {
            SaveLayout();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }
    }
}
