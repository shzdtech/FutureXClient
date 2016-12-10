using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.ComponentModel;
using Micro.Future.Windows;
using Micro.Future.Utility;
using Micro.Future.Resources.Localization;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using WpfControls;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class MarketDataControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWin = new FilterSettingsWindow();
        private IList<ContractInfo> _futurecontractList;
        private ISuggestionProvider provider;

        public MarketDataControl()
        {
            InitializeComponent();
            Initialize();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
            _filterSettingsWin.OnFiltering += _fiterSettingsWin_OnFiltering;
            quoteListView.ItemsSource = _viewSource.View;

            QuoteChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (QuoteChanged.CanChangeLiveFiltering)
            {
                QuoteChanged.LiveFilteringProperties.Add("Exchange");
                QuoteChanged.LiveFilteringProperties.Add("Contract");
                QuoteChanged.IsLiveFiltering = true;
            }

            mColumns = ColumnObject.GetColumns(quoteListView);

        }

        private void Initialize()
        {
            this._futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            //this.SuggestContract = _futurecontractList.Select(ci => ci.Contract).Distinct().ToList();

            //behaviors:AutoCompleteBehavior.AutoCompleteItemsSource="{Binding SuggestContract1}"
            //WPFTextBoxAutoComplete.AutoCompleteBehavior.SetAutoCompleteItemsSource(FastOrderContract, SuggestContract);
            //behaviors: AutoCompleteBehavior.AutoCompleteStringComparison = "InvariantCultureIgnoreCase"
            //WPFTextBoxAutoComplete.AutoCompleteBehavior.SetAutoCompleteStringComparison(FastOrderContract, StringComparison.InvariantCultureIgnoreCase);
            //回调函数
            //FastOrderContract.Provider = new SuggestionProvider(  (string c)=>{ return _futurecontractList.Select(ci => ci.Contract.StartsWith(c));}  );
            this.provider = new SuggestionProvider((string c) => { return _futurecontractList.Where(ci => ci.Contract.StartsWith(c)).Select(cn => cn.Contract); });
            contractTextBox.Provider = this.provider;
        }

        public ICollectionViewLiveShaping QuoteChanged { get; set; }

        private void _fiterSettingsWin_OnFiltering(string tabTitle, string exchange, string portfolio, string underlying, string contract)
        {
            Filter(tabTitle, exchange, portfolio, underlying, contract);
        }

        public event Action<MarketDataVM> OnQuoteSelected;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {   
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                UnsubMarketData(SeletedQuoteVM);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                ResubMarketData();
        }

        private IEnumerable<MarketDataVM> SeletedQuoteVM
        {
            get
            {
                var selectedItems = quoteListView.SelectedItems;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    yield return selectedItems[i] as MarketDataVM;
                }
            }
        }

        public LayoutAnchorablePane AnchorablePane
        {
            get;
            set;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (contractTextBox.Text == "")
            {
                this.contractTextBox.Background = new SolidColorBrush(Colors.Red);
                MessageBox.Show("输入合约不能为空");
                this.contractTextBox.Background = new SolidColorBrush(Colors.White);
                return;
            }

            using (var clientCtx = new ClientDbContext())
            {
                var query = from contractInfo in clientCtx.ContractInfo where contractInfo.Contract == contractTextBox.Text select contractInfo;
                if (query.Any()==false)
                {
                    this.contractTextBox.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("输入合约不存在");
                    contractTextBox.Text = "";
                    this.contractTextBox.Background = new SolidColorBrush(Colors.White);
                }
            }

            var quote = contractTextBox.Text;

            var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                       QuoteVMCollection.FirstOrDefault((obj) => string.Compare(obj.Contract, quote, true) == 0);


            if (item != null)
            {
                quoteListView.SelectedItem = item;
            }
            else
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);     
            }
        }

        private void quoteListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnQuoteSelected != null)
            {
                MarketDataVM quoteVM = quoteListView.SelectedItem as MarketDataVM;
                OnQuoteSelected(quoteVM);
            }
        }

        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            var exchangeList = new List<string> { string.Empty };
            //exchangeList.AddRange((from p in (IEnumerable<QuoteViewModel>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_quoteSettingsWin.ExchangeCollection = exchangeList;

            _filterSettingsWin.Show();
        }


        private void MenuItem_Click_ShowCustomizedContractTab(object sender, RoutedEventArgs e)
        {
            if(AnchorablePane!=null)
                AnchorablePane.AddContent(new MarketDataControl()).Title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        //DataType is for window style, tabIndex is for 
        public void Filter(string tabTitle, string exchange, string portfolio,string underlying, string contract)
        {
            for (int count = 0; count < this.AnchorablePane.ChildrenCount; count++)
            {
                if (this.AnchorablePane.Children[count].Title.Equals(tabTitle))
                { 
                    MessageBox.Show("已存在同名窗口,请重新输入.");
                    return;
                }
            }
            this.AnchorablePane.SelectedContent.Title = tabTitle;

            if (quoteListView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                MarketDataVM qvm = o as MarketDataVM;

                if (qvm.Exchange.ContainsAny(exchange) &&
                    qvm.Exchange.ContainsAny(portfolio) &&
                    qvm.Contract.ContainsAny(contract) &&
                    qvm.Contract.ContainsAny(underlying))
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByContract(string contract)
        {
            if (quoteListView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(quoteListView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                MarketDataVM qvm = o as MarketDataVM;

                if (qvm.Contract.Contains(contract))
                {
                    return true;
                }

                return false;
            };
        }
        
        
        
    }
}
