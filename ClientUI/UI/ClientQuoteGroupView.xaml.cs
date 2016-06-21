using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.ComponentModel;
using Micro.Future.Windows;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class ClientQuoteGroupView : UserControl, IReloadData
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();
        public LayoutContent LayoutContent { get; set; }


        public ClientQuoteGroupView()
        {
            InitializeComponent();
            _viewSource.Source = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
            quoteListView.ItemsSource = _viewSource.View;


        }

        public event Action<QuoteViewModel> OnQuoteSelected;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                UnsubMarketData(SeletedQuoteVM);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                ResubMarketData();
        }

        private IEnumerable<QuoteViewModel> SeletedQuoteVM
        {
            get
            {
                var selectedItems = quoteListView.SelectedItems;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    yield return selectedItems[i] as QuoteViewModel;
                }
            }
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            var quote = contractTextBox.Text;

            var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                       QuoteVMCollection.Find((obj)=>string.Compare(obj.Contract, quote, true) == 0);

            if (item != null)
            {
                quoteListView.SelectedItem = item;
            }
            else
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                    SubMarketData(quote);
            }
        }

        private void quoteListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnQuoteSelected != null)
            {
                QuoteViewModel quoteVM = quoteListView.SelectedItem as QuoteViewModel;
                OnQuoteSelected(quoteVM);
            }
        }

        private void MenuItem_Click_Columns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            QuoteSettingsWindow win = new QuoteSettingsWindow()
            {
                ExchangeCollection = (from p in (IEnumerable<QuoteViewModel>)_viewSource.Source
                                      select p.Exchange).Distinct()
            };
                 
            if (win.ShowDialog() == true)
            {
                if (LayoutContent != null)
                    LayoutContent.Title = win.QuoteTitle;
                Filter(win.QuoteExchange, win.QuoteContract, win.QuoteUnderlying);
            }
        }

        public void Filter(string exchange, string underlying, string contract)
        {
            if (quoteListView == null)
            {
                return;
            }

            ICollectionView view = _viewSource.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                QuoteViewModel qvm = o as QuoteViewModel;

                if ((string.IsNullOrEmpty(exchange) || qvm.Exchange.Contains(exchange)) &&
                (string.IsNullOrEmpty(contract) || qvm.Contract.Contains(contract)) &&
                (string.IsNullOrEmpty(underlying) || qvm.Contract.Contains(underlying)))
                {
                    return true;
                }

                return false;
            };
        }

        public void FilterByContract(string contract)
        {
            if (quoteListView == null)
            {
                return;
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(quoteListView.ItemsSource);
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                QuoteViewModel qvm = o as QuoteViewModel;

                if (qvm.Contract.Contains(contract))
                {
                    return true;
                }

                return false;
            };
        }

    }
}
