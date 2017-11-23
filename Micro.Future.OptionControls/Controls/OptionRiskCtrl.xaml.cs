using Micro.Future.Business.Handler.Router;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionRiskCtrl : UserControl, ILayoutAnchorableControl, IReloadData

    {
        public class StrategyBaseVM
        {
            public string OptionContract
            {
                get;
                set;
            }
            public string Contract
            {
                get;
                set;
            }
            public string Expiration
            {
                get;
                set;
            }
            public string FutureExpiration
            {
                get;
                set;
            }
            public bool RiskGraphEnable
            {
                get;
                set;
            }
            public double Valuation
            {
                get;
                set;
            }
            public bool Selected
            {
                get;
                set;
            }
            public double LastPrice
            {
                get;
                set;
            }
            public double SettlePrice
            {
                get;
                set;
            }
            public MarketDataVM MktVM
            {
                get;
                set;
            }

        }
        private IList<ContractInfo> _futurecontractList;

        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private TraderExHandler _ctpoptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private CTPETFTraderHandler _ctpetfTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();
        private CTPSTOCKTraderHandler _ctpstockTradeHandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();
        private OTCOptionTradeHandler _otcoptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCETFTradeHandler _otcetfTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradeHandler>();
        private OTCStockTradeHandler _otcstockTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradeHandler>();


        private Timer _timer;
        private const int UpdateInterval = 1000;
        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public string SelectedContract
        {
            get;
            set;
        }
        public string SelectedOptionContract
        {
            get;
            set;
        }
        public OptionRiskCtrl()
        {
            InitializeComponent();
            portfolioLayout.CanClose = false;
            portfolioLayout.CanHide = false;
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            //var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            var otcmarketdataHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
            var domesticTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            var otcTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            var compositeTradeHandler = CompositeTradeExHandler.DefaultInstance;
            domesticPositionsWindow.TradeHandler = compositeTradeHandler;
            otcPositionsWindow.TradeHandler = otcTradeHandler;
            otcPositionsWindow.MarketDataHandler = otcmarketdataHandler;
            domesticTradeWindow.TradeHandler = compositeTradeHandler;
            otcTradeWindow.TradeHandler = otcTradeHandler;
            //marketDataLV.MarketDataHandler = marketdataHandler;
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
            domesticTradeHandler.MessageWrapper.SignInManager.OnLogged += SignInManager_OnLogged;
            otcTradeHandler.MessageWrapper.SignInManager.OnLogged += OTCSignInManager_OnLogged;
        }

        private void OTCSignInManager_OnLogged(IUserInfo obj)
        {
            otcPositionsWindow.ReloadData();
        }

        private void SignInManager_OnLogged(IUserInfo obj)
        {
            domesticPositionsWindow.ReloadData();
        }

        private void ReloadDataCallback(object state)
        {
            Dispatcher.Invoke(async () =>
             {
                 var portfolio = portfolioCtl.portfolioCB.SelectedValue?.ToString();
                 //await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                 if (SelectedContract != null)
                 {
                     var _handler = OTCTradeHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                     //if (_handler != null)
                     //{
                     //    var riskVMlist = await _handler.QueryRiskAsync(portfolio);
                     //    greeksControl.BindingToSource(riskVMlist);
                     //}
                     if (_otcoptionTradeHandler.MessageWrapper.HasSignIn)
                     {
                         var riskVMlist = await _otcoptionTradeHandler.QueryRiskAsync(portfolio);
                         greeksControl.BindingToSource(riskVMlist);
                     }
                     else if (_otcetfTradeHandler.MessageWrapper.HasSignIn)
                     {
                         var riskVMlist = await _otcetfTradeHandler.QueryRiskAsync(portfolio);
                         greeksControl.BindingToSource(riskVMlist);
                     }
                     else if (_otcstockTradeHandler.MessageWrapper.HasSignIn)
                     {
                         var riskVMlist = await _otcstockTradeHandler.QueryRiskAsync(portfolio);
                         greeksControl.BindingToSource(riskVMlist);
                     }
                 }
             });
        }
        private async void PortfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedContract = portfolioCtl.SelectedContract;
            SelectedOptionContract = portfolioCtl.SelectedOptionContract;
            if (SelectedContract != null)
            {
                var portfolio = portfolioCtl.portfolioCB.SelectedValue?.ToString();
                if (portfolio != null)
                {
                    var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                    var strategyVMCollection = _handler?.StrategyVMCollection;
                    if (strategyVMCollection != null)
                    {
                        var portfolioVM = _handler?.PortfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                        //var basecontractsList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                        //        .Select(c => c.BaseContract).Distinct().ToList();
                        //var pricingContractList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                        //    .SelectMany(c => c.PricingContractParams).Select(c => c.Contract).Distinct().ToList();
                        //var hedgeContractList = portfolioVM.HedgeContractParams
                        //    .Select(c => c.Contract).Distinct().ToList();
                        //var mixed1ContractList = basecontractsList.Union(pricingContractList).ToList();
                        //var mixedContractList = mixed1ContractList.Union(hedgeContractList).ToList();
                        var strategyContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract))
                            .GroupBy(s => s.BaseContract).Select(c => new StrategyBaseVM { Contract = c.First().BaseContract, OptionContract = c.First().Contract }).ToList();
                        var strategyPricingContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio)
                                            .SelectMany(c => c.PricingContractParams).Select(c => c.Contract).Distinct().ToList();
                        List<string> strategyUnderlyingList = new List<string>();
                        List<string> strategyUnderlyingContractList = new List<string>();
                        if (strategyPricingContractList.Count != 0)
                        {
                            foreach (var contract in strategyPricingContractList)
                            {
                                strategyUnderlyingList.AddRange(_futurecontractList.Where(c => c.Contract == contract).Select(c => c.ProductID));
                            }
                        }
                        var UnderlyingList = strategyUnderlyingList.Distinct();
                        if (UnderlyingList != null)
                        {
                            foreach (var underlying in UnderlyingList)
                            {
                                strategyUnderlyingContractList.AddRange(_futurecontractList.Where(c => c.ProductID == underlying).Select(c => c.Contract));
                            }
                        }
                        if (strategyContractList.Count != 0)
                        {
                            var contractList = strategyContractList.Union(strategyUnderlyingContractList.Select(c => new StrategyBaseVM { Contract = c }));
                            contractList = contractList.GroupBy(c => c.Contract).Select(c => c.FirstOrDefault()).ToList();
                            var selectedContract = contractList.Select(c => c.Contract).First();
                            marketDataLV.MarketDataHandler = MarketDataHandlerRouter.DefaultInstance.GetMessageHandlerByContract(selectedContract);
                            QuoteVMCollection.Clear();
                            foreach (var vm in contractList)
                            {
                                if (!string.IsNullOrEmpty(vm.Contract))
                                {
                                    var mktDataVM = await marketDataLV.MarketDataHandler.SubMarketDataAsync(vm.Contract);
                                    if (mktDataVM != null)
                                    {
                                        QuoteVMCollection.Add(mktDataVM);
                                    }
                                }
                            }
                        }
                        marketDataLV.quoteListView.ItemsSource = QuoteVMCollection;
                    }
                    //var _otcoptiontradehandler = OTCTradeHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedOptionContract);
                    //if (_otcoptiontradehandler != null)
                    //{
                    //    var riskVMlist = await _otcoptiontradehandler.QueryRiskAsync(portfolio);
                    //    greeksControl.BindingToSource(riskVMlist);
                    //}
                    if (_otcoptionTradeHandler.MessageWrapper.HasSignIn)
                    {
                        var riskVMlist = await _otcoptionTradeHandler.QueryRiskAsync(portfolio);
                        greeksControl.BindingToSource(riskVMlist);
                    }
                    else if (_otcetfTradeHandler.MessageWrapper.HasSignIn)
                    {
                        var riskVMlist = await _otcetfTradeHandler.QueryRiskAsync(portfolio);
                        greeksControl.BindingToSource(riskVMlist);
                    }
                    else if (_otcstockTradeHandler.MessageWrapper.HasSignIn)
                    {
                        var riskVMlist = await _otcstockTradeHandler.QueryRiskAsync(portfolio);
                        greeksControl.BindingToSource(riskVMlist);
                    }
                    domesticPositionsWindow.FilterByPortfolio(portfolio);
                    otcPositionsWindow.FilterByPortfolio(portfolio);
                    domesticTradeWindow.FilterByPortfolio(portfolio);
                    otcTradeWindow.FilterByPortfolio(portfolio);

                    _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
                }
                else
                    ClearSource();
            }
            else
                ClearSource();
        }
        public void ClearSource()
        {
            marketDataLV.quoteListView.ItemsSource = null;
            greeksControl.RiskVMCollection.Clear();
            domesticPositionsWindow.FilterByPortfolio(portfolioCtl.portfolioCB.SelectedValue?.ToString());
            otcPositionsWindow.FilterByPortfolio(portfolioCtl.portfolioCB.SelectedValue?.ToString());
            domesticTradeWindow.FilterByPortfolio(portfolioCtl.portfolioCB.SelectedValue?.ToString());
            otcTradeWindow.FilterByPortfolio(portfolioCtl.portfolioCB.SelectedValue?.ToString());
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
            var _handler = OTCTradeHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);

            var layoutInfo = ClientDbContext.GetLayout(_handler.MessageWrapper.User.Id, optionRiskCtrlDM.Uid);
            if (layoutInfo != null)
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionRiskCtrlDM);

                using (var reader = new StringReader(layoutInfo.LayoutCFG))
                {
                    layoutSerializer.Deserialize(reader);
                }
            }

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
        public void SaveLayout()
        {

            var layoutInfo = ClientDbContext.GetLayout(_otcOptionTradeHandler.MessageWrapper.User?.Id, optionRiskCtrlDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionRiskCtrlDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            if (_otcOptionTradeHandler.MessageWrapper.User != null)
                ClientDbContext.SaveLayoutInfo(_otcOptionTradeHandler.MessageWrapper.User.Id, optionRiskCtrlDM.Uid, strBuilder.ToString());
        }

        public void OnClosing()
        {
            SaveLayout();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }
    }
}
