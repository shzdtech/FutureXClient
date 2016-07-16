using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Threading;
using System.Windows.Controls.Ribbon;
using System.Xml.Serialization;
using System.IO;
using Micro.Future.ViewModel;
using System.ComponentModel;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.Windows.Input;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;

namespace Micro.Future.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public static MainWindow MyInstance;
        public Dictionary<string, QuoteGroup> GroupMap { get; set; }

        private string mUserLayoutConfig { get; set; }
        private string mDefaultConfig { get; set; }

        public KeyboardOrderViewModel KeyOrderViewModel { get; set; }

        public bool NewGroupReady { get; set; }

        private string myArchiveFile;
        private ArchiveWrapper mArchieveWarpper;
        private ICollection<string> _mdServers;
        private ICollection<string> _trdServers;

        private string mAdminUser = "admin";

        public MainWindow()
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
            //this.Title += " (" + Utility.Utility.getClientVersion() + ")";

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

                _mdServers = config.Content["MARKETDATASERVER"].Values;
                _trdServers = config.Content["TRADESERVER"].Values;

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

        private void Sync_Tick(object sender, EventArgs e)
        {
            TradeHandler.Instance.SyncFromServer();
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
            LoadDomLogin();
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
            if (TradeHandler.Instance != null)
            {
                //OrderHandler.LogOut();
            }
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
                if (GroupMap.ContainsKey(name))
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
            var firstDocumentPane = dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                bool found = false;
                foreach (LayoutDocument child in firstDocumentPane.Children)
                {
                    if (child.Title == name)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    LayoutDocument doc = new LayoutDocument();
                    doc.Title = name;
                    firstDocumentPane.Children.Add(doc);
                    QuoteGroupView view = new QuoteGroupView();
                    doc.Content = view;

                    foreach (InstrumentViewModel instrument in GroupMap[name].Quotes)
                    {
                        if (!MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().WatchList.Contains(instrument.InstrumentID))
                        {
                            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().WatchList.Add(instrument.InstrumentID);
                            Logger.Debug("subscribed: " + instrument.InstrumentID);

                            QuoteViewModel quote = new QuoteViewModel() { Contract = instrument.InstrumentID };
                            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection.Add(quote);
                        }

                        //get real quote
                        var query1 = from row in MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection
                                     where row.Contract == instrument.InstrumentID select row;
                        if (query1.Count() > 0)
                        {
                            view.Quotes.Add(query1.ElementAt(0));
                        }
                    }

                    MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().SubMarketData();

                    firstDocumentPane.SelectedContentIndex = firstDocumentPane.Children.Count - 1;
                }
            }
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

        public void LoadDomLogin()
        {
            DomesticLogin win = new DomesticLogin(_mdServers, _trdServers);
            win.ShowDialog();
            if (win.IsSucceeded)
            {
                PringToStatusSemi("内盘登录成功:" + win.User.BrokerID + ":" + win.User.UserID);
                CreateHandlers(win.User);
            }
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

        #region Keyboard event
        public void StartListening()
        {
            KeyDown += RibbonWindow_KeyUp;
        }

        public void StopListening()
        {
            KeyDown -= RibbonWindow_KeyUp;
        }

        private void RibbonWindow_KeyUp(object sender, KeyEventArgs e)
        {
            int keyNum = (int)e.Key;
            if ((keyNum >= 74) && (keyNum <= 83))
            {
                keyNum -= 74;

                foreach (ButtonSetting button in KeyOrderViewModel.Buttons)
                {
                    if (button.Num == keyNum)
                    {
                        try
                        {
                            string message = "数字键: " + keyNum + " 已按下";
                            Logger.Debug(message);
                            PringToStatus(message);

                            if (!TradeHandler.Instance.OneKeyMakeOrder(FastOrderCtl.ViewModel, button, KeyOrderViewModel))
                            {
                                Logger.Debug("一键下单未正确响应");
                                //Logger.Dump<AddOrderViewModel>(MyInstance.FastOrderCtl.ViewModel);
                                //Logger.Dump<ButtonSetting>(button);
                                //Logger.Dump<KeyboardOrderViewModel>(MyInstance.KeyOrderViewModel);
                            }
                        }
                        catch (System.NullReferenceException ex)
                        {
                            MessageBox.Show("Order handler hasn't been created yet", "Warning");
                            Logger.Debug(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Warning");
                            Logger.Debug(ex.Message);
                        }
                    }
                }
            }
        }
        #endregion

        private void CreateHandlers(SignInOptions user)
        {
            Config config = new Config(Settings.Default.ConfigFile);

            //MarketData Handler
            MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();

            //Trader Handler
            //TradeHandler.Instance.RegisterMessageWrapper(PBMessageWrapManager.Instance[MsgWrapName.CTP_TRD]);
            TradeHandler.Instance.Initialize(user, config.Content["ORDER"]);

            TradeHandler.Instance.InitAllInfo();

            executionWindow.ExecutionTreeView.ItemsSource = TradeHandler.Instance.ExecutionViewModel;
            ICollectionView view = CollectionViewSource.GetDefaultView(executionWindow.ExecutionTreeView.ItemsSource);
            view.SortDescriptions.Add(
                new SortDescription("BrokerOrderSeq", ListSortDirection.Ascending));

            view.SortDescriptions.Add(
                new SortDescription("IsOrderOrTrade", ListSortDirection.Descending));

            view.SortDescriptions.Add(
                new SortDescription("InsertTime", ListSortDirection.Ascending));

            positionsWindow.PositionListView.ItemsSource = TradeHandler.Instance.Positions;

            FundGrid.DataContext = TradeHandler.Instance.Fund;

            //sync position and fund
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Sync_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();
        }
    }

    public class ArchiveWrapper
    {
        public List<QuoteGroup> Groups { get; set; }
        public KeyboardOrderViewModel KeySetting { get; set; }
    }
}
