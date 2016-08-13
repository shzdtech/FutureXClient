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
    public partial class MainWindow : Window
    {
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _accountSignIner = new PBSignInManager();

        public MainWindow()
        {
            InitializeComponent();
            Title += " (" + MFUtilities.ClientVersion + ")";
            Initialize();
        }

        public void Initialize()
        {
            _accountSignIner.OnLogged += OnLogged;
            Login();
        }


        void OnLogged(IUserInfo userInfo)
        {
            var roleType = userInfo.Role.ToString();
            var frameDict = (Dictionary<string, IList<string>>)ConfigurationManager.GetSection("frames/roles");
            IList<string> frames;
            if (frameDict.TryGetValue(roleType, out frames))
            {
                foreach (var frame in frames)
                {
                    var frameUI = Activator.CreateInstance(Type.GetType(frame)) as IUserFrame;
                    if (frameUI != null)
                    {
                        mainPane.AddContent(frameUI).Title = frameUI.Title;

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

                        frameUI.LoginAsync(_accountSignIner.SignInOptions.UserName, _accountSignIner.SignInOptions.Password);
                    }
                }
            }

           


        }

        private void Login()
        {
            LoginWindow loginWindow = new LoginWindow(_accountSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["ACCOUNTSERVER.ADDRESS"].Values
            };

            if (!loginWindow.ShowDialog().Value)
                Close();
        }
    }
}
