using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionRiskCtrl : UserControl, ILayoutAnchorableControl

    {
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public OptionRiskCtrl()
        {
            InitializeComponent();
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            var domesticTradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            var otcTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            domesticPositionsWindow.TradeHandler = domesticTradeHandler;
            otcPositionsWindow.TradeHandler = otcTradeHandler;
            domesticTradeWindow.TradeHandler = domesticTradeHandler;
            otcTradeWindow.TradeHandler = otcTradeHandler;
            marketDataLV.MarketDataHandler = marketdataHandler;
            portfolioCtl.portfolioCB.SelectionChanged += PortfolioCB_SelectionChanged;
        }
        private async void PortfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var portfolio = portfolioCtl.portfolioCB.SelectedValue.ToString();
            var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
            var basecontractsList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                    .Select(c => c.BaseContract).Distinct().ToList();
            var pricingContractParams = strategyVMCollection.Select(c => c.PricingContractParams).Distinct();
            //var pricingContracts = pricingContractParams.Select(c => c.)
            QuoteVMCollection.Clear();
            foreach (var contract in basecontractsList)
            {
                var mktDataVM = await marketDataLV.MarketDataHandler.SubMarketDataAsync(contract);
                if (mktDataVM != null)
                {
                    QuoteVMCollection.Add(mktDataVM);
                }
            }
            marketDataLV.quoteListView.ItemsSource = QuoteVMCollection;
        }
        private LayoutAnchorablePane _pane;
        public LayoutAnchorablePane AnchorablePane
        {
            get
            {
                return _pane;
            }
            set
            {
                _pane = value;
                portfolioCtl.LayoutContent = _pane.SelectedContent;
            }
        }
    }
}
