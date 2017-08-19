using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private TraderExHandler _tradeExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private AbstractOTCHandler _abstractOTCHandler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
        private MarketDataHandler _marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();



        private IList<ContractInfo> _contractList;
        private IList<ContractInfo> _futurecontractList;
        private IEnumerable<ContractKeyVM> _subbedContracts;

        private IList<ColumnObject> _optionColumns;

        //~OpMarketMakerCtrl()
        //{
        //    AutoOrderUpdate(false);
        //}

        public ObservableCollection<CallPutTDOptionVM> CallPutTDOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutTDOptionVM>();
        public ObservableCollection<PositionVM> PositionVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();
        public ObservableCollection<MarketDataVM> QuoteVMCollection1
        {
            get;
        } = new ObservableCollection<MarketDataVM>();
        public IEnumerable<ContractKeyVM> SubbedContracts
        {
            get;
            private set;
        }

        public void GetContractInfo()
        {
            _futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();

            exchangeCB.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            underlyingEX1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            pricingModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("pm");
        }
        public void Initialize()
        {
            //_futurecontractList = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_FUTURE);
            //var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            //var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            //_contractList = options.Union(otcOptions).ToList();

            //exchangeCB.ItemsSource = _contractList.Select(c => c.Exchange).Distinct();
            //underlyingEX1.ItemsSource = _futurecontractList.Select(c => c.Exchange).Distinct();
            //pricingModelCB.ItemsSource = _otcOptionHandler.GetModelParamsVMCollection("pm");

            // Initialize Market Data
            quoteListView1.ItemsSource = QuoteVMCollection1;
            option_priceLV.ItemsSource = CallPutTDOptionVMCollection;

            _otcOptionHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;
            _tradeExHandler.OnPositionUpdated += OnPositionUpdated;


            //StrategyVM.MaxLimitOrder = await _otcOptionHandler.QueryMaxLimitOrderAsync();

            // Set columns tree
            var marketNode = new ColumnObject(new GridViewColumn() { Header = "行情" });
            var ivolNode = new ColumnObject(new GridViewColumn() { Header = "波动率" });
            var riskGreekNode = new ColumnObject(new GridViewColumn() { Header = "风险参数" });
            var theoPriceNode = new ColumnObject(new GridViewColumn() { Header = "理论价格" });
            var positionNode = new ColumnObject(new GridViewColumn() { Header = "持仓" });
            var QTNode = new ColumnObject(new GridViewColumn() { Header = "交易相关" });

            marketNode.Children.Add(ColumnObject.CreateColumn(PBid));
            marketNode.Children.Add(ColumnObject.CreateColumn(PBidSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(PAsk));
            marketNode.Children.Add(ColumnObject.CreateColumn(PAskSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(PCloseValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(PPreCloseValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(POpenValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(PVolume));
            marketNode.Children.Add(ColumnObject.CreateColumn(PTurnover));
            marketNode.Children.Add(ColumnObject.CreateColumn(POpenInterest));
            marketNode.Children.Add(ColumnObject.CreateColumn(PHighValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(PLowValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(PLastPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(PSettlePrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(PPreSettlePrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(PUpperLimitPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(PLowerLimitPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(PMid));
            marketNode.Children.Add(ColumnObject.CreateColumn(CBid));
            marketNode.Children.Add(ColumnObject.CreateColumn(CBidSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(CAsk));
            marketNode.Children.Add(ColumnObject.CreateColumn(CAskSize));
            marketNode.Children.Add(ColumnObject.CreateColumn(CCloseValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(CPreCloseValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(COpenValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(CVolume));
            marketNode.Children.Add(ColumnObject.CreateColumn(CTurnover));
            marketNode.Children.Add(ColumnObject.CreateColumn(COpenInterest));
            marketNode.Children.Add(ColumnObject.CreateColumn(CHighValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(CLowValue));
            marketNode.Children.Add(ColumnObject.CreateColumn(CLastPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(CSettlePrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(CPreSettlePrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(CUpperLimitPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(CLowerLimitPrice));
            marketNode.Children.Add(ColumnObject.CreateColumn(CMid));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(PMidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(TheoAskVol));
            ivolNode.Children.Add(ColumnObject.CreateColumn(TheoBidVol));
            ivolNode.Children.Add(ColumnObject.CreateColumn(TheoMidVol));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CBidIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CAskIV));
            ivolNode.Children.Add(ColumnObject.CreateColumn(CMidIV));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(PBidTheo));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(PAskTheo));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PRho));
            positionNode.Children.Add(ColumnObject.CreateColumn(PPosition));
            positionNode.Children.Add(ColumnObject.CreateColumn(TotalPosition));
            positionNode.Children.Add(ColumnObject.CreateColumn(MixFuture));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(PTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(Vega));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(Gamma));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CDelta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CTheta));
            riskGreekNode.Children.Add(ColumnObject.CreateColumn(CRho));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(CBidTheo));
            theoPriceNode.Children.Add(ColumnObject.CreateColumn(CAskTheo));
            positionNode.Children.Add(ColumnObject.CreateColumn(CPosition));
            QTNode.Children.Add(ColumnObject.CreateColumn(PBidQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(PBidQV));
            QTNode.Children.Add(ColumnObject.CreateColumn(PBidCnt));
            QTNode.Children.Add(ColumnObject.CreateColumn(PAskQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(PAskQV));
            QTNode.Children.Add(ColumnObject.CreateColumn(PAskCnt));
            QTNode.Children.Add(ColumnObject.CreateColumn(PAODepth));
            QTNode.Children.Add(ColumnObject.CreateColumn(PNotCross));
            QTNode.Children.Add(ColumnObject.CreateColumn(PCloseMode));
            QTNode.Children.Add(ColumnObject.CreateColumn(POrderCnt));
            QTNode.Children.Add(ColumnObject.CreateColumn(CBidQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(CBidQV));
            QTNode.Children.Add(ColumnObject.CreateColumn(CBidCnt));
            QTNode.Children.Add(ColumnObject.CreateColumn(CAskQT));
            QTNode.Children.Add(ColumnObject.CreateColumn(CAskQV));
            QTNode.Children.Add(ColumnObject.CreateColumn(CAskCnt));
            QTNode.Children.Add(ColumnObject.CreateColumn(CAODepth));
            QTNode.Children.Add(ColumnObject.CreateColumn(CNotCross));
            QTNode.Children.Add(ColumnObject.CreateColumn(CCloseMode));
            QTNode.Children.Add(ColumnObject.CreateColumn(COrderCnt));
            marketNode.Initialize();
            ivolNode.Initialize();
            riskGreekNode.Initialize();
            theoPriceNode.Initialize();
            positionNode.Initialize();
            QTNode.Initialize();
            _optionColumns = new List<ColumnObject>() { marketNode, ivolNode, riskGreekNode, theoPriceNode, positionNode, QTNode };

        }


        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM vm)
        {
            CallPutTDOptionVMCollection.Update(vm);
        }
        private void OnPositionUpdated(PositionVM vm)
        {
            CallPutTDOptionVMCollection.UpdatePosition(vm);

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
                                           orderby c.UnderlyingContract ascending
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
            if (AutoOrder_CheckBox.IsChecked.Value)
                AutoOrderUpdate(false);

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

                    var marketDataList = await _marketdataHandler.SubMarketDataAsync(optionList.Select(c => new ContractKeyVM(c.Exchange, c.Contract)));
                    var retList = _otcOptionHandler.MakeCallPutTDOptionData(strikeList, callList, putList, marketDataList);
                    _subbedContracts = await _otcOptionHandler.SubTradingDeskDataAsync(optionList.Select(c => new ContractKeyVM(c.Exchange, c.Contract)));

                    CallPutTDOptionVMCollection.Clear();
                    foreach (var vm in retList)
                    {
                        CallPutTDOptionVMCollection.Add(vm);
                        var CallPositionLong = 0;
                        var CallPositionShort = 0;
                        var PutPositionLong = 0;
                        var PutPositionShort = 0;
                        var CallPositionVMLong = _tradeExHandler.PositionVMCollection.FirstOrDefault(c => c.Contract == vm.CallOptionVM.Contract && c.Direction == PositionDirectionType.PD_LONG);
                        var PutPositionVMLong = _tradeExHandler.PositionVMCollection.FirstOrDefault(c => c.Contract == vm.PutOptionVM.Contract && c.Direction == PositionDirectionType.PD_LONG);
                        var CallPositionVMShort = _tradeExHandler.PositionVMCollection.FirstOrDefault(c => c.Contract == vm.CallOptionVM.Contract && c.Direction == PositionDirectionType.PD_SHORT);
                        var PutPositionVMShort = _tradeExHandler.PositionVMCollection.FirstOrDefault(c => c.Contract == vm.PutOptionVM.Contract && c.Direction == PositionDirectionType.PD_SHORT);
                        if (CallPositionVMLong != null)
                        {
                            CallPositionLong = CallPositionVMLong.Position;
                        }
                        if (CallPositionVMShort != null)
                        {
                            CallPositionShort = CallPositionVMShort.Position;
                        }
                        if (PutPositionVMLong != null)
                        {
                            PutPositionLong = PutPositionVMLong.Position;
                        }
                        if (PutPositionVMShort != null)
                        {
                            PutPositionShort = PutPositionVMShort.Position;
                        }
                        vm.PutOptionVM.Position = PutPositionLong - PutPositionShort;
                        vm.CallOptionVM.Position = CallPositionLong - CallPositionShort;
                    }
                    var strategyVM = CallPutTDOptionVMCollection.FirstOrDefault().CallStrategyVM ??
                        CallPutTDOptionVMCollection.LastOrDefault().CallStrategyVM;
                    if (strategyVM != null)
                    {
                        pricingModelCB.ItemsSource = null;
                        volModelLB.Content = null;
                        riskFree_Interest.DataContext = null;
                        adjustment.DataContext = null;
                        AutoOrder_CheckBox.DataContext = null;
                        underlyingEX1.ItemsSource = null;
                        underlyingCB1.ItemsSource = null;
                        orderConditionCombo.SelectedValue = null;
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
                                AutoOrder_CheckBox.DataContext = strategyVM;
                                CountertextBox.DataContext = strategyVM;
                                AutoMaxTradeUpdate((int)CountertextBox.Value);
                                TickSizeIUD.DataContext = strategyVM;
                                orderConditionCombo.SelectedValue = OrderConditionType.LIMIT;
                                orderConditionSelectionChanged(OrderConditionType.LIMIT);
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
        private void IntSpinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }
        public void AutoMaxTradeUpdate(int autoMaX)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.MaxAutoTrade = autoMaX;
                        vm.CallStrategyVM.UpdateStrategy();
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.MaxAutoTrade = autoMaX;
                        vm.PutStrategyVM.UpdateStrategy();
                    }
                }
            }
        }

        private void MaxAutoTradeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var strategyVM = updownctrl.DataContext as StrategyVM;
                if (strategyVM != null)
                {
                    int maxAutoTrade = (int)e.NewValue;
                    AutoMaxTradeUpdate(maxAutoTrade);
                }
            }
        }
        public void TickSizeUpdate(int ticksize)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.TickSize = ticksize;
                        vm.CallStrategyVM.UpdateStrategy();
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.TickSize = ticksize;
                        vm.PutStrategyVM.UpdateStrategy();
                    }
                }
            }
        }

        private void TickSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var strategyVM = updownctrl.DataContext as StrategyVM;
                if (strategyVM != null)
                {
                    int ticksize = (int)e.NewValue;
                    TickSizeUpdate(ticksize);
                }
            }
        }


        private void underlyingEX1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = underlyingEX1.SelectedItem?.ToString();
            if (exchange != null)
            {
                var underlying = (from c in _futurecontractList
                                  where c.Exchange == exchange.ToString()
                                  orderby c.ProductID ascending
                                  select c.ProductID).Distinct().ToList();
                //underlyingCB1.ItemsSource = _futurecontractList.Where(c => c.Exchange == exchange).Select(c => c.ProductID).Distinct();
                underlyingCB1.ItemsSource = underlying;
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
                                           orderby c.Contract ascending
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
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as IntegerUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                StrategyVM strategyVM = updownctrl.Tag as StrategyVM;
                if (strategyVM != null)
                {
                    strategyVM.UpdateStrategy();
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Enter)
                {

                    StrategyVM strategyVM = ctrl.Tag as StrategyVM;
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

        private void Risk_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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

        public void AutoOrderUpdate(bool autoStatus)
        {

            var orderCDSelectedValue = orderConditionCombo.SelectedValue == null ? OrderConditionType.LIMIT : (OrderConditionType)orderConditionCombo.SelectedValue;
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.ConditionType = orderCDSelectedValue;
                        vm.CallStrategyVM.Hedging = autoStatus;
                        vm.CallStrategyVM.UpdateStrategy();

                    }
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.ConditionType = orderCDSelectedValue;

                        vm.PutStrategyVM.Hedging = autoStatus;
                        vm.PutStrategyVM.UpdateStrategy();
                    }
                }
            }
        }
        private void AutoOrder_Checked(object sender, RoutedEventArgs e)
        {
            AutoOrderUpdate(true);
        }

        private void AutoOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoOrderUpdate(false);
        }
        public void BidNotCrossUpdate(bool bncStatus)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.BidNotCross = bncStatus;
                        vm.CallStrategyVM.UpdateStrategy();
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.BidNotCross = bncStatus;
                        vm.PutStrategyVM.UpdateStrategy();
                    }
                }
            }
        }

        private void BidNotCross_Checked(object sender, RoutedEventArgs e)
        {
            BidNotCrossUpdate(true);
        }

        private void BidNotCross_Unchecked(object sender, RoutedEventArgs e)
        {
            BidNotCrossUpdate(false);
        }
        public void CounterRefreshUpdate()
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.UpdateStrategy(true);
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.UpdateStrategy(true);
                    }
                }
            }
        }

        private void RefreshCounter_Click(object sender, RoutedEventArgs e)
        {
            CounterRefreshUpdate();
        }
        public void PCloseModeUpdate(bool bncStatus)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.PutStrategyVM != null)
                    {
                        vm.PutStrategyVM.CloseMode = bncStatus;
                        vm.PutStrategyVM.UpdateStrategy();
                    }
                }
            }
        }
        public void CCloseModeUpdate(bool bncStatus)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        vm.CallStrategyVM.CloseMode = bncStatus;
                        vm.CallStrategyVM.UpdateStrategy();
                    }
                }
            }
        }

        private void PCloseMode_Checked(object sender, RoutedEventArgs e)
        {
            PCloseModeUpdate(true);
        }

        private void PCloseMode_Unchecked(object sender, RoutedEventArgs e)
        {
            PCloseModeUpdate(false);

        }

        private void CCloseMode_Checked(object sender, RoutedEventArgs e)
        {
            CCloseModeUpdate(true);

        }

        private void CCloseMode_UnChecked(object sender, RoutedEventArgs e)
        {
            CCloseModeUpdate(false);

        }

        private void option_priceLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void orderCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        if(orderConditionCombo.SelectedValue!=null)
                        {
                            vm.CallStrategyVM.ConditionType = (OrderConditionType)orderConditionCombo.SelectedValue;
                            vm.CallStrategyVM.UpdateStrategy(true);
                        }
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        if (orderConditionCombo.SelectedValue != null)
                        {
                            vm.PutStrategyVM.ConditionType = (OrderConditionType)orderConditionCombo.SelectedValue;
                            vm.PutStrategyVM.UpdateStrategy(true);
                        }
                    }
                }
            }
        }
        private void orderConditionSelectionChanged(OrderConditionType ordertype)
        {
            if (CallPutTDOptionVMCollection != null)
            {
                foreach (var vm in CallPutTDOptionVMCollection)
                {
                    if (vm.CallStrategyVM != null)
                    {
                        if (orderConditionCombo.SelectedValue != null)
                        {
                            vm.CallStrategyVM.ConditionType = ordertype;
                            vm.CallStrategyVM.UpdateStrategy(true);
                        }
                    }
                    if (vm.PutStrategyVM != null)
                    {
                        if (orderConditionCombo.SelectedValue != null)
                        {
                            vm.PutStrategyVM.ConditionType = ordertype;
                            vm.PutStrategyVM.UpdateStrategy(true);
                        }

                    }
                }
            }
        }

    }
}
