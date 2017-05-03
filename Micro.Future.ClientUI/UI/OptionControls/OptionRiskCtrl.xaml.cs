using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class OptionRiskCtrl : UserControl, ILayoutAnchorableControl, IReloadData

    {
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private Timer _timer;
        private const int UpdateInterval = 1000;
        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public OptionRiskCtrl()
        {
            InitializeComponent();
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            var otcmarketdataHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
            var domesticTradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            var otcTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            domesticPositionsWindow.TradeHandler = domesticTradeHandler;
            domesticPositionsWindow.MarketDataHandler = marketdataHandler;
            otcPositionsWindow.TradeHandler = otcTradeHandler;
            otcPositionsWindow.MarketDataHandler = otcmarketdataHandler;
            domesticTradeWindow.TradeHandler = domesticTradeHandler;
            otcTradeWindow.TradeHandler = otcTradeHandler;
            marketDataLV.MarketDataHandler = marketdataHandler;
            marketDataLV.AnchorablePane = quotePane;
            quotePane.Children[0].Title = WPFUtility.GetLocalizedString("Quote", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            domesticPositionsWindow.AnchorablePane = domesticPositionPane;
            domesticPositionPane.Children[0].Title = WPFUtility.GetLocalizedString("PositionWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            otcPositionsWindow.AnchorablePane = otcPositionPane;
            otcPositionPane.Children[0].Title = WPFUtility.GetLocalizedString("PositionWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            domesticTradeWindow.AnchorablePane = domesticTradePane;
            domesticTradePane.Children[0].Title = WPFUtility.GetLocalizedString("TradeWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            otcTradeWindow.AnchorablePane = otcTradePane;
            otcTradePane.Children[0].Title = WPFUtility.GetLocalizedString("TradeWindow", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            portfolioCtl.portfolioCB.SelectionChanged += PortfolioCB_SelectionChanged;

        }
        private async void ReloadDataCallback(object state)
        {
            await Dispatcher.Invoke(async () =>
             {
                 var portfolio = portfolioCtl.portfolioCB.SelectedValue?.ToString();
                 await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                 var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                 greeksControl.GreekListView.ItemsSource = null;
                 greeksControl.GreekListView.ItemsSource = riskVMlist;
             });
        }
        private async void PortfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var portfolio = portfolioCtl.portfolioCB.SelectedValue?.ToString();
            var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
            var hedgeVMCollection = _otcOptionHandler?.HedgeVMCollection;
            var basecontractsList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                    .Select(c => c.BaseContract).Distinct().ToList();
            var pricingContractList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                .SelectMany(c => c.PricingContractParams).Select(c => c.Contract).Distinct().ToList();
            var hedgeContractList = hedgeVMCollection.Where(c => c.Portfolio == portfolio)
                .SelectMany(c => c.HedgeContracts).Select(c => c.Contract).Distinct().ToList();
            var mixed1ContractList = basecontractsList.Union(pricingContractList).ToList();
            var mixedContractList = mixed1ContractList.Union(hedgeContractList).ToList();
            QuoteVMCollection.Clear();
            foreach (var contract in mixedContractList)
            {
                if (!String.IsNullOrEmpty(contract))
                {
                    var mktDataVM = await marketDataLV.MarketDataHandler.SubMarketDataAsync(contract);
                    if (mktDataVM != null)
                    {
                        QuoteVMCollection.Add(mktDataVM);
                    }
                }
            }
            marketDataLV.quoteListView.ItemsSource = QuoteVMCollection;
            var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
            greeksControl.GreekListView.ItemsSource = riskVMlist;
            domesticPositionsWindow.FilterByPortfolio(portfolio);
            otcPositionsWindow.FilterByPortfolio(portfolio);
            domesticTradeWindow.FilterByPortfolio(portfolio);
            otcTradeWindow.FilterByPortfolio(portfolio);

            _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);

        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void ReloadData()
        {
            otcTradeWindow.ReloadData();
            domesticPositionsWindow.ReloadData();
            otcPositionsWindow.ReloadData();
            domesticTradeWindow.ReloadData();
            domesticPositionsWindow.ShowCloseAll = false;
            otcPositionsWindow.ShowCloseAll = false;
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

        public string PersistanceId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
