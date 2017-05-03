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
    public partial class PositionControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private const string DEFAULT_ID = "6210A109-5291-4CEF-866E-9CEC7EF3A602";
        private Timer _timer;
        private const int UpdateInterval = 1000;
        private IList<ColumnObject> mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public FilterSettingsWindow FilterSettingsWin { get; }
           = new FilterSettingsWindow() { PersistanceId = typeof(PositionControl).Name, CancelClosing = true };

        public BaseTraderHandler TradeHandler { get; set; }
        public BaseMarketDataHandler MarketDataHandler { get; set; }

        private static ISet<MarketDataVM> _marketDataList = new HashSet<MarketDataVM>();


        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }
        public PositionControl(string persisitentId, string filterId, BaseTraderHandler tradeHander, BaseMarketDataHandler marketHandler)
        {
            InitializeComponent();
            TradeHandler = tradeHander;
            MarketDataHandler = marketHandler;
            if (TradeHandler != null && MarketDataHandler != null)
                Initialize();
            PersistanceId = persisitentId;
            FilterSettingsWin.PersistanceId = persisitentId;
            //MessageHandlerContainer.DefaultInstance
            //.Get<MarketDataHandler>().OnNewMarketData += OnNewMarketData;
            FilterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
            //PositionChanged = _viewSource.View as ICollectionViewLiveShaping;
            //if (PositionChanged.CanChangeLiveFiltering)
            //{
            //    PositionChanged.LiveFilteringProperties.Add("Direction");
            //    PositionChanged.IsLiveFiltering = true;
            //}
            FilterSettingsWin.FilterId = filterId;

        }

        private void PositionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                PositionVM position = e.NewItems[0] as PositionVM;
                LoadMarketData(position.Contract);
            }
        }

        public void OnNewMarketData(MarketDataVM mktDataVM)
        {
            if (TradeHandler.PositionContractSet.Contains(mktDataVM.Contract))
            {
                Task.Run(() =>
                {
                    lock (TradeHandler.PositionVMCollection)
                    {
                        var positions = TradeHandler.PositionVMCollection.FindByContract(mktDataVM.Contract);
                        foreach (var positionVM in positions)
                        {
                            if (positionVM.Direction == PositionDirectionType.PD_LONG)
                            {
                                positionVM.Profit = (mktDataVM.LastPrice - positionVM.MeanCost) * positionVM.Position * positionVM.Multiplier;
                            }
                            else if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                            {
                                positionVM.Profit = (positionVM.MeanCost - mktDataVM.LastPrice) * positionVM.Position * positionVM.Multiplier;
                            }
                        }
                    }
                });
            }
        }

        public PositionControl()
        {
            InitializeComponent();
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

        public static event Action<PositionVM> OnPositionSelected;

        private void ReloadDataCallback(object state)
        {
            TradeHandler.QueryPosition();
        }

        public void ReloadData()
        {
            Initialize();
            _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
            LayoutAnchorable defaultTab =
                AnchorablePane.Children.FirstOrDefault(pane => ((PositionControl)pane.Content).FilterSettingsWin.FilterId == DEFAULT_ID);

            AnchorablePane.Children.Clear();
            if (defaultTab != null)
                AnchorablePane.Children.Add(defaultTab);


            var filtersettings = ClientDbContext.GetFilterSettings(TradeHandler.MessageWrapper.User.Id, PersistanceId);

            bool found = false;

            foreach (var fs in filtersettings)
            {
                var positionctrl = new PositionControl(PersistanceId, fs.Id, TradeHandler, MarketDataHandler);
                AnchorablePane.AddContent(positionctrl).Title = fs.Title;
                positionctrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract, fs.Portfolio);

                if (fs.Id == DEFAULT_ID)
                    found = true;
            }

            if (found)
                AnchorablePane.Children.Remove(defaultTab);
        }

        private async void LoadMarketData(string contract)
        {

            if (MarketDataHandler != null)
                _marketDataList.Add(await MarketDataHandler.SubMarketDataAsync(contract));
        }

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
        private void MenuItem_Click_ClosePosition(object sender, RoutedEventArgs e)
        {
            PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
            //var orderVM = new OrderVM(MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>());
            var orderVM = new OrderVM(TradeHandler);
            if (positionVM != null)
            {
                if (positionVM.Position != 0)
                {
                    var mktdataVM = _marketDataList.FirstOrDefault(c => c.Contract == positionVM.Contract);
                    if (mktdataVM != null)
                    {
                        if (positionVM.Direction == PositionDirectionType.PD_LONG)
                        {
                            if (positionVM.Position != 0)
                            {
                                var cvt = new EnumToFriendlyNameConverter();
                                //string msg = string.Format("委托确认\n昨仓 合约：{0}，价格：{1}，方向：卖, 手数：{2}，开平：平仓\n今仓 合约：{3}，价格：{4}，方向：卖, 手数：{5}，开平：平今",
                                //    positionVM.Contract, mktdataVM.BidPrice, positionVM.YdPosition, positionVM.Contract, mktdataVM.BidPrice, positionVM.TodayPosition);
                                //MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                //if (dr == MessageBoxResult.OK)

                                if (positionVM.YdPosition != 0 & mktdataVM.BidSize != 0 & positionVM.TodayPosition == 0)
                                {
                                    string msg = string.Format("{0}: {1} 卖 {2}手 平仓",
                                        positionVM.Contract, mktdataVM.BidPrice, positionVM.YdPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSEYESTERDAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.YdPosition;
                                        orderVM.Direction = DirectionType.SELL;
                                        orderVM.LimitPrice = mktdataVM.BidPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                    }
                                }
                                if (positionVM.TodayPosition != 0 & mktdataVM.BidSize != 0 & positionVM.YdPosition == 0)
                                {
                                    string msg = string.Format("{0}: {1} 卖 {2}手 平仓",
                                           positionVM.Contract, mktdataVM.BidPrice, positionVM.TodayPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSETODAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.TodayPosition;
                                        orderVM.Direction = DirectionType.SELL;
                                        orderVM.LimitPrice = mktdataVM.BidPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                    }
                                }
                                if (positionVM.TodayPosition != 0 & mktdataVM.BidSize != 0 & positionVM.YdPosition != 0)
                                {
                                    string msg = string.Format("{0}: {1} 卖 {2}手 平仓\n{3}: {4} 卖 {5}手 平今",
                                           positionVM.Contract, mktdataVM.BidPrice, positionVM.YdPosition, positionVM.Contract, mktdataVM.BidPrice, positionVM.TodayPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSEYESTERDAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.YdPosition;
                                        orderVM.Direction = DirectionType.SELL;
                                        orderVM.LimitPrice = mktdataVM.BidPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSETODAY;
                                        orderVM.Volume = positionVM.TodayPosition;
                                        orderVM.SendOrder();
                                    }
                                }
                            }
                        }
                        if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                        {
                            if (positionVM.Position != 0)
                            {
                                var cvt = new EnumToFriendlyNameConverter();
                                //string msg = string.Format("委托确认\n昨仓 合约：{0}，价格：{1}，方向：买, 手数：{2}，开平：平仓\n今仓 合约：{3}，价格：{4}，方向：买, 手数：{5}，开平：平今",
                                //    positionVM.Contract, mktdataVM.AskPrice, positionVM.YdPosition, positionVM.Contract, mktdataVM.AskPrice, positionVM.TodayPosition);
                                //MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                //if (dr == MessageBoxResult.OK)

                                if (positionVM.YdPosition != 0 & mktdataVM.AskSize != 0 & positionVM.TodayPosition == 0)
                                {
                                    string msg = string.Format("{0}: {1} 买 {2}手 平仓",
                                            positionVM.Contract, mktdataVM.AskPrice, positionVM.YdPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSEYESTERDAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.YdPosition;
                                        orderVM.Direction = DirectionType.BUY;
                                        orderVM.LimitPrice = mktdataVM.AskPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                    }
                                }
                                if (positionVM.TodayPosition != 0 & mktdataVM.AskSize != 0 & positionVM.YdPosition == 0)
                                {
                                    string msg = string.Format("{0}: {1} 买 {2}手 平今",
                                            positionVM.Contract, mktdataVM.AskPrice, positionVM.TodayPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSETODAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.TodayPosition;
                                        orderVM.Direction = DirectionType.BUY;
                                        orderVM.LimitPrice = mktdataVM.AskPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                    }
                                }
                                if (positionVM.TodayPosition != 0 & mktdataVM.AskSize != 0 & positionVM.YdPosition != 0)
                                {
                                    string msg = string.Format("{0}: {1} 买 {2}手 平仓\n{3}: {4} 卖 {5}手 平今",
                                            positionVM.Contract, mktdataVM.AskPrice, positionVM.YdPosition, positionVM.Contract, mktdataVM.AskPrice, positionVM.TodayPosition);
                                    MessageBoxResult dr = System.Windows.MessageBox.Show(msg, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                    if (dr == MessageBoxResult.OK)
                                    {
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSEYESTERDAY;
                                        orderVM.Contract = positionVM.Contract;
                                        orderVM.Volume = positionVM.YdPosition;
                                        orderVM.Direction = DirectionType.BUY;
                                        orderVM.LimitPrice = mktdataVM.AskPrice;
                                        orderVM.TIF = OrderTIFType.GFD;
                                        orderVM.ExecType = OrderExecType.LIMIT;
                                        orderVM.SendOrder();
                                        orderVM.OpenClose = OrderOpenCloseType.CLOSETODAY;
                                        orderVM.Volume = positionVM.TodayPosition;
                                        orderVM.SendOrder();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var positionctrl = new PositionControl(PersistanceId, Guid.NewGuid().ToString(), TradeHandler, MarketDataHandler);
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
            _viewSource.Source = TradeHandler.PositionVMCollection;
            PositionListView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(PositionListView);
            TradeHandler.PositionVMCollection.Clear();
            TradeHandler.QueryPosition();

            TradeHandler.PositionVMCollection.CollectionChanged += PositionCollectionChanged;
            MarketDataHandler.OnNewMarketData += OnNewMarketData;
            FilterSettingsWin.UserID = TradeHandler.MessageWrapper?.User.Id;
        }

        public bool ShowCloseAll
        {
            set
            {
                if (!value)
                    ContextMenu?.Items.Remove(ClosePositionClick);
            }
        }
    }
}
