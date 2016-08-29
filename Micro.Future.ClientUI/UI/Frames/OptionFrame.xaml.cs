using Micro.Future.CustomizedControls;
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
        private AbstractSignInManager _tdSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<OTCOptionTradingDeskHandler>());
        private AbstractSignInManager _ctpSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<CTPOptionDataHandler>());

        private IList<ColumnObject> _optionColumns;

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
            _ctpSignIner.SignIn();
        }

        public void Initialize()
        {



            var msgWrapper = _tdSignIner.MessageWrapper;

            _tdSignIner.OnLogged += OptionLoginStatus.OnLogged;
            _tdSignIner.OnLoginError += OptionLoginStatus.OnDisconnected;
            msgWrapper.MessageClient.OnDisconnected += OptionLoginStatus.OnDisconnected;

            MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>().RegisterMessageWrapper(msgWrapper);

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

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

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

        private void MenuItem_Click_OptionColumns(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(_optionColumns);
            win.Show();
        }

    }
}

