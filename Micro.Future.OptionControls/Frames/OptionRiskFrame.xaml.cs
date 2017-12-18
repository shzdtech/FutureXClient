using Micro.Future.CustomizedControls;
using Micro.Future.CustomizedControls.Windows;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionRiskFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _otcOptionSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private TraderExHandler _traderexHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private AbstractSignInManager _ctpTdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());



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
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
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
            return Task.FromResult(true);
        }

        public void Initialize()
        {
            // Initialize Market Data
        }

        private void _ctpTdSignIner_OnLogged(IUserInfo obj)
        {
            _traderexHandler.QueryPosition();
        }


        private void TradingServerLogin()
        {
            if (!_ctpTdSignIner.MessageWrapper.HasSignIn)
            {
                _ctpTdSignIner.SignIn();
            }
        }


        private void ctpTradingLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TradingServerLogin();
        }



        private void _tdSignIner_OnLogged(IUserInfo obj)
        {
            _otcOptionTradeHandler.RegisterMessageWrapper(_otcOptionTradeHandler.MessageWrapper);
            _otcOptionHandler.QueryPortfolio();
            Thread.Sleep(2000);
            LoginTaskSource.TrySetResult(true);
            Reload();
            //var layoutInfo = ClientDbContext.GetLayout(_otcOptionTradeHandler.MessageWrapper.User.Id, optionRiskDM.Uid);
            //if (layoutInfo != null)
            //{
            //    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionRiskDM);

            //    using (var reader = new StringReader(layoutInfo.LayoutCFG))
            //    {
            //        layoutSerializer.Deserialize(reader);
            //    }
            //}
        }
        private void TDServerLogin()
        {
            if (!_otcOptionSignIner.MessageWrapper.HasSignIn)
            {
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
            var portfolio = optionRiskCtrl.portfolioCtl.portfolioCB.SelectedValue?.ToString();
            var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
            optionRiskCtrl.greeksControl.GreekListView.ItemsSource = riskVMlist;
        }

        private void OptionRiskCtrl_Closed(object sender, EventArgs e)
        {
            optionRiskCtrl.portfolioCtl.AutoHedgeUpdate(false);
        }

        public void SaveLayout()
        {

            var layoutInfo = ClientDbContext.GetLayout(_otcOptionTradeHandler.MessageWrapper.User?.Id, optionRiskDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionRiskDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            if (_otcOptionTradeHandler.MessageWrapper.User != null)
                ClientDbContext.SaveLayoutInfo(_otcOptionTradeHandler.MessageWrapper.User.Id, optionRiskDM.Uid, strBuilder.ToString());
        }

        public void OnClosing()
        {
            SaveLayout();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

        private void refreshAll_Click(object sender, RoutedEventArgs e)
        {
            optionRiskGraphCtrl.ReloadDataCallback();
            optionRiskGraphCtrl.resetGraph();
            optionContractRiskGraphCtrl.ReloadDataCallback();
            optionContractRiskGraphCtrl.resetGraph();
            optionMatrixCtrl.makeTable(optionMatrixCtrl.VolCnt, optionMatrixCtrl.VolSize, optionMatrixCtrl.PriceCnt, optionMatrixCtrl.PriceSize);
            optionMatrixCtrl.ReloadDataCallback();

        }
        private void Add_RiskAnalysisGraph_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("RiskAnalysisGraph", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var riskAnalysisGraphCtrl = new OptionRiskCtrl();
            optionRiskPane.AddContent(riskAnalysisGraphCtrl).Title = title;
        }
        private void Add_RiskGraph_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("RiskGraph", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var riskGraphCtrl = new OptionRiskGraphCtrl();
            optionRiskPane.AddContent(riskGraphCtrl).Title = title;
        }
        private void Add_ContractRiskGraph_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("ContractRiskGraph", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var contractriskGraphCtrl = new OptionContractRiskGraphCtrl();
            optionRiskPane.AddContent(contractriskGraphCtrl).Title = title;
        }
        private void Add_RiskMatrix_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("RiskMatrix", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var riskMatrixCtrl = new OptionMatrixCtrl();
            optionRiskPane.AddContent(riskMatrixCtrl).Title = title;
        }
        private void MenuItem_AddTrade_Click(object sender, RoutedEventArgs e)
        {
            AddTradeRecordWindow win = new AddTradeRecordWindow();
            win.AddButton.Click += AddButton_Click;
            win.Show();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            optionRiskGraphCtrl.ReloadDataCallback();
            optionContractRiskGraphCtrl.ReloadDataCallback();
            optionMatrixCtrl.ReloadDataCallback();
        }

        private void MenuItem_SyncPosition_Click(object sender, RoutedEventArgs e)
        {
            PositionDifferWindow win = new PositionDifferWindow();
            win.SyncButton.Click += AddButton_Click;
            win.ETFSyncButton.Click += AddButton_Click;
            win.StockSyncButton.Click += AddButton_Click;
            win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            win.ETFTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
            win.StockTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
            win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            win.QueryPositionDiffer();
            if(win.TotalSysPosition==0 && win.TotalPosition!=0)
            {
                string msg = string.Format("大宗商品账户正在查询，请稍后重试");
                MessageBoxResult dr = System.Windows.MessageBox.Show(msg);
            }
            if (win.TotalETFSysPosition == 0 && win.TotalETFPosition != 0)
            {
                string msg = string.Format("ETF期权账户正在查询，请稍后重试");
                MessageBoxResult dr = System.Windows.MessageBox.Show(msg);
            }
            if (win.TotalStockSysPosition == 0 && win.TotalStockPosition != 0)
            {
                string msg = string.Format("证券账户正在查询，请稍后重试");
                MessageBoxResult dr = System.Windows.MessageBox.Show(msg);
            }
            win.Show();
        }
        //private void MenuItem_SyncStockPosition_Click(object sender, RoutedEventArgs e)
        //{
        //    PositionDifferWindow win = new PositionDifferWindow();
        //    win.SyncButton.Click += AddButton_Click;
        //    win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
        //    win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>();
        //    win.QueryPositionDiffer();
        //    win.Show();
        //}
        //private void MenuItem_SyncETFPosition_Click(object sender, RoutedEventArgs e)
        //{
        //    PositionDifferWindow win = new PositionDifferWindow();
        //    win.SyncButton.Click += AddButton_Click;
        //    win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
        //    win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();
        //    win.QueryPositionDiffer();
        //    win.Show();
        //}
        private void MenuItem_TradeFutureSync_Click(object sender, RoutedEventArgs e)
        {
            TradeDifferWindow win = new TradeDifferWindow();
            win.SyncButton.Click += AddButton_Click;
            win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            win.ETFTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
            win.StockTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
            win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            win.QueryTradeDiffer();
            win.Show();
        }
        private void MenuItem_TradeETFSync_Click(object sender, RoutedEventArgs e)
        {
            TradeDifferWindow win = new TradeDifferWindow();
            win.SyncButton.Click += AddButton_Click;
            win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
            win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();
            win.QueryTradeDiffer();
            win.Show();
        }
        private void MenuItem_TradeStockSync_Click(object sender, RoutedEventArgs e)
        {
            TradeDifferWindow win = new TradeDifferWindow();
            win.SyncButton.Click += AddButton_Click;
            win.TradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
            win.TradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>();
            win.QueryTradeDiffer();
            win.Show();
        }
    }
}

