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

        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        public BaseTraderHandler TradeHandler { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }

        public RiskAccountInfoControl()
        {
            InitializeComponent();
            //var fund = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVM;
            FundListView.ItemsSource = FundVMCollection;
            mColumns = ColumnObject.GetColumns(FundListView);           
        }

        private void UpdateAccountInfoCallback(object state)
        {
            TradeHandler.QueryAccountInfo();
        }

        public void ReloadData()
        {
            var fund = TradeHandler.FundVM;
            Dispatcher.Invoke(() =>
            {
                FundVMCollection.Clear();
                FundVMCollection.Add(fund);
            });
            _timer = new Timer(UpdateAccountInfoCallback, null, UpdateInterval, UpdateInterval); 
        }
        public event Action<FundVM> OnAccountSelected;
        private void FundListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FundVM fundVM = FundListView.SelectedItem as FundVM;
            OnAccountSelected?.Invoke(fundVM);
        }

        public void Initialize()
        {
        }
        private void MenuItem_Click_Login(object sender, RoutedEventArgs e)
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            var otctradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();                      
            FrameLoginWindow win = new FrameLoginWindow(tradeHandler.MessageWrapper.SignInManager, otctradeHandler.MessageWrapper.SignInManager);
            win.userTxt.Clear();
            win.passwordTxt.Clear();
            win.ShowDialog();
        }
    }
}
