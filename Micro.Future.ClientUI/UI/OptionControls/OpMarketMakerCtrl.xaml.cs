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
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OpMarketMakerCtrl : UserControl
    {
        public event Action<string, bool> OnPutBidStatusChanged;
        public event Action<string, bool> OnPutAskStatusChanged;
        public event Action<string, bool> OnCallBidStatusChanged;
        public event Action<string, bool> OnCallAskStatusChanged;

        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();

        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;
        private IEnumerable<ContractKeyVM> _subbedContracts;

        private IList<ColumnObject> _optionColumns;

        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();
        public ObservableCollection<MarketDataVM> QuoteVMCollection1
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public IEnumerable<ContractKeyVM> SubbedContracts
        {
            get;
            private set;
        }


        public void Initialize()
        {
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();

            exchangeCB.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            pricingModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("pm");

            // Initialize Market Data
            quoteListView1.ItemsSource = QuoteVMCollection1;
            option_priceLV.ItemsSource = CallPutTDOptionVMCollection;
            _otcOptionHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;


            // Set columns tree
            var marketNode = new ColumnObject(new GridViewColumn() { Header = "行情" });
            var ivolNode = new ColumnObject(new GridViewColumn() { Header = "隐含波动率" });
            var theovolNode = new ColumnObject(new GridViewColumn() { Header = "理论波动率" });
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
            marketNode.Children.Add(ColumnObject.CreateColumn(CMid));
            marketNode.Children.Add(ColumnObject.CreateColumn(PMid));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PMidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CMidIV));
            theovolNode.Children.Add(ColumnObject.CreateColumn(TheoAskVol));
            theovolNode.Children.Add(ColumnObject.CreateColumn(TheoBidVol));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PAskDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CAskDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PAskVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CAskVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PAskGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CAskGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PAskTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CAskTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PBidDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CBidDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PBidVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CBidVega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PBidGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CBidGamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PBidTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CBidTheta));
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
            theovolNode.Initialize();
            riskGreekNode.Initialize();
            theoPriceNode.Initialize();
            QVNode.Initialize();
            positionNode.Initialize();
            QTNode.Initialize();
            _optionColumns = new List<ColumnObject>() { marketNode, ivolNode, theovolNode, riskGreekNode, theoPriceNode, QVNode, positionNode, QTNode };

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
            underlyingCB.ItemsSource = null;
            underlyingContractCB.ItemsSource = null;
            expireDateCB.ItemsSource = null;
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
        }

        private void underlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var contract = underlyingContractCB.SelectedValue;
            if (contract != null)
            {
                var expireDate = (from c in _contractList
                                  where c.UnderlyingContract == contract.ToString() && c.Exchange == exchangeCB.SelectedValue.ToString()
                                  select c.ExpireDate).Distinct().ToList();

                expireDateCB.ItemsSource = expireDate;
            }

        }
        private async void expireDateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = exchangeCB.SelectedValue?.ToString();
            if (exchange != null)
            {
                if (expireDateCB.SelectedValue != null)
                {
                    var ed = expireDateCB.SelectedValue.ToString();
                    var uc = underlyingContractCB.SelectedValue.ToString();

                    var optionList = (from c in _contractList
                                      where c.Exchange == exchange && c.UnderlyingContract == uc && c.ExpireDate == ed
                                      select c).ToList();

                    var strikeList = (from o in optionList
                                      orderby o.StrikePrice descending
                                      select o.StrikePrice).Distinct().ToList();

                    var callList = (from o in optionList
                                    where o.ContractType == (int)ContractType.CONTRACTTYPE_CALL_OPTION
                                    orderby o.StrikePrice descending
                                    select new ContractKeyVM(exchange, o.Contract)).ToList();

                    var putList = (from o in optionList
                                   where o.ContractType == (int)ContractType.CONTRACTTYPE_PUT_OPTION
                                   orderby o.StrikePrice descending
                                   select new ContractKeyVM(exchange, o.Contract)).ToList();

                    var retList = _otcOptionHandler.MakeCallPutTDOptionData(strikeList, callList, putList);
                    _subbedContracts = await _otcOptionHandler.SubTradingDeskDataAsync(optionList.Select(c => new ContractKeyVM(c.Exchange, c.Contract)));

                    CallPutTDOptionVMCollection.Clear();
                    foreach (var vm in retList)
                    {
                        CallPutTDOptionVMCollection.Add(vm);
                    }
                    var strategyVM = CallPutTDOptionVMCollection.FirstOrDefault().CallStrategyVM ?? 
                        CallPutTDOptionVMCollection.LastOrDefault().CallStrategyVM;
                    if (strategyVM != null)
                    {
                        pricingModelCB.ItemsSource = null;
                        volModelLB.Content = null;
                        riskFree_Interest.DataContext = null;
                        adjustment.DataContext = null;
                        underlyingEX1.ItemsSource = null;
                        underlyingCB1.ItemsSource = null;
                        underlyingContractCB1.ItemsSource = null;
                        underlyingEX1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
                        pricingModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("pm");
                        if (strategyVM != null)
                        {
                            var pricingContractParamVM = strategyVM.PricingContractParams?.FirstOrDefault();
                            if (pricingContractParamVM != null)
                            {
                                var futureexchange = pricingContractParamVM.Exchange;
                                var futurecontract = pricingContractParamVM.Contract;
                                var futureunderlying = _futurecontractList.FirstOrDefault(c => c.Exchange == futureexchange && c.Contract == futurecontract)?.ProductID;
                                var adjust = pricingContractParamVM.Adjust;
                                var pricingmodel = strategyVM.PricingModel;
                                var volmodel = strategyVM.VolModel;
                                pricingModelCB.SelectedValue = pricingmodel;
                                underlyingEX1.SelectedValue = futureexchange;
                                underlyingCB1.SelectedValue = futureunderlying;
                                underlyingContractCB1.SelectedValue = futurecontract;
                                volModelLB.Content = volmodel;
                                adjustment.Value = adjust;
                                var modelVM = pricingModelCB.SelectedItem as ModelParamsVM;
                                if (modelVM != null)
                                {
                                    riskFree_Interest.DataContext = modelVM;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }


        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = underlyingEX1.SelectedItem?.ToString();
            if (exchange != null)
            {
                underlyingCB1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
                underlyingContractCB1.ItemsSource = null;
            }
        }
        private void underlyingCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var productId = underlyingCB1.SelectedItem?.ToString();

            if (productId != null)
            {
                var underlyingContracts = (from c in _futurecontractList
                                           where c.ProductID == productId
                                           select c.Contract).Distinct().ToList();

                underlyingContractCB1.ItemsSource = underlyingContracts;
            }
        }
        public async void underlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var uc = underlyingContractCB1.SelectedItem?.ToString();
            var uexchange = underlyingEX1.SelectedItem?.ToString();
            if (uc != null)
            {
                var handler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                QuoteVMCollection1.Clear();
                var mktDataVM = await handler.SubMarketDataAsync(uc);
                if (mktDataVM != null)
                    QuoteVMCollection1.Add(mktDataVM);
                var contract = _subbedContracts?.FirstOrDefault();
                if (contract != null)
                {
                    var strategy =
                        _otcOptionHandler.StrategyVMCollection.FirstOrDefault(s => s.Exchange == contract.Exchange && s.Contract == contract.Contract);
                    var pricingContract = strategy.PricingContractParams.FirstOrDefault();
                    if (pricingContract != null && pricingContract.Exchange == uexchange && pricingContract.Contract == uc)
                        return;

                    if (pricingContract == null)
                    {
                        pricingContract = new PricingContractParamVM();
                        strategy.PricingContractParams.Add(pricingContract);
                    }
                    pricingContract.Exchange = uexchange;
                    pricingContract.Contract = uc;
                    _otcOptionHandler.UpdateStrategyPricingContracts(strategy, StrategyVM.Model.PM);
                }
            }
        }

        private void pricingModelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelParam = pricingModelCB.SelectedItem as ModelParamsVM;
            if (modelParam != null)
            {
                riskFree_Interest.DataContext = modelParam;
                var option = CallPutTDOptionVMCollection.FirstOrDefault();
                if (option?.CallStrategyVM == null || option.CallStrategyVM.PricingModel == modelParam.InstanceName)
                    return;
                option.CallStrategyVM.PricingModel = modelParam.InstanceName;
                _otcOptionHandler.UpdateStrategyModel(option.CallStrategyVM, StrategyVM.Model.PM);
                option.PutStrategyVM.PricingModel = modelParam.InstanceName;
                _otcOptionHandler.UpdateStrategyModel(option.PutStrategyVM, StrategyVM.Model.PM);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {

                    StrategyVM strategyVM = ctrl.DataContext as StrategyVM;
                    if (strategyVM != null)
                    {
                        if (e.Key == Key.Enter)
                            strategyVM.UpdateStrategy();
                        else
                        {
                            ctrl.DataContext = null;
                            ctrl.DataContext = strategyVM;
                        }
                    }
                    ctrl.Background = Brushes.White;
                }
                else
                {
                    ctrl.Background = Brushes.MistyRose;
                }
            }
        }

        private void PutBidCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnPutBidStatusChanged?.Invoke(checkbox.Tag.ToString(), checkbox.IsChecked.Value);
        }
        private void PutBidCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnPutBidStatusChanged?.Invoke(checkbox.Tag.ToString(), false);
        }
        private void PutAskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnPutAskStatusChanged?.Invoke(checkbox.Tag.ToString(), checkbox.IsChecked.Value);
        }
        private void PutAskCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnPutAskStatusChanged?.Invoke(checkbox.Tag.ToString(), false);
        }
        private void CallBidCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnCallBidStatusChanged?.Invoke(checkbox.Tag.ToString(), checkbox.IsChecked.Value);
        }
        private void CallBidCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnCallBidStatusChanged?.Invoke(checkbox.Tag.ToString(), false);
        }
        private void CallAskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnCallAskStatusChanged?.Invoke(checkbox.Tag.ToString(), checkbox.IsChecked.Value);
        }
        private void CallAskCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null)
                OnCallAskStatusChanged?.Invoke(checkbox.Tag.ToString(), false);
        }

        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var modelParamsVM = updownctrl.DataContext as ModelParamsVM;
                if (modelParamsVM != null)
                {
                    var key = updownctrl.Tag.ToString();
                    _otcOptionHandler.UpdateModelParams(modelParamsVM.InstanceName, key, updownctrl.Value.Value);
                }
            }
        }
        private void Adjustment_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue != null)
            {
                if (_otcOptionHandler != null)
                {
                    var callPutTDOptionVM = CallPutTDOptionVMCollection.FirstOrDefault();
                    if (callPutTDOptionVM != null)
                    {
                        var strategyVM = callPutTDOptionVM.CallStrategyVM;
                        if (strategyVM != null)
                        {
                            var pricingContract = strategyVM.PricingContractParams.FirstOrDefault();
                            if (pricingContract != null)
                            {
                                pricingContract.Adjust = (double)e.NewValue;
                                _otcOptionHandler.UpdateStrategyPricingContracts(strategyVM, StrategyVM.Model.PM);
                            }
                        }
                    }
                }
            }
        }

    }
}
