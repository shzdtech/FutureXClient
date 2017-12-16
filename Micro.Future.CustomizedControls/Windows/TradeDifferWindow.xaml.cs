using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Micro.Future.CustomizedControls.Windows
{
    public partial class TradeDifferWindow : Window
    {
        public BaseTraderHandler TradeHandler { get; set; }
        public BaseTraderHandler ETFTradeHandler { get; set; }
        public BaseTraderHandler StockTradeHandler { get; set; }

        public List<TradeDifferVM> TradeSyncList { get; } = new List<TradeDifferVM>();
        public List<TradeDifferVM> ETFTradeSyncList { get; } = new List<TradeDifferVM>();
        public List<TradeDifferVM> StockTradeSyncList { get; } = new List<TradeDifferVM>();

        public BaseTradingDeskHandler TradingDeskHandler { get; set; }
        public ObservableCollection<PortfolioVM> PortfolioCollection
        {
            get
            {
                return TradingDeskHandler.PortfolioVMCollection;
            }
        }
        public OTCOptionTradingDeskHandler OTCTradingDeskHandler
        {
            get
            {
                return MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            }
        }
        public TradeDifferWindow()
        {
            InitializeComponent();
            //TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        }
        public void QueryTradeDiffer()
        {
            if (TradeHandler != null)
                TradeHandler.QueryTradeDiffer();
            TradeListView.ItemsSource = TradeHandler.TradeDifferVMCollection;
            if (ETFTradeHandler != null)
                ETFTradeHandler.QueryTradeDiffer();
            ETFTradeListView.ItemsSource = ETFTradeHandler.TradeDifferVMCollection;
            if (StockTradeHandler != null)
                StockTradeHandler.QueryTradeDiffer();
            StockTradeListView.ItemsSource = StockTradeHandler.TradeDifferVMCollection;
        }
        private async void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (TradeSyncList != null)
            {
                foreach (var tradeDiffer in TradeSyncList)
                {
                    await TradeHandler.SyncTradeAsync(tradeDiffer);
                }
            }
            QueryTradeDiffer();
        }
        private async void ETFButton_Click_Add(object sender, RoutedEventArgs e)
        {
            if (ETFTradeSyncList != null)
            {
                foreach (var tradeDiffer in ETFTradeSyncList)
                {
                    await ETFTradeHandler.SyncTradeAsync(tradeDiffer);
                }
            }
            QueryTradeDiffer();
        }
        private async void StockButton_Click_Add(object sender, RoutedEventArgs e)
        {
            if (StockTradeSyncList != null)
            {
                foreach (var tradeDiffer in StockTradeSyncList)
                {
                    await StockTradeHandler.SyncTradeAsync(tradeDiffer);
                }
            }
            QueryTradeDiffer();
        }
        private void tradeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                TradeSyncList.Add(tradeDifferVM);
            }
        }
        private void etftradeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                ETFTradeSyncList.Add(tradeDifferVM);
            }
        }
        private void stocktradeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                StockTradeSyncList.Add(tradeDifferVM);
            }
        }
        private void tradeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                TradeSyncList.Remove(tradeDifferVM);
            }
        }
        private void etftradeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                ETFTradeSyncList.Remove(tradeDifferVM);
            }
        }
        private void stocktradeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                TradeDifferVM tradeDifferVM = ctrl.DataContext as TradeDifferVM;
                StockTradeSyncList.Remove(tradeDifferVM);
            }
        }
        private void TradeListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, TradeListView.Items);
            }
        }
        private void ETFTradeListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, ETFTradeListView.Items);
            }
        }
        private void StockTradeListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, StockTradeListView.Items);
            }
        }
    }
}
