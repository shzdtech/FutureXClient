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
using Micro.Future.CustomizedControls;
using Micro.Future.CustomizedControls.Controls;
using Micro.Future.Resources.Localization;
using Micro.Future.LocalStorage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;

namespace Micro.Future.UI
{
    /// <summary>
    /// Positions.xaml 的交互逻辑
    /// </summary>
    public partial class PositionControl : UserControl, IReloadData, ILayoutAnchorableControl
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        private FilterSettingsWindow _filterSettingsWin =
            new FilterSettingsWindow() { PersistanceId = typeof(PositionControl).Name, CancelClosing = true };
        private static ObservableCollection<PositionVM> PositionCollection
        {
            get;
        } = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().PositionVMCollection;

        public static TraderExHandler TradeHandler
        {
            get;
        } = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();

        private static IList<MarketDataVM> _marketDataList = new List<MarketDataVM>();

        public LayoutContent LayoutContent { get; set; }

        public LayoutAnchorablePane AnchorablePane { get; set; }

        public string PersistanceId
        {
            get;
            set;
        }

        public PositionControl(string filterId)
        {
            InitializeComponent();

            _viewSource.Source = PositionCollection;

            MessageHandlerContainer.DefaultInstance
            .Get<MarketDataHandler>().OnNewMarketData += PositionControl_OnNewMarketData;

            _filterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;

            PositionListView.ItemsSource = _viewSource.View;


            PositionChanged = _viewSource.View as ICollectionViewLiveShaping;
            if (PositionChanged.CanChangeLiveFiltering)
            {
                PositionChanged.LiveFilteringProperties.Add("Direction");
                PositionChanged.IsLiveFiltering = true;
            }

            mColumns = ColumnObject.GetColumns(PositionListView);
            _filterSettingsWin.FilterId = filterId;

        }

        public static void PositionControl_OnNewMarketData(MarketDataVM mktDataVM)
        {
            var positions = PositionCollection.FindByContract(mktDataVM.Contract);
            foreach (var positionVM in positions)
            {
                if (positionVM.Direction == PositionDirectionType.PD_LONG)
                {
                    positionVM.Profit = (mktDataVM.LastPrice - positionVM.MeanCost) * positionVM.Position * positionVM.Multiplier;
                }
                else if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                {
                    positionVM.Profit = (positionVM.MeanCost - mktDataVM.LastPrice) * positionVM.Position * positionVM.Multiplier;
                }
            }
        }

        public PositionControl() : this(Guid.NewGuid().ToString())
        {
        }
        public ICollectionViewLiveShaping PositionChanged { get; set; }
        private void _filterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _filterSettingsWin.FilterTabTitle;
            Filter(tabTitle, exchange, underlying, contract);
        }

        public static event Action<PositionVM> OnPositionSelected;


        public void ReloadData()
        {
            TradeHandler.PositionVMCollection.Clear();
            TradeHandler.QueryPosition();

            while (AnchorablePane.ChildrenCount > 1)
                AnchorablePane.Children.RemoveAt(1);

            var filtersettings = ClientDbContext.GetFilterSettings(TradeHandler.MessageWrapper.User.Id, _filterSettingsWin.PersistanceId);

            foreach (var fs in filtersettings)
            {
                var positionctrl = new PositionControl(fs.Id);
                AnchorablePane.AddContent(positionctrl).Title = fs.Title;
                positionctrl.Filter(fs.Title, fs.Exchange, fs.Underlying, fs.Contract);
            }
            if (filtersettings.Any())
                AnchorablePane.RemoveChildAt(0);

            LoadMarketData();
        }

        private async void LoadMarketData()
        {
            await Task.Delay(5000);
            _marketDataList = await MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>()
                .SubMarketDataAsync(PositionCollection.Select(c=>c.Contract).Distinct());
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
            _filterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            _filterSettingsWin.Show();
        }


        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            if (AnchorablePane != null)
                AnchorablePane.AddContent(new PositionControl()).Title = WPFUtility.GetLocalizedString("Position", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
        }

        private void MenuItem_Click_DeleteWindow(object sender, RoutedEventArgs e)
        {

            ClientDbContext.DeleteFilterSettings(_filterSettingsWin.FilterId);
            AnchorablePane.RemoveChild(AnchorablePane.SelectedContent);
        }


        private void PositionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionVM positionVM = PositionListView.SelectedItem as PositionVM;
            OnPositionSelected?.Invoke(positionVM);
        }

        public void Filter(string tabTitle, string exchange, string underlying, string contract)
        {
            if (PositionListView == null)
            {
                return;
            }

            this.AnchorablePane.SelectedContent.Title = tabTitle;
            _filterSettingsWin.FilterTabTitle = tabTitle;
            _filterSettingsWin.FilterExchange = exchange;
            _filterSettingsWin.FilterUnderlying = underlying;
            _filterSettingsWin.FilterContract = contract;

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

        private void PositionListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, PositionListView.Items);
            }
        }
    }
}
