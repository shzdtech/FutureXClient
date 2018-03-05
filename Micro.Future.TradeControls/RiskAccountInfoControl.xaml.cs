using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System.Threading;
using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Micro.Future.CustomizedControls.Windows;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class RiskAccountInfoControl : UserControl, IReloadData
    {
        private IList<ColumnObject> mColumns;
        private Timer _timer;
        private const int UpdateInterval = 2000;
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());
        private AbstractSignInManager _otcTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradeHandler>());

        public ObservableCollection<TradingDeskVM> TradingDeskVMCollection
        {
            get;
            set;
        } = new ObservableCollection<TradingDeskVM>();

        public AbstractOTCHandler TradeHandler { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }

        public RiskAccountInfoControl()
        {
            InitializeComponent();
            //FundListView.ItemsSource = TradingDeskVMCollection;
            mColumns = ColumnObject.GetColumns(FundListView);           
        }

        private void UpdateAccountInfoCallback(object state)
        {
            TradeHandler.QueryTradingDesk();
        }

        public void ReloadData()
        {
            _timer = new Timer(UpdateAccountInfoCallback, null, UpdateInterval, UpdateInterval); 
        }
        public event Action<TradingDeskVM> OnAccountSelected;
        public event Action OnClickLogin;
        private void FundListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TradingDeskVM tradingdeskVM = FundListView.SelectedItem as TradingDeskVM;
            OnAccountSelected?.Invoke(tradingdeskVM);
        }

        public void Initialize()
        {
        }
        private void MenuItem_Click_Login(object sender, RoutedEventArgs e)
        {
            OnClickLogin();
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            var otctradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();                      
            FrameLoginWindow win = new FrameLoginWindow(tradeHandler.MessageWrapper.SignInManager, otctradeHandler.MessageWrapper.SignInManager);
            win.userTxt.Clear();
            win.passwordTxt.Clear();
            win.ShowDialog();
        }
    }
}
