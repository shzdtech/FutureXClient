﻿using Micro.Future.CustomizedControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls.Primitives;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ForeignMarketFrame : UserControl, IUserFrame
    {
        public ForeignMarketFrame()
        {
            InitializeComponent();
        }


        public string Title
        {
            get
            {
                return "Frame1";
            }
        }

        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return null;
            }
        }

        public void LoginAsync(string usernname, string password, string server)
        {

        }
    }
}
