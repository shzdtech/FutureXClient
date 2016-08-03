using System;
using System.Windows;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using System.Configuration;
using System.Collections.Generic;
using Micro.Future.CustomizedControls;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainOTCWindow : Window
    {
        private const string CST_CONTROL_ASSEMBLY = "Micro.Future.Resources.Localization";
        private const string RESOURCE_FILE = "Resources";
        private Config _config = new Config(Settings.Default.ConfigFile);
        private PBSignInManager _accountSignIner = new PBSignInManager();

        //Mark of initial window

        public MainOTCWindow()
        {
            InitializeComponent();
            Title += " (" + MFUtilities.ClientVersion + ")";
            Initialize();
        }

        public void Initialize()
        {

            _accountSignIner.OnLoginError += OnErrorMessageRecv;
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

        private void OnErrorMessageRecv(MessageException errRsult)
        {
            MessageBox.Show(this, errRsult.Message, WPFUtility.GetLocalizedString("Error", RESOURCE_FILE), MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private bool Login()
        {
            LoginWindow loginWindow = new LoginWindow(_accountSignIner)
            {
                MD5Round = 2,
                AddressCollection = _config.Content["ACCOUNTSERVER.ADDRESS"].Values
            };

            if (!loginWindow.ShowDialog().Value)
                Close();

            return true;
        }
    }
}
