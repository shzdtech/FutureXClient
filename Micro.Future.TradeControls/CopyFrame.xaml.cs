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

        public MessageHandlerContainer GetUserMessageContainer(string userId)
        {
            MessageHandlerContainer ret;
            if (!_userMsgContainer.TryGetValue(userId, out ret))
            {
                ret = new MessageHandlerContainer();
                _userMsgContainer[userId] = ret;

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

                ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;

                ServerLogin(ctpMdSignIner);
                ServerLogin(ctpTradeSignIner);
                ServerLogin(otcTradeSignIner);
                ServerLogin(otcTradingDeskSignIner);
                ServerLogin(otcOptionDataSignIner);

                ctpMdSignIner.SignInOptions.UserName = userId;
                ctpTradeSignIner.SignInOptions.UserName = userId;
                otcTradeSignIner.SignInOptions.UserName = userId;
                otcTradingDeskSignIner.SignInOptions.UserName = userId;
                otcOptionDataSignIner.SignInOptions.UserName = userId;

                ServerLogin(ctpMdSignIner);
                ServerLogin(ctpTradeSignIner);
                ServerLogin(otcTradeSignIner);
                ServerLogin(otcTradingDeskSignIner);
                ServerLogin(otcOptionDataSignIner);
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
        }
        public void OnAccountSelected(FundVM fundVM)
        {
            if (fundVM.AccountID!=null)
            {
                var ret = GetUserMessageContainer(fundVM.AccountID);
                MessageHandlerContainer.DefaultInstance = ret;
                var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
                var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                tradeWindow.TradeHandler = tradeHandler;
                positionsWindow.TradeHandler = tradeHandler;
                positionsWindow.MarketDataHandler = marketdataHandler;
                FastOrderCtl.TradeHandler = tradeHandler;
                FastOrderCtl.MarketDataHandler = marketdataHandler;
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_FUTURE);
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_OPTIONS);
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
            positionsWindow.ReloadData();
            Thread.Sleep(1000);
            tradeWindow.DEFAULT_ID = DEFAULT_ID;
            tradeWindow.ReloadData();
            Thread.Sleep(1000);
            FastOrderCtl.ReloadData();
        }
        private void _otcTradingDeskSignIner_OnLogged(IUserInfo obj)
        {
            _otcTradingDeskSignInerOnLogged();
        }
        private void _otcTradingDeskSignInerOnLogged()
        {
            var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            tradingdeskHandler.QueryTradingDesk();
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

            _ctpMdSignIner.SignInOptions.BrokerID = brokerId;
            _ctpMdSignIner.SignInOptions.UserName = usernname;
            _ctpMdSignIner.SignInOptions.Password = password;
            var entries = _ctpMdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpMdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

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
            //TradingServerLogin();
            OTCTradeServerLogin();
            return LoginTaskSource.Task;
        }

        private void ServerLogin(AbstractSignInManager signiner)
        {
            signiner.SignIn();
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
