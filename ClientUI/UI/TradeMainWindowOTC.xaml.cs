using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls.Ribbon;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using Micro.Future.ViewModel;
using System.ComponentModel;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.Windows.Input;
using Micro.Future.Message;
using Micro.Future.Message;
using Micro.Future.Message.PBMessageHandler;
using Micro.Future.Constant;
using Micro.Future.Util;
using Micro.Future.Properties;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TradeMainWindowOTC : RibbonWindow
    {
        public static TradeMainWindowOTC MyInstance;
        public Dictionary<string, QuoteGroup> GroupMap { get; set; }

        private string mUserLayoutConfig { get; set; }
        private string mDefaultConfig { get; set; }

        private List<InstrumentViewModel> mQuoteInfoList = new List<InstrumentViewModel>();
        public List<InstrumentViewModel> QuoteInfoList
        {
            get { return mQuoteInfoList; }
            set { mQuoteInfoList = value; }
        }

        public KeyboardOrderViewModel KeyOrderViewModel { get; set; }

        public bool NewGroupReady { get; set; }

        private string myArchiveFile;
        private ArchiveWrapper mArchieveWarpper;
        private List<string> mFrontIDs;
        private string mAdminUser = "admin";

        public TradeMainWindowOTC()
        {
            Init();
        }

        private void Init()
        {
            GroupMap = new Dictionary<string, QuoteGroup>();
            GroupMap.Add("能源", new QuoteGroup() { Name = "能源", Predefined = true });
            GroupMap.Add("市场指数", new QuoteGroup() { Name = "市场指数", Predefined = true });
            GroupMap.Add("外汇", new QuoteGroup() { Name = "外汇", Predefined = true });
            GroupMap.Add("工业材料", new QuoteGroup() { Name = "工业材料", Predefined = true });
            GroupMap.Add("利率", new QuoteGroup() { Name = "利率", Predefined = true });
            GroupMap.Add("金属", new QuoteGroup() { Name = "金属", Predefined = true });
            GroupMap.Add("谷物与油籽", new QuoteGroup() { Name = "谷物与油籽", Predefined = true });
            GroupMap.Add("软产品与家畜", new QuoteGroup() { Name = "软产品与家畜", Predefined = true });
            GroupMap.Add("贵金属", new QuoteGroup() { Name = "贵金属", Predefined = true });

            MyInstance = this;

            InitializeComponent();
        }

        public void Load()
        {
            try
            {
                MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                MessageHandlerContainer.DefaultInstance.Get<TradeHandler>();

                foreach (var msgwrp in MessageHandlerContainer.DefaultInstance)
                {
                    msgwrp.Value.MessageWrapper.OnUnsolvedErr += OnUnsolvedErr;
                }

                Config config = new Config(Settings.Default.ConfigFile);
                //Config configForLoop = new Config(@"WPF.GUI");
                //string configFile = config.Content["LOG"]["CONFIG"];
                //Logger.StartLog(configFile);

                //foreach (string key1 in configForLoop.Content.Keys)
                //{
                //    foreach (string key2 in configForLoop.Content[key1].Keys)
                //    {
                //        string newValue = config.Content[key1][key2].Replace("$SERVER$", server);
                //        config.Content[key1][key2] = newValue;
                //    }
                //}

                Logger.Debug("MainWindow starts");

                myArchiveFile = config.Content["GEN"]["archive"];
                mAdminUser = config.Content["GEN"]["admin"];

                mUserLayoutConfig = @"layoutuser.cfg";
                mDefaultConfig = @"layoutdefault.cfg";

                mFrontIDs = new List<string>();
                mFrontIDs = config.Content["FRONTID"].Values.ToList<string>();

                //OrderHandler.Connect();

                Deserialize();

                //UserTextStatus.Text = "当前用户：" + User;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "警告");
                Logger.Debug(ex.Message);
            }
        }

        void OnUnsolvedErr(object obj)
        {
            throw new NotImplementedException();
        }

        public void SaveUserLayout()
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new StreamWriter(mUserLayoutConfig))
                serializer.Serialize(stream);
            Logger.Debug("SaveUserLayout");
        }

        public void LoadUserLayout()
        {
            LoadLayout(mUserLayoutConfig);
        }

        private void LoadLayout(string config)
        {
            if (File.Exists(config))
            {
                var currentContentsList = dockingManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();
                var serializer = new XmlLayoutSerializer(dockingManager);
                using (var stream = new StreamReader(config))
                    serializer.Deserialize(stream);

                Logger.Debug("LoadLayout: " + config);
            }
            else
            {
                Logger.Warn("LoadLayout user config doesn't exist");
            }
        }

        public void LoadDefaultLayout()
        {
            LoadLayout(mDefaultConfig);
        }

        private void Serialize(ArchiveWrapper obj)
        {
            Type[] types = new Type[]{typeof(List<QuoteGroup>), typeof(KeyboardOrderViewModel), typeof (QuoteGroup), typeof(InstrumentViewModel)
                ,typeof(CloseChoiceType), typeof(List<ButtonSetting>), typeof(ButtonSetting), typeof(ActionChoiceType)};

            XmlSerializer mySerializer = new XmlSerializer(typeof(ArchiveWrapper), types);
            // To write to a file, create a StreamWriter object.
            using (FileStream myWriter = new FileStream(myArchiveFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                mySerializer.Serialize(myWriter, obj);
            }

            Logger.Debug("Serialize");
        }

        private void Deserialize()
        {
            if (!File.Exists(myArchiveFile))
            {
                List<ButtonSetting> buttonList = new List<ButtonSetting>();
                buttonList.Add(new ButtonSetting() { Num = 1, ActionChoice = (int)ActionChoiceType.Buy, BidOrAskePrice = 0 });
                buttonList.Add(new ButtonSetting() { Num = 2, ActionChoice = (int)ActionChoiceType.Buy, BidOrAskePrice = 1 });
                buttonList.Add(new ButtonSetting() { Num = 3, ActionChoice = (int)ActionChoiceType.Sell, BidOrAskePrice = 0 });
                buttonList.Add(new ButtonSetting() { Num = 4, ActionChoice = (int)ActionChoiceType.Sell, BidOrAskePrice = 1 });
                buttonList.Add(new ButtonSetting() { Num = 5, ActionChoice = (int)ActionChoiceType.AllCancel, BidOrAskePrice = 0 });
                KeyOrderViewModel = new KeyboardOrderViewModel()
                {
                    OneKeyOrderPermitted = false,
                    OpenOnlyOneDirectionOrder = false,
                    OpenOnlyOneOrder = false,
                    Buttons = buttonList,
                    CloseChoice = CloseChoiceType.All,
                    DisplayName = "KeyOrderViewModel"
                };

                return;
            }

            ArchiveWrapper myObject;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer mySerializer = new XmlSerializer(typeof(ArchiveWrapper));

            // To read the file, create a FileStream.
            using (FileStream myFileStream = new FileStream(myArchiveFile, FileMode.Open))
            {
                // Call the Deserialize method and cast to the object type.
                myObject = (ArchiveWrapper)mySerializer.Deserialize(myFileStream);
                foreach (QuoteGroup group in myObject.Groups)
                {
                    if (!group.Predefined)
                    {
                        GroupMap.Add(group.Name, group);

                        //(_dataCollection["产品种类"] as GalleryData<string>).CategoryDataCollection.Add(new GalleryCategoryData<string> { Label = doc.Title });
                    }
                }

                KeyOrderViewModel = myObject.KeySetting;
            }

            Logger.Debug("Deserialize");
        }

        private void NewColumn(string columnName, DataTable dt)
        {
            DataColumn column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = columnName;
            dt.Columns.Add(column);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            LoadUserLayout();
            LoadLogin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewQuoteWin win = new NewQuoteWin();
            win.Show();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            mArchieveWarpper = new ArchiveWrapper()
            {
                Groups = GroupMap.Values.ToList(),
                KeySetting = KeyOrderViewModel
            };
            Serialize(mArchieveWarpper);
            Application.Current.Shutdown();
        }

        private void newQuoteGroupRibbonText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string name = (sender as RibbonTextBox).Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                NewGroupReady = false;
            }
            else
            {
                if (MainWindow.MyInstance.GroupMap.ContainsKey(name))
                {
                    MessageBox.Show("报价组：" + name + " 重名！", "出错了");
                    NewGroupReady = false;
                }
                else
                {
                    NewGroupReady = true;
                }
            }
        }

        public void AddDefinedGroup(string name)
        {

        }

        public bool CanAddDefinedGroup(string name)
        {
            if ((dockingManager != null) && (!string.IsNullOrEmpty(name)))
            {
                var firstDocumentPane = dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (firstDocumentPane != null)
                {
                    foreach (LayoutDocument child in firstDocumentPane.Children)
                    {
                        if (child.Title == name)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            return true;
        }

        public void LoadLogin()
        {
            
        }

        public bool IsAdmin()
        {
            //if (User == mAdminUser)
            //{
            //    return true;
            //}

            return false;
        }

        public void PringToStatus(string message)
        {
            ProcessStatus.Text = message;
        }
        public void PringToStatusSemi(string message)
        {
            SemiStatusTextBlock.Text = message;
        }

        public void ChangeTheme(int index)
        {
            switch (index)
            {
                case 0:
                    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.GenericTheme();
                    break;
                case 1:
                    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.VS2010Theme();
                    break;
                case 2:
                    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.AeroTheme();
                    break;
                case 3:
                    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.MetroTheme();
                    break;
                //case 4:
                //    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.ExpressionLightTheme();
                //    break;
                //case 5:
                //    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.ExpressionDarkTheme();
                //    break;
                default:
                    dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.GenericTheme();
                    break;
            }

        }
    }
}
