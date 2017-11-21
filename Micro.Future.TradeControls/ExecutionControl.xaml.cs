using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Windows;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.UI;
using Micro.Future.Utility;
using Micro.Future.CustomizedControls;
using Micro.Future.CustomizedControls.Controls;
using System;
using Micro.Future.Resources.Localization;
using Micro.Future.LocalStorage;
using System.Threading.Tasks;
using System.Threading;

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ExecutionControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private IList<ColumnObject> mColumns;
        private const string EXECUTION_DEFAULT_ID = "394B67D4-87AA-47DB-B1DD-5A213714D02A";

        public BaseTraderHandler TradeHandler { get; set; }
        public BaseMarketDataHandler MarketDataHandler { get; set; }

        private CollectionViewSource _viewSource = new CollectionViewSource();
        public FilterSettingsWindow FilterSettingsWin { get; } = new FilterSettingsWindow() { PersistanceId = typeof(ExecutionControl).Name, CancelClosing = true };
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
        public IEnumerable<OrderStatus> OrderStatuses { get; set; }

        public ExecutionControl(string persisitentId, string filterId, BaseTraderHandler tradeHander, string tabTitle = null, string exchange = null, string underlying = null, string contract = null, string portfolio = null)
        {
            InitializeComponent();
            TradeHandler = tradeHander;
            DEFAULT_ID = EXECUTION_DEFAULT_ID;

            if (TradeHandler != null)
                Initialize();
            PersistanceId = persisitentId;
            FilterSettingsWin.PersistanceId = persisitentId;
            FilterSettingsWin.OnFiltering += _executionSettingsWin_OnFiltering;
            FilterSettingsWin.FilterId = filterId;
            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;
            FilterSettingsWin.FilterPortfolio = portfolio;
        }

        public ExecutionControl()
        {
            InitializeComponent();
            DEFAULT_ID = EXECUTION_DEFAULT_ID;
            FilterSettingsWin.OnFiltering += _executionSettingsWin_OnFiltering;
            FilterSettingsWin.PersistanceId = PersistanceId;
            FilterSettingsWin.FilterId = DEFAULT_ID;
        }

        public ICollectionViewLiveShaping ExecutionChanged { get; set; }


        private void _executionSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;
            Filter();
        }

        private void RadioButton_Checked_AllOrder(object sender, RoutedEventArgs e)
        {
            FilterByStatus(null);
        }

        public void Refresh()
        {
            FilterByStatus(null);
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            var exchangeList = new List<string> { string.Empty };
            //exchangeList.AddRange((from p in (IEnumerable<OrderVM>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_executionSettingsWin.ExchangeCollection = exchangeList;
            FilterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            FilterSettingsWin.Show();
        }

        //public void Filter(string tabTitle,string exchange, string underlying, string contract, IEnumerable<OrderStatus> status)
        //{
        //    if (ExecutionTreeView == null)
        //    {
        //        return;
        //    }

        //    this.AnchorablePane.SelectedContent.Title = tabTitle;
        //    _filterSettingsWin.FilterTabTitle = tabTitle;
        //    _filterSettingsWin.FilterExchange = exchange;
        //    _filterSettingsWin.FilterUnderlying = underlying;
        //    _filterSettingsWin.FilterContract = contract;

        //    ICollectionView view = _viewSource.View;
        //    view.Filter = delegate (object o)
        //    {
        //        if (contract == null)
        //            return true;

        //        OrderVM evm = o as OrderVM;

        //        if (evm.Exchange.ContainsAny(exchange) &&
        //            evm.Contract.ContainsAny(contract) &&
        //            evm.Contract.ContainsAny(underlying) &&
        //            ((status == null) || status.Contains(evm.Status)))
        //        {
        //            return true;
        //        }

        //        return false;
        //    };
        //}

        public void Filter()
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            var tabTitle = FilterSettingsWin.FilterTabTitle;
            var exchange = FilterSettingsWin.FilterExchange;
            var underlying = FilterSettingsWin.FilterUnderlying;
            var contract = FilterSettingsWin.FilterContract;
            var portfolio = FilterSettingsWin.FilterPortfolio;

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                OrderVM evm = o as OrderVM;

                if (evm.Exchange.ContainsAny(exchange) &&
                    evm.Contract.ContainsAny(contract) &&
                    evm.Contract.ContainsAny(underlying) &&
                    evm.Portfolio.ContainsAny(portfolio) &&
                    ((OrderStatuses == null) || !OrderStatuses.Any() || OrderStatuses.Contains(evm.Status)))
                {
                    return true;
                }

                return false;
            };
        }
        public void FilterByPortfolio(string portfolio)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (portfolio == null)
                    return true;

                OrderVM ovm = o as OrderVM;

                if (ovm.Contract.Contains(portfolio))
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByContract(string contract)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                OrderVM ovm = o as OrderVM;

                if (ovm.Contract.Contains(contract))
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByStatus(IEnumerable<OrderStatus> statuses)
        {
            OrderStatuses = statuses;
            Filter();

        }

        public void Save()
        {
            FilterSettingsWin.Save();
            if (OrderStatuses != null)
            {
                foreach (var status in OrderStatuses)
                {
                    ClientDbContext.SaveOrderStatus(TradeHandler.MessageWrapper.User.Id, (int)status, FilterSettingsWin.FilterId);
                }
            }
        }

        private void RadioButton_Checked_TradedOrder(object sender, RoutedEventArgs e)
        {
            OrderStatuses = new HashSet<OrderStatus> { OrderStatus.ALL_TRADED };
            FilterByStatus(OrderStatuses);

        }

        private void RadioButton_Checked_CanceledOrder(object sender, RoutedEventArgs e)
        {
            OrderStatuses = new HashSet<OrderStatus> { OrderStatus.CANCELED };
            FilterByStatus(OrderStatuses);
        }

        private void RadioButton_Checked_RejectedOrder(object sender, RoutedEventArgs e)
        {
            OrderStatuses = new HashSet<OrderStatus> { OrderStatus.CANCEL_REJECTED,
                OrderStatus.REJECTED };
            FilterByStatus(OrderStatuses);
        }

        private void RadioButton_Checked_ActiveOrder(object sender, RoutedEventArgs e)
        {

            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                OrderVM ovm = o as OrderVM;

                return ovm.Active;
            };
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int brokerOrderSeq = int.Parse(b.CommandParameter.ToString());

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                ExecutionViewModel evm = o as ExecutionViewModel;
                if (evm.IsOrderOrTrade)
                {
                    if (evm.BrokerOrderSeq == brokerOrderSeq)
                    {
                        if (evm.Flag == null)
                        {
                            evm.Flag = true;
                        }
                        else
                        {
                            evm.Flag = !evm.Flag;
                        }
                    }
                    return true;
                }
                else
                {
                    if (evm.BrokerOrderSeq == brokerOrderSeq)
                    {
                        evm.Expanded = !evm.Expanded;
                        return evm.Expanded;
                    }
                }

                return false;
            };
        }

        private void ExecutionTreeView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, ExecutionTreeView.Items);
            }
        }

        private void MenuItem_Click_CancelOrder(object sender, RoutedEventArgs e)
        {
            OrderVM item = ExecutionTreeView.SelectedItem as OrderVM;
            if ((item != null) && item.Active)
            {
                MessageBoxResult dr = MessageBox.Show("是否确认取消订单", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                    MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().CancelOrder(item);
            }
            else
            {
                MessageBox.Show("该笔订单无法取消", "错误");
            }
        }

        private void MenuItem_Click_CancelAllOrder(object sender, RoutedEventArgs e)
        {
            //var OrdersActive = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OrderVMCollection.Where(o => o.Active);
            var OrdersActive = TradeHandler.OrderVMCollection.Where(o => o.Active);

            if (OrdersActive.Any())
            {
                MessageBoxResult dr = MessageBox.Show("是否确认取消所有订单", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    foreach (var orderactive in OrdersActive)
                    {
                        //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().CancelOrder(orderactive);
                        TradeHandler.CancelOrder(orderactive);

                    }
                }
            }
            else
            {
                MessageBox.Show("无可取消的订单", "错误");
            }

        }


        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.ShowDialog();
        }

        private void MenuItem_Click_DeleteWindow(object sender, RoutedEventArgs e)
        {

            ClientDbContext.DeleteFilterSettings(FilterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }

        private void MenuItem_Click_ShowAllExecution(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var executionControl = new ExecutionControl(PersistanceId, Guid.NewGuid().ToString(), TradeHandler);
                AnchorablePane.AddContent(executionControl).Title = title;
                executionControl.FilterSettingsWin.FilterTabTitle = title;
                executionControl.FilterSettingsWin.Save();
            }
        }

        public void ReloadData()
        {
            Initialize();
            //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OrderVMCollection.Clear();
            //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryOrder();
            LayoutAnchorable defaultTab =
                AnchorablePane.Children.FirstOrDefault(pane => ((ExecutionControl)pane.Content).FilterSettingsWin.FilterId == DEFAULT_ID);

            AnchorablePane.Children.Clear();
            if (defaultTab != null)
                AnchorablePane.Children.Add(defaultTab);
            var accountHandler = MessageHandlerContainer.DefaultInstance.Get<AccountHandler>();
            var filtersettings = ClientDbContext.GetFilterSettings(accountHandler.MessageWrapper.User.Id, PersistanceId);
            //var filtersettings = ClientDbContext.GetFilterSettings(TradeHandler.MessageWrapper.User.Id, PersistanceId);
            //var userId = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().MessageWrapper.User.Id;
            var userId = TradeHandler.MessageWrapper.User.Id;
            bool found = false;
            foreach (var fs in filtersettings)
            {
                var executionctrl = new ExecutionControl(PersistanceId, fs.Id, TradeHandler, fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
                AnchorablePane.AddContent(executionctrl).Title = fs.Title;
                if (fs.Id == DEFAULT_ID)
                    found = true;
                var statuses = ClientDbContext.GetOrderStatus(accountHandler.MessageWrapper.User.Id, fs.Id);
                executionctrl.FilterByStatus(statuses.Select(c => (OrderStatus)c));
                if (statuses.Contains((int)OrderStatus.OPENED))
                {
                    var titleopen = WPFUtility.GetLocalizedString("Opened", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                    executionctrl.FilterSettingsWin.Title += "  " + titleopen + " ";
                }
                else if ((statuses.Contains((int)OrderStatus.ALL_TRADED)) || (statuses.Contains((int)OrderStatus.PARTIAL_TRADING)))
                {
                    var titletraded = WPFUtility.GetLocalizedString("TRADED", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                    executionctrl.FilterSettingsWin.Title += "  " + titletraded + " ";
                }
            }
            if (found)
                AnchorablePane.Children.Remove(defaultTab);
        }

        public void Initialize()
        {
            _viewSource.Source = TradeHandler?.OrderVMCollection;
            ExecutionTreeView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(ExecutionTreeView);
            ExecutionChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (ExecutionChanged.CanChangeLiveFiltering)
            {
                ExecutionChanged.LiveFilteringProperties.Add("Status");
                ExecutionChanged.IsLiveFiltering = true;
            }
            TradeHandler.OrderVMCollection.Clear();
            TradeHandler.QueryOrder();
            FilterSettingsWin.UserID = TradeHandler.MessageWrapper?.User?.Id;
        }
    }
}
