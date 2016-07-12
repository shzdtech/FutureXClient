using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Micro.Future.Windows;
using System.Linq;
using System.Collections.Generic;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.Utility;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class ClientPositionWindow : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWin =
            new FilterSettingsWindow() { CancelClosing = true };

        public LayoutContent LayoutContent { get; set; }

        ~ClientPositionWindow()
        {
            _filterSettingsWin.CancelClosing = false;
            _filterSettingsWin.Close();
        }

        public ClientPositionWindow()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance
                .Get<TraderExHandler>().PositionVMCollection;

            _filterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;

            PositionListView.ItemsSource = _viewSource.View;


            PositionChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (PositionChanged.CanChangeLiveFiltering)
            {
                PositionChanged.LiveFilteringProperties.Add("Direction");
                PositionChanged.IsLiveFiltering = true;
            }

            mColumns = ColumnObject.GetColumns(PositionListView);
        }

        public ICollectionViewLiveShaping PositionChanged { get; set; }
        private void _filterSettingsWin_OnFiltering(string exchange, string underlying, string contract)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWin.FilterTitle;
            Filter(exchange, underlying, contract);
        }

        public event Action<PositionVM> OnPositionSelected;


        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().PositionVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryPosition();
        }

        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.ShowDialog();
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            var exchangeList = new List<string> { string.Empty };
            //exchangeList.AddRange((from p in (IEnumerable<PositionVM>)_viewSource.Source
            //                       select p.Exchange).Distinct());
            //_positionSettingsWin.ExchangeCollection = exchangeList;

            _filterSettingsWin.Show();
        }


        private void PositionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnPositionSelected != null)
            {
                PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
                OnPositionSelected(positionVM);
            }
        }

        public void Filter(string exchange, string underlying, string contract)
        {
            if (PositionListView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (pvm.Exchange.ContainsAny(exchange) &&
                    pvm.Contract.ContainsAny(underlying) &&
                    pvm.Contract.ContainsAny(contract))
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

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (direction == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (direction == pvm.Direction)
                {
                    return true;
                }

                return false;
            };
        }
    }
}
