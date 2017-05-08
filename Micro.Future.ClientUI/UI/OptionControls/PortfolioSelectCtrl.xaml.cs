using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Utility;
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
        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;

        ~PortfolioSelectCtrl()
        {
            AutoHedgeUpdate(false);
        }
        public PortfolioSelectCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();

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
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var portfolioVMCollection = _otcOptionHandler?.PortfolioVMCollection;
                var portfolioVM = portfolioVMCollection.FirstOrDefault(c => c.Name == portfolio);
                var strategySymbolList = strategyVMCollection.Where(c => c.Portfolio == portfolio)
                    .Select(c => new { StrategyName = c.StrategySym }).Distinct().ToList();
                strategyListView.ItemsSource = strategySymbolList;
                hedgeListView.ItemsSource = portfolioVM.HedgeContractParams;
                //var portfolioDataContext = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection
                //    .Where(c => c.Name == portfolio).Distinct();
                DelayTxt.DataContext = portfolioVM;
                Threshold.DataContext = portfolioVM;
                AutoHedge_CheckBox.DataContext = portfolioVM;
                var basecontractsList = strategyVMCollection.Select(c => c.BaseContract).Distinct().ToList();
                foreach (var sVM in strategyVMCollection)
                {
                    var _pricingcontractList = sVM.PricingContractParams.Select(c => c.Contract).Distinct().ToList();
                }
                PortfolioIndex = portfolioCB.SelectedValue.ToString();
                //var mixcontractList = basecontractsList.Union(_pricingcontractList).ToList();
            }

        }

        private void Delay_KeyUp(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                var portfolioVM = DelayTxt.DataContext as PortfolioVM;
                if (e.Key == Key.Enter)
                {
                    //portfolioVM.Delay = DelayTxt.;
                    portfolioVM.UpdatePortfolio();
                }
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
        private void DelayValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    int delay = (int)e.NewValue;
                    portfolioVM.Delay = delay;
                    portfolioVM.UpdatePortfolio();
                }
            }
        }
        private void ThresholdValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var portfolioVM = updownctrl.DataContext as PortfolioVM;
                if (portfolioVM != null)
                {
                    double threshold = (double)e.NewValue;
                    portfolioVM.Threshold = threshold;
                    portfolioVM.UpdatePortfolio();
                }
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
                            _otcOptionHandler.UpdateHedgeContracts(hedgeVM, portfolio);
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
                            _otcOptionHandler.UpdateHedgeContracts(hedgeVM, portfolio);
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
            var portfolioVM = _otcOptionHandler.PortfolioVMCollection.FirstOrDefault(c => c.Name == PortfolioIndex);
            if (portfolioVM != null)
            {
                portfolioVM.Hedging = autoStatus;
                portfolioVM.UpdatePortfolio();
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
