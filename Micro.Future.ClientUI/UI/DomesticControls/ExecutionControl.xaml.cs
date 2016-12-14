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

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ExecutionControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private ColumnObject[] mColumns;

        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWindow = new FilterSettingsWindow();
        private FilterSettingsWindow _filterSettingsWin =
    new FilterSettingsWindow() { PersistanceId = typeof(ExecutionControl).Name, CancelClosing = true };
        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane{ get; set;}
        public string PersistanceId
        {
            get;
            set;
        }

        public ExecutionControl()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance
                .Get<TraderExHandler>().OrderVMCollection;

            _filterSettingsWindow.OnFiltering += _executionSettingsWin_OnFiltering;

            ExecutionTreeView.ItemsSource = _viewSource.View;

            mColumns = ColumnObject.GetColumns(ExecutionTreeView);

        }

        private void _executionSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWindow.FilterTabTitle;
            Filter(tabTitle, exchange, underlying, contract);
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
            _filterSettingsWindow.Show();
        }

        public void Filter(string tabTitle,string exchange, string underlying, string contract)
        {
            if (ExecutionTreeView == null)
            {
                return;
            }

            for (int count = 0; count < this.AnchorablePane.ChildrenCount; count++)
            {
                //MessageBox.Show(this.AnchorablePane.Children[count].Title);
                if (this.AnchorablePane.Children[count].Title.Equals(tabTitle))
                {
                    MessageBox.Show("已存在同名窗口,请重新输入.");
                    return;
                }
            }

            this.AnchorablePane.SelectedContent.Title = tabTitle;

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


        private void MenuItem_Click_ShowAllExecution(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
                AnchorablePane.AddContent(new ExecutionControl()).Title = WPFUtility.GetLocalizedString("AllExecution", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }



        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().OrderVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryOrder();
            var filtersettings = ClientDbContext.GetFilterSettings(MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().MessageWrapper.User.Id, PersistanceId);
            foreach (var fs in filtersettings)
            {
                var executionctrl = new PositionControl();
                AnchorablePane.AddContent(executionctrl).Title = fs.Title;
                executionctrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
            }
        }
    }
}
