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
    public partial class StockFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _ctpMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTPSTOCKMDHandler>());
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTPSTOCKTraderHandler>());
        private PBSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());
        private bool _logged;

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

            MarketDataServerLogin();
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

        public StockFrame()
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

            // Initialize Market Data
            var msgWrapper = _ctpMdSignIner.MessageWrapper;

            _ctpMdSignIner.OnLogged += ctpLoginStatus.OnLogged;
            _ctpMdSignIner.OnLogged += _ctpMdSignIner_OnLogged;
            _ctpMdSignIner.OnLoginError += ctpLoginStatus.OnDisconnected;
            _ctpMdSignIner.OnLoginError += _ctpMdSignIner_OnLoggedError;
            msgWrapper.MessageClient.OnDisconnected += ctpLoginStatus.OnDisconnected;
            MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>().RegisterMessageWrapper(msgWrapper);

            // Initialize Trading Server
            msgWrapper = _ctpTradeSignIner.MessageWrapper;

            _ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;
            _ctpTradeSignIner.OnLoginError += _ctpTradeSignIner_OnLoginError;
            _ctpTradeSignIner.OnLogged += ctpTradeLoginStatus.OnLogged;
            _ctpTradeSignIner.OnLoginError += ctpTradeLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += ctpTradeLoginStatus.OnDisconnected;

            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
            tradeHandler.RegisterMessageWrapper(msgWrapper);
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>();

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

        private async void _ctpMdSignIner_OnLogged(IUserInfo obj)
        {
            LoginTaskSource.TrySetResult(true);
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>();
            await tradeHandler.SyncContractInfoAsync();
            marketDataLV.ReloadData();
        }
        private void _ctpMdSignIner_OnLoggedError(object obj)
        {
            marketDataLV.ReloadData();
            LoginTaskSource.TrySetResult(true);
        }
        private void _ctpTradeSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
            LoginTaskSource.TrySetResult(true);
        }

        private void MarketDataServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                ctpLoginStatus.Prompt = "正在连接CTP行情服务器...";
                _ctpMdSignIner.SignIn();
            }
        }

        private void ctpMdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MarketDataServerLogin();
        }
        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                ctpTradeLoginStatus.Prompt = "正在连接CTP交易服务器...";
                _ctpTradeSignIner.SignIn();
            }
        }


        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
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
        private void _ctpTradeSignInerOnLogged()
        {
            //var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>();
            //await tradeHandler.SyncContractInfoAsync();
            //marketDataLV.ReloadData();
            //marketDataLV.GetContractInfo();
            Thread.Sleep(1000);
            clientFundLV.ReloadData();
            Thread.Sleep(1000);
            positionsWindow.ReloadData();
            Thread.Sleep(1000);
            tradeWindow.ReloadData();
            Thread.Sleep(1000);
            executionWindow.ReloadData();

            //LoginTaskSource.TrySetResult(true);

            var titleOpened = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWinOpened = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
            executionWinOpened.FilterSettingsWin.Title += "(" + titleOpened + ")";
            executionWinOpened.FilterSettingsWin.FilterTabTitle = titleOpened;
            executionWinOpened.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENED, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWinOpened).Title = titleOpened;

            var titleTraded = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWinTraded = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
            executionWinTraded.FilterSettingsWin.Title += "(" + titleTraded + ")";
            executionWinTraded.FilterSettingsWin.FilterTabTitle = titleTraded;
            executionWinTraded.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED, OrderStatus.PARTIAL_TRADED });
            executionPane.AddContent(executionWinTraded).Title = titleTraded;
        }
        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            //quotePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var marketDataWin = new MarketDataControl(marketDataLV.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>());
            marketDataWin.FilterSettingsWin.Title += "(" + title + ")";
            marketDataWin.FilterSettingsWin.FilterTabTitle = title;
            quotePane.AddContent(marketDataWin).Title = title;
            marketDataWin.FilterSettingsWin.Save();
        }


        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            //executionPane.AddContent(new ExecutionControl(Guid.NewGuid().ToString())).Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Opened(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
            executionWin.FilterSettingsWin.Title += "(" + title + ")";
            executionWin.FilterSettingsWin.FilterTabTitle = title;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENED, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            executionPane.AddContent(executionWin).Title = title;
            executionWin.Save();
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var executionWin = new ExecutionControl(executionWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
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
            var tradeRecordWin = new TradeRecordControl(tradeWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>());
            tradeRecordWin.FilterSettingsWin.Title += "(" + title + ")";
            tradeRecordWin.FilterSettingsWin.FilterTabTitle = title;
            tradePane.AddContent(tradeRecordWin).Title = title;
            tradeRecordWin.FilterSettingsWin.Save();
        }
        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            //positionPane.AddContent(new PositionControl()).Title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var positionWin = new PositionControl(positionsWindow.PersistanceId, Guid.NewGuid().ToString(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>(), MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>());
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
            var mdHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>();
            await mdHandler.SyncContractInfoAsync(true);
            MessageBox.Show(Application.Current.MainWindow, "合约已刷新，请重新启动应用！");
        }

        public void SaveLayout()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
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
            FrameLoginWindow win = new FrameLoginWindow(_ctpTradeSignIner);
            win.Closed += _currentLoginWindow_Closed;
            win.OnLogged += LoginWindow_OnLogged;
            win.ShowDialog();
        }
    }
}
