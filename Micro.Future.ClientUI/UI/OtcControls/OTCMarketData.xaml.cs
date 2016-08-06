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

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class OTCMarketData : UserControl, IReloadData
    {
        public OTCMarketData()
        {
            InitializeComponent();

            quoteListView.ItemsSource =
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            //win.Show();
        }
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().UnsubMarketData(SeletedQuoteVM);
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().ResubMarketData();
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
                       QuoteVMCollection.Find((obj) => string.Compare(obj.Contract, quote, true) == 0);

            if (item != null)
            {
                quoteListView.SelectedItem = item;
            }
            else
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
            }
        }

        
    }
}
