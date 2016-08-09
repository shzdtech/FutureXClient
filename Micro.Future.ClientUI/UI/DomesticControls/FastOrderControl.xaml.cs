using System.Windows.Controls;
using System.Windows.Input;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System.Text.RegularExpressions;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderControl : UserControl
    {
        private string _currentContract;

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
                _currentContract = quoteVM.Contract;
                DataContext = quoteVM;
                OrderVM.Contract = quoteVM.Contract;

            }
        }

        public void OnPositionSelected(PositionVM positionVM)
        {
            if (positionVM != null)
            {
                OrderVM.Contract = positionVM.Contract;
                OrderVM.OffsetFlag = OrderOffsetType.CLOSE;

                if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                {
                    OrderVM.Direction = DirectionType.BUY;
                }
                else
                {
                    OrderVM.Direction = DirectionType.SELL;
                }
            }
        }

        public OrderVM OrderVM
        {
            get;
            private set;
        }

        public FastOrderControl()
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


        private void LabelBidPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
        }

        private void LabelAskPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
        }

        private void FastOrderContract_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentContract != null && FastOrderContract.Text != _currentContract)
                stackPanelPrices.DataContext = null;
        }

    

        private void SizeTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            var source = "^[0-9]*$";

            if (Regex.IsMatch(SizeTxt.Value.ToString(), source) == true)
            {
                if (SizeTxt.Value < 1)
                {
                    MessageBox.Show("输入数值至少为1");
                    SizeTxt.Value = 1;
                }
            }
            else
            {
                MessageBox.Show("只可以输入数字！");
                SizeTxt.Value = 1;
            }
            
        }

     
        
        private void LimitTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            var source = "^[-]?[0-9]{1,5}";
            if (Regex.IsMatch(LimitTxt.Value.ToString(), source) == true)
            {
                MessageBox.Show("只可以输入数字！");
                SizeTxt.Value = 1;
            }
        }

        


        //


    }
}
