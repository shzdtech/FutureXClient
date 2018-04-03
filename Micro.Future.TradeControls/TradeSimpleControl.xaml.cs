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
    public partial class TradeSimpleControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private const string TRADE_DEFAULT_ID = "E0FD10D9-8D28-4DDE-B2BC-96FAC72992C8";
        private Timer _timer;
        private const int UpdateInterval = 2000;
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
        public LayoutAnchorablePane AnchorablePane { get; set; }

        public TradeSimpleControl(string persisitentId, string filterId, BaseTraderHandler tradeHander = null)
        {
            InitializeComponent();
            DEFAULT_ID = TRADE_DEFAULT_ID;
            mColumns = ColumnObject.GetColumns(TradeTreeView);
            TradeHandler = tradeHander;
            if (TradeHandler != null)
                Initialize();
            PersistanceId = persisitentId;
        }

        public TradeSimpleControl() : this(TRADE_DEFAULT_ID, null)
        {
            InitializeComponent();
            DEFAULT_ID = TRADE_DEFAULT_ID;
            mColumns = ColumnObject.GetColumns(TradeTreeView);
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


        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.ShowDialog();
        }

        private void MenuItem_Click_AllTrade(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
            {
                var title = WPFUtility.GetLocalizedString("AllTraded", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
                var tradeRecordControltrl = new TradeRecordControl(PersistanceId, Guid.NewGuid().ToString(), TradeHandler);
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

            Initialize();

        }

        public void Initialize()
        {
            _viewSource.Source = TradeHandler.TradeVMCollection;
            TradeTreeView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(TradeTreeView);
            //TradeHandler.TradeVMCollection.Clear();
            TradeHandler.QueryTrade();
            _timer = new Timer(UpdateTradeCallback, null, UpdateInterval, UpdateInterval);
        }
        private void UpdateTradeCallback(object state)
        {
            TradeHandler.QueryTrade();
        }
        public void BindingToListView(BaseTraderHandler tradeHandler)
        {
            _viewSource.Source = tradeHandler.TradeVMCollection;
            TradeTreeView.ItemsSource = _viewSource.View;
        }
    }
}
