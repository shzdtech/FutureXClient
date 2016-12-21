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
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public IList<ContractInfo> FuturecontractList
        {
            get
            {
                return ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            }
        }
        protected readonly MarketContract _userContractDbCtx;
        private FilterSettingsWindow _filterSettingsWin =
            new FilterSettingsWindow() { PersistanceId = typeof(MarketDataControl).Name, CancelClosing = true };

        public string PersistanceId
        {
            get;
            set;
        }

        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();

        public MarketDataControl(string filterId)
        {
            InitializeComponent();
            Initialize();
            _filterSettingsWin.FilterId = filterId;
        }

        public MarketDataControl() : this(Guid.NewGuid().ToString())
        {
        }

        private void Initialize()
        {
            _filterSettingsWin.OnFiltering += _fiterSettingsWin_OnFiltering;
            _viewSource.Source = QuoteVMCollection;
            QuoteChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (QuoteChanged.CanChangeLiveFiltering)
            {
                QuoteChanged.LiveFilteringProperties.Add("Exchange");
                QuoteChanged.LiveFilteringProperties.Add("Contract");
                QuoteChanged.IsLiveFiltering = true;
            }
            quoteListView.ItemsSource = _viewSource.View;

            mColumns = ColumnObject.GetColumns(quoteListView);

            contractTextBox.Provider = new SuggestionProvider((string c) => { return FuturecontractList.Where(ci => ci.Contract.StartsWith(c, true, null)).Select(cn => cn.Contract); });
        }

        public ICollectionViewLiveShaping QuoteChanged { get; set; }

        public virtual async void LoadUserContracts()
        {
            var userId = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().MessageWrapper?.User?.Id;
            if (userId == null)
                return;

            var contracts = ClientDbContext.GetUserContracts(userId, _filterSettingsWin.FilterId);
            if (contracts.Any())
            {
                var list = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketDataAsync(contracts);
                foreach (var mktVM in list)
                {
                    Dispatcher.Invoke(() => QuoteVMCollection.Add(mktVM));
                }
            }
        }


        private void _fiterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
        {
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;

            Filter(tabTitle, exchange, underlying, contract);
        }

        public static event Action<MarketDataVM> OnQuoteSelected;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            foreach (var mktVM in SeletedQuoteVM)
                QuoteVMCollection.Remove(mktVM);

            //MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
            //    UnsubMarketData(SeletedQuoteVM);
        }

        public void ReloadData()
        {
            while (AnchorablePane.ChildrenCount > 1)
                AnchorablePane.Children.RemoveAt(1);

            // MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().ResubMarketData();
            var filtersettings = ClientDbContext.GetFilterSettings(MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().MessageWrapper.User.Id, _filterSettingsWin.PersistanceId);
            foreach (var fs in filtersettings)
            {
                var marketdatactrl = new MarketDataControl(fs.Id);
                AnchorablePane.AddContent(marketdatactrl).Title = fs.Title;
                marketdatactrl.LoadUserContracts();
                marketdatactrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
            }
            if (filtersettings.Any())
                AnchorablePane.RemoveChildAt(0);
            else
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
            if (!FuturecontractList.Any(c => c.Contract == quote))
            {
                MessageBox.Show("输入合约" + quote + "不存在");
                return;
            }

            ClientDbContext.SaveMarketContract(MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().MessageWrapper.User.Id,
                quote, _filterSettingsWin.FilterId);

            var item = QuoteVMCollection.FirstOrDefault(c => c.Contract == quote);

            if (item != null)
            {
                quoteListView.SelectedItem = item;
            }
            else
            {
                var mktDataVM = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketDataAsync(quote);
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

            ClientDbContext.DeleteFilterSettings(_filterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }
        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            //exchangeList.AddRange((from p in (IEnumerable<QuoteViewModel>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_quoteSettingsWin.ExchangeCollection = exchangeList;
            _filterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            _filterSettingsWin.Show();
        }


        private void MenuItem_Click_ShowCustomizedContractTab(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                AnchorablePane.AddContent(new MarketDataControl()).Title
                    = WPFUtility.GetLocalizedString("Optional", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            }
        }

        //DataType is for window style, tabIndex is for 
        public void Filter(string tabTitle, string exchange, string underlying, string contract)
        {
            if (quoteListView == null)
            {
                return;
            }

            _filterSettingsWin.FilterTabTitle = tabTitle;
            _filterSettingsWin.FilterExchange = exchange;
            _filterSettingsWin.FilterUnderlying = underlying;
            _filterSettingsWin.FilterContract = contract;

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
                AddQuote();
            }
        }

        private void contractTextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddQuote();
        }
    }
}
