using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Micro.Future.ViewModel;
using PBMsgTrader;
using Micro.Future.Business;

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class OTCExecutionWindow : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;

        public ExecutionCollection ExecutionVMCollection;

        public OTCExecutionWindow()
        {
            InitializeComponent();

            mColumns = ColumnObject.GetColumns(ExecutionTreeView);


            ExecutionVMCollection = new ExecutionCollection(this);
            ExecutionTreeView.ItemsSource = TraderExHandler.Instance.ExecutionVMCollection =
            ExecutionVMCollection;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            FilterByStatus(0);
        }

        public void Refresh()
        {
            FilterByStatus(0);
        }

        private void FilterByStatus(PBOrderStatus status)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate(object o)
            {
                ExecutionViewModel evm = o as ExecutionViewModel;
                if (evm.IsOrderOrTrade)
                {
                    if ((int)status == 0)
                    {
                        return true;
                    }

                    if (evm.Status == status)
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            FilterByStatus(PBOrderStatus.ALL_FINISHED);
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            FilterByStatus(PBOrderStatus.TTIS_ORDER_CANCEL_SUCCESS);
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate(object o)
            {
                ExecutionViewModel evm = o as ExecutionViewModel;
                if (evm.IsOrderOrTrade)
                {
                    if ((int)evm.Status == 0)
                    {
                        return true;
                    }

                    if ((evm.Status == PBOrderStatus.TTIS_ORDER_INSERT_FAILED) ||
                        (evm.Status == PBOrderStatus.TTIS_ORDER_CANCEL_FAILED) ||
                        (evm.Status == PBOrderStatus.TTIS_ORDER_OTHER ))
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int brokerOrderSeq = int.Parse(b.CommandParameter.ToString());

            ICollectionView view = CollectionViewSource.GetDefaultView(ExecutionTreeView.ItemsSource);
            view.Filter = delegate(object o)
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OrderVM item = ExecutionTreeView.SelectedItem as OrderVM;
            if ((item != null) && item.Active )
            {
                TraderExHandler.Instance.CancelOrder(item);
            }
            else
            {
                MessageBox.Show("该笔订单无法取消", "错误");
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        public void ReloadData()
        {
            throw new NotImplementedException();
        }
    }
}
