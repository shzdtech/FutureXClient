using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using WpfControls;
using Xceed.Wpf.Toolkit;
using Micro.Future.Utility;
using System.Globalization;
using Micro.Future.Message;
using Micro.Future.ViewModel;

namespace Micro.Future.UI.OptionControls
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class OpOrderWin : Window
    {
        private string _currentContract;
        public IList<ContractInfo> FuturecontractList
        {
            get
            {
                return ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            }
        }
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
        public OrderVM OrderVM
        {
            get;
            private set;
        }
        public OpOrderWin()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            FastOrderContract.Provider = new SuggestionProvider((string c) => { return FuturecontractList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
            TradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
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
        public void OnQuoteSelected(PricingVM quoteVM)
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
                OrderVM.LimitPrice = quoteVM.MidPrice;
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
