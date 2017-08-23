using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
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
using System.Windows.Shapes;

namespace Micro.Future.CustomizedControls.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class PositionDifferWindow : Window
    {
        public BaseTraderHandler TradeHandler { get; set; }

        public PositionDifferWindow()
        {
            InitializeComponent();
            TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            TradeHandler.QueryPositionDiffer();
            PositionListView.ItemsSource = TradeHandler.PositionDifferVMCollection;
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            TradeHandler.SyncPosition();
            this.Close();
        }
    }
}
