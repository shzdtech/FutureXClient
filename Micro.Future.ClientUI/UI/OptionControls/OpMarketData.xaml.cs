using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
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
using Micro.Future.Resources.Localization;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OpMarketData : UserControl
    {

        private IList<ContractInfo> _contractList;
        private CollectionViewSource _viewSource = new CollectionViewSource();

        public OpMarketData()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            using (var clientCache = new ClientDbContext())
            {
                _contractList = clientCache.ContractInfo.Where(c => c.ProductType == 1).ToList();
            }

            underlyingCB.ItemsSource = _contractList.Select(c => c.ProductID).Distinct();
            underlyingCB1.ItemsSource = _contractList.Select(c => c.ProductID).Distinct();
            _viewSource.Source = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
            quoteListView1.ItemsSource = _viewSource.View;
            quoteListView2.ItemsSource = _viewSource.View;


        }

        private void underlyingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB.SelectedItem.ToString();

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId
                                           select c.UnderlyingContract).Distinct().ToList();

                underlyingContractCB.ItemsSource = underlyingContracts;
            }
        }

        private void underlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (underlyingContractCB.SelectedItem != null)
            {
                var uc = underlyingContractCB.SelectedItem.ToString();

                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var handler = MessageHandlerContainer.DefaultInstance.Get<CTPOptionDataHandler>();

                var callList = (from o in optionList
                                where o.ContractType == 2
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == 3
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();
                handler.CallPutOptionVMCollection.Clear();

                handler.SubCallPutOptionData(strikeList, callList, putList);
            }
        }

        private void underlyingCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB1.SelectedItem.ToString();

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId
                                           select c.UnderlyingContract).Distinct().ToList();

                underlyingContractCB1.ItemsSource = underlyingContracts;
            }
        }
        private void underlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (underlyingContractCB1.SelectedItem != null)
            {
                var uc = underlyingContractCB1.SelectedItem.ToString();

                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var handler = MessageHandlerContainer.DefaultInstance.Get<CTPOptionDataHandler>();

                var callList = (from o in optionList
                                where o.ContractType == 2
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == 3
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();
                handler.CallPutOptionVMCollection.Clear();

                handler.SubCallPutOptionData(strikeList, callList, putList);
            }
        }

        private void underlyingTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void contractTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (contractTextBox1.Text == "")
                {
                    this.contractTextBox1.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("输入合约不能为空");
                    this.contractTextBox1.Background = new SolidColorBrush(Colors.White);
                    return;
                }

                using (var clientCtx = new ClientDbContext())
                {
                    var query = from contractInfo in clientCtx.ContractInfo where contractInfo.Contract == contractTextBox1.Text select contractInfo;
                    if (query.Any() == false)
                    {
                        this.contractTextBox1.Background = new SolidColorBrush(Colors.Red);
                        MessageBox.Show("输入合约不存在");
                        contractTextBox1.Text = "";
                        this.contractTextBox1.Background = new SolidColorBrush(Colors.White);
                    }
                }

                var quote = contractTextBox1.Text;

                var item = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().
                           QuoteVMCollection.FirstOrDefault((obj) => string.Compare(obj.Contract, quote, true) == 0);

                if (item != null)
                {
                    quoteListView1.SelectedItem = item;
                }
                else
                {
                    MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData(quote);
                }
            }
        }

        private void exchange1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
