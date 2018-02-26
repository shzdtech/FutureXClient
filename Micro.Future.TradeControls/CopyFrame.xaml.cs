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
using System.Collections.ObjectModel;
using Micro.Future.ViewModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class CopyFrame : UserControl, IUserFrame
    {
        private const string DEFAULT_ID = "D97F60E1-0433-4886-99E6-C4AD46A7D33A";

        private AbstractSignInManager _ctpMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<MarketDataHandler>());
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());
        private AbstractSignInManager _otcTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradeHandler>());
        private AbstractSignInManager _otcTradingDeskSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private AbstractSignInManager _otcOptionDataSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionDataHandler>());
        private AbstractSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());

        private IDictionary<string, MessageHandlerContainer> _userMsgContainer = new Dictionary<string, MessageHandlerContainer>();

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


        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public MessageHandlerContainer GetUserMessageContainer(string userName)
        {
            MessageHandlerContainer ret;
            if (!_userMsgContainer.TryGetValue(userName, out ret))
            {
                ret = new MessageHandlerContainer();
                _userMsgContainer[userName] = ret;

                var ctpMdSignIner = new PBSignInManager(_ctpMdSignIner.SignInOptions);
                ret.Get<MarketDataHandler>().RegisterMessageWrapper(ctpMdSignIner.MessageWrapper);
                var ctpTradeSignIner = new PBSignInManager(_ctpTradeSignIner.SignInOptions);
                ret.Get<TraderExHandler>().RegisterMessageWrapper(ctpTradeSignIner.MessageWrapper);
                var otcTradeSignIner = new PBSignInManager(_otcTradeSignIner.SignInOptions);
                ret.Get<OTCOptionTradeHandler>().RegisterMessageWrapper(otcTradeSignIner.MessageWrapper);
                var otcTradingDeskSignIner = new PBSignInManager(_otcTradingDeskSignIner.SignInOptions);
                ret.Get<OTCOptionTradingDeskHandler>().RegisterMessageWrapper(otcTradingDeskSignIner.MessageWrapper);
                var otcOptionDataSignIner = new PBSignInManager(_otcOptionDataSignIner.SignInOptions);
                ret.Get<OTCOptionDataHandler>().RegisterMessageWrapper(otcOptionDataSignIner.MessageWrapper);
                var accountSignIner = new PBSignInManager(_accountSignIner.SignInOptions);
                ret.Get<AccountHandler>().RegisterMessageWrapper(otcOptionDataSignIner.MessageWrapper);

                ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;

                var taskList = new List<Task<TaskResult<IUserInfo, MessageException>>>();

                Task.Run(() =>
                {
                    taskList.Add(ServerLoginAsync(ctpMdSignIner));
                    taskList.Add(ServerLoginAsync(ctpTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradingDeskSignIner));
                    taskList.Add(ServerLoginAsync(otcOptionDataSignIner));
                    taskList.Add(ServerLoginAsync(accountSignIner));


                    Task.WaitAll(taskList.ToArray());

                    ctpMdSignIner.SignInOptions.UserName = userName;
                    ctpTradeSignIner.SignInOptions.UserName = userName;
                    otcTradeSignIner.SignInOptions.UserName = userName;
                    otcTradingDeskSignIner.SignInOptions.UserName = userName;
                    otcOptionDataSignIner.SignInOptions.UserName = userName;

                    taskList.Clear();
                    taskList.Add(ServerLoginAsync(ctpMdSignIner));
                    taskList.Add(ServerLoginAsync(ctpTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradingDeskSignIner));
                    taskList.Add(ServerLoginAsync(otcOptionDataSignIner));
                    taskList.Add(ServerLoginAsync(accountSignIner));

                    Task.WaitAll(taskList.ToArray());

                    var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                    tradingdeskHandler.QueryAllModelParamsAsync();
                });
            }

            return ret;
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

        public CopyFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
                LoginTaskSource.TrySetResult(true);
            }
        }

        public void Initialize()
        {
            // Initailize UI events
            clientFundLV.OnAccountSelected += OnAccountSelected;
            tradeWindow.AnchorablePane = tradePane;
            positionsWindow.AnchorablePane = positionPane;
        }
        public void OnAccountSelected(TradingDeskVM tradingdeskVM)
        {
            if (tradingdeskVM.UserName != null)
            {
                var ret = GetUserMessageContainer(tradingdeskVM.UserName);
                MessageHandlerContainer.DefaultInstance = ret;
                var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
                var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                tradeWindow.TradeHandler = tradeHandler;
                positionsWindow.TradeHandler = tradeHandler;
                positionsWindow.MarketDataHandler = marketdataHandler;
                FastOrderCtl.TradeHandler = tradeHandler;
                FastOrderCtl.MarketDataHandler = marketdataHandler;
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_FUTURE);
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_OPTIONS);
                positionsWindow.DEFAULT_ID = DEFAULT_ID;
                tradeWindow.DEFAULT_ID = DEFAULT_ID;

                //positionsWindow.ReloadData();
                //tradeWindow.ReloadData();
                //riskparamsControl.RiskParamSP.Children.Add(new Xceed.Wpf.Toolkit.DoubleUpDown());
            }
        }
        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            _ctpTradeSignInerOnLogged();
        }
        private async void _ctpTradeSignInerOnLogged()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            await tradeHandler.SyncContractInfoAsync();
            Thread.Sleep(1000);
            positionsWindow.DEFAULT_ID = DEFAULT_ID;
            Dispatcher.Invoke(() =>
            {
                positionsWindow.ReloadData();
                Thread.Sleep(1000);
                tradeWindow.DEFAULT_ID = DEFAULT_ID;
                tradeWindow.ReloadData();
                Thread.Sleep(1000);
                FastOrderCtl.ReloadData();
            });
        }
        private void _otcTradingDeskSignIner_OnLogged(IUserInfo obj)
        {
            _otcTradingDeskSignInerOnLogged();
        }
        private async void _otcTradingDeskSignInerOnLogged()
        {
            var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            await tradingdeskHandler.QueryTradingDeskAsync();
            clientFundLV.TradingDeskVMCollection = tradingdeskHandler.TradingDeskVMCollection;
            clientFundLV.FundListView.ItemsSource = tradingdeskHandler.TradingDeskVMCollection;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        public void OnClosing()
        {

        }
        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _otcTradingDeskSignIner.OnLogged += _otcTradingDeskSignIner_OnLogged;

            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(_ctpMdSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>().RegisterMessageWrapper(_otcTradingDeskSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>().RegisterMessageWrapper(_otcOptionDataSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>().RegisterMessageWrapper(_otcTradeSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<AccountHandler>().RegisterMessageWrapper(_accountSignIner.MessageWrapper);


            _ctpMdSignIner.SignInOptions.BrokerID = brokerId;
            _ctpMdSignIner.SignInOptions.UserName = usernname;
            _ctpMdSignIner.SignInOptions.Password = password;
            var entries = _ctpMdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpMdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _accountSignIner.SignInOptions.BrokerID = brokerId;
            _accountSignIner.SignInOptions.UserName = usernname;
            _accountSignIner.SignInOptions.Password = password;
            if (server != null && entries.Length < 2)
                _accountSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _ctpTradeSignIner.SignInOptions.BrokerID = brokerId;
            _ctpTradeSignIner.SignInOptions.UserName = usernname;
            _ctpTradeSignIner.SignInOptions.Password = password;
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

            _otcTradeSignIner.SignInOptions.BrokerID = brokerId;
            _otcTradeSignIner.SignInOptions.UserName = usernname;
            _otcTradeSignIner.SignInOptions.Password = password;
            entries = _otcTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            TradingDeskServerLogin();
            MarketDataServerLogin();
            OTCOptionDataServerLogin();
            TradingServerLogin();
            OTCTradeServerLogin();
            AccountServerLogin();
            return LoginTaskSource.Task;
        }

        private Task<TaskResult<IUserInfo, MessageException>> ServerLoginAsync(AbstractSignInManager signiner)
        {
            return signiner.SignInAsync();
        }

        private void AccountServerLogin()
        {
            if (!_accountSignIner.MessageWrapper.HasSignIn)
            {
                _accountSignIner.SignIn();
            }
        }
        private void OTCOptionDataServerLogin()
        {
            if (!_otcOptionDataSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionMarketLoginStatus.Prompt = "连场外期权行情中";
                _otcOptionDataSignIner.SignIn();
            }
        }
        private void OTCOptionTradeServerLogin()
        {
            if (!_otcTradeSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradeLoginStatus.Prompt = "连场外期权交易中";
                _otcTradeSignIner.SignIn();
            }
        }
        private void MarketDataServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                //ctpLoginStatus.Prompt = "连CTP期权行情中";
                _ctpMdSignIner.SignIn();
            }
        }
        private void TradingDeskServerLogin()
        {
            if (!_otcTradingDeskSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradingDeskStatus.Prompt = "连期权TD中.";
                _otcTradingDeskSignIner.SignIn();
            }
        }
        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                //ctpTradeLoginStatus.Prompt = "连CTP期权交易中";
                _ctpTradeSignIner.SignIn();
            }
        }
        private void OTCTradeServerLogin()
        {
            if (!_otcTradeSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradeLoginStatus.Prompt = "连OTC期权交易中";
                _otcTradeSignIner.SignIn();
            }
        }
    }
}
