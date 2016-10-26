using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _tdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionHandler>());
        // private AbstractSignInManager _ctpSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTPOptionDataHandler>());
        private AbstractOTCHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();

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

        public Task<bool> LoginAsync(string usernname, string password, string server)
        {
            _tdSignIner.SignInOptions.UserName = usernname;
            _tdSignIner.SignInOptions.Password = password;

            var entries = _tdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _tdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            //entries = _ctpSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            //if (server != null && entries.Length < 2)
            //    _ctpSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            TDServerLogin();

            return LoginTaskSource.Task;
        }

        public void Initialize()
        {
            // Initialize Market Data


            var msgWrapper = _tdSignIner.MessageWrapper;
            _tdSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += _tdSignIner_OnLoginError;
            _tdSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            _tdSignIner.OnLogged += _tdSignIner_OnLogged;

            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;
            _otcOptionHandler.RegisterMessageWrapper(msgWrapper);
            optionPane.AddContent(new OptionModelCtrl()).Title = "Model";
            optionPane.AddContent(new OpMarketMakerCtrl()).Title = "Market Maker";
            optionPane.AddContent(new OpHedgeCtrl()).Title = "Hedge";
        }

        private void _tdSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }

        private async void _tdSignIner_OnLogged(IUserInfo obj)
        {
            await _otcOptionHandler.QueryStrategyAsync();
            await _otcOptionHandler.QueryAllModelParamsAsync();

            LoginTaskSource.TrySetResult(true);
        }

        private void TDServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _tdSignIner.SignIn();
            }
        }


        public OptionFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
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
                optionPane.AddContent(optionModelCtrl).Title = volModelSettingsWin.VolModelTabTitle;
                _otcOptionHandler.NewWingModelInstance(volModelSettingsWin.VolModelTabTitle);
                var modelparamsVM = await _otcOptionHandler.QueryModelParamsAsync(volModelSettingsWin.VolModelTabTitle);
                optionModelCtrl.WMSettingsLV.DataContext = modelparamsVM;
            }
        }

        private void Add_Quote_Click(object sender, RoutedEventArgs e)
        {
            OpMarketMakerCtrl opMarketMakerCtrl = new OpMarketMakerCtrl();
            optionPane.AddContent(opMarketMakerCtrl).Title = "OptionQuote";
        }
    }
}

