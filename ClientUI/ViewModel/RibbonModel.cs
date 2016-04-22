using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Micro;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using System.Linq;
using Micro.Future.Util;
using Micro.Future.UI;
using Micro.Future.Message;

namespace Micro.Future.ViewModel
{
    public static class RibbonModel
    {
        #region Application Menu

        public static string ResourceAssembly
        {
            get
            {
                return typeof(ViewModelBase).Assembly.FullName;
            }
        }

        public static ControlData ExitTtisv1
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "退出";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        ButtonData buttonData = new ButtonData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Close_16x16.png", UriKind.Relative),
                            Command = ApplicationCommands.Close,
                            KeyTip = "X",
                        };
                        _dataCollection[Str] = buttonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData DomLogin
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "内盘登录";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/VPN.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadDomLogin, DefaultCanExecute),
                            ToolTipDescription = "内盘登录",
                            ToolTipTitle = "系统",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData DomLogout
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "内盘退出";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/DisconnectedDrive.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadDomLogout, DefaultCanExecute),
                            ToolTipDescription = "内盘退出",
                            ToolTipTitle = "系统",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData DeleteQuote
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "删除行情";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/delete.png", UriKind.Relative),
                            Command = new DelegateCommand(DeleteCurrentQuote, DefaultCanExecute),
                            ToolTipDescription = "删除选中的行情报价",
                            ToolTipTitle = "报价",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData DeleteAllQuotes
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "删除全部";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Erase.png", UriKind.Relative),
                            Command = new DelegateCommand(ClearQuotes, DefaultCanExecute),
                            ToolTipDescription = "删除当前报价组中所有的报价",
                            ToolTipTitle = "报价",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData NewGroup
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "新建";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            ToolTipTitle = "报价组",
                            ToolTipDescription = "新建一个自定义报价组",
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/NewDocument_32x32.png", UriKind.Relative),
                            Command = new DelegateCommand(NewQuoteGroup, CheckNewGroupReady),
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData DeleteGroup
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "DeleteGroup";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "删除",
                            ToolTipDescription = "删除当前的报价组",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/Delete_black_32x32.png", UriKind.Relative),
                            Command = new DelegateCommand(DeleteQuoteGroup, DefaultCanExecute),
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FastAddGroup
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "快速添加";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        string ToolTipTitle = "报价组";
                        string ToolTipDescription = "快速添加预定义报价组";

                        ComboBoxData comboBoxData = new ComboBoxData()
                        {
                            ToolTipTitle = ToolTipTitle,
                            ToolTipDescription = ToolTipDescription,
                            KeyTip = "FF",
                        };

                        _dataCollection[Str] = comboBoxData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FontFaceGallery
        {
            get
            {

                if (MainWindow.MyInstance == null)
                {
                    return null;
                }

                lock (_lockObject)
                {
                    string Str = "产品种类";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GalleryData<string> galleryData = new GalleryData<string>();
                        GalleryCategoryData<string> allFontsCategoryData = new GalleryCategoryData<string>()
                        {
                            //Label = "请选在以下种类‘"
                        };

                        foreach (string productClass in MainWindow.MyInstance.GroupMap.Keys)
                        {
                            allFontsCategoryData.GalleryItemDataCollection.Add(productClass);
                        }

                        galleryData.CategoryDataCollection.Add(allFontsCategoryData);

                        Action<string> ChangeFontFace = delegate(string parameter)
                        {
                            MainWindow.MyInstance.AddDefinedGroup(parameter);
                        };

                        Func<string, bool> CanChangeFontFace = delegate(string parameter)
                        {
                            //return MainWindow.MyInstance.CanAddDefinedGroup(parameter);
                            return true;
                        };

                        galleryData.Command = new DelegateCommand<string>(ChangeFontFace, CanChangeFontFace);
                        _dataCollection[Str] = galleryData;
                    }


                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData MarketQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Market";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "市场",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_Blue.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestMarketInfo, DefaultCanExecute),
                            ToolTipDescription = "查询交易所信息",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData InstrumentQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Instrument";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "合约",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_Green.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestInstrumentInfo, DefaultCanExecute),
                            ToolTipDescription = "查询合约信息",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData FundQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Fund";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "资金",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_Purple.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestFundInfo, DefaultCanExecute),
                            ToolTipDescription = "查询资金",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData PositionQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Position";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "持仓",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_red.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestPosition, DefaultCanExecute),
                            ToolTipDescription = "查询持仓",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData OrderQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Order";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "订单",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_yellow.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestOrder, DefaultCanExecute),
                            ToolTipDescription = "查询订单",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData TradeQuery
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Trade";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "交易",
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/1532_Flag_system_Blue.png", UriKind.Relative),
                            Command = new DelegateCommand(RequestTrade, DefaultCanExecute),
                            ToolTipDescription = "查询交易",
                            ToolTipTitle = "查询"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData AdvancedMakeOrder
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "高级下单";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Calculator.png", UriKind.Relative),
                            Command = new DelegateCommand(AdvancedOrder, DefaultCanExecute),
                            ToolTipDescription = "复杂的交易下单功能",
                            ToolTipTitle = "高级下单"
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData SystemSetting
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "系统设置";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Settings.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadSystemSetting, DefaultCanExecute),
                            ToolTipDescription = "系统设置，主题，语言，配色等",
                            ToolTipTitle = "系统设置",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData OTCContactCtrl
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "联系信息";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Contact_48x48.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadOTCContact, DefaultCanExecute),
                            ToolTipDescription = "OTC联系信息",
                            ToolTipTitle = "联系信息",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData ThemeSetting
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "样式设置";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/ThemeFonts.png", UriKind.Relative),
                            Command = new DelegateCommand(DefaultExecuted, DefaultCanExecute),
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData OneButtonOrderSetting
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "一键下单设置";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Keyboard.png", UriKind.Relative),
                            Command = new DelegateCommand(OneButtonOrder, DefaultCanExecute),
                            ToolTipDescription = "设置一键下单数字快捷键等",
                            ToolTipTitle = "一键下单设置",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData PasswordSetting
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "密码设置";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/NewPermission_32x32.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadPasswordChange, DefaultCanExecute),
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData NewUser
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "创建用户";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Shading_16x16.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadCreateUser, CanCreateUser),
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData SaveLayoutCtrl
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "保存布局";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "保存",
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/FloppyDisk.png", UriKind.Relative),
                            Command = new DelegateCommand(SaveUserLayout, DefaultCanExecute),
                            ToolTipDescription = "保存当前的布局",
                            ToolTipTitle = "布局",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData LoadLayoutCtrl
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "恢复布局";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = "恢复",
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/112_RefreshArrow_Blue_32x32_72.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadUserLayout, DefaultCanExecute),
                            ToolTipDescription = "恢复上一次保存的布局",
                            ToolTipTitle = "布局",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        public static ControlData LoadDefaultLayoutCtrl
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "默认";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/1460_PaintPalette_48x48.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadDefaultLayout, DefaultCanExecute),
                            ToolTipDescription = "恢复初始的布局",
                            ToolTipTitle = "布局",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        #endregion Application Menu


        #region Help Button

        public static ControlData Help
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Help";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        ButtonData Data = new ButtonData()
                        {
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Help_16x16.png", UriKind.Relative),
                            ToolTipTitle = "Help (F1)",
                            ToolTipDescription = "Microsoft Ribbon for WPF",
                        };
                        _dataCollection[Str] = Data;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        #endregion
        public static ControlData Clipboard
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "Clipboard";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        GroupData Data = new GroupData(Str)
                        {
                            SmallImage = new Uri("/" + ResourceAssembly + ";component/Images/Paste_16x16.png", UriKind.Relative),
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/Paste_32x32.png", UriKind.Relative),
                            KeyTip = "ZC",
                        };
                        _dataCollection[Str] = Data;
                    }

                    return _dataCollection[Str];
                }
            }
        }

        private static void DefaultExecuted()
        {
        }

        private static bool DefaultCanExecute()
        {
            return true;
        }

        private static bool CheckNewGroupReady()
        {
            if (MainWindow.MyInstance == null)
            {
                return true;
            }
            else
            {
                return MainWindow.MyInstance.NewGroupReady;
            }
        }

        public static void LoadDomLogin()
        {
            MainWindow.MyInstance.LoadDomLogin();
        }

        public static void LoadDomLogout()
        {
            //MainWindow.MyInstance.OrderHandler.LogOut();
        }

        public static void DeleteCurrentQuote()
        {
            try
            {
                var documentPane = MainWindow.MyInstance.dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
                if (documentPane != null)
                {
                    if (documentPane.SelectedContent == null)
                    {
                        MessageBox.Show("无可用报价组", "提示");
                        return;
                    }
                    var view = documentPane.SelectedContent.Content as QuoteGroupView;
                    var list = view.QuoteListView as ListView;
                    var quotes = list.ItemsSource as ObservableCollection<QuoteViewModel>;
                    if (list.SelectedIndex >= 0)
                    {
                        quotes.RemoveAt(list.SelectedIndex);
                    }
                    else
                    {
                        MessageBox.Show("请选择要删除的报价", "提示");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "出错了");
            }
        }

        public static void ClearQuotes()
        {
            try
            {
                var documentPane = MainWindow.MyInstance.dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
                if (documentPane != null)
                {
                    if (documentPane.SelectedContent == null)
                    {
                        MessageBox.Show("无可用报价组", "提示");
                        return;
                    }
                    var view = documentPane.SelectedContent.Content as QuoteGroupView;
                    var list = view.QuoteListView as ListView;
                    var quotes = list.ItemsSource as ObservableCollection<QuoteViewModel>;
                    quotes.Clear();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "出错了");
            }
        }

        public static void RenameQuoteGroup(string newName)
        {
            var documentPane = MainWindow.MyInstance.dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
            if (documentPane != null)
            {
                var doc = documentPane.SelectedContent as LayoutDocument;
                if (MainWindow.MyInstance.GroupMap.ContainsKey(doc.Title))
                {
                    if (!MainWindow.MyInstance.GroupMap[doc.Title].Predefined)
                    {
                        var items = (_dataCollection["产品种类"] as GalleryData<string>).CategoryDataCollection[0].GalleryItemDataCollection;
                        items.Remove(doc.Title);
                        items.Add(newName);
                        QuoteGroup qg = MainWindow.MyInstance.GroupMap[doc.Title];
                        MainWindow.MyInstance.GroupMap.Remove(doc.Title);
                        doc.Title = newName;
                        MainWindow.MyInstance.GroupMap.Add(doc.Title, qg);
                    }
                }
            }
        }

        private static void NewQuoteGroup()
        {
            NewQuoteGroup(MainWindow.MyInstance.newQuoteGroupRibbonText.Text.Trim());
        }

        public static void NewQuoteGroup(string name)
        {
            var firstDocumentPane = MainWindow.MyInstance.dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument();
                string groupname = name;
                doc.Title = groupname;
                firstDocumentPane.Children.Add(doc);
                QuoteGroupView view = new QuoteGroupView();
                doc.Content = view;
                firstDocumentPane.SelectedContentIndex = firstDocumentPane.Children.Count - 1;

                MainWindow.MyInstance.GroupMap.Add(doc.Title, new QuoteGroup() { Name = doc.Title, Predefined = false });

                (_dataCollection["产品种类"] as GalleryData<string>).CategoryDataCollection[0].GalleryItemDataCollection.Add(doc.Title);
            }
        }

        public static void DeleteQuoteGroup()
        {
            var documentPane = MainWindow.MyInstance.dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
            if (documentPane != null)
            {
                var doc = documentPane.SelectedContent as LayoutDocument;
                if (null == doc)
                {
                    MessageBox.Show("请选择报价组");
                    return;
                }
                if (MainWindow.MyInstance.GroupMap.ContainsKey(doc.Title))
                {
                    if (!MainWindow.MyInstance.GroupMap[doc.Title].Predefined)
                    {
                        var items = (_dataCollection["产品种类"] as GalleryData<string>).CategoryDataCollection[0].GalleryItemDataCollection;
                        items.Remove(doc.Title);
                        MainWindow.MyInstance.FastAddGroupCombo.Text = "";
                        MainWindow.MyInstance.GroupMap.Remove(doc.Title);
                    }
                }
                documentPane.Children.RemoveAt(documentPane.SelectedContentIndex);
            }
        }

        public static void SaveUserLayout()
        {
            MainWindow.MyInstance.SaveUserLayout();
        }

        public static void LoadUserLayout()
        {
            MainWindow.MyInstance.LoadUserLayout();
        }

        public static void LoadDefaultLayout()
        {
            MainWindow.MyInstance.LoadDefaultLayout();
        }

        public static void LoadPasswordChange()
        {
            NewUserWindow win = new NewUserWindow();
            win.Title = "更改密码";
            win.UserTxt.IsEnabled = false;
            win.Show();
        }

        public static void LoadSystemSetting()
        {
            SystemSettingWindow win = new SystemSettingWindow();
            win.Show();
        }

        public static void LoadOTCContact()
        {
            OTCContactWindow win = new OTCContactWindow();
            win.ReloadData();
            win.Show();
        }

        public static void LoadCreateUser()
        {
            NewUserWindow win = new NewUserWindow();
            win.Title = "创建用户";
            win.ExistingPassword.IsEnabled = false;
            win.Show();
        }

        public static bool CanCreateUser()
        {
            if (MainWindow.MyInstance == null)
            {
                return true;
            }

            return MainWindow.MyInstance.IsAdmin();

        }

        #region Manual query
        private static void RequestMarketInfo()
        {
            TradeHandler.Instance.RequestMarketInfo();
        }

        private static void RequestInstrumentInfo()
        {
            TradeHandler.Instance.RequestInstrumentInfo();
        }

        private static void RequestFundInfo()
        {
            TradeHandler.Instance.RequestFundInfo();
        }

        private static void RequestPosition()
        {
            TradeHandler.Instance.RequestPositionInfo();
        }

        private static void RequestOrder()
        {
            TradeHandler.Instance.RequestOrders();
        }

        private static void RequestTrade()
        {
            TradeHandler.Instance.RequestTrades();
        }

        private static void AdvancedOrder()
        {
            AdvancedMakeOrderWin win = new AdvancedMakeOrderWin();
            win.Show();
        }

        private static void OneButtonOrder()
        {
            FastOrderSettingWin win = new FastOrderSettingWin();
            win.SetDataContext(MainWindow.MyInstance.KeyOrderViewModel);
            win.Show();
        }

        #endregion

        #region Data

        private const string HelpFooterTitle = "Press F1 for more help.";
        private static object _lockObject = new object();
        private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();

        // Store any data that doesnt inherit from ControlData
        private static Dictionary<string, object> _miscData = new Dictionary<string, object>();

        #endregion Data

    }
}
