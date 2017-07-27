using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.UI.OptionControls;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _otcOptionSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();

        //private List<PBSignInManager> _signIns = new List<PBSignInManager>();
        //private List<AbstractOTCHandler> _otcHdls = new List<AbstractOTCHandler>();
        //private int _cnt = 0;

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }


        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exOptionMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exOptionStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _otcOptionSignIner.SignInOptions.BrokerID = brokerId;
            _otcOptionSignIner.SignInOptions.UserName = usernname;
            _otcOptionSignIner.SignInOptions.Password = password;

            var entries = _otcOptionSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcOptionSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            //entries = _ctpSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            //if (server != null && entries.Length < 2)
            //    _ctpSignIner.SignInOptions.FrontServer = server + ':' + entries[0];
            //Test(_tdSignIner.SignInOptions);

            TDServerLogin();

            return LoginTaskSource.Task;
        }

        //public void Test(SignInOptions signInOpt)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        PBSignInManager tradeSignIner = new PBSignInManager(signInOpt);
        //        tradeSignIner.OnLogged += Test_OnLogged;
        //        _signIns.Add(tradeSignIner);
        //        tradeSignIner.SignIn();
        //        var hdl = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
        //        hdl.RegisterMessageWrapper(tradeSignIner.MessageWrapper);
        //        _otcHdls.Add(hdl);
        //    }
        //}

        //private void Test_OnLogged(IUserInfo userInfo)
        //{
        //    Console.WriteLine("Test Logged: " + userInfo.Name);

        //    if (++_cnt == 50)
        //    {
        //        foreach (var hdl in _otcHdls)
        //            hdl.QueryStrategy();
        //    }
        //}

        public void Initialize()
        {
            // Initialize Market Data


            var msgWrapper = _otcOptionSignIner.MessageWrapper;
            _otcOptionSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _otcOptionSignIner.OnLoginError += _tdSignIner_OnLoginError;
            _otcOptionSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            _otcOptionSignIner.OnLogged += _tdSignIner_OnLogged;

            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;
            _otcOptionHandler.RegisterMessageWrapper(msgWrapper);
            //optionPane.AddContent(new OptionModelCtrl()).Title = "Model";
            //optionPane.AddContent(new OpMarketMakerCtrl()).Title = "Market Maker";
            //optionPane.AddContent(new OpHedgeCtrl()).Title = "Hedge";
        }

        private void _tdSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }

        private async void _tdSignIner_OnLogged(IUserInfo obj)
        {
            _otcOptionTradeHandler.RegisterMessageWrapper(_otcOptionHandler.MessageWrapper);

            await _otcOptionHandler.QueryStrategyAsync();
            await _otcOptionHandler.QueryAllModelParamsAsync();
            await _otcOptionTradeHandler.SyncContractInfoAsync();
            optionModelCtrl.ReloadData();

            LoginTaskSource.TrySetResult(true);

            //var layoutInfo = ClientDbContext.GetLayout(_otcOptionTradeHandler.MessageWrapper.User.Id, optionDM.Uid);
            //if (layoutInfo != null)
            //{
            //    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionDM);

            //    using (var reader = new StringReader(layoutInfo.LayoutCFG))
            //    {
            //        layoutSerializer.Deserialize(reader);
            //    }
            //}
        }

        private void TDServerLogin()
        {
            if (!_otcOptionSignIner.MessageWrapper.HasSignIn)
            {
                OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _otcOptionSignIner.SignIn();
            }
            else
            {
                _tdSignIner_OnLogged(_otcOptionSignIner.LoggedUser);
            }
        }


        public OptionFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
                optionModelCtrl.OpMarketControl.volModelCB1.SelectionChanged += VolModelCB1_SelectionChanged;
            }
        }
        private void VolModelCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var volModel = optionModelCtrl.OpMarketControl.volModelCB1.SelectedItem as ModelParamsVM;
            if (volModel != null)
            {
                if( OpMarketMakerLV.expireDateCB.SelectedValue !=null )
                OpMarketMakerLV.volModelLB.Content = volModel;
            }
        }
        private void OptionWin_KeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {

                    OptionVM optionVM = ctrl.DataContext as OptionVM;
                    if (optionVM != null)
                    {
                        if (e.Key == Key.Enter)
                            optionVM.UpdateOptionParam();
                        else
                        {
                            ctrl.DataContext = null;
                            ctrl.DataContext = optionVM;
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

        private void OptionLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TDServerLogin();
        }

        private async void Add_Model_Click(object sender, RoutedEventArgs e)
        {
            VolModelSettingsWindow volModelSettingsWin = new VolModelSettingsWindow();
            if (volModelSettingsWin.ShowDialog().Value)
            {
                OptionModelCtrl optionModelCtrl = new OptionModelCtrl();
                //optionPane.AddContent(optionModelCtrl).Title = volModelSettingsWin.VolModelTabTitle;
                _otcOptionHandler.NewWingModelInstance(volModelSettingsWin.VolModelTabTitle);
                var modelparamsVM = await _otcOptionHandler.QueryModelParamsAsync(volModelSettingsWin.VolModelTabTitle);
                optionModelCtrl.WMSettingsLV.DataContext = modelparamsVM;
            }
        }

        private void Add_Quote_Click(object sender, RoutedEventArgs e)
        {
            OpMarketMakerCtrl opMarketMakerCtrl = new OpMarketMakerCtrl();
            //optionPane.AddContent(opMarketMakerCtrl).Title = "OptionQuote";
        }

        private void Add_Order_Click(object sender, RoutedEventArgs e)
        {
            OpOrderWin win = new OpOrderWin();
            win.Show();
        }

        private void Add_VolModel_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("Model", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var modelWin = new OptionModelCtrl();
            optionModelPane.AddContent(modelWin).Title = title;
        }

        private void Add_MarketMaker_Click(object sender, RoutedEventArgs e)
        {
            var title = WPFUtility.GetLocalizedString("MarketMaker", LocalizationInfo.ResourceFile, LocalizationInfo.AssemblyName);
            var marketmakerWin = new OpMarketMakerCtrl();
            optionModelPane.AddContent(marketmakerWin).Title = title;
        }

        private void OpMarketMaker_Closed(object sender, EventArgs e)
        {
            OpMarketMakerLV.AutoOrderUpdate(false);

        }

        public void SaveLayout()
        {

            var layoutInfo = ClientDbContext.GetLayout(_otcOptionTradeHandler.MessageWrapper.User?.Id, optionDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            ClientDbContext.SaveLayoutInfo(_otcOptionTradeHandler.MessageWrapper.User.Id, optionDM.Uid, strBuilder.ToString());
            optionModelCtrl.SaveLayout();
        }

        public void OnClosing()
        {
            SaveLayout();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }
    }
}

