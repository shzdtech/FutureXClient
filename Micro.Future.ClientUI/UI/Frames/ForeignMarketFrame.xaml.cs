using Micro.Future.CustomizedControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls.Primitives;
using Micro.Future.Message;
using Micro.Future.Business.Handler.Business;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ForeignMarketFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _ctsMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTSMarketDataHandler>());
        private AbstractSignInManager _ctsTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTSTradeHandler>());


        public ForeignMarketFrame()
        {
            InitializeComponent();
        }

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public string Title
        {
            get
            {
                return "Frame1";
            }
        }

        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return null;
            }
        }

        public Task<bool> LoginAsync(string usernname, string password, string server)
        {
            _ctsMdSignIner.SignInOptions.UserName = _ctsTradeSignIner.SignInOptions.UserName = "SZhou";
            _ctsMdSignIner.SignInOptions.Password = _ctsTradeSignIner.SignInOptions.Password = "sean91";
            _ctsMdSignIner.SignInOptions.BrokerID = _ctsTradeSignIner.SignInOptions.BrokerID = "simulator";
            var entries = _ctsMdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctsMdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            entries = _ctsTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctsTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            MarketDataServerLogin();
            TradingServerLogin();

            return LoginTaskSource.Task;
        }

        private void MarketDataServerLogin()
        {
            if (!_ctsMdSignIner.MessageWrapper.HasSignIn)
            {
                ctsLoginStatus.Prompt = "正在连接CTS行情服务器...";
                _ctsMdSignIner.SignIn();
            }
        }

        private void TradingServerLogin()
        {
            if (!_ctsTradeSignIner.MessageWrapper.HasSignIn)
            {
                ctsTradeLoginStatus.Prompt = "正在连接CTS交易服务器...";
                _ctsTradeSignIner.SignIn();
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

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();
    }
}
