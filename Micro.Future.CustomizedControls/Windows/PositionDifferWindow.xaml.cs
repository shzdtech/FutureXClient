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
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class PositionDifferWindow : Window
    {
        public BaseTraderHandler TradeHandler { get; set; }
        public List<PositionDifferVM> PositionSyncList { get; } = new List<PositionDifferVM>();
        public ObservableCollection<PortfolioVM> PortfolioCollection
        {
            get {return MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection; }
        }
        public List<String> PortfolioNameCollection
        {
            get;
        }
        public PositionDifferWindow()
        {
            InitializeComponent();
            TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            PortfolioNameCollection = PortfolioCollection.Select(c => c.Name).ToList();
            TradeHandler.QueryPositionDiffer();
            PositionListView.ItemsSource = TradeHandler.PositionDifferVMCollection;
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (PositionSyncList != null)
            {
                TradeHandler.SyncPosition(PositionSyncList);
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
        private void positionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                PositionDifferVM positionDifferVM = ctrl.DataContext as PositionDifferVM;
                PositionSyncList.Remove(positionDifferVM);
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
    }
}
