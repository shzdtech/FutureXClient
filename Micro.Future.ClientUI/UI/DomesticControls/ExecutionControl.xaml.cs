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
        private ColumnObject[] mColumns;

        private CollectionViewSource _viewSource = new CollectionViewSource();
        public FilterSettingsWindow FilterSettingsWin { get; } = new FilterSettingsWindow() { PersistanceId = typeof(ExecutionControl).Name, CancelClosing = true };
        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane { get; set; }
        public string PersistanceId
        {
            get;
            set;
        }
        public IEnumerable<OrderStatus> OrderStatuses { get; set; }

        public ExecutionControl(string filterId, string tabTitle = null, string exchange = null, string underlying = null, string contract = null)
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance
                .Get<TraderExHandler>().OrderVMCollection;

            FilterSettingsWin.OnFiltering += _executionSettingsWin_OnFiltering;

            ExecutionTreeView.ItemsSource = _viewSource.View;

            ExecutionChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (ExecutionChanged.CanChangeLiveFiltering)
            {
                ExecutionChanged.LiveFilteringProperties.Add("Status");
                ExecutionChanged.IsLiveFiltering = true;
            }

            mColumns = ColumnObject.GetColumns(ExecutionTreeView);

            FilterSettingsWin.FilterId = filterId;
            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;

        }

        public ExecutionControl() : this(Guid.NewGuid().ToString())
        {
        }

        public ICollectionViewLiveShaping ExecutionChanged { get; set; }


        private void _executionSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
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

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                OrderVM evm = o as OrderVM;

                if (evm.Exchange.ContainsAny(exchange) &&
                    evm.Contract.ContainsAny(contract) &&
                    evm.Contract.ContainsAny(underlying) &&
                    ((OrderStatuses == null) || OrderStatuses.Contains(evm.Status)))
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

        private void MenuItem_Click_ShowAllExecution(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var executionControl = new ExecutionControl();
                AnchorablePane.AddContent(new ExecutionControl()).Title = title;
                executionControl.FilterSettingsWin.FilterTabTitle = title;
                executionControl.FilterSettingsWin.Save();
            }
        }



        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OrderVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryOrder();

            while (AnchorablePane.ChildrenCount > 1)
                AnchorablePane.Children.RemoveAt(1);

            var filtersettings = ClientDbContext.GetFilterSettings(MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().MessageWrapper.User.Id, FilterSettingsWin.PersistanceId);

            foreach (var fs in filtersettings)
            {
                var executionctrl = new ExecutionControl(fs.Id, fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
                AnchorablePane.AddContent(executionctrl).Title = fs.Title;
                executionctrl.Filter();
            }
            if (filtersettings.Any())
                AnchorablePane.RemoveChildAt(0);
        }
    }
}
