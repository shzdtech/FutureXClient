﻿using Micro.Future.ViewModel;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            ModelParams.Params.Add(new NamedParamVM() { Name = "test", Value = 123 });

            grid.DataContext = ModelParams;
        }

        public ModelParamsVM ModelParams
        {
            get;
        } = new ModelParamsVM();
    }
}