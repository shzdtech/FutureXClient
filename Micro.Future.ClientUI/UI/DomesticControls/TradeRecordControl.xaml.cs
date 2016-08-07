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
using Micro.Future.Utility;
using Micro.Future.CustomizedControls;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.Resources.Localization;

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TradeRecordControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWin = new FilterSettingsWindow();


        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane { get; set; }

        public TradeRecordControl()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance?.Get<TraderExHandler>()?.TradeVMCollection;

            _filterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
            TradeTreeView.ItemsSource = _viewSource.View;

            mColumns = ColumnObject.GetColumns(TradeTreeView);
        }

        private void _filterSettingsWin_OnFiltering(string exchange, string underlying, string contract)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWin.FilterTitle;
            Filter(exchange, underlying, contract);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
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
            //exchangeList.AddRange((from p in (IEnumerable<TradeVM>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_tradeSettingsWin.ExchangeCollection = exchangeList;

            _filterSettingsWin.Show();
        }

        public void FilterByStatus(IEnumerable<OrderOffsetType> statuses)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (statuses == null)
                    return true;

                TradeVM tvm = o as TradeVM;

                if (statuses.Contains(tvm.OpenClose))
                {
                    return true;
                }

                return false;
            };
        }

        public void Filter(string exchange, string underlying, string contract)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                TradeVM tvm = o as TradeVM;

                if (tvm.Exchange.ContainsAny(exchange) &&
                    tvm.Contract.ContainsAny(contract) &&
                    tvm.Contract.ContainsAny(underlying))
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByContract(string contract)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(TradeTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                TradeVM tvm = o as TradeVM;

                if (tvm.Contract.Contains(contract))
                {
                    return true;
                }

                return false;
            };
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int brokerOrderSeq = int.Parse(b.CommandParameter.ToString());

            ICollectionView view = CollectionViewSource.GetDefaultView(TradeTreeView.ItemsSource);
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
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                    SortDescriptionCollection sdc = TradeTreeView.Items.SortDescriptions;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }
        }

        private void MenuItem_Click_CancelOrder(object sender, RoutedEventArgs e)
        {
            OrderVM item = TradeTreeView.SelectedItem as OrderVM;
            if ((item != null) && item.Active)
            {
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

        private void MenuItem_Click_OpenTrade(object sender, RoutedEventArgs e)
        {
            var tradeWin = new TradeRecordControl();
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
            if (AnchorablePane != null)
                AnchorablePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Open", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_CloseTrade(object sender, RoutedEventArgs e)
        {
            var tradeWin = new TradeRecordControl();
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.CLOSE });
            if (AnchorablePane != null)
                AnchorablePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Close", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_AllTrade(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
                AnchorablePane.AddContent(new TradeRecordControl()).Title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void TradeTreeView_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().TradeVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryTrade();
        }
    }
}
