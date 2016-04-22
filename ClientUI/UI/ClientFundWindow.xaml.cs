using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ClientFundWindow : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;

        public ClientFundWindow()
        {
            InitializeComponent();
            var fundVMCollection = new DispatchObservableCollection<FundVM>(this);
            FundListView.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVMCollection =
            fundVMCollection;

            mColumns = ColumnObject.GetColumns(FundListView);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryAccountInfo();
        }
    }
}
