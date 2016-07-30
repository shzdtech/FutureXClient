using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class AccountInfoControl : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;

        public AccountInfoControl()
        {
            InitializeComponent();

            FundListView.ItemsSource =
                MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVMCollection;

            mColumns = ColumnObject.GetColumns(FundListView);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryAccountInfo();
        }

        private void MenuItemColumns_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
    }
}
