using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using System;
using System.ComponentModel;
using System.Windows.Data;
using Micro.Future.Utility;
using System.Collections.Generic;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class OPPositionLV : UserControl
    {
        private IList<ColumnObject> mColumns;
        private FilterSettingsWindow _filterSettingsWin = new FilterSettingsWindow() { CancelClosing = true };
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public LayoutContent LayoutContent { get; set; }
        public LayoutAnchorablePane AnchorablePane { get; set; }


        public OPPositionLV()
        {
            InitializeComponent();
            _viewSource.Source = MessageHandlerContainer.DefaultInstance
            .Get<TraderExHandler>().PositionVMCollection;
            PositionListView.ItemsSource = _viewSource.View;
            _filterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
            mColumns = ColumnObject.GetColumns(PositionListView);
        }

        public event Action<PositionVM> OnPositionSelected;

        private void PositionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnPositionSelected != null)
            {
                PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
                OnPositionSelected(positionVM);
            }
        }
        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().PositionVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().QueryPosition();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
        private void _filterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract,string portfolio)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWin.FilterTabTitle;
            Filter(tabTitle, exchange, underlying, contract, portfolio);
        }
        public void Filter(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            if (PositionListView == null)
            {
                return;
            }

            this.AnchorablePane.SelectedContent.Title = tabTitle;

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                PositionVM pvm = o as PositionVM;

                if (pvm.Exchange.ContainsAny(exchange) &&
                    pvm.Contract.ContainsAny(underlying) &&                    
                    pvm.Contract.ContainsAny(contract) &&
                    pvm.Portfolio.ContainsAny(portfolio))
                {
                    return true;
                }

                return false;
            };
        }


    }
}
