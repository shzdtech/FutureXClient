using System.Collections.Generic;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class RisksFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _otcOptionSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();

        public RisksFrame()
        {
            InitializeComponent();
            Initialize();
        }

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

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

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
        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _otcOptionSignIner.SignInOptions.BrokerID = brokerId;
            _otcOptionSignIner.SignInOptions.UserName = usernname;
            _otcOptionSignIner.SignInOptions.Password = password;

            var entries = _otcOptionSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcOptionSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            //entries = _ctpSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            //if (server != null && entries.Length < 2)
            //    _ctpSignIner.SignInOptions.FrontServer = server + ':' + entries[0];
            //Test(_tdSignIner.SignInOptions);

            TDServerLogin();

            return LoginTaskSource.Task;
        }

        public void Initialize()
        {
            //portfolioCtl.AnchorablePane = portfolioselectPane;
            marketDataLV.AnchorablePane = quotePane;
            //greeksControl.AnchorablePane = greeksPane;
            positionsWindow.AnchorablePane = positionPane;
            tradeWindow.AnchorablePane = tradePane;
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            marketDataLV.MarketDataHandler = marketdataHandler;

        }
        private void _tdSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }
        private async void _tdSignIner_OnLogged(IUserInfo obj)
        {
            _otcOptionTradeHandler.RegisterMessageWrapper(_otcOptionHandler.MessageWrapper);
            await _otcOptionHandler.QueryStrategyAsync();
            await _otcOptionHandler.QueryAllModelParamsAsync();
            await _otcOptionTradeHandler.SyncContractInfoAsync();

            LoginTaskSource.TrySetResult(true);
        }
        private void TDServerLogin()
        {
            if (!_otcOptionSignIner.MessageWrapper.HasSignIn)
            {
                //OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _otcOptionSignIner.SignIn();
            }
            else
            {
                _tdSignIner_OnLogged(_otcOptionSignIner.LoggedUser);
            }
        }

    }
}
