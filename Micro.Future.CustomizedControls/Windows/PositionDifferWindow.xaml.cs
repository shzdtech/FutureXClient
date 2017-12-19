using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
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
    public partial class PositionDifferWindow : Window
    {
        public BaseTraderHandler TradeHandler { get; set; }
        public BaseTraderHandler ETFTradeHandler { get; set; }
        public BaseTraderHandler StockTradeHandler { get; set; }
        public List<PositionDifferVM> PositionSyncList { get; } = new List<PositionDifferVM>();
        public List<PositionDifferVM> ETFPositionSyncList { get; } = new List<PositionDifferVM>();
        public List<PositionDifferVM> StockPositionSyncList { get; } = new List<PositionDifferVM>();
        public int TotalSysPosition { get; set; }
        public int TotalPosition { get; set; }
        public int TotalETFSysPosition { get; set; }
        public int TotalETFPosition { get; set; }
        public int TotalStockSysPosition { get; set; }
        public int TotalStockPosition { get; set; }
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
        public PositionDifferWindow()
        {
            InitializeComponent();
            //TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();

        }
        public void FutureQueryDiffer()
        {
            SyncButton.IsEnabled = true;
            TotalSysPosition = 0;
            TotalPosition = 0;
            if (TradeHandler != null)
            {
                TradeHandler.QueryPositionDiffer();
                TradeHandler.QueryPosition();
                PositionListView.ItemsSource = TradeHandler.PositionDifferVMCollection;

                foreach (var positiondiffer in TradeHandler.PositionDifferVMCollection)
                {
                    TotalSysPosition = TotalSysPosition + positiondiffer.SysPosition;
                }
                foreach (var position in TradeHandler.PositionVMCollection)
                {
                    TotalPosition = TotalPosition + position.Position;
                }
                if (TotalSysPosition == 0 && TotalPosition != 0)
                {
                    PositionListView.ItemsSource = null;
                    SyncButton.IsEnabled = false;
                }
            }
        }
        public void ETFQueryDiffer()
        {
            ETFSyncButton.IsEnabled = true;
            TotalETFSysPosition = 0;
            TotalETFPosition = 0;
            if (ETFTradeHandler != null)
            {
                ETFTradeHandler.QueryPositionDiffer();
                ETFTradeHandler.QueryPosition();
                ETFPositionListView.ItemsSource = ETFTradeHandler.PositionDifferVMCollection;

                foreach (var positiondiffer in ETFTradeHandler.PositionDifferVMCollection)
                {
                    TotalETFSysPosition = TotalETFSysPosition + positiondiffer.SysPosition;
                }
                foreach (var position in ETFTradeHandler.PositionVMCollection)
                {
                    TotalETFPosition = TotalETFPosition + position.Position;
                }
                if (TotalETFSysPosition == 0 && TotalETFPosition != 0)
                {
                    ETFPositionListView.ItemsSource = null;
                    ETFSyncButton.IsEnabled = false;
                }
            }
        }
        public void StockQueryDiffer()
        {
            StockSyncButton.IsEnabled = true;
            TotalStockPosition = 0;
            TotalStockSysPosition = 0;
            if (StockTradeHandler != null)
            {
                StockTradeHandler.QueryPositionDiffer();
                StockTradeHandler.QueryPosition();
                StockPositionListView.ItemsSource = StockTradeHandler.PositionDifferVMCollection;

                foreach (var positiondiffer in StockTradeHandler.PositionDifferVMCollection)
                {
                    TotalStockSysPosition = TotalStockSysPosition + positiondiffer.SysPosition;
                }
                foreach (var position in StockTradeHandler.PositionVMCollection)
                {
                    TotalStockPosition = TotalStockPosition + position.Position;
                }
                if (TotalStockSysPosition == 0 && TotalStockPosition != 0)
                {
                    StockPositionListView.ItemsSource = null;
                    StockSyncButton.IsEnabled = false;
                }
            }
        }

        public void QueryPositionDiffer()
        {
            FutureQueryDiffer();
            ETFQueryDiffer();
            StockQueryDiffer();
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (PositionSyncList != null)
            {
                TradeHandler.SyncPosition(PositionSyncList);
                Close();
            }
        }
        private void ETFButton_Click_Add(object sender, RoutedEventArgs e)
        {
            if (ETFPositionSyncList != null)
            {
                ETFTradeHandler.SyncPosition(ETFPositionSyncList);
                Close();
            }
        }
        private void StockButton_Click_Add(object sender, RoutedEventArgs e)
        {
            if (StockPositionSyncList != null)
            {
                StockTradeHandler.SyncPosition(StockPositionSyncList);
                Close();
            }
        }
        private void positionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                PositionSyncList.Add(positionDifferVM);
            }
        }
        private void etfpositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                ETFPositionSyncList.Add(positionDifferVM);
            }
        }
        private void stockpositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                StockPositionSyncList.Add(positionDifferVM);
            }
        }
        private void positionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                PositionSyncList.Remove(positionDifferVM);
            }
        }
        private void etfpositionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                ETFPositionSyncList.Remove(positionDifferVM);
            }
        }
        private void stockpositionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                StockPositionSyncList.Remove(positionDifferVM);
            }
        }
        private void PositionListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, PositionListView.Items);
            }
        }
        private void ETFPositionListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, ETFPositionListView.Items);
            }
        }
        private void StockPositionListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, StockPositionListView.Items);
            }
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            FutureQueryDiffer();
        }
        private void ETFQueryButton_Click(object sender, RoutedEventArgs e)
        {
            ETFQueryDiffer();
        }
        private void StockQueryButton_Click(object sender, RoutedEventArgs e)
        {
            StockQueryDiffer();
        }
    }
}
