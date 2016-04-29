using System.Windows.Controls;
using System.Windows.Input;
using Micro.Future.ViewModel;
using Micro.Future.Message;

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

        public void OnQuoteSelected(QuoteViewModel quoteVM)
        {
            if (quoteVM != null)
            {
                LabelUpperPrice.DataContext = quoteVM;
                LabelLowerPrice.DataContext = quoteVM;
                LabelBidPrice.DataContext = quoteVM;
                LabelAskPrice.DataContext = quoteVM;
                OrderVM.Contract = quoteVM.Symbol;

            }
        }

        public void OnPositionSelected(PositionVM positionVM)
        {
            if (positionVM != null)
            {
                OrderVM.Contract = positionVM.Contract;
                OrderVM.OffsetFlag = OrderOffsetType.CLOSE;
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

        private OrderVM _orderVM = new OrderVM();

        private void labelupperprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelUpperPrice.Content.ToString());
        }

        private void labellowerprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelLowerPrice.Content.ToString());
        }
    }
}
