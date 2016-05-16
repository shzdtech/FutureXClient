using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Micro.Future.Windows;
using System.Collections.ObjectModel;
using System.Linq;

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

        public event Action<PositionVM> OnPositionSelected;


        public void ReloadData()
        {

            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryPosition();
        }

        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            Window1 win = new Window1();
            var positionVMCollection = (ObservableCollection<PositionVM>)PositionListView.ItemsSource;
            win.ExchangeCollection = (from p in positionVMCollection select p.Exchange).Distinct();
            if (win.ShowDialog() == true)
            {
                FilterByExchange(win.PositionExchange);
                FilterByContract(win.PositionContract);
                FilterByContract(win.PositionUnderlying);
            }
        }


        private void PositionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnPositionSelected != null)
            {
                PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
                OnPositionSelected(positionVM);
            }
        }

        private void FilterByExchange(string exchange)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(PositionListView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (exchange == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (exchange.Contains(pvm.Exchange))
                {
                    return true;
                }

                return false;
            };
        }

        private void FilterByContract(string contract)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(PositionListView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (contract.Contains(pvm.Contract))
                {
                    return true;
                }

                return false;
            };
        }

        private void FilterByDirection(PositionDirectionType? direction)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(PositionListView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (direction == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (direction==pvm.Direction)
                {
                    return true;
                }

                return false;
            };
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
