﻿using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Micro.Future.UI
{
    /// <summary>
    /// StrategyFrame.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyFrame : UserControl, IUserFrame
    {
        private AbstractSignInManager _otcSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AbstractOTCHandler>());



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

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server)
        {
            _otcSignIner.SignInOptions.BrokerID = brokerId;
            _otcSignIner.SignInOptions.UserName = usernname;
            _otcSignIner.SignInOptions.Password = password;

            var entries = _otcSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _otcSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            TDServerLogin();

            return LoginTaskSource.Task;
        }

        public void Initialize()
        {

            var handler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
            strategyListView.OTCHandler = handler;
            contractParamListView.OTCHandler = handler;

            // Initialize Market Data
            var msgWrapper = _otcSignIner.MessageWrapper;

            _otcSignIner.OnLogged += TdLoginStatus.OnLogged;
            _otcSignIner.OnLoginError += TdLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += TdLoginStatus.OnDisconnected;
            _otcSignIner.OnLogged += _otcSignIner_OnLogged;
            _otcSignIner.OnLoginError += _otcSignIner_OnLoginError;

            handler.RegisterMessageWrapper(msgWrapper); ;
        }

        private void _otcSignIner_OnLoginError(MessageException obj)
        {
            LoginTaskSource.TrySetException(obj);
        }

        private async void _otcSignIner_OnLogged(IUserInfo obj)
        {
            var handler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();
            strategyListView.ReloadData();
            contractParamListView.ReloadData();
            await MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().QueryPortfolioAsync();
            LoginTaskSource.TrySetResult(true);

            //var layoutInfo = ClientDbContext.GetLayout(handler.MessageWrapper.User.Id, strategyDM.Uid);
            //if (layoutInfo != null)
            //{
            //    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(strategyDM);

            //    using (var reader = new StringReader(layoutInfo.LayoutCFG))
            //    {
            //        layoutSerializer.Deserialize(reader);
            //    }
            //}
        }

        private void TDServerLogin()
        {
            if (!_otcSignIner.MessageWrapper.HasSignIn)
            {
                TdLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _otcSignIner.SignIn();
            }
        }

        private void TdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            TDServerLogin();
        }
        public void SaveLayout()
        {
            var handler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();

            var layoutInfo = ClientDbContext.GetLayout(handler.MessageWrapper.User?.Id, strategyDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(strategyDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            ClientDbContext.SaveLayoutInfo(handler.MessageWrapper.User.Id, strategyDM.Uid, strBuilder.ToString());
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
