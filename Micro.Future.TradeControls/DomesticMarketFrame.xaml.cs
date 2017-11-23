using System.Collections.Generic;
using System.Windows;
using Micro.Future.Message;
using System.ComponentModel;
using System.Windows.Controls;
using Micro.Future.CustomizedControls;
using System;
using System.Windows.Controls.Primitives;
using Micro.Future.Resources.Localization;
using Micro.Future.CustomizedControls.Windows;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.LocalStorage;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.IO;
using System.Text;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class DomesticMarketFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _ctpMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<MarketDataHandler>());
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());
        private AbstractSignInManager _otcTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradeHandler>());
        private AbstractSignInManager _otcTradingDeskSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private AbstractSignInManager _otcOptionDataSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionDataHandler>());

        private PBSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());

        private const string DEFAULT_ID = "D97F60E1-0433-4886-99E6-C4AD46A7D33B";
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
            _ctpMdSignIner.SignInOptions.BrokerID = brokerId;
            _ctpMdSignIner.SignInOptions.UserName = usernname;
            _ctpMdSignIner.SignInOptions.Password = password;
            var entries = _ctpMdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpMdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];
            entries = _ctpTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            //TradingDesk登录
            _otcTradingDeskSignIner.SignInOptions.BrokerID = brokerId;
            _otcTradingDeskSignIner.SignInOptions.UserName = usernname;
            _otcTradingDeskSignIner.SignInOptions.Password = password;
            entries = _otcTradingDeskSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcTradingDeskSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _otcOptionDataSignIner.SignInOptions.BrokerID = brokerId;
            _otcOptionDataSignIner.SignInOptions.UserName = usernname;
            _otcOptionDataSignIner.SignInOptions.Password = password;
            entries = _otcOptionDataSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcOptionDataSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            entries = _otcTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];
            TradingDeskServerLogin();
            MarketDataServerLogin();
            OTCOptionDataServerLogin();
            //TradingServerLogin();

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
            // Initailize UI events

            MarketDataControl.OnQuoteSelected += FastOrderCtl.OnQuoteSelected;
            PositionControl.OnPositionSelected += FastOrderCtl.OnPositionSelected;
            marketDataLV.AnchorablePane = quotePane;
            executionWindow.AnchorablePane = executionPane;
            tradeWindow.AnchorablePane = tradePane;
            positionsWindow.AnchorablePane = positionPane;
            quotePane.Children[0].Title = WPFUtility.GetLocalizedString("Quote", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            executionPane.Children[0].Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            tradePane.Children[0].Title = WPFUtility.GetLocalizedString("TradeWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            positionPane.Children[0].Title = WPFUtility.GetLocalizedString("PositionWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            // Initialize Market Data
            ctpLoginStatus.Prompt = "CTP期权行情未连";
            ctpTradeLoginStatus.Prompt = "CTP期权交易未连";
            otcOptionMarketLoginStatus.Prompt = "场外期权行情未连";
            otcOptionTradeLoginStatus.Prompt = "场外期权交易未连";
            otcOptionTradingDeskStatus.Prompt = "场外期权TD未连";
            var msgWrapper = _ctpMdSignIner.MessageWrapper;
            _ctpMdSignIner.OnLogged += ctpLoginStatus.OnLogged;
            _ctpMdSignIner.OnLoginError += ctpLoginStatus.OnDisconnected;
            _ctpMdSignIner.OnLogged += _ctpMdSignIner_OnLogged;
            msgWrapper.MessageClient.OnDisconnected += ctpLoginStatus.OnDisconnected;
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(msgWrapper);
            _otcTradingDeskSignIner.OnLogged += _otcTradingDeskSignIner_Onlogged;
            _otcTradingDeskSignIner.OnLogged += otcOptionTradingDeskStatus.OnLogged;
            _otcTradingDeskSignIner.OnLoginError += otcOptionTradingDeskStatus.OnDisconnected;
            _otcTradeSignIner.OnLogged += _otcTradeSignIner_Onlogged;
            _otcTradeSignIner.OnLogged += otcOptionTradeLoginStatus.OnLogged;
            _otcTradeSignIner.OnLoginError += otcOptionTradeLoginStatus.OnDisconnected;
            // Initialize Trading Server
            msgWrapper = _ctpTradeSignIner.MessageWrapper;

            _ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;
            _ctpTradeSignIner.OnLoginError += _ctpTradeSignIner_OnLoginError;
            _ctpTradeSignIner.OnLogged += ctpTradeLoginStatus.OnLogged;
            _ctpTradeSignIner.OnLoginError += ctpTradeLoginStatus.OnDisconnected;

            msgWrapper.MessageClient.OnDisconnected += ctpTradeLoginStatus.OnDisconnected;

            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            tradeHandler.RegisterMessageWrapper(msgWrapper);
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();

            msgWrapper = _otcTradeSignIner.MessageWrapper;
            var otctradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            otctradeHandler.RegisterMessageWrapper(msgWrapper);

            _otcOptionDataSignIner.OnLogged += _otcOptionDataSignIner_OnLogged;
            _otcOptionDataSignIner.OnLogged += otcOptionMarketLoginStatus.OnLogged;
            _otcOptionDataSignIner.OnLoginError += otcOptionMarketLoginStatus.OnDisconnected;
            msgWrapper = _otcOptionDataSignIner.MessageWrapper;
            var otcoptiondataHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
            otcoptiondataHandler.RegisterMessageWrapper(msgWrapper);

            //msgWrapper = _otcTradingDeskSignIner.MessageWrapper;
            var otcoptiontradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            otcoptiontradingdeskHandler.RegisterMessageWrapper(msgWrapper);

            clientFundLV.TradeHandler = tradeHandler;
            marketDataLV.MarketDataHandler = marketdataHandler;
            marketDataLV.ProductTypeList.Add(ProductType.PRODUCT_FUTURE);
            marketDataLV.ProductTypeList.Add(ProductType.PRODUCT_OPTIONS);
            FastOrderCtl.TradeHandler = tradeHandler;
            FastOrderCtl.MarketDataHandler = marketdataHandler;
            FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_FUTURE);
            FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_OPTIONS);
            executionWindow.TradeHandler = tradeHandler;
            executionWindow.MarketDataHandler = marketdataHandler;
            tradeWindow.TradeHandler = tradeHandler;
            positionsWindow.TradeHandler = tradeHandler;
            positionsWindow.MarketDataHandler = marketdataHandler;
        }

        private async void _otcOptionDataSignIner_OnLogged(IUserInfo obj)
        {
            var otcoptiontradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            otcoptiontradingdeskHandler.QueryStrategy();
            otcoptiontradingdeskHandler.QueryPortfolio();
            await otcoptiontradingdeskHandler.QueryAllModelParamsAsync();
            var otcoptiondataHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
            await otcoptiondataHandler.SyncContractInfoAsync();
        }

        private void _ctpMdSignIner_OnLogged(IUserInfo obj)
        {
            marketDataLV.DEFAULT_ID = DEFAULT_ID;
            marketDataLV.ReloadData();
            //LoginTaskSource.TrySetResult(true);
        }
        private void _otcTradingDeskSignIner_Onlogged(IUserInfo obj)
        {
            LoginTaskSource.TrySetResult(true);
        }
        private void _otcTradeSignIner_Onlogged(IUserInfo obj)
        {
            _otcTradeSignIner_Onlogged();
        }
        private void _otcTradeSignIner_Onlogged()
        {

        }
        private void _ctpTradeSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }
        private void OTCOptionDataServerLogin()
        {
            if (!_otcOptionDataSignIner.MessageWrapper.HasSignIn)
            {
                otcOptionMarketLoginStatus.Prompt = "连场外期权行情中";
                _otcOptionDataSignIner.SignIn();
            }
        }
        private void OTCOptionTradeServerLogin()
        {
            if (!_otcTradeSignIner.MessageWrapper.HasSignIn)
            {
                otcOptionTradeLoginStatus.Prompt = "连场外期权交易中";
                _otcTradeSignIner.SignIn();
            }
        }
        private void MarketDataServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                ctpLoginStatus.Prompt = "连CTP期权行情中";
                _ctpMdSignIner.SignIn();
            }
        }
        private void TradingDeskServerLogin()
        {
            if (!_otcTradingDeskSignIner.MessageWrapper.HasSignIn)
            {
                otcOptionTradingDeskStatus.Prompt = "连期权TD中.";
                _otcTradingDeskSignIner.SignIn();
            }
        }
        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                ctpTradeLoginStatus.Prompt = "连CTP期权交易中";
                _ctpTradeSignIner.SignIn();
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
        private void otcMdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            OTCOptionDataServerLogin();
        }
        private void otcTradeLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            OTCOptionTradeServerLogin();
        }
        private void otcTradingDeskLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingDeskServerLogin();
        }

        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            _ctpTradeSignInerOnLogged();
            //var layoutInfo = ClientDbContext.GetLayout(tradeHandler.MessageWrapper.User.Id, domesticDM.Uid);
            //if (layoutInfo != null)
            //{
            //    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(domesticDM);

            //    using (var reader = new StringReader(layoutInfo.LayoutCFG))
            //    {
            //        layoutSerializer.Deserialize(reader);
            //    }
            //}
        }
        private async void _ctpTradeSignInerOnLogged()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            await tradeHandler.SyncContractInfoAsync();
            marketDataLV.DEFAULT_ID = DEFAULT_ID;
            marketDataLV.ReloadData();
            //marketDataLV.GetContractInfo();
            Thread.Sleep(1000);
            clientFundLV.ReloadData();
            Thread.Sleep(1000);
            positionsWindow.DEFAULT_ID = DEFAULT_ID;
            positionsWindow.ReloadData();
            Thread.Sleep(1000);
            tradeWindow.DEFAULT_ID = DEFAULT_ID;
            tradeWindow.ReloadData();
            Thread.Sleep(1000);
            executionWindow.DEFAULT_ID = DEFAULT_ID;
            executionWindow.ReloadData();

            //LoginTaskSource.TrySetResult(true);

            var titleOpened = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWinOpened = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            executionWinOpened.FilterSettingsWin.Title += "(" + titleOpened + ")";
            executionWinOpened.FilterSettingsWin.FilterTabTitle = titleOpened;
            executionWinOpened.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENED, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWinOpened).Title = titleOpened;

            var titleTraded = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWinTraded = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            executionWinTraded.FilterSettingsWin.Title += "(" + titleTraded + ")";
            executionWinTraded.FilterSettingsWin.FilterTabTitle = titleTraded;
            executionWinTraded.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED, OrderStatus.PARTIAL_TRADED });
            executionPane.AddContent(executionWinTraded).Title = titleTraded;
        }
        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            //quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var marketDataWin = new MarketDataControl(marketDataLV.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>());
            marketDataWin.FilterSettingsWin.Title += "(" + title + ")";
            marketDataWin.FilterSettingsWin.FilterTabTitle = title;
            quotePane.AddContent(marketDataWin).Title = title;
            marketDataWin.FilterSettingsWin.Save();
        }


        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("", "CFFEX", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("", "SHFE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("", "DCE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            var quoteGrpVw = new MarketDataControl();
            quoteGrpVw.Filter("", "CZCE", "", "");
            quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("CFFEX", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            //executionPane.AddContent(new ExecutionControl(Guid.NewGuid().ToString())).Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Opened(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENED, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
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
            var tradeRecordWin = new TradeRecordControl(tradeWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            tradeRecordWin.FilterSettingsWin.Title += "(" + title + ")";
            tradeRecordWin.FilterSettingsWin.FilterTabTitle = title;
            tradePane.AddContent(tradeRecordWin).Title = title;
            tradeRecordWin.FilterSettingsWin.Save();
        }
        private void MenuItem_Click_FastOrderCtrl(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("FastOrderControl", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var fastOrderCtrl = new FastOrderControl();
            fastorderPane.AddContent(fastOrderCtrl).Title = title;
        }
        private void MenuItem_Click_AccountCtrl(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("AccountControl", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var accountCtrl = new AccountInfoControl();
            fundPane.AddContent(accountCtrl).Title = title;
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
            var positionWin = new PositionControl(positionsWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>(), MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>());
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
            var mdHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            await mdHandler.SyncContractInfoAsync(true);
            MessageBox.Show(Application.Current.MainWindow, "合约已刷新，请重新启动应用！");
        }

        public void SaveLayout()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            if (tradeHandler.MessageWrapper.User != null)
            {
                var layoutInfo = ClientDbContext.GetLayout(tradeHandler.MessageWrapper.User.Id, domesticDM.Uid);
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(domesticDM);
                var strBuilder = new StringBuilder();
                using (var writer = new StringWriter(strBuilder))
                {
                    layoutSerializer.Serialize(writer);
                }
                ClientDbContext.SaveLayoutInfo(tradeHandler.MessageWrapper.User?.Id, domesticDM.Uid, strBuilder.ToString());
            }
        }
        private void LoginWindow_OnLogged(FrameLoginWindow sender, IUserInfo userInfo)
        {
            _ctpTradeSignInerOnLogged();
            sender.Close();
        }
        private void _currentLoginWindow_Closed(object sender, EventArgs e)
        {

        }
        public void OnClosing()
        {
            SaveLayout();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

        private void MenuItem_Click_Login(object sender, RoutedEventArgs e)
        {
            FrameLoginWindow win = new FrameLoginWindow(_ctpTradeSignIner, _otcTradeSignIner);
            win.Closed += _currentLoginWindow_Closed;
            win.OnLogged += LoginWindow_OnLogged;
            win.ShowDialog();
        }
    }
}
