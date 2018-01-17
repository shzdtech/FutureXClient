using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Micro.Future.Windows;
using System.Linq;
using System.Collections.Generic;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.Utility;
using Micro.Future.CustomizedControls;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.Resources.Localization;
using Micro.Future.LocalStorage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Specialized;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class PositionProfitControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private const string POSITION_DEFAULT_ID = "6210A109-5291-4CEF-866E-9CEC7EF3A603";
        private Timer _timer;
        private const int UpdateInterval = 2000;
        private IList<ColumnObject> mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public FilterSettingsWindow FilterSettingsWin { get; }
           = new FilterSettingsWindow() { PersistanceId = typeof(PositionProfitControl).Name, CancelClosing = true };

        public BaseTraderHandler TradeHandler { get; set; }
        //public BaseMarketDataHandler MarketDataHandler { get; set; }
        //private static ISet<MarketDataVM> _marketDataList = new HashSet<MarketDataVM>();
        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }
        private string _defaultId;
        public string DEFAULT_ID
        {
            get
            {
                return _defaultId;
            }
            set
            {
                _defaultId = value;
                FilterSettingsWin.FilterId = value;
            }
        }
        public PositionProfitControl(string persisitentId, string filterId, BaseTraderHandler tradeHander)
        {
            InitializeComponent();
            DEFAULT_ID = POSITION_DEFAULT_ID;
            TradeHandler = tradeHander;
            //MarketDataHandler = marketHandler;
            if (TradeHandler != null)
                Initialize();
            PersistanceId = persisitentId;
            FilterSettingsWin.PersistanceId = persisitentId;
            FilterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
            FilterSettingsWin.FilterId = filterId;

        }

        //private void PositionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        PositionVM position = e.NewItems[0] as PositionVM;
        //        LoadMarketData(position.Contract);
        //    }
        //}

        //public void OnNewMarketData(MarketDataVM mktDataVM)
        //{
        //    if (TradeHandler.PositionContractSet.Contains(mktDataVM.Contract))
        //    {
        //        Task.Run(() =>
        //        {
        //            lock (TradeHandler.PositionVMCollection)
        //            {
        //                var positions = TradeHandler.PositionVMCollection.FindByContract(mktDataVM.Contract);
        //                foreach (var positionVM in positions)
        //                {
        //                    if (positionVM.Direction == PositionDirectionType.PD_LONG)
        //                    {
        //                        positionVM.Profit = (mktDataVM.LastPrice - positionVM.MeanCost) * positionVM.Position * positionVM.Multiplier;
        //                    }
        //                    else if (positionVM.Direction == PositionDirectionType.PD_SHORT)
        //                    {
        //                        positionVM.Profit = (positionVM.MeanCost - mktDataVM.LastPrice) * positionVM.Position * positionVM.Multiplier;
        //                    }
        //                }
        //            }
        //        });
        //    }
        //}

        public PositionProfitControl()
        {
            InitializeComponent();
            DEFAULT_ID = POSITION_DEFAULT_ID;
            FilterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
            FilterSettingsWin.PersistanceId = PersistanceId;
            FilterSettingsWin.FilterId = DEFAULT_ID;
        }
        public ICollectionViewLiveShaping PositionChanged { get; set; }
        private void _filterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            //if (LayoutContent != null)
            //    LayoutContent.Title = _filterSettingsWin.FilterTabTitle;
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;
            Filter(tabTitle, exchange, underlying, contract, portfolio);
        }

        public event Action<PositionVM> OnPositionSelected;

        public void ReloadData()
        {
            Initialize();

            LayoutAnchorable defaultTab =
                AnchorablePane.Children.FirstOrDefault(pane => ((PositionProfitControl)pane.Content).FilterSettingsWin.FilterId == DEFAULT_ID);

            AnchorablePane.Children.Clear();
            if (defaultTab != null)
                AnchorablePane.Children.Add(defaultTab);

            var accountHandler = MessageHandlerContainer.DefaultInstance.Get<AccountHandler>();
            var filtersettings = ClientDbContext.GetFilterSettings(accountHandler.MessageWrapper.User.Id, PersistanceId);
            //var filtersettings = ClientDbContext.GetFilterSettings(TradeHandler.MessageWrapper.User?.Id, PersistanceId);

            bool found = false;

            foreach (var fs in filtersettings)
            {
                var positionctrl = new PositionProfitControl(PersistanceId, fs.Id, TradeHandler);
                AnchorablePane.AddContent(positionctrl).Title = fs.Title;
                positionctrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract, fs.Portfolio);

                if (fs.Id == DEFAULT_ID)
                    found = true;
            }

            if (found)
                AnchorablePane.Children.Remove(defaultTab);

            //_timer = new Timer(UpdatePositionCallback, null, UpdateInterval, UpdateInterval);
        }

        private void UpdatePositionCallback(object state)
        {
            TradeHandler.QueryPositionProfit();
        }

        //private async void LoadMarketData(string contract)
        //{

        //    if (MarketDataHandler.MessageWrapper != null && MarketDataHandler.MessageWrapper.HasSignIn)
        //    {
        //        _marketDataList.Add(await MarketDataHandler.SubMarketDataAsync(contract));
        //        //await Task.Run(async () =>
        //        // {
        //        //     _marketDataList.Add(await MarketDataHandler.SubMarketDataAsync(contract));
        //        // });
        //    }


        //}

        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.ShowDialog();
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            //exchangeList.AddRange((from p in (IEnumerable<PositionVM>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_positionSettingsWin.ExchangeCollection = exchangeList;
            FilterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            FilterSettingsWin.Show();
        }   
        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("PositionProfit", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var positionctrl = new PositionProfitControl(PersistanceId, Guid.NewGuid().ToString(), TradeHandler);
                AnchorablePane.AddContent(positionctrl).Title = title;
                positionctrl.FilterSettingsWin.FilterTabTitle = title;
                positionctrl.FilterSettingsWin.Save();
            }
        }

        private void MenuItem_Click_DeleteWindow(object sender, RoutedEventArgs e)
        {

            ClientDbContext.DeleteFilterSettings(FilterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }

        public void DeletePositionDB()
        {
            ClientDbContext.DeleteFilterSettings(FilterSettingsWin.FilterId);
        }


        private void PositionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
            OnPositionSelected?.Invoke(positionVM);
        }

        public void Filter(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            if (PositionListView == null)
            {
                return;
            }

            //this.AnchorablePane.SelectedContent.Title = tabTitle;
            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;
            FilterSettingsWin.FilterPortfolio = portfolio;


            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (pvm.Exchange.ContainsAny(exchange) &&
                    pvm.Contract.ContainsAny(underlying) &&
                    pvm.Contract.ContainsAny(contract) &&
                    pvm.Portfolio.ContainsAny(portfolio))
                {
                    return true;
                }

                return false;
            };
        }
        private void FilterByDirection(PositionDirectionType? direction)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (direction == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (direction == pvm.Direction)
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByPortfolio(string portfolio)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            if(view!=null)
            {
                view.Filter = delegate (object o)
                {
                    if (portfolio == null)
                        return true;

                    PositionVM pvm = o as PositionVM;

                    if (portfolio == pvm.Portfolio)
                    {
                        return true;
                    }

                    return false;
                };
            }
        }

        private void PositionListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, PositionListView.Items);
            }
        }

        public void Initialize()
        {
            _viewSource.Source = TradeHandler.PositionProfitVMCollection;
            PositionListView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(PositionListView);
            TradeHandler.PositionProfitVMCollection.Clear();
            TradeHandler.QueryPositionProfit();

            //TradeHandler.PositionVMCollection.CollectionChanged += PositionCollectionChanged;
            //MarketDataHandler.OnNewMarketData += OnNewMarketData;
            FilterSettingsWin.UserID = TradeHandler.MessageWrapper?.User?.Id;
        }
    }
}
