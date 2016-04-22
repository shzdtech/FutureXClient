using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class PositionWindow : UserControl
    {
        private ColumnObject[] mColumns;

        public PositionWindow()
        {
            InitializeComponent();

            mColumns = ColumnObject.GetColumns(PositionListView);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            PositionViewModel vm = PositionListView.SelectedItem as PositionViewModel;
            if (vm != null)
            {
                TradeHandler.Instance.CloseMarketOrder(vm);
            }
            else
            {
                MessageBox.Show("请选择持仓合约", "错误");
            }
        }
    }
}
