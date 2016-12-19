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

        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();

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
            _contractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);

            underlyingEX.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            exchange1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            exchange2.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            quoteListView1.ItemsSource = QuoteVMCollection1;
            quoteListView2.ItemsSource = QuoteVMCollection2;

            volModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("vm");
            
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
            if (underlyingContractCB.SelectedValue != null)
            {
                var uc = underlyingContractCB.SelectedValue.ToString();


                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

                var callList = (from o in optionList
                                where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();
                CallPutTDOptionVMCollection.Clear();
                var retList = handler.SubCallPutTDOptionData(strikeList, callList, putList);
                foreach (var vm in retList)
                {
                    CallPutTDOptionVMCollection.Add(vm);
                }
            }
        }
        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            if (underlyingContractCB1.SelectedValue != null)
            {
                var uc = underlyingContractCB1.SelectedValue.ToString();

                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

                var callList = (from o in optionList
                                where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();
                CallPutTDOptionVMCollection.Clear();
                var retList = handler.SubCallPutTDOptionData(strikeList, callList, putList);
                foreach (var vm in retList)
                {
                    CallPutTDOptionVMCollection.Add(vm);
                }
            }
        }
        private void underlyingTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void underlyingEX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = underlyingEX.SelectedValue.ToString();
            underlyingCB.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
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
        private void contract1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (contract1.SelectedItem != null)
            {
                var uexchange = exchange1.SelectedValue.ToString();
                var uc = contract1.SelectedItem.ToString();
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection1.Clear();
                var mktDataVM = handler.SubMarketData(uc);
                QuoteVMCollection1.Add(mktDataVM);
                var strategyhandler = _otcOptionHandler;
                foreach (var option in CallPutTDOptionVMCollection)
                {
                    var callpcp = option.CallStrategyVM?.PricingContractParams.FirstOrDefault();
                    if (callpcp != null && callpcp.Contract != uc)
                    {
                        callpcp.Contract = uc;
                        callpcp.Exchange = uexchange;
                        strategyhandler.UpdateStrategyPricingContracts(option.CallStrategyVM);
                    }

                    var putpcp = option.PutStrategyVM?.PricingContractParams.FirstOrDefault();
                    if (putpcp != null && putpcp.Contract != uc)
                    {
                        putpcp.Contract = uc;
                        putpcp.Exchange = uexchange;
                        strategyhandler.UpdateStrategyPricingContracts(option.PutStrategyVM);
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
        private void contract2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contract2.SelectedItem != null)
            {
                var uexchange = exchange1.SelectedValue.ToString();
                var uc = contract2.SelectedItem.ToString();
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection2.Clear();
                var mktDataVM = handler.SubMarketData(uc);
                if (mktDataVM == null)
                    return;

                QuoteVMCollection2.Add(mktDataVM);
                var strategyhandler = _otcOptionHandler;
                foreach (var option in CallPutTDOptionVMCollection)
                {
                    var callpcp = option.CallStrategyVM?.PricingContractParams.FirstOrDefault();
                    if (callpcp != null && callpcp.Contract != uc)
                    {
                        callpcp.Contract = uc;
                        callpcp.Exchange = uexchange;
                        strategyhandler.UpdateStrategyPricingContracts(option.CallStrategyVM);
                    }

                    var putpcp = option.PutStrategyVM?.PricingContractParams.FirstOrDefault();
                    if (putpcp != null && putpcp.Contract != uc)
                    {
                        putpcp.Contract = uc;
                        putpcp.Exchange = uexchange;
                        strategyhandler.UpdateStrategyPricingContracts(option.PutStrategyVM);
                    }
                }
            }

        }
        private void volModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = volModelCB.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                foreach (var option in CallPutTDOptionVMCollection)
                {
                    if (option.CallStrategyVM == null)
                        return;
                    option.CallStrategyVM.VolModel = modelParam.InstanceName;
                    _otcOptionHandler.UpdateStrategy(option.CallStrategyVM);
                    option.PutStrategyVM.VolModel = modelParam.InstanceName;
                    _otcOptionHandler.UpdateStrategy(option.PutStrategyVM);
                }
            }
        }

        
    }
}
