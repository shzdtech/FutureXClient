using System;
using System.Windows;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using System.Configuration;
using System.Collections.Generic;
using Micro.Future.CustomizedControls;
using Micro.Future.Resources.Localization;

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


        public MainWindow()
        {
            InitializeComponent();
            Title += " (" + MFUtilities.ClientVersion + ")";
            Initialize();
        }

        public void Initialize()
        {
            Login();
        }


        private void Login()
        {
            _currentLoginWindow = new LoginWindow(_accountSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["ACCOUNTSERVER.ADDRESS"].Values
            };

            _currentLoginWindow.OnLogged += LoginWindow_OnLogged;

            _currentLoginWindow.ShowDialog();
        }

        private async void LoginWindow_OnLogged(LoginWindow sender, IUserInfo userInfo)
        {
            var roleType = userInfo.Role.ToString();
            var frameDict = (Dictionary<string, IList<string>>)ConfigurationManager.GetSection("frames/roles");
            IList<string> frames;
            if (frameDict.TryGetValue(roleType, out frames))
            {
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
                                statusBar.Items.Add(statusbaritem);
                        }

                        var entries = _accountSignIner.SignInOptions.FrontServer.Split(':');

                        try
                        {
                            await frameUI.LoginAsync(_accountSignIner.SignInOptions.UserName, _accountSignIner.SignInOptions.Password, entries[0]);
                        }
                        catch (AggregateException ex)
                        {
                            MessageBox.Show(this, ex.Message);
                        }
                    }
                }
            }

            sender.Close();
        }

        public void ReportStatus(string statusMsg)
        {
            _currentLoginWindow?.ReportStatus(statusMsg);
        }
    }
}
