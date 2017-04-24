using Micro.Future.Message;
using Micro.Future.Utility;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class PortfolioSelectCtrl : UserControl
    {
        public LayoutContent LayoutContent { get; set; }

        public PortfolioSelectCtrl()
        {
            InitializeComponent();
            portfolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
        }
        private void strategyListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, strategyListView.Items);
            }
        }

        private void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue.ToString();

            }

        }
    }
}
