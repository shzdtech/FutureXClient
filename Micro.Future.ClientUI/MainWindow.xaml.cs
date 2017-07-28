using System;
using System.Windows;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using System.Configuration;
using System.Collections.Generic;
using Micro.Future.CustomizedControls;
using Micro.Future.Resources.Localization;
using AutoUpdaterDotNET;
using System.Windows.Controls;
using Micro.Future.CustomizedControls.Windows;
using System.Linq;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IStatusCollector
    {
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());
        private LoginWindow _currentLoginWindow;

        public static int maketDataTabCount = 0;

        private bool _logged;


        public MainWindow()
        {
            InitializeComponent();

            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(Settings.Default.AutoUpdateAddress);
        }
        private async void MenuItem_RefreshContracts_Click(object sender, RoutedEventArgs e)
        {
            var tradeHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
            await tradeHandler.SyncContractInfoAsync(true);
            MessageBox.Show(Application.Current.MainWindow, "合约已刷新，请重新启动应用！");
        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                    AutoUpdater.Start(Settings.Default.AutoUpdateAddress);
                    Dispatcher.Invoke(() =>
                    {
                        Hide();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        Hide();
                        Title += " (" + args.CurrentVersion + ")";
                        Initialize();
                    });
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    Hide();
                    Title += " (" + MFUtilities.ClientVersion + ")";
                    Initialize();
                });
            }
        }

        private void _currentLoginWindow_Closed(object sender, EventArgs e)
        {
            if (!_logged)
            {
                MessageBoxResult dr = MessageBox.Show("是否退出", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    Login();
                }
            }
        }

        public void Initialize()
        {
            Login();
            Show();
        }


        private void Login()
        {
            _accountSignIner.SignInOptions.EncryptPassword = true;
            _currentLoginWindow = new LoginWindow(_accountSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["ACCOUNTSERVER.FRONT"].Values.ToList(),
                NameCollection = _config.Content["ACCOUNTSERVER.FRONTNAME"].Values.ToList()

            };
            _currentLoginWindow.Closed += _currentLoginWindow_Closed;
            _currentLoginWindow.OnLogged += LoginWindow_OnLogged;

            _currentLoginWindow.ShowDialog();
        }

        private async void LoginWindow_OnLogged(LoginWindow sender, IUserInfo userInfo)
        {
            MessageHandlerContainer.DefaultInstance.Get<AccountHandler>().RegisterMessageWrapper(_accountSignIner.MessageWrapper);

            var roleType = userInfo.Role.ToString();
            var frameDict = (Dictionary<string, IList<string>>)ConfigurationManager.GetSection("frames/roles");
            IList<string> frames;
            if (frameDict.TryGetValue(roleType, out frames))
            {
                mainPane.Children.Clear();
                mainMenu.Items.Clear();
                statusBar.Items.Clear();

                var statusBarIdSet = new HashSet<string>();

                sender.DataLoadingProgressBar.Maximum = frames.Count;
                foreach (var frame in frames)
                {
                    sender.DataLoadingProgressBar.Value++;

                    var frameUI = Activator.CreateInstance(Type.GetType(frame)) as IUserFrame;
                    if (frameUI != null)
                    {
                        frameUI.StatusReporter = this;
                        mainPane.AddContent(frameUI).Title = frameUI.Title;

                        ReportStatus("Loading " + frameUI.Title + " ...");

                        if (frameUI.FrameMenus != null)
                        {
                            foreach (var menuitem in frameUI.FrameMenus)
                                mainMenu.Items.Add(menuitem);
                        }

                        if (frameUI.StatusBarItems != null)
                        {
                            foreach (var statusbaritem in frameUI.StatusBarItems)
                            {
                                if (!string.IsNullOrEmpty(statusbaritem.Uid))
                                {
                                    if (statusBarIdSet.Contains(statusbaritem.Uid))
                                    {
                                        continue;
                                    }
                                    statusBarIdSet.Add(statusbaritem.Uid);
                                }
                                statusBar.Items.Add(statusbaritem);
                            }
                        }

                        var entries = _accountSignIner.SignInOptions.FrontServer.Split(':');

                        try
                        {
                            _logged = await frameUI.LoginAsync(_accountSignIner.SignInOptions.BrokerID, _accountSignIner.SignInOptions.UserName, _accountSignIner.SignInOptions.Password, entries[0]);
                            if (!_logged)
                                Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message);
                        }
                    }
                }
            }

            foreach (var menuitem in SysMenus)
                mainMenu.Items.Add(menuitem);

            sender.Close();
        }

        public void ReportStatus(string statusMsg)
        {
            _currentLoginWindow?.ReportStatus(statusMsg);
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_logged)
            {
                MessageBoxResult dr = MessageBox.Show("是否退出", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                }

                foreach (var pane in mainPane.Children)
                {
                    var frame = pane.Content as IUserFrame;
                    if (frame != null)
                    {
                        try
                        {
                            frame.OnClosing();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        public IEnumerable<MenuItem> SysMenus
        {
            get
            {
                return Resources["sysMenuItems"] as IEnumerable<MenuItem>;
            }
        }
        private void MenuItem_Click_ResetPassword(object sender, RoutedEventArgs e)
        {

            ResetPasswordWindow win = new ResetPasswordWindow(_accountSignIner.SignInOptions.Password) { MD5Round = 2 };

            win.ShowDialog();

        }
    }
}
