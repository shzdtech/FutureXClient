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

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ClientTradeWindow : UserControl,IReloadData
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();

        public LayoutContent LayoutContent { get; set; }

        public ClientTradeWindow()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance
                .Get<TraderExHandler>().TradeVMCollection;

            TradeTreeView.ItemsSource = _viewSource.View;

            mColumns = ColumnObject.GetColumns(TradeTreeView);
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
            TradeSettingsWindow win = new TradeSettingsWindow()
            {
                ExchangeCollection = (from p in (IEnumerable<TradeVM>)_viewSource.Source
                                      select p.Exchange).Distinct()
            };

            if (win.ShowDialog() == true)
            {
                if (LayoutContent != null)
                    LayoutContent.Title = win.TradeTitle;
                Filter(win.TradeExchange, win.TradeContract, win.TradeUnderlying);

            }
        }

        public void FilterByStatus(IEnumerable<OrderOffsetType> statuses)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            CollectionViewSource viewSource = new CollectionViewSource();
            viewSource.Source = TradeTreeView.ItemsSource;
            ICollectionView view = viewSource.View;
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

                if ((string.IsNullOrEmpty(exchange) || tvm.Exchange.Contains(exchange)) &&
                (string.IsNullOrEmpty(contract) || tvm.Contract.Contains(contract)) &&
                (string.IsNullOrEmpty(underlying) || tvm.Contract.Contains(underlying)))
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
