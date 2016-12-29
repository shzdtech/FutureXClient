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
        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        public string PersistanceId
        {
            get;
            set;
        }

        public AccountInfoControl()
        {
            InitializeComponent();
            var fund = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().FundVM;
            FundVMCollection.Add(fund);
            FundListView.ItemsSource = FundVMCollection;
            mColumns = ColumnObject.GetColumns(FundListView);
            int interval = 2000;
            _timer = new Timer(ReloadDataCallback, null, interval, interval);
        }

        private void ReloadDataCallback(object state)
        {
            Dispatcher.Invoke(ReloadData);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryAccountInfo();
        }

        private void MenuItemColumns_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
    }
}
