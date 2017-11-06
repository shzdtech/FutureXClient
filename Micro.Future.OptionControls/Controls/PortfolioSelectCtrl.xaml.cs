using Micro.Future.Business.Handler.Router;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Utility;
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
using WpfControls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class PortfolioSelectCtrl : UserControl
    {
        public LayoutContent LayoutContent { get; set; }
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private OTCETFTradingDeskHandler _otcETFTradingDeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();
        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;
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
        public ObservableCollection<PortfolioVM> PortfolioVMCollection
        {
            get;
            set;
        }
        ~PortfolioSelectCtrl()
        {
            AutoHedgeUpdate(false);
        }
        public PortfolioSelectCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>()?.PortfolioVMCollection;
            foreach (var vm in MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>()?.PortfolioVMCollection)
            {
                if (vm != null)
                    portfolioVMCollection.Add(vm);
            }
            foreach (var vm in MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>()?.PortfolioVMCollection)
            {
                if (vm != null)
                    portfolioVMCollection.Add(vm);
            }
            PortfolioVMCollection = portfolioVMCollection;
            portfolioCB.ItemsSource = PortfolioVMCollection;
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            options.Union(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_ETFOPTION));
            options.Union(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_STOCK));
            options.Union(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION));
            _contractList = options.ToList();
        }

        public string PersistanceIdcomboBox
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

        public string PortfolioIndex
        {
            get;
            set;
        }

        private void strategyListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, strategyListView.Items);
            }
        }

        private void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutoHedge_CheckBox.IsChecked.Value)
                AutoHedgeUpdate(false);

            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue.ToString();
                var hedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                SelectedContract = hedgeVM.Select(c => c.Contract).FirstOrDefault();
                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                var strategyVMCollection = _handler?.StrategyVMCollection;
                var strategyVM = strategyVMCollection.FirstOrDefault();
                if(strategyVM!=null)
                {
                    SelectedOptionContract = strategyVM.Contract;
                    var portfolioVMCollection = _handler?.PortfolioVMCollection;
                    var portfolioVM = portfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                    var strategySymbolList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                        .Select(c => new { StrategyName = c.StrategySym }).Distinct().ToList();
                    strategyListView.ItemsSource = strategySymbolList;
                    hedgeListView.ItemsSource = portfolioVM.HedgeContractParams.OrderByDescending(c => c.Contract);
                    //var portfolioDataContext = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection
                    //    .Where(c => c.Name == portfolio).Distinct();
                    DelayTxt.DataContext = portfolioVM;
                    Threshold.DataContext = portfolioVM;
                    HedgeVolumeIUD.DataContext = portfolioVM;
                    AutoHedge_CheckBox.DataContext = portfolioVM;
                    var basecontractsList = strategyVMCollection.Select(c => c.BaseContract).Distinct().ToList();
                    foreach (var sVM in strategyVMCollection)
                    {
                        var _pricingcontractList = sVM.PricingContractParams.Select(c => c.Contract).Distinct().ToList();
                    }
                    PortfolioIndex = portfolioCB.SelectedValue.ToString();
                    HedgeVolumeIUD.Value = 1;
                }
                //var mixcontractList = basecontractsList.Union(_pricingcontractList).ToList();
            }

        }

        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        private void HedgeVolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    lock (this)
                    {
                        int hedgeVolume = (int)e.NewValue;
                        portfolioVM.HedgeVolume = hedgeVolume;
                        portfolioVM.UpdatePortfolioAsync().WaitAsync();
                    }
                }
            }
        }

        private void DelayValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                updownctrl.IsEnabled = false;
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {

                    int delay = (int)e.NewValue;
                    portfolioVM.Delay = delay;
                    portfolioVM.UpdatePortfolioAsync().WaitAsync();
                }
                updownctrl.IsEnabled = true;
            }
        }
        private void ThresholdValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                updownctrl.IsEnabled = false;
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    double threshold = (double)e.NewValue;
                    portfolioVM.Threshold = threshold;
                    portfolioVM.UpdatePortfolioAsync().WaitAsync();
                }
                updownctrl.IsEnabled = true;
            }
        }

        private void HedgeContractUpdate(AutoCompleteTextBox hedgeContract)
        {
            if (hedgeContract != null && !string.IsNullOrEmpty(hedgeContract.Filter))
            {
                var hedgeVM = hedgeContract.DataContext as HedgeVM;
                if (hedgeVM != null)
                {
                    string quote = hedgeContract.SelectedItem == null ? hedgeContract.Filter : hedgeContract.SelectedItem.ToString();
                    var list = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE).Where(c => c.Exchange == hedgeVM.Exchange && c.ProductID == hedgeVM.Underlying);
                    if (!list.Any((c) => string.Compare(c.Contract, quote, true) == 0))
                    {
                        System.Windows.MessageBox.Show("输入合约" + quote + "不存在");
                        hedgeContract.Filter = hedgeVM.Contract;
                        return;
                    }
                    else
                    {
                        var portfolio = portfolioCB.SelectedValue?.ToString();
                        if (portfolio != null)
                        {
                            hedgeVM.Contract = quote;
                            if (portfolioCB.SelectedValue != null)
                            {
                                var selectedHedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                                SelectedContract = selectedHedgeVM.Select(c => c.Contract).FirstOrDefault();
                                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                                _handler.UpdateHedgeContracts(hedgeVM, portfolio);
                            }
                        }
                    }
                }
            }
        }
        private void HedgeContract_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var hedgeContract = sender as AutoCompleteTextBox;
                HedgeContractUpdate(hedgeContract);
            }
        }

        //private void HedgeContract_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var hedgeContract = sender as AutoCompleteTextBox;
        //        HedgeContractUpdate(hedgeContract);
        //}

        //private void HedgeContractTextBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var hedgeContract = sender as AutoCompleteTextBox;
        //    if (hedgeContract != null)
        //    {
        //        var hedgeVM = hedgeContract.DataContext as HedgeVM;
        //        if (hedgeVM != null)
        //        {
        //            var list = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE).Where(c => c.Exchange == hedgeVM.Exchange && c.ProductID == hedgeVM.Underlying);
        //            hedgeContract.Provider = new SuggestionProvider((string c) => { return list.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        //            hedgeContract.SelectedItem = hedgeVM.Contract;
        //        }
        //    }
        //}
        private void HedgeContractComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var hedgeContract = sender as ComboBox;
            if (hedgeContract != null)
            {
                var hedgeVM = hedgeContract.DataContext as HedgeVM;
                if (hedgeVM != null)
                {
                    var list = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE).Where(c => c.Exchange == hedgeVM.Exchange && c.ProductID == hedgeVM.Underlying).Select(c => c.Contract);
                    hedgeContract.ItemsSource = list;
                    hedgeContract.SelectedItem = hedgeVM.Contract;
                }
            }
        }

        private void HedgeContractComboUpdate(ComboBox hedgeContract)
        {
            if (hedgeContract != null)
            {
                var hedgeVM = hedgeContract.DataContext as HedgeVM;
                if (hedgeVM != null)
                {
                    string quote = hedgeContract.SelectedItem.ToString();
                    var list = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE).Where(c => c.Exchange == hedgeVM.Exchange && c.ProductID == hedgeVM.Underlying);
                    if (!list.Any((c) => string.Compare(c.Contract, quote, true) == 0))
                    {
                        System.Windows.MessageBox.Show("输入合约" + quote + "不存在");
                        hedgeContract.SelectedItem = hedgeVM.Contract;
                        return;
                    }
                    else
                    {
                        var portfolio = portfolioCB.SelectedValue?.ToString();
                        if (portfolio != null)
                        {
                            hedgeVM.Contract = quote;
                            if (portfolioCB.SelectedValue != null)
                            {
                                var selectedHedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
                                SelectedContract = selectedHedgeVM.Select(c => c.Contract).FirstOrDefault();
                                var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
                                _handler.UpdateHedgeContracts(hedgeVM, portfolio);
                            }
                        }
                    }
                }
            }
        }

        private void HedgeContract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var hedgeContract = sender as ComboBox;
            HedgeContractComboUpdate(hedgeContract);
        }
        public void AutoHedgeUpdate(bool autoStatus)
        {
            var selectedHedgeVM = PortfolioVMCollection.Where(c => c.Name == portfolioCB.SelectedValue.ToString()).Select(c => c.HedgeContractParams).FirstOrDefault();
            SelectedContract = selectedHedgeVM.Select(c => c.Contract).FirstOrDefault();
            var _handler = TradingDeskHandlerRouter.DefaultInstance.GetMessageHandlerByContract(SelectedContract);
            if (_handler != null)
            {
                var portfolioVM = _handler.PortfolioVMCollection.FirstOrDefault(c => c.Name == PortfolioIndex);
                if (portfolioVM != null)
                {
                    lock (this)
                    {
                        portfolioVM.Hedging = autoStatus;
                        portfolioVM.UpdatePortfolioAsync().WaitAsync();
                    }
                }
            }
        }

        private void AutoHedge_Checked(object sender, RoutedEventArgs e)
        {
            hedgeListView.IsEnabled = false;
        }

        private void AutoHege_Unchecked(object sender, RoutedEventArgs e)
        {
            hedgeListView.IsEnabled = true;

        }
    }
}
