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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OpMarketData : UserControl
    {
        private OTCOptionHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();
        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;

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
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();

            underlyingEX.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            exchange1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            exchange2.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            quoteListView1.ItemsSource = QuoteVMCollection1;
            quoteListView2.ItemsSource = QuoteVMCollection2;

            volModelCB1.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("vm");

        }
        private void underlyingEX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            underlyingCB.ItemsSource = null;
            underlyingContractCB.ItemsSource = null;
            expireDateCB.ItemsSource = null;
            var exchange = underlyingEX.SelectedValue.ToString();
            underlyingCB.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
        }
        private void underlyingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
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

                SubbedContracts = await _otcOptionHandler.SubTradingDeskDataAsync(optionList);
            }
        }
        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            underlyingCB1.ItemsSource = null;
            underlyingContractCB1.ItemsSource = null;
            expireDateCB1.ItemsSource = null;
            var exchange = underlyingEX1.SelectedValue.ToString();
            underlyingCB1.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
        }
        private void underlyingCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB1.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
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

                SubbedContracts2 = await _otcOptionHandler.SubTradingDeskDataAsync(optionList);
            }
        }

        private void exchange1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchange1.SelectedValue.ToString();
            underlying1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            contract1.ItemsSource = null;
        }
        private void underlying1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlying1.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _futurecontractList
                                           where c.ProductID == productId.ToString()
                                           select c.Contract).Distinct().ToList();

                contract1.ItemsSource = underlyingContracts;

            }
        }
        private async void contract1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contract1.SelectedItem != null)
            {
                var uexchange = exchange1.SelectedValue.ToString();
                var uc = contract1.SelectedItem.ToString();
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection1.Clear();

                var mktDataVM = await handler.SubMarketDataAsync(uc);
                if (mktDataVM != null)
                {
                    QuoteVMCollection1.Add(mktDataVM);
                    var contract = SubbedContracts?.FirstOrDefault();
                    if (contract != null)
                    {
                        var strategy =
                            _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                        var pricingContract = strategy.IVMContractParams.FirstOrDefault();
                        if (pricingContract != null)
                        {
                            pricingContract.Exchange = uexchange;
                            pricingContract.Contract = uc;
                        }
                        _otcOptionHandler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.IVM);
                    }
                }
            }
        }
        private void exchange2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchange2.SelectedValue.ToString();
            underlying2.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            contract2.ItemsSource = null;

        }
        private void underlying2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlying2.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _futurecontractList
                                           where c.ProductID == productId.ToString()
                                           select c.Contract).Distinct().ToList();

                contract2.ItemsSource = underlyingContracts;

            }
        }
        private async void contract2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contract2.SelectedItem != null)
            {
                var uexchange = exchange1.SelectedValue.ToString();
                var uc = contract2.SelectedItem.ToString();
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection2.Clear();
                var mktDataVM = await handler.SubMarketDataAsync(uc);
                if (mktDataVM != null)
                {
                    QuoteVMCollection2.Add(mktDataVM);
                    var contract = SubbedContracts2?.FirstOrDefault();
                    if (contract != null)
                    {
                        var strategy =
                            _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                        var pricingContract = strategy.VMContractParams.FirstOrDefault();
                        if (pricingContract != null)
                        {
                            pricingContract.Exchange = uexchange;
                            pricingContract.Contract = uc;
                        }
                        _otcOptionHandler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.VM);
                    }
                }
            }
        }
        private void volModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = volModelCB.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                var contract = SubbedContracts?.FirstOrDefault();
                if (contract != null)
                {
                    var strategy =
                        _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    strategy.IVModel = modelParam.InstanceName;
                    _otcOptionHandler.UpdateStrategyModel(strategy, StrategyVM.Model.IVM);
                }
            }
        }

        private void volModelCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = volModelCB1.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                var contract = SubbedContracts2?.FirstOrDefault();
                if (contract != null)
                {
                    var strategy =
                        _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    strategy.VolModel = modelParam.InstanceName;
                    _otcOptionHandler.UpdateStrategyModel(strategy, StrategyVM.Model.VM);
                }
            }
        }

        private void Adjustment1_KeyDown(object sender, KeyEventArgs e)
        {
            DoubleUpDown ctrl = sender as DoubleUpDown;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {
                    if (_otcOptionHandler != null)
                    {
                        if (e.Key == Key.Enter)
                        {
                            var contract = SubbedContracts?.FirstOrDefault();
                            if (contract != null)
                            {
                                var strategy =
                                    _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                                var pricingContract = strategy.IVMContractParams.FirstOrDefault();
                                if (pricingContract != null)
                                {
                                    pricingContract.Adjust = ctrl.Value.Value;
                                }
                                _otcOptionHandler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.IVM);
                            }
                        }
                    }
                }
            }
        }

        private void Adjustment2_KeyDown(object sender, KeyEventArgs e)
        {
            DoubleUpDown ctrl = sender as DoubleUpDown;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {
                    if (_otcOptionHandler != null)
                    {
                        if (e.Key == Key.Enter)
                        {
                            var contract = SubbedContracts?.FirstOrDefault();
                            if (contract != null)
                            {
                                var strategy =
                                    _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                                var pricingContract = strategy.VMContractParams.FirstOrDefault();
                                if (pricingContract != null)
                                {
                                    pricingContract.Adjust = ctrl.Value.Value;
                                }
                                _otcOptionHandler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.VM);
                            }
                        }
                    }
                }
            }
        }
    }
}
