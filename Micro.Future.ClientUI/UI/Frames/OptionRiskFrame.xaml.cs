using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.UI.OptionControls;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionRiskFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _otcOptionSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private AbstractOTCHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();

        //private List<PBSignInManager> _signIns = new List<PBSignInManager>();
        //private List<AbstractOTCHandler> _otcHdls = new List<AbstractOTCHandler>();
        //private int _cnt = 0;

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }


        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exOptionMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exOptionStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

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

        //public void Test(SignInOptions signInOpt)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        PBSignInManager tradeSignIner = new PBSignInManager(signInOpt);
        //        tradeSignIner.OnLogged += Test_OnLogged;
        //        _signIns.Add(tradeSignIner);
        //        tradeSignIner.SignIn();
        //        var hdl = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
        //        hdl.RegisterMessageWrapper(tradeSignIner.MessageWrapper);
        //        _otcHdls.Add(hdl);
        //    }
        //}

        //private void Test_OnLogged(IUserInfo userInfo)
        //{
        //    Console.WriteLine("Test Logged: " + userInfo.Name);

        //    if (++_cnt == 50)
        //    {
        //        foreach (var hdl in _otcHdls)
        //            hdl.QueryStrategy();
        //    }
        //}

        public void Initialize()
        {
            // Initialize Market Data


            var msgWrapper = _otcOptionSignIner.MessageWrapper;
            _otcOptionSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _otcOptionSignIner.OnLoginError += _tdSignIner_OnLoginError;
            _otcOptionSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            _otcOptionSignIner.OnLogged += _tdSignIner_OnLogged;

            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;
            _otcOptionHandler.RegisterMessageWrapper(msgWrapper);
            //optionPane.AddContent(new OptionModelCtrl()).Title = "Model";
            //optionPane.AddContent(new OpMarketMakerCtrl()).Title = "Market Maker";
            //optionPane.AddContent(new OpHedgeCtrl()).Title = "Hedge";
        }

        private void _tdSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }

        private void _tdSignIner_OnLogged(IUserInfo obj)
        {
            _otcOptionTradeHandler.RegisterMessageWrapper(_otcOptionHandler.MessageWrapper);
            LoginTaskSource.TrySetResult(true);
            Reload();
        }
        private void TDServerLogin()
        {
            if (!_otcOptionSignIner.MessageWrapper.HasSignIn)
            {
                OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _otcOptionSignIner.SignIn();
            }
           else
            {
                Reload();
            }
        }
        public OptionRiskFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
            }
        }
        private void OptionLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TDServerLogin();
        }
        private void Reload()
        {
            optionRiskCtrl.ReloadData();
        }

        private async void QueryRiskTest(object sender, RoutedEventArgs e)
        {
            var portfolio = optionRiskCtrl.portfolioCtl.portfolioCB.SelectedValue.ToString();
            var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
            optionRiskCtrl.greeksControl.GreekListView.ItemsSource = riskVMlist;
        }
    }
}

