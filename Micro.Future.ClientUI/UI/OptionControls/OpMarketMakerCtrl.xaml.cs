﻿using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OpMarketMakerCtrl : UserControl
    {
        private OTCOptionHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

        private CollectionViewSource _viewSourcePosition = new CollectionViewSource();
        private CollectionViewSource _viewSourceRisk = new CollectionViewSource();
        private CollectionViewSource _viewSourcePutOption = new CollectionViewSource();
        private CollectionViewSource _viewSourceCallOption = new CollectionViewSource();
        private CollectionViewSource _viewSourceVolatility = new CollectionViewSource();
        private CollectionViewSource _viewSource1 = new CollectionViewSource();

        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;


        private IList<ColumnObject> _optionColumns;

        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();
        public ObservableCollection<MarketDataVM> QuoteVMCollection1
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public ObservableCollection<StrategyVM> StrategyVMCollection
        {
            get;
        } = new ObservableCollection<StrategyVM>();


        public void Initialize()
        {
            _contractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);

            exchangeCB.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            _viewSource1.Source = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection;
            //underlyingCB.ItemsSource = _contractList.Select(c => c.ProductID).Distinct();
            // Initialize Market Data
            quoteListView1.ItemsSource = QuoteVMCollection1;
            pricingModelCB.ItemsSource = StrategyVMCollection.Select(c => c.PricingModel).Distinct();
            volModelCB.ItemsSource = StrategyVMCollection.Select(c => c.VolModel).Distinct();
            option_priceLV.ItemsSource = CallPutTDOptionVMCollection;
            _otcOptionHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;


            // Set columns tree
            var marketNode = new ColumnObject(new GridViewColumn() { Header = "行情" });
            var ivolNode = new ColumnObject(new GridViewColumn() { Header = "隐含波动率" });
            var riskGreekNode = new ColumnObject(new GridViewColumn() { Header = "风险参数" });
            var theoPriceNode = new ColumnObject(new GridViewColumn() { Header = "理论价格" });
            var QVNode = new ColumnObject(new GridViewColumn() { Header = "订单量" });
            var positionNode = new ColumnObject(new GridViewColumn() { Header = "持仓" });
            var QTNode = new ColumnObject(new GridViewColumn() { Header = "交易开关" });

            marketNode.Children.Add(ColumnObject.CreateColumn(PBid));
            marketNode.Children.Add(ColumnObject.CreateColumn(PBidSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(PAsk));
            marketNode.Children.Add(ColumnObject.CreateColumn(PAskSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(CBid));
            marketNode.Children.Add(ColumnObject.CreateColumn(CBidSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(CAsk));
            marketNode.Children.Add(ColumnObject.CreateColumn(CAskSize));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PMidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CMidIV));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CTheta));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(PBidTheo));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(PAskTheo));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(CBidTheo));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(CAskTheo));
            QVNode.Children.Add(ColumnObject.CreateColumn(PBidQV));
            QVNode.Children.Add(ColumnObject.CreateColumn(PAskQV));
            QVNode.Children.Add(ColumnObject.CreateColumn(CBidQV));
            QVNode.Children.Add(ColumnObject.CreateColumn(CAskQV));
            positionNode.Children.Add(ColumnObject.CreateColumn(PPosition));
            positionNode.Children.Add(ColumnObject.CreateColumn(CPosition));
            QTNode.Children.Add(ColumnObject.CreateColumn(PAskQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(PBidQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(CBidQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(CAskQT));

            marketNode.Initialize();
            ivolNode.Initialize();
            riskGreekNode.Initialize();
            theoPriceNode.Initialize();
            QVNode.Initialize();
            positionNode.Initialize();
            QTNode.Initialize();
            _optionColumns = new List<ColumnObject>() { marketNode, ivolNode, riskGreekNode, theoPriceNode, QVNode, positionNode, QTNode };

        }

        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM vm)
        {
            CallPutTDOptionVMCollection.Update(vm);
        }

        public OpMarketMakerCtrl()
        {

            InitializeComponent();
            Initialize();
        }

        public OptionVM OptionVM
        {
            get;
            private set;
        } = new OptionVM();

        private void MenuItem_Click_OptionColumns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(_optionColumns);
            win.Show();
        }

        private void exchangeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchangeCB.SelectedValue.ToString();
            underlyingCB.ItemsSource = _contractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();

        }

        private void underlyingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _contractList
                                           where c.ProductID == productId.ToString()
                                           select c.UnderlyingContract).Distinct().ToList();

                underlyingContractCB.ItemsSource = underlyingContracts;
            }
            //var productId = underlyingCB.SelectedItem.ToString();

            //if (productId != null)
            //{
            //    var underlyingContracts = (from c in _contractList
            //                               where c.ProductID == productId
            //                               select c.UnderlyingContract).Distinct().ToList();

            //    underlyingContractCB.ItemsSource = underlyingContracts;
            //}
        }

        private void underlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (underlyingContractCB.SelectedValue != null)
            {
                var uc = underlyingContractCB.SelectedValue.ToString();


                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

                var callList = (from o in optionList
                                where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();
                CallPutTDOptionVMCollection.Clear();
                var retList = handler.SubCallPutTDOptionData(strikeList, callList, putList);
                foreach (var vm in retList)
                {
                    CallPutTDOptionVMCollection.Add(vm);
                }
            }
            //if (underlyingContractCB.SelectedItem != null)
            //{
            //    var uc = underlyingContractCB.SelectedItem.ToString();

            //    var optionList = (from c in _contractList
            //                      where c.UnderlyingContract == uc
            //                      select c).ToList();

            //    var strikeList = (from o in optionList
            //                      orderby o.StrikePrice
            //                      select o.StrikePrice).Distinct().ToList();

            //    var handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

            //    var callList = (from o in optionList
            //                    where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
            //                    orderby o.StrikePrice
            //                    select o.Contract).Distinct().ToList();

            //    var putList = (from o in optionList
            //                   where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
            //                   orderby o.StrikePrice
            //                   select o.Contract).Distinct().ToList();
            //    CallPutTDOptionVMCollection.Clear();
            //    var retList = handler.SubCallPutTDOptionData(strikeList, callList, putList);
            //    foreach (var vm in retList)
            //    {
            //        CallPutTDOptionVMCollection.Add(vm);
            //    }
            //}
        }
        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = underlyingEX1.SelectedValue.ToString();
            underlyingCB1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
            underlyingContractCB1.ItemsSource = null;
        }
        private void underlyingCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB1.SelectedValue;

            if (productId != null)
            {
                var underlyingContracts = (from c in _futurecontractList
                                           where c.ProductID == productId.ToString()
                                           select c.Contract).Distinct().ToList();

                underlyingContractCB1.ItemsSource = underlyingContracts;
            }
        }
        public void underlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (underlyingContractCB1.SelectedItem != null)
            {
                var uc = underlyingContractCB1.SelectedItem.ToString();
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection1.Clear();
                var mktDataVM = handler.SubMarketData(uc);
                QuoteVMCollection1.Add(mktDataVM);
            }
        }

        private void pricingModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pricingModelCB.SelectedItem != null)
            {
                var pm = pricingModelCB.SelectedItem.ToString();
                var handler = _otcOptionHandler;
                foreach (var option in CallPutTDOptionVMCollection)
                {
                    option.CallStrategyVM.PricingModel = pm;
                     handler.UpdateStrategy(option.CallStrategyVM);
                    option.PutStrategyVM.PricingModel = pm;
                     handler.UpdateStrategy(option.PutStrategyVM);
                }

            }
        }

        private void volModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (volModelCB.SelectedItem != null)
            {
                var vm = volModelCB.SelectedItem.ToString();
                var handler = _otcOptionHandler;
                foreach (var option in CallPutTDOptionVMCollection)
                {
                    option.CallStrategyVM.VolModel = vm;
                    handler.UpdateStrategy(option.CallStrategyVM);
                    option.PutStrategyVM.VolModel = vm;
                    handler.UpdateStrategy(option.PutStrategyVM);
                }

            }
        }
    }
}
