﻿using System;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionModelCtrl : UserControl
    {
        public OptionModelCtrl()
        {
            InitializeComponent();
        }

        private void onunderlyingContractCB_SelectionChanged(string Contract)
        {
            var opMarketMakerCtrl = new OpMarketMakerCtrl();
            var volCurvCtrl = new VolCurvCtrl();

        }

    }
}
