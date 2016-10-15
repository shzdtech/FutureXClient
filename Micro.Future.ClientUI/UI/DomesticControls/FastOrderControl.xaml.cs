using System.Windows.Controls;
using System.Windows.Input;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System.Text.RegularExpressions;
using System;
using System.Windows.Media;
using System.Collections;

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
                value.OnOrderError += Callback_OnOrderError; 
            }
        }

        private void Callback_OnOrderError(Exception obj)
        {
            if (obj.Message.Equals("订单合约不能为空") | obj.Message.Equals("输入合约不存在"))
            { FastOrderContract.Background = new SolidColorBrush(Colors.Red);
                MessageBox.Show(obj.Message);
                FastOrderContract.Background = new SolidColorBrush(Colors.White); }
            if (obj.Message.Equals("订单数量不正确"))
            {
                SizeTxt.Background = new SolidColorBrush(Colors.Red);
                MessageBox.Show(obj.Message);
                SizeTxt.Background = new SolidColorBrush(Colors.White);
            } 

        }

        public bool SubmitEnabled
        {
            get;
            set;
        }

        public void OnQuoteSelected(MarketDataVM quoteVM)
        {
            if (quoteVM != null)
            {
                _currentContract = quoteVM.Contract;
                stackPanelPrices.DataContext = quoteVM;
                OrderVM.Contract = quoteVM.Contract;
                OrderVM.LimitPrice = quoteVM.LastPrice;
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
            //To bound data for portolioCB          
            //MessageBox.Show(MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().PortfolioVMCollection.ToString());  
            
            portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().PortfolioVMCollection;


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




    }
}
