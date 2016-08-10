﻿using Micro.Future.CustomizedControls;
using Micro.Future.Message;
using Micro.Future.Resources.Localization;
using Micro.Future.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        private AbstractSignInManager _tdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCMDTradingDeskHandler>());

        private CollectionViewSource _viewSource = new CollectionViewSource();
        private ColumnObject[] mColumns;

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


        public void LoginAsync(string usernname, string password)
        {
            _tdSignIner.SignInOptions.UserName = usernname;
            _tdSignIner.SignInOptions.Password = password;
        }

        public void Initialize()
        {

            // Initialize Market Data
            var msgWrapper = _tdSignIner.MessageWrapper;

            _tdSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<OTCMDTradingDeskHandler>().RegisterMessageWrapper(msgWrapper);

            TDServerLogin();
        }

        private void TDServerLogin()
        {
            if (!_tdSignIner.MessageWrapper.HasSignIn)
            {
                OptionLoginStatus.Prompt = "正在连接TradingDesk服务器...";
                _tdSignIner.SignIn();
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

            var traderExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            traderExHandler.OnUpdateOption();

            StrikePricePanel.DataContext = new NumericalSimVM();
            //VolatilityPanel.DataContext = new OptionVM();
            //VolatilityPanel1.DataContext = new OptionVM();
            price_greekLV.DataContext = new PriceGreekVM();
            volatilityLV.DataContext = new VolatilityVM();
            positionLV.DataContext = new PositionVM();
            riskLV.DataContext = new RiskVM();
            PlotVolatility.Model = traderExHandler.OptionOxyVM.PlotModel;
            VegaPosition.Model = traderExHandler.OptionOxyVM.PlotModelBar;

            mColumns = ColumnObject.GetColumns(listView_numSim);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        public IEnumerable ExpirationMonthCollection
        {
            set
            {
                contractExpirationMonth.ItemsSource = value;
            }
        }



        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void miniSteps_KeyUp(object sender, KeyEventArgs e)
        {

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
    }
}
