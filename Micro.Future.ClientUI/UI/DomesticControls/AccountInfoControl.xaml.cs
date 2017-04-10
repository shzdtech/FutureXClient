using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System.Threading;
using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class AccountInfoControl : UserControl, IReloadData
    {
        private IList<ColumnObject> mColumns;
        private Timer _timer;
        private const int UpdateInterval = 2000;

        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        public BaseTraderHandler TradeHandler { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }

        public AccountInfoControl()
        {
            InitializeComponent();
            //var fund = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVM;
            FundListView.ItemsSource = FundVMCollection;
            mColumns = ColumnObject.GetColumns(FundListView);           
        }

        private void ReloadDataCallback(object state)
        {
            ReloadData();
        }

        public void ReloadData()
        {
            var fund = TradeHandler.FundVM;
            Dispatcher.Invoke(() =>
            {
                FundVMCollection.Clear();
                FundVMCollection.Add(fund);
            });
            _timer = new Timer(ReloadDataCallback, null, UpdateInterval, UpdateInterval);
            TradeHandler.QueryAccountInfo();
        }

        private void MenuItemColumns_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        public void Initialize()
        {
        }
    }
}
