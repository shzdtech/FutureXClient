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
        public void QueryPositionDiffer()
        {
            if (TradeHandler != null)
                TradeHandler.QueryPositionDiffer();
            PositionListView.ItemsSource = TradeHandler.PositionDifferVMCollection;
            if (ETFTradeHandler != null)
                ETFTradeHandler.QueryPositionDiffer();
            ETFPositionListView.ItemsSource = ETFTradeHandler.PositionDifferVMCollection;
            if (StockTradeHandler != null)
                StockTradeHandler.QueryPositionDiffer();
            StockPositionListView.ItemsSource = StockTradeHandler.PositionDifferVMCollection;
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

    }
}
