using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
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
using Micro.Future.Resources.Localization;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Micro.Future.CustomizedControls;
using Xceed.Wpf.Toolkit;
using Micro.Future.Business.Handler.Router;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OpMarketData : UserControl
    {
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;
        private IList<ContractInfo> _optioncontractList;
        private IList<ContractInfo> _etfcontractList;
        private IList<ContractInfo> _optionList;


        public OpMarketData()
        {
            InitializeComponent();
            Initialize();
        }

        public IEnumerable<ContractKeyVM> SubbedContracts
        {
            get;
            private set;
        }
        public string SelectedContract
        {
            get;
            private set;
        }
        public string SelectedVolContract
        {
            get;
            private set;
        }
        public IEnumerable<ContractKeyVM> SubbedContracts2
        {
            get;
            private set;
        }

        public ObservableCollection<MarketDataVM> QuoteVMCollection1
        {
            get;
        } = new ObservableCollection<MarketDataVM>();

        public ObservableCollection<MarketDataVM> QuoteVMCollection2
        {
            get;
        } = new ObservableCollection<MarketDataVM>();

        public void Initialize()
        {
            //_futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            //var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            //var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            //_contractList = options.Union(otcOptions).ToList();

            //underlyingEX.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            //underlyingEX1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            //exchange1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            //exchange2.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            quoteListView1.ItemsSource = QuoteVMCollection1;
            quoteListView2.ItemsSource = QuoteVMCollection2;
            volModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("ivm");
            volModelCB1.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("vm");
        }
        public void GetContractInfo()
        {
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            var etfOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_ETFOPTION);
            var otcETFOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_ETFOPTION);
            var stocks = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_STOCK);
            var otcStocks = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_STOCK);
            _optioncontractList = options.Union(otcOptions).ToList();
            _etfcontractList = etfOptions.Union(otcETFOptions).ToList();
            _contractList = _optioncontractList.Union(_etfcontractList).ToList();
            _contractList = _contractList.Union(_futurecontractList).ToList();
            _contractList = _contractList.Union(stocks).ToList();
            _contractList = _contractList.Union(otcStocks).ToList();
            _optionList = _optioncontractList.Union(_etfcontractList).ToList();
            underlyingEX.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            exchange1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            exchange2.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
        }
        private void underlyingEX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            underlyingCB.ItemsSource = null;
            underlyingContractCB.ItemsSource = null;
            expireDateCB.ItemsSource = null;
            var exchange = underlyingEX.SelectedValue.ToString();
            var productID = (from c in _optionList
                             where c.Exchange == exchange.ToString() && c.Contract!=c.ProductID
                             orderby c.ProductID ascending
                             select c.ProductID).Distinct().ToList();
            //underlyingCB.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            underlyingCB.ItemsSource = productID;
        }
        private void underlyingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
                                           orderby c.UnderlyingContract ascending
                                           select c.UnderlyingContract).Distinct().ToList();

                underlyingContractCB.ItemsSource = underlyingContracts;
            }
        }
        private void underlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var contract = underlyingContractCB.SelectedValue;
            if (contract != null)
            {
                var expireDate = (from c in _contractList
                                  where c.UnderlyingContract == contract.ToString() && c.Exchange == underlyingEX.SelectedValue.ToString()
                                  orderby c.ExpireDate ascending
                                  select c.ExpireDate).Distinct().ToList();

                expireDateCB.ItemsSource = expireDate;
            }


        }
        private async void expireDateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (expireDateCB.SelectedValue != null)
            {
                var ed = expireDateCB.SelectedValue.ToString();
                var uc = underlyingContractCB.SelectedValue.ToString();
                var exchange = underlyingEX.SelectedValue.ToString();

                var optionList = (from c in _contractList
                                  where c.Exchange == exchange && c.UnderlyingContract == uc && c.ExpireDate == ed
                                  select new ContractKeyVM(c.Exchange, c.Contract)).ToList();
                SelectedContract = (from c in _contractList
                                      where c.Exchange == exchange && c.UnderlyingContract == uc && c.ExpireDate == ed
                                      select c.Contract).FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                SubbedContracts = await _handler.SubTradingDeskDataAsync(optionList);
                exchange1.ItemsSource = null;
                underlying1.ItemsSource = null;
                contract1.ItemsSource = null;
                volModelCB.ItemsSource = null;
                adjustment1.Value = null;
                exchange1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
                volModelCB.ItemsSource = _handler.GetModelParamsVMCollection("ivm");
                var contract = SubbedContracts?.FirstOrDefault();
                if (contract != null)
                {
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    if (strategy != null)
                    {
                        var pricingContract = strategy.IVMContractParams?.FirstOrDefault();
                        if (pricingContract != null)
                        {
                            var futureexchange = pricingContract.Exchange;
                            var futurecontract = pricingContract.Contract;
                            var futureunderlying = _contractList.FirstOrDefault(c => c.Exchange == futureexchange && c.Contract == futurecontract)?.ProductID;
                            var volmodel = strategy.IVModel;
                            var adjust = pricingContract.Adjust;
                            volModelCB.SelectedValue = volmodel;
                            exchange1.SelectedValue = futureexchange;
                            var exchangeTemp = exchange1.SelectedValue?.ToString();
                            if (exchangeTemp != null)
                            {
                                var productID = (from c in _contractList
                                                 where c.Exchange == exchange.ToString()
                                                 orderby c.ProductID ascending
                                                 select c.ProductID).Distinct().ToList();
                                //underlying1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchangeTemp).Select(c => c.ProductID).Distinct();
                                underlying1.ItemsSource = productID;
                                contract1.ItemsSource = null;
                            }
                            underlying1.SelectedValue = futureunderlying;
                            var productIdTemp = underlying1.SelectedValue?.ToString();

                            if (productIdTemp != null)
                            {
                                var underlyingContracts = (from c in _contractList
                                                           where c.ProductID == productIdTemp.ToString()
                                                           select c.Contract).Distinct().ToList();
                                contract1.ItemsSource = underlyingContracts;

                            }
                            contract1.SelectedValue = futurecontract;
                            var contractInfo = ClientDbContext.FindContract(futurecontract);
                            adjustment1.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                            adjustment1.Value = adjust;
                            var modelVM = volModelCB.SelectedItem as ModelParamsVM;
                            if (modelVM != null)
                            {
                                riskFree_Interest.DataContext = null;
                                riskFree_Interest.DataContext = modelVM;
                            }
                        }
                    }
                }
            }
        }
        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            underlyingCB1.ItemsSource = null;
            underlyingContractCB1.ItemsSource = null;
            expireDateCB1.ItemsSource = null;
            var exchange = underlyingEX1.SelectedValue.ToString();
            var productID = (from c in _optionList
                             where c.Exchange == exchange.ToString() && c.Contract != c.ProductID
                             orderby c.ProductID ascending
                             select c.ProductID).Distinct().ToList();
            //underlyingCB1.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            underlyingCB1.ItemsSource = productID;
        }
        private void underlyingCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB1.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
                                           orderby c.UnderlyingContract ascending
                                           select c.UnderlyingContract).Distinct().ToList();

                underlyingContractCB1.ItemsSource = underlyingContracts;
            }
        }
        public void underlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var contract = underlyingContractCB1.SelectedValue;

            if (contract != null)
            {
                var expireDate = (from c in _contractList
                                  where c.UnderlyingContract == contract.ToString() && c.Exchange == underlyingEX1.SelectedValue.ToString()
                                  orderby c.ExpireDate ascending
                                  select c.ExpireDate).Distinct().ToList();

                expireDateCB1.ItemsSource = expireDate;
            }
        }
        private async void expireDateCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (expireDateCB1.SelectedValue != null)
            {
                var ed = expireDateCB1.SelectedValue.ToString();
                var uc = underlyingContractCB1.SelectedValue.ToString();
                var exchange = underlyingEX1.SelectedValue.ToString();

                var optionList = (from c in _contractList
                                  where c.Exchange == exchange && c.UnderlyingContract == uc && c.ExpireDate == ed
                                  select new ContractKeyVM(c.Exchange, c.Contract)).ToList();
                SelectedVolContract = (from c in _contractList
                                    where c.Exchange == exchange && c.UnderlyingContract == uc && c.ExpireDate == ed
                                    select c.Contract).FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedVolContract);
                SubbedContracts2 = await _handler.SubTradingDeskDataAsync(optionList);
                exchange2.ItemsSource = null;
                underlying2.ItemsSource = null;
                contract2.ItemsSource = null;
                volModelCB1.ItemsSource = null;
                adjustment2.Value = null;
                exchange2.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
                volModelCB1.ItemsSource = _handler.GetModelParamsVMCollection("vm");
                var contract = SubbedContracts2?.FirstOrDefault();
                if (contract != null)
                {
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    var pricingContract = strategy?.VMContractParams.FirstOrDefault();
                    if (pricingContract != null)
                    {
                        var futureexchange = pricingContract.Exchange;
                        var futurecontract = pricingContract.Contract;
                        var futureunderlying = _contractList.FirstOrDefault(c => c.Exchange == futureexchange && c.Contract == futurecontract)?.ProductID;
                        var volmodel = strategy.VolModel;
                        var adjust = pricingContract.Adjust;
                        volModelCB1.SelectedValue = volmodel;
                        exchange2.SelectedValue = futureexchange;
                        var exchangeTemp = exchange2.SelectedValue?.ToString();
                        if (exchangeTemp != null)
                        {
                            var productID = (from c in _contractList
                                             where c.Exchange == exchange.ToString()
                                             orderby c.ProductID ascending
                                             select c.ProductID).Distinct().ToList();
                            //underlying2.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchangeTemp).Select(c => c.ProductID).Distinct();
                            underlying2.ItemsSource = productID;
                            contract2.ItemsSource = null;
                        }
                        underlying2.SelectedValue = futureunderlying;
                        var productIdTemp = underlying2.SelectedValue?.ToString();

                        if (productIdTemp != null)
                        {
                            var underlyingContracts = (from c in _contractList
                                                       where c.ProductID == productIdTemp.ToString()
                                                       select c.Contract).Distinct().ToList();
                            contract2.ItemsSource = underlyingContracts;

                        }
                        contract2.SelectedValue = futurecontract;
                        var contractInfo = ClientDbContext.FindContract(futurecontract);
                        adjustment2.Increment = contractInfo == null ? 1 : contractInfo.PriceTick;
                        adjustment2.Value = adjust;
                    }
                }

            }
        }

        private void exchange1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchange1.SelectedItem?.ToString();
            if (exchange != null)
            {
                var productID = (from c in _contractList
                                 where c.Exchange == exchange.ToString()
                                 orderby c.ProductID ascending
                                 select c.ProductID).Distinct().ToList();
                //underlying1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
                underlying1.ItemsSource = productID;
                contract1.ItemsSource = null;
            }
        }
        private void underlying1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlying1.SelectedValue?.ToString();

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
                                           orderby c.Contract ascending
                                           select c.Contract).Distinct().ToList();

                contract1.ItemsSource = underlyingContracts;

            }
        }
        private async void contract1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contract1.SelectedItem != null)
            {
                var uexchange = exchange1.SelectedValue?.ToString();
                var uc = contract1.SelectedItem?.ToString();
                var handler = MarketDataHandlerRouter.DefaultInstance.GetMessageHandlerByContract(uc);
                QuoteVMCollection1.Clear();

                var mktDataVM = await handler.SubMarketDataAsync(uc);
                if (mktDataVM != null)
                {
                    QuoteVMCollection1.Add(mktDataVM);
                }

                var contract = SubbedContracts?.FirstOrDefault();
                if (contract != null)
                {
                    var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    var pricingContract = strategy.IVMContractParams.FirstOrDefault();
                    if (pricingContract != null && pricingContract.Exchange == uexchange && pricingContract.Contract == uc)
                        return;

                    if (pricingContract == null)
                    {
                        pricingContract = new PricingContractParamVM();
                        strategy.IVMContractParams.Add(pricingContract);
                    }
                    pricingContract.Exchange = uexchange;
                    pricingContract.Contract = uc;
                    _handler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.IVM);
                }
            }
        }
        private void exchange2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchange2.SelectedValue?.ToString();
            //var productID = (from c in _futurecontractList
            //                 where c.Exchange == exchange.ToString()
            //                 orderby c.ProductID ascending
            //                 select c.ProductID).Distinct().ToList();
            underlying2.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            //underlying2.ItemsSource = productID;
            contract2.ItemsSource = null;

        }
        private void underlying2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlying2.SelectedValue?.ToString();

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
                                           orderby c.Contract ascending
                                           select c.Contract).Distinct().ToList();

                contract2.ItemsSource = underlyingContracts;

            }
        }
        private async void contract2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contract2.SelectedItem != null)
            {
                var uexchange = exchange2.SelectedValue?.ToString();
                var uc = contract2.SelectedItem?.ToString();
                var handler = MarketDataHandlerRouter.DefaultInstance.GetMessageHandlerByContract(uc);
                QuoteVMCollection2.Clear();
                var mktDataVM = await handler.SubMarketDataAsync(uc);
                if (mktDataVM != null)
                {
                    QuoteVMCollection2.Add(mktDataVM);
                }
                var contract = SubbedContracts2?.FirstOrDefault();
                if (contract != null)
                {
                    var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedVolContract);
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    var pricingContract = strategy.VMContractParams.FirstOrDefault();
                    if (pricingContract != null && pricingContract.Exchange == uexchange && pricingContract.Contract == uc)
                        return;

                    if (pricingContract == null)
                    {
                        pricingContract = new PricingContractParamVM();
                        strategy.VMContractParams.Add(pricingContract);
                    }
                    pricingContract.Exchange = uexchange;
                    pricingContract.Contract = uc;
                    _handler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.VM);
                }
            }
        }
        private void volModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = volModelCB.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                riskFree_Interest.DataContext = modelParam;
                var contract = SubbedContracts?.FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                if (contract != null)
                {
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    if (strategy != null && strategy.IVModel != modelParam.InstanceName)
                    {
                        strategy.IVModel = modelParam.InstanceName;
                        _handler.UpdateStrategyModel(strategy, StrategyVM.Model.IVM);
                    }
                }
            }
        }

        private void volModelCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = volModelCB1.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                var contract = SubbedContracts2?.FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedVolContract);
                if (contract != null)
                {
                    var strategy =
                        _handler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    if (strategy != null && strategy.VolModel != modelParam.InstanceName)
                    {
                        strategy.VolModel = modelParam.InstanceName;
                        _handler.UpdateStrategyModel(strategy, StrategyVM.Model.VM);
                    }
                }
            }
        }

        private void Adjustment1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue != null && e.NewValue != null)
            {
                var contract = SubbedContracts?.FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                if (contract != null)
                {
                    UpdateStrategyAdjustment(contract, StrategyVM.Model.IVM, (double)e.NewValue, _handler);
                }
            }
        }

        private void Adjustment2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue != null && e.NewValue != null)
            {
                var contract = SubbedContracts2?.FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedVolContract);
                if (contract != null)
                {
                    UpdateStrategyAdjustment(contract, StrategyVM.Model.VM, (double)e.NewValue, _handler);
                }
            }
        }
        private void UpdateStrategyAdjustment(ContractKeyVM contract, StrategyVM.Model model, double adjust, BaseTradingDeskHandler handler)
        {
            var strategy = handler.StrategyVMCollection.FirstOrDefault(s => s.Equals(contract));
            if (strategy != null)
            {
                PricingContractParamVM pricingContract = null;
                switch (model)
                {
                    case StrategyVM.Model.PM:
                        pricingContract = strategy.PricingContractParams.FirstOrDefault();
                        break;
                    case StrategyVM.Model.IVM:
                        pricingContract = strategy.IVMContractParams.FirstOrDefault();
                        break;
                    case StrategyVM.Model.VM:
                        pricingContract = strategy.VMContractParams.FirstOrDefault();
                        break;
                }

                if (pricingContract != null)
                {
                    pricingContract.Adjust = adjust;
                    handler.UpdateStrategyPricingContracts(strategy, model);
                }
            }
        }
        private void riskFree_Interest_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var modelParamsVM = updownctrl.DataContext as ModelParamsVM;
                if (modelParamsVM != null)
                {
                    var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                    var key = updownctrl.Tag.ToString();
                    _handler.UpdateModelParams(modelParamsVM.InstanceName, key, updownctrl.Value.Value);
                }
            }
        }
        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        private void OnKeyDownForColor(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Enter || e.Key == Key.Down || e.Key == Key.Up)
                {
                    ctrl.Background = Brushes.White;
                }
                else
                {
                    ctrl.Background = Brushes.MistyRose;
                }
            }
        }

    }
}
