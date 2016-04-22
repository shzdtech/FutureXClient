﻿using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class ClientPositionWindow : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;

        public ClientPositionWindow()
        {
            InitializeComponent();
            var positionVMCollection = new DispatchObservableCollection<PositionVM>(this);
            PositionListView.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().PositionVMCollection =
            positionVMCollection;

            mColumns = ColumnObject.GetColumns(PositionListView);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryPosition();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        //private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        //{
        //    PositionVM vm = PositionListView.SelectedItem as PositionVM;
        //    if (vm != null)
        //    {
        //        MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().CloseMarketOrder(vm);
        //    }
        //    else
        //    {
        //        MessageBox.Show("请选择持仓合约", "错误");
        //    }
        //}
    }
}
