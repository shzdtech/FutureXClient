using System.Windows.Controls;
using System.Windows.Input;
using Micro.Future.ViewModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class ClientFastOrderWindow : UserControl
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

        public ClientFastOrderWindow()
        {
            OrderVM = new OrderVM();
            InitializeComponent();
            DataContext = OrderVM;
        }

        private void label1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value =(double)label1.Content;
        }

        private void label3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = (double)label3.Content;
        }
    }
}
