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

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ClientExecutionWindow : UserControl,IReloadData
    {
        private ColumnObject[] mColumns;

        public ClientExecutionWindow()
        {
            InitializeComponent();

            ExecutionTreeView.ItemsSource = MessageHandlerContainer.
                DefaultInstance.Get<TraderExHandler>().OrderVMCollection;

            mColumns = ColumnObject.GetColumns(ExecutionTreeView);
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
            ExecutionSettingsWindow win = new ExecutionSettingsWindow();
            var orderVMCollection = (ObservableCollection<OrderVM>)ExecutionTreeView.ItemsSource;
            win.ExchangeCollection = (from p in orderVMCollection select p.Exchange).Distinct();
            if (win.ShowDialog() == true)
            {
                var layoutContent = WPFUtility.FindParent<LayoutContent>(this);
                if (layoutContent != null)
                    layoutContent.Title = win.ExecutionTitle;
                FilterByExchange(win.ExecutionExchange);
                FilterByContract(win.ExecutionContract);
                FilterByContract(win.ExecutionUnderlying);
            }
        }

        private void FilterByExchange(string exchange)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (exchange == null)
                    return true;

                OrderVM ovm = o as OrderVM;

                if (exchange.Contains(ovm.Exchange))
                {
                    return true;
                }

                return false;
            };
        }

        private void FilterByContract(string contract)
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

                if (contract.Contains(ovm.Contract))
                {
                    return true;
                }

                return false;
            };
        }

        private void FilterByStatus(IEnumerable<OrderStatus> statuses)
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
