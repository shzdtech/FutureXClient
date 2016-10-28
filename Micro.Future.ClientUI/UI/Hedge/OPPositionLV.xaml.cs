using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class OPPositionLV : UserControl
    {
        private ColumnObject[] mColumns;

        public OPPositionLV()
        {
            InitializeComponent();
            PositionListView.ItemsSource = MessageHandlerContainer.
                DefaultInstance.Get<TraderExHandler>().PositionVMCollection;

            mColumns = ColumnObject.GetColumns(PositionListView);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        //private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        //{
        //    PositionVM vm = PositionListView.SelectedItem as PositionVM;
        //    if (vm != null)
        //    {
        //        MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().CloseMarketOrder(vm);
        //    }
        //    else
        //    {
        //        MessageBox.Show("请选择持仓合约", "错误");
        //    }
        //}
    }
}
