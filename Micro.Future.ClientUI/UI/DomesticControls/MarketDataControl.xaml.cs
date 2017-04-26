using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows.Input;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class MarketDataControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private const string DEFAULT_ID = "D97F60E1-0433-4886-99E6-C4AD46A7D33A";
        private IList< ColumnObject> mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public BaseMarketDataHandler MarketDataHandler { get; set; }

        public List<ContractInfo> FutureOptionList
        {
            get;
        } = new List<ContractInfo>();

        protected readonly MarketContract _userContractDbCtx;
        public FilterSettingsWindow FilterSettingsWin { get; } =
            new FilterSettingsWindow() { CancelClosing = true };

        public string PersistanceId
        {
            get;
            set;
        }

        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();

        public MarketDataControl(string persisitentId, string filterId, BaseMarketDataHandler marketDataHandler)
        {
            InitializeComponent();
            MarketDataHandler = marketDataHandler;
            if (MarketDataHandler != null)
            Initialize();
            FilterSettingsWin.OnFiltering += _fiterSettingsWin_OnFiltering;
            PersistanceId = persisitentId;
            FilterSettingsWin.PersistanceId = persisitentId; 
            FilterSettingsWin.FilterId = filterId;
        }

        public MarketDataControl()
        {
            InitializeComponent();

            FilterSettingsWin.PersistanceId = PersistanceId;
            FilterSettingsWin.FilterId = DEFAULT_ID;
        }

        public void Initialize()
        {
            _viewSource.Source = QuoteVMCollection;
            quoteListView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(quoteListView);
            FilterSettingsWin.UserID = MarketDataHandler.MessageWrapper?.User.Id;
            FutureOptionList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE));
            FutureOptionList.AddRange(ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS));
            
            QuoteChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (QuoteChanged.CanChangeLiveFiltering)
            {
                QuoteChanged.LiveFilteringProperties.Add("Exchange");
                QuoteChanged.LiveFilteringProperties.Add("Contract");
                QuoteChanged.IsLiveFiltering = true;
            }


            contractTextBox.Provider = new SuggestionProvider((string c) => { return FutureOptionList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        }

        public ICollectionViewLiveShaping QuoteChanged { get; set; }

        public virtual async void LoadUserContracts()
        {
            var userId = MarketDataHandler.MessageWrapper?.User?.Id;
            if (userId == null)
                return;

            var contracts = ClientDbContext.GetUserContracts(userId, FilterSettingsWin.FilterId).Select(c=>new ContractKeyVM(string.Empty, c));
            if (contracts.Any())
            {
                try
                {
                    var list = await MarketDataHandler.SubMarketDataAsync(contracts);
                    foreach (var mktVM in list)
                    {
                        Dispatcher.Invoke(() => QuoteVMCollection.Add(mktVM));
                    }
                }
                catch
                { }
            }
        }


        private void _fiterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
        {
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;

            Filter(tabTitle, exchange, underlying, contract);
        }

        public static event Action<MarketDataVM> OnQuoteSelected;
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            string userId = MarketDataHandler.MessageWrapper?.User?.Id;
            string tabId = FilterSettingsWin.FilterId;
            foreach (var mktVM in SeletedQuoteVM)
            {
                QuoteVMCollection.Remove(mktVM);
                ClientDbContext.DeleteUserContracts(userId, tabId, mktVM.Contract);
            }

            //MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
            //    UnsubMarketData(SeletedQuoteVM);
        }

        public void ReloadData()
        {
            Initialize();
            LayoutAnchorable defaultTab =
                AnchorablePane.Children.FirstOrDefault(pane => ((MarketDataControl)pane.Content).FilterSettingsWin.FilterId == DEFAULT_ID);
            AnchorablePane.Children.Clear();
            if (defaultTab != null)
                AnchorablePane.Children.Add(defaultTab);
            // MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().ResubMarketData();
            var filtersettings = ClientDbContext.GetFilterSettings(MarketDataHandler.MessageWrapper.User.Id, PersistanceId);
            bool found = false;
            foreach (var fs in filtersettings)
            {
                var marketdatactrl = new MarketDataControl(PersistanceId, fs.Id, MarketDataHandler);
                AnchorablePane.AddContent(marketdatactrl).Title = fs.Title;
                marketdatactrl.LoadUserContracts();
                marketdatactrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
                if (fs.Id == DEFAULT_ID)
                    found = true;
            }
            if (found)
                AnchorablePane.Children.Remove(defaultTab);

            LoadUserContracts();

        }

        private IEnumerable<MarketDataVM> SeletedQuoteVM
        {
            get
            {
                var marketList = new List<MarketDataVM>();
                if (quoteListView.SelectedItems != null)
                    foreach (var mktVM in quoteListView.SelectedItems)
                    {
                        marketList.Add((MarketDataVM)mktVM);
                    }

                return marketList;
            }
        }

        public LayoutAnchorablePane AnchorablePane
        {
            get;
            set;
        }

        private async void AddQuote()
        {
            string quote = contractTextBox.SelectedItem == null ? contractTextBox.Filter : contractTextBox.SelectedItem.ToString();
            if (!FutureOptionList.Any((c) => string.Compare(c.Contract, quote, true) == 0))
            {
                MessageBox.Show("输入合约" + quote + "不存在");
                contractTextBox.Filter = string.Empty;
                return;
            }

            ClientDbContext.SaveMarketContract(MarketDataHandler.MessageWrapper.User.Id,
                quote, FilterSettingsWin.FilterId);

            var item = QuoteVMCollection.FirstOrDefault(c => c.Contract == quote);

            if (item != null)
            {
                quoteListView.SelectedItem = item;
            }
            else
            {
                var mktDataVM = await MarketDataHandler.SubMarketDataAsync(quote);
                if (mktDataVM != null)
                {
                    QuoteVMCollection.Add(mktDataVM);
                }
            }
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            AddQuote();
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

        private void MenuItem_Click_DeleteWindow(object sender, RoutedEventArgs e)
        {

            ClientDbContext.DeleteFilterSettings(FilterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }
        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            //exchangeList.AddRange((from p in (IEnumerable<QuoteViewModel>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_quoteSettingsWin.ExchangeCollection = exchangeList;
            FilterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            FilterSettingsWin.Show();
        }


        private void MenuItem_Click_ShowCustomizedContractTab(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var marketDataControl = new MarketDataControl(PersistanceId, Guid.NewGuid().ToString(), MarketDataHandler);
                AnchorablePane.AddContent(marketDataControl).Title = title;
                marketDataControl.FilterSettingsWin.FilterTabTitle = title;
                marketDataControl.FilterSettingsWin.Save();
            }
        }

        //DataType is for window style, tabIndex is for 
        public void Filter(string tabTitle, string exchange, string underlying, string contract)
        {
            if (quoteListView == null)
            {
                return;
            }

            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                MarketDataVM qvm = o as MarketDataVM;

                if (qvm == null)
                {
                    return true;
                }

                if (qvm.Exchange.ContainsAny(exchange) &&
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

        private void quoteListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, quoteListView.Items);
            }
        }

        private void AddQuote_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if(!string.IsNullOrEmpty(contractTextBox.Filter))
                {
                    AddQuote();
                }
            }
        }

        private void contractTextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddQuote();
        }

    }
}
