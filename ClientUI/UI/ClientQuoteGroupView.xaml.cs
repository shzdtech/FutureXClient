using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class ClientQuoteGroupView : UserControl, IReloadData
    {
        public DispatchObservableCollection<QuoteViewModel> QuoteVMCollection 
        { get; private set; }

        public ClientQuoteGroupView()
        {
            InitializeComponent();

            QuoteVMCollection = new DispatchObservableCollection<ViewModel.QuoteViewModel>(this);
            quoteListView.ItemsSource = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection =
                QuoteVMCollection;

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
                for(int i = 0; i < selectedItems.Count;i++)
                {
                    yield return selectedItems[i] as QuoteViewModel;
                }
            }
        }
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            var quote = contractTextBox.Text;
            var item = from q in QuoteVMCollection where quote == q.Symbol select q;
            if (item.Any())
            {
                quoteListView.SelectedItem = item.First();
            }
            else
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote); 
            }
        }
    }
}
