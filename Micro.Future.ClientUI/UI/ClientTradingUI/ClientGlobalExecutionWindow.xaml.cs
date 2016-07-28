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

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGlobalExecutionWindow : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;

        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWin = new FilterSettingsWindow();


        public LayoutContent LayoutContent { get; set; }

        public ClientGlobalExecutionWindow()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance
                .Get<TraderExHandler>().OrderVMCollection;

            _filterSettingsWin.OnFiltering += _executionSettingsWin_OnFiltering;

            ExecutionTreeView.ItemsSource = _viewSource.View;

            mColumns = ColumnObject.GetColumns(ExecutionTreeView);
        }

        private void _executionSettingsWin_OnFiltering(string exchange, string underlying, string contract)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWin.FilterTitle;
            Filter(exchange, underlying, contract);
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

            _filterSettingsWin.Show();
        }

        public void Filter(string exchange, string underlying, string contract)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                OrderVM evm = o as OrderVM;

                if (evm.Exchange.ContainsAny(exchange) &&
                    evm.Contract.ContainsAny(contract) &&
                    evm.Contract.ContainsAny(underlying))
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
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (statuses == null)
                    return true;

                OrderVM ovm = o as OrderVM;

                if (statuses.Contains(ovm.Status))
                {
                    return true;
                }

                return false;
            };
        }

        private void RadioButton_Checked_TradedOrder(object sender, RoutedEventArgs e)
        {
            FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED });
        }

        private void RadioButton_Checked_CanceledOrder(object sender, RoutedEventArgs e)
        {
            FilterByStatus(new List<OrderStatus> { OrderStatus.CANCELED });
        }

        private void RadioButton_Checked_RejectedOrder(object sender, RoutedEventArgs e)
        {
            FilterByStatus(new List<OrderStatus> { OrderStatus.OPEN_REJECTED,
                OrderStatus.CANCEL_REJECTED,
                OrderStatus.REJECTED });
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
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //Get clicked column
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    //Get binding property of clicked column
                    string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                    SortDescriptionCollection sdc = ExecutionTreeView.Items.SortDescriptions;
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
            OrderVM item = ExecutionTreeView.SelectedItem as OrderVM;
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

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OrderVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryOrder();
        }
    }
}
