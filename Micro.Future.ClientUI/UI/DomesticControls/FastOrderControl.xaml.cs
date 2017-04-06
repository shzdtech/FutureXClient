using System.Windows.Controls;
using System.Windows.Input;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using WpfControls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using System.Threading.Tasks;
using Micro.Future.Utility;
using System.Globalization;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderControl : UserControl
    {
        private string _currentContract;
        public IList<ContractInfo> FuturecontractList
        {
            get
            {
                return ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            }
        }
        public BaseTraderHandler TradeHandler
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


        public FastOrderControl()
        {
            InitializeComponent();
            //To bound data for portolioCB          
            //MessageBox.Show(MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().PortfolioVMCollection.ToString());  
            Initialize();
        }

        private void Initialize()
        {
            portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            FastOrderContract.Provider = new SuggestionProvider((string c) => { return FuturecontractList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        }



        private void Callback_OnOrderError(Exception obj)
        {
            if (obj.Message.Equals("订单合约不能为空") | obj.Message.Equals("输入合约不存在"))
            {
                FastOrderContract.Background = new SolidColorBrush(Colors.Red);
                System.Windows.MessageBox.Show(obj.Message);
                FastOrderContract.Background = new SolidColorBrush(Colors.White);
            }
            if (obj.Message.Equals("订单数量不正确"))
            {
                SizeTxt.Background = new SolidColorBrush(Colors.Red);
                System.Windows.MessageBox.Show(obj.Message);
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
                checkBox.IsEnabled = true;
                radioButtonBuy.IsChecked = true;
                //radioButtonSell.IsChecked = false;
                RadioA.IsChecked = true;
                //RadioB.IsChecked = false;
                //RadioC.IsChecked = false;
                _currentContract = quoteVM.Contract;
                OrderVM.Contract = quoteVM.Contract;
                FastOrderContract.SelectedItem = OrderVM.Contract;
                stackPanelPrices.DataContext = quoteVM;
                OrderVM.LimitPrice = quoteVM.LastPrice;
                var contractInfo = ClientDbContext.FindContract(OrderVM.Contract);
                LimitTxt.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                if (radioButtonBuy.IsChecked.Value)
                {
                    if (LabelAskPrice.Content != null)
                        LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
                }
                else if (radioButtonSell.IsChecked.Value)
                {
                    if (LabelBidPrice.Content != null)
                        LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
                }
                if (checkBox.IsChecked.Value)
                    LimitTxt.Increment = null;
            }
        }

        public void OnPositionSelected(PositionVM positionVM)
        {
            if (positionVM != null)
            {
                OrderVM.Contract = positionVM.Contract;
                var quote = OrderVM.Contract;
                if (quote != null)
                {
                    checkBox.IsEnabled = true;
                    radioButtonBuy.IsChecked = true;
                    //radioButtonSell.IsChecked = false;
                    RadioA.IsChecked = true;
                    //RadioB.IsChecked = false;
                    //RadioC.IsChecked = false;
                    FastOrderContract.SelectedItem = quote;
                    //OrderVM.Volume = positionVM.Position;
                    Task.Run(async () =>
                    {
                        var item = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketDataAsync(quote);
                        if (item != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                stackPanelPrices.DataContext = item;
                                var contractInfo = ClientDbContext.FindContract(quote);
                                LimitTxt.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                                if (radioButtonBuy.IsChecked.Value)
                                {
                                    if (LabelAskPrice.Content != null)
                                        LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
                                }
                                else if (radioButtonSell.IsChecked.Value)
                                {
                                    if (LabelBidPrice.Content != null)
                                        LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
                                }
                                if (checkBox.IsChecked.Value)
                                    LimitTxt.Increment = null;
                            });
                        }
                    });

                    //else
                    //{
                    //    MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
                    //}

                    //OrderVM.OpenClose = OrderOpenCloseType.CLOSE;

                    //if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                    //{
                    //    OrderVM.Direction = DirectionType.BUY;
                    //}
                    //else
                    //{
                    //    OrderVM.Direction = DirectionType.SELL;
                    //}
                }
            }
        }

        private void SendOrder(object sender, RoutedEventArgs e)
        {
            var contract = FastOrderContract.SelectedItem == null ? FastOrderContract.Filter : FastOrderContract.SelectedItem.ToString();
            if (LimitTxt.Value != null & !string.IsNullOrEmpty(contract) & SizeTxt != null)
            {
                OrderVM.LimitPrice = LimitTxt.Value.Value;
                OrderVM.Contract = contract;
                OrderVM.Volume = (int)SizeTxt.Value;
                var cvt = new EnumToFriendlyNameConverter();
                string msg = string.Format("是否确认下单?\n合约：{0}，价格：{1}，手数：{2}, 方向：{3}，开平：{4}", OrderVM.Contract, OrderVM.LimitPrice, OrderVM.Volume,
                    cvt.Convert(OrderVM.Direction, typeof(DirectionType), null, CultureInfo.CurrentUICulture),
                    cvt.Convert(OrderVM.OpenClose, typeof(OrderOpenCloseType), null, CultureInfo.CurrentUICulture));
                MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    OrderVM.SendOrder();
                }
            }
        }


        public OrderVM OrderVM
        {
            get;
            private set;
        }

        private void labelupperprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!checkBox.IsChecked.Value)
            { LimitTxt.Value = double.Parse(LabelUpperPrice.Content.ToString()); }
        }

        private void labellowerprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!checkBox.IsChecked.Value)
                LimitTxt.Value = double.Parse(LabelLowerPrice.Content.ToString());
        }


        private void LabelBidPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!checkBox.IsChecked.Value)
                LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
        }

        private void LabelAskPrice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!checkBox.IsChecked.Value)
                LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
        }

        private async void LoadContract()
        {
            if (FastOrderContract.SelectedItem == null)
            {
                FastOrderContract.SelectedItem = FastOrderContract.Filter ?? string.Empty;
            }

            var contract = FastOrderContract.SelectedItem.ToString();
            if (FuturecontractList.Any(c => c.Contract == contract))
            {
                OrderVM.Contract = contract;
                var quote = OrderVM.Contract;
                var item = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketDataAsync(quote);
                if (item != null)
                {
                    stackPanelPrices.DataContext = item;
                    var contractInfo = ClientDbContext.FindContract(quote);
                    LimitTxt.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                    checkBox.IsEnabled = true;
                    radioButtonBuy.IsChecked = true;
                    //radioButtonSell.IsChecked = false;
                    RadioA.IsChecked = true;
                    //RadioB.IsChecked = false;
                    //RadioC.IsChecked = false;
                    LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("AskPrice.Value") { Mode = BindingMode.OneWay });
                }
            }
            else
            {
                checkBox.IsChecked = false;
                checkBox.IsEnabled = false;
            }
        }

        private void FastOrderContract_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadContract();
                if (radioButtonBuy.IsChecked.Value)
                {
                    if (LabelAskPrice.Content != null)
                        LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
                }
                else if (radioButtonSell.IsChecked.Value)
                {
                    if (LabelBidPrice.Content != null)
                        LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
                }
            }
        }

        private void FastOrderContract_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadContract();
            if (radioButtonBuy.IsChecked.Value)
            {
                if (LabelAskPrice.Content != null)
                    LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
            }
            else if (radioButtonSell.IsChecked.Value)
            {
                if (LabelBidPrice.Content != null)
                    LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
            }

        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            //var quote = FastOrderContract.Text;
            //var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
            //if (!string.IsNullOrEmpty(FastOrderContract.Text))
            //{
            //checkBox.IsEnabled = true;
            if (OrderVM.Direction == DirectionType.BUY)
            {
                LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("AskPrice.Value") { Mode = BindingMode.OneWay });
                LimitTxt.Increment = null;
            }
            else if (OrderVM.Direction == DirectionType.SELL)
            {
                LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("BidPrice.Value") { Mode = BindingMode.OneWay });
                LimitTxt.Increment = null;
            }
            //}
            //else
            //    checkBox.IsEnabled = false;
        }

        private void BuyChecked(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked.Value)
                LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("AskPrice.Value") { Mode = BindingMode.OneWay });
            else if (LabelAskPrice.Content != null)
                LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
        }

        private void SellChecked(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked.Value)
                LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("BidPrice.Value") { Mode = BindingMode.OneWay });
            else if (LabelBidPrice.Content != null)
                LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearAllBindings(LimitTxt);
            var contractInfo = ClientDbContext.FindContract(OrderVM.Contract ?? string.Empty);
            LimitTxt.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;            
                if (radioButtonBuy.IsChecked.Value)
            {
                if (LabelAskPrice.Content != null)
                    LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
            }
                else if (radioButtonSell.IsChecked.Value)
            {
                if (LabelBidPrice.Content != null)
                    LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
            }
        }
    }
}
