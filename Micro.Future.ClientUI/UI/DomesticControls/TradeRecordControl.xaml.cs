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
using Micro.Future.LocalStorage;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Micro.Future.UI
{
    /// <summary>
    /// OrderDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TradeRecordControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private const string DEFAULT_ID = "E0FD10D9-8D28-4DDE-B2BC-96FAC72992C8";
        private IList<ColumnObject> mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public FilterSettingsWindow FilterSettingsWin
        { get; } = new FilterSettingsWindow() { PersistanceId = typeof(TradeRecordControl).Name, CancelClosing = true };
        public BaseTraderHandler TradeHandler { get; set; }
        public string PersistanceId
        {
            get;
            set;
        }

        public LayoutAnchorablePane AnchorablePane { get; set; }

        public TradeRecordControl(string filterId, BaseTraderHandler tradeHander = null)
        {
            InitializeComponent();

            TradeHandler = tradeHander;
            if (TradeHandler != null)
                Initialize();

            //_viewSource.Source = MessageHandlerContainer.DefaultInstance?.Get<TraderExHandler>()?.TradeVMCollection;

            FilterSettingsWin.OnFiltering += FilterSettingsWin_OnFiltering;
            FilterSettingsWin.FilterId = filterId;
        }

        public TradeRecordControl() : this(DEFAULT_ID, null)
        {
        }

        private void FilterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
        {
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;
            Filter(tabTitle, exchange, underlying, contract);
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
            FilterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            FilterSettingsWin.Show();
        }

        public void FilterByStatus(IEnumerable<OrderOpenCloseType> statuses)
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

        public void Filter(string tabTitle, string exchange, string underlying, string contract)
        {
            if (TradeTreeView == null)
            {
                return;
            }

            // AnchorablePane.SelectedContent.Title = tabTitle;
            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;


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
            FilterByStatus(new List<OrderOpenCloseType> { OrderOpenCloseType.OPEN });
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
                //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().CancelOrder(item);
                TradeHandler.CancelOrder(item);

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
            var tradeWin = new TradeRecordControl(Guid.NewGuid().ToString(), TradeHandler);
            tradeWin.FilterByStatus(new List<OrderOpenCloseType> { OrderOpenCloseType.OPEN });
            if (AnchorablePane != null)
                AnchorablePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Open", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_CloseTrade(object sender, RoutedEventArgs e)
        {
            var tradeWin = new TradeRecordControl();
            tradeWin.FilterByStatus(new List<OrderOpenCloseType> { OrderOpenCloseType.CLOSE });
            if (AnchorablePane != null)
                AnchorablePane.AddContent(tradeWin).Title = WPFUtility.GetLocalizedString("Close", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_AllTrade(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var tradeRecordControltrl = new TradeRecordControl(Guid.NewGuid().ToString(), TradeHandler);
                AnchorablePane.AddContent(tradeRecordControltrl).Title = title;
                tradeRecordControltrl.FilterSettingsWin.FilterTabTitle = title;
                tradeRecordControltrl.FilterSettingsWin.Save();
            }
        }
        private void MenuItem_Click_DeleteWindow(object sender, RoutedEventArgs e)
        {

            ClientDbContext.DeleteFilterSettings(FilterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }

        private void TradeTreeView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, TradeTreeView.Items);
            }
        }

        public void ReloadData()
        {
            //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().TradeVMCollection.Clear();
            //MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryTrade();
            Initialize();
            //while (AnchorablePane.ChildrenCount > 1)
            //    AnchorablePane.Children.RemoveAt(1);
            LayoutAnchorable defaultTab = 
                AnchorablePane.Children.FirstOrDefault(pane => ((TradeRecordControl)pane.Content).FilterSettingsWin.FilterId == DEFAULT_ID);

            AnchorablePane.Children.Clear();
            if (defaultTab != null)
                AnchorablePane.Children.Add(defaultTab);

            var filtersettings = ClientDbContext.GetFilterSettings(TradeHandler.MessageWrapper.User.Id, FilterSettingsWin.PersistanceId);

            bool found = false;

            foreach (var fs in filtersettings)
            {
                var traderecordctrl = new TradeRecordControl(fs.Id, TradeHandler);
                AnchorablePane.AddContent(traderecordctrl).Title = fs.Title;
                traderecordctrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract);

                if (fs.Id == DEFAULT_ID)
                    found = true;
            }

            if (found)
                AnchorablePane.Children.Remove(defaultTab);
        }

        public void Initialize()
        {
            _viewSource.Source = TradeHandler?.TradeVMCollection;
            TradeTreeView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(TradeTreeView);
            TradeHandler.TradeVMCollection.Clear();
            TradeHandler.QueryTrade();
            FilterSettingsWin.UserID = TradeHandler.MessageWrapper.User.Id;
        }
    }
}
