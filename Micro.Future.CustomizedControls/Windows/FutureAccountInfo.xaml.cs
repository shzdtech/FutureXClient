﻿using Micro.Future.Message;
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
using System.Windows.Shapes;

namespace Micro.Future.CustomizedControls.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class FutureAccountInfoWindow : Window
    {
        private CollectionViewSource _viewSource = new CollectionViewSource();

        public FutureAccountInfoWindow()
        {
            InitializeComponent();
            FutrueAccountInfoGrid.DataContext = MessageHandlerContainer.DefaultInstance
            .Get<TraderExHandler>().FundVM;
        }
    }
}
