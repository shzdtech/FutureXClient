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

        public TraderExHandler TradeHandler
        {
            get
            {
                return OrderVM?.TradeHandler;
            }
            set
            {
                OrderVM = new OrderVM(value);
                OrderVM.Volume = 1;
                DataContext = OrderVM;
            }
        }

        public bool SubmitEnabled
        {
            get;
            set;
        }

        public void OnQuoteSelected(QuoteViewModel quoteVM)
        {
            if (quoteVM != null)
            {
                _currentContract = quoteVM.Contract;
                stackPanelPrices.DataContext = quoteVM;
                OrderVM.Contract = quoteVM.Contract;
                OrderVM.LimitPrice = quoteVM.MatchPrice;
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
            InitializeComponent();
        }

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


       

        private void LimitTxt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            /*
            var source = "^[-]?[0-9]{1,5}";
            if (Regex.IsMatch(LimitTxt.Value.ToString(), source) == true)
            {
                MessageBox.Show("只可以输入数字！");
                SizeTxt.Value = 1;
            }
            */
        }

        

        private void BuySummitButton_Click(object sender, RoutedEventArgs e)
        {
            
            var sizeSource = "^[1-9][0-9]{1,5}";
            if (Regex.IsMatch(SizeTxt.Value.ToString(), sizeSource) == true)
            {
                if (SizeTxt.Value < 1)
                {
                    MessageBox.Show("输入数值至少为1");
                    SizeTxt.Value = 1;
                }
            }
            else
            {
                MessageBox.Show("只可以输入正整数,且单笔购买数量最多为10万！");
                SizeTxt.Value = 1;
                return;
            }
        }


        //


    }
}
