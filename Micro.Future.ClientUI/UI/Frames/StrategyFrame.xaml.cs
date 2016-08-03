using Micro.Future.CustomizedControls;
using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Resources.Localization;
using Micro.Future.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Micro.Future.UI
{
    /// <summary>
    /// StrategyFrame.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyFrame : UserControl, IUserFrame
    {
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _tdSignIner = new PBSignInManager();

        public StrategyFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
            }


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
                return Resources["exTDMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exTDStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public void LoginAsync(string usernname, string password)
        {
            _tdSignIner.SignInOptions.UserName = usernname;
            _tdSignIner.SignInOptions.Password = password;
        }

        public void Initialize()
        {

            // Initialize Market Data
            var msgWrapper = _tdSignIner.MessageWrapper;
            msgWrapper.MessageClient.OnDisconnected += TD_OnDisconnected;

            _tdSignIner.OnLoginError += OnErrorMessageRecv;
            _tdSignIner.OnLogged += TdLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += TdLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += TdLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<OTCMDTradingDeskHandler>().RegisterMessageWrapper(msgWrapper);
            MessageHandlerContainer.DefaultInstance.Get<OTCMDTradingDeskHandler>().OnError += OnErrorMessageRecv;

            TDServerLogin();
        }

        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(errRsult.Message, WPFUtility.GetLocalizedString("Error", LocalizationInfo.ResourceFile), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void TD_OnDisconnected(Exception ex)
        {
            MessageBox.Show("请点击状态栏中的连接按钮尝试重新连接", "TradingDesk服务器失去连接", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TDServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                var mdCfg = _config.Content["OTCTDSERVER"];
                _tdSignIner.SignInOptions.FrontServer = mdCfg["ADDRESS"];
                TdLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _tdSignIner.SignIn();
            }
        }
    }
}
