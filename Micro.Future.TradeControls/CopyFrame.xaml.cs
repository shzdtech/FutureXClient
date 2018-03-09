using System.Collections.Generic;
using System.Windows;
using Micro.Future.Message;
using System.ComponentModel;
using System.Windows.Controls;
using Micro.Future.CustomizedControls;
using System;
using System.Windows.Controls.Primitives;
using Micro.Future.Resources.Localization;
using Micro.Future.CustomizedControls.Windows;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.LocalStorage;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using Micro.Future.ViewModel;
using System.Linq;
using Xceed.Wpf.Toolkit;
using System.Windows.Input;
using System.Windows.Media;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class CopyFrame : UserControl, IUserFrame
    {
        private const string DEFAULT_ID = "D97F60E1-0433-4886-99E6-C4AD46A7D33A";

        private AbstractSignInManager _ctpMdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<MarketDataHandler>());
        private AbstractSignInManager _ctpTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<TraderExHandler>());
        private AbstractSignInManager _otcTradeSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradeHandler>());
        private AbstractSignInManager _otcTradingDeskSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private AbstractSignInManager _otcOptionDataSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionDataHandler>());
        private AbstractSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());

        private IDictionary<string, MessageHandlerContainer> _userMsgContainer = new Dictionary<string, MessageHandlerContainer>();

        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public string InstanceName
        {
            get; set;
        }


        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public MessageHandlerContainer GetUserMessageContainer(string userName)
        {
            MessageHandlerContainer msgContainer;
            if (!_userMsgContainer.TryGetValue(userName, out msgContainer))
            {
                msgContainer = new MessageHandlerContainer();
                _userMsgContainer[userName] = msgContainer;
                var ctpMdSignIner = new PBSignInManager(_ctpMdSignIner.SignInOptions);
                msgContainer.Get<MarketDataHandler>().RegisterMessageWrapper(ctpMdSignIner.MessageWrapper);
                var ctpTradeSignIner = new PBSignInManager(_ctpTradeSignIner.SignInOptions);
                msgContainer.Get<TraderExHandler>().RegisterMessageWrapper(ctpTradeSignIner.MessageWrapper);
                var otcTradeSignIner = new PBSignInManager(_otcTradeSignIner.SignInOptions);
                msgContainer.Get<OTCOptionTradeHandler>().RegisterMessageWrapper(otcTradeSignIner.MessageWrapper);
                var otcTradingDeskSignIner = new PBSignInManager(_otcTradingDeskSignIner.SignInOptions);
                msgContainer.Get<OTCOptionTradingDeskHandler>().RegisterMessageWrapper(otcTradingDeskSignIner.MessageWrapper);
                var otcOptionDataSignIner = new PBSignInManager(_otcOptionDataSignIner.SignInOptions);
                msgContainer.Get<OTCOptionDataHandler>().RegisterMessageWrapper(otcOptionDataSignIner.MessageWrapper);
                var accountSignIner = new PBSignInManager(_accountSignIner.SignInOptions);
                msgContainer.Get<AccountHandler>().RegisterMessageWrapper(otcOptionDataSignIner.MessageWrapper);

                ctpTradeSignIner.OnLogged += _ctpTradeSignIner_OnLogged;

                var taskList = new List<Task<TaskResult<IUserInfo, MessageException>>>();


                Task.Run(() =>
                {
                    taskList.Add(ServerLoginAsync(ctpMdSignIner));
                    taskList.Add(ServerLoginAsync(ctpTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradingDeskSignIner));
                    taskList.Add(ServerLoginAsync(otcOptionDataSignIner));
                    taskList.Add(ServerLoginAsync(accountSignIner));


                    Task.WaitAll(taskList.ToArray());

                    ctpMdSignIner.SignInOptions.UserName = userName;
                    ctpTradeSignIner.SignInOptions.UserName = userName;
                    otcTradeSignIner.SignInOptions.UserName = userName;
                    otcTradingDeskSignIner.SignInOptions.UserName = userName;
                    otcOptionDataSignIner.SignInOptions.UserName = userName;

                    taskList.Clear();
                    taskList.Add(ServerLoginAsync(ctpMdSignIner));
                    taskList.Add(ServerLoginAsync(ctpTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradeSignIner));
                    taskList.Add(ServerLoginAsync(otcTradingDeskSignIner));
                    taskList.Add(ServerLoginAsync(otcOptionDataSignIner));
                    taskList.Add(ServerLoginAsync(accountSignIner));

                    Task.WaitAll(taskList.ToArray());

                    Task.Run(() =>
                    {
                        var tradingdeskHandler = msgContainer.Get<OTCOptionTradingDeskHandler>();                        
                        var taskportfolio = tradingdeskHandler.QueryPortfolioAsync();
                        taskportfolio.Wait();
                        var task = tradingdeskHandler.QueryAllModelParamsAsync();
                        task.Wait();
                        ObservableCollection<ModelParamsVM> modelparamsVMCollection;
                        if (task.Result.TryGetValue("risk", out modelparamsVMCollection))
                            riskparamsControl.Dispatcher.Invoke(() => riskparamsControl.RiskParamNameListView.ItemsSource = modelparamsVMCollection);
                    });

                });
            }

            return msgContainer;
        }

        private void RiskparamsControl_OnModelSelected(ModelParamsVM obj)
        {
            InstanceName = obj.InstanceName;
            var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            riskparamsControl.RiskParamSP.Children.Clear();
            var task = tradingdeskHandler.QueryModelParamsDefAsync(obj.Model);
            task.Wait();
            var paramdef = task.Result;
            var riskparam = tradingdeskHandler.GetModelParamsVMCollection("risk");
            if (riskparam == null)
            {
                return;
            }
            var rparam = riskparam.FirstOrDefault(c => c.Model == paramdef.ModelName && c.InstanceName == obj.InstanceName);
            if (rparam == null)
            {
                return;
            }

            foreach (var def in paramdef.Params)
            {
                if (def.DataType == 2 || def.DataType == 1)
                {
                    if (def.Visible == true)
                    {
                        string msg = string.Format("F{0}", def.Digits);
                        Xceed.Wpf.Toolkit.DoubleUpDown a = new Xceed.Wpf.Toolkit.DoubleUpDown()
                        { Increment = def.Step, Minimum = def.MinVal, Maximum = def.MaxVal, FormatString = msg, IsEnabled = def.Enable };
                        a.UpdateValueOnEnterKey = true;
                        a.Spinned += A_Spinned;
                        a.ValueChanged += A_ValueChanged;
                        a.KeyUp += A_KeyUp;
                        //a.SetBinding(Xceed.Wpf.Toolkit.DoubleUpDown.ValueProperty, string.Format("[{0}].Value", def.Name));
                        a.Tag = def.Name;
                        var modelParam = rparam[def.Name];
                        if (modelParam != null)
                            a.Value = modelParam.Value;
                        riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    }
                }
                else if (def.DataType == 0)
                {
                    if (def.Visible == true)
                    {
                        Label a = new Label() { Content = string.Format("[{0}].Value", def.Name) };
                        riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    }
                }
                else if (def.Name == "__action__")
                {
                    ComboBox a = new ComboBox() { };
                    a.ItemsSource = Enum.GetValues(typeof(ParamActionType)).Cast<ParamActionType>().ToList();
                    a.SelectedValue = (ParamActionType)def.DefaultVal;
                    var modelParam = rparam[def.Name];
                    if (modelParam != null)
                        a.SelectedValue = (ParamActionType)modelParam.Value;
                    a.Tag = def.Name;
                    a.IsEnabled = def.Enable;
                    //a.SetBinding(ComboBox.SelectedValuePathProperty, string.Format("[{0}].Value", def.Name));
                    riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    a.SelectionChanged += A_SelectionChanged;
                }
                else if (def.Name == "__enabled__")
                {
                    ComboBox a = new ComboBox() { };
                    a.ItemsSource = Enum.GetValues(typeof(ParamEnableType)).Cast<ParamEnableType>().ToList();
                    a.SelectedValue = (ParamEnableType)def.DefaultVal;
                    var modelParam = rparam[def.Name];
                    if (modelParam != null)
                        a.SelectedValue = (ParamEnableType)modelParam.Value;
                    a.Tag = def.Name;
                    a.IsEnabled = def.Enable;
                    //a.SetBinding(ComboBox.SelectedValuePathProperty, string.Format("[{0}].Value", def.Name));
                    riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    a.SelectionChanged += A_SelectionChanged;
                }
                else if (def.Name == "__match__")
                {
                    ComboBox a = new ComboBox() { };
                    a.ItemsSource = Enum.GetValues(typeof(ParamMatchType)).Cast<ParamMatchType>().ToList();
                    a.SelectedValue = (ParamMatchType)def.DefaultVal;
                    var modelParam = rparam[def.Name];
                    if (modelParam != null)
                        a.SelectedValue = (ParamMatchType)modelParam.Value;
                    a.Tag = def.Name;
                    a.IsEnabled = def.Enable;
                    //a.SetBinding(ComboBox.SelectedValuePathProperty, string.Format("[{0}].Value", def.Name));
                    riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    a.SelectionChanged += A_SelectionChanged;
                }
                else if (def.Name == "__type__")
                {
                    ComboBox a = new ComboBox() { };
                    a.ItemsSource = Enum.GetValues(typeof(ParamRiskControlType)).Cast<ParamRiskControlType>().ToList();
                    a.SelectedValue = (ParamRiskControlType)def.DefaultVal;
                    var modelParam = rparam[def.Name];
                    if (modelParam != null)
                        a.SelectedValue = (ParamRiskControlType)modelParam.Value;
                    a.Tag = def.Name;
                    a.IsEnabled = def.Enable;
                    //a.SetBinding(ComboBox.SelectedValuePathProperty, string.Format("[{0}].Value", def.Name));
                    riskparamsControl.RiskParamSP.Children.Add(new GroupBox() { Content = a, Header = def.Name });
                    a.SelectionChanged += A_SelectionChanged;
                }
            }
            var riskparams = tradingdeskHandler.GetModelParamsVMCollection("risk");
            if (riskparams != null)
            {
                riskparamsControl.RiskParamSP.DataContext = riskparams.FirstOrDefault(c => c.Model == paramdef.ModelName);
            }
        }

        private void A_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Enter || e.Key == Key.Down || e.Key == Key.Up)
                {
                    ctrl.Background = Brushes.White;
                }
                else
                {
                    ctrl.Background = Brushes.MistyRose;
                }
            }
        }

        private void A_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            var value = combobox.SelectedIndex;
            var key = combobox.Tag.ToString();
            if (!string.IsNullOrEmpty(key))
            {
                var _handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                _handler.UpdateModelParams(InstanceName, key, value);
            }
        }

        private void A_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var modelParamsVM = updownctrl.DataContext as ModelParamsVM;
                if (modelParamsVM != null)
                {
                    var key = updownctrl.Tag.ToString();
                    double value = (double)e.NewValue;
                    var _handler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                    _handler.UpdateModelParams(modelParamsVM.InstanceName, key, value);
                }
            }
        }

        private void A_Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public CopyFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
                LoginTaskSource.TrySetResult(true);
                riskparamsControl.OnModelSelected += RiskparamsControl_OnModelSelected;
            }
        }

        public void Initialize()
        {
            // Initailize UI events
            clientFundLV.OnAccountSelected += OnAccountSelected;
            clientFundLV.OnClickLogin += OnClickLogin;
            tradeWindow.AnchorablePane = tradePane;
            positionsWindow.AnchorablePane = positionPane;
        }
        public void OnClickLogin()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            var otctradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
            FrameLoginWindow win = new FrameLoginWindow(tradeHandler.MessageWrapper.SignInManager, otctradeHandler.MessageWrapper.SignInManager);
            win.userTxt.Clear();
            win.passwordTxt.Clear();
            win.OnLogged += Win_OnLogged;
            win.ShowDialog();
        }

        private void Win_OnLogged(FrameLoginWindow sender, IUserInfo userInfo)
        {
            //var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            //FastOrderCtl.TradeHandler = tradeHandler;
            //FastOrderCtl.ReloadData();
            try
            {
                sender.Dispatcher.Invoke(() => sender.Close());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void OnAccountSelected(TradingDeskVM tradingdeskVM)
        {
            riskparamsControl.RiskParamNameListView.ItemsSource = null;
            riskparamsControl.RiskParamSP.Children.Clear();
            if (tradingdeskVM.UserName != null)
            {
                var ret = GetUserMessageContainer(tradingdeskVM.UserName);
                MessageHandlerContainer.DefaultInstance = ret;
                var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
                var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                if(!tradingdeskHandler.PortfolioVMCollection.Any())
                {
                    var taskportfolio = tradingdeskHandler.QueryPortfolioAsync();
                    taskportfolio.Wait();
                }
                tradeWindow.TradeHandler = tradeHandler;
                positionsWindow.TradeHandler = tradeHandler;
                positionsWindow.MarketDataHandler = marketdataHandler;
                FastOrderCtl.TradeHandler = tradeHandler;
                FastOrderCtl.MarketDataHandler = marketdataHandler;
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_FUTURE);
                FastOrderCtl.ProductTypeList.Add(ProductType.PRODUCT_OPTIONS);
                positionsWindow.DEFAULT_ID = DEFAULT_ID;
                tradeWindow.DEFAULT_ID = DEFAULT_ID;

                ObservableCollection<ModelParamsVM> modelparamsVMCollection;
                if (tradingdeskHandler.ModelParamsDict.TryGetValue("risk", out modelparamsVMCollection))
                    riskparamsControl.RiskParamNameListView.ItemsSource = modelparamsVMCollection;
                if (_ctpTradeSignIner.MessageWrapper.HasSignIn)
                    controlReload();
                //positionsWindow.ReloadData();
                //tradeWindow.ReloadData();
                //riskparamsControl.RiskParamSP.Children.Add(new Xceed.Wpf.Toolkit.DoubleUpDown());
            }
        }
        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            _ctpTradeSignInerOnLogged();
        }
        private void _ctpTradeSignInerOnLogged()
        {
            controlReload();
        }

        private void controlReload()
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            tradeHandler.SyncContractInfoAsync().Wait();
            Thread.Sleep(1000);
            positionsWindow.DEFAULT_ID = DEFAULT_ID;
            positionsWindow.Dispatcher.Invoke(() => positionsWindow.ReloadData());
            Thread.Sleep(1000);
            tradeWindow.DEFAULT_ID = DEFAULT_ID;
            tradeWindow.Dispatcher.Invoke(() => tradeWindow.ReloadData());
            Thread.Sleep(1000);
            FastOrderCtl.Dispatcher.Invoke(() => FastOrderCtl.ReloadData());
        }
        private void _otcTradingDeskSignIner_OnLogged(IUserInfo obj)
        {
            _otcTradingDeskSignInerOnLogged();
        }
        private async void _otcTradingDeskSignInerOnLogged()
        {
            var tradingdeskHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
            await tradingdeskHandler.QueryTradingDeskAsync();
            //clientFundLV.TradingDeskVMCollection = tradingdeskHandler.TradingDeskVMCollection;
            clientFundLV.FundListView.ItemsSource = tradingdeskHandler.TradingDeskVMCollection;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        public void OnClosing()
        {

        }
        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _otcTradingDeskSignIner.OnLogged += _otcTradingDeskSignIner_OnLogged;

            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().RegisterMessageWrapper(_ctpMdSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().RegisterMessageWrapper(_ctpTradeSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>().RegisterMessageWrapper(_otcTradingDeskSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>().RegisterMessageWrapper(_otcOptionDataSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>().RegisterMessageWrapper(_otcTradeSignIner.MessageWrapper);
            MessageHandlerContainer.DefaultInstance.Get<AccountHandler>().RegisterMessageWrapper(_accountSignIner.MessageWrapper);

            var entries = _ctpMdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            _accountSignIner.SignInOptions.BrokerID = brokerId;
            _accountSignIner.SignInOptions.UserName = usernname;
            _accountSignIner.SignInOptions.Password = password;
            if (server != null && entries.Length < 2)
                _accountSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _ctpMdSignIner.SignInOptions.BrokerID = brokerId;
            _ctpMdSignIner.SignInOptions.UserName = usernname;
            _ctpMdSignIner.SignInOptions.Password = password;
            if (server != null && entries.Length < 2)
                _ctpMdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _ctpTradeSignIner.SignInOptions.BrokerID = brokerId;
            _ctpTradeSignIner.SignInOptions.UserName = usernname;
            _ctpTradeSignIner.SignInOptions.Password = password;
            entries = _ctpTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            //TradingDesk登录
            _otcTradingDeskSignIner.SignInOptions.BrokerID = brokerId;
            _otcTradingDeskSignIner.SignInOptions.UserName = usernname;
            _otcTradingDeskSignIner.SignInOptions.Password = password;
            entries = _otcTradingDeskSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcTradingDeskSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _otcOptionDataSignIner.SignInOptions.BrokerID = brokerId;
            _otcOptionDataSignIner.SignInOptions.UserName = usernname;
            _otcOptionDataSignIner.SignInOptions.Password = password;
            entries = _otcOptionDataSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcOptionDataSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            _otcTradeSignIner.SignInOptions.BrokerID = brokerId;
            _otcTradeSignIner.SignInOptions.UserName = usernname;
            _otcTradeSignIner.SignInOptions.Password = password;
            entries = _otcTradeSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcTradeSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            TradingDeskServerLogin();
            MarketDataServerLogin();
            OTCOptionDataServerLogin();
            TradingServerLogin();
            OTCTradeServerLogin();
            AccountServerLogin();
            return LoginTaskSource.Task;
        }

        private Task<TaskResult<IUserInfo, MessageException>> ServerLoginAsync(AbstractSignInManager signiner)
        {
            return signiner.SignInAsync();
        }

        private void AccountServerLogin()
        {
            if (!_accountSignIner.MessageWrapper.HasSignIn)
            {
                _accountSignIner.SignIn();
            }
        }
        private void OTCOptionDataServerLogin()
        {
            if (!_otcOptionDataSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionMarketLoginStatus.Prompt = "连场外期权行情中";
                _otcOptionDataSignIner.SignIn();
            }
        }
        private void OTCOptionTradeServerLogin()
        {
            if (!_otcTradeSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradeLoginStatus.Prompt = "连场外期权交易中";
                _otcTradeSignIner.SignIn();
            }
        }
        private void MarketDataServerLogin()
        {
            if (!_ctpMdSignIner.MessageWrapper.HasSignIn)
            {
                //ctpLoginStatus.Prompt = "连CTP期权行情中";
                _ctpMdSignIner.SignIn();
            }
        }
        private void TradingDeskServerLogin()
        {
            if (!_otcTradingDeskSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradingDeskStatus.Prompt = "连期权TD中.";
                _otcTradingDeskSignIner.SignIn();
            }
        }
        private void TradingServerLogin()
        {
            if (!_ctpTradeSignIner.MessageWrapper.HasSignIn)
            {
                //ctpTradeLoginStatus.Prompt = "连CTP期权交易中";
                _ctpTradeSignIner.SignIn();
            }
        }
        private void OTCTradeServerLogin()
        {
            if (!_otcTradeSignIner.MessageWrapper.HasSignIn)
            {
                //otcOptionTradeLoginStatus.Prompt = "连OTC期权交易中";
                _otcTradeSignIner.SignIn();
            }
        }
    }
}
