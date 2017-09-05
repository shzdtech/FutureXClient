using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
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
using WpfControls;
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class AddTradeRecordWindow : Window
    {
        public TradeInfoVM TradeInfoVM
        {
            get;
        } = new TradeInfoVM(null);
        public BaseTraderHandler TradeHandler
        {
            get
            {
                return TradeInfoVM?.TradeHandler;
            }
            set
            {
                DataContext = TradeInfoVM;
            }
        }
        public List<ContractInfo> FutureContractList
        {
            get;
        } = new List<ContractInfo>();
        public AddTradeRecordWindow()
        {
            InitializeComponent();
            //TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            DataContext = TradeInfoVM;
            GetContractInfo();
            //exchangeCB.ItemsSource = FutureContractList.Select(c => c.Exchange).Distinct();
            portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection;
            radioButtonBuy.IsChecked = true;
            RadioA.IsChecked = true;
            SizeTxt.Value = 1;
        }
        public void GetContractInfo()
        {
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_ETFOPTION));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_STOCK));
            FastOrderContract.Provider = new SuggestionProvider((string c) => { return FutureContractList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        }
        private async void LoadContract()
        {
            if (FastOrderContract.SelectedItem == null)
            {
                FastOrderContract.SelectedItem = FastOrderContract.Filter ?? string.Empty;
            }
            var contract = FastOrderContract.SelectedItem.ToString();
            if (FutureContractList.Any(c => c.Contract == contract))
            {
                TradeInfoVM.Contract = contract;
                var quote = TradeInfoVM.Contract;
                var item = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketDataAsync(quote);
                if (item != null)
                {
                    var contractInfo = ClientDbContext.FindContract(quote);
                    LimitTxt.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                    radioButtonBuy.IsChecked = true;
                    RadioA.IsChecked = true;
                    //LimitTxt.SetBinding(DoubleUpDown.ValueProperty, new Binding("AskPrice.Value") { Mode = BindingMode.OneWay });
                    LimitTxt.Value = item.LastPrice;
                    SizeTxt.Value = 1;
                }
            }
        }

        private void FastOrderContract_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadContract();
            }
        }

        private void FastOrderContract_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadContract();

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var contract = FastOrderContract.SelectedItem == null ? FastOrderContract.Filter : FastOrderContract.SelectedItem.ToString();
            if (LimitTxt.Text != null & !string.IsNullOrEmpty(contract) & SizeTxt != null)
            {
                //TradeInfoVM.Exchange = exchangeCB.SelectedItem.ToString();
                TradeInfoVM.Price = LimitTxt.Value.Value;
                TradeInfoVM.Contract = contract;
                TradeInfoVM.Volume = (int)SizeTxt.Value;
                TradeInfoVM.Portfolio = portofolioCB.SelectedValue?.ToString();
            }
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().AddTrade(TradeInfoVM);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
