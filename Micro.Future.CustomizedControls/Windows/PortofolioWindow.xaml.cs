using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Micro.Future.CustomizedControls.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class PortoforlioWindow : Window, IReloadData
    {
        private ColumnObject[] mColumns;
        private CollectionViewSource _viewSource = new CollectionViewSource();

        public PortoforlioWindow()
        {
            InitializeComponent();

            _viewSource.Source = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
            portofolioListView.ItemsSource = _viewSource.View;
            mColumns = ColumnObject.GetColumns(portofolioListView);
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

        private IEnumerable<MarketDataVM> SeletedQuoteVM
        {
            get
            {
                var selectedItems = portofolioListView.SelectedItems;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    yield return selectedItems[i] as MarketDataVM;
                }
            }
        }


        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            if (portofolioTextBox.Text == "")
            {
                this.portofolioTextBox.Background = new SolidColorBrush(Colors.Red);
                MessageBox.Show("输入不能为空");
                this.portofolioTextBox.Background = new SolidColorBrush(Colors.White);
                return;
            }

            using (var clientCtx = new ClientDbContext())
            {
                var query = from contractInfo in clientCtx.ContractInfo where contractInfo.Contract == portofolioTextBox.Text select contractInfo;
                if (query.Any() == false)
                {
                    this.portofolioTextBox.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("输入不存在");
                    portofolioTextBox.Text = "";
                    this.portofolioTextBox.Background = new SolidColorBrush(Colors.White);
                }
            }

            var quote = portofolioTextBox.Text;

            var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                       QuoteVMCollection.FirstOrDefault((obj) => string.Compare(obj.Contract, quote, true) == 0);


            if (item != null)
            {
                portofolioListView.SelectedItem = item;
            }
            else
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
            }
        }
    }
}
