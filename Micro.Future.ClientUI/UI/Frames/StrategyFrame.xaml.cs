﻿using Micro.Future.CustomizedControls;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
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
        private AbstractSignInManager _tdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCMDTradingDeskHandler>());

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

        private void TDServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                TdLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _tdSignIner.SignIn();
            }
        }

        private void TdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TDServerLogin();
        }
    }
}
