﻿using Micro.Future.CustomizedControls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.ViewModel;
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
        private AbstractSignInManager _ctpSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTPOptionDataHandler>());
        private CTPOptionDataHandler _ctpOptionHandler = MessageHandlerContainer.DefaultInstance.Get<CTPOptionDataHandler>();

        private CollectionViewSource _viewSourcePosition = new CollectionViewSource();
        private CollectionViewSource _viewSourceRisk = new CollectionViewSource();
        private CollectionViewSource _viewSourcePutOption = new CollectionViewSource();
        private CollectionViewSource _viewSourceCallOption = new CollectionViewSource();
        private CollectionViewSource _viewSourceVolatility = new CollectionViewSource();

        private IList<ContractInfo> _contractList;

        private ColumnObject[] _optionColumns;

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


        public void LoginAsync(string usernname, string password, string server)
        {
            _tdSignIner.SignInOptions.UserName = _ctpSignIner.SignInOptions.UserName = usernname;
            _tdSignIner.SignInOptions.Password = _ctpSignIner.SignInOptions.Password = password;

            var entries = _tdSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _tdSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            entries = _ctpSignIner.SignInOptions.FrontServer.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (server != null && entries.Length < 2)
                _ctpSignIner.SignInOptions.FrontServer = server + ':' + entries[0];

            TDServerLogin();
            MDServerLogin();
        }

        public void Initialize()
        {
            using (var clientCache = new ClientDbContext())
            {
                _contractList = clientCache.ContractInfo.Where(c => c.ProductType == 1).ToList();
            }

            underlyingCB.ItemsSource = _contractList.Select(c => c.ProductID).Distinct();
            // Initialize Market Data
            var msgWrapper = _ctpSignIner.MessageWrapper;
            _ctpSignIner.OnLogged += OptionMdLoginStatus.OnLogged;
            _ctpSignIner.OnLoginError += OptionMdLoginStatus.OnDisconnected;
            _ctpSignIner.OnLogged += _ctpSignIner_OnLogged;
            msgWrapper.MessageClient.OnDisconnected += OptionMdLoginStatus.OnDisconnected;
            _ctpOptionHandler.RegisterMessageWrapper(msgWrapper);


            msgWrapper = _tdSignIner.MessageWrapper;
            _tdSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;
            MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>().RegisterMessageWrapper(msgWrapper);

            var traderExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            _viewSourcePosition.Source = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().RiskVMCollection;
            _viewSourceRisk.Source = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().PositionVMCollection;
            option_priceLV.ItemsSource = _ctpOptionHandler.CallPutOptionVMCollection;
            _viewSourceVolatility.Source = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().VolatilityVMCollection;
            positionLV.ItemsSource = _viewSourcePosition.View;
            riskLV.ItemsSource = _viewSourceRisk.View;
            //volatilityLV.ItemsSource = _viewSourceVolatility.View;
            traderExHandler.OnUpdateOption();
            traderExHandler.OnUpdateTest();
            StrikePricePanel.DataContext = new NumericalSimVM();
            //VolatilityPanel.DataContext = new OptionVM();
            //VolatilityPanel1.DataContext = new OptionVM();
            PlotVolatility.Model = traderExHandler.OptionOxyVM.PlotModel;
            VegaPosition.Model = traderExHandler.OptionOxyVM.PlotModelBar;

            _optionColumns = ColumnObject.GetColumns(option_priceLV);

        }

        private void _ctpSignIner_OnLogged(IUserInfo obj)
        {
            UpdateOption();
        }

        private void TDServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _tdSignIner.SignIn();
            }
        }

        private void MDServerLogin()
        {
            if (!_ctpSignIner.MessageWrapper.HasSignIn)
            {
                OptionMdLoginStatus.Prompt = "正在连接期权行情服务器...";
                _ctpSignIner.SignIn();
            }
        }

        public OptionVM OptionVM
        {
            get;
            private set;
        } = new OptionVM();

        public OptionOxyVM OptionOxyVM
        {
            get;
        } = new OptionOxyVM();



        public OptionFrame()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
                Initialize();
            }
        }

        public IEnumerable ExpirationMonthCollection
        {
            set
            {
                contractExpirationMonth.ItemsSource = value;
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

        private void MenuItem_Click_OptionColumns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(_optionColumns);
            win.Show();
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

        private void UpdateOption()
        {
            if (underlyingContractCB.SelectedItem != null && _ctpOptionHandler.MessageWrapper != null)
            {
                var uc = underlyingContractCB.SelectedItem.ToString();

                var optionList = (from c in _contractList
                                  where c.UnderlyingContract == uc
                                  select c).ToList();

                var strikeList = (from o in optionList
                                  orderby o.StrikePrice
                                  select o.StrikePrice).Distinct().ToList();

                var callList = (from o in optionList
                                where o.ContractType == 2
                                orderby o.StrikePrice
                                select o.Contract).Distinct().ToList();

                var putList = (from o in optionList
                               where o.ContractType == 3
                               orderby o.StrikePrice
                               select o.Contract).Distinct().ToList();

                var oldList = (from o in _ctpOptionHandler.CallPutOptionVMCollection
                               select o.CallOptionVM.Contract).ToList();
                _ctpOptionHandler.UnsubMarketData(oldList);

                oldList = (from o in _ctpOptionHandler.CallPutOptionVMCollection
                           select o.PutOptionVM.Contract).ToList();
                _ctpOptionHandler.UnsubMarketData(oldList);

                _ctpOptionHandler.CallPutOptionVMCollection.Clear();
                _ctpOptionHandler.SubCallPutOptionData(strikeList, callList, putList);
            }
        }


        private void underlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOption();
        }

        private void OptionMdLoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            MDServerLogin();
        }
    }
}

