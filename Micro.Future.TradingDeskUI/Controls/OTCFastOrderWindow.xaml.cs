using System.Windows.Controls;
using Micro.Future.ViewModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class OTCFastOrderWindow : UserControl
    {
        private bool submitEnabled;
        public bool SubmitEnabled
        {
            get
            {
                return submitEnabled;
            }
            set
            {
                submitEnabled = value;
            }
        }

        public OrderVM OrderVM
        {
            get;
            private set;
        }

        public OTCFastOrderWindow()
        {
            OrderVM = new OrderVM();
            InitializeComponent();
            DataContext = OrderVM;
        }
        

    }
}
