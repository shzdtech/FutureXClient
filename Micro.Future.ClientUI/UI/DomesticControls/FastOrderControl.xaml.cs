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

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderControl : UserControl
    {
        private string _currentContract;
        private IList<ContractInfo> _futurecontractList;
        //private List<string> SuggestContract;
        //        private IEnumerable<string> SuggestContract;
        private ISuggestionProvider provider;


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
            this._futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            //this.SuggestContract = _futurecontractList.Select(ci => ci.Contract).Distinct().ToList();

            //behaviors:AutoCompleteBehavior.AutoCompleteItemsSource="{Binding SuggestContract1}"
            //WPFTextBoxAutoComplete.AutoCompleteBehavior.SetAutoCompleteItemsSource(FastOrderContract, SuggestContract);
            //behaviors: AutoCompleteBehavior.AutoCompleteStringComparison = "InvariantCultureIgnoreCase"
            //WPFTextBoxAutoComplete.AutoCompleteBehavior.SetAutoCompleteStringComparison(FastOrderContract, StringComparison.InvariantCultureIgnoreCase);
            //回调函数
            //FastOrderContract.Provider = new SuggestionProvider(  (string c)=>{ return _futurecontractList.Select(ci => ci.Contract.StartsWith(c));}  );
            this.provider = new SuggestionProvider((string c) => { return _futurecontractList.Where(ci => ci.Contract.StartsWith(c)).Select(cn=>cn.Contract); });
            FastOrderContract.Provider = this.provider;
            


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
                var quote = OrderVM.Contract;
                var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote); ;
                if (item != null)
                {
                    stackPanelPrices.DataContext = item;
                }
                //else
                //{
                //    MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
                //}

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

        /*
        private void FastOrderContract_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentContract != null && FastOrderContract.Text != _currentContract)
                stackPanelPrices.DataContext = null;
        }

        private void SendOrderButton(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("是否确认下单", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                OrderVM.SendOrder();
            }

        }


        //function for Popup

        //public List<string> listContract;
        //public Popup pop = new Popup();

        //private void FastOrderContract_KeyUp(object sender, KeyEventArgs e)
        //{
        //    //MessageBox.Show("Pop");
        //    this.showPopup();
        //}

        ////To remove Popup
        //private void FastOrderContract_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (this.pop.IsMouseOver == false)
        //    {
        //        this.removePopup();
        //    }
        //}

        //private void removePopup()
        //{
        //    if (pop != null)
        //    {
        //        //MessageBox.Show("Pop is not null");
        //        this.pop.IsOpen = false;
        //        //PopupFastOrderContract.
        //    }
        //}

        //private void showPopup()
        //{
        //    if (listContract == null)
        //    {
        //        if (MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().getContractNameList() != null)
        //        {

        //            listContract = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().getContractNameList();
        //        }
        //        else
        //        {
        //            MessageBox.Show("listContract is exist");
        //        }

        //    }

        //   var item = listContract.Where(a => a.Contains(FastOrderContract.Text));
        //   if (item.ToList<string>().Count > 0)
        //   {
        //        pop = this.createPopup(pop, item.ToList<string>(), FastOrderContract);
        //        pop.IsOpen = true;
        //   }
        //   else
        //   {
        //        pop.IsOpen = false;
        //   }

        //}

        //private Popup createPopup(Popup pop, List<string> listSource, UIElement element)
        //{
        //    Border border = new Border();
        //    border.BorderBrush = new SolidColorBrush(Colors.Black);
        //    //border.BorderThickness = new Thickness(1.0);//设置边框宽度  
        //    StackPanel panel1 = new StackPanel();
        //    panel1.Children.Clear();
        //    panel1.Background = new SolidColorBrush(Colors.LightGray);
        //    ListBox listbox = new ListBox();
        //    listbox.Background = new SolidColorBrush(Colors.WhiteSmoke);
        //    listbox.MinWidth = 100;
        //    listbox.Height = 120;
        //    listbox.ItemsSource = listSource;
        //    listbox.MouseDoubleClick += new MouseButtonEventHandler(listbox_MouseDoubleClick);
        //    panel1.Children.Add(listbox);
        //    border.Child = panel1;
        //    pop.Child = border;
        //    pop.PlacementTarget = element;
        //    return pop;
        //}

        //public void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    ListBox box = sender as ListBox;
        //    string itemvalue = box.SelectedValue as string;
        //    //this.PopupFastOrderContract.Text = itemvalue;
        //    this.FastOrderContract.Text = itemvalue;
        //    pop.IsOpen = false;
        //}

        //private void portofolioCB_KeyDown(object sender, KeyEventArgs e)
        //{
        //    MessageBox.Show("itemsource");
        //    if(this.portofolioCB.ItemsSource == null)
        //    {
        //        this.portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().PortfolioVMCollection;
        //    }

        //}
    }
}
