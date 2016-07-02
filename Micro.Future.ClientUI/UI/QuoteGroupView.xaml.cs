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

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class QuoteGroupView : UserControl
    {
        public ObservableCollection<QuoteViewModel> Quotes { get; set; }
        private ColumnObject[] mColumns;

        public QuoteGroupView()
        {
            InitializeComponent();

            Quotes = new ObservableCollection<QuoteViewModel>();
            QuoteListView.ItemsSource = Quotes;

            mColumns = ColumnObject.GetColumns(QuoteListView);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.Show();
        }

        private void QuoteListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            QuoteViewModel quote = QuoteListView.SelectedItem as QuoteViewModel;
            if (quote != null)
            {
                //MainWindow.MyInstance.FastOrderCtl.FastOrderContractTxt.DataContext = quote;
                MainWindow.MyInstance.FastOrderCtl.PricePanel.DataContext = quote;
                MainWindow.MyInstance.FastOrderCtl.ViewModel.SymbolID = quote.Contract;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            CommonRibbon.LoadNewQuoteWin();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ViewModel.RibbonModel.DeleteCurrentQuote();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            ViewModel.RibbonModel.ClearQuotes();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var dlg = new InputBox("重命名报价组");
            dlg.Owner = MainWindow.MyInstance;
            if (dlg.ShowDialog() == true)
            {
                ViewModel.RibbonModel.RenameQuoteGroup(dlg.Value);
            }

            dlg.Close();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            var dlg = new InputBox("新增报价组");
            dlg.Owner = MainWindow.MyInstance;
            if (dlg.ShowDialog() == true)
            {
                ViewModel.RibbonModel.NewQuoteGroup(dlg.Value);
            }

            dlg.Close();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            ViewModel.RibbonModel.DeleteQuoteGroup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string symbol = b.CommandParameter.ToString();
            QuoteViewModel quote = null;
            foreach (QuoteViewModel view in 
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection)
            {
                if (view.Contract == symbol)
                {
                    quote = view;
                    
                    break;
                }
            }

            PositionViewModel longPosition = null;
            PositionViewModel shortPosition = null;
            foreach (PositionViewModel view in TradeHandler.Instance.Positions)
            {
                if (view.RawData.InstrumentID == symbol)
                {
                    if (view.RawData.PosiDirection == "2")
                    {
                        longPosition = view;
                    }
                    else
                    {
                        shortPosition = view;
                    }
                }
            }

            FundViewModel fund = TradeHandler.Instance.Fund;

            //to load advanced order window
            AdvancedMakeOrderWin win = new AdvancedMakeOrderWin();
            win.SetBindingData(quote, longPosition, shortPosition, fund);
            win.Show();
        }
    }
}
