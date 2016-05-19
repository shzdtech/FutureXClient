﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Windows;
using System.Collections.ObjectModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ClientTradeWindow : UserControl,IReloadData
    {
        private ColumnObject[] mColumns;

        public ClientTradeWindow()
        {
            InitializeComponent();

            TradeTreeView.ItemsSource = MessageHandlerContainer.
                DefaultInstance.Get<TraderExHandler>().TradeVMCollection;

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
            TradeSettingsWindow win = new TradeSettingsWindow();
            var tradeVMCollection = (ObservableCollection<TradeVM>)TradeTreeView.ItemsSource;
            win.ExchangeCollection = (from p in tradeVMCollection select p.Exchange).Distinct();
            if (win.ShowDialog() == true)
            {
                FilterByExchange(win.TradeExchange);
                FilterByContract(win.TradeContract);
                FilterByContract(win.TradeUnderlying);
            }
        }

        private void FilterByStatus(IEnumerable<OrderOffsetType> statuses)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(TradeTreeView.ItemsSource);
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

        private void FilterByExchange(string exchange)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(TradeTreeView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (exchange == null)
                    return true;

                TradeVM tvm = o as TradeVM;

                if (exchange.Contains(tvm.Exchange))
                {
                    return true;
                }

                return false;
            };
        }

        private void FilterByContract(string contract)
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

                if (contract.Contains(tvm.Contract))
                {
                    return true;
                }

                return false;
            };
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            //FilterByStatus(new List<TradingType> { TradingType.ALL_TRADED });
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
