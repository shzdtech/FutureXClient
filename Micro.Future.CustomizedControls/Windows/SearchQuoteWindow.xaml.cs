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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// NewQuote.xaml 的交互逻辑
    /// </summary>
    public partial class NewQuoteWin : Window
    {
        public NewQuoteWin()
        {
            InitializeComponent();
        }

        private string _selectedSymbol;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(_selectedSymbol);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTree(true);
        }

        private void InitTree(bool byProductClassOrExchange)
        {
            CountryViewModel viewModel ;
            if (byProductClassOrExchange)
            {
                var query = from info in InstrumentVMList.Instance select info.ProductClass;
                viewModel = new CountryViewModel(query.Distinct().ToArray(), byProductClassOrExchange);
                
            }
            else
            {
                var query = from info in InstrumentVMList.Instance select info.RawData.ExchangeID;
                viewModel = new CountryViewModel(query.Distinct().ToArray(), byProductClassOrExchange);
            }

            base.DataContext = viewModel;
        }

        private void ProductListTreeView_Selected(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as TreeViewItem).HasHeader)
            {
                CityViewModel cv = (e.OriginalSource as TreeViewItem).Header as CityViewModel;
                if (cv != null)
                {
                    cv.LoadDetails(this);
                    _selectedSymbol = cv.Name;
                }
            }
        }

        void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CountryViewModel root = ProductListTreeView.DataContext as CountryViewModel;
                if (root!=null)
                {
                   root.SearchCommand.Execute(null);
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitTree((sender as ComboBox).SelectedIndex == 0);
        }

    }
}
