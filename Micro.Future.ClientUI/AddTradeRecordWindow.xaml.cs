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
            TradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            DataContext = TradeInfoVM;
            portofolioCB.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection;
            radioButtonBuy.IsChecked = true;
            RadioA.IsChecked = true;
        }
        public void GetContractInfo()
        {
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_ETFOPTION));
            FutureContractList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_STOCK));
            FastOrderContract.Provider = new SuggestionProvider((string c) => { return FutureContractList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        }
        private void LoadContract()
        {
            if (FastOrderContract.SelectedItem == null)
            {
                FastOrderContract.SelectedItem = FastOrderContract.Filter ?? string.Empty;
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
                //TradeInfoVM.Price = LimitTxt.Text;
                TradeInfoVM.Contract = contract;
                TradeInfoVM.Volume = (int)SizeTxt.Value;
                TradeInfoVM.Portfolio = portofolioCB.SelectedValue?.ToString();
            }
            TradeHandler.AddTrade(TradeInfoVM);
        }

    }
}
